using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace Topic1.WorkWithFile
{
    public class WorkWithFile
    {
        private static string[] filepaths = new string[]
        {
            "E:\\BanVe\\DWG\\02. B2F ~ Block B ~ General Note.dwg",
            "E:\\BanVe\\DWG\\03. B2F ~ Block B ~ Lower slab at B2F_Segment.dwg",
            "E:\\BanVe\\DWG\\04. B2F ~ Block B ~ General Layout Sec 1-3.dwg"
        };

        [CommandMethod("SaveMultiFile")]
        public static void SaveMultiFile()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            foreach (string path in filepaths)
            {
                Database newDb = new Database(false, true);
                using (newDb)
                {
                    try
                    {
                        newDb.ReadDwgFile(path, FileOpenMode.OpenForReadAndAllShare, false, null);
                        using (var trans = newDb.TransactionManager.StartTransaction())
                        {
                            BlockTable blkTable = trans.GetObject(newDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                            BlockTableRecord blkTblRec = trans.GetObject(blkTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                            Circle circle = new Circle();
                            circle.Radius = 10;
                            circle.Center = new Point3d(0, 0, 0);
                            blkTblRec.AppendEntity(circle);
                            trans.AddNewlyCreatedDBObject(circle, true);
                            trans.Commit();
                        }
                        newDb.SaveAs(path, DwgVersion.Current);
                    }
                    catch
                    {
                        doc.Editor.WriteMessage("Không tìm thấy file!");
                    }
                }
            }
        }
    }
}
