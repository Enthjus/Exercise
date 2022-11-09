﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using LibraryCad;
using acad = Autodesk.AutoCAD.ApplicationServices.Application;
using System.Windows.Forms;
using System.Linq;
using System.IO;

namespace Topic1
{
    public partial class LayersForm : Form
    {

        [CommandMethod("LayerController")]
        public static void LayerController()
        {
            LayersForm form = new LayersForm();
            //acad.ShowModalDialog(Form);
            acad.ShowModelessDialog(form);
        }

        public LayersForm()
        {
            InitializeComponent();
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            AddForm addForm = new AddForm(this);
            acad.ShowModalDialog(addForm);
            if (addForm.isSave) LoadData();
        }

        private void Layers_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        public void LoadData()
        {
            dtgv_Layers.MultiSelect = false;
            // Load layers lưu
            if (Variable.layerInfos.Count > 0)
            {
                dtgv_Layers.DataSource = null;
                dtgv_Layers.DataSource = Variable.layerInfos;
            }
            else dtgv_Layers.DataSource = null;
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            var doc = acad.DocumentManager.CurrentDocument;
            var db = doc.Database;
            string layerName = dtgv_Layers.SelectedCells[0].OwningRow.Cells["Name"].Value.ToString();
            var msg = LibraryCad.LayerFunc.LayerDelete(doc, layerName);
            if(msg.Contains("have been deleted"))
            {
                var layer = Variable.layerInfos.SingleOrDefault(ly => ly.Name == layerName);
                Variable.layerInfos.Remove(layer);
                LoadData();
            }
            if (msg != null) MessageBox.Show(msg);
        }

        private void btn_XuatFile_Click(object sender, EventArgs e)
        {
            if (dtgv_Layers.Rows.Count != 0)
            {
                FileDialog savef = new SaveFileDialog();
                string fname = null;
                savef.Filter = "Text File (*.txt)|*.txt";
                var res = savef.ShowDialog();
                if (res == DialogResult.Cancel) { return; }
                fname = savef.FileName;
                StreamWriter write = new StreamWriter(fname, false);
                int yy = dtgv_Layers.RowCount;
                for (int i = 0; i < yy; i++)
                {
                    string line = dtgv_Layers.Rows[i].Cells[0].Value.ToString() + "|" + dtgv_Layers.Rows[i].Cells[1].Value.ToString();
                    write.WriteLine(line);
                }
                write.Close();
            }
            else
            {
                MessageBox.Show(" - Hiện Tại Chưa Có Dữ Liệu!!!", "Thông Báo!!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btn_NhapFile_Click(object sender, EventArgs e)
        {
            if (dtgv_Layers.Rows.Count != 0)
            {
                DialogResult kq = MessageBox.Show(" - Bạn Có Muốn Xóa Dữ Liệu Cũ Không!", "Thông Báo!!!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (kq == DialogResult.Yes)
                {
                    var doc = acad.DocumentManager.CurrentDocument;
                    var db = doc.Database;
                    //Xóa dữ liệu cũ
                    for (int ii = dtgv_Layers.Rows.Count - 1; ii >= 0; ii--)
                    {
                        string layerName = dtgv_Layers.Rows[ii].Cells["Name"].Value.ToString();
                        var msg = LibraryCad.LayerFunc.LayerDelete(doc, layerName) + "\n";
                        if (msg.Contains("have been deleted"))
                        {
                            var layer = Variable.layerInfos.SingleOrDefault(ly => ly.Name == layerName);
                            Variable.layerInfos.Remove(layer);
                        }
                    }
                    //Thêm dữ liệu mới
                    int i = 0;
                    Variable.Import_txt(i);
                    LoadData();
                }
                if (kq == DialogResult.No)
                {
                    //Thêm dữ liệu mới
                    int i = dtgv_Layers.RowCount;
                    Variable.Import_txt(i);
                    LoadData();
                }
            }
            else
            {
                //Thêm dữ liệu mới
                int i = 0;
                Variable.Import_txt(i);
                LoadData();
            }
        }
    }
}
