using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;
using LibraryCad.Mathematic;
using LibraryCad.Models;
using LibraryCad.ObjectsFunc.DimensionObject;
using LibraryCad.ObjectsFunc.LineObject;
using LibraryCad.Sub;
using System;
using System.Collections.Generic;

namespace AcadProject.AcadManip.WorkWithObject
{
    public class WorkWithLine
    {
        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

        [CommandMethod("SumMultiLine")]
        public static void SumMultiLine()
        {
            Document doc = Application.DocumentManager.CurrentDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            {
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        var sumLine = 0.0;
                        // Parse các đối tượng được chọn thành list đoạn thẳng
                        var lines = LineFunc.SelectionSetToListLine(doc);
                        // Cộng độ dài các đoạn thẳng
                        if (lines != null)
                        {
                            foreach (var line in lines)
                            {
                                sumLine += line.Length;
                            }
                        }
                        // In ra editor
                        doc.Editor.WriteMessage("Tổng độ dài các đoạn thẳng = " + sumLine);
                    }
                    catch (System.Exception ex)
                    {
                        doc.Editor.WriteMessage(ex.Message);
                        trans.Abort();
                    }
                }
            }
        }

        [CommandMethod("AddLine")]
        public static void AddLine()
        {
            Document doc = Application.DocumentManager.CurrentDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            {
                var pt1 = SubFunc.PickPoint(doc);
                var pt2 = SubFunc.PickPoint(doc);
                if (pt1 == null || pt2 == null) return;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord tableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    // tạo 1 đoạn thẳng bắt đầu ở 5,5 và kết thúc ở 12,3
                    using (Line acLine = new Line(pt1.point, pt2.point))
                    {
                        tableRec.AppendEntity(acLine);
                        trans.AddNewlyCreatedDBObject(acLine, true);
                    }
                    trans.Commit();
                }
            }
        }

        [CommandMethod("ExtendObject")]
        public static void ExtendObject()
        {
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            using (acDoc.LockDocument())
            {
                // Start a transaction
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    // Open the Block table for read
                    BlockTable acBlkTbl;
                    acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                                       OpenMode.ForRead) as BlockTable;
                    // Open the Block table record Model space for write
                    BlockTableRecord acBlkTblRec;
                    acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                          OpenMode.ForWrite) as BlockTableRecord;
                    // Create a line that starts at (4,4,0) and ends at (7,7,0)
                    Line acLine = new Line(new Point3d(4, 4, 0),
                                                 new Point3d(7, 7, 0));
                    acLine.SetDatabaseDefaults();
                    // Add the new object to the block table record and the transaction
                    acBlkTblRec.AppendEntity(acLine);
                    acTrans.AddNewlyCreatedDBObject(acLine, true);
                    // Update the display and diaplay a message box
                    acDoc.Editor.Regen();
                    Application.ShowAlertDialog("Before extend");
                    // Double the length of the line
                    acLine.EndPoint = acLine.EndPoint + acLine.Delta;
                    // Save the new object to the database
                    acTrans.Commit();
                }
            }
        }

        [CommandMethod("IntersectTest")]
        static public void IntersectTest()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            Transaction tr = doc.TransactionManager.StartTransaction();
            using (tr)
            {
                PromptEntityOptions peo = new PromptEntityOptions("Select a line: ");
                PromptEntityResult per = ed.GetEntity(peo);
                if (per.Status == Autodesk.AutoCAD.EditorInput.PromptStatus.OK)
                {
                    Line line = (Line)tr.GetObject(per.ObjectId, OpenMode.ForRead);

                    Point3dCollection p3c = new Point3dCollection();
                    p3c.Add(line.StartPoint);
                    p3c.Add(line.EndPoint);

                    TypedValue[] filterlist = new TypedValue[1];
                    filterlist[0] = new TypedValue(0, "Line");
                    SelectionFilter filter = new SelectionFilter(filterlist);
                    Autodesk.AutoCAD.EditorInput.PromptSelectionResult psr;
                    psr = ed.SelectFence(p3c, filter);

                    if (psr.Status == Autodesk.AutoCAD.EditorInput.PromptStatus.OK)
                    {
                        Point3dCollection intersectPts = new Point3dCollection();
                        foreach (ObjectId leaderId in psr.Value.GetObjectIds())
                        {
                            Entity entity = (Entity)tr.GetObject(leaderId, OpenMode.ForRead);
                            if (entity.ObjectId != line.ObjectId)
                            {
                                line.IntersectWith(entity, Intersect.OnBothOperands, intersectPts, 0, 0);
                            }
                        }
                    }
                }
            }
        }

        [CommandMethod("myb")]
        public static void MyBreakLine()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            TypedValue[] filter = new TypedValue[1];
            filter[0] = new TypedValue(0, "Line");
            SelectionFilter sf = new SelectionFilter(filter);
            var res1 = SubFunc.GetListSelection(doc, "\nSelect lines: ", sf);
            if (res1 == null) return;

            //PromptPointOptions opt2 = new PromptPointOptions("\nselect second point:");
            //opt2.AllowNone = true;
            //PromptPointResult res2 = ed.GetPoint(opt2);
            //PromptPointOptions opt3 = new PromptPointOptions("\nselect second point:");
            //opt3.AllowNone = true;
            //PromptPointResult res3 = ed.GetPoint(opt3);
            foreach (ObjectId id in res1)
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite, false);
                    Line line = tr.GetObject(id, OpenMode.ForRead) as Line;
                    Point3dCollection p3c = new Point3dCollection();
                    p3c.Add(line.StartPoint);
                    p3c.Add(line.EndPoint);

                    TypedValue[] filterlist = new TypedValue[1];
                    filterlist[0] = new TypedValue(0, "Line");
                    SelectionFilter ft = new SelectionFilter(filterlist);
                    PromptSelectionResult psr;
                    psr = ed.SelectFence(p3c, ft);

                    if (psr.Status == PromptStatus.OK)
                    {
                        Point3dCollection intersectPts = new Point3dCollection();
                        foreach (ObjectId leaderId in psr.Value.GetObjectIds())
                        {
                            Entity entity = (Entity)tr.GetObject(leaderId, OpenMode.ForRead);
                            if (entity.ObjectId != line.ObjectId)
                            {
                                line.IntersectWith(entity, Intersect.OnBothOperands, intersectPts, 0, 0);
                            }
                        }
                        if (intersectPts.Count == 0) continue;
                        if (intersectPts.Count == 1)
                        {
                            Line line1 = new Line(intersectPts[0], line.StartPoint);
                            Line line2 = new Line(intersectPts[0], line.EndPoint);
                            if(line1.Length > line2.Length)
                            {
                                line.UpgradeOpen();
                                line.EndPoint = intersectPts[0];
                                line.DowngradeOpen();
                                tr.Commit();
                                continue;
                            }
                            else if(line2.Length > line1.Length)
                            {
                                line.UpgradeOpen();
                                line.StartPoint = intersectPts[0];
                                line.DowngradeOpen();
                                tr.Commit();
                                continue;
                            }
                        }
                        List<double> pars = new List<double>();
                        foreach (Point3d pt in intersectPts)
                        {
                            pars.Add(line.GetParameterAtPoint(pt));
                        }
                        DBObjectCollection objs = line.GetSplitCurves(new DoubleCollection(pars.ToArray()));
                        foreach (Line ll in objs)
                        {
                            for (int j = 0; j < intersectPts.Count - 1; j++)
                            {
                                if ((ll.StartPoint != intersectPts[j] && ll.StartPoint != intersectPts[j + 1]) ^ (ll.EndPoint != intersectPts[j] && ll.EndPoint != intersectPts[j + 1]))
                                {
                                    btr.AppendEntity(ll);
                                    tr.AddNewlyCreatedDBObject(ll, true);
                                }
                            }
                        }
                        line.UpgradeOpen();
                        line.Erase();
                    }
                    tr.Commit();
                }
                //Line l = (Line)tr.GetObject(res1.ObjectId, OpenMode.ForRead);
                //List<double> pars = new List<double>();
                ////Point3d pt1 = l.GetClosestPointTo(res1.PickedPoint, false);
                //Point3d pt1 = new Point3d();
                //Point3d pt2 = new Point3d();
                //pt1 = l.GetClosestPointTo(res3.Value, false);
                //pars.Add(l.GetParameterAtPoint(pt1));
                //BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite, false);
                //if (res2.Status == PromptStatus.OK)
                //{
                //    pt2 = l.GetClosestPointTo(res2.Value, false);
                //    pars.Add(l.GetParameterAtPoint(pt2));
                //    pars.Sort();
                //    DBObjectCollection objs = l.GetSplitCurves(new DoubleCollection(pars.ToArray()));
                //    foreach (Line ll in objs)
                //    {
                //        if ((ll.StartPoint != pt1 && ll.StartPoint != pt2) ^ (ll.EndPoint != pt1 && ll.EndPoint != pt2))
                //        {
                //            btr.AppendEntity(ll);
                //            tr.AddNewlyCreatedDBObject(ll, true);
                //        }
                //    }
                //}
                //else
                //{
                //    DBObjectCollection objs = l.GetSplitCurves(new DoubleCollection(pars.ToArray()));
                //    foreach (Line ll in objs)
                //    {
                //        btr.AppendEntity(ll);
                //        tr.AddNewlyCreatedDBObject(ll, true);
                //    }
                //}
                //l.UpgradeOpen();
                //l.Erase();
            }
        }

        [CommandMethod("FLT", CommandFlags.UsePickSet & CommandFlags.Redraw)]

        static public void FilletLines()
        {
            Transaction tr = db.TransactionManager.StartTransaction();

            using (tr)
            {
                try
                {
                    // Prompt for the fillet radius

                    PromptDoubleOptions pdo = new PromptDoubleOptions("\nEnter the fillet radius: ");
                    pdo.AllowZero = false;

                    pdo.AllowNegative = false;

                    pdo.AllowNone = false;

                    PromptDoubleResult pdr = ed.GetDouble(pdo);

                    if (pdr.Status != PromptStatus.OK)

                        return;

                    double rad = pdr.Value;

                    // Prompt for the lines to fillet

                    PromptEntityOptions peo = new PromptEntityOptions("\nSelect first line:");

                    PromptEntityResult per = ed.GetEntity(peo);

                    if (per.Status != PromptStatus.OK)

                        return;

                    ObjectId fid = per.ObjectId;

                    peo.Message = "\nSelect second line:";

                    per = ed.GetEntity(peo);

                    if (per.Status != PromptStatus.OK)

                        return;

                    ObjectId sid = per.ObjectId;

                    // get entities

                    Entity ent1 = tr.GetObject(fid, OpenMode.ForRead) as Entity;

                    Entity ent2 = tr.GetObject(sid, OpenMode.ForRead) as Entity;
                    // cast entites as lines
                    Line line1 = ent1 as Line;

                    Line line2 = ent2 as Line;

                    Point3dCollection intpts = new Point3dCollection();
                    // get intersection between lines
                    line1.IntersectWith(line2, Intersect.OnBothOperands, intpts, 0, 0);

                    if (intpts.Count != 1)
                    {
                        Application.ShowAlertDialog("Lines are colinear or does not intersects");

                        return;
                    }

                    Point3d ip = intpts[0];

                    Point3d midp1;

                    Point3d midp2;
                    // compare points
                    if (!(line1.StartPoint.Equals(ip)))
                    {
                        line1.UpgradeOpen();

                        line1.EndPoint = line1.StartPoint;

                        line1.StartPoint = ip;
                    }
                    midp1 = line1.GetPointAtDist(rad);

                    if (!(line2.StartPoint.Equals(ip)))
                    {
                        line2.UpgradeOpen();

                        line2.EndPoint = line2.StartPoint;

                        line2.StartPoint = ip;
                    }
                    midp2 = line2.GetPointAtDist(rad);
                    // get point on bisector
                    Point3d midp = new Point3d(
                    (midp1.X + midp2.X) / 2.0,
                    (midp1.Y + midp2.Y) / 2.0,
                    (midp1.Z + midp2.Z) / 2.0);
                    // get angles along lines from intersection
                    double ang1 = SubFunc.AngleFromX(ip, midp1);

                    double ang2 = SubFunc.AngleFromX(ip, midp2);
                    // get bisector angle
                    double ang = SubFunc.AngleFromX(ip, midp);
                    // calculate angle between lines
                    double angc = Math.Abs(ang2 - ang1);
                    // get a half of them
                    double bis = angc / 2.0;
                    // calculate hypotenuse
                    double hyp = rad / Math.Sin(bis);
                    // calculate center point of filleting arc
                    Point3d cp = SubFunc.PolarPoint(ip, ang, hyp);
                    // calculate another leg of a triangle
                    double cat = Math.Sqrt((Math.Pow(hyp, 2)) - (Math.Pow(rad, 2)));
                    // calculate center point on arc
                    Point3d pa = SubFunc.PolarPoint(ip, ang, hyp - rad);
                    // calculate start point of arc
                    Point3d ps = SubFunc.PolarPoint(ip, ang1, cat);
                    // calculate end point of arc
                    Point3d pe = SubFunc.PolarPoint(ip, ang2, cat);
                    // define arc
                    Arc arc = new Arc();
                    // check on direction of points
                    if (SubFunc.isLeft(midp2, ip, midp1))
                    {
                        arc = new Arc(cp, rad, SubFunc.AngleFromX(cp, pe), SubFunc.AngleFromX(cp, ps));

                    }
                    else
                    {
                        arc = new Arc(cp, rad, SubFunc.AngleFromX(cp, ps), SubFunc.AngleFromX(cp, pe));

                    }
                    // trim lines by arc
                    line1.UpgradeOpen();

                    line1.StartPoint = ps;

                    line2.UpgradeOpen();

                    line2.StartPoint = pe;

                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                    // add arc to space
                    btr.AppendEntity(arc);

                    tr.AddNewlyCreatedDBObject(arc, true);

                    tr.Commit();
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    Application.ShowAlertDialog(ex.Message);
                }
            }//dispose transaction
        }

        [CommandMethod("filletlines", CommandFlags.UsePickSet | CommandFlags.Redraw)]
        public static void FL()
        {
            Transaction tr = db.TransactionManager.StartTransaction();

            using (tr)
            {
                try
                {
                    // Prompt for the fillet radius

                    PromptDoubleOptions pdo = new PromptDoubleOptions("\nEnter the fillet radius: ");
                    pdo.AllowZero = false;

                    pdo.AllowNegative = false;

                    pdo.AllowNone = false;

                    PromptDoubleResult pdr = ed.GetDouble(pdo);

                    if (pdr.Status != PromptStatus.OK)

                        return;

                    double rad = pdr.Value;

                    // Prompt for the lines to fillet

                    PromptEntityOptions peo = new PromptEntityOptions("\nSelect first line:");
                    peo.SetRejectMessage("\nSelect lines only");

                    peo.AddAllowedClass(typeof(Line), true);

                    PromptEntityResult per = ed.GetEntity(peo);

                    if (per.Status != PromptStatus.OK)

                        return;

                    ObjectId fid = per.ObjectId;

                    peo.Message = "\nSelect second line:";

                    per = ed.GetEntity(peo);

                    if (per.Status != PromptStatus.OK)

                        return;

                    ObjectId sid = per.ObjectId;

                    ObjectId[] ids = new ObjectId[2];

                    ids[0] = fid;

                    ids[1] = sid;

                    DBObject obj1 = tr.GetObject(fid, OpenMode.ForWrite);

                    DBObject obj2 = tr.GetObject(sid, OpenMode.ForWrite);

                    Application.SetSystemVariable("FILLETRAD", rad);

                    Application.SetSystemVariable("CMDECHO", 0);

                    ResultBuffer buf = new ResultBuffer();

                    buf.Add(new TypedValue(5005, "_FILLET"));

                    buf.Add(new TypedValue(5006, fid));

                    buf.Add(new TypedValue(5006, sid));

                    ed.Command(buf.UnmanagedObject);

                    buf.Dispose();

                    Application.SetSystemVariable("CMDECHO", 1);

                    tr.Commit();
                }

                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    ed.WriteMessage(ex.Message + "\n" + ex.StackTrace);
                }
            }
        }

        [CommandMethod("testJoinLines", "tjl", CommandFlags.Modal | CommandFlags.UsePickSet)]//OK
        public void TestConnectedLines()
        {
            PromptEntityOptions pso = new PromptEntityOptions("\nPick a single line to join: ");

            pso.SetRejectMessage("\nObject must be of type Line!");

            pso.AddAllowedClass(typeof(Line), false);

            PromptEntityResult res = ed.GetEntity(pso);

            if (res.Status != PromptStatus.OK) return;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForRead) as BlockTableRecord;

                List<ObjectId> ids = JoinLines(btr, res.ObjectId);

                ObjectId[] lines = ids.ToArray();

                ed.SetImpliedSelection(lines);
                //or
                //ed.SetImpliedSelection(ids.OfType<ObjectId>().ToArray());
                PromptSelectionResult chres = ed.SelectImplied();

                if (chres.Status != PromptStatus.OK)
                {
                    ed.WriteMessage("\nNothing added in the chain!");

                    return;
                }

                Polyline3d polyline = new Polyline3d();

                Entity[] entities = new Entity[lines.Length];
                int i = 0;

                foreach (SelectedObject selobj in chres.Value)
                {
                    Entity subent = tr.GetObject(selobj.ObjectId, OpenMode.ForWrite) as Entity;
                    polyline.JoinEntity(subent);
                }


                tr.Commit();
            }

        }

        private void SelectConnectedLines(BlockTableRecord btr, List<ObjectId> ids, ObjectId id)
        {
            Entity en = id.GetObject(OpenMode.ForRead, false) as Entity;

            Line ln = en as Line;

            if (ln != null)

                foreach (ObjectId idx in btr)
                {
                    Entity ex = idx.GetObject(OpenMode.ForRead, false) as Entity;

                    Line lx = ex as Line;

                    if (((ln.StartPoint == lx.StartPoint) || (ln.StartPoint == lx.EndPoint)) ||

                        ((ln.EndPoint == lx.StartPoint) || (ln.EndPoint == lx.EndPoint)))

                        if (!ids.Contains(idx))
                        {
                            ids.Add(idx);

                            SelectConnectedLines(btr, ids, idx);
                        }
                }
        }

        public List<ObjectId> JoinLines(BlockTableRecord btr, ObjectId id)
        {
            List<ObjectId> ids = new List<ObjectId>();

            SelectConnectedLines(btr, ids, id);

            return ids;
        }
    }
}
