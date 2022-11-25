using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using LibraryCad.Models;

namespace LibraryCad
{
    public class SubFunc
    {
        /// <summary>
        /// Hàm bắt điểm người dùng chọn
        /// </summary>
        /// <param name="doc">Document</param>
        public static PointInf PickPoint(Document doc)
        {
            using (OpenCloseTransaction trans = doc.Database.TransactionManager.StartOpenCloseTransaction())
            {
                try
                {
                    PointInf pointInf = new PointInf();
                    PromptPointOptions options = new PromptPointOptions("");
                    PromptPointResult result;
                    options.Message = "\nChọn điểm: ";
                    result = doc.Editor.GetPoint(options);
                    Point3d point = result.Value;
                    if (result.Status == PromptStatus.Cancel) pointInf.status = false;
                    else pointInf.status = true;
                    pointInf.point = point;
                    trans.Commit();
                    return pointInf;
                }
                catch (System.Exception ex)
                {
                    doc.Editor.WriteMessage(ex.Message);
                    trans.Abort();
                    return null;
                }
            }
        }

        /// <summary>
        /// Hàm lấy chuỗi từ editor
        /// </summary>
        /// <param name="doc">Document</param>
        /// <returns></returns>
        public static string GetString(Document doc, string str)
        {
            try
            {
                // Lấy chuỗi người dùng nhập vào
                PromptStringOptions options;
                options = new PromptStringOptions("\n" + str);
                options.AllowSpaces = false;
                PromptResult result = doc.Editor.GetString(options);
                return result.StringResult;
            }
            catch (System.Exception ex)
            {
                doc.Editor.WriteMessage(ex.Message + "\n");
                return null;
            }
        }

        /// <summary>
        /// Hàm lấy các đối tượng được chọn theo filter truyển vào
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="msg">Chuỗi hiển thị cho người dùng</param>
        /// <param name="slft">Đối tượng muốn lọc</param>
        /// <returns>Set các đối tượng</returns>
        public static ObjectId[] GetListSelection(Document doc, string msg, SelectionFilter slft = null)
        {
            try
            {
                PromptSelectionOptions options = new PromptSelectionOptions();
                options.MessageForAdding = $"\n{msg}";
                PromptSelectionResult selResult;
                // Nếu không có filter thì lấy hết
                if (slft == null)
                {
                    selResult = doc.Editor.GetSelection();
                }
                // Nếu có filter thì lấy theo filter
                else
                {
                    selResult = doc.Editor.GetSelection(options, slft);
                }
                // Nếu prompt status là OK, thì trả về object id
                if (selResult.Status == PromptStatus.OK)
                {
                    return selResult.Value.GetObjectIds();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Hàm set xdata cho object
        /// </summary>
        /// <param name="objId">object id</param>
        /// <param name="nameApp">name app của xdata</param>
        /// <param name="xStr">string của xdata</param>
        public static void SetXdata(ObjectId objId, string nameApp, string xStr)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (Transaction trans = db.TransactionManager.StartOpenCloseTransaction())
            {
                try
                {
                    DBObject obj = trans.GetObject(objId, OpenMode.ForWrite);
                    AddRegAppTableRecord(nameApp);
                    ResultBuffer rb = new ResultBuffer(new TypedValue(1001, nameApp), new TypedValue(1000, xStr));
                    obj.XData = rb;
                    rb.Dispose();
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
        /// Hàm thêm RegAppTable
        /// </summary>
        /// <param name="regAppName">tên RegAppTable</param>
        public static void AddRegAppTableRecord(string regAppName)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (var trans = doc.TransactionManager.StartTransaction())
            {
                try
                {
                    RegAppTable rat = (RegAppTable)trans.GetObject(db.RegAppTableId, OpenMode.ForRead, false);
                    if (!rat.Has(regAppName))
                    {
                        rat.UpgradeOpen();
                        RegAppTableRecord ratr = new RegAppTableRecord();
                        ratr.Name = regAppName;
                        rat.Add(ratr);
                        trans.AddNewlyCreatedDBObject(ratr, true);
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
        /// Check vị trí dim người dùng muốn chọn
        /// </summary>
        /// <param name="str">chuỗi người dùng nhập</param>
        /// <returns>số nguyên</returns>
        public static int ChooseDimPosition(string str)
        {
            if (str == "in" || str == "i")
            {
                return 1;
            }
            else if (str == "out" || str == "o" || str == "")
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}
