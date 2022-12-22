using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.PlottingServices;
using LibraryCad.ObjectsFunc.BlockObject;
using System.IO;

namespace LibraryCad.FileManip.Plot
{
    public class PlotFunc
    {
        #region Plot Current Model
        /// <summary>
        /// Hàm in layer hiện tại thành file pdf
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="fileName">Tên file</param>
        public static void PlotModel(Document doc, string fileName)
        {
            Database db = doc.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                var blk = BlockFunc.PickBlock(doc, "- Chọn block:");
                // Khởi tạo lớp LayoutManager   
                LayoutManager layoutMng = LayoutManager.Current;

                // Lấy layout hiện tại
                Layout layout = trans.GetObject(layoutMng.GetLayoutId(layoutMng.CurrentLayout), OpenMode.ForRead) as Layout;

                // Lấy thông tin in ấn từ layout
                PlotInfo plotInfo = new PlotInfo();
                plotInfo.Layout = layout.ObjectId;

                // Lấy bản copy của PlotSettings từ layout
                PlotSettings plSettings = new PlotSettings(layout.ModelType);
                plSettings.CopyFrom(layout);

                // Lấy PlotSettingsValidator hiện tại
                PlotSettingsValidator plSetVld = PlotSettingsValidator.Current;

                // Đặt kiểu bản in
                plSetVld.SetPlotType(plSettings, Autodesk.AutoCAD.DatabaseServices.PlotType.Extents);

                // Đặt tỉ lệ cho bản in
                //plSetVld.SetUseStandardScale(plSettings, true);
                plSetVld.SetStdScaleType(plSettings, StdScaleType.ScaleToFit);
                plSetVld.SetPlotWindowArea(plSettings, new Extents2d(new Point2d(blk.GeometricExtents.MinPoint.X, blk.GeometricExtents.MinPoint.Y), new Point2d(blk.GeometricExtents.MaxPoint.X, blk.GeometricExtents.MaxPoint.Y)));

                // Căn giữa bản in
                plSetVld.SetPlotCentered(plSettings, true);

                // Cài thiết bị in 
                plSetVld.SetPlotConfigurationName(plSettings, "DWG To PDF.pc3", "ANSI_A_(8.50_x_11.00_Inches)");

                // Đặt thông tin bản in làm thông tin ghi đè vì thông tin này sẽ không được lưu lại vào layout
                plotInfo.OverrideSettings = plSettings;

                // Xác thực thông tin bản in
                PlotInfoValidator plInfVld = new PlotInfoValidator();
                plInfVld.MediaMatchingPolicy = MatchingPolicy.MatchEnabled;
                plInfVld.Validate(plotInfo);

                // Kiểm tra xem bản in đã được tiến hành hay chưa
                if (PlotFactory.ProcessPlotState == ProcessPlotState.NotPlotting)
                {
                    using (PlotEngine plotEngine = PlotFactory.CreatePublishEngine())
                    {
                        // Theo dõi tiến trình bản in bằng Progress dialog
                        PlotProgressDialog plProgDialog = new PlotProgressDialog(false, 1, true);

                        using (plProgDialog)
                        {
                            // Xác định các thông báo trạng thái sẽ hiển thị khi bắt đầu bản in
                            plProgDialog.set_PlotMsgString(PlotMessageIndex.DialogTitle, "Plot Progress");

                            plProgDialog.set_PlotMsgString(PlotMessageIndex.CancelJobButtonMessage, "Cancel Job");

                            plProgDialog.set_PlotMsgString(PlotMessageIndex.CancelSheetButtonMessage, "Cancel Sheet");

                            plProgDialog.set_PlotMsgString(PlotMessageIndex.SheetSetProgressCaption, "Sheet Set Progress");

                            plProgDialog.set_PlotMsgString(PlotMessageIndex.SheetProgressCaption, "Sheet Progress");

                            // Đặt phạm vi tiến độ bản in
                            plProgDialog.LowerPlotProgressRange = 0;
                            plProgDialog.UpperPlotProgressRange = 100;
                            plProgDialog.PlotProgressPos = 0;

                            // Hiển thị hộp thoại tiến độ
                            plProgDialog.OnBeginPlot();
                            plProgDialog.IsVisible = true;

                            // Bắt đầu vẽ layout
                            plotEngine.BeginPlot(plProgDialog, null);

                            // Xác định đầu ra bản vẽ
                            plotEngine.BeginDocument(plotInfo, doc.Name, null, 1, true, "D:\\MyPlot\\" + fileName);

                            // Hiển thị thông tin về bản vẽ hiện tại
                            plProgDialog.set_PlotMsgString(PlotMessageIndex.Status, "Plotting: " + doc.Name + " - " + layout.LayoutName);

                            // Đặt phạm vi tiến độ của sheet
                            plProgDialog.OnBeginSheet();
                            plProgDialog.LowerSheetProgressRange = 0;
                            plProgDialog.UpperSheetProgressRange = 100;
                            plProgDialog.SheetProgressPos = 0;

                            // Vẽ sheet/layout đẩu tiên
                            PlotPageInfo plPageInfo = new PlotPageInfo();
                            plotEngine.BeginPage(plPageInfo, plotInfo, true, null);

                            plotEngine.BeginGenerateGraphics(null);
                            plotEngine.EndGenerateGraphics(null);

                            // Hoàn tất vẽ sheet/layout
                            plotEngine.EndPage(null);
                            plProgDialog.SheetProgressPos = 100;
                            plProgDialog.OnEndSheet();

                            // Hoàn tất vẽ document
                            plotEngine.EndDocument(null);

                            // Hoàn tất bản vẽ
                            plProgDialog.PlotProgressPos = 100;
                            plProgDialog.OnEndPlot();
                            plotEngine.EndPlot(null);
                        }
                    }
                }
            }
        }
        #endregion

        public static void xPlot_Khung(Point2d Window1, Point2d Window2, string Mayin, string Khogiay, string fileName)
        {
            // Lấy bản vẽ hiện hành
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            // Lấy cơ sở dữ liệu của bản vẽ
            Database acCurDb = acDoc.Database;
            // Bắt đầu transaction
            using (Transaction tr = acDoc.TransactionManager.StartTransaction())
            {
                // Sử dụng Try để phòng lỗi trong quá trình in
                try
                {
                    // Lấy ra Layout hiện hành của bản vẽ
                    LayoutManager layMgr = LayoutManager.Current;
                    // Lấy ObjectId của Layout hiện hành
                    ObjectId loObjId = layMgr.GetLayoutId(layMgr.CurrentLayout);
                    // Mở layout để đọc thông tin
                    Layout lo = (Layout)tr.GetObject(loObjId, OpenMode.ForRead);
                    // Lấy đối tượng thiết lập thông tin in ấn
                    PlotSettings ps = new PlotSettings(lo.ModelType);
                    // Sao chép các khai báo in ấn từ layout hiện tại
                    ps.CopyFrom(lo);
                    // Khai báo mới một đối tượng thông tin in
                    PlotInfo pi = new PlotInfo();
                    // Gán thuộc tính Layout là layout hiện tại
                    pi.Layout = loObjId;
                    // Lấy đối tượng xác nhận cấu hình in hiện tại
                    PlotSettingsValidator psv =
                    Autodesk.AutoCAD.DatabaseServices.PlotSettingsValidator.Current;
                    // Xác nhận các thông số máy in và khổ giấy
                    psv.SetPlotConfigurationName(ps, Mayin, Khogiay);
                    // Sử dụng SetPlotWindowArea để khai báo phạm vi in
                    psv.SetPlotWindowArea(ps, new Extents2d(Window1, Window2));
                    // Khai báo kiểu in Window
                    psv.SetPlotType(ps, Autodesk.AutoCAD.DatabaseServices.PlotType.Window);
                // Thiết lập CenterPlot
 psv.SetPlotCentered(ps, true);
                    // Đặt tỷ lệ in Fit to Page
                    psv.SetStdScaleType(ps, StdScaleType.ScaleToFit);
                    // Khai báo không scale độ mảnh nét in theo tỷ lệ in
                    ps.ScaleLineweights = false;

                    // Tái tạo lại máy in, kiểu nét in và danh sách các phương tiện in 
                    // (Phải gọi trước khi đặt các thiết lập in)
                    psv.RefreshLists(ps);
                    // Xác định tùy chọn bóng mờ cho khung in
                    ps.ShadePlot = PlotSettingsShadePlotType.AsDisplayed;
                    ps.ShadePlotResLevel = ShadePlotResLevel.Normal;
                    // Khai báo các tùy chọn in khác
                    ps.PrintLineweights = true;
                    ps.PlotPlotStyles = true;
                    ps.DrawViewportsFirst = true;
                    // Đặt thuộc tính in khổ giấy ngang Landscape
                    psv.SetPlotRotation(ps, PlotRotation.Degrees090);

                    // Đặt bảng nét in theo tên
                    //psv.SetCurrentStyleSheet(ps, "monochrome");
                    // Định hệ đơn vị in
                    psv.SetPlotPaperUnits(ps, PlotPaperUnit.Millimeters);
                    // Ghi đề thông tin in
                    pi.OverrideSettings = ps;
                    // Tạo mới bộ máy in để bắt đầu thực hiện in
                    PlotEngine pe = PlotFactory.CreatePublishEngine();
                    try
                    {
                        pe.BeginPlot(null, null);
                        PlotInfoValidator validator = new PlotInfoValidator();
                        validator.MediaMatchingPolicy =
                        MatchingPolicy.MatchEnabled;
                        validator.Validate(pi);
                        pe.BeginDocument(pi, "", null, 1, true, fileName);
                        PlotPageInfo pageInfo = new PlotPageInfo();
                        pe.BeginPage(pageInfo, pi, true, null);
                        pe.BeginGenerateGraphics(null);
                        pe.EndGenerateGraphics(null);
                        pe.EndPage(null);
                        pe.EndDocument(null);
                        pe.EndPlot(null);
                    }
                    catch (System.Exception ex)
                    {
                        acDoc.Editor.WriteMessage(ex.Message);
                    }
                    pe.Destroy();
                }
                catch (System.Exception ex)
                {
                    acDoc.Editor.WriteMessage(ex.Message);
                }
                tr.Commit();
            }
        }

    }
}
