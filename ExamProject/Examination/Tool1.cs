using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;
using LibraryCad.ObjectsFunc.PolylineObject;
using LibraryCad.Sub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamProject.Examination
{
    public class Tool1
    {
        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

        [CommandMethod("OFIN")]
        public void OffsetInside()
        {
            OffsetPolyline(true);
        }

        [CommandMethod("OFOUT")]
        public void OffsetOutside()
        {
            OffsetPolyline(false);
        }

        private void OffsetPolyline(bool inside)
        {
            PromptEntityOptions peo = new PromptEntityOptions("\nSelect a polyline: ");
            peo.SetRejectMessage("Selected object is not a polyline.");
            peo.AddAllowedClass(typeof(Polyline), true);

            PromptDistanceOptions pdo = new PromptDistanceOptions("\nSpecify the offset distance: ");
            pdo.AllowNegative = false;
            pdo.AllowZero = false;
            PromptDoubleResult pdr = ed.GetDistance(pdo);
            if (pdr.Status != PromptStatus.OK) return;
            double offsetDist = inside ? pdr.Value : -pdr.Value;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                while (true)
                {
                    PromptEntityResult per = ed.GetEntity(peo);
                    if (per.Status != PromptStatus.OK)
                        break;
                    Polyline pline = (Polyline)tr.GetObject(per.ObjectId, OpenMode.ForRead);
                    DBObjectCollection offsets = pline.GetOffsetCurves(pline.GetArea() < 0.0 ? offsetDist : -offsetDist);
                    foreach (DBObject obj in offsets)
                    {
                        Entity ent = (Entity)obj;
                        btr.AppendEntity(ent);
                        tr.AddNewlyCreatedDBObject(ent, true);
                        db.TransactionManager.QueueForGraphicsFlush();
                    }
                }
                tr.Commit();
            }
        }

        [CommandMethod("Test", CommandFlags.Modal)]
        public void Test()
        {
            using (doc.LockDocument())
            {
                PromptEntityOptions peo = new PromptEntityOptions("\nChọn đường tâm: ");
                peo.SetRejectMessage("Only a polyline !");
                peo.AddAllowedClass(typeof(Polyline), true);

                PromptDoubleOptions pdo1 =
                    new PromptDoubleOptions("\nNhập kích thước lòng đường: ");
                pdo1.AllowZero = false;
                PromptDoubleResult pdr1 = ed.GetDouble(pdo1);
                if (pdr1.Status != PromptStatus.OK) return;
                double roadDist = pdr1.Value / 2;

                PromptDoubleOptions pdo2 =
                    new PromptDoubleOptions("\nNhập kích thước hè: ");
                pdo2.AllowZero = false;
                PromptDoubleResult pdr2 = ed.GetDouble(pdo2);
                if (pdr2.Status != PromptStatus.OK) return;
                double roadsideDist = roadDist + pdr2.Value;

                //PromptKeywordOptions pko =
                //    new PromptKeywordOptions("\nEnter the offset side [In/Out/Left/Right/Both]", "In Out Left Right Both");
                //PromptResult pr = ed.GetKeywords(pko);
                //if (pr.Status != PromptStatus.OK) return;
                //PolylineExtension.OffsetSide side;
                //switch (pr.StringResult)
                //{
                //    case "In": side = PolylineExtension.OffsetSide.In; break;
                //    case "Out": side = PolylineExtension.OffsetSide.Out; break;
                //    case "Left": side = PolylineExtension.OffsetSide.Left; break;
                //    case "Right": side = PolylineExtension.OffsetSide.Right; break;
                //    default: side = PolylineExtension.OffsetSide.Both; break;
                //}

                PolylineExtension.OffsetSide side = PolylineExtension.OffsetSide.Both;

                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                    while (true)
                    {
                        PromptEntityResult per = ed.GetEntity(peo);
                        if (per.Status != PromptStatus.OK) break;

                        Polyline pline = (Polyline)tr.GetObject(per.ObjectId, OpenMode.ForRead);

                        // Open the Linetype table for read
                        LinetypeTable acLineTypTbl = tr.GetObject(db.LinetypeTableId,
                                                               OpenMode.ForRead) as LinetypeTable;
                        string sLineTypName = "Continuous";
                        if (acLineTypTbl.Has(sLineTypName) == false)
                        {
                            db.LoadLineTypeFile(sLineTypName, "acad.lin");
                        }

                        foreach (Polyline pl in pline.Offset(roadsideDist, side))
                        {
                            pl.Linetype = sLineTypName; 
                            pl.Layer = "HTKT_GT_Chigioi_ddo";
                            btr.AppendEntity(pl);
                            tr.AddNewlyCreatedDBObject(pl, true);
                        }
                        foreach (Polyline pl in pline.Offset(roadDist, side))
                        {
                            pl.Linetype = sLineTypName;
                            pl.Layer = "GT-DUONG";
                            btr.AppendEntity(pl);
                            tr.AddNewlyCreatedDBObject(pl, true);
                        }
                        db.TransactionManager.QueueForGraphicsFlush();
                    }
                    tr.Commit();
                }
            }
        }

        [CommandMethod("INS")]
        public void InterSectionPoint()
        {
            Line pl1 = null;
            Line pl2 = null;
            Entity ent = null;
            PromptEntityOptions peo = null;
            PromptEntityResult per = null;
            using (Transaction tx = db.TransactionManager.StartTransaction())
            {
                //Select first polyline
                peo = new PromptEntityOptions("Select firtst line:");
                per = ed.GetEntity(peo);
                if (per.Status != PromptStatus.OK)
                {
                    return;
                }
                //Get the polyline entity
                ent = (Entity)tx.GetObject(per.ObjectId, OpenMode.ForRead);
                if (ent is Line)
                {
                    pl1 = ent as Line;
                }
                //Select 2nd polyline
                peo = new PromptEntityOptions("\n Select Second line:");
                per = ed.GetEntity(peo);
                if (per.Status != PromptStatus.OK)
                {
                    return;
                }
                ent = (Entity)tx.GetObject(per.ObjectId, OpenMode.ForRead);
                if (ent is Line)
                {
                    pl2 = ent as Line;
                }
                Point3dCollection pts3D = new Point3dCollection();
                //Get the intersection Points between line 1 and line 2
                pl1.IntersectWith(pl2, Intersect.ExtendBoth, pts3D, IntPtr.Zero, IntPtr.Zero);
                foreach (Point3d pt in pts3D)
                {
                    // ed.WriteMessage("\n intersection point :",pt);
                    ed.WriteMessage("Point number {0}: ({1}, {2}, {3})", pt.X, pt.Y, pt.Z);

                }

                tx.Commit();
            }

        }
    }
}
