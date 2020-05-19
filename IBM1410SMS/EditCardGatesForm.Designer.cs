namespace IBM1410SMS
{
    partial class EditCardGatesForm
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
            this.volumeComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.cardTypeComboBox = new System.Windows.Forms.ComboBox();
            this.cardGatesDataGridView = new System.Windows.Forms.DataGridView();
            this.applyButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.cardGatesDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // volumeComboBox
            // 
            this.volumeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.volumeComboBox.FormattingEnabled = true;
            this.volumeComboBox.Location = new System.Drawing.Point(89, 14);
            this.volumeComboBox.Name = "volumeComboBox";
            this.volumeComboBox.Size = new System.Drawing.Size(265, 21);
            this.volumeComboBox.TabIndex = 1;
            this.toolTip1.SetToolTip(this.volumeComboBox, "Select the SMS volume describing the desired card, then select the card to edit w" +
        "ithin that volume");
            this.volumeComboBox.SelectedIndexChanged += new System.EventHandler(this.volumeComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Volume: ";
            this.toolTip1.SetToolTip(this.label1, "Select the SMS volume describing the desired card, then select the card to edit w" +
        "ithin that volume");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Card Type:";
            this.toolTip1.SetToolTip(this.label2, "Select the SMS volume describing the desired card, then select the card to edit w" +
        "ithin that volume");
            // 
            // cardTypeComboBox
            // 
            this.cardTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cardTypeComboBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cardTypeComboBox.FormattingEnabled = true;
            this.cardTypeComboBox.Location = new System.Drawing.Point(89, 45);
            this.cardTypeComboBox.Name = "cardTypeComboBox";
            this.cardTypeComboBox.Size = new System.Drawing.Size(265, 22);
            this.cardTypeComboBox.TabIndex = 2;
            this.cardTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.cardTypeComboBox_SelectedIndexChanged);
            // 
            // cardGatesDataGridView
            // 
            this.cardGatesDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cardGatesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.cardGatesDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.cardGatesDataGridView.Location = new System.Drawing.Point(15, 106);
            this.cardGatesDataGridView.Name = "cardGatesDataGridView";
            this.cardGatesDataGridView.Size = new System.Drawing.Size(735, 269);
            this.cardGatesDataGridView.TabIndex = 4;
            this.cardGatesDataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.cardGatesDataGridView_CellValidating);
            this.cardGatesDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.cardGatesDataGridView_CellValueChanged);
            this.cardGatesDataGridView.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.cardGatesDataGridView_EditingControlShowing);
            this.cardGatesDataGridView.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.cardGatesDataGridView_UserDeletingRow);
            // 
            // applyButton
            // 
            this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.applyButton.Location = new System.Drawing.Point(15, 393);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(75, 23);
            this.applyButton.TabIndex = 5;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelButton.Location = new System.Drawing.Point(451, 393);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(258, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "Gates";
            // 
            // EditCardGatesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(762, 428);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.cardGatesDataGridView);
            this.Controls.Add(this.cardTypeComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.volumeComboBox);
            this.Name = "EditCardGatesForm";
            this.Text = "Edit Card Gates";
            ((System.ComponentModel.ISupportInitialize)(this.cardGatesDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox volumeComboBox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cardTypeComboBox;
        private System.Windows.Forms.DataGridView cardGatesDataGridView;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label3;
    }
}