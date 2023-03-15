using System.Collections.Generic;
using System.IO;
using System.Text;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.PlottingServices;
using Autodesk.AutoCAD.Publishing;
using Autodesk.AutoCAD.Runtime;
using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AcadProject.AcadDrawManip.AcadPlotting
{
    public class MultiSheetsPdf
    {
        [CommandMethod("PlotPdf")]
        public void PlotPdf()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            short bgp = (short)AcAp.GetSystemVariable("BACKGROUNDPLOT");
            try
            {
                AcAp.SetSystemVariable("BACKGROUNDPLOT", 0);
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    List<Layout> layouts = new List<Layout>();
                    DBDictionary layoutDict = db.LayoutDictionaryId.GetObject(OpenMode.ForRead) as DBDictionary;
                    foreach (DBDictionaryEntry entry in layoutDict)
                    {
                        if (entry.Key != "Model")
                        {
                            layouts.Add(tr.GetObject(entry.Value, OpenMode.ForRead) as Layout);
                        }
                    }
                    layouts.Sort((l1, l2) => l1.TabOrder.CompareTo(l2.TabOrder));

                    string filename = Path.ChangeExtension(db.Filename, "pdf");

                    AcadMultiSheetsPdf plotter = new AcadMultiSheetsPdf(filename, layouts);
                    plotter.Publish();

                    tr.Commit();
                }
            }
            catch (System.Exception e)
            {
                Editor ed = AcAp.DocumentManager.MdiActiveDocument.Editor;
                ed.WriteMessage("\nError: {0}\n{1}", e.Message, e.StackTrace);
            }
            finally
            {
                AcAp.SetSystemVariable("BACKGROUNDPLOT", bgp);
            }
        }
    }
}

