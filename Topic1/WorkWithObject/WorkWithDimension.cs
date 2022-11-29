using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using LibraryCad;

namespace Topic1
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
