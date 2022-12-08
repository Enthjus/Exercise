using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.ObjectsFunc.ArcObject;
using LibraryCad.ObjectsFunc.DimensionObject;

namespace Topic1.WorkWithObject
{
    public class WorkWithArc
    {
        [CommandMethod("DimMArc")]
        public static void DimMultiArc()
        {
            try
            {
                Document doc = Application.DocumentManager.MdiActiveDocument;
                var arcs = ArcFunc.SelectionSetToListArc(doc);
                DimensionFunc.DimMultiArc(arcs, doc);
            }
            catch
            {
                return;
            }
        }
    }
}
