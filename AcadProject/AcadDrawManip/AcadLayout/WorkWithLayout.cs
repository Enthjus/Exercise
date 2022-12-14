using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;
using LibraryCad.ObjectsFunc.BlockObject;
using LibraryCad.Sub;

namespace AcadProject.AcadDrawManip.AcadLayout
{
    public class WorkWithLayout
    {
        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

        [CommandMethod("LayoutList")]
        public static void LayoutList()
        {
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DBDictionary layouts = trans.GetObject(db.LayoutDictionaryId, OpenMode.ForRead) as DBDictionary;
                ed.WriteMessage("\nLayouts: ");
                foreach (var layout in layouts)
                {
                    ed.WriteMessage("\n " + layout.Key);
                }
            }
        }

        [CommandMethod("CL")]
        public void CreateLayout()
        {
            if (doc == null) return;
            var ext = new Extents2d();
            using (doc.LockDocument())
            {
                using (var tr = db.TransactionManager.StartTransaction())
                {
                    var blk = BlockFunc.PickBlock(doc);
                    var id = LayoutManager.Current.CreateAndMakeLayoutCurrent("ABC");
                    var lay = (Layout)tr.GetObject(id, OpenMode.ForWrite);
                    lay.SetPlotSettings("ANSI_B_(11.00_x_17.00_Inches)", "monochrome.ctb", "DWF6 ePlot.pc3");
                    ext = new Extents2d(new Point2d(blk.GeometricExtents.MinPoint.X, blk.GeometricExtents.MinPoint.Y), new Point2d(blk.GeometricExtents.MaxPoint.X, blk.GeometricExtents.MaxPoint.Y));
                    //ext = lay.GetMaximumExtents();
                    lay.ApplyToViewport(tr, 2, vp =>
                    {
                        vp.ResizeViewport(ext, 0.8);
                        if (Extensions.ValidDbExtents(db.Extmin, db.Extmax))
                        {
                            vp.FitContentToViewport(new Extents3d(new Point3d(blk.GeometricExtents.MinPoint.X, blk.GeometricExtents.MinPoint.Y,
                                blk.GeometricExtents.MinPoint.Z), new Point3d(blk.GeometricExtents.MaxPoint.X, blk.GeometricExtents.MaxPoint.Y,
                                blk.GeometricExtents.MaxPoint.Z)), 0.9);
                            //vp.FitContentToViewport(new Extents3d(db.Extmin, db.Extmax), 0.9);
                        }
                        vp.Locked = true;
                    });
                    tr.Commit();
                }
                ed.Command("_.ZOOM", "_E");
                ed.Command("_.ZOOM", ".7X");
                ed.Regen();
            }
        }
    }
}
