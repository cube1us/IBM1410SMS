﻿namespace IBM1410SMS
{
    partial class EditCableEdgeConnectionBlockForm
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
            this.diagramRowTextBox = new System.Windows.Forms.TextBox();
            this.diagramColumnTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pageTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.machineTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.volumeTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cableEdgeConnectionBlockTitleTextBox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.machineComboBox = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.frameComboBox = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.gateComboBox = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.ecoTagComboBox = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.panelComboBox = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.cardRowComboBox = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.cardTypeComboBox = new System.Windows.Forms.ComboBox();
            this.cardColumnTextBox = new System.Windows.Forms.TextBox();
            this.applyButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.cableEdgeConnectionBlockDrawingTextBox = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // diagramRowTextBox
            // 
            this.diagramRowTextBox.Location = new System.Drawing.Point(263, 82);
            this.diagramRowTextBox.Name = "diagramRowTextBox";
            this.diagramRowTextBox.ReadOnly = true;
            this.diagramRowTextBox.Size = new System.Drawing.Size(69, 20);
            this.diagramRowTextBox.TabIndex = 30;
            this.diagramRowTextBox.TabStop = false;
            this.toolTip1.SetToolTip(this.diagramRowTextBox, "Page Row contining this Cable/Edge Connection Block");
            // 
            // diagramColumnTextBox
            // 
            this.diagramColumnTextBox.Location = new System.Drawing.Point(103, 82);
            this.diagramColumnTextBox.Name = "diagramColumnTextBox";
            this.diagramColumnTextBox.ReadOnly = true;
            this.diagramColumnTextBox.Size = new System.Drawing.Size(69, 20);
            this.diagramColumnTextBox.TabIndex = 29;
            this.diagramColumnTextBox.TabStop = false;
            this.toolTip1.SetToolTip(this.diagramColumnTextBox, "Page Column contining this Cable/Edge Connection Block");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 85);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 13);
            this.label5.TabIndex = 28;
            this.label5.Text = "Diagram Column:";
            this.toolTip1.SetToolTip(this.label5, "Diagram Column contining this Logic Block");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(183, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 27;
            this.label4.Text = "Diagram Row:";
            this.toolTip1.SetToolTip(this.label4, "Diagram Row contining this Logic Block");
            // 
            // pageTextBox
            // 
            this.pageTextBox.Location = new System.Drawing.Point(51, 47);
            this.pageTextBox.Name = "pageTextBox";
            this.pageTextBox.ReadOnly = true;
            this.pageTextBox.Size = new System.Drawing.Size(118, 20);
            this.pageTextBox.TabIndex = 26;
            this.pageTextBox.TabStop = false;
            this.toolTip1.SetToolTip(this.pageTextBox, "Page name for this drawing (xx.yy.zz.s)");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "Page:";
            this.toolTip1.SetToolTip(this.label3, "Page name for this drawing (xx.yy.zz.s)");
            // 
            // machineTextBox
            // 
            this.machineTextBox.Location = new System.Drawing.Point(103, 12);
            this.machineTextBox.Name = "machineTextBox";
            this.machineTextBox.ReadOnly = true;
            this.machineTextBox.Size = new System.Drawing.Size(69, 20);
            this.machineTextBox.TabIndex = 24;
            this.machineTextBox.TabStop = false;
            this.toolTip1.SetToolTip(this.machineTextBox, "Machine for Volume that contains this Connection drawing page.");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Diagram Machine:";
            this.toolTip1.SetToolTip(this.label1, "Machine for Volume that contains this ALD drawing page.");
            // 
            // volumeTextBox
            // 
            this.volumeTextBox.Location = new System.Drawing.Point(240, 12);
            this.volumeTextBox.Name = "volumeTextBox";
            this.volumeTextBox.ReadOnly = true;
            this.volumeTextBox.Size = new System.Drawing.Size(302, 20);
            this.volumeTextBox.TabIndex = 22;
            this.volumeTextBox.TabStop = false;
            this.toolTip1.SetToolTip(this.volumeTextBox, "Volume Set and Volume containing this page");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(183, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "Volume:";
            this.toolTip1.SetToolTip(this.label2, "Volume Set and Volume containing this drawing");
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(349, 85);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 31;
            this.label6.Text = "Top Note:";
            this.toolTip1.SetToolTip(this.label6, "The title that may appear above the logic block (e.g. \"GATE D\")");
            // 
            // cableEdgeConnectionBlockTitleTextBox
            // 
            this.cableEdgeConnectionBlockTitleTextBox.Location = new System.Drawing.Point(418, 82);
            this.cableEdgeConnectionBlockTitleTextBox.MaxLength = 10;
            this.cableEdgeConnectionBlockTitleTextBox.Name = "cableEdgeConnectionBlockTitleTextBox";
            this.cableEdgeConnectionBlockTitleTextBox.Size = new System.Drawing.Size(76, 20);
            this.cableEdgeConnectionBlockTitleTextBox.TabIndex = 20;
            this.toolTip1.SetToolTip(this.cableEdgeConnectionBlockTitleTextBox, "The text that may appear above the logic block (e.g. \"GATE D\")");
            this.cableEdgeConnectionBlockTitleTextBox.TextChanged += new System.EventHandler(this.cableEdgeConnectionBlockTitleTextBox_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 136);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(126, 13);
            this.label11.TabIndex = 45;
            this.label11.Text = "Card Location:  Machine:";
            this.toolTip1.SetToolTip(this.label11, "The machine associated with the card for this logic block (e.g. 1411, 1415, etc.)" +
        "");
            // 
            // machineComboBox
            // 
            this.machineComboBox.DisplayMember = "name";
            this.machineComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.machineComboBox.FormattingEnabled = true;
            this.machineComboBox.Location = new System.Drawing.Point(142, 133);
            this.machineComboBox.Name = "machineComboBox";
            this.machineComboBox.Size = new System.Drawing.Size(86, 21);
            this.machineComboBox.TabIndex = 60;
            this.toolTip1.SetToolTip(this.machineComboBox, "The machine associated with the card for this Cable/Edge Connection block (e.g. 1" +
        "411, 1415, etc.)");
            this.machineComboBox.SelectedIndexChanged += new System.EventHandler(this.machineComboBox_SelectedIndexChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(258, 136);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(39, 13);
            this.label12.TabIndex = 47;
            this.label12.Text = "Frame:";
            this.toolTip1.SetToolTip(this.label12, "The Frame containing the card associated with this logic block");
            // 
            // frameComboBox
            // 
            this.frameComboBox.DisplayMember = "name";
            this.frameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.frameComboBox.FormattingEnabled = true;
            this.frameComboBox.Location = new System.Drawing.Point(303, 133);
            this.frameComboBox.Name = "frameComboBox";
            this.frameComboBox.Size = new System.Drawing.Size(54, 21);
            this.frameComboBox.TabIndex = 64;
            this.toolTip1.SetToolTip(this.frameComboBox, "The Frame containing the card associated with this cable/edge connection block");
            this.frameComboBox.SelectedIndexChanged += new System.EventHandler(this.frameComboBox_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 171);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(33, 13);
            this.label13.TabIndex = 52;
            this.label13.Text = "Gate:";
            this.toolTip1.SetToolTip(this.label13, "The Gate containing the card associated with this logic block");
            // 
            // gateComboBox
            // 
            this.gateComboBox.DisplayMember = "name";
            this.gateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.gateComboBox.FormattingEnabled = true;
            this.gateComboBox.Location = new System.Drawing.Point(49, 168);
            this.gateComboBox.Name = "gateComboBox";
            this.gateComboBox.Size = new System.Drawing.Size(54, 21);
            this.gateComboBox.TabIndex = 70;
            this.toolTip1.SetToolTip(this.gateComboBox, "The Gate containing the card associated with this cable/edgie connection block");
            this.gateComboBox.SelectedIndexChanged += new System.EventHandler(this.gateComboBox_SelectedIndexChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(253, 171);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(54, 13);
            this.label14.TabIndex = 54;
            this.label14.Text = "ECO Tag:";
            this.toolTip1.SetToolTip(this.label14, "The ECO Tag that appears on the right hand side of the logic block after the fram" +
        "e (e.g. A, B, C, D, E, ...)");
            // 
            // ecoTagComboBox
            // 
            this.ecoTagComboBox.DisplayMember = "name";
            this.ecoTagComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ecoTagComboBox.FormattingEnabled = true;
            this.ecoTagComboBox.Location = new System.Drawing.Point(315, 168);
            this.ecoTagComboBox.Name = "ecoTagComboBox";
            this.ecoTagComboBox.Size = new System.Drawing.Size(42, 21);
            this.ecoTagComboBox.TabIndex = 55;
            this.toolTip1.SetToolTip(this.ecoTagComboBox, "The ECO Tag that appears on the right hand side of the cable/edge connection bloc" +
        "k after the frame (e.g. A, B, C, D, E, ...)");
            this.ecoTagComboBox.SelectedIndexChanged += new System.EventHandler(this.ecoTagComboBox_SelectedIndexChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(10, 206);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(37, 13);
            this.label15.TabIndex = 56;
            this.label15.Text = "Panel:";
            this.toolTip1.SetToolTip(this.label15, "The panel containing the card associated with this logic block");
            // 
            // panelComboBox
            // 
            this.panelComboBox.DisplayMember = "panel";
            this.panelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.panelComboBox.FormattingEnabled = true;
            this.panelComboBox.Location = new System.Drawing.Point(53, 203);
            this.panelComboBox.Name = "panelComboBox";
            this.panelComboBox.Size = new System.Drawing.Size(44, 21);
            this.panelComboBox.TabIndex = 80;
            this.toolTip1.SetToolTip(this.panelComboBox, "The panel containing the card associated with this cable/edge connection block");
            this.panelComboBox.SelectedIndexChanged += new System.EventHandler(this.panelComboBox_SelectedIndexChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(117, 206);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(57, 13);
            this.label16.TabIndex = 58;
            this.label16.Text = "Card Row:";
            this.toolTip1.SetToolTip(this.label16, "The row of the card slot that contains the card associated with this logic block");
            // 
            // cardRowComboBox
            // 
            this.cardRowComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cardRowComboBox.FormattingEnabled = true;
            this.cardRowComboBox.Location = new System.Drawing.Point(180, 203);
            this.cardRowComboBox.Name = "cardRowComboBox";
            this.cardRowComboBox.Size = new System.Drawing.Size(44, 21);
            this.cardRowComboBox.TabIndex = 84;
            this.toolTip1.SetToolTip(this.cardRowComboBox, "The row of the card slot that contains the card associated with this cable/edge c" +
        "onnection block");
            this.cardRowComboBox.SelectedIndexChanged += new System.EventHandler(this.cardRowComboBox_SelectedIndexChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(237, 206);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(70, 13);
            this.label17.TabIndex = 60;
            this.label17.Text = "Card Column:";
            this.toolTip1.SetToolTip(this.label17, "The column of the card slot that contains the card associated with this logic blo" +
        "ck");
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(10, 241);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(59, 13);
            this.label18.TabIndex = 62;
            this.label18.Text = "Card Type:";
            this.toolTip1.SetToolTip(this.label18, "The card type of the card associated with this logic block (e.g. AEK)");
            // 
            // cardTypeComboBox
            // 
            this.cardTypeComboBox.DisplayMember = "type";
            this.cardTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cardTypeComboBox.FormattingEnabled = true;
            this.cardTypeComboBox.Location = new System.Drawing.Point(75, 238);
            this.cardTypeComboBox.Name = "cardTypeComboBox";
            this.cardTypeComboBox.Size = new System.Drawing.Size(86, 21);
            this.cardTypeComboBox.TabIndex = 90;
            this.toolTip1.SetToolTip(this.cardTypeComboBox, "The card type of the card associated with this cable/edge connection block (e.g. " +
        "CABL, CONN)");
            this.cardTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.cardTypeComboBox_SelectedIndexChanged);
            // 
            // cardColumnTextBox
            // 
            this.cardColumnTextBox.Location = new System.Drawing.Point(315, 204);
            this.cardColumnTextBox.MaxLength = 2;
            this.cardColumnTextBox.Name = "cardColumnTextBox";
            this.cardColumnTextBox.Size = new System.Drawing.Size(42, 20);
            this.cardColumnTextBox.TabIndex = 85;
            this.toolTip1.SetToolTip(this.cardColumnTextBox, "The column of the card slot that contains the card associated with this cable/edg" +
        "e connection block");
            this.cardColumnTextBox.TextChanged += new System.EventHandler(this.cardColumnTextBox_TextChanged);
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(17, 401);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(75, 23);
            this.applyButton.TabIndex = 58;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(155, 401);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 23);
            this.deleteButton.TabIndex = 111;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(305, 401);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 112;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // cableEdgeConnectionBlockDrawingTextBox
            // 
            this.cableEdgeConnectionBlockDrawingTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cableEdgeConnectionBlockDrawingTextBox.Location = new System.Drawing.Point(414, 147);
            this.cableEdgeConnectionBlockDrawingTextBox.Multiline = true;
            this.cableEdgeConnectionBlockDrawingTextBox.Name = "cableEdgeConnectionBlockDrawingTextBox";
            this.cableEdgeConnectionBlockDrawingTextBox.Size = new System.Drawing.Size(152, 192);
            this.cableEdgeConnectionBlockDrawingTextBox.TabIndex = 104;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(401, 117);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(178, 13);
            this.label20.TabIndex = 105;
            this.label20.Text = "Cable/Edge Connection Block";
            // 
            // EditCableEdgeConnectionBlockForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 470);
            this.Controls.Add(this.cardColumnTextBox);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.cableEdgeConnectionBlockDrawingTextBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.cardTypeComboBox);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.cardRowComboBox);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.panelComboBox);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.ecoTagComboBox);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.gateComboBox);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.frameComboBox);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.machineComboBox);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.cableEdgeConnectionBlockTitleTextBox);
            this.Controls.Add(this.label6);
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
            this.Name = "EditCableEdgeConnectionBlockForm";
            this.Text = "Edit Cable/EdgeConnection Block";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox diagramRowTextBox;
        private System.Windows.Forms.TextBox diagramColumnTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox pageTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox machineTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox volumeTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox cableEdgeConnectionBlockTitleTextBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox machineComboBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox frameComboBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox gateComboBox;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox ecoTagComboBox;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox panelComboBox;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox cardRowComboBox;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.ComboBox cardTypeComboBox;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox cableEdgeConnectionBlockDrawingTextBox;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox cardColumnTextBox;
    }
}