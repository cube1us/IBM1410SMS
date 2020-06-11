namespace IBM1410SMS
{
    partial class ReportCardTypeUsageForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
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
        private void InitializeComponent()
        {
            this.cardTypeComboBox = new System.Windows.Forms.ComboBox();
            this.cardTypeUsageDataGridView = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cardGateComboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.symbolTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.polarityComboBox = new System.Windows.Forms.ComboBox();
            this.reportButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.cardTypeUsageDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // cardTypeComboBox
            // 
            this.cardTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cardTypeComboBox.FormattingEnabled = true;
            this.cardTypeComboBox.Location = new System.Drawing.Point(148, 31);
            this.cardTypeComboBox.Name = "cardTypeComboBox";
            this.cardTypeComboBox.Size = new System.Drawing.Size(121, 21);
            this.cardTypeComboBox.TabIndex = 0;
            this.cardTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.cardTypeComboBox_SelectedIndexChanged);
            // 
            // cardTypeUsageDataGridView
            // 
            this.cardTypeUsageDataGridView.AllowUserToAddRows = false;
            this.cardTypeUsageDataGridView.AllowUserToDeleteRows = false;
            this.cardTypeUsageDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cardTypeUsageDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.cardTypeUsageDataGridView.Location = new System.Drawing.Point(45, 141);
            this.cardTypeUsageDataGridView.Name = "cardTypeUsageDataGridView";
            this.cardTypeUsageDataGridView.ReadOnly = true;
            this.cardTypeUsageDataGridView.Size = new System.Drawing.Size(513, 300);
            this.cardTypeUsageDataGridView.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Card Type: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(236, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Card Type Usage Report";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(304, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "(Opt.) Card Gate:";
            // 
            // cardGateComboBox
            // 
            this.cardGateComboBox.DisplayMember = "number";
            this.cardGateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cardGateComboBox.FormattingEnabled = true;
            this.cardGateComboBox.Location = new System.Drawing.Point(437, 31);
            this.cardGateComboBox.Name = "cardGateComboBox";
            this.cardGateComboBox.Size = new System.Drawing.Size(44, 21);
            this.cardGateComboBox.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(42, 81);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(132, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "(Opt.) Logic Block Symbol:";
            // 
            // symbolTextBox
            // 
            this.symbolTextBox.Location = new System.Drawing.Point(180, 78);
            this.symbolTextBox.Name = "symbolTextBox";
            this.symbolTextBox.Size = new System.Drawing.Size(89, 20);
            this.symbolTextBox.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(304, 81);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(111, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "(Opt.) Output Polarity: ";
            // 
            // polarityComboBox
            // 
            this.polarityComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.polarityComboBox.FormattingEnabled = true;
            this.polarityComboBox.Location = new System.Drawing.Point(437, 78);
            this.polarityComboBox.Name = "polarityComboBox";
            this.polarityComboBox.Size = new System.Drawing.Size(44, 21);
            this.polarityComboBox.TabIndex = 9;
            // 
            // reportButton
            // 
            this.reportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.reportButton.Location = new System.Drawing.Point(265, 472);
            this.reportButton.Name = "reportButton";
            this.reportButton.Size = new System.Drawing.Size(75, 23);
            this.reportButton.TabIndex = 10;
            this.reportButton.Text = "Report";
            this.reportButton.UseVisualStyleBackColor = true;
            this.reportButton.Click += new System.EventHandler(this.reportButton_Click);
            // 
            // ReportCardTypeUsageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 521);
            this.Controls.Add(this.reportButton);
            this.Controls.Add(this.polarityComboBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.symbolTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cardGateComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cardTypeUsageDataGridView);
            this.Controls.Add(this.cardTypeComboBox);
            this.Name = "ReportCardTypeUsageForm";
            this.Text = "ReportCardTypeUsage";
            ((System.ComponentModel.ISupportInitialize)(this.cardTypeUsageDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cardTypeComboBox;
        private System.Windows.Forms.DataGridView cardTypeUsageDataGridView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cardGateComboBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox symbolTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox polarityComboBox;
        private System.Windows.Forms.Button reportButton;
    }
}