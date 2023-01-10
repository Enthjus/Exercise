using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;
using LibraryCad.ObjectsFunc.CircleObject;
using LibraryCad.Sub;
using System;

namespace AcadProject.AcadManip.WorkWithObject
{
    public class WorkWithCircle
    {
        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

        [CommandMethod("TriangleWithCircle")]
        public static void TriangleWithCircle()
        {
            using (doc.LockDocument())
            {
                // Chọn các đối tượng chỉ lấy circle
                var circles = CircleFunc.ParseSelectionToListCircle(doc);
                if (circles == null) return;
                // Gọi hàm vẽ đường tròn nội tiếp
                foreach (var circle in circles)
                {
                    if(Init.triangleStatus == 1)
                    {
                        CircleFunc.TriangleInscribedInCircle(doc, circle);
                    }
                    else
                    {
                        CircleFunc.TriangleCircumscribedAboutCircle(doc, circle);
                    }
                }
            }
        }

        [CommandMethod("TriangleInscribedWithCircle")]
        public static void TriangleInscribedWithCircle()
        {
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
            using (doc.LockDocument())
            {
                // Vẽ đường tròn
                CircleFunc.DrawCircle(doc);
            }
        }

        public static Point2d PolarPoint2D(Point2d basepoint, double angle, double distance)
        {
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
}
