using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topic1.WorkWithObject
{
    public class WorkWithPolyline
    {
        [CommandMethod("Dimpolyline")]
        public static void DimPolyline()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            // Lọc các đối tượng được chọn chỉ lấy polyline
            var plines = LibraryCad.Polyline.PolylineFunc.SelectionSetToPolyline(doc);
            if(plines == null) return;
            // Cho người dùng chọn dim mặt trong hay ngoài 
            var dimPosition = LibraryCad.SubFunc.GetString(doc, "Nhập in(i) nếu muốn dim bên trong, out(o) nếu muốn dim bên ngoài, không nhập gì thì sẽ theo mặc định: ");
            var dimStatus = LibraryCad.SubFunc.ChooseDimPosition(dimPosition);
            if (dimStatus == -1) return;
            foreach (var pline in plines)
            {
                // Lấy các điểm gấp khúc của polyline
                var plineInfos = LibraryCad.Polyline.PolylineFunc.GetVerticesOfPline(pline);
                foreach (var plineInfo in plineInfos) 
                {
                    // Nếu là đường cong thì dim theo đường cong
                    if (plineInfo.Status)
                    {
                        LibraryCad.DimensionFunc.DimArc(plineInfo.CircularArc, doc, dimStatus);
                    }
                    // Đoạn thẳng thì dim theo đoạn thẳng
                    else
                    {
                        var points = new List<Point3d>()
                        {
                            plineInfo.StartPoint,
                            plineInfo.EndPoint
                        };
                        LibraryCad.DimensionFunc.DimPolyline(points, doc, dimStatus);
                    }
                }
            }
        }
    }
}
