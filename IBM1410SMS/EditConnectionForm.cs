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
    public partial class EditConnectionForm : Form
    {

        DBSetup db = DBSetup.Instance;

        Table<Cardgate> cardGateTable;
        Table<Gatepin> gatePinTable;
        Table<Connection> connectionTable;
        Table<Dotfunction> dotFunctionTable;
        Table<Sheetedgeinformation> sheetEdgeInformationTable;
        Table<Edgeconnector> edgeConnectorTable;
        Table<Diagramblock> diagramBlockTable;
        Table<Page> pageTable;
        Table<Diagrampage> diagramPageTable;
        Table<Ibmlogicfunction> ibmLogicFunctionTable;

        Connection currentConnection;
        Diagramblock currentDiagramBlock;
        Diagrampage currentDiagramPage;
        Machine currentMachine;
        Volumeset currentVolumeSet;
        Volume currentVolume;
        Page currentPage;
        Cardtype currentCardType;

        List<Gatepin> currentPinList;
        List<Gatepin> currentLoadPinList;
        List<Diagramblock> diagramBlockList;
        List<Dotfunction> dotFunctionList;
        List<Edgeconnector> edgeConnectorList;
        List<Edgeconnector> edgeConnectorList2;
        List<Edgeconnector> gateEdgeConnectorList;
        List<Sheetedgeinformation> sheetEdgeInformationList;
        List<Gatepin> currentConnectingPinList = null;

        bool isInput;
        bool populatingDialog = true;
        int LoadIBMLogicFunctionKey = -1;

        public EditConnectionForm(
            Diagramblock diagramBlock,
            Machine machine,
            Volumeset volumeSet,
            Volume volume,
            Page page,
            Diagrampage diagramPage,
            Cardtype cardType,
            int connectionKey,
            bool isInput) {

            InitializeComponent();

            this.isInput = isInput;
            currentMachine = machine;
            currentVolumeSet = volumeSet;
            currentVolume = volume;
            currentDiagramBlock = diagramBlock;
            currentDiagramPage = diagramPage;
            currentPage = page;
            currentCardType = cardType;
            Connection connection = null;

            cardGateTable = db.getCardGateTable();
            gatePinTable = db.getGatePinTable();
            dotFunctionTable = db.getDotFunctionTable();
            connectionTable = db.getConnectionTable();
            edgeConnectorTable = db.getEdgeConnectorTable();
            sheetEdgeInformationTable = db.getSheetEdgeInformationTable();
            diagramBlockTable = db.getDiagramBlockTable();
            pageTable = db.getPageTable();
            diagramPageTable = db.getDiagramPageTable();
            ibmLogicFunctionTable = db.getIbmLogicFunctionTable();

            machineTextBox.Text = machine.name;
            volumeTextBox.Text = volumeSet.machineType + "/" + volumeSet.machineSerial +
                " Volume: " + volume.name;
            pageTextBox.Text = page.name;
            diagramColumnTextBox.Text = diagramBlock.diagramColumn.ToString();
            diagramRowTextBox.Text = diagramBlock.diagramRow;
            cardTypeTextBox.Text = cardType.type;

            //  Get the IBM logic function that is labeled "L".

            List<Ibmlogicfunction> ibmLogicFunctionList = ibmLogicFunctionTable.getWhere(
                "WHERE label='" + "L" + "'");
            if(ibmLogicFunctionList.Count > 0) {
                LoadIBMLogicFunctionKey = ibmLogicFunctionList[0].idIBMLogicFunction;
            }

            //  Get the specified connection, if any...  Otherwise, just leave
            //  it as null.

            if(connectionKey != 0) {
                connection = connectionTable.getByKey(connectionKey);
                
                //  The existing toEdgeConnectorReferences might be null.
                //  If so, set them to empty strings.

                if(connection.toEdgeConnectorReference == null) {
                    connection.toEdgeConnectorReference = "";
                }

                if(connection.toEdge2ndConnectorReference == null) {
                    connection.toEdge2ndConnectorReference = "";
                }
            }

            //  Set the buttons and labels with the appropriate text.

            if(connection == null) {
                applyButton.Text = "Add";
                deleteButton.Visible = false;
            }
            else {
                applyButton.Text = "Apply";
                deleteButton.Visible = true;
            }

            if(isInput) {
                loadPinComboBox.Visible = false;
                loadPinLabel.Visible = false;
                openCollectorLabel.Visible = false;
            }

            gatePinIOLabel.Text =
                (connection != null ? "Edit " : "Add ") +
                (isInput ? "Input to " : "Output from ") +
                "Pin:";

            connectionLabel.Text =
                "VVV " + (isInput ? "From the Output " : "To the Input ") +
                "Below VVV";

            sheetOrgDestLabel.Text = isInput ? "Source Sheet: " : "Destination Sheet: ";

            //  Populate the pins of the gate in the ALD diagram we are working on.

            currentPinList = Helpers.getGatePins(cardType.idCardType, isInput);

            //  If this is an output, Populate the load pin list.  If
            //  there are no load pins, hide the combo box and its label.
            //  (Load pins have IBM Logic Function of "L").

            if (!isInput) {
                currentLoadPinList = new List<Gatepin>();
                foreach(Gatepin pin in currentPinList) {
                    Cardgate gate = cardGateTable.getByKey(pin.cardGate);
                    if(gate.positiveLogicFunction == LoadIBMLogicFunctionKey) {
                        currentLoadPinList.Add(pin);
                    }
                }
                if(currentLoadPinList.Count > 0) {
                    //  Add an "empty" pin at the top - for no load.
                    Gatepin dummyPin = new Gatepin();
                    dummyPin.idGatePin = 0;
                    dummyPin.pin = "";
                    currentLoadPinList.Insert(0, dummyPin);
                    loadPinComboBox.Visible = true;
                    loadPinLabel.Visible = true;
                    loadPinComboBox.DataSource = currentLoadPinList;
                    loadPinComboBox.SelectedItem = dummyPin;
                }
                else {
                    loadPinComboBox.Visible = false;
                    loadPinLabel.Visible = false;
                }
            }

            //  Possibly, someday, use a list with the load pins for pinComboBox...

            pinComboBox.DataSource = currentPinList;

            //  Populate the Gate: Coordinate combo box.

            diagramBlockList = diagramBlockTable.getWhere(
                "WHERE diagramPage='" + diagramBlock.diagramPage + "'" +
                " ORDER BY diagramColumn, diagramRow");

            gateBlockComboBox.Items.Clear();
            foreach(Diagramblock block in diagramBlockList) {
                gateBlockComboBox.Items.Add(block.diagramColumn.ToString() + 
                    block.diagramRow);
            }

            //  Populate the Dot Function: Coordinate combo box.

            dotFunctionList = dotFunctionTable.getWhere(
                "WHERE diagramPage='" + diagramBlock.diagramPage + "'" +
                " ORDER BY diagramColumnToLeft, diagramRowTop");

            dotBlockComboBox.Items.Clear();
            foreach(Dotfunction dot in dotFunctionList) {
                dotBlockComboBox.Items.Add(dot.diagramColumnToLeft.ToString() + 
                    dot.diagramRowTop);
            }

            //  Populate the Edge Connector Combo Box information

            sheetEdgeInformationList = sheetEdgeInformationTable.getWhere(
                "WHERE diagramPage='" + diagramBlock.diagramPage + "'" +
                " AND " + (isInput ? "leftSide='1'" : "rightSide='1'") +
                " ORDER BY sheetedgeinformation.row, signalName");

            sheetEdgeComboBox.Items.Clear();
            foreach(Sheetedgeinformation edgeInfo in sheetEdgeInformationList) {
                sheetEdgeComboBox.Items.Add(edgeInfo.row + " / " + edgeInfo.signalName);
            }

            //  Set up the list of edge connection references.  
            //  We only need the first entry for each chain, as that
            //  will have the reference number we need.

            edgeConnectorList = edgeConnectorTable.getWhere(
                "WHERE diagramPage='" + diagramBlock.diagramPage + "'" +
                " AND edgeconnector.order='0'");
            Edgeconnector dummyEdge = new Edgeconnector();
            dummyEdge.diagramPage = 0;
            dummyEdge.reference = "";
            edgeConnectorList.Insert(0, dummyEdge);
            refComboBox.DataSource = edgeConnectorList;

            edgeConnectorList2 = new List<Edgeconnector>(edgeConnectorList);
            ref2ComboBox.DataSource = edgeConnectorList2;

            //  Clone the edge reference list for the gate comobo box, so it and
            //  the edge reference ones don't change in sync.

            gateEdgeConnectorList = new List<Edgeconnector>(edgeConnectorList);
            gateRefComboBox.DataSource = gateEdgeConnectorList;

            //  If we are editing an existing connection, select things in the dialog
            //  accordingly.

            //  Select defaults so I don't have to repeat them later.

            gateBlockComboBox.SelectedIndex = -1;
            connectingPinComboBox.SelectedIndex = -1;
            dotBlockComboBox.SelectedIndex = -1;
            sheetEdgeComboBox.SelectedIndex = -1;
            sheetTextBox.Text = "";

            refComboBox.SelectedItem = edgeConnectorList[0];
            ref2ComboBox.SelectedItem = edgeConnectorList2[0];
        
            if (connection == null) {
                //  Set up dialog defaults...
                if(currentPinList.Count > 0) {
                    pinComboBox.SelectedItem = currentPinList[0];
                    openCollectorLabel.Visible = isOpenCollector((Gatepin)pinComboBox.SelectedItem);
                }
            }
            else if(isInput) {
                //  Input TO this gate.
                pinComboBox.SelectedItem = currentPinList.Find(
                    x => x.pin == connection.toPin);
                switch(connection.from) {
                    case "P":
                        gateRadioButton.Checked = true;
                        Diagramblock fromBlock = diagramBlockTable.getByKey(
                            connection.fromDiagramBlock);
                        gateBlockComboBox.SelectedItem = 
                            fromBlock.diagramColumn.ToString() + fromBlock.diagramRow;
                        populateGatePinComboBox();
                        if (connection.fromPin != "--") {
                            connectingPinComboBox.Visible = true;
                            gateRefComboBox.Visible = true;
                            gatePinLabel.Visible = true;
                            gateRefLabel.Visible = true;
                            connectingPinComboBox.SelectedIndex =
                                currentConnectingPinList.FindIndex(
                                    x => x.pin == connection.fromPin);
                        }
                        else {
                            connectingPinComboBox.Visible = false;
                            gateRefComboBox.Visible = false;
                            gatePinLabel.Visible = false;
                            gateRefLabel.Visible = false;
                        }
                        break;
                    case "D":
                        dotRadioButton.Checked = true;
                        Dotfunction dot = dotFunctionTable.getByKey(
                            connection.fromDotFunction);
                        dotBlockComboBox.SelectedItem = dot.diagramColumnToLeft.ToString() +
                            dot.diagramRowTop;
                        break;
                    case "E":
                        edgeRadioButton.Checked = true;
                        //  Get the information about THIS sheet for combo box...
                        Sheetedgeinformation sheetEdge = sheetEdgeInformationTable.getByKey(
                            connection.fromEdgeSheet);
                        Sheetedgeinformation originEdgeSheet =
                            sheetEdgeInformationTable.getByKey(
                                connection.fromEdgeOriginSheet);                        
                        sheetEdgeComboBox.SelectedItem =
                            sheetEdge.row + " / " + sheetEdge.signalName;
                        sheetTextBox.Text = Helpers.getDiagramPageName(
                            originEdgeSheet.diagramPage);
                        refComboBox.SelectedItem = edgeConnectorList.Find(
                            x => x.reference == connection.fromEdgeConnectorReference);
                        if(refComboBox.SelectedItem == null) {
                            refComboBox.SelectedItem = edgeConnectorList[0];
                        }
                        ref2ComboBox.SelectedItem = edgeConnectorList2[0];
                        break;
                    default:
                        break;
                }
            }
            else {
                //  Output FROM this gate
                pinComboBox.SelectedItem = currentPinList.Find(
                    x => x.pin == connection.fromPin);
                openCollectorLabel.Visible = isOpenCollector((Gatepin)pinComboBox.SelectedItem);
                if (connection.fromPhasePolarity == "-") {
                    negativeRadioButton.Checked = true;
                }
                else {
                    positiveRadioButton.Checked = true;
                }
                if (connection.fromLoadPin != null && connection.fromLoadPin.Length > 0) {
                    loadPinComboBox.SelectedItem = currentLoadPinList.Find(
                        x => x.pin == connection.fromLoadPin);
                }
                switch (connection.to) {
                    case "P":
                        gateRadioButton.Checked = true;
                        Diagramblock toBlock = diagramBlockTable.getByKey(
                            connection.toDiagramBlock);
                        gateBlockComboBox.SelectedItem =
                            toBlock.diagramColumn.ToString() + toBlock.diagramRow;
                        populateGatePinComboBox();
                        if (connection.toPin != "--") {
                            connectingPinComboBox.Visible = true;
                            gateRefComboBox.Visible = true;
                            gatePinLabel.Visible = true;
                            gateRefLabel.Visible = true;
                            connectingPinComboBox.SelectedIndex =
                                currentConnectingPinList.FindIndex(
                                    x => x.pin == connection.toPin);
                            gateRefComboBox.SelectedItem = gateEdgeConnectorList.Find(
                                x => x.reference == connection.toEdgeConnectorReference);
                            if(gateRefComboBox.SelectedItem == null) {
                                gateRefComboBox.SelectedItem = gateEdgeConnectorList[0];
                            }
                        }
                        else {
                            connectingPinComboBox.Visible = false;
                            gateRefComboBox.Visible = false;
                            gatePinLabel.Visible = false;
                            gateRefLabel.Visible = false;
                        }
                        break;
                    case "D":
                        dotRadioButton.Checked = true;
                        Dotfunction dot = dotFunctionTable.getByKey(
                            connection.toDotFunction);
                        dotBlockComboBox.SelectedItem = dot.diagramColumnToLeft.ToString() +
                            dot.diagramRowTop;
                        break;
                    case "E":
                        edgeRadioButton.Checked = true;
                        //  Get the information about THIS sheet for combo box...
                        Sheetedgeinformation sheetEdge = sheetEdgeInformationTable.getByKey(
                            connection.toEdgeSheet);
                        Sheetedgeinformation destinationEdgeSheet =
                            sheetEdgeInformationTable.getByKey(
                                connection.toEdgeDestinationSheet);
                        sheetEdgeComboBox.SelectedItem =
                            sheetEdge.row + " / " + sheetEdge.signalName;
                        sheetTextBox.Text = Helpers.getDiagramPageName(
                            destinationEdgeSheet.diagramPage);
                        refComboBox.SelectedItem = edgeConnectorList.Find(
                            x => x.reference == connection.toEdgeConnectorReference);
                        if(refComboBox.SelectedItem == null) {
                            refComboBox.SelectedItem = edgeConnectorList[0];
                        }
                        ref2ComboBox.SelectedItem = edgeConnectorList2.Find(
                            x => x.reference == connection.toEdge2ndConnectorReference);
                        if(ref2ComboBox.SelectedItem == null) {
                            ref2ComboBox.SelectedItem = edgeConnectorList2[0];
                        }
                        break;
                    default:
                        break;
                }
            }

            setRadioButtonControls();

            currentConnection = connection;
            if(currentConnection == null) {
                currentConnection = new Connection();
                currentConnection.idConnection = 0;
            }

            populatingDialog = false;

            pinComboBox.Select();

        }

        void populateGatePinComboBox() {

            int column = 0;
            string coordinate = (string)gateBlockComboBox.SelectedItem;
            // currentConnectingPinList = new List<GatePinEntry>();
            currentConnectingPinList = new List<Gatepin>();

            //  First, find the selected diagram block from the list 
            //  (the combo box has a constructed string sheet coordinate)

            int.TryParse(coordinate.Substring(0, 1), out column);
            Diagramblock block = diagramBlockList.Find(
                x => x.diagramColumn == column && x.diagramRow == coordinate.Substring(1));
            if (block == null || block.idDiagramBlock == 0) {
                connectingPinComboBox.Items.Clear();
                MessageBox.Show("DiagramBlock at diagram coordinate " + column.ToString() +
                    coordinate.Substring(1) + " not found.", "Diagram Block not found",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            //  Then get a list of all of the pins associated with that block's 
            //  card type, using the opposite sense for isInput

            currentConnectingPinList = Helpers.getGatePins(block.cardType, !isInput);
            connectingPinComboBox.DataSource = currentConnectingPinList;

        }

        private void setRadioButtonControls() {
            if(gateRadioButton.Checked) {
                gateBlockComboBox.Enabled = true;
                connectingPinComboBox.Enabled = true;
                gateRefComboBox.Enabled = true;
                gateRefLabel.Enabled = true;
                gateCoordinateLabel.Enabled = true;
                gatePinLabel.Enabled = true;
                dotBlockComboBox.Enabled = false;
                dotCoordinateLabel.Enabled = false;
                sheetEdgeComboBox.Enabled = false;
                edgeSignalLabel.Enabled = false;
                sheetOrgDestLabel.Enabled = false;
                edgeRefLabel.Enabled = false;
                edgeRef2Label.Enabled = false;
                refComboBox.Enabled = false;
                ref2ComboBox.Enabled = false;
                sheetTextBox.Enabled = false;
                gateBlockComboBox.Select();
            }
            else if(dotRadioButton.Checked) {
                gateBlockComboBox.Enabled = false;
                connectingPinComboBox.Enabled = false;
                gateRefComboBox.Enabled = false;
                gateRefLabel.Enabled = false;
                gateCoordinateLabel.Enabled = false;
                gatePinLabel.Enabled = false;
                dotBlockComboBox.Enabled = true;
                dotCoordinateLabel.Enabled = true;
                sheetEdgeComboBox.Enabled = false;
                edgeSignalLabel.Enabled = false;
                sheetOrgDestLabel.Enabled = false;
                edgeRefLabel.Enabled = false;
                edgeRef2Label.Enabled = false;
                refComboBox.Enabled = false;
                ref2ComboBox.Enabled = false;
                sheetTextBox.Enabled = false;
                dotBlockComboBox.Select();
            }
            else if (edgeRadioButton.Checked) {
                gateBlockComboBox.Enabled = false;
                connectingPinComboBox.Enabled = false;
                gateRefComboBox.Enabled = false;
                gateRefLabel.Enabled = false;
                gateCoordinateLabel.Enabled = false;
                gatePinLabel.Enabled = false;
                dotBlockComboBox.Enabled = false;
                dotCoordinateLabel.Enabled = false;
                sheetEdgeComboBox.Enabled = true;
                edgeSignalLabel.Enabled = true;
                sheetOrgDestLabel.Enabled = true;
                edgeRefLabel.Enabled = true;
                edgeRef2Label.Enabled = true;
                refComboBox.Enabled = true;
                ref2ComboBox.Enabled = true;
                sheetTextBox.Enabled = true;
                sheetEdgeComboBox.Select();
            }
            else {
                gateBlockComboBox.Enabled = false;
                connectingPinComboBox.Enabled = false;
                gateRefComboBox.Enabled = false;
                gateRefLabel.Enabled = false;
                gateCoordinateLabel.Enabled = false;
                gatePinLabel.Enabled = false;
                dotBlockComboBox.Enabled = false;
                dotCoordinateLabel.Enabled = false;
                sheetEdgeComboBox.Enabled = false;
                edgeSignalLabel.Enabled = false;
                sheetOrgDestLabel.Enabled = false;
                edgeRefLabel.Enabled = false;
                edgeRef2Label.Enabled = false;
                sheetTextBox.Enabled = false;
            }
        }

        private void gateBlockComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            //  If we are still populating the dialog, ignore..
            if(populatingDialog) {
                return;
            }

            //  Otherwise, repopulate the pin combo box to match the selected gate.

            populateGatePinComboBox();

            //  Then reset the pin combo box to select the first item, if any.

            if (connectingPinComboBox.Items.Count > 0) {
                connectingPinComboBox.SelectedIndex = 0;
            }
        }

        private void pinComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            
            //  If populating dialog, ignore...
            if(populatingDialog) {
                return;
            }

            //  Set on or off the O/C indicator if ouput.

            if (!isInput) {
                openCollectorLabel.Visible = isOpenCollector((Gatepin)pinComboBox.SelectedItem);
            }

            //  Make the connectingPinComboBox invisible if pin "--" is selected.

            Gatepin pin = (Gatepin)pinComboBox.SelectedItem;
            if(pin.pin == "--") {
                connectingPinComboBox.Visible = false;
                gateRefComboBox.Visible = false;
                gatePinLabel.Visible = false;
                gateRefLabel.Visible = false;
                gateRadioButton.Checked = true;
                gateBlockComboBox.Select();
            }
            else {
                connectingPinComboBox.Visible = true;
                gateRefComboBox.Visible = true;
                gatePinLabel.Visible = true;
                gateRefLabel.Visible = true;
            }
        }

        private void sheetEdgeComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            //  If populating dialog ignore...

            if(populatingDialog) {
                return;
            }

            //  Get the list of sheet edge information entries that are outputs
            //  whose name matches the selected name.  (There should be 0 or 1)

            //  Find the sheet edge information whose RIGHT side matches the selected
            //  signal name.  We expect there to only be none or 1.

            List<Sheetedgeinformation> originSheetList = sheetEdgeInformationTable.getWhere(
                "WHERE signalName='" +
                sheetEdgeInformationList[sheetEdgeComboBox.SelectedIndex].signalName + "'" +
                " AND rightSide='1'");

            //  If this is an output, and there are no existing connections and there
            //  are more than one destination for the selected entry, bring up a 
            //  separate dialog populated with all of the destinations that are known.

            if (!isInput && originSheetList.Count == 1) {

                if(connectionTable.getWhere("WHERE toEdgeSheet='" +
                    originSheetList[0].idSheetEdgeInformation + "'").Count > 0) {
                    populateButton.Enabled = false;
                    return;
                }

                //  OK, so there are no existing origin sheet (output) connections for
                //  this signal name.  Is there more than one destination for this signal name?
                
                List<Sheetedgeinformation> destinationSheetList = sheetEdgeInformationTable.getWhere(
                    "WHERE signalName='" +
                    sheetEdgeInformationList[sheetEdgeComboBox.SelectedIndex].signalName + "'" +
                    " AND leftSide='1'");

                //  Decided that instead, we would always make the populate button available,
                //  so long as their are no existing connections.

                //if(destinationSheetList.Count <= 1) {
                //    populateButton.Enabled = false;
                //    return;
                //}

                populateButton.Enabled = true;
                return;
            }

            //  If there is more than one origin sheet entry, or none, just return.
            //  We would not know what to do with it.

            if(!isInput) {
                return;
            }

            //  Ok, this is an input.  If there is exactly one sheet edge information entry
            //  whose RIGHT side matches the selected entry, then fill in the sheet number.

            populateButton.Enabled = false;
            if(originSheetList.Count == 1) {
                sheetTextBox.Text = Helpers.getDiagramPageName(originSheetList[0].diagramPage);
            }
            else {
                sheetTextBox.Text = "";
            }

        }


        private void gateRadioButton_CheckedChanged(object sender, EventArgs e) {
            setRadioButtonControls();
        }

        private void dotRadioButton_CheckedChanged(object sender, EventArgs e) {
            setRadioButtonControls();
        }

        private void edgeRadioButton_CheckedChanged(object sender, EventArgs e) {
            setRadioButtonControls();
        }

        //  Handle some keys specially, to make things go faster...

        private void EditConnectionForm_KeyPress(object sender, KeyPressEventArgs e) {

            switch (e.KeyChar) {
                case '+':
                    positiveRadioButton.Checked = true;
                    e.Handled = true;
                    break;
                case '-':
                    negativeRadioButton.Checked = true;
                    e.Handled = true;
                    break;
                case 'G':
                    gateRadioButton.Checked = true;
                    e.Handled = true;
                    break;
                case 'D':
                    dotRadioButton.Checked = true;
                    e.Handled = true;
                    break;
                case 'E':
                    edgeRadioButton.Checked = true;
                    e.Handled = true;
                    break;
                case '/':
                    pinComboBox.SelectedIndex = currentPinList.Count - 1;
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }


        private void cancelButton_Click(object sender, EventArgs e) {
            //  Buh bye....
            this.Close();
        }

        private void deleteButton_Click(object sender, EventArgs e) {

            //  This item does not depend on any others, so deleting is pretty easy.

            if(currentConnection.idConnection == 0) {
                MessageBox.Show("Deleting New Connection - No action required",
                    "Deleting New Connection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult status = MessageBox.Show(
                "Please confirm that you wish to delete this connection (Database ID " + 
                currentConnection.idConnection + ")", "Confirm Delete of Connection",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            if(status == DialogResult.OK) {
                connectionTable.deleteByKey(currentConnection.idConnection);
                MessageBox.Show("Connection (Database ID " + currentConnection.idConnection +
                    ") Deleted", "Connection Deleted.",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }

        //  Special populate button adds all the known edge connections for this signal

        private void populateButton_Click(object sender, EventArgs e) {

            EditSheetEdgeOutputConnectionsForm EditSheetEdgeOutputConnectionsForm =
                new EditSheetEdgeOutputConnectionsForm(
                    currentDiagramBlock,
                    currentMachine,
                    currentVolumeSet,
                    currentVolume,
                    currentPage,
                    currentDiagramPage,
                    currentCardType,
                    ((Gatepin)pinComboBox.SelectedItem).pin,
                    currentLoadPinList.Count > 0 ?
                        ((Gatepin)loadPinComboBox.SelectedItem).pin :
                        "",
                    negativeRadioButton.Checked ? "-" : "+",
                    edgeConnectorList,
                    sheetEdgeInformationList[sheetEdgeComboBox.SelectedIndex]);

            EditSheetEdgeOutputConnectionsForm.ShowDialog();

            //  Once we are done, we may probably need to disable the populate 
            //  button again.  For now, take the easy way out.  If the dialog
            //  stopped due to errors, the user will have to leave this dialog
            //  to fix them anyway.

            populateButton.Enabled = false;
        }


        private void applyButton_Click(object sender, EventArgs e) {

            string message = "";

            if (pinComboBox.SelectedIndex < 0) {
                MessageBox.Show("Input/Output Pin not specified", "Input/Output Pin Required");
                return;
            }

            //  If this is an input, and the input pin is not "--",
            //  check to see if the same pin is used twice.

            //  If this is an output, see what pins are already selected.  If there are more
            //  than one (including this pin) [or two if this is for a TOG toggle switch] issue
            //  a warning.  Also issue a warning if the polarities vary.

            if(isInput && ((Gatepin)pinComboBox.SelectedItem).pin != "--") {
                List<Connection> existingConnections = connectionTable.getWhere(
                    "WHERE toDiagramBlock='" + currentDiagramBlock.idDiagramBlock + "'");

                foreach(Connection connection in existingConnections) {
                    if(connection.idConnection != currentConnection.idConnection &&
                        connection.toPin == ((Gatepin)pinComboBox.SelectedItem).pin) {
                        DialogResult status = MessageBox.Show(
                            "WARNING: This diagram block would have more than one " +
                            "occurrence of pin " + ((Gatepin)pinComboBox.SelectedItem).pin,
                            "WARNING: Multiple inputs to same pin",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                        if (status == DialogResult.Cancel) {
                            return;
                        }
                    }
                }

            }
            else if(!isInput) {

                List<string> pins = new List<string>();
                bool positivePolarity = false;
                bool negativePolarity = false;

                if(positiveRadioButton.Checked) {
                    positivePolarity = true;
                }
                else {
                    negativePolarity = true;
                }

                pins.Add(((Gatepin)pinComboBox.SelectedItem).pin);

                List<Connection> existingConnections = connectionTable.getWhere(
                    "WHERE fromDiagramBlock='" + currentDiagramBlock.idDiagramBlock + "'");


                foreach (Connection connection in existingConnections) {
                    if (pins.IndexOf(connection.fromPin) < 0) {
                        pins.Add(connection.fromPin);
                    }

                    //  Don't look at the "old" polarity for this connection itself, 
                    //  as we already accounted for the new polarity, dnd it might be 
                    //  changing.

                    if (connection.idConnection == currentConnection.idConnection) {
                        continue;
                    }

                    if (connection.fromPin.Equals(((Gatepin)pinComboBox.SelectedItem).pin)) {
                        if (connection.fromPhasePolarity == "+") {
                            positivePolarity = true;
                        }
                        else {
                            negativePolarity = true;
                        }
                    }
                }

                if (pins.Count > ((currentDiagramBlock.symbol == "TOG"  ||
                        currentDiagramBlock.symbol == "ALT") ? 2 : 1)) {
                    DialogResult status = MessageBox.Show(
                        "WARNING: This diagram block would have " + pins.Count +
                        " different output pins: " +
                        string.Join(",", pins.ToArray()),
                        "WARNING: Multiple output pins",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (status == DialogResult.Cancel) {
                        return;
                    }
                }

                if (positivePolarity && negativePolarity) {
                    DialogResult status = MessageBox.Show(
                        "WARNING: This diagram block would have + and - from the same pin",
                        "WARNING: Multiple conflicting polarities",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (status == DialogResult.Cancel) {
                        return;
                    }
                }

            }

            db.BeginTransaction();

            if (isInput) {
                currentConnection.to = "P";
                currentConnection.toDiagramBlock = currentDiagramBlock.idDiagramBlock;
                currentConnection.toPin = ((Gatepin)pinComboBox.SelectedItem).pin;
                currentConnection.toDotFunction = 0;
                currentConnection.toEdgeSheet = 0;
                currentConnection.toEdgeDestinationSheet = 0;
                currentConnection.toEdgeConnectorReference = "";
                currentConnection.toEdge2ndConnectorReference = "";
                if (gateRadioButton.Checked) {
                    if(gateBlockComboBox.SelectedIndex < 0) {
                        MessageBox.Show("From Gate not selected", "From Gate Required");
                        db.CancelTransaction();
                        return;
                    }
                    if(connectingPinComboBox.SelectedIndex < 0) {
                        MessageBox.Show("From Gate Pin not selected", "From Pin Required");
                        db.CancelTransaction();
                        return;
                    }
                    currentConnection.from = "P";
                    currentConnection.fromDiagramBlock =
                        diagramBlockList[gateBlockComboBox.SelectedIndex].idDiagramBlock;
                    if (currentConnection.toPin != "--") {
                        currentConnection.fromPin = currentConnectingPinList[
                            connectingPinComboBox.SelectedIndex].pin;
                    }
                    else {
                        currentConnection.fromPin = "--";
                    }
                    currentConnection.fromDotFunction = 0;
                    currentConnection.fromEdgeSheet = 0;
                    currentConnection.fromEdgeOriginSheet = 0;

                    //  If this is a new input to this gate from another gate, then
                    //  assume positive polarity.

                    if (currentConnection.fromPhasePolarity == null ||
                        currentConnection.fromPhasePolarity.Length == 0) {                         
                        currentConnection.fromPhasePolarity = "+";
                    }

                }
                else if (dotRadioButton.Checked) {
                    if(dotBlockComboBox.SelectedIndex < 0) {
                        MessageBox.Show("Dot Function block not selected", "Dot Function Block Required");
                        db.CancelTransaction();
                        return;
                    }
                    currentConnection.from = "D";
                    currentConnection.fromDotFunction =
                        dotFunctionList[dotBlockComboBox.SelectedIndex].idDotFunction;
                    currentConnection.fromDiagramBlock = 0;
                    currentConnection.fromPin = "";
                    currentConnection.fromPhasePolarity = "";
                    currentConnection.fromEdgeSheet = 0;
                    currentConnection.fromEdgeOriginSheet = 0;
                }
                else if(edgeRadioButton.Checked) {
                    if(sheetEdgeComboBox.SelectedIndex < 0) {
                        MessageBox.Show("Sheet Edge Signal not selected", "Sheet Edge Signal Required");
                        db.CancelTransaction();
                        return;
                    }
                    if(sheetTextBox.Text.Length <= 0) {
                        MessageBox.Show("Sheet Edge Sheet not identified", "Sheet Edge Sheet Required");
                        db.CancelTransaction();
                        return;
                    }
                    currentConnection.from = "E";
                    currentConnection.fromEdgeSheet =
                        sheetEdgeInformationList[sheetEdgeComboBox.SelectedIndex].idSheetEdgeInformation;
                    currentConnection.fromEdgeOriginSheet = getSheetEdgeKey(
                        currentMachine,
                        sheetTextBox.Text,
                        sheetEdgeInformationList[sheetEdgeComboBox.SelectedIndex].signalName,
                        isInput,
                        out message);
                    currentConnection.fromPin = "";
                    currentConnection.fromDiagramBlock = 0;
                    currentConnection.fromDotFunction = 0;
                    currentConnection.fromPhasePolarity = "";
                    currentConnection.fromEdgeConnectorReference =
                        ((Edgeconnector)refComboBox.SelectedItem).reference;
                    if (currentConnection.fromEdgeOriginSheet == 0) {
                        MessageBox.Show(message,"Page/Diagram Page not defined",
                            MessageBoxButtons.OK,MessageBoxIcon.Stop);
                        db.CancelTransaction();
                        return;
                    }
                }
                else {
                    MessageBox.Show("A Type of Connection Must be selected.",
                        "No Type Selected.",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    db.CancelTransaction();
                    gateRadioButton.Focus();
                    return;
                }
            }
            else {
                //  Output
                currentConnection.from = "P";
                currentConnection.fromDiagramBlock = currentDiagramBlock.idDiagramBlock;
                currentConnection.fromPin = ((Gatepin)pinComboBox.SelectedItem).pin;
                if(currentLoadPinList.Count > 0) {
                    currentConnection.fromLoadPin = ((Gatepin)loadPinComboBox.SelectedItem).pin;
                }
                else {
                    currentConnection.fromLoadPin = "";
                }
                //  Polarity defaults to + if users selects neither, and it wasn't
                //  already filled in.
                currentConnection.fromPhasePolarity = negativeRadioButton.Checked ?
                    "-" : "+";
                currentConnection.fromDotFunction = 0;
                currentConnection.fromEdgeSheet = 0;
                currentConnection.fromEdgeOriginSheet = 0;
                if (gateRadioButton.Checked) {
                    if (gateBlockComboBox.SelectedIndex < 0) {
                        MessageBox.Show("From Gate not selected", "From Gate Required");
                        db.CancelTransaction();
                        return;
                    }
                    if (connectingPinComboBox.SelectedIndex < 0) {
                        MessageBox.Show("From Gate Pin not selected", "From Pin Required");
                        db.CancelTransaction();
                        return;
                    }
                    currentConnection.to = "P";
                    currentConnection.toDiagramBlock =
                        diagramBlockList[gateBlockComboBox.SelectedIndex].idDiagramBlock;
                    if(currentConnection.fromPin != "--") { 
                    currentConnection.toPin = currentConnectingPinList[
                        connectingPinComboBox.SelectedIndex].pin;
                    }
                    else {
                        currentConnection.toPin = "--";
                    }
                    currentConnection.toDotFunction = 0;
                    currentConnection.toEdgeSheet = 0;
                    currentConnection.toEdgeDestinationSheet = 0;
                    currentConnection.toEdgeConnectorReference =
                        ((Edgeconnector)gateRefComboBox.SelectedItem).reference;
                    //  Can't have a second conenctor reference to a gate...
                    currentConnection.toEdge2ndConnectorReference = "";
                }
                else if (dotRadioButton.Checked) {
                    if (dotBlockComboBox.SelectedIndex < 0) {
                        MessageBox.Show("Dot Function block not selected", "Dot Function Block Required");
                        db.CancelTransaction();
                        return;
                    }
                    currentConnection.to = "D";
                    currentConnection.toDotFunction =
                        dotFunctionList[dotBlockComboBox.SelectedIndex].idDotFunction;
                    currentConnection.toDiagramBlock = 0;
                    currentConnection.toPin = "";
                    currentConnection.toEdgeSheet = 0;
                    currentConnection.toEdgeDestinationSheet = 0;
                    currentConnection.toEdgeConnectorReference = "";
                    currentConnection.toEdge2ndConnectorReference = "";
                }
                else if (edgeRadioButton.Checked) {
                    if (sheetEdgeComboBox.SelectedIndex < 0) {
                        MessageBox.Show("Sheet Edge Signal not selected", "Sheet Edge Signal Required");
                        db.CancelTransaction();
                        return;
                    }
                    if (sheetTextBox.Text.Length <= 0) {
                        MessageBox.Show("Sheet Edge Sheet not identified", "Sheet Edge Sheet Required");
                        db.CancelTransaction();
                        return;
                    }
                    currentConnection.to = "E";
                    currentConnection.toEdgeSheet =
                        sheetEdgeInformationList[sheetEdgeComboBox.SelectedIndex].idSheetEdgeInformation;
                    currentConnection.toEdgeDestinationSheet = getSheetEdgeKey(
                        currentMachine,
                        sheetTextBox.Text,
                        sheetEdgeInformationList[sheetEdgeComboBox.SelectedIndex].signalName,
                        isInput,
                        out message);
                    currentConnection.toEdgeConnectorReference =
                        ((Edgeconnector)refComboBox.SelectedItem).reference;                    
                    currentConnection.toEdge2ndConnectorReference =
                        ((Edgeconnector)ref2ComboBox.SelectedItem).reference;
                    if (currentConnection.toEdgeDestinationSheet == 0) {
                        MessageBox.Show(message, "Page/Diagram Page not defined",
                            MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        db.CancelTransaction();
                        return;
                    }
                    currentConnection.toPin = "";
                    currentConnection.toDiagramBlock = 0;
                    currentConnection.toDotFunction = 0;
                }
                else {
                    MessageBox.Show("A Type of Connection Must be selected.",
                        "No Type Selected.",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    db.CancelTransaction();
                    gateRadioButton.Focus();
                    return;
                }
            }

            //  Finally ready to insert/update

            if(currentConnection.idConnection == 0) {
                currentConnection.idConnection = IdCounter.incrementCounter();
                connectionTable.insert(currentConnection);
                message = "Added ";
            }
            else {
                connectionTable.update(currentConnection);
                message = "Updated ";
            }

            db.CommitTransaction();

            MessageBox.Show(message + " connection, Database ID=" +
                currentConnection.idConnection);

            this.Close();
        }

        //  Method to determine if a given output pin is open collector

        private bool isOpenCollector(Gatepin pin) {
            if(pin == null) {
                return true;        //  Well, I guess a nonexistent pin is O/C  ;)
            }
            Cardgate gate = cardGateTable.getByKey(pin.cardGate);
            return (gate != null && gate.openCollector > 0);
        }
        
        //  Shared method to get the key of a sheet edge connection.

        public static int getSheetEdgeKey(Machine machine, string pageName, 
            string signalName, bool isInput, out string message) {

            DBSetup db = DBSetup.Instance;
            Table<Page> pageTable = db.getPageTable();
            Table<Diagrampage> diagramPageTable = db.getDiagramPageTable();
            Table<Sheetedgeinformation> sheetEdgeInformationTable =
                db.getSheetEdgeInformationTable();

            //  First, find a page matching the current machine and page name.
            //  If none is found, return 0.

            message = "";
            List<Page> pageList = pageTable.getWhere(
                "WHERE machine='" + machine.idMachine + "'" +
                " AND page.name='" + pageName + "'");

            if(pageList.Count == 0) {
                message = "Page " + pageName + " not found for this machine.\n";
                return 0;
            }

            List<Diagrampage> diagramPageList = diagramPageTable.getWhere(
                "WHERE diagrampage.page='" + pageList[0].idPage + "'");

            if(diagramPageList.Count == 0) {
                message = "Diagram Page " + pageName + " not found for this machine.\n";
                return 0;
            }

            //  Look for the sheet edge information on the OTHER sheet from this one
            //  So input/output sense is reversed!  If this is an input, then the one
            //  we are looking for would be on the other sheet's RIGHT side, and 
            //  vice/versa.

            List<Sheetedgeinformation> seiList = sheetEdgeInformationTable.getWhere(
                "WHERE diagramPage='" + diagramPageList[0].idDiagramPage + "'" +
                " AND signalName='" + signalName + "'" +
                " AND " + (isInput ? "rightSide='1'" : "leftSide='1'"));
            
            //  If we get a match, we don't care about row, really.
            //  That can be adjusted on the other sheet that this refers to.

            if(seiList.Count > 0) {
                return seiList[0].idSheetEdgeInformation;
            }

            //  If none is found, we will add it, assuming row "A" - it
            //  can be adjusted later on the other sheet.

            Sheetedgeinformation sheetInfo = new Sheetedgeinformation();
            sheetInfo.diagramPage = diagramPageList[0].idDiagramPage;
            sheetInfo.row = "A";
            sheetInfo.signalName = signalName;
            sheetInfo.idSheetEdgeInformation = IdCounter.incrementCounter();

            //  Deduce leftSide / rightSide - if this is an input then
            //  the *other* sheet is an output.

            sheetInfo.leftSide = isInput ? 0 : 1;
            sheetInfo.rightSide = isInput ? 1 : 0;

            sheetEdgeInformationTable.insert(sheetInfo);
            message += "Added Sheet Edge Information for page " +
                sheetInfo.diagramPage + ", signal name " +
                sheetInfo.signalName +
                " Database ID=" + sheetInfo.idSheetEdgeInformation + "\n";
            return sheetInfo.idSheetEdgeInformation;
        }

    }
}
