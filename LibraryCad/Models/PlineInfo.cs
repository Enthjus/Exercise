using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
