using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System.Collections.Generic;

namespace Topic1.WorkWithObject
{
    public class WorkWithBlock
    {
        [CommandMethod("CreateBlock")]
        public static void CreateBlock()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (doc.LockDocument())
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    PromptStringOptions stringOptions = new PromptStringOptions("\nEnter new block name:");
                    stringOptions.AllowSpaces = true;
                    string blockName = "";
                    do
                    {
                        PromptResult promptResult = ed.GetString(stringOptions);
                        if (promptResult.Status != PromptStatus.OK) return;
                        try
                        {
                            // Check xem tên block đã tồn tại hay chưa nếu chưa thì tạo
                            SymbolUtilityServices.ValidateSymbolName(promptResult.StringResult, false);
                            if (blockTable.Has(promptResult.StringResult))
                            {
                                ed.WriteMessage("\nA block with this name already exists.");
                            }
                            else
                            {
                                blockName = promptResult.StringResult;
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
                    DBObjectCollection entities = LibraryCad.LineFunc.SquareOfLines(5);
                    foreach (Entity entity in entities)
                    {
                        tableRec.AppendEntity(entity);
                        trans.AddNewlyCreatedDBObject(entity, true);
                    }
                    BlockTableRecord blkTableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    BlockReference blockRef = new BlockReference(Point3d.Origin, tableRecID);
                    // Tạo xdata cho block 
                    LibraryCad.SubFunc.AddRegAppTableRecord("Phuc");
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

        [CommandMethod("CopyBlocksBetweenDatabases", CommandFlags.Session)]
        public static void CopyBlocksBetweenDatabases()
        {
            ObjectIdCollection objIdColl = new ObjectIdCollection();
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            // List thông tin các khối
            var blockInfos = new List<LibraryCad.Models.BlockInfo>();
            using (DocumentLock acLckDocCur = doc.LockDocument())
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    TypedValue[] typeds = new TypedValue[]
                    {
                        new TypedValue((int)DxfCode.Start, "INSERT")
                    };
                    SelectionFilter filter = new SelectionFilter(typeds);
                    var objIds = LibraryCad.SubFunc.GetListSelection(doc, "\n- Chọn block:", filter);
                    if (objIds != null)
                    {
                        objIdColl = new ObjectIdCollection();
                        foreach (var objId in objIds)
                        {
                            // Lấy thông tin các khối được chọn
                            BlockReference block = trans.GetObject(objId, OpenMode.ForRead) as BlockReference;
                            objIdColl.Add(block.BlockId);
                            var blockInfo = new LibraryCad.Models.BlockInfo();
                            blockInfo.Name = block.Name;
                            blockInfo.Location = block.Position;
                            blockInfo.Rotate = block.Rotation;
                            blockInfos.Add(blockInfo);
                        }
                    }
                    trans.Commit();
                }
            }
            if (blockInfos.Count == 0) return;
            // Lấy đường dẫn của file muốn thao tác
            string sLocalRoot = Application.GetSystemVariable("LOCALROOTPREFIX") as string;
            string sTemplatePath = sLocalRoot + "Template\\Test.dwg";
            Database newDB = new Database(false, true);
            using (newDB)
            {
                // Đọc file được chỉ định
                try
                {
                    newDB.ReadDwgFile(sTemplatePath, FileOpenMode.OpenForReadAndAllShare, false, null);
                }
                catch
                {
                    doc.Editor.WriteMessage("Không tìm thấy file!");
                }
                using (Transaction trans = newDB.TransactionManager.StartTransaction())
                {
                    BlockTable blockTable = trans.GetObject(newDB.BlockTableId, OpenMode.ForRead) as BlockTable;
                    // Clone đối tượng qua database khác
                    IdMapping acIdMap = new IdMapping();
                    db.WblockCloneObjects(objIdColl, blockTable.ObjectId, acIdMap, DuplicateRecordCloning.Ignore, false);
                    foreach(var bi in blockInfos)
                    {
                        // Đưa khối từ bảng block vào trang vẽ
                        BlockTableRecord blockTableRec = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                        BlockReference blockRef = new BlockReference(bi.Location, blockTable[bi.Name]);
                        blockRef.Rotation = bi.Rotate;
                        blockTableRec.AppendEntity(blockRef);
                        trans.AddNewlyCreatedDBObject(blockRef, true);
                    }
                    trans.Commit();
                    // Lưu bản vẽ
                    newDB.SaveAs(sTemplatePath, DwgVersion.Current);
                }
            }
        }

        [CommandMethod("CopyObjectsBetweenDatabases", CommandFlags.Session)]
        public static void CopyObjectsBetweenDatabases()
        {
            ObjectIdCollection objIdColl = new ObjectIdCollection();
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (DocumentLock lockDoc = doc.LockDocument())
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    // Lấy list các DBObjects
                    var objIds = LibraryCad.SubFunc.GetListSelection(doc, "\n- Chọn các đối tượng muốn sao chép: ");
                    if (objIds != null)
                    {
                        objIdColl = new ObjectIdCollection();
                        foreach (var objId in objIds)
                        {
                            objIdColl.Add(objId);
                        }
                    }
                    trans.Commit();
                }
            }
            Database newDB = new Database(true, true);
            using (newDB)
            {
                using (Transaction trans = newDB.TransactionManager.StartTransaction())
                {
                    BlockTable blockTable = trans.GetObject(newDB.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord blockTableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    // Clone đối tượng qua database mới tạo
                    IdMapping acIdMap = new IdMapping();
                    db.WblockCloneObjects(objIdColl, blockTableRec.ObjectId, acIdMap, DuplicateRecordCloning.Ignore, false);
                    trans.Commit();
                    // Lưu thành file mới
                    newDB.SaveAs("D:\\TestFile1.dwg", DwgVersion.Current);
                }
            }
        }

        [CommandMethod("CopyFileWithText")]
        public static void CopyFileWithText()
        {
            LibraryCad.FileFunc.CreateFileWithText("C:\\Users\\OMEN\\AppData\\Local\\Autodesk\\AutoCAD 2021\\R24.0\\enu\\Template\\Test.dwg");
        }
    }
}
