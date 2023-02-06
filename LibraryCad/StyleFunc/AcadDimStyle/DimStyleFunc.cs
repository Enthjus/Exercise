using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

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
    }
}
