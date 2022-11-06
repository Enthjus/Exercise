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
            // Get the current document and database
            Document doc = Application.DocumentManager.CurrentDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (doc.LockDocument())
            {
                // Bắt đầu transaction
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        var sumLine = 0.0;

                        // Parse selection set thành list line
                        var lines = LibraryCad.Functions.SelectionSetToListLine(doc);

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
            // Get the current document and database
            Document doc = Application.DocumentManager.CurrentDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (doc.LockDocument())
            {
                // Start a transaction
                using (Transaction acTrans = db.TransactionManager.StartTransaction())
                {
                    // Open the Block table for read
                    BlockTable acBlkTbl;
                    acBlkTbl = acTrans.GetObject(db.BlockTableId,
                                                    OpenMode.ForRead) as BlockTable;

                    // Open the Block table record Model space for write
                    BlockTableRecord acBlkTblRec;
                    acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                    OpenMode.ForWrite) as BlockTableRecord;

                    // Create a line that starts at 5,5 and ends at 12,3
                    using (Line acLine = new Line(new Point3d(5, 5, 0),
                                                  new Point3d(12, 3, 0)))
                    {
                        var angle = acLine.Angle;
                        doc.Editor.WriteMessage("angle: " + angle);
                        doc.Editor.WriteMessage("\nvector: " + acLine.Delta);
                        acLine.EndPoint.RotateBy(angle, new Vector3d(0, 2, 0), acLine.StartPoint);
                        // Add the new object to the block table record and the transaction
                        acBlkTblRec.AppendEntity(acLine);
                        acTrans.AddNewlyCreatedDBObject(acLine, true);
                    }

                    // Save the new object to the database
                    acTrans.Commit();
                }
            }
        }
    }
}
