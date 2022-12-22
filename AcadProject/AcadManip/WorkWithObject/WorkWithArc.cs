using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;
using LibraryCad.ObjectsFunc.ArcObject;
using LibraryCad.ObjectsFunc.DimensionObject;
using LibraryCad.Sub;

namespace AcadProject.AcadManip.WorkWithObject
{
    public class WorkWithArc
    {
        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

        [CommandMethod("DimMArc")]
        public static void DimMultiArc()
        {
            try
            {
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
