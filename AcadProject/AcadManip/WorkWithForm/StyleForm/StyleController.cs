using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using LibraryCad.DocumentManager;
using LibraryCad.StyleFunc.AcadTextStyle;
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

            var ofd = new Autodesk.AutoCAD.Windows.OpenFileDialog("Select a file using an OpenFileDialog", documents,
                            "*",
                            "File Date Test T22",
                            Autodesk.AutoCAD.Windows.OpenFileDialog.OpenFileDialogFlags.DefaultIsFolder |
                            Autodesk.AutoCAD.Windows.OpenFileDialog.OpenFileDialogFlags.ForceDefaultFolder // .AllowMultiple
                          );
            DialogResult sdResult = ofd.ShowDialog();

            if (sdResult != DialogResult.OK) return;

            fileName = ofd.Filename;

            var styles = TextStyleFunc.FindAllTextStyle(fileName);

            dgvTextStyle.DataSource = styles.Select(x => new { TextStyle = x }).ToList();
            dgvTextStyle.Show();
        }

        private void btnCreateStyle_Click(object sender, EventArgs e)
        {
            TextStyleFunc.GetStyleId(dgvTextStyle.SelectedCells[0].OwningRow.Cells["TextStyle"].Value.ToString(), doc, fileName);
        }
    }
}
