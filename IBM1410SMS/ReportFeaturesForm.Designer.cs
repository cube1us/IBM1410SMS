namespace IBM1410SMS
{
    partial class ReportFeaturesForm
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
            this.machineComboBox = new System.Windows.Forms.ComboBox();
            this.machineLabel = new System.Windows.Forms.Label();
            this.reportButton = new System.Windows.Forms.Button();
            this.directoryTextBox = new System.Windows.Forms.TextBox();
            this.directoryButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // machineComboBox
            // 
            this.machineComboBox.DisplayMember = "name";
            this.machineComboBox.FormattingEnabled = true;
            this.machineComboBox.Location = new System.Drawing.Point(86, 21);
            this.machineComboBox.Name = "machineComboBox";
            this.machineComboBox.Size = new System.Drawing.Size(142, 21);
            this.machineComboBox.TabIndex = 3;
            this.toolTip1.SetToolTip(this.machineComboBox, "Select the base machine for which to report features");
            this.machineComboBox.SelectedIndexChanged += new System.EventHandler(this.machineComboBox_SelectedIndexChanged);
            // 
            // machineLabel
            // 
            this.machineLabel.AutoSize = true;
            this.machineLabel.Location = new System.Drawing.Point(29, 24);
            this.machineLabel.Name = "machineLabel";
            this.machineLabel.Size = new System.Drawing.Size(51, 13);
            this.machineLabel.TabIndex = 2;
            this.machineLabel.Text = "Machine:";
            this.toolTip1.SetToolTip(this.machineLabel, "Select the base machine for which to report features");
            // 
            // reportButton
            // 
            this.reportButton.Location = new System.Drawing.Point(299, 118);
            this.reportButton.Name = "reportButton";
            this.reportButton.Size = new System.Drawing.Size(75, 23);
            this.reportButton.TabIndex = 9;
            this.reportButton.Text = "Report";
            this.reportButton.UseVisualStyleBackColor = true;
            this.reportButton.Click += new System.EventHandler(this.reportButton_Click);
            // 
            // directoryTextBox
            // 
            this.directoryTextBox.Location = new System.Drawing.Point(154, 67);
            this.directoryTextBox.Name = "directoryTextBox";
            this.directoryTextBox.ReadOnly = true;
            this.directoryTextBox.Size = new System.Drawing.Size(490, 20);
            this.directoryTextBox.TabIndex = 8;
            this.toolTip1.SetToolTip(this.directoryTextBox, "Enter the directory for the report output log");
            // 
            // directoryButton
            // 
            this.directoryButton.Location = new System.Drawing.Point(30, 65);
            this.directoryButton.Name = "directoryButton";
            this.directoryButton.Size = new System.Drawing.Size(108, 23);
            this.directoryButton.TabIndex = 7;
            this.directoryButton.Text = "Output Directory";
            this.toolTip1.SetToolTip(this.directoryButton, "Click to enable selection of output directory for the report");
            this.directoryButton.UseVisualStyleBackColor = true;
            this.directoryButton.Click += new System.EventHandler(this.directoryButton_Click);
            // 
            // ReportFeaturesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(693, 174);
            this.Controls.Add(this.reportButton);
            this.Controls.Add(this.directoryTextBox);
            this.Controls.Add(this.directoryButton);
            this.Controls.Add(this.machineComboBox);
            this.Controls.Add(this.machineLabel);
            this.Name = "ReportFeaturesForm";
            this.Text = "Features Report";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox machineComboBox;
        private System.Windows.Forms.Label machineLabel;
        private System.Windows.Forms.Button reportButton;
        private System.Windows.Forms.TextBox directoryTextBox;
        private System.Windows.Forms.Button directoryButton;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}