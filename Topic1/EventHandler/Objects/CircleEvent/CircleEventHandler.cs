using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;

namespace Topic1.EventHandler.Objects.CircleEvent
{
    public class CircleEventHandler
    {
        Circle circle = null;

        [CommandMethod("AddCircleObjEvent")]
        public void AddCircleObjEvent()
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

        [CommandMethod("RemoveCircleObjEvent")]
        public void RemoveCircleObjEvent()
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

        public void CircleMod(object senderObj, EventArgs evtArgs)
        {
            if (circle.IsErased)
            {
                Application.ShowAlertDialog("The circle " + circle.ToString() + " is deleted!");
            }
            else
            {
                Application.ShowAlertDialog("The area of " + circle.ToString() + " is: " + circle.Circumference);
            }
        }
    }
}
