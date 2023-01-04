using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCad.Sub
{
    public static class AlgerbraicArea
    {
        public static double GetArea(Point2d pt1, Point2d pt2, Point2d pt3)
        {
            return (((pt2.X - pt1.X) * (pt3.Y - pt1.Y)) -
                        ((pt3.X - pt1.X) * (pt2.Y - pt1.Y))) / 2.0;
        }

        public static double GetArea(this CircularArc2d arc)
        {
            double rad = arc.Radius;
            double ang = arc.IsClockWise ?
                arc.StartAngle - arc.EndAngle :
                arc.EndAngle - arc.StartAngle;
            return rad * rad * (ang - Math.Sin(ang)) / 2.0;
        }

        public static double GetArea(this Polyline pline)
        {
            CircularArc2d arc = new CircularArc2d();
            double area = 0.0;
            int last = pline.NumberOfVertices - 1;
            Point2d p0 = pline.GetPoint2dAt(0);

            if (pline.GetBulgeAt(0) != 0.0)
            {
                area += pline.GetArcSegment2dAt(0).GetArea();
            }
            for (int i = 1; i < last; i++)
            {
                area += GetArea(p0, pline.GetPoint2dAt(i), pline.GetPoint2dAt(i + 1));
                if (pline.GetBulgeAt(i) != 0.0)
                {
                    area += pline.GetArcSegment2dAt(i).GetArea(); ;
                }
            }
            if ((pline.GetBulgeAt(last) != 0.0) && pline.Closed)
            {
                area += pline.GetArcSegment2dAt(last).GetArea();
            }
            return area;
        }
    }
}
