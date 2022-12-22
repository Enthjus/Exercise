using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using LibraryCad.DocumentManager;
using LibraryCad.FileManip.Plot;
using LibraryCad.ObjectsFunc.BlockObject;
using LibraryCad.Sub;
using System;
using System.Collections.Generic;
using System.IO;

namespace AcadProject.AcadDrawManip.AcadPlotting
{
    public class PlotFromModelSpace
    {
        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

        [CommandMethod("PlotCurrentLayout")]
        public static void PlotCurrentLayout()
        {
            using (doc.LockDocument())
            {
                var fileName = SubFunc.GetString(doc, "- Nhập tên file muồn xuất ra: ");
                PlotFunc.PlotModel(doc, fileName);
            }
        }

        [CommandMethod("PL")]
        public static void PlotLayout()
        {
            try
            {
                var blk = BlockFunc.PickBlock(doc, "- Chọn block:");
                var blkName = blk.Name;


                TypedValue[] tvBlock = new TypedValue[]
                {
                        new TypedValue((int)DxfCode.Start, "INSERT")
                };
                SelectionFilter filter = new SelectionFilter(tvBlock);
                PromptEntityOptions dd = new PromptEntityOptions("Nhat block");
                dd.SetRejectMessage("\nChi dc chon doi tuong la Block.");
                dd.AddAllowedClass(typeof(BlockReference), true);
                var per = ed.GetEntity(dd);
                string name = per.StringResult;
                List<BlockReference> blkRefs = new List<BlockReference>();
                blkRefs.Add(blk);
                using (doc.LockDocument())
                {
                    using (Transaction trans = db.TransactionManager.StartTransaction())
                    {
                        BlockReference blkResf = trans.GetObject(per.ObjectId, OpenMode.ForRead) as BlockReference;



                        BlockTable blkTbl = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;


                        BlockTableRecord blkTblRec = trans.GetObject(blkTbl[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;
                        foreach (ObjectId id in blkTblRec)
                        {
                            string sd = id.ObjectClass.DxfName;
                            if(id.ObjectClass == RXObject.GetClass(typeof(BlockReference)))
                            {
                                BlockReference blkRef = trans.GetObject(id, OpenMode.ForRead) as BlockReference;
                                if(blkRef.Name == blkName)
                                {
                                    blkRefs.Add((BlockReference)blkRef);
                                }
                            }
                        }
                    }
                    string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    Environment.SetEnvironmentVariable("MYDOCUMENTS", documents);

                    var ofd = new SaveFileDialog("Select a file using an OpenFileDialog", documents,
                                  "",
                                  "File Date Test T22",
                                  SaveFileDialog.SaveFileDialogFlags.DefaultIsFolder // .AllowMultiple
                                );
                    ofd.ShowDialog();
                    string remoteFileName = ofd.Filename;
                    //string plotFile = Path.GetFullPath(Application.DocumentManager.MdiActiveDocument.Database.Filename);
                    //plotFile = plotFile + "-Plot";
                    int i = 1;
                    foreach (BlockReference blkRef in blkRefs)
                    {
                        var minPt = new Point2d(blkRef.GeometricExtents.MinPoint.X, blkRef.GeometricExtents.MinPoint.Y);
                        var maxPt = new Point2d(blkRef.GeometricExtents.MaxPoint.X, blkRef.GeometricExtents.MaxPoint.Y);
                        PlotFunc.xPlot_Khung(minPt, maxPt, "DWG To PDF.pc3", "ANSI_A_(8.50_x_11.00_Inches)", remoteFileName + i.ToString());
                    }
                }
            }
            catch(System.Exception ex)
            {
                ed.WriteMessage(ex.Message);
            }
        }

        [CommandMethod("PlotterLocalMediaNameList")]
        public static void PlotterLocalMediaNameList()
        {
            using (PlotSettings plSettings = new PlotSettings(true))
            {
                PlotSettingsValidator plSetVld = PlotSettingsValidator.Current;
                // Đặt thiết bị vẽ và khổ giấy
                plSetVld.SetPlotConfigurationName(plSettings, "DWF6 ePlot.pc3", "ANSI_A_(8.50_x_11.00_Inches)");
                doc.Editor.WriteMessage("\nCanonical and Local media names: ");
                int cnt = 0;
                foreach (string mediaName in plSetVld.GetCanonicalMediaNameList(plSettings))
                {
                    // Xuất ra tên của media khả dụng cho thiết bị được chỉ định
                    ed.WriteMessage("\n  " + mediaName + " | " + plSetVld.GetLocaleMediaName(plSettings, cnt));
                    cnt = cnt + 1;
                }
            }
        }

        [CommandMethod("PlotterList")]
        public static void PlotterList()
        {
            ed.WriteMessage("\nPlot devices: ");
            foreach (string plotDevice in PlotSettingsValidator.Current.GetPlotDeviceList())
            {
                // Xuất ra tên của thiết bị vẽ khả dụng
                ed.WriteMessage("\n  " + plotDevice);
            }
        }

        [CommandMethod("ChangeLayoutPlotSettings")]
        public static void ChangeLayoutPlotSettings()
        {
            using (doc.LockDocument())
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    // Lấy layout hiện tại
                    LayoutManager acLayoutMgr = LayoutManager.Current;
                    Layout acLayout = trans.GetObject(acLayoutMgr.GetLayoutId(acLayoutMgr.CurrentLayout), OpenMode.ForRead) as Layout;
                    ed.WriteMessage("\nCurrent layout: " + acLayout.LayoutName);
                    ed.WriteMessage("\nCurrent device name: " + acLayout.PlotConfigurationName);

                    // Lấy bản sao của PlotSettings từ layout
                    using (PlotSettings acPlSet = new PlotSettings(acLayout.ModelType))
                    {
                        acPlSet.CopyFrom(acLayout);

                        // Cập nhật thuộc tính PlotConfigurationName của đối tượng PlotSettings
                        PlotSettingsValidator acPlSetVdr = PlotSettingsValidator.Current;
                        acPlSetVdr.SetPlotConfigurationName(acPlSet, "DWG To PDF.pc3", "ANSI_B_(11.00_x_17.00_Inches)");

                        // Thu phóng để hiển thị toàn bộ bài báo
                        acPlSetVdr.SetZoomToPaperOnUpdate(acPlSet, true);

                        // Cập nhật layout
                        trans.GetObject(acLayoutMgr.GetLayoutId(acLayoutMgr.CurrentLayout), OpenMode.ForWrite);
                        acLayout.CopyFrom(acPlSet);
                    }
                    ed.WriteMessage("\nNew device name: " + acLayout.PlotConfigurationName);
                    trans.Commit();
                }
                ed.Regen();
            }
        }

        [CommandMethod("polyview")]
        static public void CreateNonRectangularViewports()
        {
            // Lấy bản vẻ hiện hành và lưu vào biến doc
            Document doc = Application.DocumentManager.MdiActiveDocument;
            // Lấy dữ liệu bản vẽ cùng bộ editor
            Database db = doc.Database;
            Editor ed = doc.Editor;
            // Để truy xuất bản vẽ cần bắt đầu một transaction
            Transaction tr = db.TransactionManager.StartTransaction();
            using (tr)
            {
                // Mở Block table để đọc
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                // Mở Block table record của Paper space để ghi dữ liệu
                BlockTableRecord ps =
               (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.PaperSpace], OpenMode.ForWrite);
                // Tạo một polyline kín để định hình viewport
                var blk = BlockFunc.PickBlock(doc, "- chọn block:");
                // Thêm đối tượng Polyline vào paper space
                ObjectId id = ps.AppendEntity(blk);
                tr.AddNewlyCreatedDBObject(blk, true);
                // Định nghĩ mới đối tượng viewport
                Viewport vp = new Viewport();
                // Thêm viewport vào paper space
                ps.AppendEntity(vp);
                tr.AddNewlyCreatedDBObject(vp, true);
                // Xác định phạm vi đường bao của viewport là polyline kín đã tạo
                vp.NonRectClipEntityId = id;
                vp.NonRectClipOn = true;
                // Mở viewport
                vp.On = true;
                // Lưu thay đổi và đóng transaction
                tr.Commit();
            }
            // Chuyển sang chế độ paper space
            db.TileMode = false;
        }
    }
}
