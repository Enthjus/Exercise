using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using LibraryCad.Models;
using System.Collections.Generic;

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
            var layerObj = new LayerObject();
            using (var trans = doc.Database.TransactionManager.StartOpenCloseTransaction())
            {
                // Tạo filter để lọc đối tượng theo ý muốn
                TypedValue[] tvPline = new TypedValue[]
                {
                    new TypedValue((int)DxfCode.Start, "LWPOLYLINE")
                };
                SelectionFilter slftPline = new SelectionFilter(tvPline);
                try
                {
                    // Lấy polyline từ các đối tượng được chọn
                    var plineEts = LibraryCad.LayerFunc.GetEntityByFilterAndLayer(slftPline, layerInfo.Name, doc);
                    // Tính tổng độ dài các polyline và area nếu polyline đóng
                    foreach (var plineEt in plineEts)
                    {
                        var pline = trans.GetObject(plineEt.ObjectId, OpenMode.ForRead) as Autodesk.AutoCAD.DatabaseServices.Polyline;
                        perimeter += pline.Length;
                        if (pline.Closed) area += pline.Area;
                    }
                    layerObj.LayerName = layerInfo.Name;
                    layerObj.Perimeter = perimeter;
                    layerObj.Area = area;
                    return layerObj;
                }
                catch (System.Exception ex)
                {
                    doc.Editor.WriteMessage(ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// Hàm lấy diểm gấp khúc của polyline
        /// </summary>
        /// <param name="polyline">polyline</param>
        /// <returns>list các point</returns>
        public static List<Models.PlineInfo> GetVerticesOfPline(Autodesk.AutoCAD.DatabaseServices.Polyline polyline)
        {
            try
            {
                var plineInfos = new List<Models.PlineInfo>();
                if (polyline != null)
                {
                    // Quét qua từng điểm gấp khúc của đoạn thẳng
                    var verticesNum = polyline.NumberOfVertices;
                    for (int i = 0; i < verticesNum; i++)
                    {
                        // Khi điểm cuối trùng điểm đầu hoặc polyline đóng thì thoát khỏi vòng lặp
                        if (i == verticesNum - 1 && polyline.GetPoint3dAt(i) == polyline.GetPoint3dAt(0) || i == verticesNum - 1 && polyline.Closed == false) break;
                        Models.PlineInfo plineInfo = new Models.PlineInfo();
                        plineInfo.Status = false;
                        // Nếu là đường cong thì status = true và gán đường cong
                        if (polyline.GetSegmentType(i) == SegmentType.Arc)
                        {
                            var arc = polyline.GetArcSegmentAt(i);
                            if(arc != null)
                            {
                                plineInfo.CircularArc = arc;
                                plineInfo.Status = true;
                            }
                        }
                        plineInfo.StartPoint = polyline.GetPoint3dAt(i);
                        if(i == verticesNum - 1)
                        {
                            plineInfo.EndPoint = polyline.GetPoint3dAt(0);
                        }
                        else
                        {
                            plineInfo.EndPoint = polyline.GetPoint3dAt(i + 1);
                        }
                        if (plineInfo != null)
                        {
                            plineInfos.Add(plineInfo);
                        }
                    }
                    return plineInfos;
                }
                return plineInfos;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Hàm convert selection set thành list polyline
        /// </summary>
        /// <param name="objIds">list id object</param>
        /// <param name="doc">Document</param>
        /// <returns>list polyline</returns>
        public static List<Autodesk.AutoCAD.DatabaseServices.Polyline> SelectionSetToPolyline(Document doc)
        {
            try
            {
                Database db = doc.Database;
                var plines = new List<Autodesk.AutoCAD.DatabaseServices.Polyline>();    
                using (doc.LockDocument())
                {
                    using (Transaction trans = db.TransactionManager.StartOpenCloseTransaction())
                    {
                        TypedValue[] tvPline = new TypedValue[]
                        {
                            new TypedValue((int)DxfCode.Start, "LWPOLYLINE")
                        };
                        SelectionFilter slftPline = new SelectionFilter(tvPline);
                        var objIds = LibraryCad.SubFunc.GetListSelection(doc, "- Chọn polyline:", slftPline);
                        foreach (var objId in objIds)
                        {
                            if (objId == null) continue;
                            var pline = trans.GetObject(objId, OpenMode.ForRead) as Autodesk.AutoCAD.DatabaseServices.Polyline;
                            plines.Add(pline);
                        }
                        return plines;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
