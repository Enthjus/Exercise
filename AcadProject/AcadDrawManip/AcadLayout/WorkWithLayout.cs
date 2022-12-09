using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;

namespace Topic1.AcadDrawManip.AcadLayout
{
    public class WorkWithLayout
    {
        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

        [CommandMethod("LayoutList")]
        public static void LayoutList()
        {
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DBDictionary layouts = trans.GetObject(db.LayoutDictionaryId, OpenMode.ForRead) as DBDictionary;
                ed.WriteMessage("\nLayouts: ");
                foreach (var layout in layouts)
                {
                    ed.WriteMessage("\n " + layout.Key);
                }
            }
        }
    }
}
