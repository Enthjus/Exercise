using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;

namespace Topic1.EventHandler
{
    public class EventHandler
    {
        [CommandMethod("AddAppEvent")]
        public void AddAppEvent()
        {
            Application.SystemVariableChanged +=
                new Autodesk.AutoCAD.ApplicationServices.
                    SystemVariableChangedEventHandler(appSysVarChanged);
        }

        [CommandMethod("RemoveAppEvent")]
        public void RemoveAppEvent()
        {
            Application.SystemVariableChanged -=
                new Autodesk.AutoCAD.ApplicationServices.
                    SystemVariableChangedEventHandler(appSysVarChanged);
        }

        public void appSysVarChanged(object senderObj, Autodesk.AutoCAD.ApplicationServices.SystemVariableChangedEventArgs sysVarChEvtArgs)
        {
            object oVal = Application.GetSystemVariable(sysVarChEvtArgs.Name);

            // Display a message box with the system variable name and the new value
            Application.ShowAlertDialog(sysVarChEvtArgs.Name + " was changed." +
                                        "\nNew value: " + oVal.ToString());
        }

        [CommandMethod("AddDocColEvent")]
        public void AddDocColEvent()
        {
            Application.DocumentManager.DocumentActivated +=
                new DocumentCollectionEventHandler(docColDocAct);
        }

        [CommandMethod("RemoveDocColEvent")]
        public void RemoveDocColEvent()
        {
            Application.DocumentManager.DocumentActivated -=
                new DocumentCollectionEventHandler(docColDocAct);
        }

        public void docColDocAct(object senderObj, DocumentCollectionEventArgs docColDocActEvtArgs)
        {
            Application.ShowAlertDialog(docColDocActEvtArgs.Document.Name + " was activated.");
        }

        [CommandMethod("AddDocEvent")]
        public void AddDocEvent()
        {
            // Get the current document
            Document doc = Application.DocumentManager.MdiActiveDocument;
            doc.BeginDocumentClose += new DocumentBeginCloseEventHandler(docBeginDocClose);
        }

        [CommandMethod("RemoveDocEvent")]
        public void RemoveDocEvent()
        {
            // Get the current document
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            acDoc.BeginDocumentClose -= new DocumentBeginCloseEventHandler(docBeginDocClose);
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
