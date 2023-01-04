using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCad.ObjectsFunc.PolylineObject
{
    public static class PolylineExtension
    {
        /// <summary>
        /// Enumeration of offset side options
        /// </summary>
        public enum OffsetSide { In, Out, Left, Right, Both }

        /// <summary>
        /// Offset the source polyline to specified side(s).
        /// </summary>
        /// <param name="source">The polyline to be offseted.</param>
        /// <param name="offsetDist">The offset distance.</param>
        /// <param name="side">The offset side(s).</param>
        /// <returns>A polyline sequence resulting from the offset of the source polyline.</returns>
        public static IEnumerable<Polyline> Offset(this Polyline source, double offsetDist, OffsetSide side)
        {
            offsetDist = Math.Abs(offsetDist);
            IEnumerable<Polyline> offsetRight = source.GetOffsetCurves(offsetDist).Cast<Polyline>();
            double areaRight = offsetRight.Select(pline => pline.Area).Sum();
            IEnumerable<Polyline> offsetLeft = source.GetOffsetCurves(-offsetDist).Cast<Polyline>();
            double areaLeft = offsetLeft.Select(pline => pline.Area).Sum();
            switch (side)
            {
                case OffsetSide.In:
                    if (areaRight < areaLeft)
                    {
                        offsetLeft.Dispose();
                        return offsetRight;
                    }
                    else
                    {
                        offsetRight.Dispose();
                        return offsetLeft;
                    }
                case OffsetSide.Out:
                    if (areaRight < areaLeft)
                    {
                        offsetRight.Dispose();
                        return offsetLeft;
                    }
                    else
                    {
                        offsetLeft.Dispose();
                        return offsetRight;
                    }
                case OffsetSide.Left:
                    offsetRight.Dispose();
                    return offsetLeft;
                case OffsetSide.Right:
                    offsetLeft.Dispose();
                    return offsetRight;
                case OffsetSide.Both:
                    return offsetRight.Concat(offsetLeft);
                default:
                    return null;
            }
        }

        private static void Dispose(this IEnumerable<Polyline> plines)
        {
            foreach (Polyline pline in plines)
            {
                pline.Dispose();
            }
        }
    }
}
