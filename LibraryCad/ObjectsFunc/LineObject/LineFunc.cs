using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using LibraryCad.Models;
using System.Collections.Generic;

namespace LibraryCad
{
    public class LineFunc
    {
        /// <summary>
        /// Hàm convert selectionSet thành list Line
        /// </summary>
        /// <param name="selectSet">Các đối tượng được chọn</param>
        /// <param name="doc">Document</param>
        /// <returns>List Line</returns>
        public static List<Line> SelectionSetToListLine(Document doc)
        {
            try
            {
                using (doc.LockDocument())
                {
                    using (var trans = doc.Database.TransactionManager.StartOpenCloseTransaction())
                    {
                        var lines = new List<Line>();
                        var tvLine = new TypedValue[]
                        {
                        new TypedValue((int)DxfCode.Start, "LINE")
                        };
                        var filter = new SelectionFilter(tvLine);
                        var objectIds = SubFunc.GetListSelection(doc, "- Chọn các đoạn thẳng: ", filter);
                        if (objectIds == null) return null;
                        // Lấy đoạn thẳng từ list object id 
                        foreach (ObjectId objId in objectIds)
                        {
                            var line = trans.GetObject(objId, OpenMode.ForRead) as Line;
                            if (line != null)
                            {
                                lines.Add(line);
                            }
                        }
                        return lines;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Hàm tính tổng các line theo layer
        /// </summary>
        /// <param name="layerInfo">thông tin layer</param>
        /// <param name="doc">document</param>
        /// <returns></returns>
        public static double LineProperties(LayerInfo layerInfo, Document doc)
        {
            var perimeter = 0.0;
            using (var trans = doc.Database.TransactionManager.StartOpenCloseTransaction())
            {
                // Tạo filter để lọc đối tượng theo dạng mong muốn
                TypedValue[] tvLine = new TypedValue[]
                {
                    new TypedValue((int)DxfCode.Start, "LINE")
                };
                SelectionFilter slftLine = new SelectionFilter(tvLine);
                try
                {
                    var lineEts = LibraryCad.LayerFunc.GetEntityByFilterAndLayer(slftLine, layerInfo.Name, doc);
                    // Cộng tổng độ dài các đoạn thẳng lấy được
                    foreach (var lineEt in lineEts)
                    {
                        var line = trans.GetObject(lineEt.ObjectId, OpenMode.ForRead) as Line;
                        perimeter += line.Length;
                    }
                    return perimeter;
                }
                catch (System.Exception ex)
                {
                    doc.Editor.WriteMessage(ex.Message);
                    return 0;
                }
            }
        }

        /// <summary>
        /// Hàm nối 4 điểm thành hình vuông theo tham số truyền vào
        /// </summary>
        /// <param name="size">kích thước</param>
        /// <returns></returns>
        public static DBObjectCollection SquareOfLines(double size)
        {
            DBObjectCollection entities = new DBObjectCollection();
            Point3d[] points =
            {
                new Point3d(-size, -size, 0),
                new Point3d(size, -size, 0),
                new Point3d(size, size, 0),
                new Point3d(-size, size, 0),
            };
            int max = points.GetUpperBound(0);
            for (int i = 0; i <= max; i++)
            {
                int j = (i == max ? 0 : i + 1);
                Line line = new Line(points[i], points[j]);
                entities.Add(line);
            }
            return entities;
        }

        /// <summary>
        /// Hàm tìm trung điểm của đoạn thẳng 
        /// </summary>
        /// <param name="line">đường thẳng</param>
        /// <returns></returns>
        public static Point3d GetMidpoint(Line line)
        {
            return line.GetPointAtDist(line.Length / 2.0);
        }
    }
}
