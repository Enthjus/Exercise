namespace Topic1.WorkWithForm.LayerManager.LayerForm
{
    partial class AddForm
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
            this.components = new System.ComponentModel.Container();
            this.txb_LayerName = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.btn_Add = new DevComponents.DotNetBar.ButtonX();
            this.errorProviderName = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorProviderColor = new System.Windows.Forms.ErrorProvider(this.components);
            this.txb_ColorId = new DevComponents.Editors.IntegerInput();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txb_ColorId)).BeginInit();
            this.SuspendLayout();
            // 
            // txb_LayerName
            // 
            // 
            // 
            // 
            this.txb_LayerName.Border.Class = "TextBoxBorder";
            this.txb_LayerName.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txb_LayerName.Location = new System.Drawing.Point(93, 14);
            this.txb_LayerName.Name = "txb_LayerName";
            this.txb_LayerName.PreventEnterBeep = true;
            this.txb_LayerName.Size = new System.Drawing.Size(172, 22);
            this.txb_LayerName.TabIndex = 0;
            this.txb_LayerName.Validating += new System.ComponentModel.CancelEventHandler(this.txb_LayerName_Validating);
            // 
            // labelX1
            // 
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(12, 12);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(75, 23);
            this.labelX1.TabIndex = 1;
            this.labelX1.Text = "Layer name";
            // 
            // labelX2
            // 
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(12, 43);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(75, 23);
            this.labelX2.TabIndex = 3;
            this.labelX2.Text = "Color id";
            // 
            // btn_Add
            // 
            this.btn_Add.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btn_Add.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btn_Add.Location = new System.Drawing.Point(93, 71);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(75, 23);
            this.btn_Add.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btn_Add.TabIndex = 4;
            this.btn_Add.Text = "Add";
            this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
            // 
            // errorProviderName
            // 
            this.errorProviderName.ContainerControl = this;
            // 
            // errorProviderColor
            // 
            this.errorProviderColor.ContainerControl = this;
            // 
            // txb_ColorId
            // 
            // 
            // 
            // 
            this.txb_ColorId.BackgroundStyle.Class = "DateTimeInputBackground";
            this.txb_ColorId.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txb_ColorId.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2;
            this.txb_ColorId.Location = new System.Drawing.Point(93, 43);
            this.txb_ColorId.Name = "txb_ColorId";
            this.txb_ColorId.ShowUpDown = true;
            this.txb_ColorId.Size = new System.Drawing.Size(172, 22);
            this.txb_ColorId.TabIndex = 5;
            this.txb_ColorId.Validating += new System.ComponentModel.CancelEventHandler(this.txb_ColorId_Validating);
            // 
            // AddForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 107);
            this.Controls.Add(this.txb_ColorId);
            this.Controls.Add(this.btn_Add);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.txb_LayerName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AddForm";
            this.ShowInTaskbar = false;
            this.Text = "AddForm";
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txb_ColorId)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.TextBoxX txb_LayerName;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.ButtonX btn_Add;
        private System.Windows.Forms.ErrorProvider errorProviderName;
        private System.Windows.Forms.ErrorProvider errorProviderColor;
        private DevComponents.Editors.IntegerInput txb_ColorId;
    }
}