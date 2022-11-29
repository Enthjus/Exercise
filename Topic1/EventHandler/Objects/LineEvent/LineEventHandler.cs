using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topic1.EventHandler.Objects.LineEvent
{
    public class LineEventHandler
    {
        Line line = null;

        [CommandMethod("AddLObjEvent")]
        public void AddLObjEvent()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
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
                Document doc = Application.DocumentManager.MdiActiveDocument;
                Database db = doc.Database;
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
