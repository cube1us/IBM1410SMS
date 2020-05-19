namespace IBM1410SMS
{
    partial class EditPanelsForm
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
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.selectFrameLabel = new System.Windows.Forms.Label();
            this.selectGateLabel = new System.Windows.Forms.Label();
            this.machineComboBox = new System.Windows.Forms.ComboBox();
            this.frameComboBox = new System.Windows.Forms.ComboBox();
            this.machineGateComboBox = new System.Windows.Forms.ComboBox();
            this.panelsDataGridView = new System.Windows.Forms.DataGridView();
            this.applyButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.panelsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Select machine:";
            this.toolTip1.SetToolTip(this.label1, "Select the Machine, then the Frame and then the Gate that these Panels are part o" +
        "f.");
            // 
            // selectFrameLabel
            // 
            this.selectFrameLabel.AutoSize = true;
            this.selectFrameLabel.Location = new System.Drawing.Point(23, 62);
            this.selectFrameLabel.Name = "selectFrameLabel";
            this.selectFrameLabel.Size = new System.Drawing.Size(100, 13);
            this.selectFrameLabel.TabIndex = 4;
            this.selectFrameLabel.Text = "Then Select Frame:";
            this.toolTip1.SetToolTip(this.selectFrameLabel, "Select the Machine, then the Frame and then the Gate that these Panels are part o" +
        "f.");
            // 
            // selectGateLabel
            // 
            this.selectGateLabel.AutoSize = true;
            this.selectGateLabel.Location = new System.Drawing.Point(23, 99);
            this.selectGateLabel.Name = "selectGateLabel";
            this.selectGateLabel.Size = new System.Drawing.Size(94, 13);
            this.selectGateLabel.TabIndex = 5;
            this.selectGateLabel.Text = "Then Select Gate:";
            this.toolTip1.SetToolTip(this.selectGateLabel, "Select the Machine, then the Frame and then the Gate that these Panels are part o" +
        "f.");
            // 
            // machineComboBox
            // 
            this.machineComboBox.DisplayMember = "name";
            this.machineComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.machineComboBox.FormattingEnabled = true;
            this.machineComboBox.Location = new System.Drawing.Point(141, 22);
            this.machineComboBox.Name = "machineComboBox";
            this.machineComboBox.Size = new System.Drawing.Size(172, 21);
            this.machineComboBox.TabIndex = 6;
            this.toolTip1.SetToolTip(this.machineComboBox, "Select the Machine, then the Frame and then the Gate that these Panels are part o" +
        "f.");
            this.machineComboBox.SelectedIndexChanged += new System.EventHandler(this.machineComboBox_SelectedIndexChanged);
            // 
            // frameComboBox
            // 
            this.frameComboBox.DisplayMember = "name";
            this.frameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.frameComboBox.FormattingEnabled = true;
            this.frameComboBox.Location = new System.Drawing.Point(159, 59);
            this.frameComboBox.Name = "frameComboBox";
            this.frameComboBox.Size = new System.Drawing.Size(172, 21);
            this.frameComboBox.TabIndex = 7;
            this.toolTip1.SetToolTip(this.frameComboBox, "Select the Machine, then the Frame and then the Gate that these Panels are part o" +
        "f.");
            this.frameComboBox.SelectedIndexChanged += new System.EventHandler(this.frameComboBox_SelectedIndexChanged);
            // 
            // machineGateComboBox
            // 
            this.machineGateComboBox.DisplayMember = "name";
            this.machineGateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.machineGateComboBox.FormattingEnabled = true;
            this.machineGateComboBox.Location = new System.Drawing.Point(190, 96);
            this.machineGateComboBox.Name = "machineGateComboBox";
            this.machineGateComboBox.Size = new System.Drawing.Size(172, 21);
            this.machineGateComboBox.TabIndex = 8;
            this.toolTip1.SetToolTip(this.machineGateComboBox, "Select the Machine, then the Frame and then the Gate that these Panels are part o" +
        "f.");
            this.machineGateComboBox.SelectedIndexChanged += new System.EventHandler(this.machineGateComboBox_SelectedIndexChanged);
            // 
            // panelsDataGridView
            // 
            this.panelsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.panelsDataGridView.Location = new System.Drawing.Point(26, 142);
            this.panelsDataGridView.Name = "panelsDataGridView";
            this.panelsDataGridView.Size = new System.Drawing.Size(605, 150);
            this.panelsDataGridView.TabIndex = 9;
            this.toolTip1.SetToolTip(this.panelsDataGridView, "Enter Panels Here");
            this.panelsDataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.panelsDataGridView_CellValidating);
            this.panelsDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.panelsDataGridView_CellValueChanged);
            this.panelsDataGridView.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.panelsDataGridView_UserDeletingRow);
            // 
            // applyButton
            // 
            this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.applyButton.Location = new System.Drawing.Point(26, 331);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(75, 23);
            this.applyButton.TabIndex = 10;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelButton.Location = new System.Drawing.Point(238, 331);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 11;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // EditPanelsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 386);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.panelsDataGridView);
            this.Controls.Add(this.machineGateComboBox);
            this.Controls.Add(this.frameComboBox);
            this.Controls.Add(this.machineComboBox);
            this.Controls.Add(this.selectGateLabel);
            this.Controls.Add(this.selectFrameLabel);
            this.Controls.Add(this.label1);
            this.Name = "EditPanelsForm";
            this.Text = "Edit Panels";
            this.toolTip1.SetToolTip(this, "TBA");
            ((System.ComponentModel.ISupportInitialize)(this.panelsDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label selectFrameLabel;
        private System.Windows.Forms.Label selectGateLabel;
        private System.Windows.Forms.ComboBox machineComboBox;
        private System.Windows.Forms.ComboBox frameComboBox;
        private System.Windows.Forms.ComboBox machineGateComboBox;
        private System.Windows.Forms.DataGridView panelsDataGridView;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Button cancelButton;
    }
}