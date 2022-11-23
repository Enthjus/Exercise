using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using System;

namespace Topic1
{
    public class WorkWithString
    {
        [CommandMethod("CreateMText")]
        public static void CreateMText()
        {
            Document doc = Application.DocumentManager.CurrentDocument;

            using (doc.LockDocument())
            {
                // Cho tạo nhiều chuỗi đến khi người dùng nhấn esc hoặc hủy lệnh
                while (1 == 1)
                {
                    // Lấy chuỗi từ editor
                    var text = LibraryCad.SubFunc.GetString(doc).Trim();

                    // Chọn điểm
                    var pointInf = LibraryCad.SubFunc.PickPoint(doc);

                    if (pointInf == null || pointInf.status == false) return;

                    // Tạo chuỗi
                    LibraryCad.TextFunc.CreateText(doc, text, pointInf.point);
                }
            }

        }

        [CommandMethod("ReadText")]
        public static void ReadText()
        {
            Document doc = Application.DocumentManager.CurrentDocument;
            Database db = doc.Database;

            using (doc.LockDocument())
            {
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    PromptSelectionResult acSSPrompt = doc.Editor.GetSelection();

                    if (acSSPrompt.Status == PromptStatus.OK)
                    {
                        SelectionSet acSSet = acSSPrompt.Value;

                        foreach (SelectedObject acSSObj in acSSet)
                        {
                            if (acSSObj != null)
                            {
                                DBText acText = trans.GetObject(acSSObj.ObjectId, OpenMode.ForRead) as DBText;

                                if (acText != null)
                                {
                                    doc.Editor.WriteMessage("text: " + acText.TextString);
                                }
                            }
                        }

                        trans.Commit();
                    }
                }
            }
        }

        [CommandMethod("MergeString")]
        public static void MergeString()
        {
            Document doc = Application.DocumentManager.CurrentDocument;
            Database db = doc.Database;
            
            using (doc.LockDocument())
            {
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        // Chọn điểm
                        var pointInf = LibraryCad.SubFunc.PickPoint(doc);
                        Point3d point = pointInf.point;

                        if (pointInf == null || pointInf.status == false) return;

                        // Chọn Set đối tượng
                        string mergeText = LibraryCad.TextFunc.MergeString(doc);

                        if (mergeText == "") return;

                        // Create text
                        LibraryCad.TextFunc.CreateText(doc, mergeText, point);

                        // Save the new object to the database
                        trans.Commit();
                    }
                    catch (System.Exception ex)
                    {
                        doc.Editor.WriteMessage(ex.Message);
                        trans.Abort();
                    }
                }
            }
        }
    }
}



