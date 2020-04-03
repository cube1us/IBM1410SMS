namespace IBM1410SMS
{
    partial class EditCardLocationForm
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
            this.volumeComboBox = new System.Windows.Forms.ComboBox();
            this.pageComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.selectPanelLabel = new System.Windows.Forms.Label();
            this.panelComboBox = new System.Windows.Forms.ComboBox();
            this.selectRowLabel = new System.Windows.Forms.Label();
            this.rowComboBox = new System.Windows.Forms.ComboBox();
            this.selectColumnLabel = new System.Windows.Forms.Label();
            this.columnComboBox = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cardTypeComboBox = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.featureComboBox = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.crossedOutCheckBox = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.bottomNotesTextBox = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.applyButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.cardLocationBlockDataGridView = new System.Windows.Forms.DataGridView();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.ecoComboBox = new System.Windows.Forms.ComboBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.cardLocationBlockDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Machine:";
            this.toolTip1.SetToolTip(this.label1, "Select the Machine, Volume Set, Volume and Page describing this card location");
            // 
            // machineComboBox
            // 
            this.machineComboBox.DisplayMember = "name";
            this.machineComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.machineComboBox.FormattingEnabled = true;
            this.machineComboBox.Location = new System.Drawing.Point(83, 45);
            this.machineComboBox.Name = "machineComboBox";
            this.machineComboBox.Size = new System.Drawing.Size(121, 21);
            this.machineComboBox.TabIndex = 1;
            this.toolTip1.SetToolTip(this.machineComboBox, "Select the Machine, Volume and Page describing this card location");
            this.machineComboBox.SelectedIndexChanged += new System.EventHandler(this.machineComboBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Volume:";
            this.toolTip1.SetToolTip(this.label2, "Select the Machine, Volume and Page describing this card location");
            // 
            // volumeComboBox
            // 
            this.volumeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.volumeComboBox.FormattingEnabled = true;
            this.volumeComboBox.Location = new System.Drawing.Point(83, 79);
            this.volumeComboBox.Name = "volumeComboBox";
            this.volumeComboBox.Size = new System.Drawing.Size(179, 21);
            this.volumeComboBox.TabIndex = 2;
            this.toolTip1.SetToolTip(this.volumeComboBox, "Select the Machine, Volume and Page describing this card location");
            this.volumeComboBox.SelectedIndexChanged += new System.EventHandler(this.volumeComboBox_SelectedIndexChanged);
            // 
            // pageComboBox
            // 
            this.pageComboBox.DisplayMember = "name";
            this.pageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.pageComboBox.FormattingEnabled = true;
            this.pageComboBox.Location = new System.Drawing.Point(83, 113);
            this.pageComboBox.Name = "pageComboBox";
            this.pageComboBox.Size = new System.Drawing.Size(121, 21);
            this.pageComboBox.TabIndex = 3;
            this.toolTip1.SetToolTip(this.pageComboBox, "Select the Machine, Volume and Page describing this card location");
            this.pageComboBox.ValueMember = "idPage";
            this.pageComboBox.SelectedIndexChanged += new System.EventHandler(this.pageComboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 101;
            this.label3.Text = "Page:";
            this.toolTip1.SetToolTip(this.label3, "Select the Machine, Volume and Page describing this card location");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(106, 14);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 102;
            this.label4.Text = "Drawing Page";
            this.toolTip1.SetToolTip(this.label4, "Select the Machine, Volume Set, Volume and Page describing this card location");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(394, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 103;
            this.label5.Text = "Card Slot";
            this.toolTip1.SetToolTip(this.label5, "Select the Panel, Row and Column containing this card");
            // 
            // selectPanelLabel
            // 
            this.selectPanelLabel.AutoSize = true;
            this.selectPanelLabel.Location = new System.Drawing.Point(280, 48);
            this.selectPanelLabel.Name = "selectPanelLabel";
            this.selectPanelLabel.Size = new System.Drawing.Size(37, 13);
            this.selectPanelLabel.TabIndex = 104;
            this.selectPanelLabel.Text = "Panel:";
            this.toolTip1.SetToolTip(this.selectPanelLabel, "Select the Panel, Row and Column containing this card");
            // 
            // panelComboBox
            // 
            this.panelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.panelComboBox.FormattingEnabled = true;
            this.panelComboBox.Location = new System.Drawing.Point(320, 45);
            this.panelComboBox.Name = "panelComboBox";
            this.panelComboBox.Size = new System.Drawing.Size(198, 21);
            this.panelComboBox.TabIndex = 6;
            this.toolTip1.SetToolTip(this.panelComboBox, "Select the Panel, Row and Column containing this card");
            this.panelComboBox.SelectedIndexChanged += new System.EventHandler(this.panelComboBox_SelectedIndexChanged);
            // 
            // selectRowLabel
            // 
            this.selectRowLabel.AutoSize = true;
            this.selectRowLabel.Location = new System.Drawing.Point(280, 82);
            this.selectRowLabel.Name = "selectRowLabel";
            this.selectRowLabel.Size = new System.Drawing.Size(57, 13);
            this.selectRowLabel.TabIndex = 106;
            this.selectRowLabel.Text = "Card Row:";
            this.toolTip1.SetToolTip(this.selectRowLabel, "Select the Panel, Row and Column containing this card");
            // 
            // rowComboBox
            // 
            this.rowComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rowComboBox.FormattingEnabled = true;
            this.rowComboBox.Location = new System.Drawing.Point(341, 79);
            this.rowComboBox.Name = "rowComboBox";
            this.rowComboBox.Size = new System.Drawing.Size(57, 21);
            this.rowComboBox.TabIndex = 7;
            this.toolTip1.SetToolTip(this.rowComboBox, "Select the Panel, Row and Column containing this card");
            this.rowComboBox.SelectedIndexChanged += new System.EventHandler(this.rowComboBox_SelectedIndexChanged);
            // 
            // selectColumnLabel
            // 
            this.selectColumnLabel.AutoSize = true;
            this.selectColumnLabel.Location = new System.Drawing.Point(280, 115);
            this.selectColumnLabel.Name = "selectColumnLabel";
            this.selectColumnLabel.Size = new System.Drawing.Size(50, 13);
            this.selectColumnLabel.TabIndex = 108;
            this.selectColumnLabel.Text = "Card Col:";
            this.toolTip1.SetToolTip(this.selectColumnLabel, "Select the Panel, Row and Column containing this card");
            // 
            // columnComboBox
            // 
            this.columnComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.columnComboBox.FormattingEnabled = true;
            this.columnComboBox.Location = new System.Drawing.Point(341, 112);
            this.columnComboBox.Name = "columnComboBox";
            this.columnComboBox.Size = new System.Drawing.Size(57, 21);
            this.columnComboBox.TabIndex = 8;
            this.toolTip1.SetToolTip(this.columnComboBox, "Select the Panel, Row and Column containing this card");
            this.columnComboBox.SelectedIndexChanged += new System.EventHandler(this.columnComboBox_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(15, 205);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(62, 13);
            this.label9.TabIndex = 110;
            this.label9.Text = "Card Type: ";
            this.toolTip1.SetToolTip(this.label9, "Identify the type of card in this slot");
            // 
            // cardTypeComboBox
            // 
            this.cardTypeComboBox.DisplayMember = "type";
            this.cardTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cardTypeComboBox.FormattingEnabled = true;
            this.cardTypeComboBox.Location = new System.Drawing.Point(83, 202);
            this.cardTypeComboBox.Name = "cardTypeComboBox";
            this.cardTypeComboBox.Size = new System.Drawing.Size(107, 21);
            this.cardTypeComboBox.TabIndex = 9;
            this.toolTip1.SetToolTip(this.cardTypeComboBox, "Identify the type of card in this slot");
            this.cardTypeComboBox.ValueMember = "idCardType";
            this.cardTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.cardTypeComboBox_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(213, 205);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 13);
            this.label10.TabIndex = 112;
            this.label10.Text = "Feature:";
            this.toolTip1.SetToolTip(this.label10, "Machine special feature, if any, associated with this card");
            // 
            // featureComboBox
            // 
            this.featureComboBox.DisplayMember = "code";
            this.featureComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.featureComboBox.FormattingEnabled = true;
            this.featureComboBox.Location = new System.Drawing.Point(265, 202);
            this.featureComboBox.Name = "featureComboBox";
            this.featureComboBox.Size = new System.Drawing.Size(188, 21);
            this.featureComboBox.TabIndex = 10;
            this.toolTip1.SetToolTip(this.featureComboBox, "Machine special feature, if any, associated with this card");
            this.featureComboBox.ValueMember = "idFeature";
            this.featureComboBox.SelectedIndexChanged += new System.EventHandler(this.featureComboBox_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(457, 185);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(86, 13);
            this.label11.TabIndex = 114;
            this.label11.Text = "Slot Crossed Out";
            this.toolTip1.SetToolTip(this.label11, "Select if this slot is crossed out in the Card Location Chart");
            // 
            // crossedOutCheckBox
            // 
            this.crossedOutCheckBox.AutoSize = true;
            this.crossedOutCheckBox.Location = new System.Drawing.Point(489, 207);
            this.crossedOutCheckBox.Name = "crossedOutCheckBox";
            this.crossedOutCheckBox.Size = new System.Drawing.Size(15, 14);
            this.crossedOutCheckBox.TabIndex = 11;
            this.toolTip1.SetToolTip(this.crossedOutCheckBox, "Select if this slot is crossed out in the Card Location Chart");
            this.crossedOutCheckBox.UseVisualStyleBackColor = true;
            this.crossedOutCheckBox.CheckedChanged += new System.EventHandler(this.crossedOutCheckBox_CheckedChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(15, 238);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(80, 13);
            this.label12.TabIndex = 116;
            this.label12.Text = "Bottom Note(s):";
            this.toolTip1.SetToolTip(this.label12, "Enter the notes at the bottom of the card location cell, separated by commas");
            // 
            // bottomNotesTextBox
            // 
            this.bottomNotesTextBox.Location = new System.Drawing.Point(104, 235);
            this.bottomNotesTextBox.Name = "bottomNotesTextBox";
            this.bottomNotesTextBox.Size = new System.Drawing.Size(240, 20);
            this.bottomNotesTextBox.TabIndex = 12;
            this.toolTip1.SetToolTip(this.bottomNotesTextBox, "Enter the notes at the bottom of the card location cell, separated by commas");
            this.bottomNotesTextBox.TextChanged += new System.EventHandler(this.bottomNotesTextBox_TextChanged);
            this.bottomNotesTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.bottomNotesTextBox_Validating);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(361, 239);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(192, 13);
            this.label13.TabIndex = 118;
            this.label13.Text = "(1-2 chars. ea., Separate with Commas)";
            // 
            // applyButton
            // 
            this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.applyButton.Location = new System.Drawing.Point(18, 448);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(75, 23);
            this.applyButton.TabIndex = 20;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.deleteButton.Location = new System.Drawing.Point(198, 448);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 23);
            this.deleteButton.TabIndex = 21;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelButton.Location = new System.Drawing.Point(378, 448);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 22;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // cardLocationBlockDataGridView
            // 
            this.cardLocationBlockDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cardLocationBlockDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.cardLocationBlockDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.cardLocationBlockDataGridView.Location = new System.Drawing.Point(12, 289);
            this.cardLocationBlockDataGridView.Name = "cardLocationBlockDataGridView";
            this.cardLocationBlockDataGridView.Size = new System.Drawing.Size(551, 128);
            this.cardLocationBlockDataGridView.TabIndex = 119;
            this.cardLocationBlockDataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.cardLocationBlockDataGridView_CellValidating);
            this.cardLocationBlockDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.cardLocationBlockDataGridView_CellValueChanged);
            this.cardLocationBlockDataGridView.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.cardLocationBlockDataGridView_EditingControlShowing);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(237, 273);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(109, 13);
            this.label14.TabIndex = 120;
            this.label14.Text = "Sheet References";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(15, 149);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(60, 13);
            this.label15.TabIndex = 121;
            this.label15.Text = "Page ECO:";
            // 
            // ecoComboBox
            // 
            this.ecoComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ecoComboBox.FormattingEnabled = true;
            this.ecoComboBox.Location = new System.Drawing.Point(83, 146);
            this.ecoComboBox.Name = "ecoComboBox";
            this.ecoComboBox.Size = new System.Drawing.Size(121, 21);
            this.ecoComboBox.TabIndex = 4;
            this.ecoComboBox.SelectedIndexChanged += new System.EventHandler(this.ecoComboBox_SelectedIndexChanged);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // EditCardLocationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(575, 486);
            this.Controls.Add(this.ecoComboBox);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.cardLocationBlockDataGridView);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.bottomNotesTextBox);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.crossedOutCheckBox);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.featureComboBox);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.cardTypeComboBox);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.columnComboBox);
            this.Controls.Add(this.selectColumnLabel);
            this.Controls.Add(this.rowComboBox);
            this.Controls.Add(this.selectRowLabel);
            this.Controls.Add(this.panelComboBox);
            this.Controls.Add(this.selectPanelLabel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pageComboBox);
            this.Controls.Add(this.volumeComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.machineComboBox);
            this.Controls.Add(this.label1);
            this.Name = "EditCardLocationForm";
            this.Text = "Edit Card Location";
            this.toolTip1.SetToolTip(this, "Select if this slot is crossed out in the Card Location Chart");
            this.Shown += new System.EventHandler(this.EditCardLocationForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.cardLocationBlockDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox machineComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox volumeComboBox;
        private System.Windows.Forms.ComboBox pageComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label selectPanelLabel;
        private System.Windows.Forms.ComboBox panelComboBox;
        private System.Windows.Forms.Label selectRowLabel;
        private System.Windows.Forms.ComboBox rowComboBox;
        private System.Windows.Forms.Label selectColumnLabel;
        private System.Windows.Forms.ComboBox columnComboBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cardTypeComboBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox featureComboBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox crossedOutCheckBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox bottomNotesTextBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.DataGridView cardLocationBlockDataGridView;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox ecoComboBox;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}