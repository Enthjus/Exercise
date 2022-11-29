using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using LibraryCad;
using LibraryCad.BlockObject;

namespace Topic1.WorkWithObject
{
    public class WorkWithBlock
    {
        [CommandMethod("CreateBlock")]
        public static void CreateBlock()
        {
            BlockFunc.CreateRectangleBlock();
        }

        [CommandMethod("CopyBlocksBetweenDatabases", CommandFlags.Session)]
        public static void CopyBlocksBetweenDatabases()
        {
            ObjectIdCollection objIdColl = new ObjectIdCollection();
            Document doc = Application.DocumentManager.MdiActiveDocument;
            // List thông tin các khối
            var blockInfos = BlockFunc.PickBlock(doc, objIdColl);
            if (blockInfos.Count == 0) return;
            // Lấy đường dẫn của file muốn thao tác
            string localRoot = Application.GetSystemVariable("LOCALROOTPREFIX") as string;
            string filePath = localRoot + "Template\\Test.dwg";
            // Copy các block mới chọn sang database mới theo đường dẫn truyền vào
            BlockFunc.CloneBlockToAnotherDatabase(doc, filePath, objIdColl, blockInfos);
        }

        [CommandMethod("CopyObjectsBetweenDatabases", CommandFlags.Session)]
        public static void CopyObjectsBetweenDatabases()
        {
            ObjectIdCollection objIdColl = new ObjectIdCollection();
            Document doc = Application.DocumentManager.MdiActiveDocument;
            SubFunc.PickAllObject(doc, objIdColl);
            if(objIdColl.Count == 0) return;
            SubFunc.CLoneObjectToDatabase(doc, objIdColl, "D:\\TestFile1.dwg");
        }

        [CommandMethod("CopyFileWithText")]
        public static void CopyFileWithText()
        {
            FileFunc.CreateFileWithText("C:\\Users\\OMEN\\AppData\\Local\\Autodesk\\AutoCAD 2021\\R24.0\\enu\\Template\\Test.dwg");
        }
    }
}
