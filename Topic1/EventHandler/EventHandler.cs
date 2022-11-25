using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;

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
            Application.ShowAlertDialog(docColDocActEvtArgs.Document.Name +  " was activated.");
        }

        [CommandMethod("AddDocEvent")]
        public void AddDocEvent()
        {
            // Get the current document
            Document acDoc = Application.DocumentManager.MdiActiveDocument;

            acDoc.BeginDocumentClose += new DocumentBeginCloseEventHandler(docBeginDocClose);
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

        // Global variable for polyline object
        Polyline acPoly = null;

        [CommandMethod("AddPlObjEvent")]
        public void AddPlObjEvent()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                acPoly = new Polyline();
                acPoly.AddVertexAt(0, new Point2d(1, 1), 0, 0, 0);
                acPoly.AddVertexAt(1, new Point2d(1, 2), 0, 0, 0);
                acPoly.AddVertexAt(2, new Point2d(2, 2), 0, 0, 0);
                acPoly.AddVertexAt(3, new Point2d(3, 3), 0, 0, 0);
                acPoly.AddVertexAt(4, new Point2d(3, 2), 0, 0, 0);
                acPoly.Closed = true;
                acBlkTblRec.AppendEntity(acPoly);
                acTrans.AddNewlyCreatedDBObject(acPoly, true);
                acPoly.Modified += new System.EventHandler(acPolyMod);
                acTrans.Commit();
            }
        }

        [CommandMethod("RemovePlObjEvent")]
        public void RemovePlObjEvent()
        {
            if (acPoly != null)
            {
                Document acDoc = Application.DocumentManager.MdiActiveDocument;
                Database acCurDb = acDoc.Database;
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    acPoly = acTrans.GetObject(acPoly.ObjectId, OpenMode.ForRead) as Polyline;
                    if (acPoly.IsWriteEnabled == false)
                    {
                        acTrans.GetObject(acPoly.ObjectId, OpenMode.ForWrite);
                    }
                    acPoly.Modified -= new System.EventHandler(acPolyMod);
                    acPoly = null;
                }
            }
        }

        public void acPolyMod(object senderObj, EventArgs evtArgs)
        {
            if(!acPoly.IsErased)
            {
                Application.ShowAlertDialog("The area of " + acPoly.ToString() + " is: " + acPoly.Area);
            }
            else
            {
                Application.ShowAlertDialog("The polyline " + acPoly.ToString() + " is deleted!");
            }
        }
    }
}
