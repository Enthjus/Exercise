using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.PlottingServices;

namespace LibraryCad.FileManip.Plot
{
    public class PlotFunc
    {
        public static void PlotModel(Document doc, string fileName)
        {
            Database db = doc.Database;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
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
                plSetVld.SetUseStandardScale(plSettings, true);
                plSetVld.SetStdScaleType(plSettings, StdScaleType.ScaleToFit);

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
    }
}
