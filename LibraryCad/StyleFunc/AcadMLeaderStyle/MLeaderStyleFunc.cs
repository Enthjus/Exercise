using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCad.StyleFunc.AcadMLeaderStyle
{
    public class MLeaderStyleFunc
    {
        public static void ChangeMLeaderStyle(Document doc)
        {
            var db = doc.Database;
            var ed = doc.Editor;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TypedValue[] tv = { new TypedValue(0, "MULTILEADER") };
                PromptSelectionResult sel = ed.SelectAll(new SelectionFilter(tv));
                if (sel.Status == PromptStatus.OK)
                {
                    foreach (ObjectId obj in sel.Value.GetObjectIds())
                    {
                        MLeader mld = (MLeader)tr.GetObject(obj, OpenMode.ForRead);
                        MLeaderStyle mls = (MLeaderStyle)tr.GetObject(mld.MLeaderStyle, OpenMode.ForRead);
                        mls.Name = "Phuc";
                    }
                }
                tr.Commit();
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
                DBDictionary styleTable = (DBDictionary)acTr.GetObject(db.MLeaderStyleDictionaryId, OpenMode.ForRead);
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
                    DBDictionary styleTable = (DBDictionary)acTr.GetObject(db.MLeaderStyleDictionaryId, OpenMode.ForRead);
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
                    var styleTable = (DBDictionary)tr.GetObject(openDb.MLeaderStyleDictionaryId, OpenMode.ForRead);

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
                    db.WblockCloneObjects(ids, db.MLeaderStyleDictionaryId, iMap, DuplicateRecordCloning.Ignore, false);
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
