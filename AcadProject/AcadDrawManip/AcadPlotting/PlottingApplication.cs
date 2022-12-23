using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.PlottingServices;
using Autodesk.AutoCAD.Geometry;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;

namespace AcadDrawManip.AcadPlotting.PlottingApplication
{
    public class SimplePlottingCommands
    {
        [DllImport("accore.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "acedTrans")]
        static extern int acedTrans(double[] point, IntPtr fromRb, IntPtr toRb, int disp, double[] result);
        [CommandMethod("winplot")]
        static public void WindowPlot()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;
            //PromptPointOptions ppo = new PromptPointOptions("\nSelect first corner of plot area: ");
            //ppo.AllowNone = false;
            //PromptPointResult ppr = ed.GetPoint(ppo);
            //if (ppr.Status != PromptStatus.OK) return;

            //PromptCornerOptions pco =new PromptCornerOptions("\nSelect second corner of plot area: ",first);
            //ppr = ed.GetCorner(pco);
            //if (ppr.Status != PromptStatus.OK) return;
            int n = 1;
            var pso = new PromptSelectionOptions();
            pso.MessageForAdding = "\nsilahkan select All :";
            var psr = ed.GetSelection(pso);
            if (psr.Status != PromptStatus.OK) return;
            var dic = new Dictionary<Point3d, Point3d>();
            using (var tr = db.TransactionManager.StartTransaction())
            {
                short bgPlot = (short)Application.GetSystemVariable("BACKGROUNDPLOT");
                //set the BACKGROUNDPLOT = 0 temporarily.
                Application.SetSystemVariable("BACKGROUNDPLOT", 0);
                foreach (SelectedObject ss in psr.Value)
                {
                    var br = ss.ObjectId.GetObject(OpenMode.ForRead) as BlockReference;
                    if (br.Bounds.HasValue)
                    {
                        var extents = br.Bounds;
                        var ext1 = extents.Value.MinPoint;
                        var ext2 = extents.Value.MaxPoint;
                        dic.Add(ext1, ext2);
                    }

                }
                if (dic.Count == 0) return;
                // Transform from UCS to DCS
                Point3d first; Point3d second;
                foreach (var item in dic)
                {
                    first = item.Key;
                    second = item.Value;
                    ResultBuffer rbFrom = new ResultBuffer(new TypedValue(5003, 1)), rbTo = new ResultBuffer(new TypedValue(5003, 2));
                    double[] firres = new double[] { 0, 0, 0 };
                    double[] secres = new double[] { 0, 0, 0 };
                    // Transform the first point...
                    acedTrans(first.ToArray(), rbFrom.UnmanagedObject, rbTo.UnmanagedObject, 0, firres);
                    // ... and the second
                    acedTrans(second.ToArray(), rbFrom.UnmanagedObject, rbTo.UnmanagedObject, 0, secres);
                    // We can safely drop the Z-coord at this stage
                    Extents2d window = new Extents2d(firres[0], firres[1], secres[0], secres[1]);

                    // We'll be plotting the current layout
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForRead);
                    Layout lo = (Layout)tr.GetObject(btr.LayoutId, OpenMode.ForRead);
                    // We need a PlotInfo object
                    // linked to the layout
                    PlotInfo pi = new PlotInfo();
                    pi.Layout = btr.LayoutId;
                    // We need a PlotSettings object
                    // based on the layout settings
                    // which we then customize
                    PlotSettings ps = new PlotSettings(lo.ModelType);
                    ps.CopyFrom(lo);
                    // The PlotSettingsValidator helps
                    // create a valid PlotSettings object
                    PlotSettingsValidator psv = PlotSettingsValidator.Current;
                    // We'll plot the extents, centered and
                    // scaled to fit
                    psv.SetPlotWindowArea(ps, window); psv.SetPlotType(ps, Autodesk.AutoCAD.DatabaseServices.PlotType.Window);
                    psv.SetUseStandardScale(ps, true);
                    psv.SetStdScaleType(ps, StdScaleType.ScaleToFit);
                    psv.SetPlotCentered(ps, true);
                    // We'll use the standard DWF PC3, as
                    // for today we're just plotting to file
                    psv.SetPlotConfigurationName(ps, "DWG To PDF.pc3", "ISO_full_bleed_A3_(420.00_x_297.00_MM)");
                    //  psv.SetPlotConfigurationName(ps,"DWF6 ePlot.pc3", "ANSI_A_(8.50_x_11.00_Inches)");
                    // We need to link the PlotInfo to the
                    // PlotSettings and then validate it
                    pi.OverrideSettings = ps;
                    PlotInfoValidator piv = new PlotInfoValidator();
                    piv.MediaMatchingPolicy = MatchingPolicy.MatchEnabled;
                    piv.Validate(pi);
                    // A PlotEngine does the actual plotting
                    // (can also create one for Preview)
                    if (PlotFactory.ProcessPlotState == ProcessPlotState.NotPlotting)
                    {
                        PlotEngine pe = PlotFactory.CreatePublishEngine();
                        using (pe)
                        {
                            // and allow thej user to cancel
                            PlotProgressDialog ppd = new PlotProgressDialog(false, 1, true);
                            using (ppd)
                            {
                                ppd.set_PlotMsgString(PlotMessageIndex.DialogTitle, "Custom Plot Progress");
                                ppd.set_PlotMsgString(PlotMessageIndex.CancelJobButtonMessage, "Cancel Job");
                                ppd.set_PlotMsgString(PlotMessageIndex.CancelSheetButtonMessage, "Cancel Sheet");
                                ppd.set_PlotMsgString(PlotMessageIndex.SheetSetProgressCaption, "Sheet Set Progress");
                                ppd.set_PlotMsgString(PlotMessageIndex.SheetProgressCaption, "Sheet Progress");
                                ppd.LowerPlotProgressRange = 0;
                                ppd.UpperPlotProgressRange = 100;
                                ppd.PlotProgressPos = 0;
                                // Let's start the plot, at last
                                ppd.OnBeginPlot();
                                ppd.IsVisible = true;
                                pe.BeginPlot(ppd, null);
                                // We'll be plotting a single document
                                pe.BeginDocument(pi, doc.Name, null, 1, true, "D:\\MyPlot\\aaa" + n.ToString());
                                // Which contains a single sheet
                                ppd.OnBeginSheet();
                                ppd.LowerSheetProgressRange = 0;
                                ppd.UpperSheetProgressRange = 100;
                                ppd.SheetProgressPos = 0;
                                PlotPageInfo ppi = new PlotPageInfo();
                                pe.BeginPage(ppi, pi, true, null);
                                pe.BeginGenerateGraphics(null); // [color=red]problem in this part [/color]
                                pe.EndGenerateGraphics(null);
                                // Finish the sheet
                                pe.EndPage(null);
                                ppd.SheetProgressPos = 100;
                                ppd.OnEndSheet();
                                // Finish the document
                                pe.EndDocument(null);
                                // And finish the plot
                                ppd.PlotProgressPos = 100;
                                ppd.OnEndPlot();
                                pe.EndPlot(null);
                                n++;
                            }
                        }
                    }
                    else
                    {
                        ed.WriteMessage("\nAnother plot is in progress.");
                    }
                }
                Application.SetSystemVariable("BACKGROUNDPLOT", bgPlot);
                tr.Commit();
            }
        }












    }

}