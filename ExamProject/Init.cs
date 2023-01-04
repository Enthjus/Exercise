using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;
using System;

namespace ExamProject
{
    public class Init : IExtensionApplication
    {
        public void Initialize()
        {
            DocumentManager.CreateDocument();
            //throw new System.NotImplementedException();
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            ed.WriteMessage("Load thanh cong");
        }

        public void Terminate()
        {
            throw new NotImplementedException();
        }
    }
}
