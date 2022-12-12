using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;

namespace AcadProject.AcadEventHandler.AcadObjectsEvent.PolylineEvent
{
    public class PolylineEventHandler
    {
        Polyline pline = null;

        [CommandMethod("AddPlObjEvent")]
        public void AddPlObjEvent()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // Tạo bảng để đọc và viết
                BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord tableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                // Tạo polyline đóng
                pline = new Polyline();
                pline.AddVertexAt(0, new Point2d(1, 1), 0, 0, 0);
                pline.AddVertexAt(1, new Point2d(1, 2), 0, 0, 0);
                pline.AddVertexAt(2, new Point2d(2, 2), 0, 0, 0);
                pline.AddVertexAt(3, new Point2d(3, 3), 0, 0, 0);
                pline.AddVertexAt(4, new Point2d(3, 2), 0, 0, 0);
                pline.Closed = true;
                tableRec.AppendEntity(pline);
                trans.AddNewlyCreatedDBObject(pline, true);
                // Bắt sự kiện khi thực hiện thay đổi trên polyline
                pline.Modified += new System.EventHandler(PolyMod);
                trans.Commit();
            }
        }

        [CommandMethod("RemovePlObjEvent")]
        public void RemovePlObjEvent()
        {
            if (pline != null)
            {
                Document doc = Application.DocumentManager.MdiActiveDocument;
                Database db = doc.Database;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    pline = trans.GetObject(pline.ObjectId, OpenMode.ForRead) as Polyline;
                    if (pline.IsWriteEnabled == false)
                    {
                        trans.GetObject(pline.ObjectId, OpenMode.ForWrite);
                    }
                    // Thêm bắt sự kiện lúc thay đổi
                    pline.Modified -= new System.EventHandler(PolyMod);
                    pline = null;
                }
            }
        }

        public void PolyMod(object senderObj, EventArgs evtArgs)
        {
            if (pline.IsErased)
            {
                Application.ShowAlertDialog("The polyline " + pline.ToString() + " is deleted!");
            }
            else
            {
                Application.ShowAlertDialog("The area of " + pline.ToString() + " is: " + pline.Area);
            }
        }
    }
}
