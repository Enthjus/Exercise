using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;
using LibraryCad.StyleFunc.AcadDimStyle;
using LibraryCad.StyleFunc.AcadMLeaderStyle;
using LibraryCad.StyleFunc.AcadTableStyle;
using LibraryCad.StyleFunc.AcadTextStyle;
using LibraryCad.Sub;
using System;
using System.Linq;
using System.Windows.Forms;
using acad = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AcadProject.AcadManip.WorkWithForm.StyleForm
{
    public partial class StyleController : Form
    {
        private static string fileName;

        private static Document doc = DocumentManager.doc;

        private static Database db = DocumentManager.db;

        private static Editor ed = DocumentManager.ed;

        [CommandMethod("StyleController")]
        public static void SortFormController()
        {
            StyleController form = new StyleController();
            acad.ShowModalDialog(form);
            //acad.ShowModelessDialog(form);
        }

        public StyleController()
        {
            InitializeComponent();
        }

        private void btnReadFile_Click(object sender, EventArgs e)
        {
            string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Environment.SetEnvironmentVariable("MYDOCUMENTS", documents);

            var ofd = new Autodesk.AutoCAD.Windows.OpenFileDialog("Select a file using an OpenFileDialog", documents, "dwt", "File Date Test T22", Autodesk.AutoCAD.Windows.OpenFileDialog.OpenFileDialogFlags.DefaultIsFolder | Autodesk.AutoCAD.Windows.OpenFileDialog.OpenFileDialogFlags.ForceDefaultFolder  /*.AllowMultiple*/);
            DialogResult sdResult = ofd.ShowDialog();
            
            if (sdResult != DialogResult.OK) return;

            fileName = ofd.Filename;

            var openDb = SubFunc.GetDbByPath(fileName, ed);

            var styleInfos = SubFunc.FindAllStyle(openDb);

            dgvTextStyle.DataSource = styleInfos.TextStyles.Select(x => new { TextStyle = x }).ToList();
            dgvTextStyle.Show();
            dgvDimensionStyle.DataSource = styleInfos.DimStyles.Select(x => new { DimStyle = x }).ToList();
            dgvDimensionStyle.Show();
            dgvTableStyle.DataSource = styleInfos.TableStyles.Select(x => new { TableStyle = x }).ToList();
            dgvTableStyle.Show();
            dgvMLeaderStyle.DataSource = styleInfos.MLeaderStyles.Select(x => new { MLeaderStyle = x }).ToList();
            dgvMLeaderStyle.Show();
        }

        private void btnCreateStyle_Click(object sender, EventArgs e)
        {
            var styleId = TextStyleFunc.GetStyleId(dgvTextStyle.SelectedCells[0].OwningRow.Cells["TextStyle"].Value.ToString(), doc, fileName);
            TextStyleFunc.ChangeTextSyleInModelSpace(doc, styleId);
        }

        private void btnCreateDimensionStyle_Click(object sender, EventArgs e)
        {
            var styleId = DimStyleFunc.GetStyleId(dgvDimensionStyle.SelectedCells[0].OwningRow.Cells["DimStyle"].Value.ToString(), doc, fileName);
            DimStyleFunc.ChangeDimSyleInModelSpace(doc, styleId);
        }

        private void btnCreateMLeaderStyle_Click(object sender, EventArgs e)
        {
            var styleId = MLeaderStyleFunc.GetStyleId(dgvMLeaderStyle.SelectedCells[0].OwningRow.Cells["MLeaderStyle"].Value.ToString(), doc, fileName);
            MLeaderStyleFunc.ChangeMleaderStyleInModelSpace(doc, styleId);
        }
        
        private void btnCreateTableStyle_Click(object sender, EventArgs e)
        {
            var styleId = TableStyleFunc.GetStyleId(dgvTableStyle.SelectedCells[0].OwningRow.Cells["TableStyle"].Value.ToString(), doc, fileName);
            TableStyleFunc.ChangeTableSyleInModelSpace(doc, styleId);
        }
    }
}
