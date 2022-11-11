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
                    // Open the Block table for read
                    BlockTable acBlkTbl;
                    acBlkTbl = trans.GetObject(db.BlockTableId,
                                                    OpenMode.ForRead) as BlockTable;

                    // Open the Block table record Model space for write
                    BlockTableRecord acBlkTblRec;
                    acBlkTblRec = trans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                    OpenMode.ForWrite) as BlockTableRecord;

                    var lyrTbl = trans.GetObject(db.LayerTableId,
                                        OpenMode.ForRead) as LayerTable;


                    // Create a single-line text object
                    using (DBText acText = new DBText())
                    {
                        if(lyrId != null)
                        {
                            acText.LayerId = lyrId;
                        }
                        acText.Position = point;
                        acText.Height = 200;
                        acText.TextString = text;

                        acBlkTblRec.AppendEntity(acText);
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
                // Request for objects to be selected in the drawing area
                PromptSelectionResult acSSPrompt = doc.Editor.GetSelection();

                // If the prompt status is OK, objects were selected
                if (acSSPrompt.Status == PromptStatus.OK)
                {
                    SelectionSet acSSet = acSSPrompt.Value;

                    var mergeText = "";

                    // Step through the objects in the selection set
                    foreach (SelectedObject acSSObj in acSSet)
                    {
                        // Check to make sure a valid SelectedObject object was returned
                        if (acSSObj != null && trans.GetObject(acSSObj.ObjectId, OpenMode.ForRead).GetType() == typeof(DBText))
                        {
                            // Open the selected object for write
                            DBText acText = trans.GetObject(acSSObj.ObjectId,
                                                                OpenMode.ForRead) as DBText;

                            if (acText != null || acText.TextString.Trim() != "")
                            {
                                // Merge text
                                mergeText += acText.TextString + " ";
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
