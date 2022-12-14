using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using LibraryCad.Mathematic;
using LibraryCad.Models;
using LibraryCad.ObjectsFunc.LayerObject;
using LibraryCad.Sub;
using System.Collections.Generic;
using System.Linq;

namespace LibraryCad.ObjectsFunc.CircleObject
{
    public class CircleFunc
    {
        #region Lọc lấy đường trong trong các đối tượng được chọn
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
        #endregion

        #region Vẽ đường tròn
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
        #endregion

        #region Vẽ tam giác nội tiếp đường tròn
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
        #endregion

        #region Vẽ tam giác ngoại tiếp đường tròn
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
        #endregion

        #region Lấy thuộc tính chu vi và diện tích các đường tròn theo layer
        /// <summary>
        /// Hàm tính chu vi và diện tích các circle theo layer
        /// </summary>
        /// <param name="layerInfo">thông tin layer</param>
        /// <param name="doc">document</param>
        /// <returns></returns>
        public static LayerObjectInfo CircleProperties(LayerInfo layerInfo, Document doc)
        {
            var perimeter = 0.0;
            var area = 0.0;
            var layerObj = new LayerObjectInfo();
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
                    var circles = LayerFunc.GetEntityByFilterAndLayer(slftCircle, layerInfo.Name, doc);
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
        #endregion

        #region Lấy id các đường tròn được chọn
        /// <summary>
        /// Hàm lấy object id các circle được chọn
        /// </summary>
        /// <param name="doc">Document</param>
        /// <returns>ObjectId</returns>
        public static ObjectId CircleSelect(Document doc)
        {
            try
            {
                PromptSelectionOptions prSelOptions = new PromptSelectionOptions();
                prSelOptions.SingleOnly = true;
                prSelOptions.SinglePickInSpace = true;
                TypedValue[] tvCircle = new TypedValue[]
                {
                new TypedValue((int)DxfCode.Start, "CIRCLE")
                };
                SelectionFilter filter = new SelectionFilter(tvCircle);
                PromptSelectionResult prSelResult = doc.Editor.GetSelection(prSelOptions, filter);
                if (prSelResult.Status == PromptStatus.OK)
                {
                    return prSelResult.Value.OfType<SelectedObject>().First().ObjectId;
                }
                return ObjectId.Null;
            }
            catch
            {
                return ObjectId.Null;
            }
        }
        #endregion
    }
}
