namespace AcadProject.AcadManip.WorkWithForm.StyleForm
{
    partial class StyleController
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnCreateStyle = new System.Windows.Forms.Button();
            this.btnReadFile = new System.Windows.Forms.Button();
            this.dgvTextStyle = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTextStyle)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(526, 426);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnCreateStyle);
            this.tabPage1.Controls.Add(this.btnReadFile);
            this.tabPage1.Controls.Add(this.dgvTextStyle);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(518, 397);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Text Style";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnCreateStyle
            // 
            this.btnCreateStyle.Location = new System.Drawing.Point(266, 37);
            this.btnCreateStyle.Name = "btnCreateStyle";
            this.btnCreateStyle.Size = new System.Drawing.Size(92, 23);
            this.btnCreateStyle.TabIndex = 2;
            this.btnCreateStyle.Text = "Create Style";
            this.btnCreateStyle.UseVisualStyleBackColor = true;
            this.btnCreateStyle.Click += new System.EventHandler(this.btnCreateStyle_Click);
            // 
            // btnReadFile
            // 
            this.btnReadFile.Location = new System.Drawing.Point(266, 7);
            this.btnReadFile.Name = "btnReadFile";
            this.btnReadFile.Size = new System.Drawing.Size(92, 23);
            this.btnReadFile.TabIndex = 1;
            this.btnReadFile.Text = "Read File";
            this.btnReadFile.UseVisualStyleBackColor = true;
            this.btnReadFile.Click += new System.EventHandler(this.btnReadFile_Click);
            // 
            // dgvTextStyle
            // 
            this.dgvTextStyle.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTextStyle.Enabled = false;
            this.dgvTextStyle.Location = new System.Drawing.Point(6, 6);
            this.dgvTextStyle.MultiSelect = false;
            this.dgvTextStyle.Name = "dgvTextStyle";
            this.dgvTextStyle.RowHeadersWidth = 51;
            this.dgvTextStyle.RowTemplate.Height = 24;
            this.dgvTextStyle.Size = new System.Drawing.Size(253, 385);
            this.dgvTextStyle.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(518, 397);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Dimension Style";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(518, 397);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Table Style";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 25);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(518, 397);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "MLeader Style";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // StyleController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 450);
            this.Controls.Add(this.tabControl1);
            this.Name = "StyleController";
            this.Text = "StyleController";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTextStyle)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button btnCreateStyle;
        private System.Windows.Forms.Button btnReadFile;
        private System.Windows.Forms.DataGridView dgvTextStyle;
        private System.Windows.Forms.TabPage tabPage4;
    }
}