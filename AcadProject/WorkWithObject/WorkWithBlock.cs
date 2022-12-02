﻿using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using LibraryCad;
using LibraryCad.BlockObject;
using System.Linq;

namespace Topic1.WorkWithObject
{
    public class WorkWithBlock
    {
        [CommandMethod("CreateBlock")]
        public static void CreateBlock()
        {
            BlockFunc.CreateRectangleBlock();
        }

        [CommandMethod("CopyBlocksBetweenDatabases", CommandFlags.Session)]
        public static void CopyBlocksBetweenDatabases()
        {
            ObjectIdCollection objIdColl = new ObjectIdCollection();
            Document doc = Application.DocumentManager.MdiActiveDocument;
            // List thông tin các khối
            var blockInfos = BlockFunc.PickBlock(doc, objIdColl);
            if (blockInfos.Count == 0) return;
            // Lấy đường dẫn của file muốn thao tác
            string localRoot = Application.GetSystemVariable("LOCALROOTPREFIX") as string;
            string filePath = localRoot + "Template\\Test.dwg";
            // Copy các block mới chọn sang database mới theo đường dẫn truyền vào
            BlockFunc.CloneBlockToAnotherDatabase(doc, filePath, objIdColl, blockInfos);
        }

        [CommandMethod("CopyObjectsBetweenDatabases", CommandFlags.Session)]
        public static void CopyObjectsBetweenDatabases()
        {
            ObjectIdCollection objIdColl = new ObjectIdCollection();
            Document doc = Application.DocumentManager.MdiActiveDocument;
            SubFunc.PickAllObject(doc, objIdColl);
            if(objIdColl.Count == 0) return;
            SubFunc.CLoneObjectToDatabase(doc, objIdColl, "D:\\TestFile1.dwg");
        }

        [CommandMethod("CopyFileWithText")]
        public static void CopyFileWithText()
        {
            FileFunc.CreateFileWithText("C:\\Users\\OMEN\\AppData\\Local\\Autodesk\\AutoCAD 2021\\R24.0\\enu\\Template\\Test.dwg");
        }

        [CommandMethod("JoinObjectToBlock")]
        public static void JoinObjectToBlock()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            var block = BlockFunc.PickBlock(doc);
            var entitiesToAdd = SubFunc.GetListSelection(doc, "- Chọn các đối tượng muốn thêm vào block:");
            ObjectId[] idsToAdd = new ObjectId[entitiesToAdd.Length];
            for (int i = 0; i < entitiesToAdd.Length; i++)
            {
                if (entitiesToAdd[i] == block.BlockId) continue;
                idsToAdd[i] = entitiesToAdd[i];
            }
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable blkTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord blkTblRec = trans.GetObject(block.BlockId, OpenMode.ForWrite) as BlockTableRecord;
                blkTable.UpgradeOpen();
                var blkId = blkTable.Add(blkTblRec);
                ObjectIdCollection collection = new ObjectIdCollection(idsToAdd);
                IdMapping mapping = new IdMapping();
                db.DeepCloneObjects(collection, blkId, mapping, false);
                trans.Commit();
            }
        }
    }

    public static class Extensions

    {

        public static void Add(this ObjectIdCollection col, ObjectId[] ids)

        {

            foreach (var id in ids)

            {

                if (!col.Contains(id))

                    col.Add(id);

            }

        }

    }



    public class Commands

    {

        [CommandMethod("MB")]

        public static void MergeBlocks()

        {

            var doc = Application.DocumentManager.MdiActiveDocument;

            if (doc == null)

                return;



            var db = doc.Database;

            var ed = doc.Editor;



            // Get the name of the first block to merge



            var pr = ed.GetString("\nEnter name of first block");

            if (pr.Status != PromptStatus.OK)

                return;



            string first = pr.StringResult.ToUpper();



            using (var tr = doc.TransactionManager.StartTransaction())

            {

                var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);



                // Check whether the first block exists



                if (bt.Has(first))

                {

                    // Get the name of the second block to merge



                    pr = ed.GetString("\nEnter name of second block");

                    if (pr.Status != PromptStatus.OK)

                        return;



                    string second = pr.StringResult.ToUpper();



                    // Check whether the second block exists



                    if (bt.Has(second))

                    {

                        // Get the name of the new block



                        pr = ed.GetString("\nEnter name for new block");

                        if (pr.Status != PromptStatus.OK)

                            return;



                        string merged = pr.StringResult.ToUpper();



                        // Make sure the new block doesn't already exist



                        if (!bt.Has(merged))

                        {

                            // We need to collect the contents of the two blocks



                            var ids = new ObjectIdCollection();



                            // Open the two blocks to be merged



                            var btr1 =

                              tr.GetObject(bt[first], OpenMode.ForRead) as BlockTableRecord;

                            var btr2 =

                              tr.GetObject(bt[second], OpenMode.ForRead) as BlockTableRecord;



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

                            ed.WriteMessage(

                              "\nDrawing already contains a block named \"{0}\".", merged

                            );

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

    }
}
