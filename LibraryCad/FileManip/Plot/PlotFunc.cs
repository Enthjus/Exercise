using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.PlottingServices;
using LibraryCad.ObjectsFunc.BlockObject;
using System.IO;

namespace LibraryCad.FileManip.Plot
{
    public class PlotFunc
    {
        #region In model hiện tại
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

        #region In khối được chọn
        /// <summary>
        /// Hàm in khối được chọn
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="Mayin">Thiết bị in</param>
        /// <param name="Khogiay">Khổ giấy</param>
        /// <param name="fileName">Tên file in</param>
        /// <param name="layoutToPlot">Các đối tượng khối được chọn</param>
        public static void xPlot_Khung(Document doc, string Mayin, string Khogiay, string fileName, ObjectIdCollection layoutToPlot)
        {
            // Lấy bản vẽ hiện hành
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            // Bắt đầu transaction
            using (Transaction tr = acDoc.TransactionManager.StartTransaction())
            {
                // Sử dụng Try để phòng lỗi trong quá trình in
                try
                {
                    int numSheet = 1;
                    PlotInfo pi = new PlotInfo();
                    PlotInfoValidator piv = new PlotInfoValidator();
                    piv.MediaMatchingPolicy = MatchingPolicy.MatchEnabled;
                    // Tạo mới bộ máy in để bắt đầu thực hiện in
                    PlotEngine pe = PlotFactory.CreatePublishEngine();
                    using (pe)
                    {
                        PlotProgressDialog ppd = new PlotProgressDialog(false, 1, true);
                        using (ppd)
                        {
                            foreach (ObjectId btrId in layoutToPlot)
                            {
                                //BlockTableRecord btr = tr.GetObject(btrId, OpenMode.ForRead) as BlockTableRecord;
                                BlockReference blkRef = tr.GetObject(btrId, OpenMode.ForWrite) as BlockReference;
                                var minPt = new Point2d(blkRef.GeometricExtents.MinPoint.X, blkRef.GeometricExtents.MinPoint.Y);
                                var maxPt = new Point2d(blkRef.GeometricExtents.MaxPoint.X, blkRef.GeometricExtents.MaxPoint.Y);
                                //Layout lo = tr.GetObject(btr.LayoutId, OpenMode.ForRead) as Layout;
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
                                //PlotInfo pi = new PlotInfo();
                                // Gán thuộc tính Layout là layout hiện tại
                                pi.Layout = loObjId;
                                // Lấy đối tượng xác nhận cấu hình in hiện tại
                                PlotSettingsValidator psv = PlotSettingsValidator.Current;
                                // Xác nhận các thông số máy in và khổ giấy
                                psv.SetPlotConfigurationName(ps, Mayin, Khogiay);
                                // Sử dụng SetPlotWindowArea để khai báo phạm vi in
                                psv.SetPlotWindowArea(ps, new Extents2d(minPt, maxPt));
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
                                LayoutManager.Current.CurrentLayout = lo.LayoutName;
                                // Ghi đề thông tin in
                                pi.OverrideSettings = ps;
                                piv.Validate(pi);
                                try
                                {
                                    if (numSheet == 1)
                                    {
                                        ppd.set_PlotMsgString(PlotMessageIndex.DialogTitle, "Custom Plot Progress");
                                        ppd.set_PlotMsgString(PlotMessageIndex.CancelJobButtonMessage, "Cancel Job");
                                        ppd.set_PlotMsgString(PlotMessageIndex.CancelSheetButtonMessage, "Cancel Sheet");
                                        ppd.set_PlotMsgString(PlotMessageIndex.SheetSetProgressCaption, "Sheet Set Progress");
                                        ppd.set_PlotMsgString(PlotMessageIndex.SheetProgressCaption, "Sheet Progress");
                                        ppd.LowerPlotProgressRange = 0;
                                        ppd.UpperPlotProgressRange = 100;
                                        ppd.PlotProgressPos = 0;
                                        // Let's start the plot, at last
                                        ppd.OnBeginPlot();
                                        ppd.IsVisible = true;
                                        pe.BeginPlot(ppd, null);
                                        // We'll be plotting a single document
                                        pe.BeginDocument(pi, doc.Name, null, 1, true, fileName);
                                    }
                                    ppd.StatusMsgString = "Plotting " + doc.Name.Substring(doc.Name.LastIndexOf("\\") + 1) + " - sheet " + numSheet.ToString() + " of " + layoutToPlot.Count.ToString();
                                    ppd.OnBeginSheet();
                                    ppd.LowerSheetProgressRange = 0;
                                    ppd.UpperSheetProgressRange = 100;
                                    ppd.SheetProgressPos = 0;
                                    //pe.BeginPlot(null, null);
                                    //PlotInfoValidator validator = new PlotInfoValidator();
                                    //validator.MediaMatchingPolicy = MatchingPolicy.MatchEnabled;
                                    //validator.Validate(pi);
                                    //pe.BeginDocument(pi, "", null, 1, true, fileName);
                                    PlotPageInfo pageInfo = new PlotPageInfo();
                                    pe.BeginPage(pageInfo, pi, (numSheet == layoutToPlot.Count), null);
                                    pe.BeginGenerateGraphics(null);
                                    ppd.SheetProgressPos = 50;
                                    pe.EndGenerateGraphics(null);
                                    pe.EndPage(null);
                                    ppd.OnEndSheet();
                                    numSheet++;
                                }
                                catch (System.Exception ex)
                                {
                                    acDoc.Editor.WriteMessage(ex.Message);
                                }
                                //pe.Destroy();
                            }
                            pe.EndDocument(null);
                            ppd.OnEndPlot();
                            pe.EndPlot(null);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    acDoc.Editor.WriteMessage(ex.Message);
                }
            }
        }
        #endregion

        public static void PlotMSheet(Document doc, string Mayin, string Khogiay, string fileName, ObjectIdCollection layoutToPlot)
        {
            Editor ed = doc.Editor;
            Database db = doc.Database;
            Transaction tr = db.TransactionManager.StartTransaction();
            using (tr)
            {
                //BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                PlotInfo pi = new PlotInfo();
                PlotInfoValidator piv = new PlotInfoValidator();
                piv.MediaMatchingPolicy = MatchingPolicy.MatchEnabled;

                // A PlotEngine does the actual plotting
                // (can also create one for Preview)
                if (PlotFactory.ProcessPlotState == ProcessPlotState.NotPlotting)
                {
                    PlotEngine pe = PlotFactory.CreatePublishEngine();
                    using (pe)
                    {
                        // Create a Progress Dialog to provide info
                        // and allow thej user to cancel
                        PlotProgressDialog ppd = new PlotProgressDialog(false, 1, true);
                        using (ppd)
                        {
                            ObjectIdCollection layoutsToPlot = layoutToPlot;
                            //foreach (ObjectId btrId in bt)
                            //{
                            //    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                            //    if (btr.IsLayout && btr.Name.ToUpper() != BlockTableRecord.ModelSpace.ToUpper())
                            //    {
                            //        layoutsToPlot.Add(btrId);
                            //    }
                            //}
                            int numSheet = 1;
                            foreach (ObjectId btrId in layoutsToPlot)
                            {
                                BlockReference blkRef = tr.GetObject(btrId, OpenMode.ForRead) as BlockReference;
                                var minPt = new Point2d(blkRef.GeometricExtents.MinPoint.X, blkRef.GeometricExtents.MinPoint.Y);
                                var maxPt = new Point2d(blkRef.GeometricExtents.MaxPoint.X, blkRef.GeometricExtents.MaxPoint.Y);
                                LayoutManager layMgr = LayoutManager.Current;
                                ObjectId loObjId = layMgr.GetLayoutId(layMgr.CurrentLayout);
                                Layout lo = (Layout)tr.GetObject(loObjId, OpenMode.ForRead);

                                // We need a PlotSettings object
                                // based on the layout settings
                                // which we then customize
                                PlotSettings ps = new PlotSettings(lo.ModelType);
                                ps.CopyFrom(lo);

                                // The PlotSettingsValidator helps
                                // create a valid PlotSettings object
                                PlotSettingsValidator psv = PlotSettingsValidator.Current;

                                // We'll plot the extents, centered and
                                // scaled to fit
                                psv.SetPlotWindowArea(ps, new Extents2d(minPt, maxPt));
                                psv.SetPlotType(ps, Autodesk.AutoCAD.DatabaseServices.PlotType.Extents);
                                //psv.SetUseStandardScale(ps, true);
                                psv.SetStdScaleType(ps, StdScaleType.ScaleToFit);
                                psv.SetPlotCentered(ps, true);

                                // We'll use the standard DWFx PC3, as
                                // this supports multiple sheets
                                psv.SetPlotConfigurationName(ps, Mayin, Khogiay);

                                // We need a PlotInfo object
                                // linked to the layout
                                pi.Layout = loObjId;


                                ps.ScaleLineweights = false;
                                // Xác định tùy chọn bóng mờ cho khung in
                                ps.ShadePlot = PlotSettingsShadePlotType.AsDisplayed;
                                ps.ShadePlotResLevel = ShadePlotResLevel.Normal;
                                // Khai báo các tùy chọn in khác
                                ps.PrintLineweights = true;
                                ps.PlotPlotStyles = true;
                                ps.DrawViewportsFirst = true;
                                // Đặt thuộc tính in khổ giấy ngang Landscape
                                psv.RefreshLists(ps);
                                psv.SetPlotRotation(ps, PlotRotation.Degrees090);

                                // Đặt bảng nét in theo tên
                                //psv.SetCurrentStyleSheet(ps, "monochrome");
                                // Định hệ đơn vị in
                                psv.SetPlotPaperUnits(ps, PlotPaperUnit.Millimeters);

                                // Make the layout we're plotting current
                                LayoutManager.Current.CurrentLayout = lo.LayoutName;

                                // We need to link the PlotInfo to the
                                // PlotSettings and then validate it
                                pi.OverrideSettings = ps;
                                piv.Validate(pi);
                                if (numSheet == 1)
                                {
                                    ppd.set_PlotMsgString(PlotMessageIndex.DialogTitle, "Custom Plot Progress");
                                    ppd.set_PlotMsgString(PlotMessageIndex.CancelJobButtonMessage, "Cancel Job");
                                    ppd.set_PlotMsgString(PlotMessageIndex.CancelSheetButtonMessage, "Cancel Sheet");
                                    ppd.set_PlotMsgString(PlotMessageIndex.SheetSetProgressCaption, "Sheet Set Progress");
                                    ppd.set_PlotMsgString(PlotMessageIndex.SheetProgressCaption, "Sheet Progress");
                                    ppd.LowerPlotProgressRange = 0;
                                    ppd.UpperPlotProgressRange = 100;
                                    ppd.PlotProgressPos = 0;

                                    // Let's start the plot, at last
                                    ppd.OnBeginPlot();
                                    ppd.IsVisible = true;
                                    pe.BeginPlot(ppd, null);

                                    // We'll be plotting a single document
                                    pe.BeginDocument(pi, doc.Name, null, 1, true, fileName);
                                }
                                // Which may contain multiple sheets
                                ppd.StatusMsgString = "Plotting " + doc.Name.Substring(doc.Name.LastIndexOf("\\") + 1) + " - sheet " + numSheet.ToString() + " of " + layoutsToPlot.Count.ToString();
                                ppd.OnBeginSheet();
                                ppd.LowerSheetProgressRange = 0;
                                ppd.UpperSheetProgressRange = 100;
                                ppd.SheetProgressPos = 0;
                                PlotPageInfo ppi = new PlotPageInfo();
                                pe.BeginPage(ppi, pi, (numSheet == layoutsToPlot.Count), null);
                                pe.BeginGenerateGraphics(null);
                                ppd.SheetProgressPos = 50;
                                pe.EndGenerateGraphics(null);

                                // Finish the sheet
                                pe.EndPage(null);
                                ppd.SheetProgressPos = 100;
                                ppd.OnEndSheet();
                                numSheet++;
                            }
                            // Finish the document
                            pe.EndDocument(null);

                            // And finish the plot
                            ppd.PlotProgressPos = 100;
                            ppd.OnEndPlot();
                            pe.EndPlot(null);
                        }
                    }
                }
                else
                {
                    ed.WriteMessage("\nAnother plot is in progress.");
                }
            }
        }
    }
}
