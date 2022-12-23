using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Internal;
using LibraryCad.Models;
using LibraryCad.ObjectsFunc.LineObject;
using LibraryCad.Sub;
using System.Collections.Generic;
using System.Linq;

namespace LibraryCad.ObjectsFunc.BlockObject
{
    public class BlockFunc
    {
        #region Create a rectangle block at origin point
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
                    SubFunc.AddRegAppTableRecord(doc, "Phuc");
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
        #endregion

        #region Let user to pick blocks and return blocks info
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
        #endregion

        #region Let user to pick one block
        /// <summary>
        /// Hàm cho người dùng chọn 1 block
        /// </summary>
        /// <param name="doc">Document</param>
        /// <returns>BlockReference</returns>
        public static BlockReference PickBlock(Document doc, string str = "")
        {
            try
            {
                Database db = doc.Database;
                PromptSelectionOptions prSelOpt = new PromptSelectionOptions();
                if (str == "")
                {
                    prSelOpt.MessageForAdding = "\n- Chọn block muốn thao tác: ";
                }
                else
                {
                    prSelOpt.MessageForAdding = str;
                }
                prSelOpt.SingleOnly = true;
                TypedValue[] tvBlock = new TypedValue[]
                {
                        new TypedValue((int)DxfCode.Start, "INSERT")
                };
                SelectionFilter filter = new SelectionFilter(tvBlock);
                PromptSelectionResult prSelRes = doc.Editor.GetSelection(prSelOpt, filter);
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockReference block = trans.GetObject(prSelRes.Value.OfType<SelectedObject>().First().ObjectId, OpenMode.ForRead) as BlockReference;
                    trans.Commit();
                    if (block != null) return block;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Let user pick a block and return PromptEntityResult object
        /// <summary>
        /// Hàm chọn block trả về đối tượng PromptEntityResult
        /// </summary>
        /// <param name="ed">Editor</param>
        /// <returns>Đối tượng</returns>
        public static PromptEntityResult PickBlock(Editor ed)
        {
            PromptEntityOptions options = new PromptEntityOptions("\nPick a block: ");
            options.SetRejectMessage("\nMust be a block reference: ");
            options.AddAllowedClass(typeof(BlockReference), true);

            Utils.PostCommandPrompt();
            PromptEntityResult result = ed.GetEntity(options);  //select a block reference in the drawing.
            return result;
        }
        #endregion

        #region Clone block to another database
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
        #endregion

        #region Select one entity in block
        /// <summary>
        /// Hàm chọn đối tượng trong khối
        /// </summary>
        /// <param name="ed">Editor</param>
        /// <param name="entId">Id đối tượng được chọn</param>
        /// <param name="blkTransform">Ma trận</param>
        /// <returns></returns>
        public static bool SelectNestedEntityInBlock(Editor ed, out ObjectId entId, out Matrix3d blkTransform)
        {
            entId = ObjectId.Null;
            blkTransform = Matrix3d.Identity;
            var res = ed.GetNestedEntity("\nChọn đối tượng trong block:");
            if (res.Status == PromptStatus.OK)
            {
                entId = res.ObjectId;
                blkTransform = res.Transform;
                ed.WriteMessage($"\nId đối tượng: {entId}");
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Create hightlight cover around the entity block
        /// <summary>
        /// Hàm tạo hightlight bọc quanh đối tượng được chọn trong block
        /// </summary>
        /// <param name="doc">Document</param>
        public static void HighlightEntityInBlock(Document doc)
        {
            var ed = doc.Editor;
            if (SelectNestedEntityInBlock(ed, out ObjectId nestedEntId, out Matrix3d blkTransform))
            {
                using (var highlighter = new BlockNestedEntityHighlighter())
                {
                    highlighter.HighlightEntityInBlock(nestedEntId, blkTransform);
                    ed.GetString("\nPress Enter to continue...");
                }
                ed.PostCommandPrompt();
            }
            else
            {
                ed.WriteMessage("\n*Cancel*\n");
            }
        }
        #endregion

        #region Delete 1 entity in block
        /// <summary>
        /// Hàm xóa 1 đối tượng trong khối
        /// </summary>
        /// <param name="doc">Document</param>
        public static void DeleteEntityInBlock(Document doc)
        {
            var ed = doc.Editor;
            if (SelectNestedEntityInBlock(ed, out ObjectId nestedEntId, out Matrix3d blkTransform))
            {
                using (Transaction trans = doc.Database.TransactionManager.StartTransaction())
                {
                    var ent = trans.GetObject(nestedEntId, OpenMode.ForWrite) as Entity;
                    ent.Erase();
                    trans.Commit();
                }
            }
            else
            {
                ed.WriteMessage("\nĐối tượng không phải block!\n");
            }
        }
        #endregion

        #region Change layer 1 entity in block
        /// <summary>
        /// Hàm sửa 1 đối tượng trong khối
        /// </summary>
        /// <param name="doc">Document</param>
        public static void EditEntityInBlock(Document doc)
        {
            var ed = doc.Editor;
            if (SelectNestedEntityInBlock(ed, out ObjectId nestedEntId, out Matrix3d blkTransform))
            {
                using (Transaction trans = doc.Database.TransactionManager.StartTransaction())
                {
                    var ent = trans.GetObject(nestedEntId, OpenMode.ForWrite) as Entity;
                    ent.LayerId = doc.Database.Clayer;
                    trans.Commit();
                }
            }
            else
            {
                ed.WriteMessage("\nĐối tượng không phải block!\n");
            }
        }
        #endregion

        #region Add 1 entity to block
        /// <summary>
        /// Hàm thêm 1 đối tượng vào khối
        /// </summary>
        /// <param name="doc">Document</param>
        public static void AddEntityToBlock(Document doc)
        {
            Database db = doc.Database;
            Editor ed = doc.Editor;
            try
            {
                var entToAdds = SubFunc.GetListSelection(doc, "- Chọn các đối tượng muốn thêm vào block:");
                var block = BlockFunc.PickBlock(doc);
                if (block == null) return;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockReference insert = trans.GetObject(block.ObjectId, OpenMode.ForRead) as BlockReference;
                    BlockTableRecord hostBlk = trans.GetObject(insert.BlockTableRecord, OpenMode.ForWrite) as BlockTableRecord;
                    Matrix3d mat = insert.BlockTransform.Inverse();
                    foreach (var entToAdd in entToAdds)
                    {
                        Entity ent = trans.GetObject(entToAdd, OpenMode.ForWrite) as Entity;
                        if (insert == null)
                        {
                            //ed.WriteMessage("\nThe second selected is not a block reference (INSERT).");
                            return;
                        }
                        Entity clone = ent.Clone() as Entity;
                        clone.TransformBy(mat);
                        hostBlk.AppendEntity(clone);
                        trans.AddNewlyCreatedDBObject(clone, true);
                        ent.Erase();
                    }
                    trans.Commit();
                }
                ed.Regen();
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage(ex.ToString());
            }
        }
        #endregion

        #region Add 1 block to another block
        /// <summary>
        /// Hàm thêm 1 khối vào khối
        /// </summary>
        /// <param name="doc">Document</param>
        public static void AddBlockToBlock(Document doc)
        {
            Database db = doc.Database;
            Editor ed = doc.Editor;
            try
            {
                var blkToAdd = BlockFunc.PickBlock(doc, "\n- Chọn block muốn thêm vào:");
                var block = BlockFunc.PickBlock(doc, "\n- Chọn block chính:");
                if (block == null || blkToAdd == null || block.Name == blkToAdd.Name) return;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockReference insert = trans.GetObject(block.ObjectId, OpenMode.ForRead) as BlockReference;
                    BlockTableRecord hostBlk = trans.GetObject(insert.BlockTableRecord, OpenMode.ForWrite) as BlockTableRecord;
                    Matrix3d mat = insert.BlockTransform.Inverse();
                    Entity ent = trans.GetObject(blkToAdd.ObjectId, OpenMode.ForWrite) as Entity;
                    if (insert == null) return;
                    Entity clone = ent.Clone() as Entity;
                    clone.TransformBy(mat);
                    hostBlk.AppendEntity(clone);
                    trans.AddNewlyCreatedDBObject(clone, true);
                    ent.Erase();
                    trans.Commit();
                }
                ed.Regen();
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage(ex.ToString());
            }
        }
        #endregion

        #region Get block from another file to current file
        /// <summary>
        /// Hàm lấy block từ file khác vào file hiện hành
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="blkName">tên khối</param>
        /// <param name="filePath">nguồn của file</param>
        public static void GetBlkFromAnotherDB(Document doc, string blkName, string filePath)
        {
            try
            {
                Database newDB = new Database(false, true);
                Database db = doc.Database;
                // Đọc file được chỉ định
                try
                {
                    newDB.ReadDwgFile(filePath, System.IO.FileShare.Read, true, "");
                }
                catch
                {
                    doc.Editor.WriteMessage("Không tìm thấy file!");
                }
                ObjectIdCollection objIdCol = new ObjectIdCollection();
                List<BlockInfo> blkInfs = new List<BlockInfo>();
                using (newDB)
                {
                    using (Transaction trans = newDB.TransactionManager.StartTransaction())
                    {
                        BlockTable blockTable = trans.GetObject(newDB.BlockTableId, OpenMode.ForRead) as BlockTable;
                        BlockTableRecord modelSpace = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;
                        BlockTableRecord blkTblRec = trans.GetObject(blockTable[blkName], OpenMode.ForRead) as BlockTableRecord;
                        objIdCol.Add(blkTblRec.ObjectId);
                        foreach (ObjectId id in modelSpace)
                        {
                            BlockReference blkRef = trans.GetObject(id, OpenMode.ForRead) as BlockReference;
                            if (blkRef == null) continue;
                            if (blkRef.Name == blkName)
                            {
                                BlockInfo blkInf = new BlockInfo();
                                blkInf.Name = blkName;
                                blkInf.Location = blkRef.Position;
                                blkInf.Rotate = blkRef.Rotation;
                                blkInfs.Add(blkInf);
                            }
                        }
                    }
                    IdMapping idMap = new IdMapping();
                    newDB.WblockCloneObjects(objIdCol, db.BlockTableId, idMap, DuplicateRecordCloning.Replace, false);
                }
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord blkTblRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    foreach (var blkInf in blkInfs)
                    {
                        BlockReference blkRef = new BlockReference(blkInf.Location, blockTable[blkInf.Name]);
                        blkRef.Rotation = blkInf.Rotate;
                        blkTblRec.AppendEntity(blkRef);
                        trans.AddNewlyCreatedDBObject(blkRef, true);
                    }
                    trans.Commit();
                }
            }
            catch
            {
                return;
            }
        }
        #endregion

        #region Thay đổi tâm của khối
        /// <summary>
        /// Hàm thay đổi tâm của khối
        /// </summary>
        /// <param name="ed">Editor</param>
        /// <param name="trans">Transaction</param>
        /// <param name="result">Khối được chọn</param>
        public static void ChangeBLockInsertPoint(Editor ed, Transaction trans, PromptEntityResult result, Point3d newPt, string moveStatus)
        {
            BlockReference reference = trans.GetObject(result.ObjectId, OpenMode.ForRead) as BlockReference;
            BlockTableRecord record = trans.GetObject(reference.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;

            Point3d refPos = reference.Position;                           //current position of inserted block ref.                        
            //Point3d pmin = reference.Bounds.Value.MinPoint;                //bounding box of entities.
            //Point3d pmax = reference.Bounds.Value.MaxPoint;
            //Point3d newPos = (Point3d)(pmin + (pmax - pmin) / 2);          //center point of displayed graphics.

            Vector3d vec = newPt.GetVectorTo(refPos);                     //apply your own desired points here.                  
            vec = vec.TransformBy(reference.BlockTransform.Transpose());   //
            Matrix3d mat = Matrix3d.Displacement(vec);                     //     


            foreach (ObjectId eid in record)                               //update entities in the table record.
            {
                Entity entity = (Entity)trans.GetObject(eid, OpenMode.ForRead) as Entity;

                if (entity != null)
                {
                    entity.UpgradeOpen();
                    entity.TransformBy(mat);
                    entity.DowngradeOpen();
                }
            }

            ObjectIdCollection blockReferenceIds = record.GetBlockReferenceIds(false, false); //get all instances of same block ref.

            foreach (ObjectId eid in blockReferenceIds)              //update all block references of the block modified.
            {
                BlockReference BlkRef = (BlockReference)trans.GetObject(eid, OpenMode.ForWrite);

                if(moveStatus == "y")
                {
                    BlkRef.TransformBy(mat.Inverse());  // include this line if you want block ref to stay in original location in dwg.
                }

                BlkRef.RecordGraphicsModified(true);
            }

            trans.TransactionManager.QueueForGraphicsFlush(); 

            ed.WriteMessage("\nInsertion points modified.");                                  

            trans.Commit();
        }
        #endregion
    }
}
