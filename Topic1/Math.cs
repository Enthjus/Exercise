using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using LibraryCad.Models;
using LibraryCad;

namespace Topic1
{
    public class Math
    {
        [CommandMethod("Sum")]
        public static void Sum()
        {
            // Get the current document and database
            Document doc = Application.DocumentManager.CurrentDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            // Start a transaction
            using (var trans = db.TransactionManager.StartTransaction())
            {
                try
                {
                    PromptPointOptions pPtOpts = new PromptPointOptions("");

                    // Chọn điểm
                    pPtOpts.Message = "\nChọn điểm muốn đặt kết quả: ";
                    PointInf pointInf = LibraryCad.Functions.PickPoint(doc);
                    Point3d point = pointInf.point;

                    if (pointInf == null || pointInf.status == false) return;

                    SelectionSet acSSet;

                    // Check nếu chọn toàn số thì tiếp tục
                    while (1 == 1)
                    {
                        doc.Editor.WriteMessage("\nChọn các giá trị muốn cộng: ");
                        acSSet = LibraryCad.Functions.GetListSelection(doc);
                        if (acSSet == null) return;
                        if (LibraryCad.Functions.CheckIfSelectionSetIsNumber(acSSet, doc)) break;
                        else doc.Editor.WriteMessage("\nGiá trị được chọn phải là số");
                    }

                    var sum = 0.0;

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
                                // Merge text
                                sum += double.Parse(acText.TextString);
                            }
                        }
                    }

                    // Create text
                    LibraryCad.Functions.CreateText(doc, sum.ToString(), point);

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

        [CommandMethod("SumSkipString")]
        public static void SumSkipString()
        {
            // Get the current document and database
            Document doc = Application.DocumentManager.CurrentDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            // Start a transaction
            using (var trans = db.TransactionManager.StartTransaction())
            {
                try
                {
                    PromptPointOptions pPtOpts = new PromptPointOptions("");

                    // Chọn điểm
                    pPtOpts.Message = "\nChọn điểm muốn đặt kết quả: ";
                    PointInf pointInf = LibraryCad.Functions.PickPoint(doc);
                    Point3d point = pointInf.point;

                    if (pointInf == null || pointInf.status == false) return;

                    SelectionSet acSSet;

                    // Lấy các giá trị được chọn
                    doc.Editor.WriteMessage("\nChọn các giá trị muốn cộng: ");
                    acSSet = LibraryCad.Functions.GetListSelection(doc);
                    if (acSSet == null) return;

                    var sum = 0.0;

                    // Step through the objects in the selection set
                    foreach (SelectedObject acSSObj in acSSet)
                    {
                        // Check to make sure a valid SelectedObject object was returned
                        if (acSSObj != null)
                        {
                            // Open the selected object for write
                            DBText acText = trans.GetObject(acSSObj.ObjectId,
                                                                OpenMode.ForRead) as DBText;

                            if (acText != null && LibraryCad.Functions.CheckIfNumber(acText.TextString))
                            {
                                // Merge text
                                sum += double.Parse(acText.TextString);
                            }
                        }
                    }

                    // Create text
                    LibraryCad.Functions.CreateText(doc, sum.ToString(), point);

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

        [CommandMethod("Subtraction")]
        public static void Subtraction()
        {
            // Get the current document and database
            Document doc = Application.DocumentManager.CurrentDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            // Start a transaction
            using (var trans = db.TransactionManager.StartTransaction())
            {
                try
                {
                    PromptPointOptions pPtOpts = new PromptPointOptions("");

                    // Chọn điểm
                    pPtOpts.Message = "\nChọn điểm muốn đặt kết quả: ";
                    PointInf pointInf = LibraryCad.Functions.PickPoint(doc);
                    Point3d point = pointInf.point;

                    if (pointInf == null || pointInf.status == false) return;

                    SelectionSet acSSet;

                    // Lấy các giá trị được chọn
                    doc.Editor.WriteMessage("\nChọn các giá trị muốn cộng: ");
                    acSSet = LibraryCad.Functions.GetListSelection(doc);
                    if (acSSet == null) return;

                    var subtraction = 0.0;

                    // Step through the objects in the selection set
                    foreach (SelectedObject acSSObj in acSSet)
                    {
                        // Check to make sure a valid SelectedObject object was returned
                        if (acSSObj != null)
                        {
                            // Open the selected object for write
                            DBText acText = trans.GetObject(acSSObj.ObjectId,
                                                                OpenMode.ForRead) as DBText;

                            if (acText != null && LibraryCad.Functions.CheckIfNumber(acText.TextString))
                            {
                                // Merge text
                                subtraction -= double.Parse(acText.TextString);
                            }
                        }
                    }

                    // Create text
                    LibraryCad.Functions.CreateText(doc, subtraction.ToString(), point);

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

        [CommandMethod("Multiplication")]
        public static void Multiplication()
        {
            // Get the current document and database
            Document doc = Application.DocumentManager.CurrentDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            // Start a transaction
            using (var trans = db.TransactionManager.StartTransaction())
            {
                try
                {
                    PromptPointOptions pPtOpts = new PromptPointOptions("");

                    // Chọn điểm
                    pPtOpts.Message = "\nChọn điểm muốn đặt kết quả: ";
                    PointInf pointInf = LibraryCad.Functions.PickPoint(doc);
                    Point3d point = pointInf.point;

                    if (pointInf == null || pointInf.status == false) return;

                    SelectionSet acSSet;

                    // Lấy các giá trị được chọn
                    doc.Editor.WriteMessage("\nChọn các giá trị muốn cộng: ");
                    acSSet = LibraryCad.Functions.GetListSelection(doc);
                    if (acSSet == null) return;

                    // Check nếu có số 0 thì in ra 0 luôn
                    if (LibraryCad.Functions.CheckIfHasZero(acSSet, trans))
                    {
                        LibraryCad.Functions.CreateText(doc, "0", point);

                        // Save the new object to the database
                        trans.Commit();

                        return;
                    }

                    var multiplication = 0.0;

                    // Step through the objects in the selection set
                    foreach (SelectedObject acSSObj in acSSet)
                    {
                        // Check to make sure a valid SelectedObject object was returned
                        if (acSSObj != null)
                        {
                            // Open the selected object for write
                            DBText acText = trans.GetObject(acSSObj.ObjectId,
                                                                OpenMode.ForRead) as DBText;

                            if (acText != null && LibraryCad.Functions.CheckIfNumber(acText.TextString))
                            {
                                if (multiplication == 0)
                                {
                                    multiplication = double.Parse(acText.TextString);
                                }
                                else
                                {
                                    multiplication *= double.Parse(acText.TextString);
                                }
                            }
                        }
                    }

                    // Create text
                    LibraryCad.Functions.CreateText(doc, multiplication.ToString(), point);

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

        [CommandMethod("Division")]
        public static void Division()
        {
            // Get the current document and database
            Document doc = Application.DocumentManager.CurrentDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            // Start a transaction
            using (var trans = db.TransactionManager.StartTransaction())
            {
                try
                {
                    PromptPointOptions pPtOpts = new PromptPointOptions("");
                    SelectionSet acSSet;
                    var division = 0.0;

                    // Chọn điểm
                    pPtOpts.Message = "\nChọn điểm muốn đặt kết quả: ";
                    PointInf pointInf = LibraryCad.Functions.PickPoint(doc);
                    Point3d point = pointInf.point;
                    if (pointInf == null || pointInf.status == false) return;

                    // Lấy các giá trị được chọn
                    doc.Editor.WriteMessage("\nChọn các giá trị muốn cộng: ");
                    acSSet = LibraryCad.Functions.GetListSelection(doc);
                    if (acSSet == null) return;

                    // Check nếu từ số thứ 2 trở đi nếu có 0 thì trả về luôn
                    if (LibraryCad.Functions.CheckIfOtherNumIsZero(acSSet, trans)) return;

                    // Check nếu số đầu là 0 thì trả về 0
                    if (LibraryCad.Functions.CheckIfFirstNumIsZero(acSSet, trans))
                    {
                        LibraryCad.Functions.CreateText(doc, "0", point);

                        // Save the new object to the database
                        trans.Commit();

                        return;
                    }

                    // Step through the objects in the selection set
                    foreach (SelectedObject acSSObj in acSSet)
                    {
                        // Check to make sure a valid SelectedObject object was returned
                        if (acSSObj != null)
                        {
                            // Open the selected object for write
                            DBText acText = trans.GetObject(acSSObj.ObjectId,
                                                                OpenMode.ForRead) as DBText;

                            if (acText != null && LibraryCad.Functions.CheckIfNumber(acText.TextString))
                            {
                                if (division == 0)
                                {
                                    division = double.Parse(acText.TextString);
                                }
                                else
                                {
                                    division /= double.Parse(acText.TextString);
                                }
                            }
                        }
                    }

                    // Create text
                    LibraryCad.Functions.CreateText(doc, division.ToString(), point);

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
