using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace Topic1
{
    public class WorkWithString
    {
        [CommandMethod("CreateMText")]
        public static void CreateMText()
        {
            // Get the current document and database
            Document doc = Application.DocumentManager.CurrentDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (doc.LockDocument())
            {
                // Repeat create text method
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
            // Get the current document and database
            Document doc = Application.DocumentManager.CurrentDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (doc.LockDocument())
            {
                //TypedValue[] tv = new TypedValue[]
                //{
                //    new TypedValue(,)
                //};

                // Start a transaction
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    // Request for objects to be selected in the drawing area
                    PromptSelectionResult acSSPrompt = doc.Editor.GetSelection();

                    // If the prompt status is OK, objects were selected
                    if (acSSPrompt.Status == PromptStatus.OK)
                    {
                        SelectionSet acSSet = acSSPrompt.Value;

                        // Step through the objects in the selection set
                        foreach (SelectedObject acSSObj in acSSet)
                        {
                            // Check to make sure a valid SelectedObject object was returned
                            if (acSSObj != null)
                            {
                                // Open the selected object for write
                                DBText acText = trans.GetObject(acSSObj.ObjectId,
                                                                    OpenMode.ForRead) as DBText;

                                if (acText != null)
                                {
                                    // Get text of object
                                    doc.Editor.WriteMessage("text: " + acText.TextString);
                                }
                            }
                        }

                        //// Save the new object to the database
                        //trans.Commit();
                    }
                }
            }
        }

        [CommandMethod("MergeString")]
        public static void MergeString()
        {
            // Get the current document and database
            Document doc = Application.DocumentManager.CurrentDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            // Start a transaction
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



//// Bien Layer
//using (Transaction trrr = db.TransactionManager.StartTransaction())
//{
//    // Tạo layer

//}

//using (Transaction trrr2 = db.TransactionManager.StartTransaction())
//{
//    // Tạo Cir
//    // Dung Bien Layer

//}

//using (OpenCloseTransaction tr = db.TransactionManager.StartOpenCloseTransaction())
//{
//    tr.Commit();
//}
