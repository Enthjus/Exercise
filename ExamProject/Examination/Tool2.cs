﻿using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;
using LibraryCad.Sub;
using System.Collections.Generic;

namespace ExamProject.Examination
{
    public class Tool2
    {
        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

        [CommandMethod("myb")]
        public static void MyBreakLine()
        {
            while (true)
            {
                TypedValue[] filter = new TypedValue[1];
                filter[0] = new TypedValue(0, "Line");
                SelectionFilter sf = new SelectionFilter(filter);
                var res1 = SubFunc.GetListSelection(doc, "\nSelect lines: ", sf);
                List<ObjectId> deleteId = new List<ObjectId>();
                if (res1 == null) return;
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
                            foreach (ObjectId leaderId in res1)
                            {
                                Entity entity = (Entity)tr.GetObject(leaderId, OpenMode.ForRead);
                                if (entity.ObjectId != line.ObjectId)
                                {
                                    line.IntersectWith(entity, Intersect.OnBothOperands, intersectPts, 0, 0);
                                }
                            }
                            if (intersectPts.Count == 0) continue;
                            else if (intersectPts.Count == 1)
                            {
                                Line line1 = new Line(intersectPts[0], line.StartPoint);
                                Line line2 = new Line(intersectPts[0], line.EndPoint);
                                if (line1.Length > line2.Length)
                                {
                                    line.UpgradeOpen();
                                    line.EndPoint = intersectPts[0];
                                    line.DowngradeOpen();
                                    tr.Commit();
                                    continue;
                                }
                                else if (line2.Length > line1.Length)
                                {
                                    line.UpgradeOpen();
                                    line.StartPoint = intersectPts[0];
                                    line.DowngradeOpen();
                                    tr.Commit();
                                    continue;
                                 }
                            }
                            else if(intersectPts.Count == 2)
                            {
                                deleteId.Add(id);
                                Line l1;
                                Line l2;
                                if (new Line(line.StartPoint, intersectPts[0]).Length < new Line(line.StartPoint, intersectPts[1]).Length)
                                {
                                    l1 = new Line(line.StartPoint, intersectPts[0]);
                                    l2 = new Line(line.EndPoint, intersectPts[1]);
                                }
                                else
                                {
                                    l1 = new Line(line.StartPoint, intersectPts[1]);
                                    l2 = new Line(line.EndPoint, intersectPts[0]);
                                }
                                btr.AppendEntity(l1);
                                tr.AddNewlyCreatedDBObject(l1, true);
                                btr.AppendEntity(l2);
                                tr.AddNewlyCreatedDBObject(l2, true);
                                //List<double> pars = new List<double>();
                                //foreach (Point3d pt in intersectPts)
                                //{
                                //    pars.Add(line.GetParameterAtPoint(pt));
                                //}
                                //DBObjectCollection objs = line.GetSplitCurves(new DoubleCollection(pars.ToArray()));
                                //foreach (Line ll in objs)
                                //{
                                //    for (int j = 0; j < intersectPts.Count - 1; j++)
                                //    {
                                //        if ((ll.StartPoint != intersectPts[j] && ll.StartPoint != intersectPts[j + 1]) ^ (ll.EndPoint != intersectPts[j] && ll.EndPoint != intersectPts[j + 1]))
                                //        {
                                //            btr.AppendEntity(ll);
                                //            tr.AddNewlyCreatedDBObject(ll, true);
                                //        }
                                //    }
                                //}
                                //line.UpgradeOpen();
                                //line.Erase();
                            }
                        }
                        tr.Commit();
                    }
                }
                if (deleteId.Count <= 0) continue;
                using(Transaction tran = db.TransactionManager.StartTransaction())
                {
                    foreach(var id in deleteId)
                    {
                        var line = tran.GetObject(id, OpenMode.ForWrite) as Line;
                        line.Erase();
                    }
                    tran.Commit();
                }
            }
        }
    }
}
