namespace Topic1.WorkWithForm.SubForm
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
            this.btn_PickDimension = new DevComponents.DotNetBar.ButtonX();
            this.txb_DimSum = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.btn_PrintDS = new DevComponents.DotNetBar.ButtonX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.txb_LineSum = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.btn_PickLine = new DevComponents.DotNetBar.ButtonX();
            this.btn_PrintLS = new DevComponents.DotNetBar.ButtonX();
            this.SuspendLayout();
            // 
            // btn_PickDimension
            // 
            this.btn_PickDimension.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btn_PickDimension.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btn_PickDimension.Location = new System.Drawing.Point(160, 68);
            this.btn_PickDimension.Name = "btn_PickDimension";
            this.btn_PickDimension.Size = new System.Drawing.Size(71, 42);
            this.btn_PickDimension.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btn_PickDimension.TabIndex = 0;
            this.btn_PickDimension.Text = "Pick dimension";
            this.btn_PickDimension.Click += new System.EventHandler(this.btn_PickDimension_Click);
            // 
            // txb_DimSum
            // 
            // 
            // 
            // 
            this.txb_DimSum.Border.Class = "TextBoxBorder";
            this.txb_DimSum.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txb_DimSum.Enabled = false;
            this.txb_DimSum.Location = new System.Drawing.Point(160, 40);
            this.txb_DimSum.Name = "txb_DimSum";
            this.txb_DimSum.PreventEnterBeep = true;
            this.txb_DimSum.Size = new System.Drawing.Size(150, 22);
            this.txb_DimSum.TabIndex = 1;
            // 
            // labelX1
            // 
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(63, 38);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(100, 23);
            this.labelX1.TabIndex = 2;
            this.labelX1.Text = "Dimension Sum:";
            // 
            // btn_PrintDS
            // 
            this.btn_PrintDS.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btn_PrintDS.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btn_PrintDS.Location = new System.Drawing.Point(238, 68);
            this.btn_PrintDS.Name = "btn_PrintDS";
            this.btn_PrintDS.Size = new System.Drawing.Size(72, 42);
            this.btn_PrintDS.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btn_PrintDS.TabIndex = 3;
            this.btn_PrintDS.Text = "Print";
            this.btn_PrintDS.Click += new System.EventHandler(this.btn_PrintDS_Click);
            // 
            // labelX2
            // 
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(63, 127);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(75, 23);
            this.labelX2.TabIndex = 4;
            this.labelX2.Text = "Line Sum";
            // 
            // txb_LineSum
            // 
            // 
            // 
            // 
            this.txb_LineSum.Border.Class = "TextBoxBorder";
            this.txb_LineSum.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txb_LineSum.Enabled = false;
            this.txb_LineSum.Location = new System.Drawing.Point(160, 129);
            this.txb_LineSum.Name = "txb_LineSum";
            this.txb_LineSum.PreventEnterBeep = true;
            this.txb_LineSum.Size = new System.Drawing.Size(150, 22);
            this.txb_LineSum.TabIndex = 5;
            // 
            // btn_PickLine
            // 
            this.btn_PickLine.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btn_PickLine.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btn_PickLine.Location = new System.Drawing.Point(160, 158);
            this.btn_PickLine.Name = "btn_PickLine";
            this.btn_PickLine.Size = new System.Drawing.Size(71, 23);
            this.btn_PickLine.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btn_PickLine.TabIndex = 6;
            this.btn_PickLine.Text = "Pick Line";
            this.btn_PickLine.Click += new System.EventHandler(this.btn_PickLine_Click);
            // 
            // btn_PrintLS
            // 
            this.btn_PrintLS.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btn_PrintLS.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btn_PrintLS.Location = new System.Drawing.Point(238, 158);
            this.btn_PrintLS.Name = "btn_PrintLS";
            this.btn_PrintLS.Size = new System.Drawing.Size(72, 23);
            this.btn_PrintLS.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btn_PrintLS.TabIndex = 7;
            this.btn_PrintLS.Text = "Print";
            this.btn_PrintLS.Click += new System.EventHandler(this.btn_PrintLS_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btn_PrintLS);
            this.Controls.Add(this.btn_PickLine);
            this.Controls.Add(this.txb_LineSum);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.btn_PrintDS);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.txb_DimSum);
            this.Controls.Add(this.btn_PickDimension);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX btn_PickDimension;
        private DevComponents.DotNetBar.Controls.TextBoxX txb_DimSum;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.ButtonX btn_PrintDS;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.Controls.TextBoxX txb_LineSum;
        private DevComponents.DotNetBar.ButtonX btn_PickLine;
        private DevComponents.DotNetBar.ButtonX btn_PrintLS;
    }
}