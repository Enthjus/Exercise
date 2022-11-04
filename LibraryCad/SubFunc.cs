using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System.Text.RegularExpressions;
using LibraryCad.Models;
using System.Collections.Generic;
using System;

namespace LibraryCad
{
    public class Functions
    {
        /// <summary>
        /// Hàm tạo text
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="db">Database Doc</param>
        /// <param name="text">Chuỗi</param>
        /// <param name="point">Điểm</param>
        public static void CreateText(Document doc, string text, Point3d point)
        {
            var db = doc.Database;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                try
                {
                    // Open the Block table for read
                    BlockTable acBlkTbl;
                    acBlkTbl = trans.GetObject(db.BlockTableId,
                                                    OpenMode.ForRead) as BlockTable;

                    // Open the Block table record Model space for write
                    BlockTableRecord acBlkTblRec;
                    acBlkTblRec = trans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                    OpenMode.ForWrite) as BlockTableRecord;


                    // Create a single-line text object
                    using (DBText acText = new DBText())
                    {
                        acText.Position = point;
                        acText.Height = 0.5;
                        acText.TextString = text;
                        acText.ColorIndex = 1;

                        acBlkTblRec.AppendEntity(acText);
                        trans.AddNewlyCreatedDBObject(acText, true);
                    }
                    trans.Commit();
                }
                catch (System.Exception ex)
                {
                    doc.Editor.WriteMessage(ex.Message);
                    trans.Abort();
                }
            }
        }

        /// <summary>
        /// Hàm bắt điểm người dùng chọn
        /// </summary>
        /// <param name="doc">Document</param>
        public static PointInf PickPoint(Document doc)
        {
            using (OpenCloseTransaction trans = doc.Database.TransactionManager.StartOpenCloseTransaction())
            {
                try
                {
                    PointInf pInf = new PointInf();
                    PromptPointOptions pPtOpts = new PromptPointOptions("");
                    PromptPointResult pPtRes;
                    pPtOpts.Message = "\nChọn điểm: ";
                    pPtRes = doc.Editor.GetPoint(pPtOpts);
                    Point3d point = pPtRes.Value;
                    if (pPtRes.Status == PromptStatus.Cancel) pInf.status = false;
                    else pInf.status = true;
                    pInf.point = point;
                    trans.Commit();
                    return pInf;
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
        /// Hàm lấy chuỗi từ editor
        /// </summary>
        /// <param name="doc">Document</param>
        /// <returns></returns>
        public static string GetString(Document doc)
        {
            // Get string from editor
            PromptStringOptions pStrOpts;
            pStrOpts = new PromptStringOptions("\nNhập chuỗi: ");
            pStrOpts.AllowSpaces = true;
            PromptResult pStrRes = doc.Editor.GetString(pStrOpts);
            return pStrRes.StringResult;
        }

        /// <summary>
        /// Hàm gộp các chuỗi được chọn
        /// </summary>
        /// <param name="doc">Document</param>
        /// <returns></returns>
        public static string MergeString(Document doc)
        {
            using (var trans = doc.Database.TransactionManager.StartOpenCloseTransaction())
            {
                // Request for objects to be selected in the drawing area
                PromptSelectionResult acSSPrompt = doc.Editor.GetSelection();

                // If the prompt status is OK, objects were selected
                if (acSSPrompt.Status == PromptStatus.OK)
                {
                    SelectionSet acSSet = acSSPrompt.Value;

                    var mergeText = "";

                    // Step through the objects in the selection set
                    foreach (SelectedObject acSSObj in acSSet)
                    {
                        // Check to make sure a valid SelectedObject object was returned
                        if (acSSObj != null && trans.GetObject(acSSObj.ObjectId, OpenMode.ForRead).GetType() == typeof(DBText))
                        {
                            // Open the selected object for write
                            DBText acText = trans.GetObject(acSSObj.ObjectId,
                                                                OpenMode.ForRead) as DBText;

                            if (acText != null || acText.TextString.Trim() != "")
                            {
                                // Merge text
                                mergeText += acText.TextString + " ";
                            }
                        }
                    }

                    mergeText = mergeText.Trim();

                    return mergeText;
                }
                return "";
            }
        }

        /// <summary>
        /// Hàm lấy các đối tượng được chọn
        /// </summary>
        /// <param name="doc">Document</param>
        /// <returns>Set các đối tượng</returns>
        public static SelectionSet GetListSelection(Document doc)
        {
            // Request for objects to be selected in the drawing area
            PromptSelectionResult acSSPrompt = doc.Editor.GetSelection();

            // If the prompt status is OK, objects were selected
            if (acSSPrompt.Status == PromptStatus.OK)
            {
                SelectionSet acSSet = acSSPrompt.Value;
                return acSSet;
            }
            return null;
        }

        /// <summary>
        /// Hàm convert selectionSet thành list Line
        /// </summary>
        /// <param name="selectSet">Các đối tượng được chọn</param>
        /// <param name="doc">Document</param>
        /// <returns>List Line</returns>
        public static List<Line> ParseSelectionToListLine(SelectionSet selectSet, Document doc)
        {
            using (var trans = doc.Database.TransactionManager.StartOpenCloseTransaction())
            {
                var lines = new List<Line>();
                // Step through the objects in the selection set
                foreach (SelectedObject select in selectSet)
                {
                    // Check object có null hay không và có phải là line không
                    if (select != null && trans.GetObject(select.ObjectId, OpenMode.ForRead).GetType() == typeof(Line))
                    {
                        // Open the selected object for write
                        Line acText = trans.GetObject(select.ObjectId, OpenMode.ForRead) as Line;

                        if (acText.Length != 0)
                        {
                            lines.Add(acText);
                        }
                    }
                }
                return lines;
            }
        }

        /// <summary>
        /// Hàm check số
        /// </summary>
        /// <param name="num">chuỗi số thực</param>
        /// <returns>bool</returns>
        public static bool CheckIfNumber(string num)
        {
            Regex rgx = new Regex(@"(\d+(?:\.\d+)?)");
            if (rgx.IsMatch(num)) return true;
            return false;
        }

        /// <summary>
        /// Hàm check số các đối tượng được chọn
        /// </summary>
        /// <param name="sSet">Các đối tượng được chọn</param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static bool CheckIfSelectionSetIsNumber(SelectionSet sSet, Document doc)
        {
            using (var trans = doc.Database.TransactionManager.StartOpenCloseTransaction())
            {
                foreach (SelectedObject acSSObj in sSet)
                {
                    // Check to make sure a valid SelectedObject object was returned
                    if (acSSObj != null)
                    {
                        // Open the selected object for write
                        DBText acText = trans.GetObject(acSSObj.ObjectId,
                                                            OpenMode.ForRead) as DBText;
                        // Nếu không phải số thì trả về false
                        if (!CheckIfNumber(acText.TextString)) return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Hàm lấy số từ editor
        /// </summary>
        /// <param name="doc">Document</param>
        /// <returns></returns>
        public static double GetNum(Document doc)
        {
            while (1 == 1)
            {
                // Lấy số nhập từ editor
                PromptStringOptions pStrOpts;
                pStrOpts = new PromptStringOptions("\nNhập số: ");
                pStrOpts.AllowSpaces = false;
                PromptResult pStrRes = doc.Editor.GetString(pStrOpts);
                if (CheckIfNumber(pStrRes.StringResult))
                {
                    return double.Parse(pStrRes.StringResult);
                }
            }
        }

        /// <summary>
        /// Hàm convert selectionSet thành list Circle
        /// </summary>
        /// <param name="selectSet">Các đối tượng được chọn</param>
        /// <param name="doc">Document</param>
        /// <returns>List Line</returns>
        public static List<Circle> ParseSelectionToListCircle(Document doc)
        {
            using (var trans = doc.Database.TransactionManager.StartOpenCloseTransaction())
            {
                var selSet = GetListSelection(doc);

                if(selSet == null) return null;

                var lines = new List<Circle>();
                // Step through the objects in the selection set
                foreach (SelectedObject select in selSet)
                {
                    // Check object có null hay không và có phải là line không
                    if (select != null && trans.GetObject(select.ObjectId, OpenMode.ForRead).GetType() == typeof(Circle))
                    {
                        // Open the selected object for write
                        Circle acText = trans.GetObject(select.ObjectId, OpenMode.ForRead) as Circle;

                        if (acText != null)
                        {
                            lines.Add(acText);
                        }
                    }
                }
                return lines;
            }
        }

        /// <summary>
        /// Hàm vẽ đường tròn
        /// </summary>
        /// <param name="doc"></param>
        public static void DrawCircle(Document doc)
        {
            using(var trans = doc.Database.TransactionManager.StartTransaction())
            {
                try
                {
                    doc.Editor.WriteMessage("Vẽ đường tròn!");
                    BlockTable bt = trans.GetObject(doc.Database.BlockTableId, OpenMode.ForRead) as BlockTable;

                    BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    var circleRad = GetNum(doc);
                    var centerPoint = PickPoint(doc);

                    if (!centerPoint.status) return;

                    using (var circle = new Circle())
                    {
                        circle.Radius = circleRad;
                        circle.Center = centerPoint.point;

                        btr.AppendEntity(circle);
                        trans.AddNewlyCreatedDBObject(circle, true);
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
        /// Hàm vẽ tam giác cân nội tiếp đường tròn
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="circle">Đường tròn</param>
        public static void TriangleInscribedInCircle(Document doc, Circle circle)
        {
            using (var trans = doc.Database.TransactionManager.StartTransaction())
            {
                doc.Editor.WriteMessage("Vẽ tam giác nội tiếp đường tròn!");
                BlockTable bt = trans.GetObject(doc.Database.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                // Vẽ tam giác đều nội tiếp đường tròn
                using (var pline = new Polyline())
                {
                    pline.SetDatabaseDefaults();
                    pline.AddVertexAt(0, new Point2d(circle.Center.X, circle.Center.Y + circle.Radius), 0, 0, 0);
                    pline.AddVertexAt(1, new Point2d(circle.Center.X + (circle.Radius * LibraryCad.Functions.squareRoot(3) / 2), circle.Center.Y - (circle.Radius / 2)), 0, 0, 0);
                    pline.AddVertexAt(2, new Point2d(circle.Center.X - (circle.Radius * LibraryCad.Functions.squareRoot(3) / 2), circle.Center.Y - (circle.Radius / 2)), 0, 0, 0);
                    //pline.AddVertexAt(3, new Point2d(circle.Center.X, circle.Center.Y + circle.Radius), 0, 0, 0);
                    pline.Closed = true;
                    btr.AppendEntity(pline);
                    trans.AddNewlyCreatedDBObject(pline, true);
                }

                trans.Commit();
            }
        }

        /// <summary>
        /// Hàm vẽ tam giác cân ngoại tiếp đường tròn
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="circle">Đường tròn</param>
        public static void TriangleCircumscribedAboutCircle(Document doc, Circle circle)
        {
            using (var trans = doc.Database.TransactionManager.StartTransaction())
            {
                doc.Editor.WriteMessage("Vẽ tam giác nội tiếp đường tròn!");
                BlockTable bt = trans.GetObject(doc.Database.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                // Vẽ tam giác đều nội tiếp đường tròn
                using (var pline = new Polyline())
                {
                    pline.SetDatabaseDefaults();
                    pline.AddVertexAt(0, new Point2d(circle.Center.X - (circle.Radius * LibraryCad.Functions.squareRoot(3)), circle.Center.Y - circle.Radius), 0, 0, 0);
                    pline.AddVertexAt(1, new Point2d(circle.Center.X + (circle.Radius * LibraryCad.Functions.squareRoot(3)), circle.Center.Y - circle.Radius), 0, 0, 0);
                    pline.AddVertexAt(2, new Point2d(circle.Center.X, circle.Center.Y + (circle.Radius * 2)), 0, 0, 0);
                    pline.AddVertexAt(3, new Point2d(circle.Center.X - (circle.Radius * LibraryCad.Functions.squareRoot(3)), circle.Center.Y - circle.Radius), 0, 0, 0);


                    //pline.ObjectId;
                    //pline.Handle

                    btr.AppendEntity(pline);
                    trans.AddNewlyCreatedDBObject(pline, true);
                }

                trans.Commit();
            }
        }

        // Check SelectionSet có số 0 hay không nếu có trả về true
        public static bool CheckIfHasZero(SelectionSet sSet, Transaction trans)
        {
            foreach (SelectedObject acSSObj in sSet)
            {
                // Check to make sure a valid SelectedObject object was returned
                if (acSSObj != null)
                {
                    // Open the selected object for write
                    DBText acText = trans.GetObject(acSSObj.ObjectId,
                                                        OpenMode.ForRead) as DBText;
                    // Nếu không phải số thì trả về false
                    if (CheckIfNumber(acText.TextString) && double.Parse(acText.TextString) == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Check SelectionSet nếu số đầu là 0 thì trả về true
        public static bool CheckIfFirstNumIsZero(SelectionSet sSet, Transaction trans)
        {
            // Open the selected object for write
            DBText acText = trans.GetObject(sSet[0].ObjectId,
                                                OpenMode.ForRead) as DBText;
            // Nếu không phải số thì trả về false
            if (CheckIfNumber(acText.TextString) && double.Parse(acText.TextString) == 0)
            {
                return true;
            }
            return false;
        }

        // Check SelectionSet từ số thứ 2 trở đi nếu có số 0 thì trả về true
        public static bool CheckIfOtherNumIsZero(SelectionSet sSet, Transaction trans)
        {
            for (int i = 1; i < sSet.Count; i++)
            {
                // Open the selected object for write
                DBText acText = trans.GetObject(sSet[i].ObjectId,
                                                    OpenMode.ForRead) as DBText;
                // Nếu không phải số thì trả về false
                if (CheckIfNumber(acText.TextString) && double.Parse(acText.TextString) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        // Tính căn bậc hai
        public static double squareRoot(int number)
        {
            double temp;
            double sr = number / 2;

            do
            {
                temp = sr;
                sr = (temp + (number / temp)) / 2;
            } while ((temp - sr) != 0);
            return sr;
        }
    }
}
