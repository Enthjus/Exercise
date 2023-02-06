using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using LibraryCad.DocumentManager;
using LibraryCad.ObjectsFunc.BlockObject;
using System;
using System.Linq;
using System.Windows.Forms;

namespace AcadProject
{
    public partial class UserControl1 : UserControl
    {
        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

        public UserControl1()
        {
            InitializeComponent();
            comboBoxTriangleStatus.Items.Add("Inscribed");
            comboBoxTriangleStatus.Items.Add("Circumscribed");
            if (Init.remainder == 1)
            {
                rbNotRemainder.Checked = true;
            }
            else
            {
                rbRemainder.Checked = true;
            }
            if (Init.triangleStatus == 1)
            {
                comboBoxTriangleStatus.SelectedIndex = 0;
            }
            else
            {
                comboBoxTriangleStatus.SelectedIndex = 1;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            TabbedDialogExtension.SetDirty(this, true);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabbedDialogExtension.SetDirty(this, true);
        }

        private void buttonSelectFile_Click(object sender, EventArgs e)
        {
            try
            {
                using (doc.LockDocument())
                {
                    var blockName = textBoxBlockName.Text;
                    if (blockName == "")
                    {
                        MessageBox.Show("Block name không được để trống!");
                        return;
                    }
                    OpenFileDialog file = new OpenFileDialog();
                    file.Filter = "Drawing (.dwg)|*.dwg";
                    var res = file.ShowDialog();
                    if (res == DialogResult.Cancel) return;
                    var fileName = file.FileName;
                    if (fileName == "")
                    {
                        MessageBox.Show("Tên file không hợp lệ!");
                        return;
                    }
                    BlockFunc.GetBlkFromAnotherDB(doc, blockName, fileName);
                }
            }
            catch
            {
                return;
            }
        }
    }
}
