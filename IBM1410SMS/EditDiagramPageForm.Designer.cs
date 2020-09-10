namespace IBM1410SMS
{
    partial class EditDiagramPageForm
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
            this.label14 = new System.Windows.Forms.Label();
            this.newPageButton = new System.Windows.Forms.Button();
            this.volumeSetComboBox = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.stampTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.titleTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.partTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.volumeComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.machineComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pageComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.removeButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.addApplyButton = new System.Windows.Forms.Button();
            this.ecosDataGridView = new System.Windows.Forms.DataGridView();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.sheetEdgeDataGridView = new System.Windows.Forms.DataGridView();
            this.editEdgeConnectionsButton = new System.Windows.Forms.Button();
            this.editDiagramBlocksButton = new System.Windows.Forms.Button();
            this.commentTextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.noHDLGenerationCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.ecosDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sheetEdgeDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(379, 117);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(32, 13);
            this.label14.TabIndex = 122;
            this.label14.Text = "OR:  ";
            // 
            // newPageButton
            // 
            this.newPageButton.Location = new System.Drawing.Point(418, 112);
            this.newPageButton.Name = "newPageButton";
            this.newPageButton.Size = new System.Drawing.Size(75, 23);
            this.newPageButton.TabIndex = 21;
            this.newPageButton.Text = "New Page";
            this.toolTip1.SetToolTip(this.newPageButton, "Click here to add a NEW ALD Diagram Page");
            this.newPageButton.UseVisualStyleBackColor = true;
            this.newPageButton.Click += new System.EventHandler(this.newPageButton_Click);
            // 
            // volumeSetComboBox
            // 
            this.volumeSetComboBox.DisplayMember = "machinetype";
            this.volumeSetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.volumeSetComboBox.FormattingEnabled = true;
            this.volumeSetComboBox.Location = new System.Drawing.Point(417, 28);
            this.volumeSetComboBox.Name = "volumeSetComboBox";
            this.volumeSetComboBox.Size = new System.Drawing.Size(180, 21);
            this.volumeSetComboBox.TabIndex = 2;
            this.toolTip1.SetToolTip(this.volumeSetComboBox, "Select machine, volume set and volume for this page");
            this.volumeSetComboBox.SelectedIndexChanged += new System.EventHandler(this.volumeSetComboBox_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(331, 31);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(64, 13);
            this.label13.TabIndex = 114;
            this.label13.Text = "Volume Set:";
            this.toolTip1.SetToolTip(this.label13, "Select machine, volume set and volume for this page");
            // 
            // stampTextBox
            // 
            this.stampTextBox.Location = new System.Drawing.Point(92, 243);
            this.stampTextBox.MaxLength = 45;
            this.stampTextBox.Name = "stampTextBox";
            this.stampTextBox.Size = new System.Drawing.Size(319, 20);
            this.stampTextBox.TabIndex = 50;
            this.toolTip1.SetToolTip(this.stampTextBox, "Stamp or comment on the page (e.g. FIELD USE ONLY)");
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(19, 246);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 13);
            this.label7.TabIndex = 111;
            this.label7.Text = "Stamp (opt):";
            this.toolTip1.SetToolTip(this.label7, "Stamp or comment on the page (e.g. FIELD USE ONLY)");
            // 
            // titleTextBox
            // 
            this.titleTextBox.Location = new System.Drawing.Point(92, 200);
            this.titleTextBox.MaxLength = 80;
            this.titleTextBox.Name = "titleTextBox";
            this.titleTextBox.Size = new System.Drawing.Size(319, 20);
            this.titleTextBox.TabIndex = 40;
            this.toolTip1.SetToolTip(this.titleTextBox, "Title at the top of the ALD Diagram Page");
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 203);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 110;
            this.label6.Text = "Title: ";
            this.toolTip1.SetToolTip(this.label6, "Title at the top of the ALD Diagram Page");
            // 
            // partTextBox
            // 
            this.partTextBox.Location = new System.Drawing.Point(274, 157);
            this.partTextBox.MaxLength = 20;
            this.partTextBox.Name = "partTextBox";
            this.partTextBox.Size = new System.Drawing.Size(127, 20);
            this.partTextBox.TabIndex = 31;
            this.toolTip1.SetToolTip(this.partTextBox, "Part number of the ALD diagram page");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(219, 160);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 106;
            this.label5.Text = "Part No.:";
            this.toolTip1.SetToolTip(this.label5, "Part number of the ALD diagram page");
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(91, 157);
            this.nameTextBox.MaxLength = 16;
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(100, 20);
            this.nameTextBox.TabIndex = 30;
            this.toolTip1.SetToolTip(this.nameTextBox, "Name of the page (e.g. 14.14.14.1)");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 160);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 13);
            this.label4.TabIndex = 104;
            this.label4.Text = "Page Name:";
            this.toolTip1.SetToolTip(this.label4, "Name of the page (e.g. 14.14.14.1)");
            // 
            // volumeComboBox
            // 
            this.volumeComboBox.DisplayMember = "name";
            this.volumeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.volumeComboBox.FormattingEnabled = true;
            this.volumeComboBox.Location = new System.Drawing.Point(139, 71);
            this.volumeComboBox.Name = "volumeComboBox";
            this.volumeComboBox.Size = new System.Drawing.Size(66, 21);
            this.volumeComboBox.TabIndex = 10;
            this.toolTip1.SetToolTip(this.volumeComboBox, "Select machine, volume set and volume for this page");
            this.volumeComboBox.SelectedIndexChanged += new System.EventHandler(this.volumeComboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 103;
            this.label3.Text = "Select Volume:";
            this.toolTip1.SetToolTip(this.label3, "Select machine, volume set and volume for this page");
            // 
            // machineComboBox
            // 
            this.machineComboBox.DisplayMember = "name";
            this.machineComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.machineComboBox.FormattingEnabled = true;
            this.machineComboBox.Location = new System.Drawing.Point(139, 28);
            this.machineComboBox.Name = "machineComboBox";
            this.machineComboBox.Size = new System.Drawing.Size(121, 21);
            this.machineComboBox.TabIndex = 1;
            this.toolTip1.SetToolTip(this.machineComboBox, "Select machine, volume set and volume for this page");
            this.machineComboBox.SelectedIndexChanged += new System.EventHandler(this.machineComboBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 102;
            this.label2.Text = "Select machine:";
            this.toolTip1.SetToolTip(this.label2, "Select machine, volume set and volume for this page");
            // 
            // pageComboBox
            // 
            this.pageComboBox.DisplayMember = "name";
            this.pageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.pageComboBox.FormattingEnabled = true;
            this.pageComboBox.Location = new System.Drawing.Point(139, 114);
            this.pageComboBox.Name = "pageComboBox";
            this.pageComboBox.Size = new System.Drawing.Size(212, 21);
            this.pageComboBox.TabIndex = 20;
            this.toolTip1.SetToolTip(this.pageComboBox, "Select an EXISTING page using the pull down list.");
            this.pageComboBox.SelectedIndexChanged += new System.EventHandler(this.pageComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 117);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 13);
            this.label1.TabIndex = 101;
            this.label1.Text = "Select Page (existing):";
            this.toolTip1.SetToolTip(this.label1, "Select an EXISTING page using the pull down list.");
            // 
            // removeButton
            // 
            this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.removeButton.Location = new System.Drawing.Point(300, 622);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 101;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Visible = false;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelButton.Location = new System.Drawing.Point(532, 622);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 102;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // addApplyButton
            // 
            this.addApplyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addApplyButton.Location = new System.Drawing.Point(68, 622);
            this.addApplyButton.Name = "addApplyButton";
            this.addApplyButton.Size = new System.Drawing.Size(75, 23);
            this.addApplyButton.TabIndex = 100;
            this.addApplyButton.Text = "TBA";
            this.addApplyButton.UseVisualStyleBackColor = true;
            this.addApplyButton.Click += new System.EventHandler(this.addApplyButton_Click);
            // 
            // ecosDataGridView
            // 
            this.ecosDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ecosDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ecosDataGridView.Location = new System.Drawing.Point(22, 430);
            this.ecosDataGridView.Name = "ecosDataGridView";
            this.ecosDataGridView.Size = new System.Drawing.Size(240, 164);
            this.ecosDataGridView.TabIndex = 123;
            this.ecosDataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.ecosDataGridView_CellValidating);
            this.ecosDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.ecosDataGridView_CellValueChanged);
            this.ecosDataGridView.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.ecosDataGridView_UserDeletingRow);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(65, 398);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(124, 13);
            this.label8.TabIndex = 124;
            this.label8.Text = "Diagram Page ECO\'s";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(493, 398);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(140, 13);
            this.label9.TabIndex = 126;
            this.label9.Text = "Sheet Edge Information";
            // 
            // sheetEdgeDataGridView
            // 
            this.sheetEdgeDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sheetEdgeDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.sheetEdgeDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sheetEdgeDataGridView.Location = new System.Drawing.Point(285, 430);
            this.sheetEdgeDataGridView.Name = "sheetEdgeDataGridView";
            this.sheetEdgeDataGridView.Size = new System.Drawing.Size(543, 164);
            this.sheetEdgeDataGridView.TabIndex = 127;
            this.sheetEdgeDataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.sheetEdgeDataGridView_CellValidating);
            this.sheetEdgeDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.sheetEdgeDataGridView_CellValueChanged);
            this.sheetEdgeDataGridView.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.sheetEdgeDataGridView_UserDeletingRow);
            // 
            // editEdgeConnectionsButton
            // 
            this.editEdgeConnectionsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editEdgeConnectionsButton.Location = new System.Drawing.Point(452, 206);
            this.editEdgeConnectionsButton.Name = "editEdgeConnectionsButton";
            this.editEdgeConnectionsButton.Size = new System.Drawing.Size(98, 57);
            this.editEdgeConnectionsButton.TabIndex = 128;
            this.editEdgeConnectionsButton.Text = "Edit\r\nEdge\r\nConnections";
            this.editEdgeConnectionsButton.UseVisualStyleBackColor = true;
            this.editEdgeConnectionsButton.Click += new System.EventHandler(this.editEdgeConnectionsButton_Click);
            // 
            // editDiagramBlocksButton
            // 
            this.editDiagramBlocksButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editDiagramBlocksButton.Location = new System.Drawing.Point(565, 206);
            this.editDiagramBlocksButton.Name = "editDiagramBlocksButton";
            this.editDiagramBlocksButton.Size = new System.Drawing.Size(98, 57);
            this.editDiagramBlocksButton.TabIndex = 129;
            this.editDiagramBlocksButton.Text = "Edit ALD Blocks";
            this.editDiagramBlocksButton.UseVisualStyleBackColor = true;
            this.editDiagramBlocksButton.Click += new System.EventHandler(this.editDiagramBlocksButton_Click);
            // 
            // commentTextBox
            // 
            this.commentTextBox.Location = new System.Drawing.Point(92, 285);
            this.commentTextBox.MaxLength = 1023;
            this.commentTextBox.Multiline = true;
            this.commentTextBox.Name = "commentTextBox";
            this.commentTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.commentTextBox.Size = new System.Drawing.Size(570, 95);
            this.commentTextBox.TabIndex = 60;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(19, 304);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(54, 13);
            this.label10.TabIndex = 131;
            this.label10.Text = "Comment:";
            // 
            // noHDLGenerationCheckBox
            // 
            this.noHDLGenerationCheckBox.AutoSize = true;
            this.noHDLGenerationCheckBox.Location = new System.Drawing.Point(452, 159);
            this.noHDLGenerationCheckBox.Name = "noHDLGenerationCheckBox";
            this.noHDLGenerationCheckBox.Size = new System.Drawing.Size(120, 17);
            this.noHDLGenerationCheckBox.TabIndex = 132;
            this.noHDLGenerationCheckBox.Text = "No HDL Generation";
            this.noHDLGenerationCheckBox.UseVisualStyleBackColor = true;
            // 
            // EditDiagramPageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 677);
            this.Controls.Add(this.noHDLGenerationCheckBox);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.commentTextBox);
            this.Controls.Add(this.editDiagramBlocksButton);
            this.Controls.Add(this.editEdgeConnectionsButton);
            this.Controls.Add(this.sheetEdgeDataGridView);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.ecosDataGridView);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.addApplyButton);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.newPageButton);
            this.Controls.Add(this.volumeSetComboBox);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.stampTextBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.titleTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.partTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.volumeComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.machineComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pageComboBox);
            this.Controls.Add(this.label1);
            this.Name = "EditDiagramPageForm";
            this.Text = "Edit ALD Diagram";
            this.Shown += new System.EventHandler(this.EditDiagramPageForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.ecosDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sheetEdgeDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button newPageButton;
        private System.Windows.Forms.ComboBox volumeSetComboBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox stampTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox titleTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox partTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox volumeComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox machineComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox pageComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button addApplyButton;
        private System.Windows.Forms.DataGridView ecosDataGridView;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DataGridView sheetEdgeDataGridView;
        private System.Windows.Forms.Button editEdgeConnectionsButton;
        private System.Windows.Forms.Button editDiagramBlocksButton;
        private System.Windows.Forms.TextBox commentTextBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox noHDLGenerationCheckBox;
    }
}