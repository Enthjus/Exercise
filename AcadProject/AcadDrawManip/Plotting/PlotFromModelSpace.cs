using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;
using LibraryCad.FileManip.Plot;
using LibraryCad.Sub;

namespace Topic1.AcadDrawManip.Plotting
{
    public class PlotFromModelSpace
    {
        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

        [CommandMethod("PlotCurrentLayout")]
        public static void PlotCurrentLayout()
        {
            var fileName = SubFunc.GetString(doc, "- Nhập tên file muồn xuất ra: ");
            PlotFunc.PlotModel(doc, fileName);
        }

        [CommandMethod("PlotterLocalMediaNameList")]
        public static void PlotterLocalMediaNameList()
        {
            // Get the current document and database, and start a transaction
            Document acDoc = Application.DocumentManager.MdiActiveDocument;

            using (PlotSettings plSet = new PlotSettings(true))
            {
                PlotSettingsValidator acPlSetVdr = PlotSettingsValidator.Current;

                // Set the Plotter and page size
                acPlSetVdr.SetPlotConfigurationName(plSet, "DWF6 ePlot.pc3",
                                                    "ANSI_A_(8.50_x_11.00_Inches)");

                acDoc.Editor.WriteMessage("\nCanonical and Local media names: ");

                int cnt = 0;

                foreach (string mediaName in acPlSetVdr.GetCanonicalMediaNameList(plSet))
                {
                    // Output the names of the available media for the specified device
                    acDoc.Editor.WriteMessage("\n  " + mediaName + " | " +
                                              acPlSetVdr.GetLocaleMediaName(plSet, cnt));

                    cnt = cnt + 1;
                }
            }
        }

        [CommandMethod("PlotterList")]
        public static void PlotterList()
        {
            // Get the current document and database, and start a transaction
            Document acDoc = Application.DocumentManager.MdiActiveDocument;

            acDoc.Editor.WriteMessage("\nPlot devices: ");

            foreach (string plotDevice in PlotSettingsValidator.Current.GetPlotDeviceList())
            {
                // Output the names of the available plotter devices
                acDoc.Editor.WriteMessage("\n  " + plotDevice);
            }
        }

        // Changes the plot settings for a layout directly
        [CommandMethod("ChangeLayoutPlotSettings")]
        public static void ChangeLayoutPlotSettings()
        {
            // Get the current document and database, and start a transaction
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Reference the Layout Manager
                LayoutManager acLayoutMgr = LayoutManager.Current;

                // Get the current layout and output its name in the Command Line window
                Layout acLayout = acTrans.GetObject(acLayoutMgr.GetLayoutId(acLayoutMgr.CurrentLayout),
                                                    OpenMode.ForRead) as Layout;

                // Output the name of the current layout and its device
                acDoc.Editor.WriteMessage("\nCurrent layout: " + acLayout.LayoutName);

                acDoc.Editor.WriteMessage("\nCurrent device name: " + acLayout.PlotConfigurationName);

                // Get a copy of the PlotSettings from the layout
                using (PlotSettings acPlSet = new PlotSettings(acLayout.ModelType))
                {
                    acPlSet.CopyFrom(acLayout);

                    // Update the PlotConfigurationName property of the PlotSettings object
                    PlotSettingsValidator acPlSetVdr = PlotSettingsValidator.Current;
                    acPlSetVdr.SetPlotConfigurationName(acPlSet, "DWG To PDF.pc3", "ANSI_B_(11.00_x_17.00_Inches)");

                    // Zoom to show the whole paper
                    acPlSetVdr.SetZoomToPaperOnUpdate(acPlSet, true);

                    // Update the layout
                    acTrans.GetObject(acLayoutMgr.GetLayoutId(acLayoutMgr.CurrentLayout), OpenMode.ForWrite);
                    acLayout.CopyFrom(acPlSet);
                }

                // Output the name of the new device assigned to the layout
                acDoc.Editor.WriteMessage("\nNew device name: " + acLayout.PlotConfigurationName);

                // Save the new objects to the database
                acTrans.Commit();
            }

            // Update the display
            acDoc.Editor.Regen();
        }
    }
}
