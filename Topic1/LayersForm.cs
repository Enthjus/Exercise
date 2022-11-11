using System;
using Autodesk.AutoCAD.Runtime;
using acad = Autodesk.AutoCAD.ApplicationServices.Application;
using System.Windows.Forms;
using System.IO;

namespace Topic1
{
    public partial class LayersForm : Form
    {

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

            // Nếu thêm thành công thì load lại data
            if (addForm.isSave) LoadData();
        }

        private void Layers_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        public void LoadData()
        {
            var doc = acad.DocumentManager.MdiActiveDocument;
            dtgv_Layers.DataSource = null;
            dtgv_Layers.MultiSelect = false;

            // Load layers được tạo do tool
            dtgv_Layers.DataSource = LibraryCad.LayerFunc.GetLayer(doc);
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            var doc = acad.DocumentManager.MdiActiveDocument;

            // Lấy tên layer của hàng được người dùng chọn
            string layerName = dtgv_Layers.SelectedCells[0].OwningRow.Cells["Name"].Value.ToString();

            // Xóa layer
            var msg = LibraryCad.LayerFunc.LayerDelete(doc, layerName);

            // Nếu xóa thành công load lại data
            if(msg.Contains("have been deleted"))
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
                    var doc = acad.DocumentManager.MdiActiveDocument;
                    
                    //Xóa dữ liệu cũ
                    for (int ii = dtgv_Layers.Rows.Count - 1; ii >= 0; ii--)
                    {
                        string layerName = dtgv_Layers.Rows[ii].Cells["Name"].Value.ToString();
                        LibraryCad.LayerFunc.LayerDelete(doc, layerName);
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
