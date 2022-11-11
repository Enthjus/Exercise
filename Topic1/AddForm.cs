using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Topic1
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
            // Điều kiện màu phải lớn hơn 0
            if (short.Parse(txb_ColorId.Text) <= 0)
            {
                MessageBox.Show("Thêm thất bại, color id phải lớn hơn 0");
                return;
            }

            // Lấy thông tin layer người dùng nhập vào
            var layerInfo = new LibraryCad.Models.LayerInfo();
            layerInfo.Name = txb_LayerName.Text;
            layerInfo.ColorId = short.Parse(txb_ColorId.Text);
            layerInfo.Des = "tool create layer";

            // Check có thêm thành công hay không
            isSave = LibraryCad.LayerFunc.CreateLayer(layerInfo);
            if (isSave)
            {
                MessageBox.Show("Thêm thành công");
            }
            else
            {
                MessageBox.Show("Thêm thất bại");
            }

            // Tắt form
            this.Close();
        }

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

        private void txb_ColorId_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txb_ColorId.Text))
            {
                e.Cancel = true;
                txb_ColorId.Focus();
                errorProviderColor.SetError(txb_ColorId, "ColorId can't be null!");
            }
            else
            {
                e.Cancel = false;
                errorProviderColor.SetError(txb_ColorId, "");
            }
        }
    }
}
