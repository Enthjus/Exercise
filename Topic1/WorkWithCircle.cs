using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace Topic1
{
    public class WorkWithCircle
    {
        [CommandMethod("TriangleInscribedWithCircle")]
        public static void TriangleInscribedWithCircle()
        {
            // Get the current document and database
            Document doc = Application.DocumentManager.CurrentDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (doc.LockDocument())
            {
                var circles = LibraryCad.Functions.ParseSelectionToListCircle(doc);
                foreach (var circle in circles)
                {
                    LibraryCad.Functions.TriangleInscribedInCircle(doc, circle);
                }
            }

            // Vẽ tam giác đều nội tiếp đường tròn
            //using (var line = new Line(new Point3d(circle.Center.X - (circleRad * LibraryCad.Functions.squareRoot(3) / 2), circle.Center.Y - (circleRad / 2), 0), new Point3d(circle.Center.X + (circleRad * LibraryCad.Functions.squareRoot(3) / 2), circle.Center.Y - (circleRad / 2), 0)))
            //{
            //    var circleObj = trans.GetObject(circle.Id, OpenMode.ForRead) as Entity;
            //    btr.AppendEntity(line);
            //    trans.AddNewlyCreatedDBObject(line, true);
            //    Point3dCollection pts = new Point3dCollection();
            //    line.IntersectWith(circleObj, Intersect.OnBothOperands, pts, IntPtr.Zero, IntPtr.Zero);
            //    foreach (Point3d pt in pts)
            //    {
            //        using (var ln = new Line(new Point3d(centerPt.X, centerPt.Y + circleRad, 0), pt))
            //        {
            //            btr.AppendEntity(ln);
            //            trans.AddNewlyCreatedDBObject(ln, true);
            //        }
            //    }
            //}

            //// Vẽ tam giác đều ngoại tiếp đường tròn
            //var leftPoint = new Point3d(circle.Center.X - (circleRad * LibraryCad.Functions.squareRoot(3)), circle.Center.Y - circleRad, 0);
            //var rightPoint = new Point3d(circle.Center.X + (circleRad * LibraryCad.Functions.squareRoot(3)), circle.Center.Y - circleRad, 0);
            //var topPoint = new Point3d(circle.Center.X, circle.Center.Y + (circleRad * 2), 0);
            //Point3dCollection pointCol = new Point3dCollection(new Point3d[3] { leftPoint, rightPoint, topPoint });
            //for (int i = 0; i < pointCol.Count - 1; i++)
            //{
            //    for (int j = 1; j < pointCol.Count; j++)
            //    {
            //        var triangleEdge = new Line(pointCol[i], pointCol[j]);
            //        btr.AppendEntity(triangleEdge);
            //        trans.AddNewlyCreatedDBObject(triangleEdge, true);
            //    }
            //}

            //trans.Commit();
        }

        [CommandMethod("TriangleCircumscribedAboutCircle")]
        public static void TriangleCircumscribedAboutCircle()
        {
            // Get the current document and database
            Document doc = Application.DocumentManager.CurrentDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (doc.LockDocument())
            {
                var circles = LibraryCad.Functions.ParseSelectionToListCircle(doc);
                if (circles == null) return;
                var topP = circles[0].Center + new Line(new Point3d(circles[0].Center.X, circles[0].Center.Y, 0), new Point3d(circles[0].Center.X, circles[0].Center.Y + circles[0].Radius, 0)).Delta * 2;
                ed.WriteMessage("point: " + topP.ToString());
                foreach (var circle in circles)
                {
                    LibraryCad.Functions.TriangleCircumscribedAboutCircle(doc, circle);
                }
            }
        }

        [CommandMethod("DrawCircle")]
        public static void DrawCircle()
        {
            // Get the current document and database
            Document doc = Application.DocumentManager.CurrentDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (doc.LockDocument())
            {
                LibraryCad.Functions.DrawCircle(doc);
            }
        }
    }
}
