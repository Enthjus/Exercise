using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;
using LibraryCad.FileManip.Plot;
using LibraryCad.Sub;

namespace AcadProject.AcadDrawManip.AcadPlotting
{
    public class PlotFromModelSpace
    {
        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

        [CommandMethod("PlotCurrentLayout")]
        public static void PlotCurrentLayout()
        {
            using (doc.LockDocument())
            {
                var fileName = SubFunc.GetString(doc, "- Nhập tên file muồn xuất ra: ");
                PlotFunc.PlotModel(doc, fileName);
            }
        }

        [CommandMethod("PlotterLocalMediaNameList")]
        public static void PlotterLocalMediaNameList()
        {
            using (PlotSettings plSettings = new PlotSettings(true))
            {
                PlotSettingsValidator plSetVld = PlotSettingsValidator.Current;
                // Đặt thiết bị vẽ và khổ giấy
                plSetVld.SetPlotConfigurationName(plSettings, "DWF6 ePlot.pc3", "ANSI_A_(8.50_x_11.00_Inches)");
                doc.Editor.WriteMessage("\nCanonical and Local media names: ");
                int cnt = 0;
                foreach (string mediaName in plSetVld.GetCanonicalMediaNameList(plSettings))
                {
                    // Xuất ra tên của media khả dụng cho thiết bị được chỉ định
                    ed.WriteMessage("\n  " + mediaName + " | " + plSetVld.GetLocaleMediaName(plSettings, cnt));
                    cnt = cnt + 1;
                }
            }
        }

        [CommandMethod("PlotterList")]
        public static void PlotterList()
        {
            ed.WriteMessage("\nPlot devices: ");
            foreach (string plotDevice in PlotSettingsValidator.Current.GetPlotDeviceList())
            {
                // Xuất ra tên của thiết bị vẽ khả dụng
                ed.WriteMessage("\n  " + plotDevice);
            }
        }

        [CommandMethod("ChangeLayoutPlotSettings")]
        public static void ChangeLayoutPlotSettings()
        {
            using (doc.LockDocument())
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    // Lấy layout hiện tại
                    LayoutManager acLayoutMgr = LayoutManager.Current;
                    Layout acLayout = trans.GetObject(acLayoutMgr.GetLayoutId(acLayoutMgr.CurrentLayout), OpenMode.ForRead) as Layout;
                    ed.WriteMessage("\nCurrent layout: " + acLayout.LayoutName);
                    ed.WriteMessage("\nCurrent device name: " + acLayout.PlotConfigurationName);

                    // Lấy bản sao của PlotSettings từ layout
                    using (PlotSettings acPlSet = new PlotSettings(acLayout.ModelType))
                    {
                        acPlSet.CopyFrom(acLayout);

                        // Cập nhật thuộc tính PlotConfigurationName của đối tượng PlotSettings
                        PlotSettingsValidator acPlSetVdr = PlotSettingsValidator.Current;
                        acPlSetVdr.SetPlotConfigurationName(acPlSet, "DWG To PDF.pc3", "ANSI_B_(11.00_x_17.00_Inches)");

                        // Thu phóng để hiển thị toàn bộ bài báo
                        acPlSetVdr.SetZoomToPaperOnUpdate(acPlSet, true);

                        // Cập nhật layout
                        trans.GetObject(acLayoutMgr.GetLayoutId(acLayoutMgr.CurrentLayout), OpenMode.ForWrite);
                        acLayout.CopyFrom(acPlSet);
                    }
                    ed.WriteMessage("\nNew device name: " + acLayout.PlotConfigurationName);
                    trans.Commit();
                }
                ed.Regen();
            }
        }
    }
}
