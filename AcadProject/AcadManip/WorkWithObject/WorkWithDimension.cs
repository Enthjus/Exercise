using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.ObjectsFunc.DimensionObject;

namespace AcadProject.AcadManip.WorkWithObject
{
    public class WorkWithDimension
    {
        [CommandMethod("DimensionSum")]
        public static void DimensionSum()
        {
            Document doc = Application.DocumentManager.CurrentDocument;
            DimensionFunc.DimensionSum(doc);
        }

        [CommandMethod("CreateRotatedDimension")]
        public static void CreateRotatedDimension()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            DimensionFunc.CreateRotateDim(doc);
        }
    }
}
