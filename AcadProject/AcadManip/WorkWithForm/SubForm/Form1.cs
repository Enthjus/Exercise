using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using LibraryCad;
using LibraryCad.DocumentManager;
using LibraryCad.ObjectsFunc.LineObject;
using LibraryCad.ObjectsFunc.TextObject;
using LibraryCad.Sub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using acad = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AcadProject.AcadManip.WorkWithForm.SubForm
{
    public partial class Form1 : Form
    {
        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

        public Form1()
        {
            InitializeComponent();
        }

        [CommandMethod("OpenForm")]
        public static void OpenForm()
        {
            Form1 form = new Form1();
            acad.ShowModelessDialog(form);
        }

        private void btn_PickDimension_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (doc.LockDocument())
            {
                // Tạo filter
                var tvDim = new TypedValue[]
                    {
                        new TypedValue((int)DxfCode.Start, "DIMENSION")
                    };
                var filter = new SelectionFilter(tvDim);
                // Bắt đầu transaction
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    Variable.sumDim = 0.0;
                    // Lấy list dimension
                    var dimensions = new List<Dimension>();
                    var objectIds = SubFunc.GetListSelection(doc, "", filter);
                    if (objectIds == null) return;
                    foreach (var objectId in objectIds)
                    {
                        var dimension = trans.GetObject(objectId, OpenMode.ForRead) as Dimension;
                        if (dimension != null)
                        {
                            dimensions.Add(dimension);
                        }
                    }
                    // Cộng giá trị các dim lại với nhau
                    dimensions.Where(dim => dim.Measurement > 0).ToList().ForEach(dimension => Variable.sumDim += dimension.Measurement);
                    // Làm tròn
                    Variable.sumDim = System.Math.Round(Variable.sumDim);
                    txb_DimSum.Text = Variable.sumDim.ToString();
                    trans.Commit();
                }
            }
            this.Show();
        }

        private void btn_PrintDS_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (doc.LockDocument())
            {
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    // Set layer
                    var layer = db.Clayer;
                    // Lấy điểm vừa pick
                    var ptn = SubFunc.PickPoint(doc);
                    if (ptn.status == false) return;
                    // Tạo text
                    TextFunc.CreateText(doc, Variable.sumDim.ToString(), ptn.point, layer);
                    trans.Commit();
                }
            }
        }

        private void btn_PickLine_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (doc.LockDocument())
            {
                // Bắt đầu transaction
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        Variable.sumLine = 0.0;
                        // Parse selection set thành list line
                        var lines = LineFunc.SelectionSetToListLine(doc);
                        // Cộng độ dài các đoạn thẳng
                        if (lines != null)
                        {
                            foreach (var line in lines)
                            {
                                Variable.sumLine += line.Length;
                            }
                        }
                        Variable.sumLine = System.Math.Round(Variable.sumLine);
                        txb_LineSum.Text = Variable.sumLine.ToString();
                    }
                    catch (System.Exception ex)
                    {
                        doc.Editor.WriteMessage(ex.Message);
                        trans.Abort();
                    }
                }
            }
            this.Show();
        }

        private void btn_PrintLS_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (doc.LockDocument())
            {
                // Bắt đầu transaction
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        // Set layer
                        var layer = db.Clayer;
                        // Lấy điểm vừa pick
                        var ptn = SubFunc.PickPoint(doc);
                        if (ptn.status == false) return;
                        // Tạo text
                        TextFunc.CreateText(doc, Variable.sumLine.ToString(), ptn.point, layer);
                        trans.Commit();
                    }
                    catch (System.Exception ex)
                    {
                        doc.Editor.WriteMessage(ex.Message);
                        trans.Abort();
                    }
                }
            }
        }
    }
}
