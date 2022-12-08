using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;

namespace LibraryCad.DocumentManager
{
    public class DocumentManager
    {
        internal Document doc { get; set; }

        internal Database db { get; set; }

        internal Editor ed { get; set; }
        
        internal DocumentCollectionEventHandler DocChange { get; set; }

        public DocumentManager()
        {
            this.doc = Application.DocumentManager.MdiActiveDocument;
            this.db = doc.Database;
            this.ed = doc.Editor;
            if (this.DocChange != null) return;
            this.DocChange = new DocumentCollectionEventHandler(docColDocAct);
            Application.DocumentManager.DocumentActivated += DocChange;
        }

        public void docColDocAct(object senderObj, DocumentCollectionEventArgs docColDocActEvtArgs)
        {
            this.doc = Application.DocumentManager.MdiActiveDocument;
        }

        public bool IsEventHandlerRegistered(Delegate prospectiveHandler)
        {
            if (this.DocChange != null)
            {
                foreach (Delegate existingHandler in this.DocChange.GetInvocationList())
                {
                    if (existingHandler == prospectiveHandler)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
