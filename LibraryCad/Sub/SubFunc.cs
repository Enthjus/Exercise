using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryCad.Sub
{
    public class SubFunc
    {
        #region PickPoint
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
                    if (result.Status == PromptStatus.Cancel) pointInf.status = false;
                    else pointInf.status = true;
                    Point3d point = result.Value;
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
        #endregion

        #region GetString
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
                PromptStringOptions options = new PromptStringOptions("\n" + str);
                options.AllowSpaces = false;
                PromptResult result = doc.Editor.GetString(options);
                if (result.Status != PromptStatus.OK) return "";
                return result.StringResult;
            }
            catch (System.Exception ex)
            {
                doc.Editor.WriteMessage(ex.Message + "\n");
                return null;
            }
        }
        #endregion

        #region GetListSelection
        /// <summary>
        /// Hàm lấy các đối tượng được chọn theo filter truyển vào
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="msg">Chuỗi hiển thị cho người dùng</param>
        /// <param name="slft">Đối tượng muốn lọc</param>
        /// <returns>Set các đối tượng</returns>
        public static ObjectId[] GetListSelection(Document doc, string msg, SelectionFilter filter = null)
        {
            try
            {
                PromptSelectionOptions options = new PromptSelectionOptions();
                options.MessageForAdding = $"\n{msg}";
                PromptSelectionResult selResult;
                // Nếu không có filter thì lấy hết
                if (filter == null)
                {
                    selResult = doc.Editor.GetSelection(options);
                }
                // Nếu có filter thì lấy theo filter
                else
                {
                    selResult = doc.Editor.GetSelection(options, filter);
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
        #endregion

        #region SetXdata
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
                    AddRegAppTableRecord(doc, nameApp);
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
        #endregion

        #region AddRegAppTableRecord
        /// <summary>
        /// Hàm thêm RegAppTable
        /// </summary>
        /// <param name="regAppName">tên RegAppTable</param>
        public static void AddRegAppTableRecord(Document doc, string regAppName)
        {
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
        #endregion

        #region ChooseDimPosition
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
        #endregion

        #region PickAllObject
        /// <summary>
        /// Hàm lấy danh sách id các đối tượng được chọn
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="objIdColl">Danh sách id các đối tượng</param>
        public static void PickAllObject(Document doc, ObjectIdCollection objIdColl)
        {
            Database db = doc.Database;
            using (doc.LockDocument())
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    // Lấy list các DBObjects
                    var objIds = GetListSelection(doc, "\n- Chọn các đối tượng muốn sao chép: ");
                    if (objIds != null)
                    {
                        foreach (var objId in objIds)
                        {
                            objIdColl.Add(objId);
                        }
                    }
                    trans.Commit();
                }
            }
        }
        #endregion

        #region CLoneObjectToDatabase
        /// <summary>
        /// Hàm clone các đối tượng sang database khác
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="objIdColl">Danh sách id các đối tượng</param>
        /// <param name="filePath">Đường dẫn file muốn đặt đối tượng</param>
        public static void CLoneObjectToDatabase(Document doc, ObjectIdCollection objIdColl, string filePath)
        {
            Database newDB = new Database(true, true);
            using (newDB)
            {
                using (Transaction trans = newDB.TransactionManager.StartTransaction())
                {
                    BlockTable blockTable = trans.GetObject(newDB.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord tableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    // Clone đối tượng qua database mới tạo
                    IdMapping idMap = new IdMapping();
                    doc.Database.WblockCloneObjects(objIdColl, tableRec.ObjectId, idMap, DuplicateRecordCloning.Ignore, false);
                    trans.Commit();
                    // Lưu thành file mới
                    newDB.SaveAs(filePath, DwgVersion.Current);
                }
            }
        }
        #endregion

        #region getObjIds
        /// <summary>
        /// Hàm lấy danh sách ObjectId các đối tượng được chọn
        /// </summary>
        /// <param name="ed">Editor</param>
        /// <param name="filter">Loại đối tượng muốn lấy</param>
        /// <returns>Mảng các object id</returns>
        public static ObjectId[] getObjIds(Editor ed, SelectionFilter filter)
        {
            try
            {
                PromptSelectionResult prSelResult = ed.SelectAll(filter);
                if (prSelResult.Status == PromptStatus.OK)
                {
                    var objIds = prSelResult.Value.GetObjectIds();
                    return objIds;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region giaithua
        public static int giaithua(int a)
        {
            if (a == 1)
                return 1; // Kết thúc đệ quy

            return
                a * giaithua(a - 1);  // Đệ quy
        }
        #endregion

        /// <summary>
        /// Lấy database từ nguồn file truyền vào
        /// </summary>
        /// <param name="fileName">Đường dẫn file</param>
        /// <param name="ed">Editor</param>
        /// <returns>Database tìm được</returns>
        public static Database GetDbByPath(string fileName, Editor ed)
        {
            try
            {
                Database openDb = new Database(false, true);
                openDb.ReadDwgFile(fileName, System.IO.FileShare.ReadWrite, true, "");
                return openDb;
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Lấy style từ file dwt
        /// </summary>
        /// <param name="openDb">database</param>
        /// <returns>list style</returns>
        public static StyleInfo FindAllStyle(Database openDb)
        {
            try
            {
                using (openDb)
                {
                    StyleInfo styleInfo = new StyleInfo();
                    List<String> textStyles = new List<String>();
                    List<String> dimStyles = new List<String>();
                    List<String> tableStyles = new List<String>();
                    List<String> mleaderStyles = new List<String>();
                    using (Transaction tr = openDb.TransactionManager.StartTransaction())
                    {
                        var textStyleTable = (TextStyleTable)tr.GetObject(openDb.TextStyleTableId, OpenMode.ForRead);

                        foreach (ObjectId id in textStyleTable)
                        {
                            try
                            {
                                TextStyleTableRecord tableRecord = tr.GetObject(id, OpenMode.ForRead) as TextStyleTableRecord;
                                if (tableRecord != null)
                                {
                                    textStyles.Add(tableRecord.Name);
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }

                        var dimStyleTable = (DimStyleTable)tr.GetObject(openDb.DimStyleTableId, OpenMode.ForRead);

                        foreach (ObjectId id in dimStyleTable)
                        {
                            try
                            {
                                DimStyleTableRecord tableRecord = tr.GetObject(id, OpenMode.ForRead) as DimStyleTableRecord;
                                if (tableRecord != null)
                                {
                                    dimStyles.Add(tableRecord.Name);
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }

                        DBDictionary tblStyleDic = (DBDictionary)tr.GetObject(openDb.TableStyleDictionaryId, OpenMode.ForRead);
                        foreach (DBDictionaryEntry entry in tblStyleDic)
                        {
                            try
                            {
                                TableStyle tableStyle = (TableStyle)tr.GetObject(entry.Value, OpenMode.ForRead);
                                tableStyles.Add(tableStyle.Name);
                            }
                            catch
                            {
                                continue;
                            }
                        }

                        DBDictionary mleaderStyleDic = tr.GetObject(openDb.MLeaderStyleDictionaryId, OpenMode.ForRead) as DBDictionary;
                        foreach (DBDictionaryEntry entry in mleaderStyleDic)
                        {
                            try
                            {
                                MLeaderStyle mleaderStyle = tr.GetObject(entry.Value, OpenMode.ForRead) as MLeaderStyle;
                                mleaderStyles.Add(mleaderStyle.Name);
                            }
                            catch
                            {
                                continue;
                            }
                        }

                        styleInfo.TextStyles = textStyles;
                        styleInfo.DimStyles = dimStyles;
                        styleInfo.TableStyles = tableStyles;
                        styleInfo.MLeaderStyles = mleaderStyles;
                        tr.Commit();
                        return styleInfo;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public static Point3d PolarPoint(Point3d basepoint, double angle, double distance)
        {
            return new Point3d(
            basepoint.X + (distance * Math.Cos(angle)),
            basepoint.Y + (distance * Math.Sin(angle)),
            basepoint.Z);
        }

        public static double AngleFromX(Point3d pt1, Point3d pt2)
        {
            Plane ucsplane = new Plane(new Point3d(0, 0, 0), new Vector3d(0, 0, 1));

            Vector3d vec = pt2 - pt1;

            double ang = vec.AngleOnPlane(ucsplane);

            return ang;
        }

        public static bool isLeft(Point3d spt, Point3d ept, Point3d apt)
        {
            bool result = false;

            double Ans = ((ept.X - spt.X) * (apt.Y - spt.Y) -
            (apt.X - spt.X) * (ept.Y - spt.Y));

            if (Ans > 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

    }
}
