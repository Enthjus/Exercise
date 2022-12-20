using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;

namespace LibraryCad.DocumentManager
{
    public static class DocumentManager
    {
        public static Document doc { get; set; }

        public static Database db { get; set; }

        public static Editor ed { get; set; }

        public static DocumentCollectionEventHandler DocChange { get; set; }

        public static void CreateDocument()
        {
            doc = Application.DocumentManager.MdiActiveDocument;
            db = doc.Database;
            ed = doc.Editor;
            if (DocChange != null) return;
            DocChange = new DocumentCollectionEventHandler(docColDocAct);
            Application.DocumentManager.DocumentActivated += DocChange;
        }

        public static void docColDocAct(object senderObj, DocumentCollectionEventArgs docColDocActEvtArgs)
        {
            doc = Application.DocumentManager.CurrentDocument;
            if(doc == null) return;
            db = doc.Database;
            ed = doc.Editor;
        }

        public static bool IsEventHandlerRegistered(Delegate prospectiveHandler)
        {
            if (DocChange != null)
            {
                foreach (Delegate existingHandler in DocChange.GetInvocationList())
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
