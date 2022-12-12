using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;
using System;

namespace AcadProject.AcadEventHandler.AcadObjectsEvent.LineEvent
{
    public class LineEventHandler
    {
        Line line = null;

        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

        [CommandMethod("AddLObjEvent")]
        public void AddLObjEvent()
        {
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // Tạo bảng để đọc và viết
                BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord tableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                line = new Line(new Point3d(1, 1, 0), new Point3d(3, 2, 0));
                tableRec.AppendEntity(line);
                trans.AddNewlyCreatedDBObject(line, true);
                // Bắt sự kiện khi thực hiện thay đổi trên line
                line.Modified += new System.EventHandler(LineMod);
                trans.Commit();
            }
        }

        [CommandMethod("RemoveLObjEvent")]
        public void RemoveLObjEvent()
        {
            if (line != null)
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    line = trans.GetObject(line.ObjectId, OpenMode.ForRead) as Line;
                    if (line.IsWriteEnabled == false)
                    {
                        trans.GetObject(line.ObjectId, OpenMode.ForWrite);
                    }
                    line.Modified -= new System.EventHandler(LineMod);
                    line = null;
                }
            }
        }

        public void LineMod(object senderObj, EventArgs evtArgs)
        {
            if (line.IsErased)
            {
                Application.ShowAlertDialog("The line " + line.ToString() + " is deleted!");
            }
            else
            {
                Application.ShowAlertDialog("The area of " + line.ToString() + " is: " + line.Length);
            }
        }
    }
}
