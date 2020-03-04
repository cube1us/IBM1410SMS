﻿/* 
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
        Table<Cableedgeconnectionblock> cableEdgeConnectionBlockTable;
        Table<Cableedgeconnectionecotag> cableEdgeConnectionEcoTagTable;
        Table<Machine> machineTable;
        Table<Cardtype> cardTypeTable;
        Table<Frame> frameTable;
        Table<Machinegate> machineGateTable;
        Table<Panel> panelTable;
        Table<Cardgate> cardGateTable;

        Page currentPage;
        Cableedgeconnectionpage currentCableEdgeConnectionPage;
        Machine currentMachine;
        Machine cableEdgeConnectionMachine;
        Volumeset currentVolumeSet;
        Volume currentVolume;
        Frame currentFrame = null;
        Machinegate currentMachineGate = null;
        Panel currentPanel = null;
        Cardtype currentCardType = null;

        Cableedgeconnectionblock currentCableEdgeConnectionBlock;
        CardSlotInfo currentCardSlotInfo = null;

        List<Cableedgeconnectionecotag> ecoTagList;
        List<Machine> machineList;
        List<Cardtype> cardTypeList;
        List<Cardgate> cardGateList;

        string machinePrefix;
        bool populatingDialog = true;
        bool applySuccessful = false;
        bool modifiedMachineGatePanelFrame = false;

        public EditCableEdgeConnectionBlockForm(
            Cableedgeconnectionblock cableEdgeConnectionBlock,
            Machine machine,
            Volumeset volumeSet,
            Volume volume,
            Cableedgeconnectionpage cableEdgeConnectionPage,
            string diagramRow, 
            int diagramColumn,
            Cardlocation cardLocation) {

            InitializeComponent();

            pageTable = db.getPageTable();
            cableEdgeConnectionBlockTable = db.getCableEdgeConnectionBlockTable();
            cableEdgeConnectionEcoTagTable = db.getCableEdgeConnectionECOTagTable();
            cardTypeTable = db.getCardTypeTable();
            machineTable = db.getMachineTable();
            panelTable = db.getPanelTable();
            machineGateTable = db.getMachineGateTable();
            frameTable = db.getFrameTable();
            cardGateTable = db.getCardGateTable();

            cableEdgeConnectionMachine = machine;

            //  Set up invariant lists...

            machineList = machineTable.getWhere(
                "WHERE machine.name LIKE '" + machinePrefix + "%' ORDER BY machine.name");

            if (machineList.Count == 0) {
                MessageBox.Show("No Machines Defined, cannot proceed.",
                    "No Machines Defined",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.Close();
            }

            ecoTagList = cableEdgeConnectionEcoTagTable.getWhere(
                "WHERE cableEdgeConnectionPage='" + 
                cableEdgeConnectionPage.idCableEdgeConnectionPage + "'" +
                " ORDER BY cableEdgeConnectionECOTag.name");
            //  ECO Tag also needs an empty entry...
            Cableedgeconnectionecotag emptyTag = new Cableedgeconnectionecotag();
            emptyTag.idcableEdgeConnectionECOtag = 0;
            emptyTag.name = " ";
            ecoTagList.Insert(0, emptyTag);

            cardTypeList = cardTypeTable.getWhere("ORDER BY cardtype.type");
            cardTypeList = cardTypeList.FindAll(
                x => Array.IndexOf(Helpers.cableEdgeConnectionCardTypes, x) >= 0);

            //  Fill in static combo boxes' data sources.

            ecoTagComboBox.DataSource = ecoTagList;
            machineComboBox.DataSource = machineList;
            cardTypeComboBox.DataSource = cardTypeList;

            //  Fill in constant data.

            currentVolumeSet = volumeSet;
            currentVolume = volume;
            currentPage = pageTable.getByKey(cableEdgeConnectionPage.page);
            currentCableEdgeConnectionPage = cableEdgeConnectionPage;
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

            currentCableEdgeConnectionBlock = cableEdgeConnectionBlock;
            if(currentCableEdgeConnectionBlock == null || 
                currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock == 0) {
                deleteButton.Visible = false;
                currentCableEdgeConnectionBlock = new Cableedgeconnectionblock();
                currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock = 0;
                currentCableEdgeConnectionBlock.cableEdgeConnectionPage = 
                currentCableEdgeConnectionPage.idCableEdgeConnectionPage;
                currentCableEdgeConnectionBlock.diagramRow = diagramRow;
                currentCableEdgeConnectionBlock.diagramColumn = diagramColumn;
                currentCableEdgeConnectionBlock.topNote = "";
                currentCableEdgeConnectionBlock.cardSlot = 0;
                currentCableEdgeConnectionBlock.ecotag = 0;
                currentCableEdgeConnectionBlock.originNote = "";
                currentCableEdgeConnectionBlock.destNote = "";
                currentCableEdgeConnectionBlock.explicitDestination = 0;
                currentCableEdgeConnectionBlock.impliedDestination = 0;

                if(cardLocation != null ) {
                    currentCableEdgeConnectionBlock.cardSlot = cardLocation.cardSlot;
                }
            }
            else {
                deleteButton.Visible = true;
            }

            //  Get the card slot info, if available.  (Or blanks/zero if not)

            currentCardSlotInfo = Helpers.getCardSlotInfo(currentCableEdgeConnectionBlock.cardSlot);
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

        /*
         *  REMOVE METHOD
         *  
         * 
         * 
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

        END REMOVED METHOD  */

        void populateDialog() {

            Cableedgeconnectionecotag currentEcoTag = null;
            Logiclevels templevel = null;

            int index;

            populatingDialog = true;

            cableEdgeConnectionBlockTitleTextBox.Text = currentCableEdgeConnectionBlock.topNote;

            if(currentCableEdgeConnectionBlock.ecotag != 0) {
                currentEcoTag = ecoTagList.Find(
                    x => x.idcableEdgeConnectionECOtag == currentCableEdgeConnectionBlock.ecotag);
                ecoTagComboBox.SelectedItem = currentEcoTag;
            }

            //  If there is no existing eco tag, select the first "real" entry

            if(currentEcoTag == null) {
                ecoTagComboBox.SelectedItem = currentEcoTag = 
                    ecoTagList[ecoTagList.Count > 1 ? 1 : 0];
            }

            index = Array.IndexOf(Helpers.validRows, currentCardSlotInfo.row);
            if(index < 0) {
                index = 0;
            }
            cardRowComboBox.SelectedIndex = index;

            cardColumnTextBox.Text = currentCardSlotInfo.column.ToString("D2");

            //  TODO - get type from card slot

            if(currentCableEdgeConnectionBlock.cardType != 0) {
                currentCardType = cardTypeTable.getByKey(currentCableEdgeConnectionBlock.cardType);
                cardTypeComboBox.SelectedItem = cardTypeList.Find(
                    x => x.type == currentCardType.type);
            }
            else {
                cardTypeComboBox.SelectedItem = currentCardType = cardTypeList[0];
            }

            populatingDialog = false;

        }        

        private void drawLogicbox() {

            //  TODO: Add relvant drawing sutff.

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

            string machineSuffix = ((Machine)machineComboBox.SelectedItem).name;
            string cardType = ((Cardtype)cardTypeComboBox.SelectedItem).type;

            machineSuffix = machineSuffix.Length >= 4 ? machineSuffix.Substring(2, 2) : "??";

            int column;
            int.TryParse(cardColumnTextBox.Text, out column);            

            for (int i=0; i < 1; ++i) {
                s += Environment.NewLine;
            }

            s += new string(' ', tabLen + width-1 - cableEdgeConnectionBlockTitleTextBox.Text.Length / 2) +
                cableEdgeConnectionBlockTitleTextBox.Text.ToUpper() + Environment.NewLine;

            s += tab + new string(underscore, width + 2) + Environment.NewLine;

            /*
             * Sample
            s += tab + bar + new string(' ', width-2 - (symbolTextBox.Text.Length + 1) / 2) +
                symbolTextBox.Text.ToUpper() + 
                new string(' ', width-2 - symbolTextBox.Text.Length/2) +
                bar + 
                Environment.NewLine;
            */

            s += tab + bar + machineSuffix + currentMachineGate.name +
                ((Diagramecotag) ecoTagComboBox.SelectedItem).name + 
                bar + Environment.NewLine;

            s += tab + bar + currentPanel.panel.ToString().Substring(0, 1) +
                cardRowComboBox.SelectedItem + column.ToString("D2") +
                bar +
                Environment.NewLine;

            s += tab + bar + cardType +
                (cardType.Length < 4 ? new string(' ', 4 - cardType.Length) : "") +
                bar + Environment.NewLine;

            s += tab;
            s += new string(upperscore, width + 2);
            s += Environment.NewLine;

            s += tab + "  " +
                diagramColumnTextBox.Text + diagramRowTextBox.Text + Environment.NewLine;

            cableEdgeConnectionBlockDrawingTextBox.Text = s;
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

        private void cableEdgeConnectionBlockTitleTextBox_TextChanged(object sender, EventArgs e) {
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

        //  TODO - fix

        private void cardTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            populateCardGateComboBox((Cardtype)cardTypeComboBox.SelectedItem);
            if (!populatingDialog && cardGateList.Count > 0) {
                cardGateComboBox.SelectedIndex = 0;
            }
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

            //  Update the card slot info from the dialog

            currentCardSlotInfo.machineName = ((Machine)machineComboBox.SelectedItem).name;
            currentCardSlotInfo.frameName = ((Frame)frameComboBox.SelectedItem).name;
            currentCardSlotInfo.gateName = ((Machinegate)gateComboBox.SelectedItem).name;
            currentCardSlotInfo.panelName = ((Panel)panelComboBox.SelectedItem).panel;
            currentCardSlotInfo.row = (string)cardRowComboBox.SelectedItem;
            currentCardSlotInfo.column = column;

            //  Also update some fields of the current diagram block from the dialog now.

            //  TODO: Lots to fix here.

            currentCableEdgeConnectionBlock.topNote = cableEdgeConnectionBlockTitleTextBox.Text.ToUpper();
            currentCableEdgeConnectionBlock.ecotag = ((Diagramecotag)ecoTagComboBox.SelectedItem).idDiagramECOTag;

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
            int diagramBlockKey = currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock;

            if (updating) {
                db.BeginTransaction();

                //  Get the new database key now, if we will need it, but don't overwrite
                //  the 0 just yet, so we know later if we are inserting or not.

                if(diagramBlockKey == 0) {
                    diagramBlockKey = IdCounter.incrementCounter();
                }

            }

            message = (currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock != 0 
                    ? updateAction : action) +
                " Logic block " +
                (currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock != 0 ?
                "(Database ID " + currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock + 
                ")" : "") + "\n\n";

            //  Add the card slot, if necessary.

            currentCableEdgeConnectionBlock.cardSlot = 
                Helpers.getOrAddCardSlotKey(updating, currentCardSlotInfo, out tempMessage);            


            if(updating) {
                if(currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock == 0) {
                    currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock = diagramBlockKey;
                    cableEdgeConnectionBlockTable.insert(currentCableEdgeConnectionBlock);
                    message += "New Logic Block Database ID=" + 
                        currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock;
                }
                else {
                    cableEdgeConnectionBlockTable.update(currentCableEdgeConnectionBlock);
                }
                db.CommitTransaction();
                modifiedMachineGatePanelFrame = false;
            }

            return (message);
        }

        private void deleteButton_Click(object sender, EventArgs e) {

            string message = "";


            if(message.Length > 0) {
                message = "Cannot delete Diagram Logic Block with Database ID=" +
                    currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock + ":\n\n" + message;
                MessageBox.Show(message, "Cannot Delete Diagram Logic Block",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            else {
                DialogResult result = MessageBox.Show(
                    "Confirm Deletion of Diagram Logic Block with Database ID=" +
                    currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock, "Confirm Deletion",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.OK) {
                    cableEdgeConnectionBlockTable.deleteByKey(
                        currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock);
                    MessageBox.Show("Diagram Logic Block with Database ID=" +
                        currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock + " Deleted.",
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


        //  Method to see if the diagram block was modified, or not.  Did this rather than
        //  trying to track all of the individual changes as they happen.  I took one
        //  "shortcut" - this method does NOT check to see if the only thing that changed
        //  was the direction of an Extension.  (The regular apply button updates, regardless).

        private bool isModified() {

            int column;

            int.TryParse(cardColumnTextBox.Text, out column);

            //  TODO:  Add new fields

            return (modifiedMachineGatePanelFrame ||
                currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock == 0 ||
                currentCardSlotInfo.machineName != ((Machine)machineComboBox.SelectedItem).name ||
                currentCardSlotInfo.frameName != ((Frame)frameComboBox.SelectedItem).name ||
                currentCardSlotInfo.gateName != ((Machinegate)gateComboBox.SelectedItem).name ||
                currentCardSlotInfo.panelName != ((Panel)panelComboBox.SelectedItem).panel ||
                currentCardSlotInfo.row != (string)cardRowComboBox.SelectedItem ||
                currentCardSlotInfo.column != column ||
                currentCableEdgeConnectionBlock.topNote != cableEdgeConnectionBlockTitleTextBox.Text ||
                currentCableEdgeConnectionBlock.ecotag != ((Diagramecotag)ecoTagComboBox.SelectedItem).idDiagramECOTag ||
                currentCableEdgeConnectionBlock.cardType != ((Cardtype)cardTypeComboBox.SelectedItem).idCardType
                );
        }
    }
}