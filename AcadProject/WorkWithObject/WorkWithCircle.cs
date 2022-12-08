using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.ObjectsFunc.CircleObject;

namespace Topic1.WorkWithObject
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
    }
}
