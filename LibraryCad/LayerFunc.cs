using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System.Text.RegularExpressions;
using LibraryCad.Models;
using acad = Autodesk.AutoCAD.ApplicationServices.Application;
using System.Collections.Generic;
using Autodesk.AutoCAD.Colors;

namespace LibraryCad
{
    public class LayerFunc
    {
        /// <summary>
        /// Hàm lấy layer theo tên
        /// </summary>
        /// <param name="layerName">Đối tượng Layer</param>
        /// <param name="doc">Document</param>
        /// <returns></returns>
        public static LayerTableRecord GetLayer(Dimension dim, Document doc)
        {
            using (var trans = doc.Database.TransactionManager.StartOpenCloseTransaction())
            {
                try
                {
                    LayerTable lyrTb = trans.GetObject(doc.Database.LayerTableId, OpenMode.ForRead) as LayerTable;
                    LayerTableRecord lyrTblRec = trans.GetObject(dim.LayerId, OpenMode.ForRead) as LayerTableRecord;
                    var layerName = lyrTblRec.Name;
                    if (lyrTb.Has(lyrTblRec.Name))
                    {
                        return lyrTblRec;
                    }
                    return null;
                }
                catch (System.Exception ex)
                {
                    doc.Editor.WriteMessage(ex.Message);
                    trans.Abort();
                    return null;
                }
            }
        }

        /// <summary>
        /// Hàm xóa layer
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="layerName">Tên layer</param>
        /// <returns>Chuỗi</returns>
        public static string LayerDelete(Document doc, string layerName)
        {
            var db = doc.Database;
            if (layerName == "0")
                return "Layer '0' cannot be deleted.";
            using (doc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    var layerTable = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
                    if (!layerTable.Has(layerName))
                        return "Layer '" + layerName + "' not found.";
                    try
                    {
                        var layerId = layerTable[layerName];
                        if (db.Clayer == layerId)
                            return "Current layer cannot be deleted.";
                        //ObjectIdCollection objIdColl = new ObjectIdCollection();
                        //objIdColl.Add(layerId);
                        //db.Purge(objIdColl);
                        //if(objIdColl.Count > 0)
                        //{
                        var layer = tr.GetObject(layerId, OpenMode.ForWrite) as LayerTableRecord;
                        layer.IsLocked = false;
                        var blockTable = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                        foreach (var btrId in blockTable)
                        {
                            var block = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                            foreach (var entId in block)
                            {
                                var ent = (Entity)tr.GetObject(entId, OpenMode.ForRead);
                                if (ent.Layer == layerName)
                                {
                                    ent.UpgradeOpen();
                                    ent.Erase();

                                }
                            }
                        }
                        layer.Erase();
                        tr.Commit();
                        return "Layer '" + layerName + "' have been deleted.";
                        //}
                        //return "Deleted unsuccessful";
                    }
                    catch (System.Exception e)
                    {
                        return "Error: " + e.Message;
                    }
                }
            }
        }

        /// <summary>
        /// Hàm tạo layer
        /// </summary>
        /// <param name="layerInfo">Thông tin layer</param>
        /// <returns></returns>
        public static bool CreateLayer(LayerInfo layerInfo)
        {
            var status = false;
            // Get the current document and database
            Document doc = acad.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (doc.LockDocument())
            {
                // Start a transaction
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    // Open the Layer table for read
                    LayerTable lyrTbl;
                    lyrTbl = trans.GetObject(db.LayerTableId,
                                                    OpenMode.ForRead) as LayerTable;

                    var layerName = layerInfo.Name;

                    if (lyrTbl.Has(layerName) == false)
                    {
                        using (LayerTableRecord lyrTblRec = new LayerTableRecord())
                        {
                            // Assign the layer the ACI color 3 and a name
                            lyrTblRec.Color = Color.FromColorIndex(ColorMethod.ByAci, layerInfo.ColorId);
                            lyrTblRec.Name = layerName;

                            //var layerInfo = new LibraryCad.Models.LayerInfo();
                            //layerInfo.Name = layerName;
                            //layerInfo.ColorId = short.Parse(txb_ColorId.Text);
                            //Variable.layerInfos.Add(layerInfo);


                            // Upgrade the Layer table for write
                            trans.GetObject(db.LayerTableId, OpenMode.ForWrite);

                            // Append the new layer to the Layer table and the transaction
                            lyrTbl.Add(lyrTblRec);
                            trans.AddNewlyCreatedDBObject(lyrTblRec, true);
                            status = true;
                        }
                    }
                    trans.Commit();
                }
            }
            return status;
        }
    }
}
