using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using LibraryCad.Models;
using System.Collections.Generic;

namespace LibraryCad.BlockObject
{
    public class BlockFunc
    {
        /// <summary>
        /// Hàm tạo khối hình vuông tại tọa độ (0, 0)
        /// </summary>
        public static void CreateRectangleBlock()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (doc.LockDocument())
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    string blockName = "";
                    do
                    {
                        string result = SubFunc.GetString(doc, "\nEnter new block name:");
                        if (result == "" || result == null) return;
                        try
                        {
                            // Check xem tên block đã tồn tại hay chưa nếu chưa thì tạo
                            SymbolUtilityServices.ValidateSymbolName(result, false);
                            if (blockTable.Has(result))
                            {
                                ed.WriteMessage("\nA block with this name already exists.");
                            }
                            else
                            {
                                blockName = result;
                            }
                        }
                        catch
                        {
                            ed.WriteMessage("\nInvalid block name.");
                        }
                    } while (blockName == "");
                    // Gán các thông số vào block để khởi tạo
                    BlockTableRecord tableRec = new BlockTableRecord();
                    tableRec.Name = blockName;
                    blockTable.UpgradeOpen();
                    blockTable.CreateExtensionDictionary();
                    ObjectId tableRecID = blockTable.Add(tableRec);
                    trans.AddNewlyCreatedDBObject(tableRec, true);
                    DBObjectCollection entities = LineFunc.SquareOfLines(5);
                    foreach (Entity entity in entities)
                    {
                        tableRec.AppendEntity(entity);
                        trans.AddNewlyCreatedDBObject(entity, true);
                    }
                    BlockTableRecord blkTableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    BlockReference blockRef = new BlockReference(Point3d.Origin, tableRecID);
                    // Tạo xdata cho block 
                    LibraryCad.SubFunc.AddRegAppTableRecord(doc, "Phuc");
                    ResultBuffer rb = new ResultBuffer(new TypedValue(1001, "Phuc"), new TypedValue(1000, "Block_tool"));
                    blockRef.XData = rb;
                    rb.Dispose();
                    blkTableRec.AppendEntity(blockRef);
                    trans.AddNewlyCreatedDBObject(blockRef, true);
                    trans.Commit();
                    ed.WriteMessage("\nCreated block name \"{0}\" containing {1} entities.", blockName, entities.Count);
                }
            }
        }

        /// <summary>
        /// Hàm cho chọn các khối và trả về thông tin các khối 
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="objIdColl">danh sách id các đối tượng</param>
        /// <returns></returns>
        public static List<BlockInfo> PickBlock(Document doc, ObjectIdCollection objIdColl)
        {
            Database db = doc.Database;
            var blockInfos = new List<BlockInfo>();
            using (doc.LockDocument())
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    TypedValue[] tvBlock = new TypedValue[]
                    {
                        new TypedValue((int)DxfCode.Start, "INSERT")
                    };
                    SelectionFilter filter = new SelectionFilter(tvBlock);
                    var objIds = SubFunc.GetListSelection(doc, "\n- Chọn block:", filter);
                    if (objIds != null)
                    {
                        foreach (var objId in objIds)
                        {
                            // Lấy thông tin các khối được chọn
                            BlockReference block = trans.GetObject(objId, OpenMode.ForRead) as BlockReference;
                            objIdColl.Add(block.BlockId);
                            var blockInfo = new BlockInfo();
                            blockInfo.Name = block.Name;
                            blockInfo.Location = block.Position;
                            blockInfo.Rotate = block.Rotation;
                            blockInfos.Add(blockInfo);
                        }
                    }
                    trans.Commit();
                }
            }
            return blockInfos;
        }

        /// <summary>
        /// Hàm copy các khối từ document đang mở sang database của file khác mà không cần mở file đó lên
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="filePath">Đường dẫn tới file muốn đặt các khối</param>
        /// <param name="objIdColl">Dach sách id các khối</param>
        /// <param name="blockInfos">Thông tin các khối</param>
        public static void CloneBlockToAnotherDatabase(Document doc, string filePath, ObjectIdCollection objIdColl, List<BlockInfo> blockInfos)
        {
            Database newDB = new Database(false, true);
            // Đọc file được chỉ định
            try
            {
                newDB.ReadDwgFile(filePath, FileOpenMode.OpenForReadAndAllShare, true, null);
            }
            catch
            {
                doc.Editor.WriteMessage("Không tìm thấy file!");
            }
            using (newDB)
            {
                using (Transaction trans = newDB.TransactionManager.StartTransaction())
                {
                    BlockTable blockTable = trans.GetObject(newDB.BlockTableId, OpenMode.ForRead) as BlockTable;
                    // Clone đối tượng qua database khác
                    IdMapping idMap = new IdMapping();
                    doc.Database.WblockCloneObjects(objIdColl, blockTable.ObjectId, idMap, DuplicateRecordCloning.Ignore, false);
                    foreach (var blockInfo in blockInfos)
                    {
                        // Đưa khối từ bảng insert vào trang vẽ
                        BlockTableRecord tableRec = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                        BlockReference blockRef = new BlockReference(blockInfo.Location, blockTable[blockInfo.Name]);
                        blockRef.Rotation = blockInfo.Rotate;
                        tableRec.AppendEntity(blockRef);
                        trans.AddNewlyCreatedDBObject(blockRef, true);
                    }
                    trans.Commit();
                    // Lưu bản vẽ
                    newDB.SaveAs(filePath, DwgVersion.Current);
                }
            }
        }
    }
}
