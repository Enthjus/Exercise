using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace LibraryCad
{
    public class DimensionFunc
    {
        /// <summary>
        /// Hàm lấy dimension trong các đối tượng được chọn
        /// </summary>
        /// <param name="doc">Document</param>
        /// <returns></returns>
        public static List<Dimension> GetDimensions(Document doc)
        {
            using (var trans = doc.Database.TransactionManager.StartOpenCloseTransaction())
            {
                try
                {
                    // Tạo filter để lọc các đối tượng truyền vào
                    List<Dimension> dimensions = new List<Dimension>();
                    var typeValues = new TypedValue[]
                    {
                        new TypedValue((int)DxfCode.Start, "DIMENSION")
                    };
                    var slft = new SelectionFilter(typeValues);
                    var selSet = SubFunc.GetListSelection(doc, "- Chọn các dimension mà bạn muốn tính tồng: ", slft);
                    if (selSet == null) return null;
                    // Chuyển sang dạnh dimension rồi add vào list
                    foreach (ObjectId sel in selSet)
                    {
                        Dimension dimension = trans.GetObject(sel, OpenMode.ForRead) as Dimension;
                        if (dimension != null)
                        {
                            dimensions.Add(dimension);
                        }
                    }
                    trans.Commit();
                    return dimensions;
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
        /// Hàm tạo dim trên nhiều đoạn thẳng
        /// </summary>
        /// <param name="lines">list các đoạn thẳng</param>
        /// <param name="doc">Document</param>
        public static void DimMultiLine(List<Line> lines, Document doc)
        {
            using (doc.LockDocument())
            {
                foreach (Line line in lines)
                {
                    DimLine(line, doc);
                }
            }
        }

        /// <summary>
        /// Hàm tạo dim trên 1 đoạn thẳng
        /// </summary>
        /// <param name="line">đoạn thẳng</param>
        /// <param name="doc">Document</param>
        public static void DimLine(Line line, Document doc)
        {
            Database db = doc.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //Mở block table để đọc
                BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                // Mở Block table record Model space để ghi 
                BlockTableRecord tableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                // Tạo vector để bắt điểm dim
                Vector3d ang = line.GetFirstDerivative(line.GetParameterAtPoint(line.StartPoint));
                // Đặt độ dài cho vector
                ang = ang.GetNormal() * 1;
                // Xoay vector
                ang = ang.TransformBy(Matrix3d.Rotation(System.Math.PI / 2.0, line.Normal, Point3d.Origin));
                // Tạo aligned dimension
                using (AlignedDimension algDim = new AlignedDimension(line.StartPoint, line.EndPoint, line.StartPoint + ang, "", db.Dimstyle))
                {
                    RegAppTable acRegAppTbl = trans.GetObject(db.RegAppTableId, OpenMode.ForRead) as RegAppTable;
                    // Kiểm tra xem app "ACAD_DSTYLE_DIMROTATE" đã tồn tại hay chưa và nếu chưa thì thêm vào RegApp table
                    if (acRegAppTbl.Has("ACAD_DSTYLE_DIMALIGNED") == false)
                    {
                        using (RegAppTableRecord acRegAppTblRec = new RegAppTableRecord())
                        {
                            acRegAppTblRec.Name = "ACAD_DSTYLE_DIMALIGNED";
                            trans.GetObject(db.RegAppTableId, OpenMode.ForWrite);
                            acRegAppTbl.Add(acRegAppTblRec);
                            trans.AddNewlyCreatedDBObject(acRegAppTblRec, true);
                        }
                    }
                    // Tạo result buffer để định nghĩa Xdata
                    ResultBuffer rb = new ResultBuffer();
                    rb.Add(new TypedValue((int)DxfCode.ExtendedDataRegAppName, "ACAD_DSTYLE_DIMALIGNED"));
                    rb.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 387));
                    rb.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 3));
                    rb.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 389));
                    rb.Add(new TypedValue((int)DxfCode.ExtendedDataXCoordinate, new Point3d(-1.26985, -3.91514, 0)));
                    // Thêm Xdata vào dimension
                    algDim.XData = rb;
                    // thêm đối tượng mới vào Model space và transaction
                    tableRec.AppendEntity(algDim); 
                    trans.AddNewlyCreatedDBObject(algDim, true);
                }
                trans.Commit();
            }
        }

        /// <summary>
        /// Hàm tạo dim trên nhiều đường cong
        /// </summary>
        /// <param name="arcs">list các đường cong</param>
        /// <param name="doc">Document</param>
        public static void DimMultiArc(List<Autodesk.AutoCAD.DatabaseServices.Arc> arcs, Document doc)
        {
            using (doc.LockDocument())
            {
                foreach (Autodesk.AutoCAD.DatabaseServices.Arc arc in arcs)
                {
                    DimArc(arc, doc);
                }
            }
        }

        /// <summary>
        /// Hàm tạo dim trên 1 đường cong
        /// </summary>
        /// <param name="arcs">đường cong</param>
        /// <param name="doc">Document</param>
        public static void DimArc(Autodesk.AutoCAD.DatabaseServices.Arc arc, Document doc)
        {
            Database db = doc.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //Mở block table để đọc
                BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                // Mở Block table record Model space để ghi 
                BlockTableRecord tableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                // Tạo vector để đặt điểm dim
                Vector3d ang = arc.GetFirstDerivative(arc.GetParameterAtPoint(LibraryCad.Arc.ArcFunc.GetMidpoint(arc)));
                ang = ang.GetNormal() * -1;
                ang = ang.TransformBy(Matrix3d.Rotation(System.Math.PI / 2.0, arc.Normal, Point3d.Origin));
                // Tạo arc dimension
                using (ArcDimension arcDim = new ArcDimension(arc.Center, arc.StartPoint, arc.EndPoint, LibraryCad.Arc.ArcFunc.GetMidpoint(arc) + ang, "", db.Dimstyle))
                {
                    arcDim.TextPosition = arcDim.ArcPoint;
                    RegAppTable regAppTable = trans.GetObject(db.RegAppTableId, OpenMode.ForRead) as RegAppTable;
                    // Kiểm tra xem app "ACAD_DSTYLE_DIMARC" đã được tạo chưa và nếu chưa thì thêm vào RegApp table
                    if (regAppTable.Has("ACAD_DSTYLE_DIMARC") == false)
                    {
                        using (RegAppTableRecord acRegAppTblRec = new RegAppTableRecord())
                        {
                            acRegAppTblRec.Name = "ACAD_DSTYLE_DIMARC";
                            trans.GetObject(db.RegAppTableId, OpenMode.ForWrite);
                            regAppTable.Add(acRegAppTblRec);
                            trans.AddNewlyCreatedDBObject(acRegAppTblRec, true);
                        }
                    }
                    // Tạo result buffer để định nghĩa Xdata
                    ResultBuffer rb = new ResultBuffer();
                    rb.Add(new TypedValue((int)DxfCode.ExtendedDataRegAppName, "ACAD_DSTYLE_DIMARC"));
                    rb.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 387));
                    rb.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 3));
                    rb.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 389));
                    rb.Add(new TypedValue((int)DxfCode.ExtendedDataXCoordinate, new Point3d(-1.26985, 3.91514, 0)));
                    // Thêm Xdata vào dimension
                    arcDim.XData = rb;
                    // thêm đối tượng mới vào Model space và transaction
                    tableRec.AppendEntity(arcDim);
                    trans.AddNewlyCreatedDBObject(arcDim, true);
                }

                trans.Commit();
            }
        }

        /// <summary>
        /// Hàm tạo dim từ các điểm trên polyline
        /// </summary>
        /// <param name="points">list các điểm</param>
        /// <param name="doc">Document</param>
        public static void DimPolyline(List<Point3d> points, Document doc, int dimStatus)
        {
            Database db = doc.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord tableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                int angScale;
                // Check dim status để đặt vị trí dim trong hay ngoài
                if (dimStatus == 1)
                {
                    angScale = -1;
                }
                else if (dimStatus == 0)
                {
                    angScale = 1;
                }
                else return;
                var j = 1;
                for (int i = 0; i < points.Count - 1; i++)
                {
                    var line = new Line(points[i], points[j]);
                    // Tạo vector để đặt điểm dim
                    Vector3d ang = line.GetFirstDerivative(line.GetParameterAtPoint(points[i]));
                    ang = ang.GetNormal() * angScale;
                    ang = ang.TransformBy(Matrix3d.Rotation(System.Math.PI / 2.0, line.Normal, Point3d.Origin));
                    // Tạo aligned dimension
                    using (AlignedDimension algDim = new AlignedDimension(points[i], points[j], line.StartPoint + ang, "", db.Dimstyle))
                    {
                        RegAppTable acRegAppTbl = trans.GetObject(db.RegAppTableId, OpenMode.ForRead) as RegAppTable;
                        // Kiểm tra xem app "ACAD_DSTYLE_DIMROTATE_PLINE" đã được tạo chưa và nếu chưa thì thêm vào RegApp table
                        if (acRegAppTbl.Has("ACAD_DSTYLE_DIMROTATE_PLINE") == false)
                        {
                            using (RegAppTableRecord acRegAppTblRec = new RegAppTableRecord())
                            {
                                acRegAppTblRec.Name = "ACAD_DSTYLE_DIMROTATE_PLINE";
                                trans.GetObject(db.RegAppTableId, OpenMode.ForWrite);
                                acRegAppTbl.Add(acRegAppTblRec);
                                trans.AddNewlyCreatedDBObject(acRegAppTblRec, true);
                            }
                        }
                        // Tạo result buffer để định nghĩa Xdata
                        ResultBuffer rb = new ResultBuffer();
                        rb.Add(new TypedValue((int)DxfCode.ExtendedDataRegAppName, "ACAD_DSTYLE_DIMROTATE_PLINE"));
                        rb.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 387));
                        rb.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 3));
                        rb.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 389));
                        rb.Add(new TypedValue((int)DxfCode.ExtendedDataXCoordinate, new Point3d(-1.26985, -3.91514, 0)));
                        algDim.XData = rb;
                        tableRec.AppendEntity(algDim);
                        trans.AddNewlyCreatedDBObject(algDim, true);
                    }
                    j++;
                }
                trans.Commit();
            }
        }

        /// <summary>
        /// Hàm tạo dim đường cong trên polyline
        /// </summary>
        /// <param name="arcs">đường cong</param>
        /// <param name="doc">Document</param>
        public static void DimArc(CircularArc3d circularArc, Document doc, int dimStatus)
        {
            Database db = doc.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord tableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                try
                {
                    var line = new Line(circularArc.StartPoint, circularArc.EndPoint);
                    var midLine = LibraryCad.LineFunc.GetMidpoint(line);
                    Vector3d ang = new Line(circularArc.Center, midLine).Delta;
                    ArcDimension arcDim;
                    RadialDimension radDim;
                    // Check xem người dùng nhập gì để tạo dim bên trong hay ngoài đường cong
                    if (dimStatus == 1)
                    {
                        arcDim = new ArcDimension(circularArc.Center, circularArc.StartPoint, circularArc.EndPoint, midLine - (ang / ang.Length * (1 + (circularArc.Radius - ang.Length))), "", db.Dimstyle);
                        radDim = new RadialDimension(circularArc.Center, midLine, -2, "", db.Dimstyle);
                    }
                    else if (dimStatus == 0)
                    {
                        arcDim = new ArcDimension(circularArc.Center, circularArc.StartPoint, circularArc.EndPoint, midLine + (ang / ang.Length * (1 + (circularArc.Radius - ang.Length))), "", db.Dimstyle);
                        radDim = new RadialDimension(circularArc.Center, midLine, 2, "", db.Dimstyle);
                    }
                    else
                    {
                        return;
                    }
                    // Tạo dim độ dài đường cong
                    using (arcDim)
                    {
                        arcDim.TextPosition = arcDim.ArcPoint;
                        RegAppTable regAppTable = trans.GetObject(db.RegAppTableId, OpenMode.ForRead) as RegAppTable;
                        // Kiểm tra xem app "ACAD_DSTYLE_DIMARC_LENGTH" đã được tạo chưa và nếu chưa thì thêm vào RegApp table
                        if (regAppTable.Has("ACAD_DSTYLE_DIMARC_LENGTH") == false)
                        {
                            using (RegAppTableRecord raTableRec = new RegAppTableRecord())
                            {
                                raTableRec.Name = "ACAD_DSTYLE_DIMARC_LENGTH";
                                trans.GetObject(db.RegAppTableId, OpenMode.ForWrite);
                                regAppTable.Add(raTableRec);
                                trans.AddNewlyCreatedDBObject(raTableRec, true);
                            }
                        }
                        // Tạo result buffer để định nghĩa Xdata
                        ResultBuffer acResBuf = new ResultBuffer();
                        acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataRegAppName, "ACAD_DSTYLE_DIMARC_LENGTH"));
                        acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 387));
                        acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 3));
                        acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 389));
                        acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataXCoordinate, new Point3d(-1.26985, 3.91514, 0)));
                        arcDim.XData = acResBuf;
                        tableRec.AppendEntity(arcDim);
                        trans.AddNewlyCreatedDBObject(arcDim, true);
                    }
                    // Tạo dim kích thước bán kính
                    using (radDim)
                    {
                        RegAppTable regAppTable = trans.GetObject(db.RegAppTableId, OpenMode.ForRead) as RegAppTable;
                        // Kiểm tra xem app "ACAD_DSTYLE_DIMARC_RADIUS" đã được tạo chưa và nếu chưa thì thêm vào RegApp table
                        if (regAppTable.Has("ACAD_DSTYLE_DIMARC_RADIUS") == false)
                        {
                            using (RegAppTableRecord raTableRec = new RegAppTableRecord())
                            {
                                raTableRec.Name = "ACAD_DSTYLE_DIMARC_RADIUS";
                                trans.GetObject(db.RegAppTableId, OpenMode.ForWrite);
                                regAppTable.Add(raTableRec);
                                trans.AddNewlyCreatedDBObject(raTableRec, true);
                            }
                        }
                        // Tạo result buffer để định nghĩa Xdata
                        ResultBuffer rb = new ResultBuffer();
                        rb.Add(new TypedValue((int)DxfCode.ExtendedDataRegAppName, "ACAD_DSTYLE_DIMARC_RADIUS"));
                        rb.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 387));
                        rb.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 3));
                        rb.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 389));
                        rb.Add(new TypedValue((int)DxfCode.ExtendedDataXCoordinate, new Point3d(-1.26985, 3.91514, 0)));
                        radDim.XData = rb;
                        tableRec.AppendEntity(radDim);
                        trans.AddNewlyCreatedDBObject(radDim, true);
                    }
                    trans.Commit();
                }
                catch(System.Exception ex)
                {
                    doc.Editor.WriteMessage(ex.Message);
                    trans.Abort();
                }
            }
        }

        /// <summary>
        /// Tính tổng kích thước các dim được chọn
        /// </summary>
        /// <param name="doc">Document</param>
        public static void DimensionSum(Document doc)
        {
            Database db = doc.Database;
            using (doc.LockDocument())
            {
                // Tạo filter
                var typeValues = new TypedValue[]
                {
                    new TypedValue((int)DxfCode.Start, "DIMENSION")
                };
                var slft = new SelectionFilter(typeValues);
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    var sum = 0.0;
                    var dimensions = new List<Dimension>();
                    // Lấy đối tượng theo filter
                    var objectIds = SubFunc.GetListSelection(doc, "- Chọn các dim muốm tính tổng giá trị:", slft);
                    if (objectIds == null) return;
                    foreach (var objectId in objectIds)
                    {
                        var dimension = trans.GetObject(objectId, OpenMode.ForRead) as Dimension;
                        if (dimension != null)
                        {
                            dimensions.Add(dimension);
                        }
                    }
                    // Cộng tổng các kích thước dim lại
                    dimensions.Where(dim => dim.Measurement > 0).ToList().ForEach(dimension => sum += dimension.Measurement);
                    // Set layer hiện tại
                    var layer = db.Clayer;
                    // Lấy điểm đặt kết quả
                    var point = LibraryCad.SubFunc.PickPoint(doc);
                    if (point.status == false || point == null) return;
                    LibraryCad.TextFunc.CreateText(doc, System.Math.Round(sum).ToString(), point.point, layer);
                    trans.Commit();
                }
            }
        }

        /// <summary>
        /// Hàm tạo rotate dimension
        /// </summary>
        /// <param name="doc">Document</param>
        public static void CreateRotateDim(Document doc)
        {
            Database db = doc.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord tableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                // Tạo dim dạng rotated
                using (RotatedDimension rotateDim = new RotatedDimension())
                {
                    rotateDim.XLine1Point = new Point3d(0, 0, 0);
                    rotateDim.XLine2Point = new Point3d(6, 3, 0);
                    rotateDim.Rotation = 0.707;
                    rotateDim.DimLinePoint = new Point3d(0, 5, 0);
                    rotateDim.DimensionStyle = db.Dimstyle;
                    tableRec.AppendEntity(rotateDim);
                    trans.AddNewlyCreatedDBObject(rotateDim, true);
                }
                trans.Commit();
            }
        }
    }
}
