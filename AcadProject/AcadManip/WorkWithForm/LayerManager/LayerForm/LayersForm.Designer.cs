namespace Topic1.AcadManip.WorkWithForm.LayerManager.LayerForm
{
    partial class LayersForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_Add = new DevComponents.DotNetBar.ButtonX();
            this.btn_Delete = new DevComponents.DotNetBar.ButtonX();
            this.btn_XuatFile = new DevComponents.DotNetBar.ButtonX();
            this.btn_NhapFile = new DevComponents.DotNetBar.ButtonX();
            this.dtgv_Layers = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dtgv_Layers)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Add
            // 
            this.btn_Add.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btn_Add.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btn_Add.Location = new System.Drawing.Point(383, 12);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(89, 47);
            this.btn_Add.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btn_Add.TabIndex = 1;
            this.btn_Add.Text = "Add";
            this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
            // 
            // btn_Delete
            // 
            this.btn_Delete.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btn_Delete.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btn_Delete.Location = new System.Drawing.Point(383, 65);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(89, 47);
            this.btn_Delete.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btn_Delete.TabIndex = 0;
            this.btn_Delete.Text = "Delete";
            this.btn_Delete.Click += new System.EventHandler(this.btn_Delete_Click);
            // 
            // btn_XuatFile
            // 
            this.btn_XuatFile.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btn_XuatFile.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btn_XuatFile.Location = new System.Drawing.Point(383, 118);
            this.btn_XuatFile.Name = "btn_XuatFile";
            this.btn_XuatFile.Size = new System.Drawing.Size(89, 47);
            this.btn_XuatFile.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btn_XuatFile.TabIndex = 2;
            this.btn_XuatFile.Text = "Xuat File";
            this.btn_XuatFile.Click += new System.EventHandler(this.btn_XuatFile_Click);
            // 
            // btn_NhapFile
            // 
            this.btn_NhapFile.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btn_NhapFile.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btn_NhapFile.Location = new System.Drawing.Point(383, 171);
            this.btn_NhapFile.Name = "btn_NhapFile";
            this.btn_NhapFile.Size = new System.Drawing.Size(89, 47);
            this.btn_NhapFile.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btn_NhapFile.TabIndex = 3;
            this.btn_NhapFile.Text = "Nhap FIle";
            this.btn_NhapFile.Click += new System.EventHandler(this.btn_NhapFile_Click);
            // 
            // dtgv_Layers
            // 
            this.dtgv_Layers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtgv_Layers.Location = new System.Drawing.Point(12, 12);
            this.dtgv_Layers.MultiSelect = false;
            this.dtgv_Layers.Name = "dtgv_Layers";
            this.dtgv_Layers.ReadOnly = true;
            this.dtgv_Layers.RowHeadersWidth = 51;
            this.dtgv_Layers.RowTemplate.Height = 24;
            this.dtgv_Layers.Size = new System.Drawing.Size(365, 308);
            this.dtgv_Layers.TabIndex = 4;
            // 
            // LayersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 336);
            this.Controls.Add(this.dtgv_Layers);
            this.Controls.Add(this.btn_NhapFile);
            this.Controls.Add(this.btn_XuatFile);
            this.Controls.Add(this.btn_Delete);
            this.Controls.Add(this.btn_Add);
            this.Name = "LayersForm";
            this.ShowInTaskbar = false;
            this.Text = "Layers";
            this.Load += new System.EventHandler(this.Layers_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtgv_Layers)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevComponents.DotNetBar.ButtonX btn_Add;
        private DevComponents.DotNetBar.ButtonX btn_Delete;
        private DevComponents.DotNetBar.ButtonX btn_XuatFile;
        private DevComponents.DotNetBar.ButtonX btn_NhapFile;
        private System.Windows.Forms.DataGridView dtgv_Layers;
    }
}