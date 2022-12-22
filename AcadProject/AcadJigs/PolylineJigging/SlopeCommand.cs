using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;
using System;

namespace AcadProject.AcadJigs.PolylineJigging
{
    public class SlopeCommand
    {
        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

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
