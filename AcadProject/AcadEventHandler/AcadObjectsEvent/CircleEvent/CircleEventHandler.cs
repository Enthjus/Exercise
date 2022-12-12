using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;
using LibraryCad.ObjectsFunc.CircleObject;
using System;
using System.Linq;

namespace AcadProject.AcadEventHandler.AcadObjectsEvent.CircleEvent
{
    public class CircleEventHandler
    {
        Circle circle = null;

        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

        [CommandMethod("AddCircleModifyEvent")]
        public void AddCircleModifyEvent()
        {
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
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                var cirId = CircleFunc.CircleSelect(doc);
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
