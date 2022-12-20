using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.ObjectsFunc.CircleObject;
using LibraryCad.Sub;
using System;

namespace AcadProject.AcadManip.WorkWithObject
{
    public class WorkWithCircle
    {
        [CommandMethod("TriangleInscribedWithCircle")]
        public static void TriangleInscribedWithCircle()
        {
            Document doc = Application.DocumentManager.CurrentDocument;
            using (doc.LockDocument())
            {
                // Chọn các đối tượng chỉ lấy circle
                var circles = CircleFunc.ParseSelectionToListCircle(doc);
                if (circles == null) return;
                // Gọi hàm vẽ đường tròn nội tiếp
                foreach (var circle in circles)
                {
                    CircleFunc.TriangleInscribedInCircle(doc, circle);
                }
            }
        }

        [CommandMethod("TriangleCircumscribedAboutCircle")]
        public static void TriangleCircumscribedAboutCircle()
        {
            Document doc = Application.DocumentManager.CurrentDocument;
            using (doc.LockDocument())
            {
                // Chọn các đối tượng chỉ lấy circle
                var circles = CircleFunc.ParseSelectionToListCircle(doc);
                if (circles == null) return;
                // Gọi hàm vẽ đường tròn ngoại tiếp
                foreach (var circle in circles)
                {
                    CircleFunc.TriangleCircumscribedAboutCircle(doc, circle);
                }
            }
        }

        [CommandMethod("DrawCircle")]
        public static void DrawCircle()
        {
            Document doc = Application.DocumentManager.CurrentDocument;
            using (doc.LockDocument())
            {
                // Vẽ đường tròn
                CircleFunc.DrawCircle(doc);
            }
        }

        public static Point2d PolarPoint2D(Point2d basepoint, double angle, double distance)
        {
            var a = Math.Cos(angle);
            var b = Math.Sin(angle);
            return new Point2d(basepoint.X + (distance * Math.Cos(angle)), basepoint.Y + (distance * Math.Sin(angle)));
        }

        [CommandMethod("DrawPolygon")]
        public static void DrawPolygon()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            var point = SubFunc.PickPoint(doc);
            if (point != null)
            {
                double seg = (2 * Math.PI / 3.0);
                double ang = Math.PI * 90.0 / 180.0;
                Point2d bpt = new Point2d(point.point.X, point.point.Y);

                using (Transaction transaction = doc.TransactionManager.StartTransaction())
                {
                    Polyline pl = new Polyline();

                    for (int x = 0; x < 3; x++)
                    {
                        Point2d p = PolarPoint2D(bpt, ang, 1.0);
                        pl.AddVertexAt(x, p, 0.0, 0.0, 0.0);
                        ang = ang + seg;
                    }
                    pl.Closed = true;

                    BlockTable table = (BlockTable)transaction.GetObject(db.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord record = (BlockTableRecord)transaction.GetObject(table[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    record.AppendEntity(pl);
                    transaction.AddNewlyCreatedDBObject(pl, true);
                    transaction.Commit();
                }
            }
        }
    }

    public class SlopeCommand
    {
        class SlopeJig : EntityJig
        {
            Polyline m_pline;
            Point3d m_dragPt;
            Point3d m_fixPt;

            public SlopeJig(Polyline pline, Point3d dragPt, Point3d fixPt) : base(pline)
            {
                m_dragPt = dragPt;
                m_fixPt = fixPt;
                m_pline = pline;
            }
            protected override bool Update()
            {
                Point2d pt1 = new Point2d(m_dragPt.X, m_fixPt.Y);
                Point2d pt2 = new Point2d(m_dragPt.X, m_dragPt.Y);
                m_pline.SetPointAt(1, pt1);
                m_pline.SetPointAt(2, pt2);
                return true;
            }

            protected override SamplerStatus Sampler(JigPrompts prompts)
            {
                JigPromptPointOptions jppo = new JigPromptPointOptions("\nSpecify second point: ");
                jppo.UserInputControls = (UserInputControls.Accept3dCoordinates);
                PromptPointResult ppr = prompts.AcquirePoint(jppo);
                if (ppr.Status == PromptStatus.OK)
                {
                    if (ppr.Value.IsEqualTo(m_dragPt))
                        return SamplerStatus.NoChange;
                    else
                    {
                        m_dragPt = ppr.Value;
                        return SamplerStatus.OK;
                    }
                }
                return SamplerStatus.Cancel;
            }
        }

        [CommandMethod("SLOPE")]
        public void test()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            PromptPointResult ppr = ed.GetPoint("\nSpecify first point: ");
            if (ppr.Status == PromptStatus.OK)
            {
                Matrix3d UCS = ed.CurrentUserCoordinateSystem;
                Point3d fixPt = ppr.Value.TransformBy(UCS);
                Point2d pt = new Point2d(fixPt.X, fixPt.Y);
                Polyline pline = new Polyline(3);
                pline.Closed = true;
                pline.AddVertexAt(0, pt, 0.0, 0.0, 0.0);
                pline.AddVertexAt(0, pt, 0.0, 0.0, 0.0);
                pline.AddVertexAt(0, pt, 0.0, 0.0, 0.0);

                SlopeJig jig = new SlopeJig(pline, fixPt, fixPt);
                PromptResult res = ed.Drag(jig);
                if (res.Status == PromptStatus.OK)
                {
                    Point3d p0 = pline.GetPoint3dAt(0);
                    Point3d p1 = pline.GetPoint3dAt(1);
                    Point3d p2 = pline.GetPoint3dAt(2);
                    double slope = Math.Abs((p2.Y - p0.Y) / (p2.X - p0.X));
                    Line hLine = new Line(p0, p1);
                    Line vLine = new Line(p1, p2);
                    Line sLine = new Line(p0, p2);
                    DBText txt = new DBText();
                    txt.Position = p0;
                    txt.HorizontalMode = TextHorizontalMode.TextCenter;
                    txt.VerticalMode = TextVerticalMode.TextBottom;
                    txt.AlignmentPoint = p0 + (p2 - p0).DivideBy(2.0);
                    txt.TextString = String.Format("{0:p}", slope);
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                        btr.AppendEntity(hLine);
                        tr.AddNewlyCreatedDBObject(hLine, true);
                        btr.AppendEntity(vLine);
                        tr.AddNewlyCreatedDBObject(vLine, true);
                        btr.AppendEntity(sLine);
                        tr.AddNewlyCreatedDBObject(sLine, true);
                        btr.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                        tr.Commit();
                    }
                }
                else
                    pline.Dispose();
            }
        }
    }
}
