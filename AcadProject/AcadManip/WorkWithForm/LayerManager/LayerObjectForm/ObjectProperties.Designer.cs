namespace Topic1.AcadManip.WorkWithForm.LayerManager.LayerObjectForm
{
    partial class ObjectProperties
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
            this.dtgv_Area = new System.Windows.Forms.DataGridView();
            this.btn_Export = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dtgv_Area)).BeginInit();
            this.SuspendLayout();
            // 
            // dtgv_Area
            // 
            this.dtgv_Area.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtgv_Area.Enabled = false;
            this.dtgv_Area.Location = new System.Drawing.Point(12, 12);
            this.dtgv_Area.MultiSelect = false;
            this.dtgv_Area.Name = "dtgv_Area";
            this.dtgv_Area.RowHeadersWidth = 51;
            this.dtgv_Area.RowTemplate.Height = 24;
            this.dtgv_Area.Size = new System.Drawing.Size(343, 361);
            this.dtgv_Area.TabIndex = 0;
            // 
            // btn_Export
            // 
            this.btn_Export.Location = new System.Drawing.Point(361, 12);
            this.btn_Export.Name = "btn_Export";
            this.btn_Export.Size = new System.Drawing.Size(101, 59);
            this.btn_Export.TabIndex = 2;
            this.btn_Export.Text = "Xuất File";
            this.btn_Export.UseVisualStyleBackColor = true;
            this.btn_Export.Click += new System.EventHandler(this.btn_Export_Click);
            // 
            // ObjectProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 386);
            this.Controls.Add(this.btn_Export);
            this.Controls.Add(this.dtgv_Area);
            this.Name = "ObjectProperties";
            this.ShowInTaskbar = false;
            this.Text = "Area";
            this.Load += new System.EventHandler(this.Area_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtgv_Area)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dtgv_Area;
        private System.Windows.Forms.Button btn_Export;
    }
}