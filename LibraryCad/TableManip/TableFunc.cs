using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace LibraryCad.TableManip
{
    public class TableFunc
    {
        #region Create text with table
        /// <summary>
        /// Hàm sắp xếp các text trên bản vẽ hiện hành thành các bảng theo thông tin truyền vào
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="db">Database</param>
        /// <param name="trans">Transaction</param>
        /// <param name="dbTexts">Danh sách các đối tượng text trên bản vẽ</param>
        /// <param name="point">Điểm đặt bảng</param>
        /// <param name="sortTable">Kiểu sắp xếp bảng ngang hay dọc</param>
        /// <param name="sortText">Kiểu sắp xếp chữ theo thứ tự ngang hay dọc</param>
        /// <param name="rows">Số hàng của bảng</param>
        /// <param name="cols">Số cột của bảng</param>
        /// <param name="distance">Khoảng cách các bảng</param>
        public static void CreateTextTable(Document doc, Database db, Transaction trans, List<DBText> dbTexts, Point3d point, string sortTable, string sortText, int rows, int cols, double distance)
        {
            var pt = point;
            var totalTextOld = 0;
            while (1 == 1)
            {
                BlockTable blockTable = trans.GetObject(doc.Database.BlockTableId, OpenMode.ForRead) as BlockTable;
                DBDictionary styleDic = trans.GetObject(db.TableStyleDictionaryId, OpenMode.ForRead) as DBDictionary;
                Table table = new Table();
                // Sử dụng table style
                table.TableStyle = styleDic.GetAt("Phuc");
                table.NumRows = rows;
                table.NumColumns = cols;

                table.Position = pt;
                string[,] str = new string[rows, cols];
                if(sortText == "horizontal")
                {
                    for (var i1 = 0; i1 < rows; i1++)
                    {
                        for (var i2 = 0; i2 < cols; i2++)
                        {
                            try
                            {
                                var dbText = trans.GetObject(dbTexts[totalTextOld].ObjectId, OpenMode.ForWrite) as DBText;
                                if (dbText.TextString != "")
                                {
                                    str[i1, i2] = dbText.TextString;
                                }
                                dbText.Erase();
                            }
                            catch
                            {
                                str[i1, i2] = "";
                                continue;
                            }
                            totalTextOld++;
                        }
                    }
                }
                else if(sortText == "vertical")
                {
                    for (var i1 = 0; i1 < cols; i1++)
                    {
                        for (var i2 = 0; i2 < rows; i2++)
                        {
                            var dbText = trans.GetObject(dbTexts[totalTextOld].ObjectId, OpenMode.ForWrite) as DBText;
                            if (dbText.TextString != "")
                            {
                                str[i2, i1] = dbText.TextString;
                            }
                            else
                            {
                                str[i1, i2] = "";
                            }
                            dbText.Erase();
                            totalTextOld++;
                        }
                    }
                }
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        //table.SetTextHeight(i, j, 1);
                        table.SetTextString(i, j, str[i, j]);
                        table.SetAlignment(i, j, CellAlignment.MiddleCenter);
                    }
                }
                table.GenerateLayout();
                BlockTableRecord tableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                tableRec.AppendEntity(table);
                trans.AddNewlyCreatedDBObject(table, true);
                if(sortTable == "horizontal")
                {
                    pt = new Point3d(pt.X + table.Width + distance, pt.Y, 0);
                }
                else if(sortTable == "vertical")
                {
                    pt = new Point3d(pt.X, pt.Y + table.Height + distance, 0);
                }
                if (totalTextOld >= dbTexts.Count) break;
            }
        }
        #endregion

        #region Create text without table
        /// <summary>
        /// Hàm sắp xếp các text trên bản vẽ hiện hành theo dạng bảng từ thông tin truyền vào
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="dbTexts">Danh sách các đối tượng text trên bản vẽ</param>
        /// <param name="point">Điểm đặt bảng</param>
        /// <param name="sortTable">Kiểu sắp xếp bảng ngang hay dọc</param>
        /// <param name="sortText">Kiểu sắp xếp chữ theo thứ tự ngang hay dọc</param>
        /// <param name="rows">Số hàng của bảng</param>
        /// <param name="cols">Số cột của bảng</param>
        /// <param name="distance">Khoảng cách các bảng</param>
        public static void SortWithoutTable(Transaction trans, List<DBText> dbTexts, Point3d point, string sortTable, string sortText, int rows, int cols, double distance)
        {
            if (dbTexts == null) return;
            var totalTextOld = 0;
            while (1 == 1)
            {
                var pt1 = point;
                DBText dbText = new DBText();
                if (sortText == "horizontal")
                {
                    for (var i1 = 0; i1 < rows; i1++)
                    {
                        var pt2 = pt1;
                        for (var i2 = 0; i2 < cols; i2++)
                        {
                            try
                            {
                                dbText = trans.GetObject(dbTexts[totalTextOld].ObjectId, OpenMode.ForWrite) as DBText;
                                if (dbText == null) continue;
                                dbText.Position = pt2;
                                pt2 = new Point3d(pt2.X + dbText.WidthFactor + 3, pt2.Y, 0);
                            }
                            catch
                            {
                                break;
                            }
                            totalTextOld++;
                        }
                        pt1 = new Point3d(pt1.X, pt1.Y - dbText.Height - 3, 0);
                    }
                }
                else if (sortText == "vertical")
                {
                    for (var i1 = 0; i1 < cols; i1++)
                    {
                        var pt2 = pt1;
                        for (var i2 = 0; i2 < rows; i2++)
                        {
                            try
                            {
                                dbText = trans.GetObject(dbTexts[totalTextOld].ObjectId, OpenMode.ForWrite) as DBText;
                                if (dbText == null) continue;
                                dbText.Position = pt2;
                                pt2 = new Point3d(pt2.X, pt2.Y - dbText.WidthFactor - 3, 0);
                            }
                            catch
                            {
                                break;
                            }
                            totalTextOld++;
                        }
                        pt1 = new Point3d(pt1.X + dbText.Height + 3, pt1.Y, 0);
                    }
                }
                if (sortTable == "horizontal")
                {
                    point = new Point3d(point.X + cols * (dbText.WidthFactor + 3) + distance, point.Y, 0);
                }
                else if (sortTable == "vertical")
                {
                    point = new Point3d(point.X, point.Y - rows * (dbText.Height + 3) + distance, 0);
                }
                if (totalTextOld >= dbTexts.Count) break;
            }
        }
        #endregion
    }
}
