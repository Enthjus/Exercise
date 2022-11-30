using Autodesk.AutoCAD.Geometry;

namespace LibraryCad.Models
{
    public class PlineInfo
    {
        public Point3d StartPoint { get; set; }
        public Point3d EndPoint { get; set; }
        public CircularArc3d CircularArc { get; set; }
        public bool Status { get; set; }
    }
}
