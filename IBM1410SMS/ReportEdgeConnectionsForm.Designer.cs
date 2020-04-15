namespace IBM1410SMS
{
    partial class ReportEdgeConnectionsForm
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
            this.directoryTextBox = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.reportButton = new System.Windows.Forms.Button();
            this.directoryButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Machine";
            this.toolTip1.SetToolTip(this.label1, "Select the base machine to check connections for");
            // 
            // machineComboBox
            // 
            this.machineComboBox.DisplayMember = "name";
            this.machineComboBox.FormattingEnabled = true;
            this.machineComboBox.Location = new System.Drawing.Point(111, 34);
            this.machineComboBox.Name = "machineComboBox";
            this.machineComboBox.Size = new System.Drawing.Size(121, 21);
            this.machineComboBox.TabIndex = 1;
            this.toolTip1.SetToolTip(this.machineComboBox, "Select the base machine to check connections for");
            // 
            // directoryTextBox
            // 
            this.directoryTextBox.Location = new System.Drawing.Point(158, 84);
            this.directoryTextBox.Name = "directoryTextBox";
            this.directoryTextBox.ReadOnly = true;
            this.directoryTextBox.Size = new System.Drawing.Size(406, 20);
            this.directoryTextBox.TabIndex = 3;
            this.toolTip1.SetToolTip(this.directoryTextBox, "Enter the directory for the report output log");
            // 
            // reportButton
            // 
            this.reportButton.Location = new System.Drawing.Point(287, 140);
            this.reportButton.Name = "reportButton";
            this.reportButton.Size = new System.Drawing.Size(75, 23);
            this.reportButton.TabIndex = 4;
            this.reportButton.Text = "Report";
            this.reportButton.UseVisualStyleBackColor = true;
            this.reportButton.Click += new System.EventHandler(this.reportButton_Click);
            // 
            // directoryButton
            // 
            this.directoryButton.Location = new System.Drawing.Point(45, 82);
            this.directoryButton.Name = "directoryButton";
            this.directoryButton.Size = new System.Drawing.Size(98, 23);
            this.directoryButton.TabIndex = 5;
            this.directoryButton.Text = "Output Directory";
            this.directoryButton.UseVisualStyleBackColor = true;
            this.directoryButton.Click += new System.EventHandler(this.directoryButton_Click);
            // 
            // ReportEdgeConnectionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(646, 177);
            this.Controls.Add(this.directoryButton);
            this.Controls.Add(this.reportButton);
            this.Controls.Add(this.directoryTextBox);
            this.Controls.Add(this.machineComboBox);
            this.Controls.Add(this.label1);
            this.Name = "ReportEdgeConnectionsForm";
            this.Text = "Report Edge Connections";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ComboBox machineComboBox;
        private System.Windows.Forms.TextBox directoryTextBox;
        private System.Windows.Forms.Button reportButton;
        private System.Windows.Forms.Button directoryButton;
    }
}