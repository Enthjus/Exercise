using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using LibraryCad.Models;


namespace LibraryCad.Polyline
{
    public class PolylineFunc
    {
        /// <summary>
        /// Hàm tính chu vi và diện tích các pline theo layer
        /// </summary>
        /// <param name="layerInfo">thông tin layer</param>
        /// <param name="doc">document</param>
        /// <returns></returns>
        public static LayerObject PlineProperties(LayerInfo layerInfo, Document doc)
        {
            var perimeter = 0.0;
            var area = 0.0;
            var lyObj = new LayerObject();
            using (var trans = doc.Database.TransactionManager.StartOpenCloseTransaction())
            {
                TypedValue[] tvPline = new TypedValue[]
                         {
                            new TypedValue((int)DxfCode.Start, "LWPOLYLINE")
                         };
                SelectionFilter slftPline = new SelectionFilter(tvPline);
                try
                {
                    var plineEts = LibraryCad.LayerFunc.GetEntityByFilterAndLayer(slftPline, layerInfo.Name, doc);
                    foreach (var plineEt in plineEts)
                    {
                        var pline = trans.GetObject(plineEt.ObjectId, OpenMode.ForRead) as Autodesk.AutoCAD.DatabaseServices.Polyline;
                        perimeter += pline.Length;
                        if (pline.Closed) area += pline.Area;
                    }
                    lyObj.LayerName = layerInfo.Name;
                    lyObj.Perimeter = perimeter;
                    lyObj.Area = area;
                    return lyObj;
                }
                catch (System.Exception ex)
                {
                    doc.Editor.WriteMessage(ex.Message);
                    return null;
                }
            }
        }
    }
}
