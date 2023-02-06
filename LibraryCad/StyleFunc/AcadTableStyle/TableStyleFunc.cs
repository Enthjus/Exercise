using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCad.StyleFunc.AcadTableStyle
{
    public class TableStyleFunc
    {
        public static void ChangeTableStyle(Document doc)
        {
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction trx = db.TransactionManager.StartTransaction())
            {
                BlockTable blkTbl = trx.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord blkTblRec = trx.GetObject(blkTbl[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;
                DBDictionary styleDic = trx.GetObject(db.TableStyleDictionaryId, OpenMode.ForRead) as DBDictionary;
                ObjectIdCollection ids = blkTblRec.GetPersistentReactorIds();
                foreach (ObjectId id in ids)
                {
                    if (id.ObjectClass.IsDerivedFrom(RXClass.GetClass(typeof(Table))))
                    {
                        Table table = (Table)trx.GetObject(id, OpenMode.ForWrite);
                        table.TableStyle = styleDic.GetAt("Phuc");
                    }
                }
                trx.Commit();
            }
        }
    }
}
