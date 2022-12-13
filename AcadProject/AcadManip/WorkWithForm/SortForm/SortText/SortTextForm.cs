﻿using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using LibraryCad;
using LibraryCad.DocumentManager;
using LibraryCad.ObjectsFunc.TextObject;
using LibraryCad.Sub;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using acad = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AcadProject.AcadManip.WorkWithForm.SortForm.SortText
{
    public partial class SortTextForm : Form
    {
        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

        [CommandMethod("SortFromController")]
        public static void SortFromController()
        {
            SortTextForm form = new SortTextForm();
            acad.ShowModalDialog(form);
            //acad.ShowModelessDialog(form);
        }

        public SortTextForm()
        {
            InitializeComponent();
        }

        private void buttonSort_Click(object sender, EventArgs e)
        {
            var cols = Int32.Parse(textBoxColumns.Text);
            var rows = Int32.Parse(textBoxRows.Text);
            using (doc.LockDocument())
            {
                using(Transaction trans = db.TransactionManager.StartTransaction())
                {
                    var dbTexts = new List<DBText>();
                    var tvDBText = new TypedValue[]{
                        new TypedValue((int)DxfCode.Start, "TEXT")
                    };
                    SelectionFilter filter = new SelectionFilter(tvDBText);
                    var objIds = SubFunc.getObjIds(trans, ed, filter);
                    if (objIds == null) return;
                    foreach (var objId in objIds)
                    {
                        var dbText = trans.GetObject(objId, OpenMode.ForRead) as DBText;
                        dbTexts.Add(dbText);
                    }
                    if (dbTexts.Count == 0) return;
                    var sortedList = TextFunc.SortText(dbTexts);
                    var pointInfo = SubFunc.PickPoint(doc);

                }
            }
        }

        private void textBoxColumns_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)/* && (e.KeyChar != '.')*/)
            {
                e.Handled = true;
            }

            // only allow one decimal point
            //if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            //{
            //    e.Handled = true;
            //}
        }
    }
}
