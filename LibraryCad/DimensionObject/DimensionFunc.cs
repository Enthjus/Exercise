using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

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
            Database db = doc.Database;
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
                BlockTableRecord blockTableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Vector3d ang = line.GetFirstDerivative(line.GetParameterAtPoint(line.StartPoint));
                // scale the vector by 5.0 (so that it's 5 units length)
                ang = ang.GetNormal() * 1;
                // rotate the vector
                ang = ang.TransformBy(Matrix3d.Rotation(System.Math.PI / 2.0, line.Normal, Point3d.Origin));

                // tạo aligned dimension
                using (AlignedDimension algDim = new AlignedDimension(line.StartPoint, line.EndPoint, line.StartPoint + ang, "", db.Dimstyle))
                {
                    RegAppTable acRegAppTbl = trans.GetObject(db.RegAppTableId, OpenMode.ForRead) as RegAppTable;

                    // Check to see if the app "ACAD_DSTYLE_DIMROTATE" is registered and if not add it to the RegApp table
                    if (acRegAppTbl.Has("ACAD_DSTYLE_DIMROTATE") == false)
                    {
                        using (RegAppTableRecord acRegAppTblRec = new RegAppTableRecord())
                        {
                            acRegAppTblRec.Name = "ACAD_DSTYLE_DIMROTATE";


                            trans.GetObject(db.RegAppTableId, OpenMode.ForWrite);

                            acRegAppTbl.Add(acRegAppTblRec);
                            trans.AddNewlyCreatedDBObject(acRegAppTblRec, true);
                        }
                    }

                    // Create a result buffer to define the Xdata
                    ResultBuffer acResBuf = new ResultBuffer();
                    acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataRegAppName, "ACAD_DSTYLE_DIMROTATE"));
                    acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 387));
                    acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 3));
                    acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 389));
                    acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataXCoordinate, new Point3d(-1.26985, -3.91514, 0)));

                    // Thêm Xdata vào dimension
                    algDim.XData = acResBuf;

                    // thêm đối tượng mới vào Model space và transaction
                    blockTableRec.AppendEntity(algDim); 
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
            Database db = doc.Database;
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
                BlockTableRecord blockTableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Vector3d ang = arc.GetFirstDerivative(arc.GetParameterAtPoint(LibraryCad.Arc.ArcFunc.GetMidpoint(arc)));
                // scale the vector by 1 (so that it's 1 units length)
                ang = ang.GetNormal() * -1;
                // rotate the vector
                ang = ang.TransformBy(Matrix3d.Rotation(System.Math.PI / 2.0, arc.Normal, Point3d.Origin));

                // tạo aligned dimension
                using (ArcDimension arcDim = new ArcDimension(arc.Center, arc.StartPoint, arc.EndPoint, LibraryCad.Arc.ArcFunc.GetMidpoint(arc) + ang, "", db.Dimstyle))
                {
                    arcDim.TextPosition = arcDim.ArcPoint;
                    RegAppTable acRegAppTbl = trans.GetObject(db.RegAppTableId, OpenMode.ForRead) as RegAppTable;

                    // Check to see if the app "ACAD_DSTYLE_DIMARC" is registered and if not add it to the RegApp table
                    if (acRegAppTbl.Has("ACAD_DSTYLE_DIMARC") == false)
                    {
                        using (RegAppTableRecord acRegAppTblRec = new RegAppTableRecord())
                        {
                            acRegAppTblRec.Name = "ACAD_DSTYLE_DIMARC";

                            trans.GetObject(db.RegAppTableId, OpenMode.ForWrite);

                            acRegAppTbl.Add(acRegAppTblRec);
                            trans.AddNewlyCreatedDBObject(acRegAppTblRec, true);
                        }
                    }

                    // Create a result buffer to define the Xdata
                    ResultBuffer acResBuf = new ResultBuffer();
                    acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataRegAppName, "ACAD_DSTYLE_DIMARC"));
                    acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 387));
                    acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 3));
                    acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 389));
                    acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataXCoordinate, new Point3d(-1.26985, 3.91514, 0)));

                    // Attach the Xdata to the dimension
                    arcDim.XData = acResBuf;

                    // thêm đối tượng mới vào Model space và transaction
                    blockTableRec.AppendEntity(arcDim);
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
        public static void DimPolyline(List<Point3d> points, Document doc)
        {
            Database db = doc.Database;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //Mở block table để đọc
                BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                // Mở Block table record Model space để ghi 
                BlockTableRecord blockTableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var j = 1;
                for (int i = 0; i < points.Count - 1; i++)
                {
                    var line = new Line(points[i], points[j]);

                    Vector3d ang = line.GetFirstDerivative(line.GetParameterAtPoint(points[i]));
                    // scale the vector by 5.0 (so that it's 5 units length)
                    ang = ang.GetNormal() * 1;
                    // rotate the vector
                    ang = ang.TransformBy(Matrix3d.Rotation(System.Math.PI / 2.0, line.Normal, Point3d.Origin));

                    // tạo aligned dimension
                    using (AlignedDimension algDim = new AlignedDimension(points[i], points[j], line.StartPoint + ang, "", db.Dimstyle))
                    {
                        RegAppTable acRegAppTbl = trans.GetObject(db.RegAppTableId, OpenMode.ForRead) as RegAppTable;

                        // Check to see if the app "ACAD_DSTYLE_DIMROTATE" is registered and if not add it to the RegApp table
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

                        // Create a result buffer to define the Xdata
                        ResultBuffer acResBuf = new ResultBuffer();
                        acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataRegAppName, "ACAD_DSTYLE_DIMROTATE_PLINE"));
                        acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 387));
                        acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 3));
                        acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 389));
                        acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataXCoordinate, new Point3d(-1.26985, -3.91514, 0)));

                        // Thêm Xdata vào dimension
                        algDim.XData = acResBuf;

                        // thêm đối tượng mới vào Model space và transaction
                        blockTableRec.AppendEntity(algDim);
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
        public static void DimArc(CircularArc3d circularArc, Document doc)
        {
            Database db = doc.Database;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //Mở block table để đọc
                BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                // Mở Block table record Model space để ghi 
                BlockTableRecord blockTableRec = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Autodesk.AutoCAD.DatabaseServices.Arc arc = new Autodesk.AutoCAD.DatabaseServices.Arc(circularArc.Center, circularArc.Radius, new Line(circularArc.Center, circularArc.StartPoint).Angle, new Line(circularArc.Center, circularArc.EndPoint).Angle);

                //Vector3d ang = arc.GetFirstDerivative(arc.GetParameterAtPoint(LibraryCad.Arc.ArcFunc.GetMidpoint(arc)));
                //// scale the vector by 1 (so that it's 1 units length)
                //ang = ang.GetNormal() * -1;
                //// rotate the vector
                //ang = ang.TransformBy(Matrix3d.Rotation(System.Math.PI / 2.0, arc.Normal, Point3d.Origin));

                Point3d point3 = arc.StartPoint.Add(arc.EndPoint.GetAsVector()).MultiplyBy(0.5);

                // tạo aligned dimension
                using (ArcDimension arcDim = new ArcDimension(arc.Center, arc.StartPoint, arc.EndPoint, point3/*LibraryCad.Arc.ArcFunc.GetMidpoint(arc) + ang*/, "", db.Dimstyle))
                {
                    arcDim.TextPosition = arcDim.ArcPoint;
                    RegAppTable acRegAppTbl = trans.GetObject(db.RegAppTableId, OpenMode.ForRead) as RegAppTable;

                    // Check to see if the app "ACAD_DSTYLE_DIMARC" is registered and if not add it to the RegApp table
                    if (acRegAppTbl.Has("ACAD_DSTYLE_DIMARC_LENGTH") == false)
                    {
                        using (RegAppTableRecord acRegAppTblRec = new RegAppTableRecord())
                        {
                            acRegAppTblRec.Name = "ACAD_DSTYLE_DIMARC_LENGTH";

                            trans.GetObject(db.RegAppTableId, OpenMode.ForWrite);

                            acRegAppTbl.Add(acRegAppTblRec);
                            trans.AddNewlyCreatedDBObject(acRegAppTblRec, true);
                        }
                    }

                    // Create a result buffer to define the Xdata
                    ResultBuffer acResBuf = new ResultBuffer();
                    acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataRegAppName, "ACAD_DSTYLE_DIMARC_LENGTH"));
                    acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 387));
                    acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 3));
                    acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 389));
                    acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataXCoordinate, new Point3d(-1.26985, 3.91514, 0)));

                    // Attach the Xdata to the dimension
                    arcDim.XData = acResBuf;

                    // thêm đối tượng mới vào Model space và transaction
                    blockTableRec.AppendEntity(arcDim);
                    trans.AddNewlyCreatedDBObject(arcDim, true);
                }

                using (RadialDimension radDim = new RadialDimension(circularArc.Center, LibraryCad.Arc.ArcFunc.GetMidpoint(arc), -1.5, "", db.Dimstyle))
                {
                    RegAppTable acRegAppTbl = trans.GetObject(db.RegAppTableId, OpenMode.ForRead) as RegAppTable;

                    // Check to see if the app "ACAD_DSTYLE_DIMARC" is registered and if not add it to the RegApp table
                    if (acRegAppTbl.Has("ACAD_DSTYLE_DIMARC_RADIUS") == false)
                    {
                        using (RegAppTableRecord acRegAppTblRec = new RegAppTableRecord())
                        {
                            acRegAppTblRec.Name = "ACAD_DSTYLE_DIMARC_RADIUS";

                            trans.GetObject(db.RegAppTableId, OpenMode.ForWrite);

                            acRegAppTbl.Add(acRegAppTblRec);
                            trans.AddNewlyCreatedDBObject(acRegAppTblRec, true);
                        }
                    }

                    // Create a result buffer to define the Xdata
                    ResultBuffer acResBuf = new ResultBuffer();
                    acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataRegAppName, "ACAD_DSTYLE_DIMARC_RADIUS"));
                    acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 387));
                    acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 3));
                    acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, 389));
                    acResBuf.Add(new TypedValue((int)DxfCode.ExtendedDataXCoordinate, new Point3d(-1.26985, 3.91514, 0)));

                    // Attach the Xdata to the dimension
                    radDim.XData = acResBuf;

                    // thêm đối tượng mới vào Model space và transaction
                    blockTableRec.AppendEntity(radDim);
                    trans.AddNewlyCreatedDBObject(radDim, true);
                }

                trans.Commit();
            }
        }
    }
}
