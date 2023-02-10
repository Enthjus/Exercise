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

        /// <summary>
        /// Checks if the style definition is in the active drawing, if not, pull it from the template and add it
        /// </summary>
        /// <param name="styleName">Name of the style.</param>
        /// <returns>ObjectID of Textstyle, ObjectId.Null on error</returns>
        public static ObjectId GetStyleId(String styleName, Document doc, string path)
        {
            ObjectId styleId = ObjectId.Null; ;
            Database db = doc.Database;
            using (Transaction acTr = db.TransactionManager.StartTransaction())
            {
                DBDictionary styleTable = (DBDictionary)acTr.GetObject(db.TableStyleDictionaryId, OpenMode.ForRead);
                if (styleTable.Contains(styleName))
                {
                    styleId = acTr.GetObject(styleTable.GetAt(styleName), OpenMode.ForRead).ObjectId;
                    acTr.Commit();
                    return styleId;
                }
                acTr.Commit();
            }
            if (GetStyleFromDWG(styleName, path, doc))
            {
                using (Transaction acTr = db.TransactionManager.StartTransaction())
                {
                    DBDictionary styleTable = (DBDictionary)acTr.GetObject(db.TableStyleDictionaryId, OpenMode.ForRead);
                    if (styleTable.Contains(styleName))
                    {
                        styleId = acTr.GetObject(styleTable.GetAt(styleName), OpenMode.ForRead).ObjectId;
                    }
                    acTr.Commit();
                    return styleId;
                }
            }
            // Anything below here is an error, and returns ObjectID.Null
            doc.Editor.WriteMessage("\nError creating text style: " + styleName);
            return styleId;
        }



        /// <summary>
        /// Gets the textstyle from specified drawing and adds it to the TextsStyleTable of the active drawing.
        /// Returns true if successful, returns false if style isn't found.
        /// </summary>
        /// <param name="styleName">The stylename.</param>
        /// <param name="path">The path to dwg/dwt to extract style from.</param>
        private static bool GetStyleFromDWG(string styleName, string path, Document doc)
        {
            Database db = doc.Database;
            using (Database openDb = new Database())
            {
                openDb.ReadDwgFile(path, System.IO.FileShare.Read, false, "");
                ObjectIdCollection ids = new ObjectIdCollection();
                using (Transaction tr = openDb.TransactionManager.StartTransaction())
                {
                    var styleTable = (DBDictionary)tr.GetObject(openDb.TableStyleDictionaryId, OpenMode.ForRead);

                    if (styleTable.Contains(styleName))
                    {
                        ids.Add(styleTable.GetAt(styleName));
                    }
                    tr.Commit();
                }

                //if found, add the style
                if (ids.Count != 0)
                {
                    //get the current drawing database
                    doc.LockDocument(DocumentLockMode.ProtectedAutoWrite, null, null, true);
                    IdMapping iMap = new IdMapping();
                    db.WblockCloneObjects(ids, db.TableStyleDictionaryId, iMap, DuplicateRecordCloning.Ignore, false);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
