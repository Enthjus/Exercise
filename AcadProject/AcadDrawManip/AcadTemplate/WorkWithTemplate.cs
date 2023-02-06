using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.StyleFunc.AcadDimStyle;
using LibraryCad.StyleFunc.AcadMLeaderStyle;
using LibraryCad.StyleFunc.AcadTableStyle;
using LibraryCad.StyleFunc.AcadTextStyle;
using System.Windows;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AcadProject.AcadDrawManip.AcadTemplate
{
    public class WorkWithTemplate
    {
        [CommandMethod("opdwt", CommandFlags.Session)]
        public void testOpenFromTemplate()
        {
            DocumentCollection dm = Application.DocumentManager;

            Document doc = Application.DocumentManager.MdiActiveDocument;

            Database db = doc.Database;

            Editor ed = doc.Editor;

            try
            {
                using (DocumentLock doclock = doc.LockDocument())
                {

                    string tmpPath = "PhucTemp.dwt";
                    //change file name to save:
                    string fn = @"D:\MyPlot\FromTemplate.dwg";

                    Document newdoc = dm.Add(tmpPath);

                    Database newdb = newdoc.Database;

                    using (DocumentLock newdoclock = newdoc.LockDocument())
                    {
                        using (Transaction newtr = newdoc.TransactionManager.StartTransaction())
                        {
                            dm.MdiActiveDocument = newdoc;

                            //DimStyleFunc.ChangeDimStlye(newdoc);

                            //TextStyleFunc.ChangeTextSyleInModelSpace(newdoc);

                            //TableStyleFunc.ChangeTableStyle(newdoc);

                            //MLeaderStyleFunc.ChangeMLeaderStyle(newdoc);

                            Editor newed = newdoc.Editor;
                            
                            MessageBox.Show("Saved as: " + fn);
                        }
                    }
                    newdoc.CloseAndSave(fn);

                    // just to work further if you wanted be:
                    dm.Open(fn, false);

                    MessageBox.Show("You can try to do your rest job here");
                }
                doc.CloseAndDiscard();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                // do nothing
            }
        }
    }
}
