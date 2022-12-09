using LibraryCad.Models;
using LibraryCad.ObjectsFunc.LayerObject;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Topic1.AcadManip.WorkWithForm.LayerManager.LayerForm
{
    public partial class AddForm : Form
    {
        LayersForm LayerController = null;
        public bool isSave = false;

        public AddForm(LayersForm LayerController)
        {
            InitializeComponent();
            this.LayerController = LayerController;
        }

        public AddForm()
        {
            InitializeComponent();
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            // color index không được để null
            if (txb_ColorId.Text == "")
            {
                MessageBox.Show("Thêm thất bại, color id không được để trống");
                return;
            }
            // color index phải lớn hơn 0
            if (short.Parse(txb_ColorId.Text) <= 0)
            {
                MessageBox.Show("Thêm thất bại, color id phải lớn hơn 0");
                return;
            }
            // Lấy thông tin layer người dùng nhập vào
            var layerInfo = new LayerInfo();
            layerInfo.Name = txb_LayerName.Text;
            layerInfo.ColorId = short.Parse(txb_ColorId.Text);
            layerInfo.Des = "tool create layer";
            // Check xem có thêm thành công hay không
            var msg = LayerFunc.CreateLayer(layerInfo);
            if (msg != null)
            {
                if (msg.Contains("đã tồn tại"))
                {
                    MessageBox.Show(msg);
                    return;
                }
                MessageBox.Show(msg);
            }
            else
            {
                MessageBox.Show("Thêm thất bại");
            }
            // Tắt form
            this.Close();
        }
        // Validate tên không được để trống
        private void txb_LayerName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txb_LayerName.Text))
            {
                e.Cancel = true;
                txb_LayerName.Focus();
                errorProviderName.SetError(txb_LayerName, "Name should not be left blank!");
            }
            else
            {
                e.Cancel = false;
                errorProviderName.SetError(txb_LayerName, "");
            }
        }
        // Validate màu không được để trống
        private void txb_ColorId_Validating(object sender, CancelEventArgs e)
        {
            if (txb_ColorId.Text == "")
            {
                e.Cancel = true;
                txb_ColorId.Focus();
                errorProviderColor.SetError(txb_ColorId, "Color id should not be null!");
            }
            else
            {
                e.Cancel = false;
                errorProviderName.SetError(txb_LayerName, "");
            }
        }
    }
}
