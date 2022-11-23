using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using LibraryCad;
using System.Linq;
using System.Collections.Generic;

namespace Topic1
{
    public class WorkWithDimension
    {
        [CommandMethod("DimensionSum")]
        public static void DimensionSum()
        {
            Document doc = Application.DocumentManager.CurrentDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (doc.LockDocument())
            {
                // Tạo filter
                var typeValues = new TypedValue[]
                    {
                        new TypedValue((int)DxfCode.Start, "DIMENSION")
                    };
                var slft = new SelectionFilter(typeValues);
                
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    var sum = 0.0;
                    var dimensions = new List<Dimension>();

                    // Lấy đối tượng theo filter
                    var objectIds = SubFunc.GetListSelection(doc, "", slft);
                    if (objectIds == null) return;
                    foreach (var objectId in objectIds)
                    {
                        var dimension = trans.GetObject(objectId, OpenMode.ForRead) as Dimension;
                        if (dimension != null)
                        {
                            dimensions.Add(dimension);
                        }
                    }

                    // Cộng tổng các kích thước dim lại
                    dimensions.Where(dim => dim.Measurement > 0).ToList().ForEach(dimension => sum += dimension.Measurement);

                    // Set layer hiện tại
                    var layer = db.Clayer;

                    // Lấy điểm đặt kết quả
                    var ptn = LibraryCad.SubFunc.PickPoint(doc);
                    if (ptn.status == false) return;
                    LibraryCad.TextFunc.CreateText(doc, System.Math.Round(sum).ToString(), ptn.point, layer);

                    trans.Commit();
                }
            }
        }

        [CommandMethod("CreateRotatedDimension")]
        public static void CreateRotatedDimension()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord tableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // Tạo dim dạng rotated
                using (RotatedDimension rotateDim = new RotatedDimension())
                {
                    rotateDim.XLine1Point = new Point3d(0, 0, 0);
                    rotateDim.XLine2Point = new Point3d(6, 3, 0);
                    rotateDim.Rotation = 0.707;
                    rotateDim.DimLinePoint = new Point3d(0, 5, 0);
                    rotateDim.DimensionStyle = db.Dimstyle;

                    tableRec.AppendEntity(rotateDim);
                    trans.AddNewlyCreatedDBObject(rotateDim, true);
                }

                trans.Commit();
            }
        }
    }
}
