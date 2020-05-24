namespace IBM1410SMS
{
    partial class GenerateHDLForm
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
            this.volumeSetComboBox = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.volumeComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.machineComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pagesTextBox = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.directoryTextBox = new System.Windows.Forms.TextBox();
            this.selectFolderButton = new System.Windows.Forms.Button();
            this.generateButton = new System.Windows.Forms.Button();
            this.testBenchCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // volumeSetComboBox
            // 
            this.volumeSetComboBox.DisplayMember = "machinetype";
            this.volumeSetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.volumeSetComboBox.FormattingEnabled = true;
            this.volumeSetComboBox.Location = new System.Drawing.Point(410, 16);
            this.volumeSetComboBox.Name = "volumeSetComboBox";
            this.volumeSetComboBox.Size = new System.Drawing.Size(180, 21);
            this.volumeSetComboBox.TabIndex = 124;
            this.toolTip1.SetToolTip(this.volumeSetComboBox, "Select machine, volume set and volume to use");
            this.volumeSetComboBox.SelectedIndexChanged += new System.EventHandler(this.volumeSetComboBox_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(324, 19);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(64, 13);
            this.label13.TabIndex = 131;
            this.label13.Text = "Volume Set:";
            this.toolTip1.SetToolTip(this.label13, "Select machine, volume set and volume to use");
            // 
            // volumeComboBox
            // 
            this.volumeComboBox.DisplayMember = "name";
            this.volumeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.volumeComboBox.FormattingEnabled = true;
            this.volumeComboBox.Location = new System.Drawing.Point(132, 59);
            this.volumeComboBox.Name = "volumeComboBox";
            this.volumeComboBox.Size = new System.Drawing.Size(66, 21);
            this.volumeComboBox.TabIndex = 125;
            this.toolTip1.SetToolTip(this.volumeComboBox, "Select machine, volume set and volume to use");
            this.volumeComboBox.SelectedIndexChanged += new System.EventHandler(this.volumeComboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 130;
            this.label3.Text = "Select Volume:";
            this.toolTip1.SetToolTip(this.label3, "Select machine, volume set and volume to use");
            // 
            // machineComboBox
            // 
            this.machineComboBox.DisplayMember = "name";
            this.machineComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.machineComboBox.FormattingEnabled = true;
            this.machineComboBox.Location = new System.Drawing.Point(132, 16);
            this.machineComboBox.Name = "machineComboBox";
            this.machineComboBox.Size = new System.Drawing.Size(121, 21);
            this.machineComboBox.TabIndex = 123;
            this.toolTip1.SetToolTip(this.machineComboBox, "Select machine, volume set and volume to use");
            this.machineComboBox.SelectedIndexChanged += new System.EventHandler(this.machineComboBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 129;
            this.label2.Text = "Select machine:";
            this.toolTip1.SetToolTip(this.label2, "Select machine, volume set and volume to use");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 150);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 128;
            this.label1.Text = "Page(s) to generate:";
            this.toolTip1.SetToolTip(this.label1, "Identify the pages (SQL LIKE Syntax) to generate");
            // 
            // pagesTextBox
            // 
            this.pagesTextBox.Location = new System.Drawing.Point(132, 150);
            this.pagesTextBox.Name = "pagesTextBox";
            this.pagesTextBox.Size = new System.Drawing.Size(482, 20);
            this.pagesTextBox.TabIndex = 132;
            this.toolTip1.SetToolTip(this.pagesTextBox, "Identify the pages (SQL LIKE Syntax) to generate");
            this.pagesTextBox.TextChanged += new System.EventHandler(this.pagesTextBox_TextChanged);
            // 
            // directoryTextBox
            // 
            this.directoryTextBox.Location = new System.Drawing.Point(132, 104);
            this.directoryTextBox.Name = "directoryTextBox";
            this.directoryTextBox.ReadOnly = true;
            this.directoryTextBox.Size = new System.Drawing.Size(482, 20);
            this.directoryTextBox.TabIndex = 134;
            this.toolTip1.SetToolTip(this.directoryTextBox, "Specify the directory for the output");
            // 
            // selectFolderButton
            // 
            this.selectFolderButton.Location = new System.Drawing.Point(15, 102);
            this.selectFolderButton.Name = "selectFolderButton";
            this.selectFolderButton.Size = new System.Drawing.Size(100, 23);
            this.selectFolderButton.TabIndex = 135;
            this.selectFolderButton.Text = "Output Directory";
            this.toolTip1.SetToolTip(this.selectFolderButton, "Identify the output directory for the HDL file output");
            this.selectFolderButton.UseVisualStyleBackColor = true;
            this.selectFolderButton.Click += new System.EventHandler(this.selectFolderButton_Click);
            // 
            // generateButton
            // 
            this.generateButton.Enabled = false;
            this.generateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.generateButton.Location = new System.Drawing.Point(284, 211);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(114, 23);
            this.generateButton.TabIndex = 136;
            this.generateButton.Text = "Generate VHDL";
            this.generateButton.UseVisualStyleBackColor = true;
            this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
            // 
            // testBenchCheckBox
            // 
            this.testBenchCheckBox.AutoSize = true;
            this.testBenchCheckBox.Location = new System.Drawing.Point(327, 63);
            this.testBenchCheckBox.Name = "testBenchCheckBox";
            this.testBenchCheckBox.Size = new System.Drawing.Size(174, 17);
            this.testBenchCheckBox.TabIndex = 137;
            this.testBenchCheckBox.Text = "Generate / Update Test Bench";
            this.toolTip1.SetToolTip(this.testBenchCheckBox, "Select to generate or update a test bench for page(s) specified");
            this.testBenchCheckBox.UseVisualStyleBackColor = true;
            // 
            // GenerateHDLForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 263);
            this.Controls.Add(this.testBenchCheckBox);
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
            this.Name = "GenerateHDLForm";
            this.Text = "Generate HDL";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox volumeSetComboBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox volumeComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox machineComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox pagesTextBox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox directoryTextBox;
        private System.Windows.Forms.Button selectFolderButton;
        private System.Windows.Forms.Button generateButton;
        private System.Windows.Forms.CheckBox testBenchCheckBox;
    }
}