using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace AcadProject.AcadManip.WorkWithStyle
{
    public class AtcPointStyle
    {
        [CommandMethod("CreatePoint")]
        public static void CreateStyle()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord tableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                DBPoint point = new DBPoint(new Point3d(4, 4, 0));
                db.Pdmode = 35;
                db.Pdsize = 1;
                tableRec.AppendEntity(point);
                trans.AddNewlyCreatedDBObject(point, true);
                trans.Commit();
            }
        }
    }
}
