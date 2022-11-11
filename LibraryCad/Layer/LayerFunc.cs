using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
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
                    ObjectId id = ObjectId.Null;
                    // Open the Layer table for read
                    LayerTable lyrTbl = trans.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;

                    var layerName = layerInfo.Name;

                    // Neu co thi sua
                    if (lyrTbl.Has(layerInfo.Name))
                    {
                        // Sua
                        id = lyrTbl[layerName];
                    }
                    // Neu ko thi tao
                    else
                    {
                        LayerTableRecord record = new LayerTableRecord();
                        record.Name = layerInfo.Name;
                        // Chuyển layertable từ for read sang for write
                        lyrTbl.UpgradeOpen();

                        // Append the new layer to the Layer table and the transaction
                        id = lyrTbl.Add(record);
                        trans.AddNewlyCreatedDBObject(record, true);
                        status = true;
                    }
                    //Tới đối tượng mới tạo để sửa thuộc tính
                    LayerTableRecord recordnew = trans.GetObject(id, OpenMode.ForWrite) as LayerTableRecord;
                    recordnew.Color = Color.FromColorIndex(ColorMethod.ByAci, layerInfo.ColorId);
                    recordnew.Description = layerInfo.Des;
                    trans.Commit();
                }
            }
            return status;
        }

        /// <summary>
        /// Hàm lọc đối tượng theo layer
        /// </summary>
        /// <param name="filter">loại đối tượng</param>
        /// <param name="layerName">tên layer</param>
        /// <param name="doc">document</param>
        /// <returns></returns>
        public static List<Entity> GetEntityByFilterAndLayer(SelectionFilter filter, string layerName, Document doc)
        {
            try
            {
                var result = new List<Entity>();
                using (doc.LockDocument())
                {
                    using (Transaction trans = doc.TransactionManager.StartOpenCloseTransaction())
                    {
                        PromptSelectionResult acSSPrompt;
                        acSSPrompt = doc.Editor.SelectAll(filter);
                        if (acSSPrompt.Status == PromptStatus.OK)
                        {
                            var objIds = acSSPrompt.Value.GetObjectIds();
                            foreach (var objId in objIds)
                            {
                                var ent = trans.GetObject(objId, OpenMode.ForRead) as Entity;
                                if (ent.Layer == layerName)
                                {
                                    result.Add(ent);
                                }
                            }
                        }
                        trans.Commit();
                    }
                }
                return result;
            }
            catch (System.Exception ex)
            {
                doc.Editor.WriteMessage(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Hám lấy layer tạo bởi tool
        /// </summary>
        /// <param name="doc">Document</param>
        /// <returns></returns>
        public static List<LayerInfo> GetLayer(Document doc)
        {
            var db = doc.Database;
            var layerInfos = new List<LayerInfo>();
            using (doc.LockDocument())
            {
                using (var trans = db.TransactionManager.StartOpenCloseTransaction())
                {
                    var layerTable = trans.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                    foreach (var layerId in layerTable)
                    {
                        var layer = trans.GetObject(layerId, OpenMode.ForRead) as LayerTableRecord;
                        if (layer.Description == "tool create layer")
                        {
                            var ly = new LayerInfo();
                            ly.Name = layer.Name;
                            ly.ColorId = layer.Color.ColorIndex;
                            ly.Des = layer.Description;
                            layerInfos.Add(ly);
                        }
                    }
                    return layerInfos;
                }
            }
            return null;
        }

        /// <summary>
        /// Hàm lấy tổng chu vi và diện tích các đối tượng theo từng layer
        /// </summary>
        /// <param name="doc">document</param>
        /// <param name="layerInfos">list layer</param>
        /// <returns></returns>
        public static List<LayerObject> GetProperties(Document doc, List<LayerInfo> layerInfos)
        {
            if (layerInfos == null) return null;
            var layerObjs = new List<LayerObject>();

            using (doc.LockDocument())
            {
                using (var trans = doc.Database.TransactionManager.StartOpenCloseTransaction())
                {
                    foreach (var layer in layerInfos)
                    {
                        if(layer == null) continue;
                        var perimeter = 0.0;
                        var area = 0.0;
                        var layerObject = new LayerObject();
                        layerObject.LayerName = layer.Name;
                        //Chiều dài line
                        perimeter += LibraryCad.LineFunc.LineProperties(layer, doc);

                        //Chiều dài và diện tích pline
                        var plineProp = LibraryCad.Polyline.PolylineFunc.PlineProperties(layer, doc);
                        perimeter += plineProp.Perimeter;
                        area += plineProp.Area;

                        //Chiều dài và diện tích đường tròn
                        var circleProp = LibraryCad.CircleFunc.CircleProperties(layer, doc);
                        perimeter += circleProp.Perimeter;
                        area += circleProp.Area;

                        layerObject.Perimeter = perimeter;
                        layerObject.Area = area;
                        layerObjs.Add(layerObject);
                    }
                    return layerObjs;
                }
            }
            return null;
        }

        //internal static void CreateLayers(Document _doc, List<ItemProp> _lays)
        //{
        //    if (_doc.Database != null)
        //    {
        //        using (_doc.LockDocument())
        //        {
        //            using (Transaction tr = _doc.Database.TransactionManager.StartTransaction())
        //            {
        //                ObjectId id = ObjectId.Null;
        //                string xData_NameApp = "$Inx.Cad.Mep$";
        //                string xData_LayerMep = "$Mep.Tem.Layers$";
        //                using (LayerTable layerTable = (LayerTable)tr.GetObject(_doc.Database.LayerTableId, OpenMode.ForWrite))
        //                {
        //                    LayerTableRecord rec = new LayerTableRecord();
        //                    foreach (var lay in _lays)
        //                    {
        //                        if (layerTable.Has(lay.LayerName))
        //                        {
        //                            id = layerTable[lay.LayerName];
        //                        }
        //                        else
        //                        {
        //                            rec = new LayerTableRecord
        //                            {
        //                                Name = lay.LayerName,
        //                            };
        //                            id = layerTable.Add(rec);
        //                            tr.AddNewlyCreatedDBObject(rec, true);
        //                        }
        //                        rec = (LayerTableRecord)tr.GetObject(id, OpenMode.ForWrite);

        //                        //LinetypeTableRecord ll = tr.GetObject(rec.LinetypeObjectId, OpenMode.ForRead) as LinetypeTableRecord;
        //                        //string df = ll.Name;

        //                        //
        //                        //using (var lttable = (LinetypeTable)tr.GetObject(_doc.Database.LinetypeTableId, OpenMode.ForRead))
        //                        //{
        //                        //    if (lttable.Has(lay.LineType))
        //                        //    {
        //                        //        rec.UpgradeOpen();
        //                        //        rec.LinetypeObjectId = lttable[lay.LineType];// GetLineTypeID(_doc.Database, lay.LineType);
        //                        //    }
        //                        //}
        //                        rec.Color = Color.FromColorIndex(ColorMethod.ByAci, lay.ColorValue);
        //                        //rec.LinetypeObjectId = GetLineTypeID(_doc.Database, lay.LineType);
        //                        //rec.LineWeight = GetLineWeight(lay.LineWeight);
        //                        rec.Description = lay.Description;

        //                        /*Gán xData*/
        //                        LibDictionary.SetXData(rec.ObjectId, xData_NameApp, xData_LayerMep);
        //                    }
        //                }

        //                tr.Commit();
        //            }
        //        }
        //    }
        //}
    }
}
