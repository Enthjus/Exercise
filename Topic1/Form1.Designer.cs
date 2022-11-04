namespace Topic1
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.txt_dem = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.mau = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(98, 142);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Số đối tượng:";
            // 
            // txt_dem
            // 
            this.txt_dem.Location = new System.Drawing.Point(207, 142);
            this.txt_dem.Name = "txt_dem";
            this.txt_dem.Size = new System.Drawing.Size(100, 22);
            this.txt_dem.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(101, 183);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(206, 62);
            this.button1.TabIndex = 2;
            this.button1.Text = "Chọn đối tượng";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // mau
            // 
            this.mau.Location = new System.Drawing.Point(518, 128);
            this.mau.Name = "mau";
            this.mau.Size = new System.Drawing.Size(100, 22);
            this.mau.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(457, 131);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "Màu";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(412, 174);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(206, 62);
            this.button2.TabIndex = 5;
            this.button2.Text = "Tạo Text";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mau);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txt_dem);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_dem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox mau;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
    }
}