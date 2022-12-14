using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;
using LibraryCad.ObjectsFunc.LayerObject;
using System;
using System.IO;
using System.Windows.Forms;
using acad = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AcadProject.AcadManip.WorkWithForm.LayerManager.LayerForm
{
    public partial class LayersForm : Form
    {
        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

        [CommandMethod("LayerController")]
        public static void LayerController()
        {
            LayersForm form = new LayersForm();
            acad.ShowModalDialog(form);
            //acad.ShowModelessDialog(form);
        }

        public LayersForm()
        {
            InitializeComponent();
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            // Mở form add
            AddForm addForm = new AddForm(this);
            acad.ShowModalDialog(addForm);
            // Load lại data
            LoadData();
        }

        private void Layers_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        public void LoadData()
        {
            dtgv_Layers.DataSource = null;
            dtgv_Layers.MultiSelect = false;
            // Load layers được tạo do tool
            dtgv_Layers.DataSource = LayerFunc.GetLayer(doc);
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            // Lấy tên layer của hàng được người dùng chọn
            string layerName = dtgv_Layers.SelectedCells[0].OwningRow.Cells["Name"].Value.ToString();
            // Xóa layer
            var msg = LayerFunc.DeleteLayer(doc, layerName);
            // Nếu xóa thành công load lại data
            if (msg.Contains("have been deleted"))
            {
                LoadData();
            }
            //Còn không thì in lỗi
            if (msg != null) MessageBox.Show(msg);
        }

        private void btn_XuatFile_Click(object sender, EventArgs e)
        {
            if (dtgv_Layers.Rows.Count != 0)
            {
                // Mở dialog lưu file
                FileDialog savef = new SaveFileDialog();
                string fname = "";
                // Filter chỉ hiện file text 
                savef.Filter = "Text File (*.txt)|*.txt";
                var res = savef.ShowDialog();
                if (res == DialogResult.Cancel) { return; }
                // Nếu tên file đã tồn tại thì update file còn nếu không tạo file mới
                fname = savef.FileName;
                StreamWriter write = new StreamWriter(fname, false);
                // Ghi từng dòng lên file text
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
                    //Xóa dữ liệu cũ
                    for (int ii = dtgv_Layers.Rows.Count - 1; ii >= 0; ii--)
                    {
                        string layerName = dtgv_Layers.Rows[ii].Cells["Name"].Value.ToString();
                        LayerFunc.DeleteLayer(doc, layerName);
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
