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
            // Get the current document and database
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

                    // Lấy và tạo point đặt kết quả
                    var ptn = LibraryCad.SubFunc.PickPoint(doc);
                    if (ptn.status == false) return;
                    LibraryCad.TextFunc.CreateText(doc, System.Math.Round(sum).ToString(), ptn.point, layer);

                    trans.Commit();
                }
            }
        }
    }
}
