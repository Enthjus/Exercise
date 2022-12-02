using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using LibraryCad;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Topic1.EventHandler.Objects.CircleEvent
{
    public class CircleEventHandler
    {
        Circle circle = null;

        [CommandMethod("AddCircleModifyEvent")]
        public void AddCircleModifyEvent()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // Tạo bảng để đọc và viết
                BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord tableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                circle = new Circle();
                circle.SetDatabaseDefaults();
                circle.Center = new Point3d(5, 5, 0);
                circle.Radius = 5;
                tableRec.AppendEntity(circle);
                trans.AddNewlyCreatedDBObject(circle, true);
                // Bắt sự kiện khi thực hiện thay đổi trên đường tròn
                circle.Modified += new System.EventHandler(CircleMod);
                trans.Commit();
            }
        }

        [CommandMethod("RemoveCircleModifyEvent")]
        public void RemoveCircleModifyEvent()
        {
            if (circle != null)
            {
                Document doc = Application.DocumentManager.MdiActiveDocument;
                Database db = doc.Database;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    circle = trans.GetObject(circle.ObjectId, OpenMode.ForRead) as Circle;
                    if (circle.IsWriteEnabled == false)
                    {
                        trans.GetObject(circle.ObjectId, OpenMode.ForWrite);
                    }
                    circle.Modified -= new System.EventHandler(CircleMod);
                    circle = null;
                }
            }
        }

        [CommandMethod("AddCircleDeleteEvent")]
        public void AddCircleDeleteEvent()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // Tạo bảng để đọc và viết
                BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord tableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                circle = new Circle();
                circle.SetDatabaseDefaults();
                circle.Center = new Point3d(5, 5, 0);
                circle.Radius = 5;
                tableRec.AppendEntity(circle);
                trans.AddNewlyCreatedDBObject(circle, true);
                // Bắt sự kiện khi thực hiện thay đổi trên đường tròn
                circle.Erased += new ObjectErasedEventHandler(CircleDelete);
                trans.Commit();
            }
        }

        [CommandMethod("RemoveCircleDeleteEvent")]
        public void RemoveCircleDeleteEvent()
        {
            if (circle != null)
            {
                Document doc = Application.DocumentManager.MdiActiveDocument;
                Database db = doc.Database;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    circle = trans.GetObject(circle.ObjectId, OpenMode.ForRead) as Circle;
                    if (circle.IsWriteEnabled == false)
                    {
                        trans.GetObject(circle.ObjectId, OpenMode.ForWrite);
                    }
                    circle.Erased -= new ObjectErasedEventHandler(CircleDelete);
                    circle = null;
                }
            }
        }

        [CommandMethod("AddCircleCopyEvent")]
        public void AddCircleCopyEvent()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                var cirId = CircleSelect();
                if (cirId == ObjectId.Null) return;
                circle = trans.GetObject(cirId, OpenMode.ForRead) as Circle;
                // Bắt sự kiện khi thực hiện thay đổi trên đường tròn
                circle.Copied += new ObjectEventHandler(CircleClone);
                trans.Commit();
            }
        }

        [CommandMethod("RemoveCircleCopyEvent")]
        public void RemoveCircleCopyEvent()
        {
            if (circle != null)
            {
                Document doc = Application.DocumentManager.MdiActiveDocument;
                Database db = doc.Database;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    circle = trans.GetObject(circle.ObjectId, OpenMode.ForRead) as Circle;
                    if (circle.IsWriteEnabled == false)
                    {
                        trans.GetObject(circle.ObjectId, OpenMode.ForWrite);
                    }
                    circle.Copied -= new ObjectEventHandler(CircleClone);
                    circle = null;
                }
            }
        }

        public static ObjectId CircleSelect()
        {
            try
            {
                Document doc = Application.DocumentManager.MdiActiveDocument;
                PromptSelectionOptions prSelOptions = new PromptSelectionOptions();
                prSelOptions.SingleOnly = true;
                prSelOptions.SinglePickInSpace = true;
                TypedValue[] tvCircle = new TypedValue[]
                {
                new TypedValue((int)DxfCode.Start, "CIRCLE")
                };
                SelectionFilter filter = new SelectionFilter(tvCircle);
                PromptSelectionResult prSelResult = doc.Editor.GetSelection(prSelOptions, filter);
                if (prSelResult.Status == PromptStatus.OK)
                {
                    return prSelResult.Value.OfType<SelectedObject>().First().ObjectId;
                }
                return ObjectId.Null;
            }
            catch
            {
                return ObjectId.Null;
            }
        }

        public void CircleMod(object senderObj, EventArgs evtArgs)
        {
            Application.ShowAlertDialog("The perimeter of " + circle.ToString() + " is: " + circle.Circumference);
            if (circle.IsErased) circle = null;
        }

        public void CircleDelete(object senderObj, ObjectErasedEventArgs evtArgs)
        {
            Application.ShowAlertDialog("The circle " + evtArgs.DBObject + " have been deleted!");
            circle = null;
        }

        public void CircleClone(object senderObj, EventArgs evtArgs)
        {
            Application.ShowAlertDialog("The circle " + circle.ToString() + " have been cloned!");
        }
    }
}
