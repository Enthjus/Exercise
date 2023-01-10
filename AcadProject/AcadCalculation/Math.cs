using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using LibraryCad;
using LibraryCad.DocumentManager;
using LibraryCad.Mathematic;
using LibraryCad.Models;
using LibraryCad.ObjectsFunc.TextObject;
using LibraryCad.Sub;

namespace AcadProject.AcadCalculation
{
    public class Math
    {
        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

        [CommandMethod("Sum")]
        public static void Sum()
        {
            using (doc.LockDocument())
            {
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        // Lấy điểm người dùng chọn
                        PromptPointOptions pPtOpts = new PromptPointOptions("");
                        pPtOpts.Message = "\nChọn điểm muốn đặt kết quả: ";
                        PointInf pointInf = SubFunc.PickPoint(doc);
                        Point3d point = pointInf.point;
                        if (pointInf == null || !pointInf.status) return;
                        // Lấy các giá trị được chọn và lọc trả về các chuỗi là số
                        var nums = MathFunc.SelectionSetToNumList(doc, "Chọn các số muốn cộng");
                        if (nums == null) return;
                        var sum = 0.0;
                        foreach (double num in nums)
                        {
                            sum += num;
                        }
                        // Tạo chuỗi ghi kết quả
                        if(Init.remainder == 1)
                        {
                            sum = System.Math.Round(sum);
                        }
                        TextFunc.CreateText(doc, sum.ToString(), point, db.Clayer);
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
            using (doc.LockDocument())
            {
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    PromptPointOptions pPtOpts = new PromptPointOptions("");
                    pPtOpts.Message = "\nChọn điểm muốn đặt kết quả: ";
                    PointInf pointInf = SubFunc.PickPoint(doc);
                    Point3d point = pointInf.point;
                    if (pointInf == null || !pointInf.status) return;
                    // Lấy các giá trị được chọn và lọc trả về các chuỗi là số
                    var nums = MathFunc.SelectionSetToNumList(doc, "- Chọn các giá trị muốn cộng: ");
                    if (nums == null) return;
                    var sum = 0.0;
                    foreach (double num in nums)
                    {
                        if (num != 0)
                        {
                            sum += num;
                        }
                    }
                    // Tạo chuỗi ghi kết quả
                    if (Init.remainder == 1)
                    {
                        sum = System.Math.Round(sum);
                    }
                    TextFunc.CreateText(doc, sum.ToString(), point, db.Clayer);
                    trans.Commit();
                }
            }
        }

        [CommandMethod("Subtraction")]
        public static void Subtraction()
        {
            using (doc.LockDocument())
            {
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        PromptPointOptions pPtOpts = new PromptPointOptions("");
                        pPtOpts.Message = "\nChọn điểm muốn đặt kết quả: ";
                        PointInf pointInf = SubFunc.PickPoint(doc);
                        Point3d point = pointInf.point;
                        if (pointInf == null || !pointInf.status) return;
                        // Lấy các giá trị được chọn và lọc trả về các chuỗi là số
                        var nums = MathFunc.SelectionSetToNumList(doc, "Chọn các giá trị muốn tính hiệu: ");
                        if (nums == null) return;
                        var subtraction = 0.0;
                        foreach (double num in nums)
                        {
                            if (num != 0)
                            {
                                subtraction -= num;
                            }
                        }
                        // Tạo chuỗi chứa kết quả
                        TextFunc.CreateText(doc, subtraction.ToString(), point, db.Clayer);
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
            using (doc.LockDocument())
            {
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        PromptPointOptions pPtOpts = new PromptPointOptions("");
                        pPtOpts.Message = "\nChọn điểm muốn đặt kết quả: ";
                        PointInf pointInf = SubFunc.PickPoint(doc);
                        Point3d point = pointInf.point;
                        if (pointInf == null || !pointInf.status) return;
                        // Lấy các giá trị được chọn và lọc trả về các chuỗi là số
                        var nums = MathFunc.SelectionSetToNumList(doc, "- Chọn các số muốn tính tích: ");
                        if (nums == null) return;
                        // Check nếu có số 0 thì in ra 0 luôn
                        if (MathFunc.CheckIfHasZero(nums))
                        {
                            TextFunc.CreateText(doc, "0", point, db.Clayer);
                            trans.Commit();
                            return;
                        }
                        var multiplication = 0.0;
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
                        // Tạo chuỗi ghi kết quả
                        TextFunc.CreateText(doc, multiplication.ToString(), point, db.Clayer);
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
            using (doc.LockDocument())
            {
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        PromptPointOptions pPtOpts = new PromptPointOptions("");
                        var division = 0.0;
                        pPtOpts.Message = "\nChọn điểm muốn đặt kết quả: ";
                        PointInf pointInf = SubFunc.PickPoint(doc);
                        Point3d point = pointInf.point;
                        if (pointInf == null || !pointInf.status) return;
                        // Lấy các giá trị được chọn và lọc trả về các chuỗi là số
                        var nums = MathFunc.SelectionSetToNumList(doc, "- Chọn các số muốn chia: ");
                        if (nums == null) return;
                        // Check nếu từ số thứ 2 trở đi nếu có 0 thì trả về luôn
                        if (MathFunc.CheckIfOtherNumIsZero(nums))
                        {
                            doc.Editor.WriteMessage("Không thể chia cho số 0!");
                            return;
                        }
                        // Check nếu số đầu là 0 thì trả về 0
                        if (MathFunc.CheckIfFirstNumIsZero(nums))
                        {
                            TextFunc.CreateText(doc, "0", point, db.Clayer);
                            trans.Commit();
                            return;
                        }
                        foreach (double num in nums)
                        {
                            if (division == 0)
                            {
                                division = num;
                            }
                            else
                            {
                                division /= num;
                            }
                        }
                        // Tạo chuỗi ghi kết quả
                        TextFunc.CreateText(doc, division.ToString(), point, db.Clayer);
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
