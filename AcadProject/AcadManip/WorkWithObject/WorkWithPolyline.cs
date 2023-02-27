using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;
using LibraryCad.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace AcadProject.AcadManip.WorkWithObject
{
    public class WorkWithPolyline
    {
        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

        [CommandMethod("loadjson")]
        public void LoadJson()
        {
            using (StreamReader r = new StreamReader(@"D:\c#\ATC\AddinLearning\appsetting.json"))
            {
                string json = r.ReadToEnd();
                List<AppSetting> items = JsonConvert.DeserializeObject<List<AppSetting>>(json);
                foreach (AppSetting item in items)
                {
                    doc.Editor.WriteMessage("\nData: " + item.Data + "\nStatus: " + item.Status);
                }
            }
        }

        [CommandMethod("cf")]
        public void chamfer()
        {
            if (doc == null) return;
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var ms = tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForWrite) as BlockTableRecord;
                if (ms != null)
                {
                    var pl = new Polyline();
                    pl.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
                    pl.AddVertexAt(1, new Point2d(10, 0), 0, 0, 0);
                    pl.AddVertexAt(2, new Point2d(10, 10), 0, 0, 0);
                    pl.AddVertexAt(3, new Point2d(0, 10), 0, 0, 0);
                    pl.Closed = true;
                    // Add it to the drawing
                    var id = ms.AppendEntity(pl);
                    tr.AddNewlyCreatedDBObject(pl, true);
                    //ed.Command("_.REVCLOUD", "", ss, "");
                    ed.Command("_.chamfer", "_Polyline", "_Distance", 1.0, 1.0, id, "");
                }
                tr.Commit();
            }
        }
    }
}
