using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCad.Models
{
    public class BlockInfo
    {
        public string Name { get; set; }
        public Point3d Location { get; set; }
        public double Rotate { get; set; }
    }
}
