using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace LibraryCad
{
    public class TextFunc
    {
        /// <summary>
        /// Hàm tạo text
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="db">Database Doc</param>
        /// <param name="text">Chuỗi</param>
        /// <param name="point">Điểm</param>
        public static void CreateText(Document doc, string text, Point3d point, ObjectId lyrId = new ObjectId())
        {
            var db = doc.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                try
                {
                    BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord tableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    var layerTable = trans.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                    // Tạo đối tượng chuỗi 1 dòng
                    using (DBText acText = new DBText())
                    {
                        if(lyrId != null)
                        {
                            acText.LayerId = lyrId;
                        }
                        acText.Position = point;
                        acText.Height = 200;
                        acText.TextString = text;
                        tableRec.AppendEntity(acText);
                        trans.AddNewlyCreatedDBObject(acText, true);
                    }
                    trans.Commit();
                }
                catch (System.Exception ex)
                {
                    doc.Editor.WriteMessage(ex.Message);
                    trans.Abort();
                }
            }
        }

        /// <summary>
        /// Hàm gộp các chuỗi được chọn
        /// </summary>
        /// <param name="doc">Document</param>
        /// <returns></returns>
        public static string MergeString(Document doc)
        {
            using (var trans = doc.Database.TransactionManager.StartOpenCloseTransaction())
            {
                PromptSelectionResult selResult = doc.Editor.GetSelection();
                if (selResult.Status == PromptStatus.OK)
                {
                    SelectionSet selSet = selResult.Value;
                    var mergeText = "";
                    foreach (SelectedObject selObj in selSet)
                    {
                        // Kiểm tra xem có đúng kiểu hay không
                        if (selObj != null && trans.GetObject(selObj.ObjectId, OpenMode.ForRead).GetType() == typeof(DBText))
                        {
                            DBText dbText = trans.GetObject(selObj.ObjectId, OpenMode.ForRead) as DBText;
                            if (dbText != null || dbText.TextString.Trim() != "")
                            {
                                // Gom chuỗi
                                mergeText += dbText.TextString + " ";
                            }
                        }
                    }
                    mergeText = mergeText.Trim();
                    return mergeText;
                }
                return "";
            }
        }
    }
}
