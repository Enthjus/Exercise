using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace LibraryCad.FileManip.File
{
    public class FileFunc
    {
        /// <summary>
        /// Hàm tạo file có text
        /// </summary>
        /// <param name="templatePath">Địa chỉ file</param>
        public static void CreateFileWithText(string templatePath)
        {
            // Tạo database mới
            using (var db = new Database(false, true))
            {
                // Đọc dwg file
                db.ReadDwgFile(templatePath, FileOpenMode.OpenForReadAndAllShare, false, null);
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    // Kiểm tra xem layer tên "Character" đã tồn tại chưa và tạo mới nếu chưa
                    string layerName = "Character";
                    ObjectId layerId;
                    LayerTable layerTable = trans.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                    if (layerTable.Has(layerName))
                    {
                        layerId = layerTable[layerName];
                    }
                    else
                    {
                        trans.GetObject(db.LayerTableId, OpenMode.ForWrite);
                        var layer = new LayerTableRecord
                        {
                            Name = layerName,
                            Color = Color.FromRgb(200, 30, 80)
                        };
                        layerId = layerTable.Add(layer);
                        trans.AddNewlyCreatedDBObject(layer, true);
                    }
                    BlockTableRecord tableRec = trans.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForWrite) as BlockTableRecord;
                    // Tạo dòng chữ
                    var text = new DBText
                    {
                        Position = new Point3d(2.0, 2.0, 0.0),
                        LayerId = layerId,
                        Height = 500.0,
                        TextString = "Hello, World."
                    };
                    tableRec.AppendEntity(text);
                    trans.AddNewlyCreatedDBObject(text, true);
                    trans.Commit();
                }
                // Lưu bản vẽ
                db.SaveAs(templatePath, DwgVersion.Current);
            }
        }
    }
}
