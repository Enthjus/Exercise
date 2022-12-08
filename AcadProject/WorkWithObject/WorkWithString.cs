using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using LibraryCad;
using LibraryCad.Sub;

namespace Topic1.WorkWithObject
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
                    var text = SubFunc.GetString(doc, "Nhập text muốn tạo").Trim();
                    // Chọn điểm
                    var pointInf = SubFunc.PickPoint(doc);
                    if (pointInf == null || pointInf.status == false) return;
                    // Tạo chuỗi tại điểm người dùng vừa chọn
                    TextFunc.CreateText(doc, text, pointInf.point, doc.Database.Clayer);
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
                    PromptSelectionResult result = doc.Editor.GetSelection();
                    if (result.Status == PromptStatus.OK)
                    {
                        SelectionSet selSet = result.Value;
                        foreach (SelectedObject selObj in selSet)
                        {
                            if (selObj != null)
                            {
                                DBText dbText = trans.GetObject(selObj.ObjectId, OpenMode.ForRead) as DBText;
                                if (dbText != null)
                                {
                                    doc.Editor.WriteMessage("text: " + dbText.TextString);
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
                        var pointInf = SubFunc.PickPoint(doc);
                        Point3d point = pointInf.point;
                        if (pointInf == null || pointInf.status == false) return;
                        // Chọn Set đối tượng
                        string mergeText = TextFunc.MergeString(doc);
                        if (mergeText == "") return;
                        // Tạo text theo chuỗi vừa gom được
                        TextFunc.CreateText(doc, mergeText, point, doc.Database.Clayer);
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



