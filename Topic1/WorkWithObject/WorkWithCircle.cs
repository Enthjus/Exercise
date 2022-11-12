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

            using (doc.LockDocument())
            {
                // Chọn các đối tượng chỉ lấy circle
                var circles = LibraryCad.CircleFunc.ParseSelectionToListCircle(doc);

                // Vẽ đường tròn nội tiếp
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
            Editor ed = doc.Editor;

            using (doc.LockDocument())
            {
                // Chọn các đối tượng chỉ lấy circle
                var circles = LibraryCad.CircleFunc.ParseSelectionToListCircle(doc);
                if (circles == null) return;

                // Vẽ đường tròn ngoại tiếp
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

            using (doc.LockDocument())
            {
                // Vẽ đường tròn
                LibraryCad.CircleFunc.DrawCircle(doc);
            }
        }
    }
}
