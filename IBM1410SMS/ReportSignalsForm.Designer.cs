namespace IBM1410SMS
{
    partial class ReportSignalsForm
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
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.machineComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.logLevelComboBox = new System.Windows.Forms.ComboBox();
            this.directoryTextBox = new System.Windows.Forms.TextBox();
            this.directoryButton = new System.Windows.Forms.Button();
            this.reportButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Machine:";
            this.toolTip1.SetToolTip(this.label1, "Select the base machine for which to report signals");
            // 
            // machineComboBox
            // 
            this.machineComboBox.DisplayMember = "name";
            this.machineComboBox.FormattingEnabled = true;
            this.machineComboBox.Location = new System.Drawing.Point(84, 21);
            this.machineComboBox.Name = "machineComboBox";
            this.machineComboBox.Size = new System.Drawing.Size(142, 21);
            this.machineComboBox.TabIndex = 1;
            this.toolTip1.SetToolTip(this.machineComboBox, "Select the base machine for which to report signals");
            this.machineComboBox.SelectedIndexChanged += new System.EventHandler(this.machineComboBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(259, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Log Level:";
            this.toolTip1.SetToolTip(this.label2, "Select the logging level - higher means more logging");
            // 
            // logLevelComboBox
            // 
            this.logLevelComboBox.FormattingEnabled = true;
            this.logLevelComboBox.Location = new System.Drawing.Point(323, 20);
            this.logLevelComboBox.Name = "logLevelComboBox";
            this.logLevelComboBox.Size = new System.Drawing.Size(51, 21);
            this.logLevelComboBox.TabIndex = 3;
            this.toolTip1.SetToolTip(this.logLevelComboBox, "Select the logging level - higher means more logging");
            this.logLevelComboBox.SelectedIndexChanged += new System.EventHandler(this.logLevelComboBox_SelectedIndexChanged);
            // 
            // directoryTextBox
            // 
            this.directoryTextBox.Location = new System.Drawing.Point(154, 71);
            this.directoryTextBox.Name = "directoryTextBox";
            this.directoryTextBox.ReadOnly = true;
            this.directoryTextBox.Size = new System.Drawing.Size(490, 20);
            this.directoryTextBox.TabIndex = 5;
            this.toolTip1.SetToolTip(this.directoryTextBox, "Enter the directory for the report output log");
            // 
            // directoryButton
            // 
            this.directoryButton.Location = new System.Drawing.Point(30, 69);
            this.directoryButton.Name = "directoryButton";
            this.directoryButton.Size = new System.Drawing.Size(108, 23);
            this.directoryButton.TabIndex = 4;
            this.directoryButton.Text = "Output Directory";
            this.directoryButton.UseVisualStyleBackColor = true;
            this.directoryButton.Click += new System.EventHandler(this.directoryButton_Click);
            // 
            // reportButton
            // 
            this.reportButton.Location = new System.Drawing.Point(299, 122);
            this.reportButton.Name = "reportButton";
            this.reportButton.Size = new System.Drawing.Size(75, 23);
            this.reportButton.TabIndex = 6;
            this.reportButton.Text = "Report";
            this.reportButton.UseVisualStyleBackColor = true;
            this.reportButton.Click += new System.EventHandler(this.reportButton_Click);
            // 
            // ReportSignalsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(675, 171);
            this.Controls.Add(this.reportButton);
            this.Controls.Add(this.directoryTextBox);
            this.Controls.Add(this.directoryButton);
            this.Controls.Add(this.logLevelComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.machineComboBox);
            this.Controls.Add(this.label1);
            this.Name = "ReportSignalsForm";
            this.Text = "Signals Report";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox machineComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox logLevelComboBox;
        private System.Windows.Forms.Button directoryButton;
        private System.Windows.Forms.TextBox directoryTextBox;
        private System.Windows.Forms.Button reportButton;
    }
}