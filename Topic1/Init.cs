using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

//[assembly: CommandMethod(In)]
namespace Topic1
{
    public class Init : IExtensionApplication
    {
        public void Initialize()
        {
            //throw new System.NotImplementedException();
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            ed.WriteMessage("Load thanh cong");
        }

        public void Terminate()
        {
            //throw new System.NotImplementedException();
            // Cac ham su ly khi thoat CAD.
        }
    }
}
