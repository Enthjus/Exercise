using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
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

        /// <summary>
        /// Adds an arc (fillet), if able, at each polyline vertex.
        /// </summary>
        /// <param name="pline">The instance to which the method applies.</param>
        /// <param name="radius">The arc radius.</param>
        public static void FilletAll(this Polyline pline, double radius)
        {
            int n = pline.Closed ? 0 : 1;
            for (int i = n; i < pline.NumberOfVertices - n; i += 1 + pline.FilletAt(i, radius))
            { }
        }

        /// <summary>
        /// Adds an arc (fillet) at the specified vertex.
        /// </summary>
        /// <param name="pline">The instance to which the method applies.</param>
        /// <param name="index">The index of the vertex.</param>
        /// <param name="radius">The arc radius.</param>
        /// <returns>1 if the operation succeeded, 0 if it failed.</returns>
        public static int FilletAt(this Polyline pline, int index, double radius)
        {
            int prev = index == 0 && pline.Closed ? pline.NumberOfVertices - 1 : index - 1;
            if (pline.GetSegmentType(prev) != SegmentType.Line ||
                pline.GetSegmentType(index) != SegmentType.Line)
            {
                return 0;
            }
            LineSegment2d seg1 = pline.GetLineSegment2dAt(prev);
            LineSegment2d seg2 = pline.GetLineSegment2dAt(index);
            Vector2d vec1 = seg1.StartPoint - seg1.EndPoint;
            Vector2d vec2 = seg2.EndPoint - seg2.StartPoint;
            double angle = (Math.PI - vec1.GetAngleTo(vec2)) / 2.0;
            double dist = radius * Math.Tan(angle);
            if (dist == 0.0 || dist > seg1.Length || dist > seg2.Length)
            {
                return 0;
            }
            Point2d pt1 = seg1.EndPoint + vec1.GetNormal() * dist;
            Point2d pt2 = seg2.StartPoint + vec2.GetNormal() * dist;
            double bulge = Math.Tan(angle / 2.0);
            if (Clockwise(seg1.StartPoint, seg1.EndPoint, seg2.EndPoint))
            {
                bulge = -bulge;
            }
            pline.AddVertexAt(index, pt1, bulge, 0.0, 0.0);
            pline.SetPointAt(index + 1, pt2);
            return 1;
        }

        /// <summary>
        /// Evaluates if the points are clockwise.
        /// </summary>
        /// <param name="p1">First point.</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <returns>true if points are clockwise, false otherwise.</returns>
        private static bool Clockwise(Point2d p1, Point2d p2, Point2d p3)
        {
            return ((p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X)) < 1e-8;
        }
    }
}
