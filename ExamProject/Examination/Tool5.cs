using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;
using LibraryCad.Sub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamProject.Examination
{
    public class Tool5
    {
        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

        [CommandMethod("NameRoad")]
        public static void NameRoad()
        {
            PromptDoubleOptions pdo = new PromptDoubleOptions("\nNhập chiều cao chữ:");
            pdo.AllowZero = false;
            PromptDoubleResult pdr = ed.GetDouble(pdo);
            if (pdr.Status != PromptStatus.OK) return;
            var textHeight = pdr.Value;
            var roadName = SubFunc.GetString(doc, "\nNhập tên đường:");
            if (roadName == "") return;
            while (true)
            {
                PromptPointOptions options1 = new PromptPointOptions("\nChọn điểm đầu: ");
                PromptPointResult result1 = doc.Editor.GetPoint(options1);
                if (result1.Status == PromptStatus.Cancel) return;
                var pt1 = result1.Value;
                PromptPointOptions options2 = new PromptPointOptions("\nChọn điểm cuối: ");
                PromptPointResult result2 = doc.Editor.GetPoint(options2);
                if (result2.Status == PromptStatus.Cancel) return;
                var pt2 = result2.Value;
                var line = new Line(pt1, pt2);
                var normal = ed.CurrentUserCoordinateSystem.CoordinateSystem3d.Zaxis;
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    var space = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                    var ang = (line.EndPoint - line.StartPoint).GetNormal();
                    ang = ang.GetNormal() * (textHeight / 3);
                    ang = ang.TransformBy(Matrix3d.Rotation(Math.PI / 2.0, line.Normal, Point3d.Origin));
                    var text = new DBText();
                    text.TextString = roadName;
                    text.Height = textHeight;
                    text.Rotation = SubFunc.GetRotation(line.StartPoint.GetVectorTo(line.EndPoint), normal);
                    text.Normal = normal;
                    text.Position = line.StartPoint + ang;
                    space.AppendEntity(text);
                    tr.AddNewlyCreatedDBObject(text, true);
                    tr.Commit();
                }
            }
        }
    }
}
