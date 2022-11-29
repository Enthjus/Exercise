using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topic1.EventHandler.Objects.TextEvent
{
    public class TextEventHandler
    {
        DBText dbText = null;

        [CommandMethod("AddTextObjEvent")]
        public void AddCircleObjEvent()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // Tạo bảng để đọc và viết
                BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord tableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                dbText = new DBText();
                dbText.Position = new Point3d(5, 5, 0);
                dbText.Height = 7.0;
                dbText.TextString = "HELLO";
                tableRec.AppendEntity(dbText);
                trans.AddNewlyCreatedDBObject(dbText, true);
                // Bắt sự kiện khi thực hiện thay đổi trên text
                dbText.Modified += new System.EventHandler(TextMod);
                trans.Commit();
            }
        }

        [CommandMethod("RemoveTextObjEvent")]
        public void RemoveCircleObjEvent()
        {
            if (dbText != null)
            {
                Document doc = Application.DocumentManager.MdiActiveDocument;
                Database db = doc.Database;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    dbText = trans.GetObject(dbText.ObjectId, OpenMode.ForRead) as DBText;
                    if (dbText.IsWriteEnabled == false)
                    {
                        trans.GetObject(dbText.ObjectId, OpenMode.ForWrite);
                    }
                    dbText.Modified -= new System.EventHandler(TextMod);
                    dbText = null;
                }
            }
        }

        public void TextMod(object senderObj, EventArgs evtArgs)
        {
            if (dbText.IsErased)
            {
                Application.ShowAlertDialog("The text " + dbText.ToString() + " is deleted!");
            }
            else
            {
                Application.ShowAlertDialog("The text " + dbText.ToString() + " is change to: " + dbText.TextString);
            }
        }
    }
}
