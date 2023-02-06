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
    }
}
