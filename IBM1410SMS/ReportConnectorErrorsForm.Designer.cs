namespace IBM1410SMS
{
    partial class ReportConnectorErrorsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.machineComboBox = new System.Windows.Forms.ComboBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.reportButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Machine:";
            this.toolTip1.SetToolTip(this.label1, "Select Machine to Check");
            // 
            // machineComboBox
            // 
            this.machineComboBox.DisplayMember = "name";
            this.machineComboBox.FormattingEnabled = true;
            this.machineComboBox.Location = new System.Drawing.Point(109, 18);
            this.machineComboBox.Name = "machineComboBox";
            this.machineComboBox.Size = new System.Drawing.Size(121, 21);
            this.machineComboBox.TabIndex = 1;
            this.toolTip1.SetToolTip(this.machineComboBox, "Select Machine to Check");
            this.machineComboBox.SelectedIndexChanged += new System.EventHandler(this.machineComboBox_SelectedIndexChanged);
            // 
            // reportButton
            // 
            this.reportButton.Location = new System.Drawing.Point(130, 64);
            this.reportButton.Name = "reportButton";
            this.reportButton.Size = new System.Drawing.Size(75, 23);
            this.reportButton.TabIndex = 2;
            this.reportButton.Text = "Report";
            this.reportButton.UseVisualStyleBackColor = true;
            this.reportButton.Click += new System.EventHandler(this.reportButton_Click);
            // 
            // ReportConnectorErrorsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 109);
            this.Controls.Add(this.reportButton);
            this.Controls.Add(this.machineComboBox);
            this.Controls.Add(this.label1);
            this.Name = "ReportConnectorErrorsForm";
            this.Text = "Report Connector Errors";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox machineComboBox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button reportButton;
    }
}