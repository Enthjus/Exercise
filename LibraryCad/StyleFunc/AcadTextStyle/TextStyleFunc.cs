using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCad.StyleFunc.AcadTextStyle
{
    public class TextStyleFunc
    {
        public static void ChangeTextSyleInModelSpace(Document doc)
        {
            string oldstyle = "Standard";//<-- Case-sensitive for selection filter!
            string newstyle = "Phuc";
            Editor ed = doc.Editor;
            Database db = doc.Database;

            // build selection filter
            // to select all texts and mtexts in the Model space:
            SelectionFilter sfilter = new SelectionFilter(new TypedValue[]
            {
                new TypedValue (-4,"<AND"),
                new TypedValue(-4,"<OR"),
                new TypedValue(0, "TEXT"),
                new TypedValue(0, "MTEXT"),
                new TypedValue (-4,"OR>"),
                new TypedValue((int)DxfCode.TextStyleName,oldstyle),
                new TypedValue(410, "Model"),
                new TypedValue (-4,"AND>")
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
                        TextStyleTable tt = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                        // check if the new style does exist, if not then exit command
                        if (!tt.Has(newstyle))
                        {
                            Application.ShowAlertDialog("One or both styles does not exist\nProgram exiting...");
                        }
                        ObjectId newId = tt[newstyle];
                        //iterate through the selection set
                        foreach (ObjectId id in ids)
                        {
                            if (id.IsValid && !id.IsErased)
                            {
                                Entity ent = tr.GetObject(id, OpenMode.ForRead, false) as Entity;
                                if (ent.GetType() == typeof(DBText))
                                {
                                    //cat entity as DBText
                                    DBText txt = ent as DBText;

                                    if (txt != null)
                                    {
                                        txt.UpgradeOpen();
                                        txt.TextStyleId = newId;

                                        //(in other version may be
                                        // txt.TextStyle = newId;)

                                        txt.DowngradeOpen();
                                    }
                                }

                                if (ent.GetType() == typeof(MText))
                                {
                                    //cast entity as MText
                                    MText mtxt = ent as MText;
                                    if (mtxt != null)
                                    {
                                        mtxt.UpgradeOpen();
                                        mtxt.TextStyleId = newId;

                                        //(in other version may be
                                        // mtxt.TextStyle = newId;)

                                        mtxt.DowngradeOpen();
                                    }
                                }

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
        public static ObjectId GetStyleId(String styleName, Document doc, string dwtPath)
        {
            ObjectId styleId = ObjectId.Null; ;
            ObjectIdCollection ids = new ObjectIdCollection();
            Database db = doc.Database;
            using (Transaction acTr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable styleTable = (TextStyleTable)acTr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                if (styleTable.Has(styleName))
                {
                    styleId = acTr.GetObject(styleTable[styleName], OpenMode.ForRead).ObjectId;
                    acTr.Commit();
                    return styleId;
                }
                acTr.Commit();
            }
            if (GetStyleFromDWG(styleName, dwtPath, doc))
            {
                using (Transaction acTr = db.TransactionManager.StartTransaction())
                {
                    TextStyleTable styleTable = (TextStyleTable)acTr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                    if (styleTable.Has(styleName))
                    {
                        styleId = acTr.GetObject(styleTable[styleName], OpenMode.ForRead).ObjectId;
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
            using (Database openDb = new Database(false, true))
            {
                openDb.ReadDwgFile(path, System.IO.FileShare.ReadWrite, true, "");

                ObjectIdCollection ids = new ObjectIdCollection();
                using (Transaction tr = openDb.TransactionManager.StartTransaction())
                {
                    var styleTable = (TextStyleTable)tr.GetObject(openDb.TextStyleTableId, OpenMode.ForRead);

                    if (styleTable.Has(styleName))
                    {
                        ids.Add(styleTable[styleName]);
                    }
                    tr.Commit();
                }

                //if found, add the style
                if (ids.Count != 0)
                {
                    //get the current drawing database
                    doc.LockDocument(DocumentLockMode.ProtectedAutoWrite, null, null, true);
                    IdMapping iMap = new IdMapping();
                    db.WblockCloneObjects(ids, db.TextStyleTableId, iMap, DuplicateRecordCloning.Ignore, false);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static List<String> FindAllTextStyle(string path)
        {
            try
            {
                using (Database openDb = new Database(false, true))
                {
                    openDb.ReadDwgFile(path, System.IO.FileShare.ReadWrite, true, "");
                    List<String> styles = new List<String>();
                    using (Transaction tr = openDb.TransactionManager.StartTransaction())
                    {
                        var styleTable = (TextStyleTable)tr.GetObject(openDb.TextStyleTableId, OpenMode.ForRead);

                        foreach(ObjectId id in styleTable)
                        {
                            try
                            {
                                TextStyleTableRecord tableRecord = tr.GetObject(id, OpenMode.ForRead) as TextStyleTableRecord;
                                if (tableRecord != null)
                                {
                                    styles.Add(tableRecord.Name);
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        tr.Commit();
                        return styles;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
