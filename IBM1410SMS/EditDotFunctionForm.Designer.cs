namespace IBM1410SMS
{
    partial class EditDotFunctionForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pageTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.machineTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.volumeTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.diagramColumnTextBox = new System.Windows.Forms.TextBox();
            this.diagramRowTextBox = new System.Windows.Forms.TextBox();
            this.andRadioButton = new System.Windows.Forms.RadioButton();
            this.orRadioButton = new System.Windows.Forms.RadioButton();
            this.exemptCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.applyButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.connectionsDataGridView = new System.Windows.Forms.DataGridView();
            this.label6 = new System.Windows.Forms.Label();
            this.addInputOutputButton = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.forceLogicFunction = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.connectionsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // pageTextBox
            // 
            this.pageTextBox.Location = new System.Drawing.Point(254, 23);
            this.pageTextBox.Name = "pageTextBox";
            this.pageTextBox.ReadOnly = true;
            this.pageTextBox.Size = new System.Drawing.Size(118, 20);
            this.pageTextBox.TabIndex = 15;
            this.toolTip1.SetToolTip(this.pageTextBox, "Page name for this drawing (xx.yy.zz.s)");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(213, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Page:";
            this.toolTip1.SetToolTip(this.label3, "Page name for this drawing (xx.yy.zz.s)");
            // 
            // machineTextBox
            // 
            this.machineTextBox.Location = new System.Drawing.Point(70, 23);
            this.machineTextBox.Name = "machineTextBox";
            this.machineTextBox.ReadOnly = true;
            this.machineTextBox.Size = new System.Drawing.Size(69, 20);
            this.machineTextBox.TabIndex = 13;
            this.machineTextBox.TabStop = false;
            this.toolTip1.SetToolTip(this.machineTextBox, "Machine for Volume that contains this ALD drawing page.");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Machine:";
            this.toolTip1.SetToolTip(this.label1, "Machine for Volume that contains this ALD drawing page.");
            // 
            // volumeTextBox
            // 
            this.volumeTextBox.Location = new System.Drawing.Point(70, 66);
            this.volumeTextBox.Name = "volumeTextBox";
            this.volumeTextBox.ReadOnly = true;
            this.volumeTextBox.Size = new System.Drawing.Size(302, 20);
            this.volumeTextBox.TabIndex = 11;
            this.volumeTextBox.TabStop = false;
            this.toolTip1.SetToolTip(this.volumeTextBox, "Volume Set and Volume containing this drawing");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Volume:";
            this.toolTip1.SetToolTip(this.label2, "Volume Set and Volume containing this drawing");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(198, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Diagram Row:";
            this.toolTip1.SetToolTip(this.label4, "TOP row of logic gates affected by this DOT function.");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 112);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Diagram Column:";
            this.toolTip1.SetToolTip(this.label5, "Diagram Column to the LEFT of this DOT function");
            // 
            // diagramColumnTextBox
            // 
            this.diagramColumnTextBox.Location = new System.Drawing.Point(106, 109);
            this.diagramColumnTextBox.Name = "diagramColumnTextBox";
            this.diagramColumnTextBox.ReadOnly = true;
            this.diagramColumnTextBox.Size = new System.Drawing.Size(69, 20);
            this.diagramColumnTextBox.TabIndex = 19;
            this.diagramColumnTextBox.TabStop = false;
            this.toolTip1.SetToolTip(this.diagramColumnTextBox, "Diagram Column to the LEFT of this DOT function");
            // 
            // diagramRowTextBox
            // 
            this.diagramRowTextBox.Location = new System.Drawing.Point(278, 109);
            this.diagramRowTextBox.Name = "diagramRowTextBox";
            this.diagramRowTextBox.ReadOnly = true;
            this.diagramRowTextBox.Size = new System.Drawing.Size(69, 20);
            this.diagramRowTextBox.TabIndex = 20;
            this.diagramRowTextBox.TabStop = false;
            this.toolTip1.SetToolTip(this.diagramRowTextBox, "TOP row of logic gates affected by this DOT function.");
            // 
            // andRadioButton
            // 
            this.andRadioButton.AutoSize = true;
            this.andRadioButton.Location = new System.Drawing.Point(38, 19);
            this.andRadioButton.Name = "andRadioButton";
            this.andRadioButton.Size = new System.Drawing.Size(48, 17);
            this.andRadioButton.TabIndex = 0;
            this.andRadioButton.TabStop = true;
            this.andRadioButton.Text = "AND";
            this.toolTip1.SetToolTip(this.andRadioButton, "Select to indicate this is an AND dot function");
            this.andRadioButton.UseVisualStyleBackColor = true;
            // 
            // orRadioButton
            // 
            this.orRadioButton.AutoSize = true;
            this.orRadioButton.Location = new System.Drawing.Point(92, 19);
            this.orRadioButton.Name = "orRadioButton";
            this.orRadioButton.Size = new System.Drawing.Size(41, 17);
            this.orRadioButton.TabIndex = 1;
            this.orRadioButton.TabStop = true;
            this.orRadioButton.Text = "OR";
            this.toolTip1.SetToolTip(this.orRadioButton, "Select to indicate this is an OR dot function");
            this.orRadioButton.UseVisualStyleBackColor = true;
            // 
            // exemptCheckBox
            // 
            this.exemptCheckBox.AutoSize = true;
            this.exemptCheckBox.Location = new System.Drawing.Point(383, 165);
            this.exemptCheckBox.Name = "exemptCheckBox";
            this.exemptCheckBox.Size = new System.Drawing.Size(169, 17);
            this.exemptCheckBox.TabIndex = 28;
            this.exemptCheckBox.Text = "Verified / Exempt from Checks";
            this.toolTip1.SetToolTip(this.exemptCheckBox, "Select to suppress error messages for this DOT Function in validation reports");
            this.exemptCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.orRadioButton);
            this.groupBox1.Controls.Add(this.andRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(16, 146);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(159, 44);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Logic Function";
            // 
            // applyButton
            // 
            this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.applyButton.Location = new System.Drawing.Point(12, 363);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(75, 23);
            this.applyButton.TabIndex = 22;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.deleteButton.Location = new System.Drawing.Point(162, 363);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 23);
            this.deleteButton.TabIndex = 23;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelButton.Location = new System.Drawing.Point(297, 363);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 24;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // connectionsDataGridView
            // 
            this.connectionsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.connectionsDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.connectionsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.InactiveBorder;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.connectionsDataGridView.DefaultCellStyle = dataGridViewCellStyle1;
            this.connectionsDataGridView.Location = new System.Drawing.Point(12, 221);
            this.connectionsDataGridView.Name = "connectionsDataGridView";
            this.connectionsDataGridView.Size = new System.Drawing.Size(568, 124);
            this.connectionsDataGridView.TabIndex = 25;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(145, 197);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(205, 13);
            this.label6.TabIndex = 26;
            this.label6.Text = "Connections (Gates are Read only)";
            // 
            // addInputOutputButton
            // 
            this.addInputOutputButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addInputOutputButton.Location = new System.Drawing.Point(216, 159);
            this.addInputOutputButton.Name = "addInputOutputButton";
            this.addInputOutputButton.Size = new System.Drawing.Size(115, 23);
            this.addInputOutputButton.TabIndex = 27;
            this.addInputOutputButton.Text = "Add Input/Output";
            this.addInputOutputButton.UseVisualStyleBackColor = true;
            this.addInputOutputButton.Click += new System.EventHandler(this.addOutputButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(380, 112);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(113, 13);
            this.label7.TabIndex = 29;
            this.label7.Text = "Force Logic Function: ";
            // 
            // forceLogicFunction
            // 
            this.forceLogicFunction.Location = new System.Drawing.Point(499, 109);
            this.forceLogicFunction.MaxLength = 8;
            this.forceLogicFunction.Name = "forceLogicFunction";
            this.forceLogicFunction.Size = new System.Drawing.Size(75, 20);
            this.forceLogicFunction.TabIndex = 30;
            // 
            // EditDotFunctionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 398);
            this.Controls.Add(this.forceLogicFunction);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.exemptCheckBox);
            this.Controls.Add(this.addInputOutputButton);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.connectionsDataGridView);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.diagramRowTextBox);
            this.Controls.Add(this.diagramColumnTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pageTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.machineTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.volumeTextBox);
            this.Controls.Add(this.label2);
            this.Name = "EditDotFunctionForm";
            this.Text = "Edit DOT Function";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.connectionsDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox pageTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox machineTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox volumeTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox diagramColumnTextBox;
        private System.Windows.Forms.TextBox diagramRowTextBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton orRadioButton;
        private System.Windows.Forms.RadioButton andRadioButton;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.DataGridView connectionsDataGridView;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button addInputOutputButton;
        private System.Windows.Forms.CheckBox exemptCheckBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox forceLogicFunction;
    }
}