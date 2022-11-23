using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace LibraryCad
{
    public class FileFunc
    {
        /// <summary>
        /// Hàm tạo file có text
        /// </summary>
        /// <param name="templatePath">Địa chỉ file</param>
        public static void CreateFileWithText(string templatePath)
        {
            // create a new Database (within a using statement to ensure the Database to be disposed)
            using (var db = new Database(false, true))
            {
                // read the template dwg file
                db.ReadDwgFile(templatePath, FileOpenMode.OpenForReadAndAllShare, false, null);

                // start a transaction
                using (var tr = db.TransactionManager.StartTransaction())
                {
                    // check if a layer named "Character" already exists and create it if not
                    string layerName = "Character";
                    ObjectId layerId;
                    var layerTable = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
                    if (layerTable.Has(layerName))
                    {
                        layerId = layerTable[layerName];
                    }
                    else
                    {
                        tr.GetObject(db.LayerTableId, OpenMode.ForWrite);
                        var layer = new LayerTableRecord
                        {
                            Name = layerName,
                            Color = Color.FromRgb(200, 30, 80)
                        };
                        layerId = layerTable.Add(layer);
                        tr.AddNewlyCreatedDBObject(layer, true);
                    }

                    // get the Model space
                    var modelSpace = (BlockTableRecord)tr.GetObject(
                        SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForWrite);

                    // create a text
                    var text = new DBText
                    {
                        Position = new Point3d(2.0, 2.0, 0.0),
                        LayerId = layerId,
                        Height = 500.0,
                        TextString = "Hello, World."
                    };
                    modelSpace.AppendEntity(text);
                    tr.AddNewlyCreatedDBObject(text, true);

                    // save changes to the database
                    tr.Commit();
                }

                // save the new drawing
                db.SaveAs(templatePath, DwgVersion.Current);
            }
        }
    }
}
