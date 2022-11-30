using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using LibraryCad.Models;
using System.Collections.Generic;

namespace LibraryCad
{
    public class CircleFunc
    {
        /// <summary>
        /// Hàm convert selectionSet thành list Circle
        /// </summary>
        /// <param name="doc">Document</param>
        /// <returns>List Line</returns>
        public static List<Circle> ParseSelectionToListCircle(Document doc)
        {
            using (var trans = doc.Database.TransactionManager.StartOpenCloseTransaction())
            {
                // Tạo filter để lọc các đối tượng được chọn
                var tvCircle = new TypedValue[]
                {
                        new TypedValue((int)DxfCode.Start, "CIRCLE")
                };
                var filter = new SelectionFilter(tvCircle);
                var objectIds = SubFunc.GetListSelection(doc, "- Chọn các đường tròn: ", filter);
                if (objectIds == null) return null;
                // Parse sang dạng line rồi thêm vào list
                var circles = new List<Circle>();
                foreach (ObjectId objectId in objectIds)
                {
                    Circle circle = trans.GetObject(objectId, OpenMode.ForRead) as Circle;
                    if (circle != null)
                    {
                        circles.Add(circle);
                    }
                }
                trans.Commit();
                return circles;
            }
        }

        /// <summary>
        /// Hàm vẽ đường tròn
        /// </summary>
        /// <param name="doc"></param>
        public static void DrawCircle(Document doc)
        {
            using (var trans = doc.Database.TransactionManager.StartTransaction())
            {
                doc.Editor.WriteMessage("Vẽ đường tròn!");
                BlockTable blockTable = trans.GetObject(doc.Database.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord tableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                // Lấy bán kính nhập vào
                var circleRad = SubFunc.GetString(doc, "- Nhập bán kính: ");
                if (!MathFunc.CheckIfNumber(circleRad)) return;
                // Lấy tâm đường tròn người dùng chọn
                var centerPoint = SubFunc.PickPoint(doc);
                if (!centerPoint.status) return;
                // Tạo đường tròn
                using (var circle = new Circle())
                {
                    circle.Radius = double.Parse(circleRad);
                    circle.Center = centerPoint.point;
                    tableRec.AppendEntity(circle);
                    trans.AddNewlyCreatedDBObject(circle, true);
                }
                trans.Commit();
            }
        }

        /// <summary>
        /// Hàm vẽ tam giác đều nội tiếp đường tròn
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="circle">Đường tròn</param>
        public static void TriangleInscribedInCircle(Document doc, Circle circle)
        {
            using (var trans = doc.Database.TransactionManager.StartTransaction())
            {
                doc.Editor.WriteMessage("Vẽ tam giác nội tiếp đường tròn!");
                BlockTable blockTable = trans.GetObject(doc.Database.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord tableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                // Vẽ tam giác đều nội tiếp đường tròn
                using (var pline = new Autodesk.AutoCAD.DatabaseServices.Polyline())
                {
                    pline.SetDatabaseDefaults();
                    pline.AddVertexAt(0, new Point2d(circle.Center.X, circle.Center.Y + circle.Radius), 0, 0, 0);
                    pline.AddVertexAt(1, new Point2d(circle.Center.X + (circle.Radius * MathFunc.squareRoot(3) / 2), circle.Center.Y - (circle.Radius / 2)), 0, 0, 0);
                    pline.AddVertexAt(2, new Point2d(circle.Center.X - (circle.Radius * MathFunc.squareRoot(3) / 2), circle.Center.Y - (circle.Radius / 2)), 0, 0, 0);
                    pline.Closed = true;
                    tableRec.AppendEntity(pline);
                    trans.AddNewlyCreatedDBObject(pline, true);
                }
                trans.Commit();
            }
        }

        /// <summary>
        /// Hàm vẽ tam giác đều ngoại tiếp đường tròn
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="circle">Đường tròn</param>
        public static void TriangleCircumscribedAboutCircle(Document doc, Circle circle)
        {
            using (var trans = doc.Database.TransactionManager.StartTransaction())
            {
                doc.Editor.WriteMessage("Vẽ tam giác nội tiếp đường tròn!");
                BlockTable blockTable = trans.GetObject(doc.Database.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord tableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                // Vẽ tam giác đều nội tiếp đường tròn
                using (var pline = new Autodesk.AutoCAD.DatabaseServices.Polyline())
                {
                    pline.SetDatabaseDefaults();
                    pline.AddVertexAt(0, new Point2d(circle.Center.X - (circle.Radius * MathFunc.squareRoot(3)), circle.Center.Y - circle.Radius), 0, 0, 0);
                    pline.AddVertexAt(1, new Point2d(circle.Center.X + (circle.Radius * MathFunc.squareRoot(3)), circle.Center.Y - circle.Radius), 0, 0, 0);
                    pline.AddVertexAt(2, new Point2d(circle.Center.X, circle.Center.Y + (circle.Radius * 2)), 0, 0, 0);
                    pline.Closed = true;
                    tableRec.AppendEntity(pline);
                    trans.AddNewlyCreatedDBObject(pline, true);
                }
                trans.Commit();
            }
        }

        /// <summary>
        /// Hàm tính chu vi và diện tích các circle theo layer
        /// </summary>
        /// <param name="layerInfo">thông tin layer</param>
        /// <param name="doc">document</param>
        /// <returns></returns>
        public static LayerObject CircleProperties(LayerInfo layerInfo, Document doc)
        {
            var perimeter = 0.0;
            var area = 0.0;
            var layerObj = new LayerObject();
            using (var trans = doc.Database.TransactionManager.StartOpenCloseTransaction())
            {
                // Tạo filter lấy đường tròn
                TypedValue[] tvCircle = new TypedValue[]
                {
                    new TypedValue((int)DxfCode.Start, "CIRCLE")
                };
                SelectionFilter slftCircle = new SelectionFilter(tvCircle);
                try
                {
                    var circles = LibraryCad.LayerFunc.GetEntityByFilterAndLayer(slftCircle, layerInfo.Name, doc);
                    foreach (var circleId in circles)
                    {
                        var circle = trans.GetObject(circleId.ObjectId, OpenMode.ForRead) as Circle;
                        // Chu vi đường tròn
                        perimeter += circle.Circumference;
                        // Diện tích đường tròn
                        area += circle.Area;
                    }
                    layerObj.LayerName = layerInfo.Name;
                    layerObj.Perimeter = perimeter;
                    layerObj.Area = area;
                    return layerObj;
                }
                catch (System.Exception ex)
                {
                    doc.Editor.WriteMessage(ex.Message);
                    trans.Abort();
                    return null;
                }
            }
        }
    }
}
