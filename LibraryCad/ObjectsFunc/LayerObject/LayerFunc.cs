using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using LibraryCad.Models;
using LibraryCad.ObjectsFunc.ArcObject;
using LibraryCad.ObjectsFunc.CircleObject;
using LibraryCad.ObjectsFunc.LineObject;
using LibraryCad.ObjectsFunc.PolylineObject;
using LibraryCad.Sub;
using System.Collections.Generic;
using acad = Autodesk.AutoCAD.ApplicationServices.Application;

namespace LibraryCad.ObjectsFunc.LayerObject
{
    public class LayerFunc
    {
        #region Lấy layer theo tên
        /// <summary>
        /// Hàm lấy layer theo tên
        /// </summary>
        /// <param name="layerName">Đối tượng Layer</param>
        /// <param name="doc">Document</param>
        /// <returns></returns>
        public static LayerTableRecord GetLayerByName(Dimension dim, Document doc)
        {
            using (var trans = doc.Database.TransactionManager.StartOpenCloseTransaction())
            {
                try
                {
                    LayerTable layerTable = trans.GetObject(doc.Database.LayerTableId, OpenMode.ForRead) as LayerTable;
                    LayerTableRecord layerTableRec = trans.GetObject(dim.LayerId, OpenMode.ForRead) as LayerTableRecord;
                    var layerName = layerTableRec.Name;
                    // Check tên layer có trong bảng không
                    if (layerTable.Has(layerTableRec.Name))
                    {
                        return layerTableRec;
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
        #endregion

        #region Xóa layer được tạo bởi winform
        /// <summary>
        /// Hàm xóa layer
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="layerName">Tên layer</param>
        /// <returns>Chuỗi</returns>
        public static string DeleteLayer(Document doc, string layerName)
        {
            var db = doc.Database;
            // Không cho xóa layer 0
            if (layerName == "0") return "Layer '0' cannot be deleted.";
            using (doc.LockDocument())
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    var layerTable = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForRead);
                    // Check nếu name truyền vào không tồn tại thì trả về thông báo thông báo
                    if (!layerTable.Has(layerName))
                    {
                        return "Layer '" + layerName + "' not found.";
                    }
                    try
                    {
                        ObjectId layerId = layerTable[layerName];
                        // Check nếu là layer đang sử dụng thì không thể xóa
                        if (db.Clayer == layerId)
                        {
                            return "Current layer cannot be deleted.";
                        }
                        LayerTableRecord layer = trans.GetObject(layerId, OpenMode.ForWrite) as LayerTableRecord;
                        // Bỏ khóa nếu layer đang khóa
                        layer.IsLocked = false;
                        BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                        foreach (var btrId in blockTable)
                        {
                            BlockTableRecord block = trans.GetObject(btrId, OpenMode.ForRead) as BlockTableRecord;
                            // Xóa các đối tượng đang sử dụng layer
                            foreach (var entId in block)
                            {
                                Entity ent = trans.GetObject(entId, OpenMode.ForRead) as Entity;
                                if (ent.Layer == layerName)
                                {
                                    ent.UpgradeOpen();
                                    ent.Erase();
                                }
                            }
                        }
                        // Xóa layer
                        layer.Erase();
                        trans.Commit();
                        return "Layer '" + layerName + "' have been deleted.";
                    }
                    catch (System.Exception e)
                    {
                        return "Error: " + e.Message;
                    }
                }
            }
        }
        #endregion

        #region Tạo layer bằng winform
        /// <summary>
        /// Hàm tạo layer
        /// </summary>
        /// <param name="layerInfo">Thông tin layer</param>
        /// <returns></returns>
        public static string CreateLayer(LayerInfo layerInfo)
        {
            try
            {
                var msg = "";
                Document doc = acad.DocumentManager.MdiActiveDocument;
                Database db = doc.Database;
                using (doc.LockDocument())
                {
                    using (Transaction trans = db.TransactionManager.StartTransaction())
                    {
                        ObjectId id = ObjectId.Null;
                        LayerTable layerTable = trans.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                        var layerName = layerInfo.Name;
                        // Nếu đã có thì sửa 
                        if (layerTable.Has(layerName))
                        {
                            // Sửa
                            id = layerTable[layerName];
                            msg = "Sửa thành công";
                        }
                        // Nếu chưa có thì tạo
                        else
                        {
                            foreach (var entityId in layerTable)
                            {
                                LayerTableRecord layer = trans.GetObject(entityId, OpenMode.ForRead) as LayerTableRecord;
                                if (layer.Color.ColorIndex == layerInfo.ColorId)
                                {
                                    return "Màu đã tồn tại làm ơn chọn màu khác";
                                }
                            }
                            LayerTableRecord tableRec = new LayerTableRecord();
                            SubFunc.AddRegAppTableRecord(doc, "Phuc");
                            ResultBuffer rb = new ResultBuffer(new TypedValue(1001, "Phuc"), new TypedValue(1000, "Layer create by tool"));
                            tableRec.Name = layerName;
                            tableRec.XData = rb;
                            layerTable.UpgradeOpen();
                            id = layerTable.Add(tableRec);
                            trans.AddNewlyCreatedDBObject(tableRec, true);
                            msg = "Thêm thành công";
                        }
                        //Tới đối tượng mới tạo hoặc đối tượng đã có để sửa thuộc tính
                        LayerTableRecord tableRecNew = trans.GetObject(id, OpenMode.ForWrite) as LayerTableRecord;
                        tableRecNew.Color = Color.FromColorIndex(ColorMethod.ByAci, layerInfo.ColorId);
                        tableRecNew.Description = layerInfo.Des;
                        trans.Commit();
                    }
                }
                return msg;
            }
            catch
            {
                return "Thêm thất bại";
            }
        }
        #endregion

        #region Lọc đối tượng chỉ định theo layer
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
                var entities = new List<Entity>();
                using (doc.LockDocument())
                {
                    using (Transaction trans = doc.TransactionManager.StartOpenCloseTransaction())
                    {
                        PromptSelectionResult result;
                        result = doc.Editor.SelectAll(filter);
                        if (result.Status == PromptStatus.OK)
                        {
                            var objIds = result.Value.GetObjectIds();
                            foreach (var objId in objIds)
                            {
                                var entity = trans.GetObject(objId, OpenMode.ForRead) as Entity;
                                if (entity.Layer == layerName)
                                {
                                    entities.Add(entity);
                                }
                            }
                        }
                        trans.Commit();
                    }
                }
                return entities;
            }
            catch (System.Exception ex)
            {
                doc.Editor.WriteMessage(ex.Message);
                return null;
            }
        }
        #endregion

        #region Lấy các layer được tạo bởi winform
        /// <summary>
        /// Hàm lấy layer tạo bởi tool
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
                    try
                    {
                        var layerTable = trans.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                        foreach (var layerId in layerTable)
                        {
                            var layer = trans.GetObject(layerId, OpenMode.ForRead) as LayerTableRecord;
                            try
                            {
                                // Lấy layer theo xdata đã truyền vào trước đó
                                var data = layer.XData.AsArray();
                                if ((string)data[0].Value == "Phuc" && (string)data[1].Value == "Layer create by tool")
                                {
                                    var layerInfo = new LayerInfo();
                                    layerInfo.Name = layer.Name;
                                    layerInfo.ColorId = layer.Color.ColorIndex;
                                    layerInfo.Des = layer.Description;
                                    layerInfos.Add(layerInfo);
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        trans.Commit();
                        return layerInfos;
                    }
                    catch (System.Exception ex)
                    {
                        doc.Editor.WriteMessage(ex.Message);
                        trans.Abort();
                        return null;
                    }
                }
            }
        }
        #endregion

        #region Lấy tổng chu vi và diện tích các đối tượng theo từng layer tạo bởi winform
        /// <summary>
        /// Hàm lấy tổng chu vi và diện tích các đối tượng theo từng layer
        /// </summary>
        /// <param name="doc">document</param>
        /// <param name="layerInfos">list layer</param>
        /// <returns></returns>
        public static List<LayerObjectInfo> GetObjectPropertiesByLayer(Document doc, List<LayerInfo> layerInfos)
        {
            if (layerInfos == null) return null;
            var layerObjs = new List<LayerObjectInfo>();
            using (doc.LockDocument())
            {
                using (var trans = doc.Database.TransactionManager.StartOpenCloseTransaction())
                {
                    foreach (var layer in layerInfos)
                    {
                        if (layer == null) continue;
                        var perimeter = 0.0;
                        var area = 0.0;
                        var layerObject = new LayerObjectInfo();
                        layerObject.LayerName = layer.Name;
                        try
                        {
                            //Chiều dài các line cùng layer
                            perimeter += LineFunc.LineProperties(layer, doc);
                            //Chiều dài các đường cong cùng layer
                            perimeter += ArcFunc.ArcProperties(layer, doc);
                            //Chiều dài và diện tích các pline cùng layer
                            var plineProp = PolylineFunc.PlineProperties(layer, doc);
                            perimeter += plineProp.Perimeter;
                            area += plineProp.Area;
                            //Chiều dài và diện tích các đường tròn cùng layer
                            var circleProp = CircleFunc.CircleProperties(layer, doc);
                            perimeter += circleProp.Perimeter;
                            area += circleProp.Area;
                            layerObject.Perimeter = perimeter;
                            layerObject.Area = area;
                            layerObjs.Add(layerObject);
                        }
                        catch (System.Exception ex)
                        {
                            doc.Editor.WriteMessage(ex.Message);
                            trans.Abort();
                        }
                    }
                    return layerObjs;
                }
            }
        }
        #endregion
    }
}
