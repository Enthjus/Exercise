using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;
using LibraryCad.FileManip.File;
using LibraryCad.ObjectsFunc.BlockObject;
using LibraryCad.Sub;
using System.Linq;

namespace AcadProject.AcadManip.WorkWithObject
{
    public class WorkWithBlock
    {
        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

        [CommandMethod("CreateRectangleBlock")]
        public static void CreateRectangleBlock()
        {
            BlockFunc.CreateRectangleBlock();
        }

        [CommandMethod("CopyBlocksBetweenDatabases", CommandFlags.Session)]
        public static void CopyBlocksBetweenDatabases()
        {
            using (doc.LockDocument())
            {
                ObjectIdCollection objIdColl = new ObjectIdCollection();
                // List thông tin các khối
                var blockInfos = BlockFunc.PickBlock(doc, objIdColl);
                if (blockInfos.Count == 0) return;
                // Lấy đường dẫn của file muốn thao tác
                string localRoot = Application.GetSystemVariable("LOCALROOTPREFIX") as string;
                string filePath = localRoot + "Template\\Test.dwg";
                // Copy các block mới chọn sang database mới theo đường dẫn truyền vào
                BlockFunc.CloneBlockToAnotherDatabase(doc, filePath, objIdColl, blockInfos);
            }
        }

        [CommandMethod("CopyObjectsBetweenDatabases", CommandFlags.Session)]
        public static void CopyObjectsBetweenDatabases()
        {
            using (doc.LockDocument())
            {
                ObjectIdCollection objIdColl = new ObjectIdCollection();
                SubFunc.PickAllObject(doc, objIdColl);
                if (objIdColl.Count == 0) return;
                SubFunc.CLoneObjectToDatabase(doc, objIdColl, "D:\\TestFile1.dwg");
            }
        }

        [CommandMethod("CopyFileWithText")]
        public static void CopyFileWithText()
        {
            FileFunc.CreateFileWithText("C:\\Users\\OMEN\\AppData\\Local\\Autodesk\\AutoCAD 2021\\R24.0\\enu\\Template\\Test.dwg");
        }

        [CommandMethod("AddEntToBlock")]
        public static void AddEntToBlock()
        {
            using (doc.LockDocument())
            {
                BlockFunc.AddEntityToBlock(doc);
            }
        }

        [CommandMethod("AddBlockToBlock")]
        public static void AddBlockToBlock()
        {
            using (doc.LockDocument())
            {
                BlockFunc.AddBlockToBlock(doc);
            }
        }

        [CommandMethod("HlEntInBlk")]
        public static void RunMyCommand()
        {
            using (doc.LockDocument())
            {
                BlockFunc.HighlightEntityInBlock(doc);
            }
        }

        [CommandMethod("DeleteEntityInBlock")]
        public static void DeleteEntityInBlock()
        {
            using (doc.LockDocument())
            {
                BlockFunc.DeleteEntityInBlock(doc);
                doc.Editor.Regen();
            }
        }

        [CommandMethod("EditEntityInBlock")]
        public static void EditEntityInBlock()
        {
            using (doc.LockDocument())
            {
                BlockFunc.EditEntityInBlock(doc);
                doc.Editor.Regen();
            }
        }

        [CommandMethod("ChangeBlkBasePoint")]
        public static void ChangeBlockBasePoint()
        {
            try
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    var block = BlockFunc.PickBlock(doc);
                    var oldBP = block.Position;
                    BlockTable blkTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord blkTblRec = trans.GetObject(blkTable[block.Name], OpenMode.ForWrite) as BlockTableRecord;
                    var basePoint = SubFunc.PickPoint(doc);
                    var angle = new Line(oldBP, basePoint.point).Delta;
                    blkTblRec.Origin = blkTblRec.Origin + angle;
                    trans.Commit();
                    ed.Regen();
                }
            }
            catch
            {
                return;
            }
        }

        [CommandMethod("ChangeBlkBPNotMove")]
        public static void ChangeBlkBPNotMove()
        {
            try
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    var block = BlockFunc.PickBlock(doc);
                    var oldBP = block.Position;
                    BlockTable blkTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord blkTblRec = trans.GetObject(blkTable[block.Name], OpenMode.ForWrite) as BlockTableRecord;
                    var basePoint = SubFunc.PickPoint(doc);
                    var angle = new Line(oldBP, basePoint.point).Delta;
                    blkTblRec.Origin = blkTblRec.Origin + angle;
                    var blkRef = trans.GetObject(block.Id, OpenMode.ForWrite) as BlockReference;
                    blkRef.Position = blkRef.Position + new Line(oldBP, basePoint.point).Delta;
                    trans.Commit();
                    doc.Editor.Regen();
                }
            }
            catch
            {
                return;
            }
        }

        [CommandMethod("RedefiningABlock")]
        public void RedefiningABlock()
        {
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                if (!blockTable.Has("CircleBlock"))
                {
                    using (BlockTableRecord blkTblRec = new BlockTableRecord())
                    {
                        blkTblRec.Name = "CircleBlock";
                        blkTblRec.Origin = new Point3d(0, 0, 0);
                        using (Circle acCirc = new Circle())
                        {
                            acCirc.Center = new Point3d(0, 0, 0);
                            acCirc.Radius = 2;
                            blkTblRec.AppendEntity(acCirc);
                            blockTable.UpgradeOpen();
                            blockTable.Add(blkTblRec);
                            trans.AddNewlyCreatedDBObject(blkTblRec, true);
                            using (BlockReference blkRef = new BlockReference(new Point3d(0, 0, 0), blkTblRec.Id))
                            {
                                BlockTableRecord modelSpace = trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                                modelSpace.AppendEntity(blkRef);
                                trans.AddNewlyCreatedDBObject(blkRef, true);
                                Application.ShowAlertDialog("CircleBlock đã được tạo.");
                            }
                        }
                    }
                }
                else
                {
                    BlockTableRecord blkTblRec = trans.GetObject(blockTable["CircleBlock"], OpenMode.ForWrite) as BlockTableRecord;
                    foreach (ObjectId objID in blkTblRec)
                    {
                        DBObject dbObj = trans.GetObject(objID, OpenMode.ForRead) as DBObject;
                        if (dbObj is Circle)
                        {
                            Circle acCirc = dbObj as Circle;
                            acCirc.UpgradeOpen();
                            acCirc.Radius = acCirc.Radius * 2;
                        }
                    }
                    foreach (ObjectId objID in blkTblRec.GetBlockReferenceIds(false, true))
                    {
                        BlockReference blkRef = trans.GetObject(objID, OpenMode.ForWrite) as BlockReference;
                        blkRef.RecordGraphicsModified(true);
                    }

                    Application.ShowAlertDialog("CircleBlock has been revised.");
                }
                trans.Commit();
            }
        }

        [CommandMethod("MB")]
        public static void MergeBlocks()
        {
            if (doc == null) return;
            // Get the name of the first block to merge
            var block1 = BlockFunc.PickBlock(doc);
            if (block1 == null) return;
            string first = block1.Name.ToUpper(); 
            using (var tr = doc.TransactionManager.StartTransaction())
            {
                var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                // Check whether the first block exists
                if (bt.Has(first))
                {
                    // Get the name of the second block to merge
                    var block2 = BlockFunc.PickBlock(doc);
                    if (block2 == null) return;
                    string second = block2.Name.ToUpper();
                    // Check whether the second block exists
                    if (bt.Has(second))
                    {
                        // Get the name of the new block
                        var pr = ed.GetString("\nEnter name for new block");
                        if (pr.Status != PromptStatus.OK) return;
                        string merged = pr.StringResult.ToUpper();
                        // Make sure the new block doesn't already exist
                        if (!bt.Has(merged))
                        {
                            // We need to collect the contents of the two blocks
                            var ids = new ObjectIdCollection();
                            // Open the two blocks to be merged 
                            var btr1 = tr.GetObject(bt[first], OpenMode.ForRead) as BlockTableRecord;
                            var btr2 = tr.GetObject(bt[second], OpenMode.ForRead) as BlockTableRecord;
                            // Use LINQ to get IEnumerable<ObjectId> for the blocks
                            var en1 = btr1.Cast<ObjectId>();
                            var en2 = btr2.Cast<ObjectId>();
                            // Add the complete contents to our collection
                            // (we could also apply some filtering, here, such as making
                            // sure we only include attributes with the same name once)
                            ids.Add(en1.ToArray<ObjectId>());
                            ids.Add(en2.ToArray<ObjectId>());
                            // Create a new block table record for our merged block
                            var btr = new BlockTableRecord();
                            btr.Name = merged;
                            // Add it to the block table and the transaction
                            bt.UpgradeOpen();
                            var btrId = bt.Add(btr);
                            tr.AddNewlyCreatedDBObject(btr, true);
                            // Deep clone the contents of our two blocks into the new one
                            var idMap = new IdMapping();
                            db.DeepCloneObjects(ids, btrId, idMap, false);
                            ed.WriteMessage("\nBlock \"{0}\" created.", merged);
                        }
                        else
                        {
                            ed.WriteMessage("\nDrawing already contains a block named \"{0}\".", merged);
                        }
                    }
                    else
                    {
                        ed.WriteMessage("\nBlock \"{0}\" not found.", second);
                    }
                }
                else
                {
                    ed.WriteMessage("\nBlock \"{0}\" not found.", first);
                }
                // Always commit the transaction
                tr.Commit();
            }
        }

        [CommandMethod("IB")]
        public void ImportBlocks()
        {
            DocumentCollection dm = Application.DocumentManager;
            Editor ed = dm.MdiActiveDocument.Editor;
            Database destDb = dm.MdiActiveDocument.Database;
            Database sourceDb = new Database(false, true);
            PromptResult sourceFileName;
            try
            {
                // Get name of DWG from which to copy blocks
                //sourceFileName =
                //  ed.GetString("\nEnter the name of the source drawing: ");
                // Read the DWG into a side database
                sourceDb.ReadDwgFile("C:\\Users\\OMEN\\AppData\\Local\\Autodesk\\AutoCAD 2021\\R24.0\\enu\\Template\\Test.dwg", System.IO.FileShare.Read, true, "");
                // Create a variable to store the list of block identifiers
                ObjectIdCollection blockIds = new ObjectIdCollection();
                Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = sourceDb.TransactionManager;
                using (Transaction myT = tm.StartTransaction())
                {
                    // Open the block table
                    BlockTable bt = (BlockTable)tm.GetObject(sourceDb.BlockTableId, OpenMode.ForRead, false);
                    // Check each block in the block table
                    foreach (ObjectId btrId in bt)
                    {
                        BlockTableRecord btr = (BlockTableRecord)tm.GetObject(btrId, OpenMode.ForRead, false);
                        // Only add named & non-layout blocks to the copy list
                        if (!btr.IsAnonymous && !btr.IsLayout)
                            blockIds.Add(btrId);
                        btr.Dispose();
                    }
                }
                // Copy blocks from source to destination database
                IdMapping mapping = new IdMapping();
                sourceDb.WblockCloneObjects(blockIds, destDb.BlockTableId, mapping, DuplicateRecordCloning.Replace, false);

                ed.WriteMessage("\nCopied " + blockIds.Count.ToString() + " block definitions from " + "C:\\Users\\OMEN\\AppData\\Local\\Autodesk\\AutoCAD 2021\\R24.0\\enu\\Template\\Test.dwg" + " to the current drawing."); 
            } 
            catch (Autodesk.AutoCAD.Runtime.Exception ex) 
            { 
                ed.WriteMessage("\nError during copy: " + ex.Message); 
            } 
            sourceDb.Dispose();

        }
    }
}
