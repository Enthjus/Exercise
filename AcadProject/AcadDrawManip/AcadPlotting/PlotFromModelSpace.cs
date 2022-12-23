using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.PlottingServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using LibraryCad.DocumentManager;
using LibraryCad.FileManip.Plot;
using LibraryCad.ObjectsFunc.BlockObject;
using LibraryCad.Sub;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
                using (doc.LockDocument())
                {
                    var result = BlockFunc.PickBlock(ed);
                    List<BlockReference> blkRefs = new List<BlockReference>();
                    using (Transaction trans = db.TransactionManager.StartTransaction())
                    {
                        BlockReference blkRef = trans.GetObject(result.ObjectId, OpenMode.ForRead) as BlockReference;
                        BlockTableRecord blkTblRec = trans.GetObject(blkRef.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                        ObjectIdCollection blockReferenceIds = blkTblRec.GetBlockReferenceIds(false, false);
                        string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        Environment.SetEnvironmentVariable("MYDOCUMENTS", documents);

                        var ofd = new SaveFileDialog("Select a file using an OpenFileDialog", documents,
                                      "",
                                      "File Date Test T22",
                                      SaveFileDialog.SaveFileDialogFlags.DefaultIsFolder // .AllowMultiple
                                    );
                        ofd.ShowDialog();
                        string remoteFileName = ofd.Filename;
                        PlotFunc.PlotMSheet(doc, "DWG To PDF.pc3", "ANSI_A_(8.50_x_11.00_Inches)", remoteFileName, blockReferenceIds);
                        //int i = 1;
                        //foreach (BlockReference blkRef in blkRefs)
                        //{
                        //    var minPt = new Point2d(blkRef.GeometricExtents.MinPoint.X, blkRef.GeometricExtents.MinPoint.Y);
                        //    var maxPt = new Point2d(blkRef.GeometricExtents.MaxPoint.X, blkRef.GeometricExtents.MaxPoint.Y);
                        //    i++;
                        //}
                        //foreach (ObjectId blkRefId in blockReferenceIds)
                        //{
                        //    BlockReference blkRefClone = trans.GetObject(blkRefId, OpenMode.ForWrite) as BlockReference;
                        //    blkRefs.Add(blkRefClone);
                        //}
                    }
                }
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage(ex.Message);
            }
        }

        [CommandMethod("mplot")]

        static public void MultiSheetPlot()

        {

            Document doc =

              Application.DocumentManager.MdiActiveDocument;

            Editor ed = doc.Editor;

            Database db = doc.Database;


            Transaction tr =

              db.TransactionManager.StartTransaction();

            using (tr)

            {

                BlockTable bt =

                  (BlockTable)tr.GetObject(

                    db.BlockTableId,

                    OpenMode.ForRead

                  );


                PlotInfo pi = new PlotInfo();

                PlotInfoValidator piv =

                  new PlotInfoValidator();

                piv.MediaMatchingPolicy =

                  MatchingPolicy.MatchEnabled;


                // A PlotEngine does the actual plotting

                // (can also create one for Preview)


                if (PlotFactory.ProcessPlotState ==

                    ProcessPlotState.NotPlotting)

                {

                    PlotEngine pe =

                      PlotFactory.CreatePublishEngine();

                    using (pe)

                    {

                        // Create a Progress Dialog to provide info

                        // and allow thej user to cancel


                        PlotProgressDialog ppd =

                          new PlotProgressDialog(false, 1, true);

                        using (ppd)

                        {

                            ObjectIdCollection layoutsToPlot =

                              new ObjectIdCollection();


                            foreach (ObjectId btrId in bt)

                            {

                                BlockTableRecord btr =

                                  (BlockTableRecord)tr.GetObject(

                                    btrId,

                                    OpenMode.ForRead

                                  );

                                if (btr.IsLayout &&

                                    btr.Name.ToUpper() !=

                                      BlockTableRecord.ModelSpace.ToUpper())

                                {

                                    layoutsToPlot.Add(btrId);

                                }

                            }


                            int numSheet = 1;


                            foreach (ObjectId btrId in layoutsToPlot)

                            {

                                BlockTableRecord btr =

                                  (BlockTableRecord)tr.GetObject(

                                    btrId,

                                    OpenMode.ForRead

                                  );

                                Layout lo =

                                  (Layout)tr.GetObject(

                                    btr.LayoutId,

                                    OpenMode.ForRead

                                  );


                                // We need a PlotSettings object

                                // based on the layout settings

                                // which we then customize


                                PlotSettings ps =

                                  new PlotSettings(lo.ModelType);

                                ps.CopyFrom(lo);


                                // The PlotSettingsValidator helps

                                // create a valid PlotSettings object


                                PlotSettingsValidator psv =

                                  PlotSettingsValidator.Current;


                                // We'll plot the extents, centered and

                                // scaled to fit


                                psv.SetPlotType(

                                  ps,

                                Autodesk.AutoCAD.DatabaseServices.PlotType.Extents

                                );

                                psv.SetUseStandardScale(ps, true);

                                psv.SetStdScaleType(ps, StdScaleType.ScaleToFit);

                                psv.SetPlotCentered(ps, true);


                                // We'll use the standard DWFx PC3, as

                                // this supports multiple sheets


                                psv.SetPlotConfigurationName(

                                  ps,

                                  "DWG To PDF.pc3",

                                  "ANSI_A_(8.50_x_11.00_Inches)"

                                );


                                // We need a PlotInfo object

                                // linked to the layout


                                pi.Layout = btr.LayoutId;


                                // Make the layout we're plotting current


                                LayoutManager.Current.CurrentLayout =

                                  lo.LayoutName;


                                // We need to link the PlotInfo to the

                                // PlotSettings and then validate it


                                pi.OverrideSettings = ps;

                                piv.Validate(pi);


                                if (numSheet == 1)

                                {

                                    ppd.set_PlotMsgString(

                                      PlotMessageIndex.DialogTitle,

                                      "Custom Plot Progress"

                                    );

                                    ppd.set_PlotMsgString(

                                      PlotMessageIndex.CancelJobButtonMessage,

                                      "Cancel Job"

                                    );

                                    ppd.set_PlotMsgString(

                                      PlotMessageIndex.CancelSheetButtonMessage,

                                      "Cancel Sheet"

                                    );

                                    ppd.set_PlotMsgString(

                                      PlotMessageIndex.SheetSetProgressCaption,

                                      "Sheet Set Progress"

                                    );

                                    ppd.set_PlotMsgString(

                                      PlotMessageIndex.SheetProgressCaption,

                                      "Sheet Progress"

                                    );

                                    ppd.LowerPlotProgressRange = 0;

                                    ppd.UpperPlotProgressRange = 100;

                                    ppd.PlotProgressPos = 0;


                                    // Let's start the plot, at last


                                    ppd.OnBeginPlot();

                                    ppd.IsVisible = true;

                                    pe.BeginPlot(ppd, null);


                                    // We'll be plotting a single document


                                    pe.BeginDocument(

                                      pi,

                                      doc.Name,

                                      null,

                                      1,

                                      true, // Let's plot to file

                                      "D:\\MyPlot\\pppp"

                                    );

                                }


                                // Which may contain multiple sheets


                                ppd.StatusMsgString =

                                  "Plotting " +

                                  doc.Name.Substring(

                                    doc.Name.LastIndexOf("\\") + 1

                                  ) +

                                  " - sheet " + numSheet.ToString() +

                                  " of " + layoutsToPlot.Count.ToString();


                                ppd.OnBeginSheet();


                                ppd.LowerSheetProgressRange = 0;

                                ppd.UpperSheetProgressRange = 100;

                                ppd.SheetProgressPos = 0;


                                PlotPageInfo ppi = new PlotPageInfo();

                                pe.BeginPage(

                                  ppi,

                                  pi,

                                  (numSheet == layoutsToPlot.Count),

                                  null

                                );

                                pe.BeginGenerateGraphics(null);

                                ppd.SheetProgressPos = 50;

                                pe.EndGenerateGraphics(null);


                                // Finish the sheet

                                pe.EndPage(null);

                                ppd.SheetProgressPos = 100;

                                ppd.OnEndSheet();

                                numSheet++;

                            }


                            // Finish the document


                            pe.EndDocument(null);


                            // And finish the plot


                            ppd.PlotProgressPos = 100;

                            ppd.OnEndPlot();

                            pe.EndPlot(null);

                        }

                    }

                }

                else

                {

                    ed.WriteMessage(

                      "\nAnother plot is in progress."

                    );

                }

            }

        }

        #region Các lệnh cơ bản
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
        #endregion
    }
}
