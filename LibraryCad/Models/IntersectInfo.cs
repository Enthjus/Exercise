using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCad.Models
{
    public class IntersectInfo
    {
        public List<double> Intersect { get; set; }
        public Point3dCollection IntersectPts { get; set; }
    }
}
