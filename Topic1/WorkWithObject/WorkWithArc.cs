using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topic1.WorkWithObject
{
    public class WorkWithArc
    {
        [CommandMethod("DimMArc")]
        public static void DimMultiArc()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;

            var arcs = LibraryCad.Arc.ArcFunc.SelectionSetToListArc(doc);

            LibraryCad.DimensionFunc.DimMultiArc(arcs, doc);
        }
    }
}
