using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;
using acad = Autodesk.AutoCAD.ApplicationServices.Application;

namespace Topic1.EventHandler
{
    public class EventHandler
    {
        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

        [CommandMethod("AddAppEvent")]
        public void AddAppEvent()
        {
            acad.SystemVariableChanged += new Autodesk.AutoCAD.ApplicationServices.SystemVariableChangedEventHandler(appSysVarChanged);
        }

        [CommandMethod("RemoveAppEvent")]
        public void RemoveAppEvent()
        {
            acad.SystemVariableChanged -= new Autodesk.AutoCAD.ApplicationServices.SystemVariableChangedEventHandler(appSysVarChanged);
        }

        public void appSysVarChanged(object senderObj, Autodesk.AutoCAD.ApplicationServices.SystemVariableChangedEventArgs sysVarChEvtArgs)
        {
            object oVal = acad.GetSystemVariable(sysVarChEvtArgs.Name);

            // Display a message box with the system variable name and the new value
            acad.ShowAlertDialog(sysVarChEvtArgs.Name + " was changed." + "\nNew value: " + oVal.ToString());
        }

        [CommandMethod("AddDocColEvent")]
        public void AddDocColEvent()
        {
            acad.DocumentManager.DocumentActivated += new DocumentCollectionEventHandler(docColDocAct);
        }

        [CommandMethod("RemoveDocColEvent")]
        public void RemoveDocColEvent()
        {
            acad.DocumentManager.DocumentActivated -= new DocumentCollectionEventHandler(docColDocAct);
        }

        public void docColDocAct(object senderObj, DocumentCollectionEventArgs docColDocActEvtArgs)
        {
            acad.ShowAlertDialog(docColDocActEvtArgs.Document.Name + " was activated.");
        }

        [CommandMethod("AddDocEvent")]
        public void AddDocEvent()
        {
            doc.BeginDocumentClose += new DocumentBeginCloseEventHandler(docBeginDocClose);
        }

        [CommandMethod("RemoveDocEvent")]
        public void RemoveDocEvent()
        {
            doc.BeginDocumentClose -= new DocumentBeginCloseEventHandler(docBeginDocClose);
        }

        public void docBeginDocClose(object senderObj, DocumentBeginCloseEventArgs docBegClsEvtArgs)
        {
            // Display a message box prompting to continue closing the document
            if (System.Windows.Forms.MessageBox.Show("The document is about to be closed." + "\nDo you want to continue?", "Close Document", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
            {
                docBegClsEvtArgs.Veto();
            }
        }
    }
}
