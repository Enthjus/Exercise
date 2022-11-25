using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace Topic1
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
                        var lines = LibraryCad.LineFunc.SelectionSetToListLine(doc);
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
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord tableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    // tạo 1 đoạn thẳng bắt đầu ở 5,5 và kết thúc ở 12,3
                    using (Line acLine = new Line(new Point3d(5, 5, 0), new Point3d(12, 3, 0)))
                    {
                        tableRec.AppendEntity(acLine);
                        trans.AddNewlyCreatedDBObject(acLine, true);
                    }
                    trans.Commit();
                }
            }
        }

        [CommandMethod("DimMLine")]
        public static void DimMLine()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            // Lấy list đoạn thẳng từ các đối tượng được chọn
            var lines = LibraryCad.LineFunc.SelectionSetToListLine(doc);
            if (lines.Count == 0) return;
            // Tạo dim trên list đoạn thẳng vừa nhận được
            LibraryCad.DimensionFunc.DimMultiLine(lines, doc);
        }
    }
}
