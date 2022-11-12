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
            
            using (doc.LockDocument())
            {
                // Start a transaction
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        PromptPointOptions pPtOpts = new PromptPointOptions("");

                        // Chọn điểm
                        pPtOpts.Message = "\nChọn điểm muốn đặt kết quả: ";
                        PointInf pointInf = LibraryCad.SubFunc.PickPoint(doc);
                        Point3d point = pointInf.point;

                        if (pointInf == null || pointInf.status == false) return;

                        // Check nếu chọn toàn số thì tiếp tục
                        var nums = LibraryCad.MathFunc.SelectionSetToNumList(doc, "Chọn các số muốn cộng");
                        if (nums == null) return;

                        var sum = 0.0;

                        // Step through the objects in the selection set
                        foreach (double num in nums)
                        {
                            sum += num;
                        }

                        // Create text
                        LibraryCad.TextFunc.CreateText(doc, sum.ToString(), point);

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

        [CommandMethod("SumSkipString")]
        public static void SumSkipString()
        {
            // Get the current document and database
            Document doc = Application.DocumentManager.CurrentDocument;
            Database db = doc.Database;
            
            using (doc.LockDocument())
            {
                // Start a transaction
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        PromptPointOptions pPtOpts = new PromptPointOptions("");

                        // Chọn điểm để ghi kết quả
                        pPtOpts.Message = "\nChọn điểm muốn đặt kết quả: ";
                        PointInf pointInf = LibraryCad.SubFunc.PickPoint(doc);
                        Point3d point = pointInf.point;

                        if (pointInf == null || pointInf.status == false) return;

                        // Lấy các giá trị được chọn
                        var nums = LibraryCad.MathFunc.SelectionSetToNumList(doc, "- Chọn các giá trị muốn cộng: ");
                        if (nums.Count == 0) return;

                        var sum = 0.0;

                        // Step through the objects in the selection set
                        foreach (double num in nums)
                        {
                            if (num != 0)
                            {
                                sum += num;
                            }
                        }

                        // Create text
                        LibraryCad.TextFunc.CreateText(doc, sum.ToString(), point);

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

        [CommandMethod("Subtraction")]
        public static void Subtraction()
        {
            // Get the current document and database
            Document doc = Application.DocumentManager.CurrentDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (doc.LockDocument())
            {
                // Start a transaction
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        PromptPointOptions pPtOpts = new PromptPointOptions("");

                        // Chọn điểm
                        pPtOpts.Message = "\nChọn điểm muốn đặt kết quả: ";
                        PointInf pointInf = LibraryCad.SubFunc.PickPoint(doc);
                        Point3d point = pointInf.point;

                        if (pointInf == null || pointInf.status == false) return;

                        // Lấy các giá trị được chọn
                        var nums = LibraryCad.MathFunc.SelectionSetToNumList(doc, "Chọn các giá trị muốn tính hiệu: ");
                        if (nums == null) return;

                        var subtraction = 0.0;

                        // Step through the objects in the selection set
                        foreach (double num in nums)
                        {
                            if (num != 0)
                            {
                                subtraction -= num;
                            }
                        }

                        // Create text
                        LibraryCad.TextFunc.CreateText(doc, subtraction.ToString(), point);

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

        [CommandMethod("Multiplication")]
        public static void Multiplication()
        {
            // Get the current document and database
            Document doc = Application.DocumentManager.CurrentDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (doc.LockDocument())
            {
                // Start a transaction
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        PromptPointOptions pPtOpts = new PromptPointOptions("");

                        // Chọn điểm
                        pPtOpts.Message = "\nChọn điểm muốn đặt kết quả: ";
                        PointInf pointInf = LibraryCad.SubFunc.PickPoint(doc);
                        Point3d point = pointInf.point;

                        if (pointInf == null || pointInf.status == false) return;

                        // Lấy các giá trị được chọn
                        var nums = LibraryCad.MathFunc.SelectionSetToNumList(doc, "- Chọn các số muốn tính tích: ");
                        if (nums == null) return;

                        // Check nếu có số 0 thì in ra 0 luôn
                        if (LibraryCad.MathFunc.CheckIfHasZero(nums))
                        {
                            LibraryCad.TextFunc.CreateText(doc, "0", point);
                            trans.Commit();
                            return;
                        }

                        var multiplication = 0.0;

                        // Step through the objects in the selection set
                        foreach (double num in nums)
                        {
                            if (multiplication == 0)
                            {
                                multiplication = num;
                            }
                            else
                            {
                                multiplication *= num;
                            }
                        }

                        // Create text
                        LibraryCad.TextFunc.CreateText(doc, multiplication.ToString(), point);

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

        [CommandMethod("Division")]
        public static void Division()
        {
            // Get the current document and database
            Document doc = Application.DocumentManager.CurrentDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (doc.LockDocument())
            {
                // Start a transaction
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        PromptPointOptions pPtOpts = new PromptPointOptions("");
                        var division = 0.0;

                        // Chọn điểm
                        pPtOpts.Message = "\nChọn điểm muốn đặt kết quả: ";
                        PointInf pointInf = LibraryCad.SubFunc.PickPoint(doc);
                        Point3d point = pointInf.point;
                        if (pointInf == null || pointInf.status == false) return;

                        // Lấy các giá trị được chọn
                        var nums = LibraryCad.MathFunc.SelectionSetToNumList(doc, "- Chọn các số muốn chia: ");
                        if (nums == null) return;

                        // Check nếu từ số thứ 2 trở đi nếu có 0 thì trả về luôn
                        if (LibraryCad.MathFunc.CheckIfOtherNumIsZero(nums))
                        {
                            doc.Editor.WriteMessage("Không thể chia cho số 0!");
                            return;
                        }

                        // Check nếu số đầu là 0 thì trả về 0
                        if (LibraryCad.MathFunc.CheckIfFirstNumIsZero(nums))
                        {
                            LibraryCad.TextFunc.CreateText(doc, "0", point);
                            trans.Commit();
                            return;
                        }

                        // Step through the objects in the selection set
                        foreach (double num in nums)
                        {
                            // Check to make sure a valid SelectedObject object was returned
                            if (division == 0)
                            {
                                division = num;
                            }
                            else
                            {
                                division /= num;
                            }
                        }

                        // Create text
                        LibraryCad.TextFunc.CreateText(doc, division.ToString(), point);

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
