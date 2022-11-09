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
                var circles = LibraryCad.CircleFunc.ParseSelectionToListCircle(doc);
                foreach (var circle in circles)
                {
                    LibraryCad.CircleFunc.TriangleInscribedInCircle(doc, circle);
                }
            }
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
                var circles = LibraryCad.CircleFunc.ParseSelectionToListCircle(doc);
                if (circles == null) return;
                var topP = circles[0].Center + new Line(new Point3d(circles[0].Center.X, circles[0].Center.Y, 0), new Point3d(circles[0].Center.X, circles[0].Center.Y + circles[0].Radius, 0)).Delta * 2;
                ed.WriteMessage("point: " + topP.ToString());
                foreach (var circle in circles)
                {
                    LibraryCad.CircleFunc.TriangleCircumscribedAboutCircle(doc, circle);
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
                LibraryCad.CircleFunc.DrawCircle(doc);
            }
        }
    }
}
