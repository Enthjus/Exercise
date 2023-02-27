using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;

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

        public static void ChangeTableSyleInModelSpace(Document doc, ObjectId styleId)
        {
            string oldstyle = "Standard";//<-- Case-sensitive for selection filter!
            Editor ed = doc.Editor;
            Database db = doc.Database;

            // build selection filter
            // to select all texts and mtexts in the Model space:
            SelectionFilter sfilter = new SelectionFilter(new TypedValue[]
            {
                new TypedValue((int)DxfCode.Start, "ACAD_TABLE")
            });

            // request for objects to be selected all in the  model space
            PromptSelectionResult res = ed.SelectAll(sfilter);
            try
            {
                if (res.Status == PromptStatus.OK)
                {
                    ed.WriteMessage("\nSelected {0} objects", res.Value.Count);
                    using (Transaction tr = doc.TransactionManager.StartTransaction())
                    {
                        ObjectId[] ids = res.Value.GetObjectIds();
                        // get text style table
                        DBDictionary dic = tr.GetObject(db.TableStyleDictionaryId, OpenMode.ForRead) as DBDictionary;
                        // check if the new style does exist, if not then exit command
                        if (!dic.Contains(styleId))
                        {
                            Application.ShowAlertDialog("One or both styles does not exist\nProgram exiting...");
                        }
                        ObjectId newId = styleId;
                        //iterate through the selection set
                        foreach (ObjectId id in ids)
                        {
                            if (id.IsValid && !id.IsErased)
                            {
                                Table table = tr.GetObject(id, OpenMode.ForRead, false) as Table;
                                //if (ent.GetType() == typeof(Dimension))
                                //{
                                //cat entity as DBText
                                //Dimension dim = ent as Dimension;

                                if (table != null)
                                {
                                    table.UpgradeOpen();
                                    table.TableStyle = newId;

                                    //(in other version may be
                                    // txt.TextStyle = newId;)

                                    table.DowngradeOpen();
                                }
                                //}
                            }
                        }

                        tr.Commit();
                    }
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                ed.WriteMessage("\n" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Checks if the style definition is in the active drawing, if not, pull it from the template and add it
        /// </summary>
        /// <param name="styleName">Name of the style.</param>
        /// <returns>ObjectID of Textstyle, ObjectId.Null on error</returns>
        public static ObjectId GetStyleId(String styleName, Document doc, string path)
        {
            ObjectId styleId = ObjectId.Null;
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
