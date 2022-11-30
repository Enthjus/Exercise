using Autodesk.AutoCAD.Geometry;

namespace LibraryCad.Models
{
    public class BlockInfo
    {
        public string Name { get; set; }
        public Point3d Location { get; set; }
        public double Rotate { get; set; }
    }
}
