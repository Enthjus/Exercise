using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.ObjectsFunc.DimensionObject;
using LibraryCad.ObjectsFunc.LineObject;
using LibraryCad.Sub;

namespace AcadProject.AcadManip.WorkWithObject
{
    public class WorkWithLine
    {
        [CommandMethod("SumMultiLine")]
        public static void SumMultiLine()
        {
            Document doc = Application.DocumentManager.CurrentDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            {
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        var sumLine = 0.0;
                        // Parse các đối tượng được chọn thành list đoạn thẳng
                        var lines = LineFunc.SelectionSetToListLine(doc);
                        // Cộng độ dài các đoạn thẳng
                        if (lines != null)
                        {
                            foreach (var line in lines)
                            {
                                sumLine += line.Length;
                            }
                        }
                        // In ra editor
                        doc.Editor.WriteMessage("Tổng độ dài các đoạn thẳng = " + sumLine);
                    }
                    catch (System.Exception ex)
                    {
                        doc.Editor.WriteMessage(ex.Message);
                        trans.Abort();
                    }
                }
            }
        }

        [CommandMethod("AddLine")]
        public static void AddLine()
        {
            Document doc = Application.DocumentManager.CurrentDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            {
                var pt1 = SubFunc.PickPoint(doc);
                var pt2 = SubFunc.PickPoint(doc);
                if (pt1 == null || pt2 == null) return;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord tableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    // tạo 1 đoạn thẳng bắt đầu ở 5,5 và kết thúc ở 12,3
                    using (Line acLine = new Line(pt1.point, pt2.point))
                    {
                        tableRec.AppendEntity(acLine);
                        trans.AddNewlyCreatedDBObject(acLine, true);
                    }
                    trans.Commit();
                }
            }
        }
    }
}
