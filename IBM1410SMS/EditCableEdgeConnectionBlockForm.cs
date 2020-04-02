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
        Table<Cableedgeconnectionblock> cableEdgeConnectionBlockTable;
        Table<Cableedgeconnectionecotag> cableEdgeConnectionEcoTagTable;
        Table<Machine> machineTable;
        Table<Cardtype> cardTypeTable;
        Table<Frame> frameTable;
        Table<Machinegate> machineGateTable;
        Table<Panel> panelTable;
        Table<Cardgate> cardGateTable;
        Table<Cableimplieddestinations> cableImpliedDestinationsTable;

        Page currentPage;
        Cableedgeconnectionpage currentCableEdgeConnectionPage;
        Machine currentMachine;
        Machine currentDestinationMachine;
        Machine cableEdgeConnectionMachine;
        Volumeset currentVolumeSet;
        Volume currentVolume;
        Frame currentFrame = null;
        Frame currentDestinationFrame = null;
        Machinegate currentMachineGate = null;
        Machinegate currentDestinationMachineGate = null;
        Panel currentPanel = null;
        Panel currentDestinationPanel = null;
        Cardlocation currentCardLocation = null;
        Cardlocation currentDestinationCardLocation = null;

        Cableedgeconnectionblock currentCableEdgeConnectionBlock;
        CardSlotInfo currentCardSlotInfo = null;
        CardSlotInfo currentDestinationCardSlotInfo = null;

        List<Cableedgeconnectionecotag> ecoTagList;
        List<Machine> machineList;
        List<Machine> destinationMachineList;
        List<Cardtype> cardTypeList;

        List<CardSlotInfo> impliedDestinationSources = new List<CardSlotInfo>();
        List<CardSlotInfo> impliedDestinationDestinations = new List<CardSlotInfo>();
        List<Cableimplieddestinations> impliedDestinationsRules = 
            new List<Cableimplieddestinations>();

        char[] impliedDestinationDelimeters = { ',' };
        string[] cableConnectorCardTypes = { "CONN", "CABL" };
        string[] cePanelNames = { "CE", "0" };

        string machinePrefix;
        bool populatingDialog = true;
        bool applySuccessful = false;
        bool modifiedMachineGatePanelFrame = false;
        bool newCableEdgeConnection = false;        // Tried using deleteButton.Visible, failed?

        string sourceFrameLabel = "Frame";
        string sourceGateLabel = "Gate";
        string sourcePanelLabel = "Panel";
        string sourceRowLabel = "Row";
        string sourceColumnLabel = "Column";
        string destFrameLabel = "Frame";
        string destGateLabel = "Gate";
        string destPanelLabel = "Panel";
        string destRowLabel = "Row";
        string destColumnLabel = "Column";

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
            cableImpliedDestinationsTable = db.getCableImpliedDestinationsTable();

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
                x => Array.IndexOf(Helpers.cableEdgeConnectionCardTypes, x.type) >= 0);

            impliedDestinationsRules = cableImpliedDestinationsTable.getAll();

            foreach(Cableimplieddestinations cableImpliedDestinationRule in impliedDestinationsRules) { 
                string[] tempLocation = cableImpliedDestinationRule.cableSource.Split(
                    impliedDestinationDelimeters);

                // MMMMFPRcc
                CardSlotInfo sourceInfo = new CardSlotInfo(tempLocation[0] +
                    tempLocation[1] + tempLocation[3] + tempLocation[4] + tempLocation[5] + "X");
                impliedDestinationSources.Add(sourceInfo);

                tempLocation = cableImpliedDestinationRule.cableImpliedDestination.Split(
                    impliedDestinationDelimeters);
                CardSlotInfo destInfo = new CardSlotInfo(tempLocation[0] +
                    tempLocation[1] + tempLocation[3] + tempLocation[4] + tempLocation[5] + "X");
                impliedDestinationDestinations.Add(destInfo);
            }

            //  Fill in static combo boxes' data sources.

            ecoTagComboBox.DataSource = ecoTagList;
            machineComboBox.DataSource = machineList;
            destinationMachineList = new List<Machine>(machineList);
            destinationMachineComboBox.DataSource = destinationMachineList;
            cardTypeComboBox.DataSource = cardTypeList;

            //  Fill in constant data.

            currentVolumeSet = volumeSet;
            currentVolume = volume;
            currentPage = pageTable.getByKey(cableEdgeConnectionPage.page);
            currentCableEdgeConnectionPage = cableEdgeConnectionPage;
            currentCardLocation = cardLocation;
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

            if (machineComboBox.SelectedItem == null) {
                Console.WriteLine("Machine Combo Box Selected item unexpectedly null.");
                Console.WriteLine("Diagram machine Key = " + machine.idMachine);
            }

            foreach (string row in Helpers.validRows) {
                cardRowComboBox.Items.Add(row);
                destinationRowComboBox.Items.Add(row);
            }

            //  If the cable/edge connection block object passed to us is null, create
            //  one, and fill in as much as we can from the card location
            //  info passed (if any)

            currentCableEdgeConnectionBlock = cableEdgeConnectionBlock;
            if (currentCableEdgeConnectionBlock == null ||
                currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock == 0) {
                newCableEdgeConnection = true;
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
                currentCableEdgeConnectionBlock.Destination = 0;
                currentCableEdgeConnectionBlock.doNotCrossCheckConnection = 0;
                currentCableEdgeConnectionBlock.doNotCrossCheckEdgeConnector = 0;

                if (cardLocation != null) {
                    currentCableEdgeConnectionBlock.cardSlot = cardLocation.cardSlot;
                }
            }
            else {
                deleteButton.Visible = true;
            }

            //  Get the card slot info, if available.  (Or blanks/zero if not)

            currentCardSlotInfo = Helpers.getCardSlotInfo(currentCableEdgeConnectionBlock.cardSlot);
            if (currentCardSlotInfo.column == 0) {
                currentCardSlotInfo.column = 1;
            }
            if (currentCardSlotInfo.row == "") {
                currentCardSlotInfo.row = "A";
            }

            //  If we have a destination for the connection, fill in its row
            //  and column.  Otherwise, if it is not an explicit destination,
            //  calculate the implied destination.  Otherwise, just set a
            //  default destination row and column.

            currentDestinationCardSlotInfo = Helpers.getCardSlotInfo(
                currentCableEdgeConnectionBlock.Destination);

            if (currentCableEdgeConnectionBlock.explicitDestination == 0) {

                //  Look for a matching implied destination source

                CardSlotInfo impliedDestination = 
                    findMatchingImpliedDestination(currentCardSlotInfo);

                //  If we found one, copy it.

                if(impliedDestination != null) {

                    //  Not 100% sure this is necessary, but, just in case,
                    //  copy the data, lest a reference leave the list entry
                    //  vulnerable to change.

                    copyMatchingImpliedDestination(
                        currentDestinationCardSlotInfo, impliedDestination);
                    explicitDestinationCheckBox.Checked = false;
                    disableDestinationBoxes();
                }
                else {
                    //  No match, so mark it explicit.
                    explicitDestinationCheckBox.Checked = true;
                }
            }
            else {
                explicitDestinationCheckBox.Checked = true;
            }

            //  If we didn't find a match, set some defaults.

            if (currentDestinationCardSlotInfo.column == 0) {
                currentDestinationCardSlotInfo.column = 1;
            }
            if (currentDestinationCardSlotInfo.row == "") {
                currentDestinationCardSlotInfo.row = "A";
            }

            //  If we have existing slot machine info, use it.  Otherwise use the diagram
            //  machine.

            if (currentCardSlotInfo.machineName.Length > 0) {
                currentMachine = machineList.Find(
                    x => x.name == currentCardSlotInfo.machineName);
            }
            else {
                currentMachine = machine;
            }

            machineComboBox.SelectedItem = currentMachine;


            if (currentDestinationCardSlotInfo.machineName.Length > 0) {
                currentDestinationMachine = destinationMachineList.Find(
                    x => x.name == currentDestinationCardSlotInfo.machineName);
            }
            else {
                currentDestinationMachine = destinationMachineList.Find(
                    x => x.name == currentMachine.name);
            }

            destinationMachineComboBox.SelectedItem = currentDestinationMachine;

            populateFrameComboBox();
            populateDestinationFrameComboBox();
            populateCardTypeComboBox();

            //  If we have an explicit destination flag and value, and this is not
            //  a brand new block, fill in the cable/edge connection block top 
            //  note unless it already has a value.

            if (newCableEdgeConnection == false &&
                currentCableEdgeConnectionBlock.topNote.Length == 0 &&
                cableConnectorCardTypes.Contains(((Cardtype)cardTypeComboBox.SelectedItem).type) &&
                !cePanelNames.Contains(currentDestinationCardSlotInfo.panelName) &&
                currentCableEdgeConnectionBlock.explicitDestination > 0) {
                currentCableEdgeConnectionBlock.topNote = "TO " +
                    currentDestinationCardSlotInfo.ToSmallString().ToUpper();
            }

            //  Populate the rest of the dialog, in hierarchical order.

            populateDialog();

            // Unnecessary - populateDialog() does this.populatingDialog = false;
            drawCableEdgeConnectionBox();
        }

        //  Method to populate combo boxes that depend on the selections
        //  in other combo boxes.

        //  First, the frames (origin and destination)

        void populateFrameComboBox() {

            List<Frame> frameList = frameTable.getWhere(
                "WHERE machine='" + currentMachine.idMachine + "'" +
                " ORDER BY frame.name");

            //  If there are no frames, then we cannot proceed...
            if (frameList == null || frameList.Count == 0) {
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

        //  Destination Frame

        void populateDestinationFrameComboBox() {

            List<Frame> frameList = frameTable.getWhere(
                "WHERE machine='" + currentDestinationMachine.idMachine + "'" +
                " ORDER BY frame.name");

            //  If there are no frames, then we cannot proceed...
            if (frameList == null || frameList.Count == 0) {
                return;
            }
            destinationFrameComboBox.DataSource = frameList;
            //  Select the matching entry, if possible...
            if (currentDestinationCardSlotInfo.frameName.Length > 0) {
                destinationFrameComboBox.SelectedItem = frameList.Find(
                    x => x.name == currentDestinationCardSlotInfo.frameName);
            }
            else {
                destinationFrameComboBox.SelectedItem = frameList[0];
            }
            currentDestinationFrame = (Frame)destinationFrameComboBox.SelectedItem;
            //  Then on to the gate and the rest of the dialog...
            populateDestinationMachineGateComboBox();
        }


        //  Populate the (Machine) gate combo boxes (origin and destination)

        void populateMachineGateComboBox() {

            List<Machinegate> machineGateList = machineGateTable.getWhere(
                "WHERE frame='" + currentFrame.idFrame + "'" +
                " ORDER BY machinegate.name");
            //  If there are no gates, we cannot proceed...
            if (machineGateList.Count == 0) {
                return;
            }
            gateComboBox.DataSource = machineGateList;
            //  Select the matching entry, if possible...
            if (currentCardSlotInfo.gateName.Length > 0) {
                gateComboBox.SelectedItem = machineGateList.Find(
                    x => x.name == currentCardSlotInfo.gateName);
            }
            else {
                gateComboBox.SelectedItem = machineGateList[0];
            }
            currentMachineGate = (Machinegate)gateComboBox.SelectedItem;
            //  Then on to the Panel and the rest...
            populatePanelComboBox();
        }

        //  Destination Gate

        void populateDestinationMachineGateComboBox() {

            List<Machinegate> machineGateList = machineGateTable.getWhere(
                "WHERE frame='" + currentDestinationFrame.idFrame + "'" +
                " ORDER BY machinegate.name");
            //  If there are no gates, we cannot proceed...
            if (machineGateList.Count == 0) {
                return;
            }
            destinationGateComboBox.DataSource = machineGateList;
            //  Select the matching entry, if possible...
            if (currentDestinationCardSlotInfo.gateName.Length > 0) {
                destinationGateComboBox.SelectedItem = machineGateList.Find(
                    x => x.name == currentDestinationCardSlotInfo.gateName);
            }
            else {
                destinationGateComboBox.SelectedItem = machineGateList[0];
            }
            currentDestinationMachineGate = (Machinegate)destinationGateComboBox.SelectedItem;
            //  Then on to the Panel and the rest...
            populateDestinationPanelComboBox();
        }

        //  Next, the origin and destination panels

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

        //  Destination panel

        void populateDestinationPanelComboBox() {

            List<Panel> panelList = panelTable.getWhere(
                "WHERE gate='" + currentDestinationMachineGate.idGate + "'" +
                " ORDER BY panel");
            //  If there are no panels, we cannot proceed...
            if (panelList.Count == 0) {
                return;
            }
            destinationPanelComboBox.DataSource = panelList;
            //  Select the matching entry, if possible.
            if (currentDestinationCardSlotInfo.panelName.Length > 0) {
                destinationPanelComboBox.SelectedItem = panelList.Find(
                    x => x.panel == currentDestinationCardSlotInfo.panelName);
            }
            else {
                destinationPanelComboBox.SelectedItem = panelList[0];
            }
            currentDestinationPanel = (Panel)destinationPanelComboBox.SelectedItem;

        }

        //  Routine to populate the card type combo box - we do it in two places.

        void populateCardTypeComboBox() {

            //  Get type from cable/edge connection block, or if none there, from the
            //  card location, if possible.  Otherwise, default to the first
            //  one in the list.

            if (currentCableEdgeConnectionBlock.connectionType > 0) {
                cardTypeComboBox.SelectedItem = cardTypeList.Find(
                    x => x.idCardType == currentCableEdgeConnectionBlock.connectionType);
            }
            else if (currentCardLocation != null && currentCardLocation.type != 0) {
                cardTypeComboBox.SelectedItem = cardTypeList.Find(
                    x => x.idCardType == currentCardLocation.type);
            }
            else {
                cardTypeComboBox.SelectedItem = cardTypeList[0];
            }
        }

        void populateDialog() {

            Cableedgeconnectionecotag currentEcoTag = null;

            int index;

            populatingDialog = true;

            cableEdgeConnectionBlockTitleTextBox.Text = currentCableEdgeConnectionBlock.topNote;

            if (currentCableEdgeConnectionBlock.ecotag != 0) {
                currentEcoTag = ecoTagList.Find(
                    x => x.idcableEdgeConnectionECOtag == currentCableEdgeConnectionBlock.ecotag);
                ecoTagComboBox.SelectedItem = currentEcoTag;
            }

            //  If there is no existing eco tag, select the first "real" entry

            if (currentEcoTag == null) {
                ecoTagComboBox.SelectedItem = currentEcoTag =
                    ecoTagList[ecoTagList.Count > 1 ? 1 : 0];
            }

            index = Array.IndexOf(Helpers.validRows, currentCardSlotInfo.row);
            if (index < 0) {
                index = 0;
            }

            //  Populate the checkboxes

            doNotCrossCheckConnectorsCheckBox.Checked =
                currentCableEdgeConnectionBlock.doNotCrossCheckConnection == 1;

            doNotCrossCheckEdgeConnectionCheckBox.Checked =
                currentCableEdgeConnectionBlock.doNotCrossCheckEdgeConnector == 1;

            //  Populate the connector row and column (source location)

            cardRowComboBox.SelectedIndex = index;

            cardColumnTextBox.Text = currentCardSlotInfo.column.ToString("D2");

            populateCardTypeComboBox();

            //  If the card type is not CABL or CONN, then disable the destination stuff.
            //  Otherwise, if the destination is explicit, enable them.  Otherwise, leave
            //  it alone.

            if(!cableConnectorCardTypes.Contains(
                ((Cardtype)cardTypeComboBox.SelectedItem).type) ) {
                disableDestinationBoxes();
                explicitDestinationCheckBox.Checked = false;
            }
            else if (currentCableEdgeConnectionBlock.explicitDestination > 0) {
                explicitDestinationCheckBox.Checked = true;
                enableDestinationBoxes();
            }

            //  Fill in the destination row and column

            index = Array.IndexOf(Helpers.validRows, currentDestinationCardSlotInfo.row);
            if (index < 0) {
                index = 0;
            }

            destinationRowComboBox.SelectedIndex = index;
            destinationColumnTextBox.Text = currentDestinationCardSlotInfo.column.ToString("D2");

            populatingDialog = false;

        }

        private void drawCableEdgeConnectionBox() {

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

            if (populatingDialog) {
                return;
            }

            string machineSuffix = ((Machine)machineComboBox.SelectedItem).name;
            string destinationMachineSuffix = 
                ((Machine)destinationMachineComboBox.SelectedItem).name;
            string cardType = ((Cardtype)cardTypeComboBox.SelectedItem).type;

            machineSuffix = machineSuffix.Length >= 4 ? machineSuffix.Substring(2, 2) : "??";
            destinationMachineSuffix = destinationMachineSuffix.Length >= 4 ? 
                destinationMachineSuffix.Substring(2, 2) : "??";

            int column;
            int.TryParse(cardColumnTextBox.Text, out column);

            for (int i = 0; i < 1; ++i) {
                s += Environment.NewLine;
            }

            s += new string(' ',
                tabLen + width - 1 - cableEdgeConnectionBlockTitleTextBox.Text.Length / 2) +
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

            if(cardType.Equals("SAVE")) {
                s += tab + bar + "    " + bar + Environment.NewLine;
            }
            else if(cardType.Equals("RPQ") || cardType.Equals("STRL") ||
                   cardType.Equals("TIE")) {
                s += tab + bar + "SAVE" + bar + Environment.NewLine;
            }
            else {
                s += tab + bar + currentMachine.aldMachineType + 
                    currentFrame.name + currentPanel.panel +
                    bar + Environment.NewLine;
            }

            s += tab + bar + cardType +
                (cardType.Length < 4 ? new string(' ', 4 - cardType.Length) : "") +
                bar + Environment.NewLine;


            if (cableConnectorCardTypes.Contains(cardType)) {
                if (cePanelNames.Contains(currentDestinationPanel.panel)) {
                    s += tab + bar + "CE  " + bar + Environment.NewLine;
                }
                else {
                    s += tab + bar + currentDestinationMachine.aldMachineType + 
                     currentDestinationFrame.name +
                     currentDestinationPanel.panel + bar + Environment.NewLine;
                }
            }
            else {
                s += tab + bar + "    " + bar + Environment.NewLine;
            }

            s += tab + bar + machineSuffix + currentFrame.name +
                ((Cableedgeconnectionecotag)ecoTagComboBox.SelectedItem).name +
                bar + Environment.NewLine;

            s += tab + bar + currentPanel.panel.ToString().Substring(0, 1) +
                cardRowComboBox.SelectedItem + column.ToString("D2") +
                bar +
                Environment.NewLine;

            s += tab + bar + "----" + bar + Environment.NewLine;

            s += tab;
            s += new string(upperscore, width + 2);
            s += Environment.NewLine;

            s += tab + "  " +
                diagramColumnTextBox.Text + diagramRowTextBox.Text + Environment.NewLine;

            cableEdgeConnectionBlockDrawingTextBox.Text = s;
        }


        //  Combo Box selection methods.


        private void machineComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            Machine sourceMachine = (Machine)machineComboBox.SelectedItem;

            sourceFrameLabel = sourceMachine.frameLabel;
            sourceGateLabel = sourceMachine.gateLabel;
            sourcePanelLabel = sourceMachine.panelLabel;
            sourceRowLabel = sourceMachine.rowLabel;
            sourceColumnLabel = sourceMachine.columnLabel;

            selectSourceFrameLabel.Text = sourceFrameLabel + ":";
            selectSourceGateLabel.Text = sourceGateLabel + ":";
            selectSourcePanelLabel.Text = sourcePanelLabel + ":";
            selectSourceRowLabel.Text = sourceRowLabel + ":";
            selectSourceColumnLabel.Text = sourceColumnLabel + ":";

            if (!populatingDialog) {
                currentMachine = (Machine)machineComboBox.SelectedItem;
                if (currentCardSlotInfo.machineName != currentMachine.name) {
                    currentCardSlotInfo.machineName = currentMachine.name;
                    modifiedMachineGatePanelFrame = true;
                }
                populateFrameComboBox();
            }
        }

        private void destinationMachineComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            Machine destMachine = (Machine)machineComboBox.SelectedItem;

            destFrameLabel = destMachine.frameLabel;
            destGateLabel = destMachine.gateLabel;
            destPanelLabel = destMachine.panelLabel;
            destRowLabel = destMachine.rowLabel;
            destColumnLabel = destMachine.columnLabel;

            selectDestFrameLabel.Text = destFrameLabel + ":";
            selectDestGateLabel.Text = destGateLabel + ":";
            selectDestPanelLabel.Text = destPanelLabel + ":";
            selectDestRowLabel.Text = destRowLabel + ":";
            selectDestColumnLabel.Text = destColumnLabel + ":";

            if (!populatingDialog) {
                currentDestinationMachine = (Machine)destinationMachineComboBox.SelectedItem;
                if (currentDestinationCardSlotInfo.machineName != currentDestinationMachine.name) {
                    currentDestinationCardSlotInfo.machineName = currentDestinationMachine.name;
                    modifiedMachineGatePanelFrame = true;
                }
                populateDestinationFrameComboBox();
            }

        }

        private void frameComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (!populatingDialog) {
                currentFrame = (Frame)frameComboBox.SelectedItem;
                if (currentCardSlotInfo.frameName != currentFrame.name) {
                    currentCardSlotInfo.frameName = currentFrame.name;
                    modifiedMachineGatePanelFrame = true;
                }
                populateMachineGateComboBox();
            }
        }

        private void destinationFrameComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (!populatingDialog) {
                currentDestinationFrame = (Frame)destinationFrameComboBox.SelectedItem;
                if (currentDestinationCardSlotInfo.frameName != currentDestinationFrame.name) {
                    currentDestinationCardSlotInfo.frameName = currentDestinationFrame.name;
                    modifiedMachineGatePanelFrame = true;
                }
                populateDestinationMachineGateComboBox();
            }

        }

        private void gateComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (!populatingDialog) {
                currentMachineGate = (Machinegate)gateComboBox.SelectedItem;
                if (currentCardSlotInfo.gateName != currentMachineGate.name) {
                    currentCardSlotInfo.gateName = currentMachineGate.name;
                    modifiedMachineGatePanelFrame = true;
                }
                populatePanelComboBox();
            }
        }

        private void destinationGateComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (!populatingDialog) {
                currentDestinationMachineGate = (Machinegate)destinationGateComboBox.SelectedItem;
                if (currentDestinationCardSlotInfo.gateName != currentDestinationMachineGate.name) {
                    currentDestinationCardSlotInfo.gateName = currentDestinationMachineGate.name;
                    modifiedMachineGatePanelFrame = true;
                }
                populateDestinationPanelComboBox();
            }

        }

        private void panelComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (!populatingDialog) {
                currentPanel = (Panel)panelComboBox.SelectedItem;
                if (currentCardSlotInfo.panelName != currentPanel.panel) {
                    currentCardSlotInfo.panelName = currentPanel.panel;
                    modifiedMachineGatePanelFrame = true;
                }
                updateDestinationBoxes();
                drawCableEdgeConnectionBox();
            }
        }

        private void destinationPanelComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (!populatingDialog) {
                currentDestinationPanel = (Panel)destinationPanelComboBox.SelectedItem;
                if (currentDestinationCardSlotInfo.panelName != currentDestinationPanel.panel) {
                    currentDestinationCardSlotInfo.panelName = currentDestinationPanel.panel;
                    modifiedMachineGatePanelFrame = true;
                }
                drawCableEdgeConnectionBox();
            }
        }


        private void cableEdgeConnectionBlockTitleTextBox_TextChanged(object sender, EventArgs e) {
            drawCableEdgeConnectionBox();
        }

        private void ecoTagComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            drawCableEdgeConnectionBox();
        }


        //  Handle row and column dialog changes (source and destination)

        private void cardRowComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            currentCardSlotInfo.row = (string)cardRowComboBox.SelectedItem;
            updateDestinationBoxes();
            drawCableEdgeConnectionBox();
        }

        private void destinationRowComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            drawCableEdgeConnectionBox();
        }

        private void cardColumnTextBox_TextChanged(object sender, EventArgs e) {
            int v;
            if (cardColumnTextBox.Text.Length > 0 &&
                !int.TryParse(cardColumnTextBox.Text, out v)) {
                MessageBox.Show("Card Column must be numeric!", "Invalid Card Column");
                cardColumnTextBox.Text = currentCardSlotInfo.column.ToString("D2");
            }
            // drawCableEdgeConnectionBox();  Moved to method below.
        }

        //  Only update info based on the column once the user leaves the control

        private void cardColumnTextBox_Leave(object sender, EventArgs e) {
            int v;
            int.TryParse(cardColumnTextBox.Text, out v);
            currentCardSlotInfo.column = v;
            updateDestinationBoxes();
            drawCableEdgeConnectionBox();
        }


        private void destinationColumnTextBox_TextChanged(object sender, EventArgs e) {
            int v;
            if (destinationColumnTextBox.Text.Length > 0 &&
                !int.TryParse(destinationColumnTextBox.Text, out v)) {
                MessageBox.Show("Destination Column must be numeric!", "Invalid Destination Column");
                destinationColumnTextBox.Text = currentDestinationCardSlotInfo.column.ToString("D2");
            }
            drawCableEdgeConnectionBox();
        }

        //  Handle card type changes

        private void cardTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            drawCableEdgeConnectionBox();
        }

        //  The user hits the Apply button...

        private void applyButton_Click(object sender, EventArgs e) {

            //  Time to insert/update the Cable/Edge Connection Block

            int column = 0;
            int destinationColumn = 0;
            string message = "";
            applySuccessful = false;

            if (cardColumnTextBox.Text == null || cardColumnTextBox.Text.Length == 0 ||
                !int.TryParse(cardColumnTextBox.Text, out column) ||
                column < 1 || column > 99) {
                MessageBox.Show("Card Column must be present, and be 1-99",
                    "Invalid Card Column",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                cardColumnTextBox.Focus();
                return;
            }

            if (cableConnectorCardTypes.Contains(((Cardtype)cardTypeComboBox.SelectedItem).type) &&
                (destinationColumnTextBox.Text == null || 
                    destinationColumnTextBox.Text.Length == 0 ||
                    !int.TryParse(destinationColumnTextBox.Text, out destinationColumn) ||
                    destinationColumn < 1 || destinationColumn > 99)) {
                MessageBox.Show("Destination Card Column must be present, and be 1-99",
                    "Invalid Destination Card Column",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                destinationColumnTextBox.Focus();
                return;
            }

            //  Update the card slot info from the dialog

            currentCardSlotInfo.machineName = ((Machine)machineComboBox.SelectedItem).name;
            currentCardSlotInfo.frameName = ((Frame)frameComboBox.SelectedItem).name;
            currentCardSlotInfo.gateName = ((Machinegate)gateComboBox.SelectedItem).name;
            currentCardSlotInfo.panelName = ((Panel)panelComboBox.SelectedItem).panel;
            currentCardSlotInfo.row = (string)cardRowComboBox.SelectedItem;
            currentCardSlotInfo.column = column;

            if(cableConnectorCardTypes.Contains(((Cardtype)cardTypeComboBox.SelectedItem).type)) {
                currentDestinationCardSlotInfo.machineName =
                    ((Machine)destinationMachineComboBox.SelectedItem).name;
                currentDestinationCardSlotInfo.frameName =
                    ((Frame)destinationFrameComboBox.SelectedItem).name;
                currentDestinationCardSlotInfo.gateName =
                    ((Machinegate)destinationGateComboBox.SelectedItem).name;
                currentDestinationCardSlotInfo.panelName =
                    ((Panel)destinationPanelComboBox.SelectedItem).panel;
                currentDestinationCardSlotInfo.row = (string)destinationRowComboBox.SelectedItem;
                currentDestinationCardSlotInfo.column = destinationColumn;
            }

            //  Also update some fields of the current diagram block from the dialog now.

            currentCableEdgeConnectionBlock.topNote =
                cableEdgeConnectionBlockTitleTextBox.Text.ToUpper();
            currentCableEdgeConnectionBlock.ecotag =
                ((Cableedgeconnectionecotag)ecoTagComboBox.SelectedItem).idcableEdgeConnectionECOtag;
            currentCableEdgeConnectionBlock.explicitDestination =
                (explicitDestinationCheckBox.Checked ? 1 : 0);
            currentCableEdgeConnectionBlock.connectionType =
                ((Cardtype)cardTypeComboBox.SelectedItem).idCardType;
            currentCableEdgeConnectionBlock.doNotCrossCheckConnection =
                (doNotCrossCheckConnectorsCheckBox.Checked ? 1 : 0);
            currentCableEdgeConnectionBlock.doNotCrossCheckEdgeConnector =
                (doNotCrossCheckEdgeConnectionCheckBox.Checked ? 1 : 0);
                

            //  Tell the user what the update will actually do...

            message = doUpdate(false);
            DialogResult result = MessageBox.Show(
                "Confirm you wish to apply the following: \n\n" + message,
                "Confirm Adds/Updates",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            if (result == DialogResult.OK) {
                message = doUpdate(true);
                MessageBox.Show(message, "Adds/Updates applied.",
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

            int diagramBlockKey = currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock;

            if (updating) {
                db.BeginTransaction();

                //  Get the new database key now, if we will need it, but don't overwrite
                //  the 0 just yet, so we know later if we are inserting or not.

                if (diagramBlockKey == 0) {
                    diagramBlockKey = IdCounter.incrementCounter();
                }

            }

            message = (currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock != 0
                    ? updateAction : action) +
                " Cable/Edge connection block " +
                (currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock != 0 ?
                "(Database ID " + currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock +
                ")" : "") + "\n\n";

            //  Add the card slots, if necessary.

            currentCableEdgeConnectionBlock.cardSlot =
                Helpers.getOrAddCardSlotKey(updating, currentCardSlotInfo, out tempMessage);

            message += tempMessage;

            //  For CABL or CONN, create a destiantion location if it does not already exist.

            if (cableConnectorCardTypes.Contains(((Cardtype)cardTypeComboBox.SelectedItem).type)) {
                currentCableEdgeConnectionBlock.Destination =
                    Helpers.getOrAddCardSlotKey(updating, currentDestinationCardSlotInfo,
                        out tempMessage);
            }
            else {
                currentCableEdgeConnectionBlock.Destination = 0;
                currentCableEdgeConnectionBlock.explicitDestination = 0;
                tempMessage = "Clearing Destination for non CABL or CONN types\n";
            }


            message += tempMessage;

            if (updating) {
                if (currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock == 0) {
                    currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock = diagramBlockKey;
                    cableEdgeConnectionBlockTable.insert(currentCableEdgeConnectionBlock);
                    message += "New Cable/Edge Connection Block Database ID=" +
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

            //  Here would be code to check if this database entryw as referred to by
            //  any others - but so far that does not happen with cableEdgeconnectionBlocks.

            if (message.Length > 0) {
                message = "Cannot delete Cable/Edge Connection Block with Database ID=" +
                    currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock + ":\n\n" + message;
                MessageBox.Show(message, "Cannot Delete Cable/Edge Connection Block",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            else {
                DialogResult result = MessageBox.Show(
                    "Confirm Deletion of Cable/Edge Connection Block with Database ID=" +
                    currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock, "Confirm Deletion",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.OK) {
                    cableEdgeConnectionBlockTable.deleteByKey(
                        currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock);
                    MessageBox.Show("Cable/Edge Connection Block with Database ID=" +
                        currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock + " Deleted.",
                        "Cable/Edge Connection Block Deleted",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            //  Buh bye...
            this.Close();
        }


        //  The user changed the checkbox.         

        private void explicitDestinationCheckBox_CheckedChanged(object sender, EventArgs e) {


            //  If we are changing to UNchecked, the source must be valid
            //  (i.e., in the impliedDestinationSources list), in which case
            //  we change to the corresponding destination, and clear the explicit
            //  destination flag database field.  If there is none implied entry
            //  in the list, throw up an error box, and leave it alone.

            //  If it is being checked, just set the explicit destination flag database field.

            bool newState = explicitDestinationCheckBox.Checked;

            if (newState == false) {

                //  Use *updated* card slot info for the source

                CardSlotInfo tempCardSlotInfo = new CardSlotInfo(
                    ((Machine)machineComboBox.SelectedItem).name +
                    ((Frame)frameComboBox.SelectedItem).name +
                    ((Panel)panelComboBox.SelectedItem).panel +
                    (string)cardRowComboBox.SelectedItem +
                    cardColumnTextBox.Text +
                    "X");

                CardSlotInfo impliedDestination = findMatchingImpliedDestination(
                    tempCardSlotInfo);

                //  For non CABL and CONN card types, don't check destination,
                //  otherwise, if implied destiation is 0, make sure we have a
                //  matching implied destination source, and if so, fill in the
                //  data from the implied destiantin.

                if(cableConnectorCardTypes.Contains(
                        ((Cardtype)cardTypeComboBox.SelectedItem).type)) {
                    if(impliedDestination == null) {
                        MessageBox.Show("Implicit Destination Invalid: No source location match",
                            "Implicit Destination Invalid", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        explicitDestinationCheckBox.Checked = true;
                        enableDestinationBoxes();
                    }
                    else {
                        //  This changes everything.  ;)

                        copyMatchingImpliedDestination(currentDestinationCardSlotInfo,
                            impliedDestination);
                        currentDestinationCardSlotInfo.row = (string)cardRowComboBox.SelectedItem;
                        currentDestinationMachine = destinationMachineList.Find(
                            x => x.name == currentDestinationCardSlotInfo.machineName);
                        currentCableEdgeConnectionBlock.explicitDestination = 0;

                        populatingDialog = true;
                        destinationMachineComboBox.SelectedItem = currentDestinationMachine;
                        populateDestinationFrameComboBox();     // This also does Gate,Panel,Row,Column
                        disableDestinationBoxes();
                        populateDialog();
                        populatingDialog = false;
                        drawCableEdgeConnectionBox();
                    }
                }

            }
            else {
                currentCableEdgeConnectionBlock.explicitDestination = 1;
                enableDestinationBoxes();
            }
        }

        //  Method to update destination information (and possibly even make it explicit)
        //  if the source card slot is changed and the destination was implicit.

        void updateDestinationBoxes() {
            if(explicitDestinationCheckBox.Checked == true) {
                return;
            }

            //  OK, we had an implied destination.  Does the current slot have a matching
            //  implied destination?

            CardSlotInfo impliedDestination =
                findMatchingImpliedDestination(currentCardSlotInfo);
            if(impliedDestination == null) {
                //  No match.  Go to explicit mode.
                explicitDestinationCheckBox.Checked = true;
                return;
            }

            //  Found a match.  Update data structures and dialog.

            copyMatchingImpliedDestination(
                currentDestinationCardSlotInfo, impliedDestination);

            //  Tell the selected index changed methods to back off so they don't
            //  mess with currentDestinationCardSlotInfo data.

            populatingDialog = true;

            //  And then update the destination part of the dialog.

            destinationMachineComboBox.SelectedItem =
                destinationMachineList.Find(
                    x => x.name == currentDestinationCardSlotInfo.machineName);
            populateDestinationFrameComboBox();
            populatingDialog = false;
            populateDialog();

        }

        //  Methods to disable/enable the destination boxes for implied/explicit destinations

        void disableDestinationBoxes() {
            destinationMachineComboBox.Enabled = false;
            destinationGateComboBox.Enabled = false;
            destinationFrameComboBox.Enabled = false;
            destinationPanelComboBox.Enabled = false;
            destinationRowComboBox.Enabled = false;
            destinationColumnTextBox.Enabled = false;
        }

        void enableDestinationBoxes() {
            destinationMachineComboBox.Enabled = true;
            destinationGateComboBox.Enabled = true;
            destinationFrameComboBox.Enabled = true;
            destinationPanelComboBox.Enabled = true;
            destinationRowComboBox.Enabled = true;
            destinationColumnTextBox.Enabled = true;
        }


        //  Method to see if the diagram block was modified, or not.  Did this rather than
        //  trying to track all of the individual changes as they happen.  
        //  (The regular apply button updates, regardless).

        private bool isModified() {

            int column, destinationColumn;

            int.TryParse(cardColumnTextBox.Text, out column);
            int.TryParse(destinationColumnTextBox.Text, out destinationColumn);

            return (modifiedMachineGatePanelFrame ||
                currentCableEdgeConnectionBlock.idCableEdgeConnectionBlock == 0 ||
                currentCardSlotInfo.machineName != ((Machine)machineComboBox.SelectedItem).name ||
                currentCardSlotInfo.frameName != ((Frame)frameComboBox.SelectedItem).name ||
                currentCardSlotInfo.gateName != ((Machinegate)gateComboBox.SelectedItem).name ||
                currentCardSlotInfo.panelName != ((Panel)panelComboBox.SelectedItem).panel ||
                currentCardSlotInfo.row != (string)cardRowComboBox.SelectedItem ||
                currentCardSlotInfo.column != column ||
                currentDestinationCardSlotInfo.machineName !=
                    ((Machine)destinationMachineComboBox.SelectedItem).name ||
                currentDestinationCardSlotInfo.frameName !=
                    ((Frame)destinationFrameComboBox.SelectedItem).name ||
                currentDestinationCardSlotInfo.gateName !=
                    ((Machinegate)destinationGateComboBox.SelectedItem).name ||
                currentDestinationCardSlotInfo.panelName !=
                    ((Panel)destinationPanelComboBox.SelectedItem).panel ||
                currentDestinationCardSlotInfo.row != (string)destinationRowComboBox.SelectedItem ||
                currentDestinationCardSlotInfo.column != destinationColumn ||
                currentCableEdgeConnectionBlock.topNote != cableEdgeConnectionBlockTitleTextBox.Text ||
                currentCableEdgeConnectionBlock.ecotag !=
                    ((Diagramecotag)ecoTagComboBox.SelectedItem).idDiagramECOTag ||
                currentCableEdgeConnectionBlock.connectionType !=
                    ((Cardtype)cardTypeComboBox.SelectedItem).idCardType ||
                (currentCableEdgeConnectionBlock.doNotCrossCheckConnection == 1) ==
                    doNotCrossCheckConnectorsCheckBox.Checked ||
                (currentCableEdgeConnectionBlock.doNotCrossCheckEdgeConnector == 1) ==
                    doNotCrossCheckEdgeConnectionCheckBox.Checked
                );
        }

        //  Utility method to search though the implied destination rules for a matching entry
        //  I didn't use a CardSlotInfo class method for this because of the "*"

        private CardSlotInfo findMatchingImpliedDestination(CardSlotInfo cardSlotInfo) {

            foreach (CardSlotInfo impliedSource in impliedDestinationSources) {
                if (cardSlotInfo.machineName.Equals(impliedSource.machineName) &&
                   cardSlotInfo.frameName.Equals(impliedSource.frameName) &&
                   cardSlotInfo.gateName.Equals(impliedSource.gateName) &&
                   cardSlotInfo.panelName.Equals(impliedSource.panelName) &&
                   cardSlotInfo.column == impliedSource.column &&
                   (impliedSource.row.Equals("*") ||
                    cardSlotInfo.row.Equals(
                        impliedSource.row))) {

                    //  Found a matching entry.  Return the corresponding destination

                    int i = impliedDestinationSources.IndexOf(impliedSource);
                    return impliedDestinationDestinations[i];
                }
            }

            //  None found

            return null;
        }

        //  Copy data from an implied destination to the current destination
        //  I didn't want to use references in some cases, requiring this "deep"copy.

        private void copyMatchingImpliedDestination(CardSlotInfo csInfo, 
            CardSlotInfo impliedDestination) {
            csInfo.machineName = impliedDestination.machineName;
            csInfo.frameName = impliedDestination.frameName;
            csInfo.gateName = impliedDestination.gateName;
            csInfo.panelName = impliedDestination.panelName;
            csInfo.row = currentCardSlotInfo.row;
            csInfo.column = impliedDestination.column;

        }
    }
}
