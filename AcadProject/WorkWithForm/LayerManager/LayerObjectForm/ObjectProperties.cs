using Autodesk.AutoCAD.Runtime;
using LibraryCad.ObjectsFunc.LayerObject;
using System;
using System.Windows.Forms;
using acad = Autodesk.AutoCAD.ApplicationServices.Application;

namespace Topic1.WorkWithForm.LayerManager.LayerObjectForm
{
    public partial class ObjectProperties : Form
    {
        [CommandMethod("AreaController")]
        public static void LayerController()
        {
            ObjectProperties form = new ObjectProperties();
            acad.ShowModalDialog(form);
            //acad.ShowModelessDialog(form);
        }

        public ObjectProperties()
        {
            InitializeComponent();
        }

        private void Area_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            dtgv_Area.DataSource = null;
            var doc = acad.DocumentManager.MdiActiveDocument;
            // Lấy layer được tạo bởi winform
            var layerInfos = LayerFunc.GetLayer(doc);
            // Lấy chiều dài và diện tích của các đối tượng thuộc từng layer
            var layerObjs = LayerFunc.GetObjectPropertiesByLayer(doc, layerInfos);
            dtgv_Area.DataSource = layerObjs;
        }

        private void btn_Export_Click(object sender, EventArgs e)
        {
            var doc = acad.DocumentManager.MdiActiveDocument;
            // Lấy layer được tạo bởi tool
            var layerInfos = LayerFunc.GetLayer(doc);
            // Lấy chiều dài và diện tích của các đối tượng thuộc từng layer
            var layerObjs = LayerFunc.GetObjectPropertiesByLayer(doc, layerInfos);
            // Xuất file excel
            Variable.Export_csv(layerObjs);
        }
    }
}
