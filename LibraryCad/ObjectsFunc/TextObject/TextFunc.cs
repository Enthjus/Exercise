using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryCad.ObjectsFunc.TextObject
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
        public static void CreateText(Document doc, string text, Point3d point, ObjectId lyrId)
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
                    using (DBText dbText = new DBText())
                    {
                        if (lyrId != null)
                        {
                            dbText.LayerId = lyrId;
                        }
                        dbText.Position = point;
                        dbText.Height = 1;
                        dbText.TextString = text;
                        tableRec.AppendEntity(dbText);
                        trans.AddNewlyCreatedDBObject(dbText, true);
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

        public static List<DBText> SortText(List<DBText> dbTexts)
        {
            try
            {
                var texts = new List<DBText>();
                texts = dbTexts.OrderBy(a => Int32.Parse(a.TextString)).ToList();
                return texts;
            }
            catch
            {
                return null;
            }
        }


    }
}
