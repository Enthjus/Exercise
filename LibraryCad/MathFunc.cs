using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System.Text.RegularExpressions;
using LibraryCad.Models;
using System.Collections.Generic;


namespace LibraryCad
{
    public class MathFunc
    {
        /// <summary>
        /// Hàm check số
        /// </summary>
        /// <param name="num">chuỗi số thực</param>
        /// <returns>bool</returns>
        public static bool CheckIfNumber(string num)
        {
            Regex rgx = new Regex(@"(\d+(?:\.\d+)?)");
            if (rgx.IsMatch(num)) return true;
            return false;
        }

        /// <summary>
        /// Hàm check số các đối tượng được chọn
        /// </summary>
        /// <param name="sSet">Các đối tượng được chọn</param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static bool CheckIfSelectionSetIsNumber(SelectionSet sSet, Document doc)
        {
            using (var trans = doc.Database.TransactionManager.StartOpenCloseTransaction())
            {
                foreach (SelectedObject acSSObj in sSet)
                {
                    // Check to make sure a valid SelectedObject object was returned
                    if (acSSObj != null)
                    {
                        // Open the selected object for write
                        DBText acText = trans.GetObject(acSSObj.ObjectId,
                                                            OpenMode.ForRead) as DBText;
                        // Nếu không phải số thì trả về false
                        if (!CheckIfNumber(acText.TextString)) return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Hàm lấy số từ editor
        /// </summary>
        /// <param name="doc">Document</param>
        /// <returns></returns>
        public static double GetNum(Document doc)
        {
            while (1 == 1)
            {
                // Lấy số nhập từ editor
                PromptStringOptions pStrOpts;
                pStrOpts = new PromptStringOptions("\nNhập số: ");
                pStrOpts.AllowSpaces = false;
                PromptResult pStrRes = doc.Editor.GetString(pStrOpts);
                while (1 == 1)
                {
                    if (CheckIfNumber(pStrRes.StringResult))
                    {
                        return double.Parse(pStrRes.StringResult);
                    }
                    pStrOpts = new PromptStringOptions("\nDữ liệu nhập vào phải là số: ");
                    pStrRes = doc.Editor.GetString(pStrOpts);
                }
            }
        }

        /// <summary>
        /// Hàm chỉ lấy số từ các đối tượng được chọn
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="msg">Chuỗi thông báo</param>
        /// <returns></returns>
        public static List<double> SelectionSetToNumList(Document doc, string msg)
        {
            using (var trans = doc.Database.TransactionManager.StartOpenCloseTransaction())
            {
                var typeValue = new TypedValue[]
                {
                    new TypedValue((int)DxfCode.Start, "TEXT"),
                    new TypedValue((int)DxfCode.Start, "MTEXT"),
                    new TypedValue((int)DxfCode.Start, "DTEXT")
                };
                var slft = new SelectionFilter(typeValue);
                var objectIds = SubFunc.GetListSelection(doc, msg, slft);
                if (objectIds == null) return null;
                var nums = new List<double>();
                foreach (ObjectId obj in objectIds)
                {
                    DBText s = trans.GetObject(obj, OpenMode.ForRead) as DBText;
                    if (s != null && CheckIfNumber(s.TextString))
                    {
                        nums.Add(double.Parse(s.TextString));
                    }
                }
                return nums;
            }
        }

        /// <summary>
        /// Hàm check dãy số có tồn tại số không hay không
        /// </summary>
        /// <param name="nums">dãy số truyền vào</param>
        /// <returns></returns>
        public static bool CheckIfHasZero(List<double> nums)
        {
            foreach (double num in nums)
            {
                // Check to make sure a valid SelectedObject object was returned
                if (num != null && num == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Hàm check số đầu tiên của dãy số nếu là 0 thì trả về true
        /// </summary>
        /// <param name="nums">Dãy số muốn check</param>
        /// <returns></returns>
        public static bool CheckIfFirstNumIsZero(List<double> nums)
        {
            if (nums[0] == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Hàm check các số của dãy từ số thứ 2 trở đi nếu có số 0 trả về true
        /// </summary>
        /// <param name="nums">dãy số muốn check</param>
        /// <returns></returns>
        public static bool CheckIfOtherNumIsZero(List<double> nums)
        {
            for (int i = 1; i < nums.Count; i++)
            {
                if (nums[i] == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Hàm tính căn bậc 2
        /// </summary>
        /// <param name="number">số cần tính căn</param>
        /// <returns></returns>
        public static double squareRoot(int number)
        {
            double temp;
            double sr = number / 2;

            do
            {
                temp = sr;
                sr = (temp + (number / temp)) / 2;
            } while ((temp - sr) != 0);
            return sr;
        }
    }
}
