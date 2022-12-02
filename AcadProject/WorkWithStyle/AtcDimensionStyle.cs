using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace Topic1.WorkWithStyle
{
    public class AtcDimensionStyle
    {
        [CommandMethod("NewDimStyle")]
        public void NewDimStyle()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DimStyleTable dimStyleTable = trans.GetObject(db.DimStyleTableId, OpenMode.ForRead) as DimStyleTable;
                ObjectId dimId = ObjectId.Null;
                if (!dimStyleTable.Has("Test"))
                {
                    dimStyleTable.UpgradeOpen();
                    DimStyleTableRecord newDimStyle = new DimStyleTableRecord();
                    newDimStyle.Name = "Test";
                    newDimStyle.Dimclrt = Color.FromColorIndex(ColorMethod.ByAci, 2); 
                    dimId = dimStyleTable.Add(newDimStyle);
                    trans.AddNewlyCreatedDBObject(newDimStyle, true);
                }
                else
                {
                    dimId = dimStyleTable["Test"];
                }
                DimStyleTableRecord dimTableRec = trans.GetObject(dimId, OpenMode.ForRead) as DimStyleTableRecord;
                if (dimTableRec.ObjectId != db.Dimstyle)
                {
                    db.Dimstyle = dimTableRec.ObjectId;
                    db.SetDimstyleData(dimTableRec);
                }
                trans.Commit();
            }
        }
    }
}
