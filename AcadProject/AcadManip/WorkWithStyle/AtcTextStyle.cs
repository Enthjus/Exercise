using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace AcadProject.AcadManip.WorkWithStyle
{
    public class AtcTextStyle
    {
        [CommandMethod("CreateStyle")]
        public static void CreateStyle()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord tableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                TextStyleTable textStyleTable = trans.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                if (!textStyleTable.Has("ROMANS"))
                {
                    textStyleTable.UpgradeOpen();
                    TextStyleTableRecord textStyleRecord = new TextStyleTableRecord();
                    textStyleRecord.Name = "ROMANS";
                    textStyleRecord.FileName = "romans.shx";
                    textStyleTable.Add(textStyleRecord);
                    trans.AddNewlyCreatedDBObject(textStyleRecord, true);
                }
                DBText dbText = new DBText();
                dbText.SetDatabaseDefaults();
                dbText.Position = new Point3d(5, 5, 0);
                dbText.Height = 7.0;
                dbText.TextString = "HELLO";
                dbText.TextStyleId = textStyleTable["ROMANS"];
                tableRec.AppendEntity(dbText);
                trans.AddNewlyCreatedDBObject(dbText, true);
                trans.Commit();
            }
        }
    }
}
