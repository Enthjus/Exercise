namespace AcadProject.AcadManip.WorkWithForm.SortForm.SortText
{
    partial class SortTextForm
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
            this.textBoxColumns = new System.Windows.Forms.TextBox();
            this.textBoxRows = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.radioButtonHorizontal = new System.Windows.Forms.RadioButton();
            this.radioButtonVertical = new System.Windows.Forms.RadioButton();
            this.groupBoxSortTable = new System.Windows.Forms.GroupBox();
            this.buttonSort = new System.Windows.Forms.Button();
            this.textBoxDistance = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBoxSortText = new System.Windows.Forms.GroupBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.groupBoxSortTable.SuspendLayout();
            this.groupBoxSortText.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Columns";
            // 
            // textBoxColumns
            // 
            this.textBoxColumns.Location = new System.Drawing.Point(109, 34);
            this.textBoxColumns.Name = "textBoxColumns";
            this.textBoxColumns.Size = new System.Drawing.Size(100, 22);
            this.textBoxColumns.TabIndex = 1;
            this.textBoxColumns.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxColumns_KeyPress);
            // 
            // textBoxRows
            // 
            this.textBoxRows.Location = new System.Drawing.Point(109, 65);
            this.textBoxRows.Name = "textBoxRows";
            this.textBoxRows.Size = new System.Drawing.Size(100, 22);
            this.textBoxRows.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Rows";
            // 
            // radioButtonHorizontal
            // 
            this.radioButtonHorizontal.AutoSize = true;
            this.radioButtonHorizontal.Checked = true;
            this.radioButtonHorizontal.Location = new System.Drawing.Point(13, 21);
            this.radioButtonHorizontal.Name = "radioButtonHorizontal";
            this.radioButtonHorizontal.Size = new System.Drawing.Size(85, 20);
            this.radioButtonHorizontal.TabIndex = 4;
            this.radioButtonHorizontal.TabStop = true;
            this.radioButtonHorizontal.Text = "horizontal";
            this.radioButtonHorizontal.UseVisualStyleBackColor = true;
            // 
            // radioButtonVertical
            // 
            this.radioButtonVertical.AutoSize = true;
            this.radioButtonVertical.Location = new System.Drawing.Point(13, 47);
            this.radioButtonVertical.Name = "radioButtonVertical";
            this.radioButtonVertical.Size = new System.Drawing.Size(71, 20);
            this.radioButtonVertical.TabIndex = 5;
            this.radioButtonVertical.Text = "vertical";
            this.radioButtonVertical.UseVisualStyleBackColor = true;
            // 
            // groupBoxSortTable
            // 
            this.groupBoxSortTable.Controls.Add(this.radioButtonVertical);
            this.groupBoxSortTable.Controls.Add(this.radioButtonHorizontal);
            this.groupBoxSortTable.Location = new System.Drawing.Point(21, 127);
            this.groupBoxSortTable.Name = "groupBoxSortTable";
            this.groupBoxSortTable.Size = new System.Drawing.Size(120, 78);
            this.groupBoxSortTable.TabIndex = 6;
            this.groupBoxSortTable.TabStop = false;
            this.groupBoxSortTable.Text = "Table";
            // 
            // buttonSort
            // 
            this.buttonSort.Location = new System.Drawing.Point(66, 211);
            this.buttonSort.Name = "buttonSort";
            this.buttonSort.Size = new System.Drawing.Size(75, 23);
            this.buttonSort.TabIndex = 7;
            this.buttonSort.Text = "Sort";
            this.buttonSort.UseVisualStyleBackColor = true;
            this.buttonSort.Click += new System.EventHandler(this.buttonSort_Click);
            // 
            // textBoxDistance
            // 
            this.textBoxDistance.Location = new System.Drawing.Point(109, 95);
            this.textBoxDistance.Name = "textBoxDistance";
            this.textBoxDistance.Size = new System.Drawing.Size(100, 22);
            this.textBoxDistance.TabIndex = 9;
            this.textBoxDistance.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxDistance_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(40, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 16);
            this.label3.TabIndex = 8;
            this.label3.Text = "Distance";
            // 
            // groupBoxSortText
            // 
            this.groupBoxSortText.Controls.Add(this.radioButton1);
            this.groupBoxSortText.Controls.Add(this.radioButton2);
            this.groupBoxSortText.Location = new System.Drawing.Point(148, 127);
            this.groupBoxSortText.Name = "groupBoxSortText";
            this.groupBoxSortText.Size = new System.Drawing.Size(120, 78);
            this.groupBoxSortText.TabIndex = 7;
            this.groupBoxSortText.TabStop = false;
            this.groupBoxSortText.Text = "Text";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(13, 47);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(71, 20);
            this.radioButton1.TabIndex = 5;
            this.radioButton1.Text = "vertical";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Checked = true;
            this.radioButton2.Location = new System.Drawing.Point(13, 21);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(85, 20);
            this.radioButton2.TabIndex = 4;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "horizontal";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // SortTextForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(280, 253);
            this.Controls.Add(this.groupBoxSortText);
            this.Controls.Add(this.textBoxDistance);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonSort);
            this.Controls.Add(this.groupBoxSortTable);
            this.Controls.Add(this.textBoxRows);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxColumns);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SortTextForm";
            this.Text = "SortTextForm";
            this.groupBoxSortTable.ResumeLayout(false);
            this.groupBoxSortTable.PerformLayout();
            this.groupBoxSortText.ResumeLayout(false);
            this.groupBoxSortText.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxColumns;
        private System.Windows.Forms.TextBox textBoxRows;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radioButtonHorizontal;
        private System.Windows.Forms.RadioButton radioButtonVertical;
        private System.Windows.Forms.GroupBox groupBoxSortTable;
        private System.Windows.Forms.Button buttonSort;
        private System.Windows.Forms.TextBox textBoxDistance;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBoxSortText;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
    }
}