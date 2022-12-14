using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using LibraryCad.Models;
using LibraryCad.ObjectsFunc.LayerObject;
using LibraryCad.Sub;
using System.Collections.Generic;

namespace LibraryCad.ObjectsFunc.ArcObject
{
    public class ArcFunc
    {
        #region Lấy độ dài đường cong
        /// <summary>
        /// Hàm tính tổng các đường cong theo layer
        /// </summary>
        /// <param name="layerInfo">thông tin layer</param>
        /// <param name="doc">document</param>
        /// <returns></returns>
        public static double ArcProperties(LayerInfo layerInfo, Document doc)
        {
            Database db = doc.Database;
            var perimeter = 0.0;
            using (var trans = db.TransactionManager.StartOpenCloseTransaction())
            {
                TypedValue[] tvArc = new TypedValue[]
                {
                    new TypedValue((int)DxfCode.Start, "ARC")
                };
                SelectionFilter filterArc = new SelectionFilter(tvArc);
                try
                {
                    var arcs = LayerFunc.GetEntityByFilterAndLayer(filterArc, layerInfo.Name, doc);
                    foreach (var arcId in arcs)
                    {
                        var arc = trans.GetObject(arcId.ObjectId, OpenMode.ForRead) as Autodesk.AutoCAD.DatabaseServices.Arc;
                        perimeter += arc.Length;
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
        #endregion

        #region Lọc lấy đường cong trong các đối tượng được chọn
        /// <summary>
        /// Hàm convert selectionSet thành list đường cong
        /// </summary>
        /// <param name="selectSet">Các đối tượng được chọn</param>
        /// <param name="doc">Document</param>
        /// <returns>list đường cong</returns>
        public static List<Arc> SelectionSetToListArc(Document doc)
        {
            using (doc.LockDocument())
            {
                using (var trans = doc.Database.TransactionManager.StartOpenCloseTransaction())
                {
                    var arcs = new List<Arc>();
                    var tvArc = new TypedValue[]
                    {
                        new TypedValue((int)DxfCode.Start, "ARC")
                    };
                    var filterArc = new SelectionFilter(tvArc);
                    var objectIds = SubFunc.GetListSelection(doc, "\n- Chọn các đường cong: ", filterArc);
                    if (objectIds == null) return null;
                    // Step through the objects in the selection set
                    foreach (ObjectId objId in objectIds)
                    {
                        var arc = trans.GetObject(objId, OpenMode.ForRead) as Autodesk.AutoCAD.DatabaseServices.Arc;
                        if (arc != null)
                        {
                            arcs.Add(arc);
                        }
                    }
                    return arcs;
                }
            }
        }
        #endregion

        #region Lấy trung điểm đường cong
        /// <summary>
        /// Hàm tìm trung điểm của đường cong 
        /// </summary>
        /// <param name="curve">đường cong</param>
        /// <returns></returns>
        public static Point3d GetMidpoint(Curve curve)
        {
            double d1 = curve.GetDistanceAtParameter(curve.StartParam);
            double d2 = curve.GetDistanceAtParameter(curve.EndParam);
            return curve.GetPointAtDist(d1 + ((d2 - d1) / 2.0));
        }
        #endregion
    }
}
