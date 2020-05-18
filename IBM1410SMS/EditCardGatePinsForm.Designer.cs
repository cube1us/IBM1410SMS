namespace IBM1410SMS
{
    partial class EditCardGatePinsForm
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
            this.cardTypeComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.volumeComboBox = new System.Windows.Forms.ComboBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.cardGatesComboBox = new System.Windows.Forms.ComboBox();
            this.pinsDataGridView = new System.Windows.Forms.DataGridView();
            this.applyButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.logicFunctionTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pinsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // cardTypeComboBox
            // 
            this.cardTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cardTypeComboBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cardTypeComboBox.FormattingEnabled = true;
            this.cardTypeComboBox.Location = new System.Drawing.Point(96, 57);
            this.cardTypeComboBox.Name = "cardTypeComboBox";
            this.cardTypeComboBox.Size = new System.Drawing.Size(265, 22);
            this.cardTypeComboBox.TabIndex = 2;
            this.toolTip1.SetToolTip(this.cardTypeComboBox, "Select the SMS volume describing the desired card, then select the card and gate " +
        "to edit within that volume");
            this.cardTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.cardTypeComboBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Card Type:";
            this.toolTip1.SetToolTip(this.label2, "Select the SMS volume describing the desired card, then select the card and gate " +
        "to edit within that volume");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Volume: ";
            this.toolTip1.SetToolTip(this.label1, "Select the SMS volume describing the desired card, then select the card and gate " +
        "to edit within that volume");
            // 
            // volumeComboBox
            // 
            this.volumeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.volumeComboBox.FormattingEnabled = true;
            this.volumeComboBox.Location = new System.Drawing.Point(96, 24);
            this.volumeComboBox.Name = "volumeComboBox";
            this.volumeComboBox.Size = new System.Drawing.Size(265, 21);
            this.volumeComboBox.TabIndex = 1;
            this.toolTip1.SetToolTip(this.volumeComboBox, "Select the SMS volume describing the desired card, then select the card and gate " +
        "to edit within that volume");
            this.volumeComboBox.SelectedIndexChanged += new System.EventHandler(this.volumeComboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Gate:";
            this.toolTip1.SetToolTip(this.label3, "Select the SMS volume describing the desired card, then select the card and gate " +
        "to edit within that volume");
            // 
            // cardGatesComboBox
            // 
            this.cardGatesComboBox.DisplayMember = "number";
            this.cardGatesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cardGatesComboBox.FormattingEnabled = true;
            this.cardGatesComboBox.Location = new System.Drawing.Point(96, 90);
            this.cardGatesComboBox.Name = "cardGatesComboBox";
            this.cardGatesComboBox.Size = new System.Drawing.Size(59, 21);
            this.cardGatesComboBox.TabIndex = 3;
            this.toolTip1.SetToolTip(this.cardGatesComboBox, "Select the SMS volume describing the desired card, then select the card and gate " +
        "to edit within that volume");
            this.cardGatesComboBox.SelectedIndexChanged += new System.EventHandler(this.cardGatesComboBox_SelectedIndexChanged);
            // 
            // pinsDataGridView
            // 
            this.pinsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pinsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.pinsDataGridView.Location = new System.Drawing.Point(12, 147);
            this.pinsDataGridView.Name = "pinsDataGridView";
            this.pinsDataGridView.Size = new System.Drawing.Size(492, 229);
            this.pinsDataGridView.TabIndex = 9;
            this.pinsDataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.pinsDataGridView_CellValidating);
            this.pinsDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.pinsDataGridView_CellValueChanged);
            this.pinsDataGridView.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.pinsDataGridView_UserDeletingRow);
            // 
            // applyButton
            // 
            this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.applyButton.Location = new System.Drawing.Point(12, 390);
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
            this.cancelButton.Location = new System.Drawing.Point(369, 390);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 11;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(200, 127);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 17);
            this.label4.TabIndex = 12;
            this.label4.Text = "Pins";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(213, 93);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Logic Function: ";
            // 
            // logicFunctionTextBox
            // 
            this.logicFunctionTextBox.Location = new System.Drawing.Point(302, 90);
            this.logicFunctionTextBox.Name = "logicFunctionTextBox";
            this.logicFunctionTextBox.ReadOnly = true;
            this.logicFunctionTextBox.Size = new System.Drawing.Size(100, 20);
            this.logicFunctionTextBox.TabIndex = 14;
            // 
            // EditCardGatePinsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 425);
            this.Controls.Add(this.logicFunctionTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.pinsDataGridView);
            this.Controls.Add(this.cardGatesComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cardTypeComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.volumeComboBox);
            this.Name = "EditCardGatePinsForm";
            this.Text = "Edit Card Gate Pins";
            ((System.ComponentModel.ISupportInitialize)(this.pinsDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cardTypeComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox volumeComboBox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cardGatesComboBox;
        private System.Windows.Forms.DataGridView pinsDataGridView;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox logicFunctionTextBox;
    }
}