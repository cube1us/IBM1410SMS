/* 
 *  COPYRIGHT 2018, 2019, 2020 Jay R. Jaeger
 *  
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  (file COPYING.txt) along with this program.  
 *  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MySQLFramework;

namespace IBM1410SMS
{
    public partial class EditCableEdgeConnectionBlockForm : Form
    {

        DBSetup db = DBSetup.Instance;

        Table<Page> pageTable;
        Table<Connection> connectionTable;
        Table<Diagramblock> diagramBlockTable;
        Table<Diagramecotag> diagramEcoTagTable;
        Table<Feature> featureTable;
        Table<Logiclevels> logicLevelTable;
        Table<Machine> machineTable;
        Table<Cardtype> cardTypeTable;
        Table<Frame> frameTable;
        Table<Machinegate> machineGateTable;
        Table<Panel> panelTable;
        Table<Cardgate> cardGateTable;
        Table<Gatepin> gatePinTable;
        Table<Logicfunction> logicFunctionTable;

        Page currentPage;
        Diagrampage currentDiagramPage;
        Machine currentMachine;
        Machine diagramMachine;
        Volumeset currentVolumeSet;
        Volume currentVolume;
        Frame currentFrame = null;
        Machinegate currentMachineGate = null;
        Panel currentPanel = null;
        Cardtype currentCardType = null;

        Diagramblock currentDiagramBlock;
        CardSlotInfo currentCardSlotInfo = null;

        List<Feature> featureList;
        List<Logiclevels> inputLogicLevelList;
        List<Logiclevels> outputLogicLevelList;
        List<Diagramecotag> ecoTagList;
        List<Machine> machineList;
        List<Cardtype> cardTypeList;
        List<Cardgate> cardGateList;
        List<Logicfunction> logicFunctionList;

        string machinePrefix;
        bool populatingDialog = true;
        bool applySuccessful = false;
        bool modifiedMachineGatePanelFrame = false;

        public EditCableEdgeConnectionBlockForm(
            Diagramblock diagramBlock,
            Machine machine,
            Volumeset volumeSet,
            Volume volume,
            Diagrampage diagramPage,
            string diagramRow, 
            int diagramColumn,
            Cardlocation cardLocation) {

            InitializeComponent();

            pageTable = db.getPageTable();
            connectionTable = db.getConnectionTable();
            diagramBlockTable = db.getDiagramBlockTable();
            diagramEcoTagTable = db.getDiagramEcoTagTable();
            featureTable = db.getFeatureTable();
            cardTypeTable = db.getCardTypeTable();
            logicLevelTable = db.getLogicLevelsTable();
            machineTable = db.getMachineTable();
            panelTable = db.getPanelTable();
            machineGateTable = db.getMachineGateTable();
            frameTable = db.getFrameTable();
            cardGateTable = db.getCardGateTable();
            gatePinTable = db.getGatePinTable();
            logicFunctionTable = db.getLogicFunctionTable();

            diagramMachine = machine;

            //  Set up invariant lists...

            machineList = machineTable.getWhere(
                "WHERE machine.name LIKE '" + machinePrefix + "%' ORDER BY machine.name");

            if (machineList.Count == 0) {
                MessageBox.Show("No Machines Defined, cannot proceed.",
                    "No Machines Defined",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.Close();
            }

            inputLogicLevelList = logicLevelTable.getWhere("ORDER BY logicLevel");
            //  Also need a blank logic level entry...
            Logiclevels emptyLevel = new Logiclevels();
            emptyLevel.idLogicLevels = 0;
            emptyLevel.logicLevel = "";
            inputLogicLevelList.Insert(0, emptyLevel);

            //  Second combo box needs its own copy, or it will change entries
            //  with the first one.

            outputLogicLevelList = new List<Logiclevels>(inputLogicLevelList);

            ecoTagList = diagramEcoTagTable.getWhere(
                "WHERE diagramPage='" + diagramPage.idDiagramPage + "'" +
                " ORDER BY diagramecotag.name");
            //  ECO Tag also needs an empty entry...
            Diagramecotag emptyTag = new Diagramecotag();
            emptyTag.idDiagramECOTag = 0;
            emptyTag.name = " ";
            ecoTagList.Insert(0, emptyTag);

            cardTypeList = cardTypeTable.getWhere("ORDER BY cardtype.type");

            featureList = featureTable.getWhere(
                "WHERE machine='" + machine.idMachine + "'" +
                " ORDER BY feature.code");
            Feature emptyFeature = new Feature();
            emptyFeature.idFeature = 0;
            emptyFeature.code = "";
            emptyFeature.feature = "";
            featureList.Insert(0,emptyFeature);

            logicFunctionList = logicFunctionTable.getWhere(
                "ORDER BY logicfunction.name");

            //  Fill in static combo boxes' data sources.

            inputModeComboBox.DataSource = inputLogicLevelList;
            outputModeComboBox.DataSource = outputLogicLevelList;
            ecoTagComboBox.DataSource = ecoTagList;
            cardTypeComboBox.DataSource = cardTypeList;
            machineComboBox.DataSource = machineList;
            cardTypeComboBox.DataSource = cardTypeList;
            featureComboBox.DataSource = featureList;

            //  Fill in constant data.

            currentVolumeSet = volumeSet;
            currentVolume = volume;
            currentPage = pageTable.getByKey(diagramPage.page);
            currentDiagramPage = diagramPage;
            machinePrefix = machine.name.Length >= 4 ?
                machine.name.Substring(0, 2) : "";

            machineTextBox.ReadOnly = true;
            machineTextBox.Text = machine.name;
            volumeTextBox.ReadOnly = true;
            volumeTextBox.Text = currentVolumeSet.machineType + "/" +
                currentVolumeSet.machineSerial + " Volume: " +
                currentVolume.name;
            pageTextBox.ReadOnly = true;
            pageTextBox.Text = currentPage.name;

            diagramRowTextBox.ReadOnly = true;
            diagramRowTextBox.Text = diagramRow;
            diagramColumnTextBox.ReadOnly = true;
            diagramColumnTextBox.Text = diagramColumn.ToString();

            //  Preselect the machine associated with this diagram page.
            //  It may change later depending on what is in the logic
            //  block card slot.

            machineComboBox.SelectedItem = machineList.Find(
                x => x.idMachine == machine.idMachine);

            if(machineComboBox.SelectedItem == null) {
                Console.WriteLine("Machine Combo Box Selected item unexpectedly null.");
                Console.WriteLine("Diagram machine Key = " + machine.idMachine);
            }

            foreach (string row in Helpers.validRows) {
                cardRowComboBox.Items.Add(row);
            }

            //  If the diagram block object passed to us is null, create
            //  one, and fill in as much as we can from the card location
            //  info passed (if any)

            currentDiagramBlock = diagramBlock;
            if(currentDiagramBlock == null || currentDiagramBlock.idDiagramBlock == 0) {
                deleteButton.Visible = false;
                currentDiagramBlock = new Diagramblock();
                currentDiagramBlock.idDiagramBlock = 0;
                currentDiagramBlock.extendedTo = 0;
                currentDiagramBlock.diagramPage = currentDiagramPage.idDiagramPage;
                currentDiagramBlock.diagramRow = diagramRow;
                currentDiagramBlock.diagramColumn = diagramColumn;
                currentDiagramBlock.title = "";
                currentDiagramBlock.symbol = "";
                currentDiagramBlock.feature = 0;
                currentDiagramBlock.inputMode = 0;
                currentDiagramBlock.outputMode = 0;
                currentDiagramBlock.cardSlot = 0;
                currentDiagramBlock.eco = 0;
                currentDiagramBlock.cardType = 0;
                currentDiagramBlock.blockConfiguration = "";
                currentDiagramBlock.notes = "";
                currentDiagramBlock.flipped = 0;

                if(cardLocation != null ) {
                    currentDiagramBlock.cardSlot = cardLocation.cardSlot;
                    currentDiagramBlock.cardType = cardLocation.type;
                    currentDiagramBlock.feature = cardLocation.feature;

                    //  Set the default input and output modes (logic levels) based
                    //  on the first gate in the card...

                    List<Cardgate> cardGateList = cardGateTable.getWhere(
                        "WHERE cardType='" + currentDiagramBlock.cardType + "'" +
                        " ORDER BY cardgate.number");
                    if(cardGateList.Count > 0) {
                        currentDiagramBlock.inputMode = cardGateList[0].inputLevel;
                        currentDiagramBlock.outputMode = cardGateList[0].outputLevel;
                    }
                }
            }
            else {
                deleteButton.Visible = true;
            }

            //  Get the card slot info, if available.  (Or blanks/zero if not)

            currentCardSlotInfo = Helpers.getCardSlotInfo(currentDiagramBlock.cardSlot);
            if(currentCardSlotInfo.column == 0) {
                currentCardSlotInfo.column = 1;
            }
            if(currentCardSlotInfo.row == "") {
                currentCardSlotInfo.row = "A";
            }

            //  If we have existing slot machine info, use it.  Otherwise use the diagram
            //  machine.

            if(currentCardSlotInfo.machineName.Length > 0) {
                currentMachine = machineList.Find(
                    x => x.name == currentCardSlotInfo.machineName);
                machineComboBox.SelectedItem = currentMachine;
            }
            else {
                currentMachine = machine;
            }

            //  Populate the rest of the dialog, in hierarchical order.

            populateFrameComboBox();
            populateDialog();
            populatingDialog = false;
            drawLogicbox();            
        }

        //  Method to populate combo boxes that depend on the selections
        //  in other combo boxes.

        void populateFrameComboBox() {

            List<Frame> frameList = frameTable.getWhere(
                "WHERE machine='" + currentMachine.idMachine + "'" +
                " ORDER BY frame.name");

            //  If there are no frames, then we cannot proceed...
            if (frameList.Count == 0) {
                return;
            }
            frameComboBox.DataSource = frameList;
            //  Select the matching entry, if possible...
            if (currentCardSlotInfo.frameName.Length > 0) {
                frameComboBox.SelectedItem = frameList.Find(
                    x => x.name == currentCardSlotInfo.frameName);
            }
            else {
                frameComboBox.SelectedItem = frameList[0];
            }
            currentFrame = (Frame)frameComboBox.SelectedItem;
            //  Then on to the gate and the rest of the dialog...
            populateMachineGateComboBox();
        }


        //  Populate the (Machine) gate combo box...

        void populateMachineGateComboBox() {

            List<Machinegate> machineGateList = machineGateTable.getWhere(
                "WHERE frame='" + currentFrame.idFrame + "'" +
                " ORDER BY machinegate.name");
            //  If there are no gates, we cannot proceed...
            if(machineGateList.Count == 0) {
                return;
            }
            gateComboBox.DataSource = machineGateList;
            //  Select the matching entry, if possible...
            if(currentCardSlotInfo.gateName.Length > 0) {
                gateComboBox.SelectedItem = machineGateList.Find(
                    x => x.name == currentCardSlotInfo.gateName);
            }
            else {
                gateComboBox.SelectedItem = machineGateList[0];
            }
            currentMachineGate = (Machinegate) gateComboBox.SelectedItem;
            //  Then on to the Panel and the rest...
            populatePanelComboBox();
        }

        
        void populatePanelComboBox() {

            List<Panel> panelList = panelTable.getWhere(
                "WHERE gate='" + currentMachineGate.idGate + "'" +
                " ORDER BY panel");
            //  If there are no panels, we cannot proceed...
            if (panelList.Count == 0) {
                return;
            }
            panelComboBox.DataSource = panelList;
            //  Select the matching entry, if possible.
            if (currentCardSlotInfo.panelName.Length > 0) {
                panelComboBox.SelectedItem = panelList.Find(
                    x => x.panel == currentCardSlotInfo.panelName);
            }
            else {
                panelComboBox.SelectedItem = panelList[0];
            }
            currentPanel = (Panel)panelComboBox.SelectedItem;

        }

        void populateCardGateComboBox(Cardtype cardType) {

            cardGateComboBox.Items.Clear();
            cardGateList = cardGateTable.getWhere(
                "WHERE cardType='" + cardType.idCardType + "'" +
                " ORDER BY cardgate.number");

            //  Insert a "null" card gate - we don't really want to set
            //  a gate by default - user action required.

            Cardgate dummyGate = new Cardgate();
            dummyGate.idcardGate = 0;
            dummyGate.logicFunction = 0;
            cardGateList.Insert(0,dummyGate);

            foreach(Cardgate cardGate in cardGateList) {
                string comboBoxItem = "";
                bool firstPin = true;
                List<Gatepin> gatePinList = gatePinTable.getWhere(
                    "WHERE cardGate='" + cardGate.idcardGate + "'" +
                    " ORDER BY pin");
                if (cardGate.idcardGate > 0) {
                    comboBoxItem = cardGate.number.ToString() + ": ";
                }
                else {
                    comboBoxItem = "(NONE)";  //  Dummy gate.
                }
                foreach(Gatepin pin in gatePinList) {
                    if(firstPin) {
                        firstPin = false;
                    }
                    else {
                        comboBoxItem += ",";
                    }
                    comboBoxItem += pin.pin;
                }
                Logicfunction logicFunction =
                    logicFunctionList.Find(x => x.idLogicFunction == cardGate.logicFunction);
                if (logicFunction != null) {
                    comboBoxItem += " (" + logicFunction.name + ")";
                }
                cardGateComboBox.Items.Add(comboBoxItem);
            }
        }

        void populateDialog() {

            Feature currentFeature = null;
            Diagramecotag currentEcoTag = null;
            Logiclevels templevel = null;

            int index;

            populatingDialog = true;

            diagramBlockTitleTextBox.Text = currentDiagramBlock.title;
            symbolTextBox.Text = currentDiagramBlock.symbol;
            flippedCheckBox.Checked = (currentDiagramBlock.flipped == 1);
            blockConfigurationTextBox.Text = currentDiagramBlock.blockConfiguration;
            notesTextBox.Text =
                currentDiagramBlock.notes != null ? currentDiagramBlock.notes : "";                

            if (currentDiagramBlock.feature != 0) {
                currentFeature = featureList.Find(
                    x => x.idFeature == currentDiagramBlock.feature);
                featureComboBox.SelectedItem = currentFeature;
            }
            if(currentFeature == null) {
                featureComboBox.SelectedItem = currentFeature = featureList[0];
            }

            if(currentDiagramBlock.eco != 0) {
                currentEcoTag = ecoTagList.Find(
                    x => x.idDiagramECOTag == currentDiagramBlock.eco);
                ecoTagComboBox.SelectedItem = currentEcoTag;
            }

            //  If there is no existing eco tag, select the first "real" entry

            if(currentEcoTag == null) {
                ecoTagComboBox.SelectedItem = currentEcoTag = 
                    ecoTagList[ecoTagList.Count > 1 ? 1 : 0];
            }

            templevel = null;
            if(currentDiagramBlock.inputMode != 0) {
                templevel = inputLogicLevelList.Find(
                    x => x.idLogicLevels == currentDiagramBlock.inputMode);
                inputModeComboBox.SelectedItem = templevel;
            }            
            if(templevel == null) {
                inputModeComboBox.SelectedItem = inputLogicLevelList[0];
            }

            templevel = null;
            if (currentDiagramBlock.outputMode != 0) {
                templevel = outputLogicLevelList.Find(
                    x => x.idLogicLevels == currentDiagramBlock.outputMode);
                outputModeComboBox.SelectedItem = templevel;
            }
            if (templevel == null) {
                outputModeComboBox.SelectedItem = inputLogicLevelList[0];
            }

            index = Array.IndexOf(Helpers.validRows, currentCardSlotInfo.row);
            if(index < 0) {
                index = 0;
            }
            cardRowComboBox.SelectedIndex = index;

            cardColumnTextBox.Text = currentCardSlotInfo.column.ToString("D2");

            if(currentDiagramBlock.cardType != 0) {
                currentCardType = cardTypeTable.getByKey(currentDiagramBlock.cardType);
                cardTypeComboBox.SelectedItem = cardTypeList.Find(
                    x => x.type == currentCardType.type);
            }
            else {
                cardTypeComboBox.SelectedItem = currentCardType = cardTypeList[0];
            }

            populateCardGateComboBox(currentCardType);
            if(currentDiagramBlock.cardGate != 0) {
                cardGateComboBox.SelectedIndex = 
                    cardGateList.FindIndex(x => x.idcardGate == currentDiagramBlock.cardGate);
            }
            else if(cardGateList.Count > 0) {
                cardGateComboBox.SelectedIndex = 0;
            }

            if(currentDiagramBlock.extendedTo != 0) {
                extendedCheckBox.Checked = true;
                Diagramblock extendedBlock = diagramBlockTable.getByKey(
                    currentDiagramBlock.extendedTo);
                if(extendedBlock != null || extendedBlock.idDiagramBlock != 0) {
                    if(currentDiagramBlock.diagramRow.CompareTo(extendedBlock.diagramRow) > 0) {
                        extendedAboveRadioButton.Checked = true;
                    }
                    else {
                        extendedBelowRadioButton.Checked = true;
                    }
                }
                else {
                    //  If the key doesn't match, erase the extended flag...
                    extendedCheckBox.Checked = false;
                }
            }
            else {
                extendedCheckBox.Checked = false;
            }
            if(!extendedCheckBox.Checked) {
                extendedAboveRadioButton.Checked = false;
                extendedBelowRadioButton.Checked = false;
            }

            populatingDialog = false;

        }        

        private void drawLogicbox() {

            //  Create aliases to save keystrokes.   ;)

            // Diagramblock block = currentDiagramBlock;
            string s = "";
            int tabLen = 6;
            int width = 4;
            char underscore = '_';
            char upperscore = '¯';
            char bar = '|';
            string tab = new string(' ', tabLen);

            //  Don't do anything if we are not ready...

            if(populatingDialog) {
                return;
            }

            string inLevel = ((Logiclevels)inputModeComboBox.SelectedItem).logicLevel;
            string outLevel = ((Logiclevels)outputModeComboBox.SelectedItem).logicLevel;
            string machineSuffix = ((Machine)machineComboBox.SelectedItem).name;
            string cardType = ((Cardtype)cardTypeComboBox.SelectedItem).type;

            machineSuffix = machineSuffix.Length >= 4 ? machineSuffix.Substring(2, 2) : "??";

            int column;
            int.TryParse(cardColumnTextBox.Text, out column);            

            List<Connection> connectionList = connectionTable.getWhere(
                "WHERE fromDiagramBlock='" + currentDiagramBlock.idDiagramBlock + "'");

            string polarity = connectionList.Count > 0 ? connectionList[0].fromPhasePolarity 
                : "";


            for (int i=0; i < 1; ++i) {
                s += Environment.NewLine;
            }

            //if(block.title != null && block.title.Length > 0) {
            //    s += new string(' ', tabLen + width-1 - block.title.Length / 2) +
            //        block.title;
            //}

            s += new string(' ', tabLen + width-1 - diagramBlockTitleTextBox.Text.Length / 2) +
                diagramBlockTitleTextBox.Text.ToUpper() + Environment.NewLine;

            s += tab + new string(underscore, width + 2) + Environment.NewLine;

            //s += tab + bar + new string(' ', width-2 - (block.symbol.Length + 1) / 2) +
            //    block.symbol + new string(' ', width-2 - (block.symbol.Length) / 2) 
            //    + bar + Environment.NewLine;

            s += tab + bar + new string(' ', width-2 - (symbolTextBox.Text.Length + 1) / 2) +
                symbolTextBox.Text.ToUpper() + 
                new string(' ', width-2 - symbolTextBox.Text.Length/2) +
                bar + 
                Environment.NewLine;

            s += tab + bar + ((Feature)featureComboBox.SelectedItem).code + 
                new string(' ', width - ((Feature)featureComboBox.SelectedItem).code.Length) +
                bar + (polarity == "+" ? "--" : "") + Environment.NewLine;

            s += tab + bar +
                (inLevel.Length > 1 ? inLevel.Substring(0,2) : 
                    (inLevel.Length > 0 ? inLevel.Substring(0, 1) : " ") + " ") +
                " " + (outLevel.Length > 0 ? outLevel.Substring(0, 1) : " ") +
                bar + Environment.NewLine;

            s += tab + bar + machineSuffix + currentMachineGate.name +
                ((Diagramecotag) ecoTagComboBox.SelectedItem).name + 
                bar + Environment.NewLine;

            s += tab + bar + currentPanel.panel.ToString().Substring(0, 1) +
                cardRowComboBox.SelectedItem + column.ToString("D2") +
                bar + 
                (polarity == "-" ? "--" : "") +
                Environment.NewLine;

            s += tab + bar + cardType +
                (cardType.Length < 4 ? new string(' ', 4 - cardType.Length) : "") +
                bar + Environment.NewLine;

            s += tab;
            if (blockConfigurationTextBox.Text.Length > 0) {
                s += new string(upperscore,2) + blockConfigurationTextBox.Text +
                    new string(upperscore, 4 - blockConfigurationTextBox.Text.Length);
            }
            else {
                s += new string(upperscore, width + 2);
            }
            s += Environment.NewLine;

            //s += tab + "  " + block.diagramColumn.ToString("D1") +
            //    block.diagramRow + Environment.NewLine;

            s += tab + "  " +
                diagramColumnTextBox.Text + diagramRowTextBox.Text + Environment.NewLine;

            logicBlockDrawingTextBox.Text = s;
        }


        //  Combo Box selection methods.


        private void machineComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (!populatingDialog) {
                currentMachine = (Machine)machineComboBox.SelectedItem;
                if(currentCardSlotInfo.machineName != currentMachine.name) {
                    currentCardSlotInfo.machineName = currentMachine.name;
                    modifiedMachineGatePanelFrame = true;
                }
                populateFrameComboBox();
            }
        }

        private void frameComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (!populatingDialog) {
                currentFrame = (Frame)frameComboBox.SelectedItem;
                if(currentCardSlotInfo.frameName != currentFrame.name) {
                    currentCardSlotInfo.frameName = currentFrame.name;
                    modifiedMachineGatePanelFrame = true;
                }
                populateMachineGateComboBox();
            }
        }

        private void gateComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (!populatingDialog) {
                currentMachineGate = (Machinegate)gateComboBox.SelectedItem;
                if(currentCardSlotInfo.gateName != currentMachineGate.name) {
                    currentCardSlotInfo.gateName = currentMachineGate.name;
                    modifiedMachineGatePanelFrame = true;
                }
                populatePanelComboBox();
            }
        }

        private void panelComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if(!populatingDialog) { 
                currentPanel = (Panel)panelComboBox.SelectedItem;
                if(currentCardSlotInfo.panelName != currentPanel.panel) {
                    currentCardSlotInfo.panelName = currentPanel.panel;
                    modifiedMachineGatePanelFrame = true;
                }
                drawLogicbox();
            }
        }

        private void featureComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            drawLogicbox();
        }

        private void diagramBlockTitleTextBox_TextChanged(object sender, EventArgs e) {
            drawLogicbox();
        }

        private void symbolTextBox_TextChanged(object sender, EventArgs e) {
            drawLogicbox();
        }

        private void inputModeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            drawLogicbox();
        }

        private void outputModeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            drawLogicbox();
        }

        private void ecoTagComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            drawLogicbox();
        }

        private void cardRowComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            drawLogicbox();
        }

        private void cardColumnTextBox_TextChanged(object sender, EventArgs e) {
            int v;
            if (cardColumnTextBox.Text.Length > 0 &&
                !int.TryParse(cardColumnTextBox.Text,out v)) {
                MessageBox.Show("Card Column must be numeric!", "Invalid Card Column");
                cardColumnTextBox.Text = currentCardSlotInfo.column.ToString("D2");
            }
            drawLogicbox();
        }

        private void cardTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            populateCardGateComboBox((Cardtype)cardTypeComboBox.SelectedItem);
            if (!populatingDialog && cardGateList.Count > 0) {
                cardGateComboBox.SelectedIndex = 0;
            }
            drawLogicbox();
        }

        private void blockConfigurationTextBox_TextChanged(object sender, EventArgs e) {
            drawLogicbox();
        }

        private void applyButton_Click(object sender, EventArgs e) {

            //  Time to insert/update the Logic Block

            int column = 0;
            string message = "";
            applySuccessful = false;

            if(cardColumnTextBox.Text == null || cardColumnTextBox.Text.Length == 0 ||
                !int.TryParse(cardColumnTextBox.Text, out column) || 
                column < 1 || column > 99) {
                MessageBox.Show("Card Column must be present, and be 1-99",
                    "Invalid Card Column",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                cardColumnTextBox.Focus();
                return;
            }

            if (symbolTextBox.Text == null || symbolTextBox.Text.Length == 0) {
                MessageBox.Show("Logic Block Symbol is required.",
                    "Symbol Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                symbolTextBox.Focus();
                return;
            }

            //  If the extended check box is checked, check that it makes sense..

            if(extendedCheckBox.Checked == true) {
                if ((extendedAboveRadioButton.Checked &&
                    diagramRowTextBox.Text == Helpers.validDiagramRows[0]) ||
                    (extendedBelowRadioButton.Checked &&
                    diagramRowTextBox.Text ==
                        Helpers.validDiagramRows[Helpers.validDiagramRows.Length - 1])) {
                    MessageBox.Show("Logic block in top row cannot be extended up /\n" +
                        "bottom row cannot be extended down",
                        "Invalid Logic Block Extension",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            //  Update the card slot info from the dialog

            currentCardSlotInfo.machineName = ((Machine)machineComboBox.SelectedItem).name;
            currentCardSlotInfo.frameName = ((Frame)frameComboBox.SelectedItem).name;
            currentCardSlotInfo.gateName = ((Machinegate)gateComboBox.SelectedItem).name;
            currentCardSlotInfo.panelName = ((Panel)panelComboBox.SelectedItem).panel;
            currentCardSlotInfo.row = (string)cardRowComboBox.SelectedItem;
            currentCardSlotInfo.column = column;

            //  Also update some fields of the current diagram block from the dialog now.

            currentDiagramBlock.title = diagramBlockTitleTextBox.Text.ToUpper();
            currentDiagramBlock.symbol = symbolTextBox.Text.ToUpper();
            currentDiagramBlock.feature = ((Feature)featureComboBox.SelectedItem).idFeature;
            currentDiagramBlock.inputMode = ((Logiclevels)inputModeComboBox.SelectedItem).idLogicLevels;
            currentDiagramBlock.outputMode = ((Logiclevels)outputModeComboBox.SelectedItem).idLogicLevels;
            currentDiagramBlock.eco = ((Diagramecotag)ecoTagComboBox.SelectedItem).idDiagramECOTag;
            currentDiagramBlock.cardType = ((Cardtype)cardTypeComboBox.SelectedItem).idCardType;
            if (cardGateList.Count > 0) {
                currentDiagramBlock.cardGate = cardGateList[cardGateComboBox.SelectedIndex].idcardGate;
            }
            else {
                currentDiagramBlock.cardGate = 0;
            }
            currentDiagramBlock.blockConfiguration = blockConfigurationTextBox.Text;
            currentDiagramBlock.notes = notesTextBox.Text;
            currentDiagramBlock.flipped = flippedCheckBox.Checked ? 1 : 0;

            //  Tell the user what the update will actually do...

            message = doUpdate(false);
            DialogResult result = MessageBox.Show(
                "Confirm you wish to apply the following: \n\n" + message,
                "Confirm Adds/Updates",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            if(result == DialogResult.OK) {
                message = doUpdate(true);
                MessageBox.Show(message,"Adds/Updates applied.",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                applySuccessful = true;
                this.Close();
            }
        }

        //  Method to handle update confirmation message construction, and actual updates.

        string doUpdate(bool updating) {

            string message = "";
            string tempMessage = "";

            string action = updating ? "Added" : "Adding";
            string updateAction = updating ? "Updated" : "Updating";

            int extensionKey = 0;
            int diagramBlockKey = currentDiagramBlock.idDiagramBlock;

            if (updating) {
                db.BeginTransaction();

                //  Get the new database key now, if we will need it, but don't overwrite
                //  the 0 just yet, so we know later if we are inserting or not.

                if(diagramBlockKey == 0) {
                    diagramBlockKey = IdCounter.incrementCounter();
                }

            }

            message = (currentDiagramBlock.idDiagramBlock != 0 ? updateAction : action) +
                " Logic block " +
                (currentDiagramBlock.idDiagramBlock != 0 ?
                "(Database ID " + currentDiagramBlock.idDiagramBlock + ")" : "") + "\n\n";

            //  Add the card slot, if necessary.

            currentDiagramBlock.cardSlot = 
                Helpers.getOrAddCardSlotKey(updating, currentCardSlotInfo, out tempMessage);            

            //  Handle an extended logic block...

            if (extendedCheckBox.Checked) {
                string extensionRow = Helpers.validDiagramRows[
                        extendedAboveRadioButton.Checked ?
                            Array.IndexOf(Helpers.validDiagramRows, diagramRowTextBox.Text) - 1 :
                            Array.IndexOf(Helpers.validDiagramRows, diagramRowTextBox.Text) + 1];
                List<Diagramblock> extensionList = diagramBlockTable.getWhere(
                    "WHERE diagramPage='" + currentDiagramPage.idDiagramPage + "'" +
                    " AND diagramColumn='" + currentDiagramBlock.diagramColumn + "'" +
                    " AND diagramRow='" + extensionRow + "'");
                if(extensionList.Count < 1) {
                    //  Need to add the extension logic block as a clone of this one...
                    if (updating) {
                        Diagramblock extensionBlock = new Diagramblock();
                        extensionBlock.idDiagramBlock = IdCounter.incrementCounter();
                        extensionKey = extensionBlock.idDiagramBlock;
                        extensionBlock.extendedTo = diagramBlockKey;
                        extensionBlock.diagramPage = currentDiagramBlock.diagramPage;
                        extensionBlock.diagramRow = extensionRow;
                        extensionBlock.diagramColumn = currentDiagramBlock.diagramColumn;
                        extensionBlock.title = currentDiagramBlock.title;
                        extensionBlock.symbol =
                            extendedAboveRadioButton.Checked ? currentDiagramBlock.symbol : "E";
                        extensionBlock.feature = currentDiagramBlock.feature;
                        extensionBlock.inputMode = currentDiagramBlock.inputMode;
                        extensionBlock.outputMode = currentDiagramBlock.outputMode;
                        extensionBlock.cardSlot = currentDiagramBlock.cardSlot;
                        extensionBlock.eco = currentDiagramBlock.eco;
                        extensionBlock.cardType = currentDiagramBlock.cardType;
                        extensionBlock.blockConfiguration = currentDiagramBlock.blockConfiguration;
                        extensionBlock.notes = "";
                        extensionBlock.flipped = currentDiagramBlock.flipped;
                        diagramBlockTable.insert(extensionBlock);
                    }
                    message += action + 
                        " Extension Diagram Logic Block at row " + extensionRow + "\n";
                }
                else {
                    extensionKey = extensionList[0].idDiagramBlock;
                    if(extensionList[0].extendedTo == 0) {
                        message += (updating ? "Set" : "Setting") +
                            " extendedTo field in Diagram Logic Block at row " +
                            extensionRow + " (Database ID " + extensionList[0].idDiagramBlock + ")\n";
                        if (updating) {
                            extensionList[0].extendedTo = diagramBlockKey;
                            diagramBlockTable.update(extensionList[0]);
                        }
                    }
                }
            }

            //  Set the extension key (or 0) into the diagram block

            currentDiagramBlock.extendedTo = extensionKey;

            if(updating) {
                if(currentDiagramBlock.idDiagramBlock == 0) {
                    currentDiagramBlock.idDiagramBlock = diagramBlockKey;
                    diagramBlockTable.insert(currentDiagramBlock);
                    message += "New Logic Block Database ID=" + currentDiagramBlock.idDiagramBlock;
                }
                else {
                    diagramBlockTable.update(currentDiagramBlock);
                }
                db.CommitTransaction();
                modifiedMachineGatePanelFrame = false;
            }

            return (message);
        }

        private void deleteButton_Click(object sender, EventArgs e) {

            string message = "";

            List<Connection> connectionList = connectionTable.getWhere(
                "WHERE fromDiagramBlock='" + currentDiagramBlock.idDiagramBlock + "'" +
                " OR toDiagramBlock='" + currentDiagramBlock.idDiagramBlock + "'");
            if(connectionList.Count > 0) {
                message += "There are currently " +
                    connectionList.Count + " connections still referring to it.\n";
            }

            List<Diagramblock> extensionList = diagramBlockTable.getWhere(
                "WHERE extendedTo='" + currentDiagramBlock.idDiagramBlock + "'");
            if(extensionList.Count > 0) {
                message += "There is currently an extension reference to it.\n";
            }

            if(message.Length > 0) {
                message = "Cannot delete Diagram Logic Block with Database ID=" +
                    currentDiagramBlock.idDiagramBlock + ":\n\n" + message;
                MessageBox.Show(message, "Cannot Delete Diagram Logic Block",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            else {
                DialogResult result = MessageBox.Show(
                    "Confirm Deletion of Diagram Logic Block with Database ID=" +
                    currentDiagramBlock.idDiagramBlock, "Confirm Deletion",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.OK) {
                    diagramBlockTable.deleteByKey(currentDiagramBlock.idDiagramBlock);
                    MessageBox.Show("Diagram Logic Block with Database ID=" +
                        currentDiagramBlock.idDiagramBlock + " Deleted.",
                        "Diagram Logic Block Deleted",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            //  Buh bye...
            this.Close();        
        }

        private void editConnectionsButton_Click(object sender, EventArgs e) {

            //  If something changed, apply the update.

            if (isModified()) {
                applyButton_Click(sender, e);
                //  If the apply failed, do NOT bring up the connections dialog.
                if(!applySuccessful) {
                    return;
                }
            }

            //  Bring up the connections dialog.

            EditConnectionsForm EditConnectionsForm = new EditConnectionsForm(
                currentDiagramBlock, diagramMachine, currentVolumeSet, currentVolume,
                currentPage, currentDiagramPage, currentCardType);
            EditConnectionsForm.ShowDialog();

            //  Redraw the logic box, as outputs may have changed.

            drawLogicbox();
        }


        //  Method to see if the diagram block was modified, or not.  Did this rather than
        //  trying to track all of the individual changes as they happen.  I took one
        //  "shortcut" - this method does NOT check to see if the only thing that changed
        //  was the direction of an Extension.  (The regular apply button updates, regardless).

        private bool isModified() {

            int column;

            int.TryParse(cardColumnTextBox.Text, out column);

            return (modifiedMachineGatePanelFrame ||
                currentDiagramBlock.idDiagramBlock == 0 ||
                currentCardSlotInfo.machineName != ((Machine)machineComboBox.SelectedItem).name ||
                currentCardSlotInfo.frameName != ((Frame)frameComboBox.SelectedItem).name ||
                currentCardSlotInfo.gateName != ((Machinegate)gateComboBox.SelectedItem).name ||
                currentCardSlotInfo.panelName != ((Panel)panelComboBox.SelectedItem).panel ||
                currentCardSlotInfo.row != (string)cardRowComboBox.SelectedItem ||
                currentCardSlotInfo.column != column ||
                (currentDiagramBlock.extendedTo != 0 && !extendedCheckBox.Checked) ||
                (currentDiagramBlock.extendedTo == 0 && extendedCheckBox.Checked) ||
                currentDiagramBlock.title != diagramBlockTitleTextBox.Text ||
                currentDiagramBlock.symbol != symbolTextBox.Text ||
                currentDiagramBlock.feature != ((Feature)featureComboBox.SelectedItem).idFeature ||
                currentDiagramBlock.inputMode != ((Logiclevels)inputModeComboBox.SelectedItem).idLogicLevels ||
                currentDiagramBlock.outputMode != ((Logiclevels)outputModeComboBox.SelectedItem).idLogicLevels ||
                currentDiagramBlock.eco != ((Diagramecotag)ecoTagComboBox.SelectedItem).idDiagramECOTag ||
                currentDiagramBlock.cardType != ((Cardtype)cardTypeComboBox.SelectedItem).idCardType ||
                currentDiagramBlock.cardGate != cardGateList[cardGateComboBox.SelectedIndex].idcardGate ||
                currentDiagramBlock.blockConfiguration != blockConfigurationTextBox.Text ||
                currentDiagramBlock.notes != notesTextBox.Text
                );
        }
    }
}
