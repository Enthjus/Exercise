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
                    PointInf pInf = new PointInf();
                    PromptPointOptions pPtOpts = new PromptPointOptions("");
                    PromptPointResult pPtRes;
                    pPtOpts.Message = "\nChọn điểm: ";
                    pPtRes = doc.Editor.GetPoint(pPtOpts);
                    Point3d point = pPtRes.Value;
                    if (pPtRes.Status == PromptStatus.Cancel) pInf.status = false;
                    else pInf.status = true;
                    pInf.point = point;
                    trans.Commit();
                    return pInf;
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
        public static string GetString(Document doc)
        {
            // Get string from editor
            PromptStringOptions pStrOpts;
            pStrOpts = new PromptStringOptions("\nNhập chuỗi: ");
            pStrOpts.AllowSpaces = true;
            PromptResult pStrRes = doc.Editor.GetString(pStrOpts);
            return pStrRes.StringResult;
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
            PromptSelectionOptions options = new PromptSelectionOptions();
            options.MessageForAdding = $"\n{msg}";
            PromptSelectionResult acSSPrompt;
            // Request for objects to be selected in the drawing area
            if (slft == null)
            {
                acSSPrompt = doc.Editor.GetSelection();
            }
            else
            {
                acSSPrompt = doc.Editor.GetSelection(options, slft);
            }
            // If the prompt status is OK, objects were selected
            if (acSSPrompt.Status == PromptStatus.OK)
            {
                //acSSPrompt.Value.GetObjectIds();
                //SelectionSet acSSet = acSSPrompt.Value;
                return acSSPrompt.Value.GetObjectIds(); 
            }
            return null;
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
                    DBObject obj = trans.GetObject(objId, OpenMode.ForWrite);

                    AddRegAppTableRecord(nameApp);

                    ResultBuffer rb = new ResultBuffer(new TypedValue(1001, nameApp), new TypedValue(1000, xStr));

                    obj.XData = rb;

                    rb.Dispose();

                    trans.Commit();
                }
            }

        /// <summary>
        /// Hàm thêm RegAppTable
        /// </summary>
        /// <param name="regAppName">tên RegAppTable</param>
        public static void AddRegAppTableRecord(string regAppName)
        {

            Document doc = Application.DocumentManager.MdiActiveDocument;

            Editor ed = doc.Editor;

            Database db = doc.Database;

            Transaction tr = doc.TransactionManager.StartTransaction();

            using (tr)
            {
                RegAppTable rat = (RegAppTable)tr.GetObject(db.RegAppTableId, OpenMode.ForRead, false);

                if (!rat.Has(regAppName))

                {

                    rat.UpgradeOpen();

                    RegAppTableRecord ratr = new RegAppTableRecord();

                    ratr.Name = regAppName;

                    rat.Add(ratr);

                    tr.AddNewlyCreatedDBObject(ratr, true);

                }

                tr.Commit();

            }

        }
    }
}
