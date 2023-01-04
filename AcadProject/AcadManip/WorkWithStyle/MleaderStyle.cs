using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadProject.AcadManip.WorkWithStyle
{
    public class MleaderStyle
    {
        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;
        
        private static Editor ed = DocumentManager.ed;

        [CommandMethod("CMlea")]
        public static void Creaete_MleaderStyle()
        {
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // Lập biến để lưu ObjectId của Mleader style lấy được
                ObjectId mlStyleId;
                // Lấy ra Mleader Style Dictionary của bản vẽ
                DBDictionary mlStyleDict = trans.GetObject(db.MLeaderStyleDictionaryId, OpenMode.ForRead) as DBDictionary;
                if (mlStyleDict.Contains("My_MleaderStyle"))
                {
                    mlStyleId = (ObjectId)mlStyleDict["My_MleaderStyle"];
                }
                else
                {
                    MLeaderStyle New_MLstyle = new MLeaderStyle();
                    mlStyleId = New_MLstyle.PostMLeaderStyleToDb(db, "My_MleaderStyle");
                    trans.AddNewlyCreatedDBObject(New_MLstyle, true);
                }
                // Đưa Mleader style mới về hiện hành
                db.MLeaderstyle = mlStyleId;
                // Lưu các thay đổi và đóng transaction
                trans.Commit();
            }
        }
    }
}
