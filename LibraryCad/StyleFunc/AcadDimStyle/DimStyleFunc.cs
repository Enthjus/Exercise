using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;

namespace LibraryCad.StyleFunc.AcadDimStyle
{
    public class DimStyleFunc
    {
        public static void ChangeDimStlye(Document doc)
        {
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction trx = db.TransactionManager.StartTransaction())
            {
                DimStyleTable dimTbl = (DimStyleTable)trx.GetObject(db.DimStyleTableId, OpenMode.ForRead);
                DimStyleTableRecord dimDtr = (DimStyleTableRecord)trx.GetObject(dimTbl["Phuc"], OpenMode.ForRead);
                ObjectIdCollection ids = dimDtr.GetPersistentReactorIds();
                foreach (ObjectId id in ids)
                {
                    if (id.ObjectClass.IsDerivedFrom(RXClass.GetClass(typeof(Dimension))))
                    {
                        Dimension dim = (Dimension)trx.GetObject(id, OpenMode.ForWrite);
                        dim.DimensionStyle = dimTbl["Phuc"];
                        //dim.DimensionStyleName = "DimStyle2";
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
                DimStyleTable styleTable = (DimStyleTable)acTr.GetObject(db.DimStyleTableId, OpenMode.ForRead);
                if (styleTable.Has(styleName))
                {
                    styleId = acTr.GetObject(styleTable[styleName], OpenMode.ForRead).ObjectId;
                    acTr.Commit();
                    return styleId;
                }
                acTr.Commit();
            }
            if (GetStyleFromDWG(styleName, path, doc))
            {
                using (Transaction acTr = db.TransactionManager.StartTransaction())
                {
                    DimStyleTable styleTable = (DimStyleTable)acTr.GetObject(db.DimStyleTableId, OpenMode.ForRead);
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
            using (Database openDb = new Database())
            {
                openDb.ReadDwgFile(path, System.IO.FileShare.Read, false, "");
                ObjectIdCollection ids = new ObjectIdCollection();
                using (Transaction tr = openDb.TransactionManager.StartTransaction())
                {
                    var styleTable = (DimStyleTable)tr.GetObject(openDb.DimStyleTableId, OpenMode.ForRead);

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
                    db.WblockCloneObjects(ids, db.DimStyleTableId, iMap, DuplicateRecordCloning.Ignore, false);
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
