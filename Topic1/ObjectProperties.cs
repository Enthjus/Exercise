using System;
using Autodesk.AutoCAD.Runtime;
using acad = Autodesk.AutoCAD.ApplicationServices.Application;
using System.Windows.Forms;

namespace Topic1
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
            var layerInfos = LibraryCad.LayerFunc.GetLayer(doc);
            
            // Lấy chiều dài và diện tích của các đối tượng thuộc từng layer
            var layerObjs = LibraryCad.LayerFunc.GetObjectPropertiesByLayer(doc, layerInfos);

            dtgv_Area.DataSource = layerObjs;
        }

        private void btn_Export_Click(object sender, EventArgs e)
        {
            var doc = acad.DocumentManager.MdiActiveDocument;
            
            // Lấy layer được tạo bởi tool
            var layerInfos = LibraryCad.LayerFunc.GetLayer(doc);

            // Lấy chiều dài và diện tích của các đối tượng thuộc từng layer
            var layerObjs = LibraryCad.LayerFunc.GetObjectPropertiesByLayer(doc, layerInfos);

            // Xuất file excel
            Variable.Export_csv(layerObjs);
        }
    }
}
