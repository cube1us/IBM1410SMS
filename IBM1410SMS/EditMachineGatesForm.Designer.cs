namespace IBM1410SMS
{
    partial class EditMachineGatesForm
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
            this.selectFrameLabel = new System.Windows.Forms.Label();
            this.frameComboBox = new System.Windows.Forms.ComboBox();
            this.machineGatesDataGridView = new System.Windows.Forms.DataGridView();
            this.applyButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.machineGatesDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select machine:";
            this.toolTip1.SetToolTip(this.label1, "Select the Machine, then the Frame that these Gates are part of.");
            // 
            // machineComboBox
            // 
            this.machineComboBox.DisplayMember = "name";
            this.machineComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.machineComboBox.FormattingEnabled = true;
            this.machineComboBox.Location = new System.Drawing.Point(141, 21);
            this.machineComboBox.Name = "machineComboBox";
            this.machineComboBox.Size = new System.Drawing.Size(171, 21);
            this.machineComboBox.TabIndex = 2;
            this.toolTip1.SetToolTip(this.machineComboBox, "Select the Machine, then the Frame that these Gates are part of.");
            this.machineComboBox.SelectedIndexChanged += new System.EventHandler(this.machineComboBox_SelectedIndexChanged);
            // 
            // selectFrameLabel
            // 
            this.selectFrameLabel.AutoSize = true;
            this.selectFrameLabel.Location = new System.Drawing.Point(23, 66);
            this.selectFrameLabel.Name = "selectFrameLabel";
            this.selectFrameLabel.Size = new System.Drawing.Size(100, 13);
            this.selectFrameLabel.TabIndex = 3;
            this.selectFrameLabel.Text = "Then Select Frame:";
            this.toolTip1.SetToolTip(this.selectFrameLabel, "Select the Machine, then the Frame that these Gates are part of.");
            // 
            // frameComboBox
            // 
            this.frameComboBox.DisplayMember = "name";
            this.frameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.frameComboBox.FormattingEnabled = true;
            this.frameComboBox.Location = new System.Drawing.Point(141, 63);
            this.frameComboBox.Name = "frameComboBox";
            this.frameComboBox.Size = new System.Drawing.Size(171, 21);
            this.frameComboBox.TabIndex = 4;
            this.toolTip1.SetToolTip(this.frameComboBox, "Select the Machine, then the Frame that these Gates are part of.");
            this.frameComboBox.SelectedIndexChanged += new System.EventHandler(this.frameComboBox_SelectedIndexChanged);
            // 
            // machineGatesDataGridView
            // 
            this.machineGatesDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.machineGatesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.machineGatesDataGridView.Location = new System.Drawing.Point(26, 108);
            this.machineGatesDataGridView.Name = "machineGatesDataGridView";
            this.machineGatesDataGridView.Size = new System.Drawing.Size(240, 150);
            this.machineGatesDataGridView.TabIndex = 5;
            this.toolTip1.SetToolTip(this.machineGatesDataGridView, "Enter your gate names here.  For machines that do not have gates, create on gate " +
        "for each frame, named \"GATE\"");
            this.machineGatesDataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.machineGatesDataGridView_CellValidating);
            this.machineGatesDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.machineGatesDataGridView_CellValueChanged);
            this.machineGatesDataGridView.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.machineGatesDataGridView_UserDeletingRow);
            // 
            // applyButton
            // 
            this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.applyButton.Location = new System.Drawing.Point(26, 287);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(75, 23);
            this.applyButton.TabIndex = 6;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelButton.Location = new System.Drawing.Point(191, 287);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // EditMachineGatesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 338);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.machineGatesDataGridView);
            this.Controls.Add(this.frameComboBox);
            this.Controls.Add(this.selectFrameLabel);
            this.Controls.Add(this.machineComboBox);
            this.Controls.Add(this.label1);
            this.Name = "EditMachineGatesForm";
            this.Text = "EditMachineGatesForm";
            ((System.ComponentModel.ISupportInitialize)(this.machineGatesDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ComboBox machineComboBox;
        private System.Windows.Forms.Label selectFrameLabel;
        private System.Windows.Forms.ComboBox frameComboBox;
        private System.Windows.Forms.DataGridView machineGatesDataGridView;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Button cancelButton;
    }
}