using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCad.Sub
{
    public static class Extensions
    {
        /// <summary>
        /// Đảo ngược thuộc tính X và Y của đối tượng Point2d.
        /// </summary>
        /// <param name="flip">Boolean cho biết có đảo ngược hay không.</param>
        /// <returns>Point2d gốc hoặc bản đảo ngược.</returns>
        public static Point2d Swap(this Point2d pt, bool flip = true)
        {
            return flip ? new Point2d(pt.Y, pt.X) : pt;
        }

        /// <summary>
        /// Thêm giá trị Z = 0 vào Point2d để chuyển thành Point3d.
        /// </summary>
        /// <param name="pt">Point2d muốn chuyển.</param>
        /// <returns>Point3d.</returns>
        public static Point3d Pad(this Point2d pt)
        {
            return new Point3d(pt.X, pt.Y, 0);
        }

        /// <summary>
        /// Chuyển 1 Point3d thành 1 Point2d bằng cách bỏ tọa độ Z.
        /// </summary>
        /// <param name="pt">Point3d muốn chuyển.</param>
        /// <returns>Point2d.</returns>
        public static Point2d Strip(this Point3d pt)
        {
            return new Point2d(pt.X, pt.Y);
        }

        /// <summary>
        /// Chọn 1 layout với tên đã chỉ định và cho nó thành hiện hành.
        /// </summary>
        /// <param name="name">Tên của viewport.</param>
        /// <param name="select">Check có chọn hay không.</param>
        /// <returns>ObjectId viewport mới được tạo.</returns>
        public static ObjectId CreateAndMakeLayoutCurrent(this LayoutManager lm, string name, bool select = true)
        {
            // Thử lấy layout 
            var id = lm.GetLayoutId(name);

            // Nếu layout không tồn tại thì tạo mới
            if (!id.IsValid)
            {
                id = lm.CreateLayout(name);
            }

            // Chọn layout
            if (select)
            {
                lm.CurrentLayout = name;
            }
            return id;
        }

        /// <summary>
        /// Áp dụng một hành động cho viewport được chỉ định từ layout này.
        /// Tạo 1 viewport mới nếu không tìm thấy viewport nào từ số truyền vào.
        /// </summary>
        /// <param name="trans">Transaction sử dụng để mở viewports.</param>
        /// <param name="vpNum">Số lượng viewport.</param>
        /// <param name="action">Hành động áp dụng cho từng viewports.</param>
        public static void ApplyToViewport(this Layout layout, Transaction trans, int vpNum, Action<Viewport> action)
        {
            var vpIds = layout.GetViewports();
            Viewport viewport = null;
            foreach (ObjectId vpId in vpIds)
            {
                var viewport2 = trans.GetObject(vpId, OpenMode.ForWrite) as Viewport;
                if (viewport2 != null && viewport2.Number == vpNum)
                {
                    // Tỉm thấy viewport thì gọi action
                    viewport = viewport2;
                    break;
                }
            }
            if (viewport == null)
            {
                // Không tìm thấy viewport thì tạo mới
                var blkTblRec = (BlockTableRecord)trans.GetObject(layout.BlockTableRecordId, OpenMode.ForWrite);
                viewport = new Viewport();

                // Thêm vào database
                blkTblRec.AppendEntity(viewport);
                trans.AddNewlyCreatedDBObject(viewport, true);

                // Mở viewport và grid 
                viewport.On = true;
                viewport.GridOn = true;
            }
            // Gọi chức năng lên viewport
            action(viewport);
        }

        /// <summary>
        /// Áp dụng cài đặt bản vẽ cho layout được cung cấp.
        /// </summary>
        /// <param name="pageSize">Kích thước trang.</param>
        /// <param name="styleSheet">Kiểu giấy của file (ctb or stb).</param>
        /// <param name="device">Tên của thiết bị đầu ra.</param>
        public static void SetPlotSettings(this Layout layout, string pageSize, string styleSheet, string device)
        {
            using (var plSettings = new PlotSettings(layout.ModelType))
            {
                plSettings.CopyFrom(layout);
                var plSetVld = PlotSettingsValidator.Current;

                // Đặt thiết bị
                var devs = plSetVld.GetPlotDeviceList();
                if (devs.Contains(device))
                {
                    plSetVld.SetPlotConfigurationName(plSettings, device, null);
                    plSetVld.RefreshLists(plSettings);
                }

                // Đặt phương tiện tên/kích thước
                var mediaNames = plSetVld.GetCanonicalMediaNameList(plSettings);
                if (mediaNames.Contains(pageSize))
                {
                    plSetVld.SetCanonicalMediaName(plSettings, pageSize);
                }

                // Đặt kiểu giấy
                var stSheetList = plSetVld.GetPlotStyleSheetList();
                if (stSheetList.Contains(styleSheet))
                {
                    plSetVld.SetCurrentStyleSheet(plSettings, styleSheet);
                }

                // Sao chép dữ liệu PlotSettings trở lại Layout
                var upgraded = false;
                if (!layout.IsWriteEnabled)
                {
                    layout.UpgradeOpen();
                    upgraded = true;
                }
                layout.CopyFrom(plSettings);
                if (upgraded)
                {
                    layout.DowngradeOpen();
                }
            }
        }

        /// <summary>
        /// Xác định kích thước tối đa có thể cho layout này.
        /// </summary>
        /// <returns>Phạm vi tối đa của viewport trên layout này.</returns>
        public static Extents2d GetMaximumExtents(this Layout lay)
        {
            // Nếu template của bản vẽ là imperial, chúng ta cần chia cho 1 inch tính bằng mm (25,4)
            var div = lay.PlotPaperUnits == PlotPaperUnit.Inches ? 25.4 : 1.0;

            // Chúng ta cần lật các trục nếu plot được xoay 90 hoặc 270 độ
            var doIt = lay.PlotRotation == PlotRotation.Degrees090 || lay.PlotRotation == PlotRotation.Degrees270;

            // Lấy phạm vi theo đúng đơn vị và hướng
            var min = lay.PlotPaperMargins.MinPoint.Swap(doIt) / div;
            var max = (lay.PlotPaperSize.Swap(doIt) - lay.PlotPaperMargins.MaxPoint.Swap(doIt).GetAsVector()) / div;
            return new Extents2d(min, max);
        }

        /// <summary>
        /// Đặt kích thước của viewport theo phạm vi được cung cấp.
        /// </summary>
        /// <param name="extend">Phạm vi của viewport trên trang.</param>
        /// <param name="factor">Yếu tố tùy chọn để cung cấp padding.</param>
        public static void ResizeViewport(this Viewport viewport, Extents2d extend, double factor = 1.0)
        {
            viewport.Width = (extend.MaxPoint.X - extend.MinPoint.X) * factor;
            viewport.Height = (extend.MaxPoint.Y - extend.MinPoint.Y) * factor;
            viewport.CenterPoint = (Point2d.Origin + (extend.MaxPoint - extend.MinPoint) * 0.5).Pad();
        }

        /// <summary>
        /// Đặt view ở trong viewport để chứa phạm vi model đã chỉ định.
        /// </summary>
        /// <param name="extend">The extents of the content to fit the viewport.</param>
        /// <param name="factor">Optional factor to provide padding.</param>
        public static void FitContentToViewport(this Viewport viewport, Extents3d extend, double factor = 1.0)
        {
            // Phóng to lớn hơn extents
            viewport.ViewCenter = (extend.MinPoint + ((extend.MaxPoint - extend.MinPoint) * 0.5)).Strip();

            // Nhận kích thước của view từ database extents
            var hgt = extend.MaxPoint.Y - extend.MinPoint.Y;
            var wid = extend.MaxPoint.X - extend.MinPoint.X;

            // So sánh với tỷ lệ khung hình của viewport(được lấy từ kích thước trang)
            var aspect = viewport.Width / viewport.Height;

            // Nếu nội dung rộng hơn tỷ lệ khung hình, hãy đảm bảo đặt chiều cao đề xuất lớn hơn để phù hợp với nội dung
            if (wid / hgt > aspect)
            {
                hgt = wid / aspect;
            }

            // Đặt chiều cao chính xác ở extents
            viewport.ViewHeight = hgt;

            // Đặt tỷ lệ tùy chỉnh để thu nhỏ một chút
            viewport.CustomScale *= factor;
        }

        /// <summary>
        /// Kiểm tra xem điểm min và max của extend có hợp lệ hay không
        /// </summary>
        /// <param name="min">Điểm min của extend</param>
        /// <param name="max">Điểm max của extend</param>
        /// <returns></returns>
        public static bool ValidDbExtents(Point3d min, Point3d max)
        {
            return !(min.X > 0 && min.Y > 0 && min.Z > 0 && max.X < 0 && max.Y < 0 && max.Z < 0);
        }

        public static void Add(this ObjectIdCollection col, ObjectId[] ids)
        {
            foreach (var id in ids)
            {
                if (!col.Contains(id)) col.Add(id);
            }
        }
    }
}
