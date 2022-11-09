using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
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
                var typeVal = new TypedValue[]
                {
                    new TypedValue((int)DxfCode.Start, "CIRCLE")
                };
                var slft = new SelectionFilter(typeVal);
                var objectIds = SubFunc.GetListSelection(doc, "- Chọn các đường tròn: ", slft);

                if (objectIds == null) return null;

                var circles = new List<Circle>();
                // Step through the objects in the selection set
                foreach (ObjectId objectId in objectIds)
                {
                    // Check object có null hay không và có phải là line không
                    Circle circle = trans.GetObject(objectId, OpenMode.ForRead) as Circle;

                    if (circle != null)
                    {
                        circles.Add(circle);
                    }
                }
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
                try
                {
                    doc.Editor.WriteMessage("Vẽ đường tròn!");
                    BlockTable bt = trans.GetObject(doc.Database.BlockTableId, OpenMode.ForRead) as BlockTable;

                    BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    var circleRad = MathFunc.GetNum(doc);
                    var centerPoint = SubFunc.PickPoint(doc);

                    if (!centerPoint.status) return;

                    using (var circle = new Circle())
                    {
                        circle.Radius = circleRad;
                        circle.Center = centerPoint.point;

                        btr.AppendEntity(circle);
                        trans.AddNewlyCreatedDBObject(circle, true);
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
        /// Hàm vẽ tam giác cân nội tiếp đường tròn
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="circle">Đường tròn</param>
        public static void TriangleInscribedInCircle(Document doc, Circle circle)
        {
            using (var trans = doc.Database.TransactionManager.StartTransaction())
            {
                doc.Editor.WriteMessage("Vẽ tam giác nội tiếp đường tròn!");
                BlockTable bt = trans.GetObject(doc.Database.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                // Vẽ tam giác đều nội tiếp đường tròn
                using (var pline = new Polyline())
                {
                    pline.SetDatabaseDefaults();
                    pline.AddVertexAt(0, new Point2d(circle.Center.X, circle.Center.Y + circle.Radius), 0, 0, 0);
                    pline.AddVertexAt(1, new Point2d(circle.Center.X + (circle.Radius * MathFunc.squareRoot(3) / 2), circle.Center.Y - (circle.Radius / 2)), 0, 0, 0);
                    pline.AddVertexAt(2, new Point2d(circle.Center.X - (circle.Radius * MathFunc.squareRoot(3) / 2), circle.Center.Y - (circle.Radius / 2)), 0, 0, 0);
                    //pline.AddVertexAt(3, new Point2d(circle.Center.X, circle.Center.Y + circle.Radius), 0, 0, 0);
                    pline.Closed = true;
                    btr.AppendEntity(pline);
                    trans.AddNewlyCreatedDBObject(pline, true);
                }

                trans.Commit();
            }
        }

        /// <summary>
        /// Hàm vẽ tam giác cân ngoại tiếp đường tròn
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="circle">Đường tròn</param>
        public static void TriangleCircumscribedAboutCircle(Document doc, Circle circle)
        {
            using (var trans = doc.Database.TransactionManager.StartTransaction())
            {
                doc.Editor.WriteMessage("Vẽ tam giác nội tiếp đường tròn!");
                BlockTable bt = trans.GetObject(doc.Database.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                // Vẽ tam giác đều nội tiếp đường tròn
                using (var pline = new Polyline())
                {
                    pline.SetDatabaseDefaults();
                    pline.AddVertexAt(0, new Point2d(circle.Center.X - (circle.Radius * MathFunc.squareRoot(3)), circle.Center.Y - circle.Radius), 0, 0, 0);
                    pline.AddVertexAt(1, new Point2d(circle.Center.X + (circle.Radius * MathFunc.squareRoot(3)), circle.Center.Y - circle.Radius), 0, 0, 0);
                    pline.AddVertexAt(2, new Point2d(circle.Center.X, circle.Center.Y + (circle.Radius * 2)), 0, 0, 0);
                    pline.Closed = true;


                    //pline.ObjectId;
                    //pline.Handle

                    btr.AppendEntity(pline);
                    trans.AddNewlyCreatedDBObject(pline, true);
                }

                trans.Commit();
            }
        }
    }
}
