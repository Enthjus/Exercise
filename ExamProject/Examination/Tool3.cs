using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;
using LibraryCad.Mathematic;
using LibraryCad.Sub;
using System;

namespace ExamProject.Examination
{
    public class Tool3
    {
        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

        [CommandMethod("myChamfer")]
        public static void myChamfer()
        {
            TypedValue[] filter = new TypedValue[1];
            filter[0] = new TypedValue(0, "Line");
            SelectionFilter sf = new SelectionFilter(filter);
            var res1 = SubFunc.GetListSelection(doc, "\nSelect lines: ", sf);
            if (res1 == null) return;

            var chamferDis = SubFunc.GetString(doc, "\nNhập kích thước cạnh vát:");
            if (!MathFunc.CheckIfNumber(chamferDis)) return;

            foreach (ObjectId id in res1)
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite, false);
                    Line line = new Line();
                    try
                    {
                        line = tr.GetObject(id, OpenMode.ForRead) as Line;
                    }
                    catch
                    {
                        continue;
                    }
                    if (line == null) continue;
                    Point3dCollection p3c = new Point3dCollection();
                    p3c.Add(line.StartPoint);
                    p3c.Add(line.EndPoint);

                    TypedValue[] filterlist = new TypedValue[1];
                    filterlist[0] = new TypedValue(0, "Line");
                    SelectionFilter ft = new SelectionFilter(filterlist);
                    PromptSelectionResult psr;
                    psr = ed.SelectFence(p3c, ft);
                    Line intersectLine = new Line();

                    if (psr.Status == PromptStatus.OK)
                    {
                        Point3dCollection intersectPts = new Point3dCollection();
                        foreach (ObjectId leaderId in psr.Value.GetObjectIds())
                        {
                            Line entity = (Line)tr.GetObject(leaderId, OpenMode.ForRead);
                            if (entity.ObjectId != line.ObjectId)
                            {
                                line.IntersectWith(entity, Intersect.OnBothOperands, intersectPts, 0, 0);
                                if (intersectPts.Count > 0)
                                {
                                    intersectLine = entity;
                                }
                            }
                        }
                        if (intersectPts.Count == 0) continue;
                        if (intersectPts.Count == 1)
                        {
                            var pline = new Polyline();
                            if (intersectPts[0] == line.StartPoint)
                            {
                                if (intersectPts[0] == intersectLine.StartPoint)
                                {
                                    pline.AddVertexAt(0, Extensions.Strip(line.EndPoint), 0, 0, 0);
                                    pline.AddVertexAt(1, Extensions.Strip(intersectPts[0]), 0, 0, 0);
                                    pline.AddVertexAt(2, Extensions.Strip(intersectLine.EndPoint), 0, 0, 0);
                                }
                                else if (intersectPts[0] == intersectLine.EndPoint)
                                {
                                    pline.AddVertexAt(0, Extensions.Strip(line.EndPoint), 0, 0, 0);
                                    pline.AddVertexAt(1, Extensions.Strip(intersectPts[0]), 0, 0, 0);
                                    pline.AddVertexAt(2, Extensions.Strip(intersectLine.StartPoint), 0, 0, 0);
                                }
                            }
                            else if (intersectPts[0] == line.EndPoint)
                            {
                                if (intersectPts[0] == intersectLine.StartPoint)
                                {
                                    pline.AddVertexAt(0, Extensions.Strip(line.StartPoint), 0, 0, 0);
                                    pline.AddVertexAt(1, Extensions.Strip(intersectPts[0]), 0, 0, 0);
                                    pline.AddVertexAt(2, Extensions.Strip(intersectLine.EndPoint), 0, 0, 0);
                                }
                                else if (intersectPts[0] == intersectLine.EndPoint)
                                {
                                    pline.AddVertexAt(0, Extensions.Strip(line.StartPoint), 0, 0, 0);
                                    pline.AddVertexAt(1, Extensions.Strip(intersectPts[0]), 0, 0, 0);
                                    pline.AddVertexAt(2, Extensions.Strip(intersectLine.StartPoint), 0, 0, 0);
                                }
                            }
                            var plId = btr.AppendEntity(pline);
                            tr.AddNewlyCreatedDBObject(pline, true);
                            line.UpgradeOpen();
                            line.Erase();
                            intersectLine.UpgradeOpen();
                            intersectLine.Erase();
                            var dis = Int32.Parse(chamferDis) / MathFunc.squareRoot(2);
                            ed.Command("_.chamfer", "_Polyline", "_Distance", dis, dis, plId, "");
                        }
                    }
                    tr.Commit();
                }
            }
        }

        public static void 
    }
}
