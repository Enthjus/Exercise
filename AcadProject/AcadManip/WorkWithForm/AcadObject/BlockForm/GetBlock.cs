using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;
using LibraryCad.ObjectsFunc.BlockObject;
using System;
using System.Windows.Forms;
using acad = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AcadProject.AcadManip.WorkWithForm.AcadObject.BlockForm
{
    public partial class GetBlock : Form
    {
        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

        [CommandMethod("GetBlockController")]
        public static void GetBlockController()
        {
            GetBlock form = new GetBlock();
            acad.ShowModalDialog(form);
            //acad.ShowModelessDialog(form);
        }

        public GetBlock()
        {
            InitializeComponent();
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
