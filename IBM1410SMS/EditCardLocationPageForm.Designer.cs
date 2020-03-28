namespace IBM1410SMS
{
    partial class EditCardLocationPageForm
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
            this.pageComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.machineComboBox = new System.Windows.Forms.ComboBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.volumeComboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.titleTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.ecoComboBox = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.selectPanelLabel = new System.Windows.Forms.Label();
            this.panelComboBox = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.sheetsComboBox = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.runTextBox = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.volumeSetComboBox = new System.Windows.Forms.ComboBox();
            this.previousEcoComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.partTextBox = new System.Windows.Forms.TextBox();
            this.stampTextBox = new System.Windows.Forms.TextBox();
            this.newPageButton = new System.Windows.Forms.Button();
            this.addApplyButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.purgeButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(45, 124);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select Page (existing):";
            this.toolTip1.SetToolTip(this.label1, "Select an existing page to edit that page.");
            // 
            // pageComboBox
            // 
            this.pageComboBox.DisplayMember = "name";
            this.pageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.pageComboBox.FormattingEnabled = true;
            this.pageComboBox.Location = new System.Drawing.Point(163, 121);
            this.pageComboBox.Name = "pageComboBox";
            this.pageComboBox.Size = new System.Drawing.Size(212, 21);
            this.pageComboBox.TabIndex = 99;
            this.toolTip1.SetToolTip(this.pageComboBox, "Select an existing page to edit that page.");
            this.pageComboBox.SelectedIndexChanged += new System.EventHandler(this.pageComboBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(45, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Select machine:";
            this.toolTip1.SetToolTip(this.label2, "Select Machine, then Volume");
            // 
            // machineComboBox
            // 
            this.machineComboBox.DisplayMember = "name";
            this.machineComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.machineComboBox.FormattingEnabled = true;
            this.machineComboBox.Location = new System.Drawing.Point(163, 27);
            this.machineComboBox.Name = "machineComboBox";
            this.machineComboBox.Size = new System.Drawing.Size(121, 21);
            this.machineComboBox.TabIndex = 99;
            this.toolTip1.SetToolTip(this.machineComboBox, "Select Machine, then Volume");
            this.machineComboBox.SelectedIndexChanged += new System.EventHandler(this.machineComboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(357, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Select Volume:";
            this.toolTip1.SetToolTip(this.label3, "Select Machine, then Volume");
            // 
            // volumeComboBox
            // 
            this.volumeComboBox.DisplayMember = "name";
            this.volumeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.volumeComboBox.FormattingEnabled = true;
            this.volumeComboBox.Location = new System.Drawing.Point(441, 73);
            this.volumeComboBox.Name = "volumeComboBox";
            this.volumeComboBox.Size = new System.Drawing.Size(66, 21);
            this.volumeComboBox.TabIndex = 99;
            this.toolTip1.SetToolTip(this.volumeComboBox, "Select Machine, then Volume");
            this.volumeComboBox.SelectedIndexChanged += new System.EventHandler(this.volumeComboBox_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(45, 218);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Page Name:";
            this.toolTip1.SetToolTip(this.label4, "Name of the page (e.g. 10.2.2.5)");
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(118, 215);
            this.nameTextBox.MaxLength = 16;
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(100, 20);
            this.nameTextBox.TabIndex = 7;
            this.toolTip1.SetToolTip(this.nameTextBox, "Name of the page (e.g. 10.2.2.5)");
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(45, 265);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Title: ";
            this.toolTip1.SetToolTip(this.label6, "Title at the top of the page (e.g. X Register)");
            // 
            // titleTextBox
            // 
            this.titleTextBox.Location = new System.Drawing.Point(118, 262);
            this.titleTextBox.MaxLength = 80;
            this.titleTextBox.Name = "titleTextBox";
            this.titleTextBox.Size = new System.Drawing.Size(319, 20);
            this.titleTextBox.TabIndex = 9;
            this.toolTip1.SetToolTip(this.titleTextBox, "Title at the top of the page (e.g. X Register)");
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(45, 312);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Stamp (opt):";
            this.toolTip1.SetToolTip(this.label7, "Stamp on the page (e.g. FIELD USE)");
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(45, 359);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "ECO:";
            this.toolTip1.SetToolTip(this.label8, "ECO associated with page");
            // 
            // ecoComboBox
            // 
            this.ecoComboBox.DisplayMember = "eco";
            this.ecoComboBox.FormattingEnabled = true;
            this.ecoComboBox.Location = new System.Drawing.Point(118, 356);
            this.ecoComboBox.MaxLength = 20;
            this.ecoComboBox.Name = "ecoComboBox";
            this.ecoComboBox.Size = new System.Drawing.Size(121, 21);
            this.ecoComboBox.TabIndex = 11;
            this.toolTip1.SetToolTip(this.ecoComboBox, "ECO associated with page");
            this.ecoComboBox.TextChanged += new System.EventHandler(this.ecoComboBox_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(259, 361);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(100, 13);
            this.label9.TabIndex = 16;
            this.label9.Text = "Previous ECO (opt):";
            this.toolTip1.SetToolTip(this.label9, "Previous ECO identified on  page");
            // 
            // selectPanelLabel
            // 
            this.selectPanelLabel.AutoSize = true;
            this.selectPanelLabel.Location = new System.Drawing.Point(45, 171);
            this.selectPanelLabel.Name = "selectPanelLabel";
            this.selectPanelLabel.Size = new System.Drawing.Size(37, 13);
            this.selectPanelLabel.TabIndex = 18;
            this.selectPanelLabel.Text = "Panel:";
            this.toolTip1.SetToolTip(this.selectPanelLabel, "Panel/Gate/Frame that this page lays out");
            // 
            // panelComboBox
            // 
            this.panelComboBox.DisplayMember = "panel";
            this.panelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.panelComboBox.FormattingEnabled = true;
            this.panelComboBox.Location = new System.Drawing.Point(118, 168);
            this.panelComboBox.Name = "panelComboBox";
            this.panelComboBox.Size = new System.Drawing.Size(257, 21);
            this.panelComboBox.TabIndex = 99;
            this.toolTip1.SetToolTip(this.panelComboBox, "Panel/Gate/Frame that this page lays out");
            this.panelComboBox.SelectedIndexChanged += new System.EventHandler(this.panelComboBox_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(403, 171);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(46, 13);
            this.label11.TabIndex = 20;
            this.label11.Text = "Sheets: ";
            this.toolTip1.SetToolTip(this.label11, "Number of sheets comprising this page");
            // 
            // sheetsComboBox
            // 
            this.sheetsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sheetsComboBox.FormattingEnabled = true;
            this.sheetsComboBox.Location = new System.Drawing.Point(455, 168);
            this.sheetsComboBox.Name = "sheetsComboBox";
            this.sheetsComboBox.Size = new System.Drawing.Size(52, 21);
            this.sheetsComboBox.TabIndex = 99;
            this.toolTip1.SetToolTip(this.sheetsComboBox, "Number of sheets comprising this page");
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(45, 403);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(54, 13);
            this.label12.TabIndex = 22;
            this.label12.Text = "Run (opt):";
            this.toolTip1.SetToolTip(this.label12, "The print run number listed on this page");
            // 
            // runTextBox
            // 
            this.runTextBox.Location = new System.Drawing.Point(118, 400);
            this.runTextBox.MaxLength = 10;
            this.runTextBox.Name = "runTextBox";
            this.runTextBox.Size = new System.Drawing.Size(100, 20);
            this.runTextBox.TabIndex = 13;
            this.toolTip1.SetToolTip(this.runTextBox, "The print run number listed on this page");
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(45, 77);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(61, 13);
            this.label13.TabIndex = 23;
            this.label13.Text = "Volume Set";
            this.toolTip1.SetToolTip(this.label13, "Select the volume set for the volume in which this page resides");
            // 
            // volumeSetComboBox
            // 
            this.volumeSetComboBox.DisplayMember = "machinetype";
            this.volumeSetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.volumeSetComboBox.FormattingEnabled = true;
            this.volumeSetComboBox.Location = new System.Drawing.Point(163, 73);
            this.volumeSetComboBox.Name = "volumeSetComboBox";
            this.volumeSetComboBox.Size = new System.Drawing.Size(180, 21);
            this.volumeSetComboBox.TabIndex = 99;
            this.toolTip1.SetToolTip(this.volumeSetComboBox, "Select the volume set for the volume in which this page resides");
            this.volumeSetComboBox.SelectedIndexChanged += new System.EventHandler(this.volumeSetComboBox_SelectedIndexChanged);
            // 
            // previousEcoComboBox
            // 
            this.previousEcoComboBox.DisplayMember = "eco";
            this.previousEcoComboBox.FormattingEnabled = true;
            this.previousEcoComboBox.Location = new System.Drawing.Point(365, 356);
            this.previousEcoComboBox.MaxLength = 20;
            this.previousEcoComboBox.Name = "previousEcoComboBox";
            this.previousEcoComboBox.Size = new System.Drawing.Size(142, 21);
            this.previousEcoComboBox.TabIndex = 12;
            this.toolTip1.SetToolTip(this.previousEcoComboBox, "Previous ECO associated with page");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(245, 218);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Part No.:";
            this.toolTip1.SetToolTip(this.label5, "IBM part number for this page.");
            // 
            // partTextBox
            // 
            this.partTextBox.Location = new System.Drawing.Point(300, 215);
            this.partTextBox.MaxLength = 20;
            this.partTextBox.Name = "partTextBox";
            this.partTextBox.Size = new System.Drawing.Size(127, 20);
            this.partTextBox.TabIndex = 8;
            this.toolTip1.SetToolTip(this.partTextBox, "IBM part number for this page.");
            // 
            // stampTextBox
            // 
            this.stampTextBox.Location = new System.Drawing.Point(118, 309);
            this.stampTextBox.MaxLength = 45;
            this.stampTextBox.Name = "stampTextBox";
            this.stampTextBox.Size = new System.Drawing.Size(319, 20);
            this.stampTextBox.TabIndex = 10;
            this.toolTip1.SetToolTip(this.stampTextBox, "Stamp on the page (e.g. FIELD USE)");
            // 
            // newPageButton
            // 
            this.newPageButton.Location = new System.Drawing.Point(441, 119);
            this.newPageButton.Name = "newPageButton";
            this.newPageButton.Size = new System.Drawing.Size(75, 23);
            this.newPageButton.TabIndex = 99;
            this.newPageButton.Text = "New Page";
            this.toolTip1.SetToolTip(this.newPageButton, "Click here if you wish to add a NEW page.");
            this.newPageButton.UseVisualStyleBackColor = true;
            this.newPageButton.Click += new System.EventHandler(this.newPageButton_Click);
            // 
            // addApplyButton
            // 
            this.addApplyButton.Location = new System.Drawing.Point(48, 445);
            this.addApplyButton.Name = "addApplyButton";
            this.addApplyButton.Size = new System.Drawing.Size(75, 23);
            this.addApplyButton.TabIndex = 20;
            this.addApplyButton.Text = "TBA";
            this.addApplyButton.UseVisualStyleBackColor = true;
            this.addApplyButton.Click += new System.EventHandler(this.addApplyButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(396, 445);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 23;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(164, 445);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 21;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Visible = false;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(403, 124);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(32, 13);
            this.label14.TabIndex = 100;
            this.label14.Text = "OR:  ";
            // 
            // purgeButton
            // 
            this.purgeButton.Location = new System.Drawing.Point(280, 445);
            this.purgeButton.Name = "purgeButton";
            this.purgeButton.Size = new System.Drawing.Size(75, 23);
            this.purgeButton.TabIndex = 101;
            this.purgeButton.Text = "Force Purge";
            this.purgeButton.UseVisualStyleBackColor = true;
            this.purgeButton.Click += new System.EventHandler(this.purgeButton_Click);
            // 
            // EditCardLocationPageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 492);
            this.Controls.Add(this.purgeButton);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.newPageButton);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.volumeSetComboBox);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.addApplyButton);
            this.Controls.Add(this.runTextBox);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.sheetsComboBox);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.panelComboBox);
            this.Controls.Add(this.selectPanelLabel);
            this.Controls.Add(this.previousEcoComboBox);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.ecoComboBox);
            this.Controls.Add(this.label8);
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
            this.Name = "EditCardLocationPageForm";
            this.Text = "Edit Card Location Page Form";
            this.toolTip1.SetToolTip(this, "Select the Volume Set for the Volume that contains this page.");
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox pageComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ComboBox machineComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox volumeComboBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox partTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox titleTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox stampTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox ecoComboBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox previousEcoComboBox;
        private System.Windows.Forms.Label selectPanelLabel;
        private System.Windows.Forms.ComboBox panelComboBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox sheetsComboBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox runTextBox;
        private System.Windows.Forms.Button addApplyButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox volumeSetComboBox;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button newPageButton;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button purgeButton;
    }
}