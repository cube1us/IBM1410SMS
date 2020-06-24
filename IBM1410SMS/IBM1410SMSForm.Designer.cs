namespace IBM1410SMS
{
    partial class IBM1410SMSForm
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.editMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMachinesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editEcosMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editFeaturesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editFramesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMachineGatesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editPanelsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editVolumeSetsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editVolumeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cardLocationPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cardLocationShartsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tieDownsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aldDiagramPagesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cableEdgeConnectionPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editDataMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.IBMLogicFunctionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.standardLogicFunctionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iBMLogicFamiliesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logicVoltageLevelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SMSCardsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editSMSCardTypesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editSMSCardGatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editSMSCardPinsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importFeaturesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importSMSCardsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importLocationPagesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importCardLocationChartMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importTieDownsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateHDLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cardTypeReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CONNectorCheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.edgeConnectionCheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.signalQueryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.signalReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectionChecksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.importBusSignalsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editMenuItem,
            this.editDataMenuItem,
            this.SMSCardsToolStripMenuItem,
            this.importToolStripMenuItem,
            this.generateHDLToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.reportsToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(644, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // editMenuItem
            // 
            this.editMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editMachinesMenuItem,
            this.editEcosMenuItem,
            this.editFeaturesMenuItem,
            this.editFramesMenuItem,
            this.editMachineGatesMenuItem,
            this.editPanelsMenuItem,
            this.editVolumeSetsMenuItem,
            this.editVolumeMenuItem,
            this.cardLocationPagesToolStripMenuItem,
            this.cardLocationShartsToolStripMenuItem,
            this.tieDownsMenuItem,
            this.aldDiagramPagesMenuItem,
            this.cableEdgeConnectionPagesToolStripMenuItem});
            this.editMenuItem.Name = "editMenuItem";
            this.editMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editMenuItem.Text = "Edit";
            // 
            // editMachinesMenuItem
            // 
            this.editMachinesMenuItem.Name = "editMachinesMenuItem";
            this.editMachinesMenuItem.Size = new System.Drawing.Size(234, 22);
            this.editMachinesMenuItem.Text = "Machines";
            this.editMachinesMenuItem.Click += new System.EventHandler(this.editMachinesMenuItem_Click);
            // 
            // editEcosMenuItem
            // 
            this.editEcosMenuItem.Name = "editEcosMenuItem";
            this.editEcosMenuItem.Size = new System.Drawing.Size(234, 22);
            this.editEcosMenuItem.Text = "ECOs";
            this.editEcosMenuItem.Click += new System.EventHandler(this.editEcosMenuItem_Click);
            // 
            // editFeaturesMenuItem
            // 
            this.editFeaturesMenuItem.Name = "editFeaturesMenuItem";
            this.editFeaturesMenuItem.Size = new System.Drawing.Size(234, 22);
            this.editFeaturesMenuItem.Text = "Features";
            this.editFeaturesMenuItem.Click += new System.EventHandler(this.editFeaturesMenuItem_Click);
            // 
            // editFramesMenuItem
            // 
            this.editFramesMenuItem.Name = "editFramesMenuItem";
            this.editFramesMenuItem.Size = new System.Drawing.Size(234, 22);
            this.editFramesMenuItem.Text = "Frames";
            this.editFramesMenuItem.Click += new System.EventHandler(this.editFramesMenuItem_Click);
            // 
            // editMachineGatesMenuItem
            // 
            this.editMachineGatesMenuItem.Name = "editMachineGatesMenuItem";
            this.editMachineGatesMenuItem.Size = new System.Drawing.Size(234, 22);
            this.editMachineGatesMenuItem.Text = "Machine Gates";
            this.editMachineGatesMenuItem.Click += new System.EventHandler(this.editMachineGatesMenuItem_Click);
            // 
            // editPanelsMenuItem
            // 
            this.editPanelsMenuItem.Name = "editPanelsMenuItem";
            this.editPanelsMenuItem.Size = new System.Drawing.Size(234, 22);
            this.editPanelsMenuItem.Text = "Panels";
            this.editPanelsMenuItem.Click += new System.EventHandler(this.editPanelsMenuItem_Click);
            // 
            // editVolumeSetsMenuItem
            // 
            this.editVolumeSetsMenuItem.Name = "editVolumeSetsMenuItem";
            this.editVolumeSetsMenuItem.Size = new System.Drawing.Size(234, 22);
            this.editVolumeSetsMenuItem.Text = "Volume Sets";
            this.editVolumeSetsMenuItem.Click += new System.EventHandler(this.editVolumeSetsMenuItem_Click);
            // 
            // editVolumeMenuItem
            // 
            this.editVolumeMenuItem.Name = "editVolumeMenuItem";
            this.editVolumeMenuItem.Size = new System.Drawing.Size(234, 22);
            this.editVolumeMenuItem.Text = "Volumes";
            this.editVolumeMenuItem.Click += new System.EventHandler(this.editVolumeMenuItem_Click);
            // 
            // cardLocationPagesToolStripMenuItem
            // 
            this.cardLocationPagesToolStripMenuItem.Name = "cardLocationPagesToolStripMenuItem";
            this.cardLocationPagesToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.cardLocationPagesToolStripMenuItem.Text = "Card Location Pages";
            this.cardLocationPagesToolStripMenuItem.Click += new System.EventHandler(this.cardLocationPagesToolStripMenuItem_Click);
            // 
            // cardLocationShartsToolStripMenuItem
            // 
            this.cardLocationShartsToolStripMenuItem.Name = "cardLocationShartsToolStripMenuItem";
            this.cardLocationShartsToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.cardLocationShartsToolStripMenuItem.Text = "Card Location Charts";
            this.cardLocationShartsToolStripMenuItem.Click += new System.EventHandler(this.cardLocationShartsToolStripMenuItem_Click);
            // 
            // tieDownsMenuItem
            // 
            this.tieDownsMenuItem.Name = "tieDownsMenuItem";
            this.tieDownsMenuItem.Size = new System.Drawing.Size(234, 22);
            this.tieDownsMenuItem.Text = "Tie Downs";
            this.tieDownsMenuItem.Click += new System.EventHandler(this.tieDownsMenuItem_Click);
            // 
            // aldDiagramPagesMenuItem
            // 
            this.aldDiagramPagesMenuItem.Name = "aldDiagramPagesMenuItem";
            this.aldDiagramPagesMenuItem.Size = new System.Drawing.Size(234, 22);
            this.aldDiagramPagesMenuItem.Text = "ALD Diagram Pages";
            this.aldDiagramPagesMenuItem.Click += new System.EventHandler(this.aldDiagramPagesMenuItem_Click);
            // 
            // cableEdgeConnectionPagesToolStripMenuItem
            // 
            this.cableEdgeConnectionPagesToolStripMenuItem.Name = "cableEdgeConnectionPagesToolStripMenuItem";
            this.cableEdgeConnectionPagesToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.cableEdgeConnectionPagesToolStripMenuItem.Text = "Cable/Edge Connection Pages";
            this.cableEdgeConnectionPagesToolStripMenuItem.Click += new System.EventHandler(this.cableEdgeConnectionPagesToolStripMenuItem_Click);
            // 
            // editDataMenuItem
            // 
            this.editDataMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.IBMLogicFunctionsMenuItem,
            this.standardLogicFunctionsMenuItem,
            this.iBMLogicFamiliesToolStripMenuItem,
            this.logicVoltageLevelsToolStripMenuItem});
            this.editDataMenuItem.Name = "editDataMenuItem";
            this.editDataMenuItem.Size = new System.Drawing.Size(48, 20);
            this.editDataMenuItem.Text = "Logic";
            // 
            // IBMLogicFunctionsMenuItem
            // 
            this.IBMLogicFunctionsMenuItem.Name = "IBMLogicFunctionsMenuItem";
            this.IBMLogicFunctionsMenuItem.Size = new System.Drawing.Size(208, 22);
            this.IBMLogicFunctionsMenuItem.Text = "IBM Logic Functions";
            this.IBMLogicFunctionsMenuItem.Click += new System.EventHandler(this.IBMLogicFunctionsMenuItem_Click);
            // 
            // standardLogicFunctionsMenuItem
            // 
            this.standardLogicFunctionsMenuItem.Name = "standardLogicFunctionsMenuItem";
            this.standardLogicFunctionsMenuItem.Size = new System.Drawing.Size(208, 22);
            this.standardLogicFunctionsMenuItem.Text = "Standard Logic Functions";
            this.standardLogicFunctionsMenuItem.Click += new System.EventHandler(this.standardLogicFunctionsMenuItem_Click);
            // 
            // iBMLogicFamiliesToolStripMenuItem
            // 
            this.iBMLogicFamiliesToolStripMenuItem.Name = "iBMLogicFamiliesToolStripMenuItem";
            this.iBMLogicFamiliesToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.iBMLogicFamiliesToolStripMenuItem.Text = "IBM Logic Families";
            this.iBMLogicFamiliesToolStripMenuItem.Click += new System.EventHandler(this.iBMLogicFamiliesToolStripMenuItem_Click);
            // 
            // logicVoltageLevelsToolStripMenuItem
            // 
            this.logicVoltageLevelsToolStripMenuItem.Name = "logicVoltageLevelsToolStripMenuItem";
            this.logicVoltageLevelsToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.logicVoltageLevelsToolStripMenuItem.Text = "Logic Voltage Levels";
            this.logicVoltageLevelsToolStripMenuItem.Click += new System.EventHandler(this.logicVoltageLevelsToolStripMenuItem_Click);
            // 
            // SMSCardsToolStripMenuItem
            // 
            this.SMSCardsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editSMSCardTypesToolStripMenuItem,
            this.editSMSCardGatesToolStripMenuItem,
            this.editSMSCardPinsToolStripMenuItem});
            this.SMSCardsToolStripMenuItem.Name = "SMSCardsToolStripMenuItem";
            this.SMSCardsToolStripMenuItem.Size = new System.Drawing.Size(75, 20);
            this.SMSCardsToolStripMenuItem.Text = "SMS Cards";
            // 
            // editSMSCardTypesToolStripMenuItem
            // 
            this.editSMSCardTypesToolStripMenuItem.Name = "editSMSCardTypesToolStripMenuItem";
            this.editSMSCardTypesToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.editSMSCardTypesToolStripMenuItem.Text = "Edit SMS Card Types";
            this.editSMSCardTypesToolStripMenuItem.Click += new System.EventHandler(this.editSMSCardTypesToolStripMenuItem_Click);
            // 
            // editSMSCardGatesToolStripMenuItem
            // 
            this.editSMSCardGatesToolStripMenuItem.Name = "editSMSCardGatesToolStripMenuItem";
            this.editSMSCardGatesToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.editSMSCardGatesToolStripMenuItem.Text = "Edit SMS Card Gates";
            this.editSMSCardGatesToolStripMenuItem.Click += new System.EventHandler(this.editSMSCardGatesToolStripMenuItem_Click);
            // 
            // editSMSCardPinsToolStripMenuItem
            // 
            this.editSMSCardPinsToolStripMenuItem.Name = "editSMSCardPinsToolStripMenuItem";
            this.editSMSCardPinsToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.editSMSCardPinsToolStripMenuItem.Text = "Edit SMS Card Pins";
            this.editSMSCardPinsToolStripMenuItem.Click += new System.EventHandler(this.editSMSCardPinsToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importFeaturesMenuItem,
            this.importSMSCardsMenuItem,
            this.importLocationPagesMenuItem,
            this.importCardLocationChartMenuItem,
            this.importTieDownsMenuItem,
            this.importBusSignalsToolStripMenuItem});
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.importToolStripMenuItem.Text = "Import";
            // 
            // importFeaturesMenuItem
            // 
            this.importFeaturesMenuItem.Name = "importFeaturesMenuItem";
            this.importFeaturesMenuItem.Size = new System.Drawing.Size(219, 22);
            this.importFeaturesMenuItem.Text = "Import Features";
            this.importFeaturesMenuItem.Click += new System.EventHandler(this.importFeaturesMenuItem_Click);
            // 
            // importSMSCardsMenuItem
            // 
            this.importSMSCardsMenuItem.Name = "importSMSCardsMenuItem";
            this.importSMSCardsMenuItem.Size = new System.Drawing.Size(219, 22);
            this.importSMSCardsMenuItem.Text = "Import SMS Cards";
            this.importSMSCardsMenuItem.Click += new System.EventHandler(this.importSMSCardsMenuItem_Click);
            // 
            // importLocationPagesMenuItem
            // 
            this.importLocationPagesMenuItem.Name = "importLocationPagesMenuItem";
            this.importLocationPagesMenuItem.Size = new System.Drawing.Size(219, 22);
            this.importLocationPagesMenuItem.Text = "Import Location Pages";
            this.importLocationPagesMenuItem.Click += new System.EventHandler(this.importLocationPagesToolStripMenuItem_Click);
            // 
            // importCardLocationChartMenuItem
            // 
            this.importCardLocationChartMenuItem.Name = "importCardLocationChartMenuItem";
            this.importCardLocationChartMenuItem.Size = new System.Drawing.Size(219, 22);
            this.importCardLocationChartMenuItem.Text = "Import Card Location Chart";
            this.importCardLocationChartMenuItem.Click += new System.EventHandler(this.importCardLocationChartMenuItem_Click);
            // 
            // importTieDownsMenuItem
            // 
            this.importTieDownsMenuItem.Name = "importTieDownsMenuItem";
            this.importTieDownsMenuItem.Size = new System.Drawing.Size(219, 22);
            this.importTieDownsMenuItem.Text = "Import Tie Downs";
            this.importTieDownsMenuItem.Click += new System.EventHandler(this.importTieDownsMenuItem_Click);
            // 
            // generateHDLToolStripMenuItem
            // 
            this.generateHDLToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateMenuItem,
            this.generateGroupToolStripMenuItem});
            this.generateHDLToolStripMenuItem.Name = "generateHDLToolStripMenuItem";
            this.generateHDLToolStripMenuItem.Size = new System.Drawing.Size(92, 20);
            this.generateHDLToolStripMenuItem.Text = "Generate HDL";
            // 
            // generateMenuItem
            // 
            this.generateMenuItem.Name = "generateMenuItem";
            this.generateMenuItem.Size = new System.Drawing.Size(157, 22);
            this.generateMenuItem.Text = "Generate";
            this.generateMenuItem.Click += new System.EventHandler(this.generateMenuItem_Click);
            // 
            // generateGroupToolStripMenuItem
            // 
            this.generateGroupToolStripMenuItem.Name = "generateGroupToolStripMenuItem";
            this.generateGroupToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.generateGroupToolStripMenuItem.Text = "Generate Group";
            this.generateGroupToolStripMenuItem.Click += new System.EventHandler(this.generateGroupToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // reportsToolStripMenuItem
            // 
            this.reportsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cardTypeReportToolStripMenuItem,
            this.CONNectorCheckToolStripMenuItem,
            this.edgeConnectionCheckToolStripMenuItem,
            this.signalQueryToolStripMenuItem,
            this.signalReportToolStripMenuItem,
            this.connectionChecksToolStripMenuItem});
            this.reportsToolStripMenuItem.Name = "reportsToolStripMenuItem";
            this.reportsToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.reportsToolStripMenuItem.Text = "Reports";
            // 
            // cardTypeReportToolStripMenuItem
            // 
            this.cardTypeReportToolStripMenuItem.Name = "cardTypeReportToolStripMenuItem";
            this.cardTypeReportToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.cardTypeReportToolStripMenuItem.Text = "Card Type Report";
            this.cardTypeReportToolStripMenuItem.Click += new System.EventHandler(this.cardTypeReportToolStripMenuItem_Click);
            // 
            // CONNectorCheckToolStripMenuItem
            // 
            this.CONNectorCheckToolStripMenuItem.Name = "CONNectorCheckToolStripMenuItem";
            this.CONNectorCheckToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.CONNectorCheckToolStripMenuItem.Text = "Connector Check";
            this.CONNectorCheckToolStripMenuItem.Click += new System.EventHandler(this.CONNectorCheckToolStripMenuItem_Click);
            // 
            // edgeConnectionCheckToolStripMenuItem
            // 
            this.edgeConnectionCheckToolStripMenuItem.Name = "edgeConnectionCheckToolStripMenuItem";
            this.edgeConnectionCheckToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.edgeConnectionCheckToolStripMenuItem.Text = "Edge Connection Check";
            this.edgeConnectionCheckToolStripMenuItem.Click += new System.EventHandler(this.edgeConnectionCheckToolStripMenuItem_Click);
            // 
            // signalQueryToolStripMenuItem
            // 
            this.signalQueryToolStripMenuItem.Name = "signalQueryToolStripMenuItem";
            this.signalQueryToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.signalQueryToolStripMenuItem.Text = "Signal Query";
            this.signalQueryToolStripMenuItem.Click += new System.EventHandler(this.signalQueryToolStripMenuItem_Click);
            // 
            // signalReportToolStripMenuItem
            // 
            this.signalReportToolStripMenuItem.Name = "signalReportToolStripMenuItem";
            this.signalReportToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.signalReportToolStripMenuItem.Text = "Signal Report";
            this.signalReportToolStripMenuItem.Click += new System.EventHandler(this.signalReportToolStripMenuItem_Click);
            // 
            // connectionChecksToolStripMenuItem
            // 
            this.connectionChecksToolStripMenuItem.Name = "connectionChecksToolStripMenuItem";
            this.connectionChecksToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.connectionChecksToolStripMenuItem.Text = "Connection Checks";
            this.connectionChecksToolStripMenuItem.Click += new System.EventHandler(this.connectionChecksToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "csv";
            this.openFileDialog1.FileName = "*.csv";
            this.openFileDialog1.Filter = "CSV Files|*.csv";
            this.openFileDialog1.InitialDirectory = "D:\\Users\\Jay\\Schematics\\IBM1410";
            this.openFileDialog1.Title = "Specify File To Import";
            // 
            // importBusSignalsToolStripMenuItem
            // 
            this.importBusSignalsToolStripMenuItem.Name = "importBusSignalsToolStripMenuItem";
            this.importBusSignalsToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.importBusSignalsToolStripMenuItem.Text = "Import Bus Signals";
            this.importBusSignalsToolStripMenuItem.Click += new System.EventHandler(this.importBusSignalsToolStripMenuItem_Click);
            // 
            // IBM1410SMSForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 385);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "IBM1410SMSForm";
            this.Text = "IBM SMS Data Capture";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem editMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editFeaturesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editVolumeSetsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editVolumeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editMachinesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editFramesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editMachineGatesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editPanelsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editEcosMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cardLocationPagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editDataMenuItem;
        private System.Windows.Forms.ToolStripMenuItem IBMLogicFunctionsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem standardLogicFunctionsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iBMLogicFamiliesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logicVoltageLevelsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SMSCardsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editSMSCardTypesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editSMSCardGatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editSMSCardPinsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cardLocationShartsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importFeaturesMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem importSMSCardsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importLocationPagesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importCardLocationChartMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importTieDownsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tieDownsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aldDiagramPagesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateHDLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateGroupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cardTypeReportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cableEdgeConnectionPagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CONNectorCheckToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem edgeConnectionCheckToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem signalReportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectionChecksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem signalQueryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importBusSignalsToolStripMenuItem;
    }
}

