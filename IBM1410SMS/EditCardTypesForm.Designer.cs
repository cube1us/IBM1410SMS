namespace IBM1410SMS
{
    partial class EditCardTypesForm
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
            this.cardTypeComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.partTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.typeTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.volumeComboBox = new System.Windows.Forms.ComboBox();
            this.newCardTypeButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.logicFamilyComboBox = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.approvedByTextBox = new System.Windows.Forms.TextBox();
            this.approvedDatePicker = new System.Windows.Forms.DateTimePicker();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.holePatternTextBox = new System.Windows.Forms.TextBox();
            this.singleRadioButton = new System.Windows.Forms.RadioButton();
            this.doubleRadioButton = new System.Windows.Forms.RadioButton();
            this.label11 = new System.Windows.Forms.Label();
            this.developmentNumberTextBox = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.nameTypeTextBox = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.designApproverTextBox = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.designDatePicker = new System.Windows.Forms.DateTimePicker();
            this.label15 = new System.Windows.Forms.Label();
            this.detailerTextBox = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.detailDatePicker = new System.Windows.Forms.DateTimePicker();
            this.label17 = new System.Windows.Forms.Label();
            this.designCheckerTextBox = new System.Windows.Forms.TextBox();
            this.designCheckDatePicker = new System.Windows.Forms.DateTimePicker();
            this.label18 = new System.Windows.Forms.Label();
            this.approvalDatePicker = new System.Windows.Forms.DateTimePicker();
            this.label19 = new System.Windows.Forms.Label();
            this.approverTextBox = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.modelTypeTextBox = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.modelDeviceTextBox = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.scaleTextBox = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.drawTextBox = new System.Windows.Forms.TextBox();
            this.drawDatePicker = new System.Windows.Forms.DateTimePicker();
            this.label25 = new System.Windows.Forms.Label();
            this.drawingCheckerTextBox = new System.Windows.Forms.TextBox();
            this.drawingCheckDatePicker = new System.Windows.Forms.DateTimePicker();
            this.addApplyButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.updatedVolumeComboBox = new System.Windows.Forms.ComboBox();
            this.cardNotesDataGridView = new System.Windows.Forms.DataGridView();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label26 = new System.Windows.Forms.Label();
            this.cardECOsDataGridView = new System.Windows.Forms.DataGridView();
            this.deleteButton = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.label27 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.cardNotesDataGridView)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cardECOsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Card (Existing): ";
            this.toolTip1.SetToolTip(this.label1, "Select an existing SMS card to edit or remove");
            // 
            // cardTypeComboBox
            // 
            this.cardTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cardTypeComboBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cardTypeComboBox.FormattingEnabled = true;
            this.cardTypeComboBox.Location = new System.Drawing.Point(105, 40);
            this.cardTypeComboBox.Name = "cardTypeComboBox";
            this.cardTypeComboBox.Size = new System.Drawing.Size(212, 22);
            this.cardTypeComboBox.TabIndex = 2;
            this.toolTip1.SetToolTip(this.cardTypeComboBox, "Select an existing SMS card to edit or remove");
            this.cardTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.cardTypeComboBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(331, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Part#";
            this.toolTip1.SetToolTip(this.label2, "The IBM Part Number (6 Digits)");
            // 
            // partTextBox
            // 
            this.partTextBox.Location = new System.Drawing.Point(370, 40);
            this.partTextBox.MaxLength = 16;
            this.partTextBox.Name = "partTextBox";
            this.partTextBox.Size = new System.Drawing.Size(64, 20);
            this.partTextBox.TabIndex = 3;
            this.toolTip1.SetToolTip(this.partTextBox, "The IBM Part Number (6 Digits)");
            this.partTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.partTextBox_Validating);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(449, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Type:";
            this.toolTip1.SetToolTip(this.label3, "IBM SMS Card Type (e.g. DCK-)");
            // 
            // typeTextBox
            // 
            this.typeTextBox.Location = new System.Drawing.Point(489, 40);
            this.typeTextBox.MaxLength = 8;
            this.typeTextBox.Name = "typeTextBox";
            this.typeTextBox.Size = new System.Drawing.Size(64, 20);
            this.typeTextBox.TabIndex = 4;
            this.toolTip1.SetToolTip(this.typeTextBox, "IBM SMS Card Type (e.g. DCK-)");
            this.typeTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.typeTextBox_Validating);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 14);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(138, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "SMS Card Diagram Volume:";
            this.toolTip1.SetToolTip(this.label4, "Volume in Volume Set that this card diagram appears in");
            // 
            // volumeComboBox
            // 
            this.volumeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.volumeComboBox.FormattingEnabled = true;
            this.volumeComboBox.Location = new System.Drawing.Point(156, 11);
            this.volumeComboBox.Name = "volumeComboBox";
            this.volumeComboBox.Size = new System.Drawing.Size(259, 21);
            this.volumeComboBox.TabIndex = 1;
            this.toolTip1.SetToolTip(this.volumeComboBox, "Volume in Volume Set that this card diagram appears in");
            this.volumeComboBox.SelectedIndexChanged += new System.EventHandler(this.volumeComboBox_SelectedIndexChanged);
            // 
            // newCardTypeButton
            // 
            this.newCardTypeButton.Location = new System.Drawing.Point(576, 38);
            this.newCardTypeButton.Name = "newCardTypeButton";
            this.newCardTypeButton.Size = new System.Drawing.Size(95, 23);
            this.newCardTypeButton.TabIndex = 5;
            this.newCardTypeButton.Text = "New Card Type";
            this.toolTip1.SetToolTip(this.newCardTypeButton, "Click to start adding a new Card Type");
            this.newCardTypeButton.UseVisualStyleBackColor = true;
            this.newCardTypeButton.Click += new System.EventHandler(this.newCardTypeButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(221, 77);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Name:";
            this.toolTip1.SetToolTip(this.label5, "Name for this card (at top of SMS card page)");
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(265, 74);
            this.nameTextBox.MaxLength = 80;
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(406, 20);
            this.nameTextBox.TabIndex = 7;
            this.toolTip1.SetToolTip(this.nameTextBox, "Name for this card (at top of SMS card page)");
            this.nameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.nameTextBox_Validating);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 77);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Logic Family: ";
            this.toolTip1.SetToolTip(this.label7, "Select the Logic Family (e.g., SDTRL, SDTDL, CONVERTER, etc.)");
            // 
            // logicFamilyComboBox
            // 
            this.logicFamilyComboBox.DisplayMember = "name";
            this.logicFamilyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.logicFamilyComboBox.FormattingEnabled = true;
            this.logicFamilyComboBox.Location = new System.Drawing.Point(85, 74);
            this.logicFamilyComboBox.Name = "logicFamilyComboBox";
            this.logicFamilyComboBox.Size = new System.Drawing.Size(121, 21);
            this.logicFamilyComboBox.TabIndex = 6;
            this.toolTip1.SetToolTip(this.logicFamilyComboBox, "Select the Logic Family (e.g., SDTRL, SDTDL, CONVERTER, etc.)");
            this.logicFamilyComboBox.ValueMember = "idLogicFamily";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 332);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "APPROVAL: ";
            this.toolTip1.SetToolTip(this.label8, "Overall Approval (at top of info block at bottom of page)");
            // 
            // approvedByTextBox
            // 
            this.approvedByTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.approvedByTextBox.Location = new System.Drawing.Point(79, 329);
            this.approvedByTextBox.MaxLength = 45;
            this.approvedByTextBox.Name = "approvedByTextBox";
            this.approvedByTextBox.Size = new System.Drawing.Size(100, 20);
            this.approvedByTextBox.TabIndex = 11;
            this.toolTip1.SetToolTip(this.approvedByTextBox, "Overall Approval (at top of info block at bottom of page)");
            // 
            // approvedDatePicker
            // 
            this.approvedDatePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.approvedDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.approvedDatePicker.Location = new System.Drawing.Point(226, 329);
            this.approvedDatePicker.Name = "approvedDatePicker";
            this.approvedDatePicker.Size = new System.Drawing.Size(98, 20);
            this.approvedDatePicker.TabIndex = 12;
            this.toolTip1.SetToolTip(this.approvedDatePicker, "Date of overall Approval (at top of info block at bottom of page)");
            this.approvedDatePicker.Value = new System.DateTime(1960, 1, 1, 0, 0, 0, 0);
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(187, 332);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(33, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "Date:";
            this.toolTip1.SetToolTip(this.label9, "Date of overall Approval (at top of info block at bottom of page)");
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(334, 332);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(72, 13);
            this.label10.TabIndex = 16;
            this.label10.Text = "Hole Pattern: ";
            this.toolTip1.SetToolTip(this.label10, "The hole drill pattern used for this card (6 digits)");
            // 
            // holePatternTextBox
            // 
            this.holePatternTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.holePatternTextBox.Location = new System.Drawing.Point(407, 329);
            this.holePatternTextBox.MaxLength = 8;
            this.holePatternTextBox.Name = "holePatternTextBox";
            this.holePatternTextBox.Size = new System.Drawing.Size(64, 20);
            this.holePatternTextBox.TabIndex = 13;
            this.toolTip1.SetToolTip(this.holePatternTextBox, "The hole drill pattern used for this card (6 digits)");
            // 
            // singleRadioButton
            // 
            this.singleRadioButton.AutoSize = true;
            this.singleRadioButton.Checked = true;
            this.singleRadioButton.Location = new System.Drawing.Point(10, 19);
            this.singleRadioButton.Name = "singleRadioButton";
            this.singleRadioButton.Size = new System.Drawing.Size(54, 17);
            this.singleRadioButton.TabIndex = 9;
            this.singleRadioButton.TabStop = true;
            this.singleRadioButton.Text = "Single";
            this.toolTip1.SetToolTip(this.singleRadioButton, "Select if the card is a single height card");
            this.singleRadioButton.UseVisualStyleBackColor = true;
            // 
            // doubleRadioButton
            // 
            this.doubleRadioButton.AutoSize = true;
            this.doubleRadioButton.Location = new System.Drawing.Point(82, 19);
            this.doubleRadioButton.Name = "doubleRadioButton";
            this.doubleRadioButton.Size = new System.Drawing.Size(59, 17);
            this.doubleRadioButton.TabIndex = 10;
            this.doubleRadioButton.TabStop = true;
            this.doubleRadioButton.Text = "Double";
            this.toolTip1.SetToolTip(this.doubleRadioButton, "Select if the card is a double height card");
            this.doubleRadioButton.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(250, 363);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(50, 13);
            this.label11.TabIndex = 17;
            this.label11.Text = "Dev. No.";
            this.toolTip1.SetToolTip(this.label11, "The Development No. identified on the bottom right corner of the SMS card sheet");
            // 
            // developmentNumberTextBox
            // 
            this.developmentNumberTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.developmentNumberTextBox.Location = new System.Drawing.Point(306, 360);
            this.developmentNumberTextBox.MaxLength = 8;
            this.developmentNumberTextBox.Name = "developmentNumberTextBox";
            this.developmentNumberTextBox.Size = new System.Drawing.Size(100, 20);
            this.developmentNumberTextBox.TabIndex = 15;
            this.toolTip1.SetToolTip(this.developmentNumberTextBox, "The Development No. identified on the bottom right corner of the SMS card sheet");
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(11, 363);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(71, 13);
            this.label12.TabIndex = 18;
            this.label12.Text = "Type (Name):";
            this.toolTip1.SetToolTip(this.label12, "This is a generic card type name that appears in the data block at the bottom of " +
        "the SMS card page (e.g. CARD ASM TSTR)");
            // 
            // nameTypeTextBox
            // 
            this.nameTypeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nameTypeTextBox.Location = new System.Drawing.Point(84, 360);
            this.nameTypeTextBox.MaxLength = 45;
            this.nameTypeTextBox.Name = "nameTypeTextBox";
            this.nameTypeTextBox.Size = new System.Drawing.Size(144, 20);
            this.nameTypeTextBox.TabIndex = 14;
            this.toolTip1.SetToolTip(this.nameTypeTextBox, "This is a generic card type name that appears in the data block at the bottom of " +
        "the SMS card page (e.g. CARD ASM TSTR)");
            this.nameTypeTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.nameTypeTextBox_Validating);
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(12, 394);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(43, 13);
            this.label13.TabIndex = 22;
            this.label13.Text = "Design:";
            this.toolTip1.SetToolTip(this.label13, "Initials of person that approved the original design");
            // 
            // designApproverTextBox
            // 
            this.designApproverTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.designApproverTextBox.Location = new System.Drawing.Point(55, 390);
            this.designApproverTextBox.MaxLength = 8;
            this.designApproverTextBox.Name = "designApproverTextBox";
            this.designApproverTextBox.Size = new System.Drawing.Size(58, 20);
            this.designApproverTextBox.TabIndex = 16;
            this.toolTip1.SetToolTip(this.designApproverTextBox, "Initials of person that approved the original design");
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(122, 394);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(33, 13);
            this.label14.TabIndex = 24;
            this.label14.Text = "Date:";
            this.toolTip1.SetToolTip(this.label14, "Date the orignal design was approved");
            // 
            // designDatePicker
            // 
            this.designDatePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.designDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.designDatePicker.Location = new System.Drawing.Point(156, 390);
            this.designDatePicker.Name = "designDatePicker";
            this.designDatePicker.Size = new System.Drawing.Size(102, 20);
            this.designDatePicker.TabIndex = 17;
            this.toolTip1.SetToolTip(this.designDatePicker, "Date the orignal design was approved");
            this.designDatePicker.Value = new System.DateTime(1960, 1, 1, 0, 0, 0, 0);
            // 
            // label15
            // 
            this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(11, 425);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(37, 13);
            this.label15.TabIndex = 26;
            this.label15.Text = "Detail:";
            this.toolTip1.SetToolTip(this.label15, "Initials of person approving the design detail");
            // 
            // detailerTextBox
            // 
            this.detailerTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.detailerTextBox.Location = new System.Drawing.Point(55, 422);
            this.detailerTextBox.MaxLength = 8;
            this.detailerTextBox.Name = "detailerTextBox";
            this.detailerTextBox.Size = new System.Drawing.Size(58, 20);
            this.detailerTextBox.TabIndex = 18;
            this.toolTip1.SetToolTip(this.detailerTextBox, "Initials of person approving the design detail");
            // 
            // label16
            // 
            this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(122, 425);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(33, 13);
            this.label16.TabIndex = 28;
            this.label16.Text = "Date:";
            this.toolTip1.SetToolTip(this.label16, "Date the detail was approved");
            // 
            // detailDatePicker
            // 
            this.detailDatePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.detailDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.detailDatePicker.Location = new System.Drawing.Point(156, 422);
            this.detailDatePicker.Name = "detailDatePicker";
            this.detailDatePicker.Size = new System.Drawing.Size(102, 20);
            this.detailDatePicker.TabIndex = 19;
            this.toolTip1.SetToolTip(this.detailDatePicker, "Date the detail was approved");
            this.detailDatePicker.Value = new System.DateTime(1960, 1, 1, 0, 0, 0, 0);
            // 
            // label17
            // 
            this.label17.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(12, 456);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(41, 13);
            this.label17.TabIndex = 29;
            this.label17.Text = "Check:";
            this.toolTip1.SetToolTip(this.label17, "Initials of person checking the design detail");
            // 
            // designCheckerTextBox
            // 
            this.designCheckerTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.designCheckerTextBox.Location = new System.Drawing.Point(55, 453);
            this.designCheckerTextBox.MaxLength = 8;
            this.designCheckerTextBox.Name = "designCheckerTextBox";
            this.designCheckerTextBox.Size = new System.Drawing.Size(58, 20);
            this.designCheckerTextBox.TabIndex = 20;
            this.toolTip1.SetToolTip(this.designCheckerTextBox, "Initials of person checking the design detail");
            // 
            // designCheckDatePicker
            // 
            this.designCheckDatePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.designCheckDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.designCheckDatePicker.Location = new System.Drawing.Point(156, 453);
            this.designCheckDatePicker.Name = "designCheckDatePicker";
            this.designCheckDatePicker.Size = new System.Drawing.Size(102, 20);
            this.designCheckDatePicker.TabIndex = 21;
            this.toolTip1.SetToolTip(this.designCheckDatePicker, "Date the design was checked");
            this.designCheckDatePicker.Value = new System.DateTime(1960, 1, 1, 0, 0, 0, 0);
            // 
            // label18
            // 
            this.label18.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(122, 456);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(33, 13);
            this.label18.TabIndex = 31;
            this.label18.Text = "Date:";
            this.toolTip1.SetToolTip(this.label18, "Date the design was checked");
            // 
            // approvalDatePicker
            // 
            this.approvalDatePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.approvalDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.approvalDatePicker.Location = new System.Drawing.Point(156, 484);
            this.approvalDatePicker.Name = "approvalDatePicker";
            this.approvalDatePicker.Size = new System.Drawing.Size(102, 20);
            this.approvalDatePicker.TabIndex = 23;
            this.toolTip1.SetToolTip(this.approvalDatePicker, "Date this design was approved");
            this.approvalDatePicker.Value = new System.DateTime(1960, 1, 1, 0, 0, 0, 0);
            // 
            // label19
            // 
            this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(122, 487);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(33, 13);
            this.label19.TabIndex = 35;
            this.label19.Text = "Date:";
            this.toolTip1.SetToolTip(this.label19, "Date this design was approved");
            // 
            // approverTextBox
            // 
            this.approverTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.approverTextBox.Location = new System.Drawing.Point(55, 484);
            this.approverTextBox.MaxLength = 8;
            this.approverTextBox.Name = "approverTextBox";
            this.approverTextBox.Size = new System.Drawing.Size(58, 20);
            this.approverTextBox.TabIndex = 22;
            this.toolTip1.SetToolTip(this.approverTextBox, "Initials of person approving this design");
            // 
            // label20
            // 
            this.label20.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(12, 487);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(38, 13);
            this.label20.TabIndex = 33;
            this.label20.Text = "Appro:";
            this.toolTip1.SetToolTip(this.label20, "Initials of person approving this design");
            // 
            // label21
            // 
            this.label21.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(268, 393);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(34, 13);
            this.label21.TabIndex = 36;
            this.label21.Text = "Type:";
            this.toolTip1.SetToolTip(this.label21, "The general card family, almost always SMS");
            // 
            // modelTypeTextBox
            // 
            this.modelTypeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.modelTypeTextBox.Location = new System.Drawing.Point(306, 391);
            this.modelTypeTextBox.MaxLength = 8;
            this.modelTypeTextBox.Name = "modelTypeTextBox";
            this.modelTypeTextBox.Size = new System.Drawing.Size(40, 20);
            this.modelTypeTextBox.TabIndex = 24;
            this.modelTypeTextBox.Text = "SMS";
            this.toolTip1.SetToolTip(this.modelTypeTextBox, "The general card family, almost always SMS");
            // 
            // label22
            // 
            this.label22.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(350, 394);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(44, 13);
            this.label22.TabIndex = 38;
            this.label22.Text = "Device:";
            this.toolTip1.SetToolTip(this.label22, "The device that this card was originally designed for.  E.g.  1405");
            // 
            // modelDeviceTextBox
            // 
            this.modelDeviceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.modelDeviceTextBox.Location = new System.Drawing.Point(398, 391);
            this.modelDeviceTextBox.MaxLength = 8;
            this.modelDeviceTextBox.Name = "modelDeviceTextBox";
            this.modelDeviceTextBox.Size = new System.Drawing.Size(52, 20);
            this.modelDeviceTextBox.TabIndex = 25;
            this.toolTip1.SetToolTip(this.modelDeviceTextBox, "The device that this card was originally designed for.  E.g.  1405");
            // 
            // label23
            // 
            this.label23.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(268, 425);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(40, 13);
            this.label23.TabIndex = 40;
            this.label23.Text = "Scale: ";
            this.toolTip1.SetToolTip(this.label23, "The scale of this SMS card drawing - usually NONE");
            // 
            // scaleTextBox
            // 
            this.scaleTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.scaleTextBox.Location = new System.Drawing.Point(306, 422);
            this.scaleTextBox.Name = "scaleTextBox";
            this.scaleTextBox.Size = new System.Drawing.Size(58, 20);
            this.scaleTextBox.TabIndex = 26;
            this.scaleTextBox.Text = "NONE";
            this.toolTip1.SetToolTip(this.scaleTextBox, "The scale of this SMS card drawing - usually NONE");
            // 
            // label24
            // 
            this.label24.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(267, 456);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(35, 13);
            this.label24.TabIndex = 41;
            this.label24.Text = "Draw:";
            this.toolTip1.SetToolTip(this.label24, "Initials of the person who approved the original drawing");
            // 
            // drawTextBox
            // 
            this.drawTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.drawTextBox.Location = new System.Drawing.Point(306, 453);
            this.drawTextBox.MaxLength = 8;
            this.drawTextBox.Name = "drawTextBox";
            this.drawTextBox.Size = new System.Drawing.Size(58, 20);
            this.drawTextBox.TabIndex = 27;
            this.toolTip1.SetToolTip(this.drawTextBox, "Initials of the person who approved the original drawing");
            // 
            // drawDatePicker
            // 
            this.drawDatePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.drawDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.drawDatePicker.Location = new System.Drawing.Point(370, 453);
            this.drawDatePicker.Name = "drawDatePicker";
            this.drawDatePicker.Size = new System.Drawing.Size(102, 20);
            this.drawDatePicker.TabIndex = 28;
            this.toolTip1.SetToolTip(this.drawDatePicker, "Date the original drawing for this card was approved");
            this.drawDatePicker.Value = new System.DateTime(1960, 1, 1, 0, 0, 0, 0);
            // 
            // label25
            // 
            this.label25.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(268, 487);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(41, 13);
            this.label25.TabIndex = 42;
            this.label25.Text = "Check:";
            this.toolTip1.SetToolTip(this.label25, "Initials of person who checked the original drawing");
            // 
            // drawingCheckerTextBox
            // 
            this.drawingCheckerTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.drawingCheckerTextBox.Location = new System.Drawing.Point(306, 484);
            this.drawingCheckerTextBox.MaxLength = 8;
            this.drawingCheckerTextBox.Name = "drawingCheckerTextBox";
            this.drawingCheckerTextBox.Size = new System.Drawing.Size(58, 20);
            this.drawingCheckerTextBox.TabIndex = 29;
            this.toolTip1.SetToolTip(this.drawingCheckerTextBox, "Initials of person who checked the original drawing");
            // 
            // drawingCheckDatePicker
            // 
            this.drawingCheckDatePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.drawingCheckDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.drawingCheckDatePicker.Location = new System.Drawing.Point(369, 484);
            this.drawingCheckDatePicker.Name = "drawingCheckDatePicker";
            this.drawingCheckDatePicker.Size = new System.Drawing.Size(102, 20);
            this.drawingCheckDatePicker.TabIndex = 30;
            this.toolTip1.SetToolTip(this.drawingCheckDatePicker, "Date this drawing was originally checked");
            this.drawingCheckDatePicker.Value = new System.DateTime(1960, 1, 1, 0, 0, 0, 0);
            // 
            // addApplyButton
            // 
            this.addApplyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addApplyButton.Location = new System.Drawing.Point(55, 516);
            this.addApplyButton.Name = "addApplyButton";
            this.addApplyButton.Size = new System.Drawing.Size(75, 23);
            this.addApplyButton.TabIndex = 45;
            this.addApplyButton.Text = "TBA";
            this.toolTip1.SetToolTip(this.addApplyButton, "Click to Add or Update an SMS card type entry");
            this.addApplyButton.UseVisualStyleBackColor = true;
            this.addApplyButton.Click += new System.EventHandler(this.addApplyButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelButton.Location = new System.Drawing.Point(337, 516);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 47;
            this.cancelButton.Text = "Cancel";
            this.toolTip1.SetToolTip(this.cancelButton, "Click to cancel this add / edit");
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // updatedVolumeComboBox
            // 
            this.updatedVolumeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.updatedVolumeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.updatedVolumeComboBox.FormattingEnabled = true;
            this.updatedVolumeComboBox.Location = new System.Drawing.Point(636, 194);
            this.updatedVolumeComboBox.Name = "updatedVolumeComboBox";
            this.updatedVolumeComboBox.Size = new System.Drawing.Size(220, 21);
            this.updatedVolumeComboBox.TabIndex = 8;
            this.toolTip1.SetToolTip(this.updatedVolumeComboBox, "Volume in Volume Set that this card diagram appears in");
            // 
            // cardNotesDataGridView
            // 
            this.cardNotesDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cardNotesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.cardNotesDataGridView.Location = new System.Drawing.Point(15, 125);
            this.cardNotesDataGridView.Name = "cardNotesDataGridView";
            this.cardNotesDataGridView.Size = new System.Drawing.Size(419, 172);
            this.cardNotesDataGridView.TabIndex = 10;
            this.cardNotesDataGridView.TabStop = false;
            this.cardNotesDataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.cardNotesDataGridView_CellValidating);
            this.cardNotesDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.cardNotesDataGridView_CellValueChanged);
            this.cardNotesDataGridView.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.cardNotesDataGridView_UserDeletingRow);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(209, 109);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "NOTES";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.singleRadioButton);
            this.groupBox2.Controls.Add(this.doubleRadioButton);
            this.groupBox2.Location = new System.Drawing.Point(464, 250);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(158, 47);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Card Height";
            // 
            // label26
            // 
            this.label26.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(663, 315);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(38, 13);
            this.label26.TabIndex = 43;
            this.label26.Text = "ECOs";
            // 
            // cardECOsDataGridView
            // 
            this.cardECOsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cardECOsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.cardECOsDataGridView.Location = new System.Drawing.Point(489, 331);
            this.cardECOsDataGridView.Name = "cardECOsDataGridView";
            this.cardECOsDataGridView.Size = new System.Drawing.Size(367, 173);
            this.cardECOsDataGridView.TabIndex = 44;
            this.cardECOsDataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.cardECOsDataGridView_CellValidating);
            this.cardECOsDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.cardECOsDataGridView_CellValueChanged);
            this.cardECOsDataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.cardECOsDataGridView_DataError);
            this.cardECOsDataGridView.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.cardECOsDataGridView_UserDeletingRow);
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.deleteButton.Location = new System.Drawing.Point(196, 516);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 23);
            this.deleteButton.TabIndex = 46;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // label27
            // 
            this.label27.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(461, 197);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(143, 13);
            this.label27.TabIndex = 49;
            this.label27.Text = "CHANGED Diagram Volume:";
            // 
            // EditCardTypesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(907, 551);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.updatedVolumeComboBox);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.addApplyButton);
            this.Controls.Add(this.cardECOsDataGridView);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.drawingCheckDatePicker);
            this.Controls.Add(this.drawingCheckerTextBox);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.drawDatePicker);
            this.Controls.Add(this.drawTextBox);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.scaleTextBox);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.modelDeviceTextBox);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.modelTypeTextBox);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.approvalDatePicker);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.approverTextBox);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.designCheckDatePicker);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.designCheckerTextBox);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.detailDatePicker);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.detailerTextBox);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.designDatePicker);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.designApproverTextBox);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.nameTypeTextBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.developmentNumberTextBox);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.holePatternTextBox);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.approvedDatePicker);
            this.Controls.Add(this.approvedByTextBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.logicFamilyComboBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cardNotesDataGridView);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.newCardTypeButton);
            this.Controls.Add(this.volumeComboBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.typeTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.partTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cardTypeComboBox);
            this.Controls.Add(this.label1);
            this.Name = "EditCardTypesForm";
            this.Text = "Edit SMS Card Type";
            ((System.ComponentModel.ISupportInitialize)(this.cardNotesDataGridView)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cardECOsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cardTypeComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox partTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox typeTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox volumeComboBox;
        private System.Windows.Forms.Button newCardTypeButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.DataGridView cardNotesDataGridView;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox logicFamilyComboBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox approvedByTextBox;
        private System.Windows.Forms.DateTimePicker approvedDatePicker;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox holePatternTextBox;
        private System.Windows.Forms.RadioButton singleRadioButton;
        private System.Windows.Forms.RadioButton doubleRadioButton;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox developmentNumberTextBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox nameTypeTextBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox designApproverTextBox;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.DateTimePicker designDatePicker;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox detailerTextBox;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.DateTimePicker detailDatePicker;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox designCheckerTextBox;
        private System.Windows.Forms.DateTimePicker designCheckDatePicker;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.DateTimePicker approvalDatePicker;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox approverTextBox;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox modelTypeTextBox;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox modelDeviceTextBox;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox scaleTextBox;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox drawTextBox;
        private System.Windows.Forms.DateTimePicker drawDatePicker;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox drawingCheckerTextBox;
        private System.Windows.Forms.DateTimePicker drawingCheckDatePicker;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.DataGridView cardECOsDataGridView;
        private System.Windows.Forms.Button addApplyButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.ComboBox updatedVolumeComboBox;
    }
}