using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;
using System;
using acadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using System.Linq;
using Autodesk.AutoCAD.Geometry;
using LibraryCad.Sub;
using LibraryCad.Mathematic;
using LibraryCad.ObjectsFunc.PolylineObject;

namespace ExamProject.Examination
{
    public class Tool4
    {
        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

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
                        acadApp.ShowAlertDialog("Lines are colinear or does not intersects");

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
                    acadApp.ShowAlertDialog(ex.Message);
                }
            }//dispose transaction
        }

        [CommandMethod("myFillet")]
        public static void myFillet()
        {
            var filletRad = SubFunc.GetString(doc, "\nNhập bán kính cạnh vát:");
            if (!MathFunc.CheckIfNumber(filletRad)) return;

            while (true)
            {
                TypedValue[] filter = new TypedValue[1];
                filter[0] = new TypedValue(0, "Line");
                SelectionFilter sf = new SelectionFilter(filter);
                var res1 = SubFunc.GetListSelection(doc, "\nSelect lines: ", sf);
                if (res1 == null) return;

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
                                //ed.Command("_.fillet", "_Polyline", "_Radius", filletRad, plId, "");
                                PolylineExtension.FilletAll(pline, Double.Parse(filletRad));
                            }
                        }
                        tr.Commit();
                    }
                }
            }
        }
    }
}
