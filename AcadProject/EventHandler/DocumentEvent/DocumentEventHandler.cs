using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topic1.EventHandler.DocumentEvent
{
    public class DocumentEventHandler
    {
        private static DocumentCollection docEvents;

        public void docEvents_DocumentCreated(object sender, DocumentCollectionEventArgs evtArgs)
        {
            Application.ShowAlertDialog(evtArgs.Document.Name + " opened!");
        }

        private static DocumentDestroyedEventHandler docDestroyedEvent;

        public void OnDocumentDestroyed(object sender, DocumentDestroyedEventArgs evtArgs)
        {
            Application.ShowAlertDialog(evtArgs.FileName + " closed!");
        }

        [CommandMethod("StartEventHandling")]
        public void StartEventHandling()
        {
            docEvents = Application.DocumentManager;
            docEvents.DocumentCreated += new DocumentCollectionEventHandler(docEvents_DocumentCreated);
            DocumentCollection docs = Application.DocumentManager;
            if (docDestroyedEvent == null)
            {
                docDestroyedEvent = new DocumentDestroyedEventHandler(OnDocumentDestroyed);
                docs.DocumentDestroyed += docDestroyedEvent;
            }
        }

        [CommandMethod("TestDocCollectionEvents")]
        public void TestDocCollectionEvents()
        {
            DocumentEventHandler documentEvent = new DocumentEventHandler();
            documentEvent.Register(Application.DocumentManager);
        }

        public void Register(DocumentCollection _docEvents)
        {
            docEvents = _docEvents;
            docEvents.DocumentCreated += new DocumentCollectionEventHandler(docEvents_DocumentCreated);
        }
    }
}
