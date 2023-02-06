namespace AcadProject
{
    partial class UserControl1
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.GetBlock = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxBlockName = new System.Windows.Forms.TextBox();
            this.buttonSelectFile = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.groupBoxRemainder = new System.Windows.Forms.GroupBox();
            this.rbNotRemainder = new System.Windows.Forms.RadioButton();
            this.rbRemainder = new System.Windows.Forms.RadioButton();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxTriangleStatus = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.GetBlock.SuspendLayout();
            this.groupBoxRemainder.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // GetBlock
            // 
            this.GetBlock.Controls.Add(this.label1);
            this.GetBlock.Controls.Add(this.textBoxBlockName);
            this.GetBlock.Controls.Add(this.buttonSelectFile);
            this.GetBlock.Location = new System.Drawing.Point(3, 3);
            this.GetBlock.Name = "GetBlock";
            this.GetBlock.Size = new System.Drawing.Size(387, 112);
            this.GetBlock.TabIndex = 0;
            this.GetBlock.TabStop = false;
            this.GetBlock.Text = "Get block";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Block name:";
            // 
            // textBoxBlockName
            // 
            this.textBoxBlockName.Location = new System.Drawing.Point(113, 35);
            this.textBoxBlockName.Name = "textBoxBlockName";
            this.textBoxBlockName.Size = new System.Drawing.Size(139, 22);
            this.textBoxBlockName.TabIndex = 2;
            // 
            // buttonSelectFile
            // 
            this.buttonSelectFile.Location = new System.Drawing.Point(113, 68);
            this.buttonSelectFile.Name = "buttonSelectFile";
            this.buttonSelectFile.Size = new System.Drawing.Size(75, 23);
            this.buttonSelectFile.TabIndex = 0;
            this.buttonSelectFile.Text = "Select file";
            this.buttonSelectFile.UseVisualStyleBackColor = true;
            this.buttonSelectFile.Click += new System.EventHandler(this.buttonSelectFile_Click);
            // 
            // groupBoxRemainder
            // 
            this.groupBoxRemainder.Controls.Add(this.rbNotRemainder);
            this.groupBoxRemainder.Controls.Add(this.rbRemainder);
            this.groupBoxRemainder.Location = new System.Drawing.Point(396, 3);
            this.groupBoxRemainder.Name = "groupBoxRemainder";
            this.groupBoxRemainder.Size = new System.Drawing.Size(366, 112);
            this.groupBoxRemainder.TabIndex = 2;
            this.groupBoxRemainder.TabStop = false;
            this.groupBoxRemainder.Text = "Remainder calculation";
            // 
            // rbNotRemainder
            // 
            this.rbNotRemainder.AutoSize = true;
            this.rbNotRemainder.Location = new System.Drawing.Point(36, 58);
            this.rbNotRemainder.Name = "rbNotRemainder";
            this.rbNotRemainder.Size = new System.Drawing.Size(119, 20);
            this.rbNotRemainder.TabIndex = 1;
            this.rbNotRemainder.Text = "Not Remainder";
            this.rbNotRemainder.UseVisualStyleBackColor = true;
            // 
            // rbRemainder
            // 
            this.rbRemainder.AutoSize = true;
            this.rbRemainder.Checked = true;
            this.rbRemainder.Location = new System.Drawing.Point(36, 32);
            this.rbRemainder.Name = "rbRemainder";
            this.rbRemainder.Size = new System.Drawing.Size(95, 20);
            this.rbRemainder.TabIndex = 0;
            this.rbRemainder.TabStop = true;
            this.rbRemainder.Text = "Remainder";
            this.rbRemainder.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.GetBlock);
            this.flowLayoutPanel1.Controls.Add(this.groupBoxRemainder);
            this.flowLayoutPanel1.Controls.Add(this.groupBox1);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 5);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(766, 500);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxTriangleStatus);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(3, 121);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(387, 80);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Create triangle with circle";
            // 
            // comboBoxTriangleStatus
            // 
            this.comboBoxTriangleStatus.FormattingEnabled = true;
            this.comboBoxTriangleStatus.Location = new System.Drawing.Point(147, 31);
            this.comboBoxTriangleStatus.Name = "comboBoxTriangleStatus";
            this.comboBoxTriangleStatus.Size = new System.Drawing.Size(121, 24);
            this.comboBoxTriangleStatus.TabIndex = 16;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(94, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 16);
            this.label4.TabIndex = 10;
            this.label4.Text = "Status:";
            // 
            // UserControl1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "UserControl1";
            this.Size = new System.Drawing.Size(772, 508);
            this.GetBlock.ResumeLayout(false);
            this.GetBlock.PerformLayout();
            this.groupBoxRemainder.ResumeLayout(false);
            this.groupBoxRemainder.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.GroupBox GetBlock;
        public System.Windows.Forms.Button buttonSelectFile;
        public System.ComponentModel.BackgroundWorker backgroundWorker1;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox textBoxBlockName;
        private System.Windows.Forms.GroupBox groupBoxRemainder;
        public System.Windows.Forms.RadioButton rbNotRemainder;
        public System.Windows.Forms.RadioButton rbRemainder;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.ComboBox comboBoxTriangleStatus;
        private System.Windows.Forms.Label label4;
    }
}
