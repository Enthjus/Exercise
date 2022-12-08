using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;
using LibraryCad.FileManip.Plot;
using LibraryCad.Sub;

namespace Topic1.Plotting
{
    public class PlotFromModelSpace
    {
        [CommandMethod("PlotCurrentLayout")]
        public static void PlotCurrentLayout()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            var fileName = SubFunc.GetString(doc, "- Nhập tên file muồn xuất ra: ");
            PlotFunc.PlotModel(doc, fileName);
        }
    }
}
