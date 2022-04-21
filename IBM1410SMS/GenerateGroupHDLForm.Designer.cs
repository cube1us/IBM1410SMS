namespace IBM1410SMS
{
    partial class GenerateGroupHDLForm
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
            this.selectFolderButton = new System.Windows.Forms.Button();
            this.directoryTextBox = new System.Windows.Forms.TextBox();
            this.pagesTextBox = new System.Windows.Forms.TextBox();
            this.volumeSetComboBox = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.volumeComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.machineComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.outputFileNameTextBox = new System.Windows.Forms.TextBox();
            this.generateButton = new System.Windows.Forms.Button();
            this.generateTestBenchCheckBox = new System.Windows.Forms.CheckBox();
            this.generateLampsSwitches = new System.Windows.Forms.CheckBox();
            this.workingLabel = new System.Windows.Forms.Label();
            this.generateCSharpIndicesCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // selectFolderButton
            // 
            this.selectFolderButton.Location = new System.Drawing.Point(23, 107);
            this.selectFolderButton.Name = "selectFolderButton";
            this.selectFolderButton.Size = new System.Drawing.Size(100, 23);
            this.selectFolderButton.TabIndex = 30;
            this.selectFolderButton.Text = "Output Directory";
            this.toolTip1.SetToolTip(this.selectFolderButton, "Identify the output directory for the HDL file output");
            this.selectFolderButton.UseVisualStyleBackColor = true;
            this.selectFolderButton.Click += new System.EventHandler(this.selectFolderButton_Click);
            // 
            // directoryTextBox
            // 
            this.directoryTextBox.Location = new System.Drawing.Point(140, 109);
            this.directoryTextBox.Name = "directoryTextBox";
            this.directoryTextBox.ReadOnly = true;
            this.directoryTextBox.Size = new System.Drawing.Size(482, 20);
            this.directoryTextBox.TabIndex = 32;
            this.toolTip1.SetToolTip(this.directoryTextBox, "Specify the directory for the output");
            // 
            // pagesTextBox
            // 
            this.pagesTextBox.Location = new System.Drawing.Point(140, 155);
            this.pagesTextBox.Name = "pagesTextBox";
            this.pagesTextBox.Size = new System.Drawing.Size(482, 20);
            this.pagesTextBox.TabIndex = 40;
            this.toolTip1.SetToolTip(this.pagesTextBox, "Identify the pages (SQL LIKE Syntax) to generate");
            this.pagesTextBox.TextChanged += new System.EventHandler(this.pagesTextBox_TextChanged);
            // 
            // volumeSetComboBox
            // 
            this.volumeSetComboBox.DisplayMember = "machinetype";
            this.volumeSetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.volumeSetComboBox.FormattingEnabled = true;
            this.volumeSetComboBox.Location = new System.Drawing.Point(418, 21);
            this.volumeSetComboBox.Name = "volumeSetComboBox";
            this.volumeSetComboBox.Size = new System.Drawing.Size(180, 21);
            this.volumeSetComboBox.TabIndex = 11;
            this.toolTip1.SetToolTip(this.volumeSetComboBox, "Select machine, volume set and volume to use");
            this.volumeSetComboBox.SelectedIndexChanged += new System.EventHandler(this.volumeSetComboBox_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(332, 24);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(64, 13);
            this.label13.TabIndex = 143;
            this.label13.Text = "Volume Set:";
            this.toolTip1.SetToolTip(this.label13, "Select machine, volume set and volume to use");
            // 
            // volumeComboBox
            // 
            this.volumeComboBox.DisplayMember = "name";
            this.volumeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.volumeComboBox.FormattingEnabled = true;
            this.volumeComboBox.Location = new System.Drawing.Point(140, 64);
            this.volumeComboBox.Name = "volumeComboBox";
            this.volumeComboBox.Size = new System.Drawing.Size(66, 21);
            this.volumeComboBox.TabIndex = 20;
            this.toolTip1.SetToolTip(this.volumeComboBox, "Select machine, volume set and volume to use");
            this.volumeComboBox.SelectedIndexChanged += new System.EventHandler(this.volumeComboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 142;
            this.label3.Text = "Select Volume:";
            this.toolTip1.SetToolTip(this.label3, "Select machine, volume set and volume to use");
            // 
            // machineComboBox
            // 
            this.machineComboBox.DisplayMember = "name";
            this.machineComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.machineComboBox.FormattingEnabled = true;
            this.machineComboBox.Location = new System.Drawing.Point(140, 21);
            this.machineComboBox.Name = "machineComboBox";
            this.machineComboBox.Size = new System.Drawing.Size(121, 21);
            this.machineComboBox.TabIndex = 10;
            this.toolTip1.SetToolTip(this.machineComboBox, "Select machine, volume set and volume to use");
            this.machineComboBox.SelectedIndexChanged += new System.EventHandler(this.machineComboBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 141;
            this.label2.Text = "Select machine:";
            this.toolTip1.SetToolTip(this.label2, "Select machine, volume set and volume to use");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 155);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 140;
            this.label1.Text = "Page(s) to generate:";
            this.toolTip1.SetToolTip(this.label1, "Identify the pages (SQL LIKE Syntax) to generate");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 195);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 148;
            this.label4.Text = "Output File";
            this.toolTip1.SetToolTip(this.label4, "The name of the output file containing the group HDL instantiation");
            // 
            // outputFileNameTextBox
            // 
            this.outputFileNameTextBox.Location = new System.Drawing.Point(140, 192);
            this.outputFileNameTextBox.Name = "outputFileNameTextBox";
            this.outputFileNameTextBox.Size = new System.Drawing.Size(135, 20);
            this.outputFileNameTextBox.TabIndex = 50;
            this.toolTip1.SetToolTip(this.outputFileNameTextBox, "The name of the output file containing the group HDL instantiation");
            this.outputFileNameTextBox.TextChanged += new System.EventHandler(this.outputFileName_TextChanged);
            // 
            // generateButton
            // 
            this.generateButton.Enabled = false;
            this.generateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.generateButton.Location = new System.Drawing.Point(237, 305);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(159, 23);
            this.generateButton.TabIndex = 147;
            this.generateButton.Text = "Generate VHDL Group";
            this.generateButton.UseVisualStyleBackColor = true;
            this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
            // 
            // generateTestBenchCheckBox
            // 
            this.generateTestBenchCheckBox.AutoSize = true;
            this.generateTestBenchCheckBox.Location = new System.Drawing.Point(385, 194);
            this.generateTestBenchCheckBox.Name = "generateTestBenchCheckBox";
            this.generateTestBenchCheckBox.Size = new System.Drawing.Size(174, 17);
            this.generateTestBenchCheckBox.TabIndex = 149;
            this.generateTestBenchCheckBox.Text = "Generate / Update Test Bench";
            this.generateTestBenchCheckBox.UseVisualStyleBackColor = true;
            // 
            // generateLampsSwitches
            // 
            this.generateLampsSwitches.AutoSize = true;
            this.generateLampsSwitches.Location = new System.Drawing.Point(385, 228);
            this.generateLampsSwitches.Name = "generateLampsSwitches";
            this.generateLampsSwitches.Size = new System.Drawing.Size(244, 17);
            this.generateLampsSwitches.TabIndex = 150;
            this.generateLampsSwitches.Text = "Generate Lamp/Switch Vectors in Test Bench";
            this.generateLampsSwitches.UseVisualStyleBackColor = true;
            // 
            // workingLabel
            // 
            this.workingLabel.AutoSize = true;
            this.workingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.workingLabel.ForeColor = System.Drawing.Color.Crimson;
            this.workingLabel.Location = new System.Drawing.Point(464, 72);
            this.workingLabel.Name = "workingLabel";
            this.workingLabel.Size = new System.Drawing.Size(79, 16);
            this.workingLabel.TabIndex = 151;
            this.workingLabel.Text = "WORKING";
            this.workingLabel.Visible = false;
            // 
            // generateCSharpIndicesCheckBox
            // 
            this.generateCSharpIndicesCheckBox.AutoSize = true;
            this.generateCSharpIndicesCheckBox.Location = new System.Drawing.Point(385, 262);
            this.generateCSharpIndicesCheckBox.Name = "generateCSharpIndicesCheckBox";
            this.generateCSharpIndicesCheckBox.Size = new System.Drawing.Size(224, 17);
            this.generateCSharpIndicesCheckBox.TabIndex = 152;
            this.generateCSharpIndicesCheckBox.Text = "Generate C# Lamp/Switch Vector Indices";
            this.generateCSharpIndicesCheckBox.UseVisualStyleBackColor = true;
            // 
            // GenerateGroupHDLForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(653, 354);
            this.Controls.Add(this.generateCSharpIndicesCheckBox);
            this.Controls.Add(this.workingLabel);
            this.Controls.Add(this.generateLampsSwitches);
            this.Controls.Add(this.generateTestBenchCheckBox);
            this.Controls.Add(this.outputFileNameTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.generateButton);
            this.Controls.Add(this.selectFolderButton);
            this.Controls.Add(this.directoryTextBox);
            this.Controls.Add(this.pagesTextBox);
            this.Controls.Add(this.volumeSetComboBox);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.volumeComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.machineComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "GenerateGroupHDLForm";
            this.Text = "Generate HDL Page Group";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button generateButton;
        private System.Windows.Forms.Button selectFolderButton;
        private System.Windows.Forms.TextBox directoryTextBox;
        private System.Windows.Forms.TextBox pagesTextBox;
        private System.Windows.Forms.ComboBox volumeSetComboBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox volumeComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox machineComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox outputFileNameTextBox;
        private System.Windows.Forms.CheckBox generateTestBenchCheckBox;
        private System.Windows.Forms.CheckBox generateLampsSwitches;
        private System.Windows.Forms.Label workingLabel;
        private System.Windows.Forms.CheckBox generateCSharpIndicesCheckBox;
    }
}