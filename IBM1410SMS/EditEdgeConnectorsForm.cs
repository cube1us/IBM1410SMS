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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using MySQLFramework;

namespace IBM1410SMS
{
    public partial class EditEdgeConnectorsForm : Form
    {
        DBSetup db = DBSetup.Instance;

        Table<Edgeconnector> edgeConnectorTable;
        Table<Page> pageTable;

        List<Edgeconnector> edgeConnectorList;
        List<EdgeConnections> dgvEdgeConnectionsList;
        List<EdgeConnections> originalEdgeConnectionsList;
        BindingList<EdgeConnections> edgeConnectionsBindingList;

        Diagrampage currentDiagramPage = null;
        Page currentPage = null;
        string machinePrefix;
        string rowLabel = "Row";
        bool populatingDialog = false;

        //  Number of pins/connections.  If you change this, you must also change the
        //  internal Edgeconnections class...

        static int MAXCONNECTIONS = 10;

        Regex connectionPattern = new Regex(@"(\d\d)([a-zA-z1-9])(\d)([a-zA-Z])(\d\d)(.)$");

        Font pinFont = new Font("Courier New", 10);

        public EditEdgeConnectorsForm(
            Machine currentMachine,
            Volumeset currentVolumeSet,
            Volume currentVolume,
            Diagrampage diagramPage) {
            InitializeComponent();

            pageTable = db.getPageTable();
            edgeConnectorTable = db.getEdgeConnectorTable();

            currentPage = pageTable.getByKey(diagramPage.page);
            currentDiagramPage = diagramPage;
            machinePrefix = currentMachine.name.Length >= 4 ?
                currentMachine.name.Substring(0, 2) : "";

            //  Populate the read only data.

            machineTextBox.ReadOnly = true;
            machineTextBox.Text = currentMachine.name;
            volumeTextBox.ReadOnly = true;
            volumeTextBox.Text = currentVolumeSet.machineType + "/" +
                currentVolumeSet.machineSerial + " Volume: " +
                currentVolume.name;
            pageTextBox.Text = currentPage.name;

            rowLabel = currentMachine.rowLabel;

            //  Populate the data grid view..

            populateDialog();
        }

        public void populateDialog() {

            populatingDialog = true;
            originalEdgeConnectionsList = new List<EdgeConnections>();

            edgeConnectorList = edgeConnectorTable.getWhere(
                "WHERE diagramPage='" + currentDiagramPage.idDiagramPage + "'" +
                " ORDER BY reference, edgeconnector.order");


            string lastReference = "";
            dgvEdgeConnectionsList = new List<EdgeConnections>();
            List<String> slots = new List<string>();

            foreach (Edgeconnector edgeConnector in edgeConnectorList) {
                //  If the reference changes, set up the new edgeConnections row,
                //  except for the very first time.
                if (edgeConnector.reference != lastReference) {
                    if (slots.Count > 0) {
                        EdgeConnections newEdge = new EdgeConnections();
                        newEdge.reference = lastReference;
                        newEdge.setSlots(slots.ToArray());
                        dgvEdgeConnectionsList.Add(newEdge);

                        EdgeConnections oldEdge = new EdgeConnections();
                        oldEdge.reference = lastReference;
                        oldEdge.setSlots(slots.ToArray());
                        originalEdgeConnectionsList.Add(oldEdge);
                        slots = new List<string>();
                    }
                    lastReference = edgeConnector.reference;
                }

                //  Now, reformat this entry's slot information.

                CardSlotInfo info = Helpers.getCardSlotInfo(edgeConnector.cardSlot);

                //  We want only the third and fourth characters of the machine name for the
                //  datagridview.  The first two are implied...

                if (info.machineName.Length >= 2) {
                    int start = info.machineName.Length >= 4 ? 2 : info.machineName.Length - 2;
                    info.machineName = info.machineName.Substring(start, 2);
                }

                //  Build the rest of the string, and convert to upper case.

                string newSlot = info.machineName + info.frameName + 
                    info.panelName + info.row + info.column.ToString("D2") +
                    edgeConnector.pin;
                newSlot = newSlot.ToUpper();
                slots.Add(newSlot);
            }

            //  We (ordinarily will) have one last one to add..

            if (slots.Count > 0) {
                EdgeConnections newEdge = new EdgeConnections();
                newEdge.reference = lastReference;
                newEdge.setSlots(slots.ToArray());
                dgvEdgeConnectionsList.Add(newEdge);

                EdgeConnections oldEdge = new EdgeConnections();
                oldEdge.reference = lastReference;
                oldEdge.setSlots(slots.ToArray());
                originalEdgeConnectionsList.Add(oldEdge);
            }

            //EdgeConnections test;


            //for(int row = 0; row <= 4; ++row) {
            //    test = new EdgeConnections();
            //    test.reference = "*" + row.ToString();
            //    test.pin1 = "11D1C" + row.ToString("D2") + "E";
            //    test.pin2 = "11D1C" + row.ToString("D2") + "F";
            //    test.pin3 = "11D1C" + row.ToString("D2") + "G";
            //    test.pin4 = "11D1C" + row.ToString("D2") + "H";
            //    test.pin5 = "11D1C" + row.ToString("D2") + "I";
            //    test.pin6 = "11D1C" + row.ToString("D2") + "J";
            //    test.pin7 = "11D1C" + row.ToString("D2") + "K";
            //    test.pin8 = "11D1C" + row.ToString("D2") + "L";
            //    edgeConnectionsList.Add(test);
            //}

            edgeConnectionsBindingList = new BindingList<EdgeConnections>(
                dgvEdgeConnectionsList);

            edgeConnectorDataGridView.DataSource = edgeConnectionsBindingList;

            //  Set headers' text and widths

            edgeConnectorDataGridView.Columns["reference"].HeaderText = "Ref.";
            edgeConnectorDataGridView.Columns["reference"].Width = 5 * 8;
            for (int pin = 1; pin <= MAXCONNECTIONS; ++pin) {
                string columnName = "pin" + pin.ToString();
                edgeConnectorDataGridView.Columns[columnName].HeaderText = "Connection";
                edgeConnectorDataGridView.Columns[columnName].Width = 10 * 8;
                edgeConnectorDataGridView.Columns[columnName].DefaultCellStyle.Font =
                    pinFont;
                ((DataGridViewTextBoxColumn)
                    edgeConnectorDataGridView.Columns[columnName]).MaxInputLength = 8;
            }

            populatingDialog = false;
        }

        private void edgeConnectorDataGridView_CellValidating(object sender, 
            DataGridViewCellValidatingEventArgs e) {

            //  Skip validation if we are building the datagrid view,
            //  or we are on a header row or a new row, or if the string
            //  value is empty.

            string sv = e.FormattedValue.ToString();
            string message = "";

            if (populatingDialog || e.RowIndex < 0 || sv.Length == 0 ||
                edgeConnectorDataGridView.Rows[e.RowIndex].IsNewRow) {
                return;
            }

            if(e.ColumnIndex == edgeConnectorDataGridView.Columns["reference"].Index) {
                if (sv == null || sv.Length != 1 || !char.IsLetterOrDigit(sv[0])) {
                    message = "Reference must be a single letter or digit (without the '*')" +
                        Environment.NewLine;
                }
            }

            else if(e.ColumnIndex >= edgeConnectorDataGridView.Columns["pin1"].Index &&                
                e.ColumnIndex <= edgeConnectorDataGridView.Columns["pin" + MAXCONNECTIONS].Index) {
                int entryNumber = e.ColumnIndex -
                    edgeConnectorDataGridView.Columns["pin1"].Index + 1;

                Match match = connectionPattern.Match(sv);

                if(!match.Success) {
                    message = "Entry " + entryNumber +
                        ": Connection must match ##.#.##. - MMFPRCCP" +
                        Environment.NewLine;
                }
                else {
                    string rowName = match.Groups[4].Value.ToUpper();
                    string pinName = match.Groups[6].Value.ToUpper();
                    string panelName = match.Groups[3].Value;
                    string machineSuffix = match.Groups[1].Value;
                    //  No try needed below - already validated in RegEx
                    int columnNumber = Int32.Parse(match.Groups[5].Value);
                    if(!IBMSMSPackaging.isValidRowColumn(machinePrefix + machineSuffix,
                        panelName,rowName,columnNumber)) {
                        message = "Entry " + entryNumber + ": Invalid Row/Col. for this Panel " + 
                            rowLabel + " name. " + 
                            Environment.NewLine;
                    }
                    if(Array.IndexOf(Helpers.validPins,pinName[0]) < 0) {
                        message += "Entry " + entryNumber + ": Invalid pin name." +
                            Environment.NewLine;
                    }                    
                }                
            }

            edgeConnectorDataGridView.Rows[e.RowIndex].ErrorText = message;

            if (message.Length > 0) {
                e.Cancel = true;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            //  Bye bye...
            this.Close();
        }

        private void applyButton_Click(object sender, EventArgs e) {

            string message = "";

            //  The entries in the data grid view have already been checked, so
            //  not much to do here, other than manage the confirmation/update
            //  process...

            message = applyUpdates(false);

            //DialogResult status = MessageBox.Show("Please confirm the following updates:\n\n" +
            //    message, "Confirm Updates",
            //    MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            ImporterLogDisplayForm importerLogDisplayForm;
            importerLogDisplayForm = new ImporterLogDisplayForm(
                "Please confirm the following updates:", message);

            DialogResult status = importerLogDisplayForm.ShowDialog();

            if(status == DialogResult.OK) {
                message = applyUpdates(true);
                //MessageBox.Show("Updates Completed:\n\n" +
                //    message, "Updates Completed",
                //    MessageBoxButtons.OK, MessageBoxIcon.Information);
                importerLogDisplayForm = new ImporterLogDisplayForm(
                    "Update Status:", message);
                importerLogDisplayForm.ShowDialog();
            }
        }

        private string applyUpdates(bool doUpdate) {
            
            string message = "";
            string outMessage = "";
            string[] slots;
            bool errors = false;

            if(doUpdate) {
                db.BeginTransaction();
            }

            //  First, delete the originals...

            foreach(EdgeConnections connector in originalEdgeConnectionsList) {
                message += deleteConnectors(doUpdate, connector.reference);
            }

            //  Now re-add everything that is left (even if nothing changed). 

            foreach(EdgeConnections connector in dgvEdgeConnectionsList) {

                //  Get the individual slots from the data grid row entry.

                slots = connector.getSlots();

                for(int order = 0; order < slots.Length; ++ order) {

                    //  Only process those entries that have a connection string...

                    if(slots[order] != null && slots[order].Length > 0) {

                        Edgeconnector newEdge = new Edgeconnector();
                        CardSlotInfo cardSlotInfo = new CardSlotInfo();
                        Match match = connectionPattern.Match(slots[order]);

                        if(!match.Success) {
                            message += "ERROR IN PATTERN MATCH against " +
                                slots[order] + " in reference " + connector.reference +
                                Environment.NewLine;
                            errors = true;
                            continue;
                        }

                        //  Get the info to look up (and possibly add) the card slot,
                        //  and do the lookup.

                        cardSlotInfo.machineName = machinePrefix + match.Groups[1].Value;
                        cardSlotInfo.frameName = match.Groups[2].Value.ToUpper();
                        //  1410 Dependency Below?
                        cardSlotInfo.gateName = cardSlotInfo.frameName;
                        cardSlotInfo.panelName = match.Groups[3].Value.ToUpper();
                        cardSlotInfo.row = match.Groups[4].Value.ToUpper();
                        cardSlotInfo.column = int.Parse(match.Groups[5].Value);

                        newEdge.cardSlot = Helpers.getOrAddCardSlotKey(
                            doUpdate, cardSlotInfo, out outMessage);

                        if(doUpdate && newEdge.cardSlot == 0) {
                            message += "ERROR:  Unable to get or add card slot for " +
                                slots[order] + " (" + outMessage + ")" +
                                Environment.NewLine;
                            errors = true;
                            continue;
                        }
                        else if(outMessage.Length > 0) {
                            message += outMessage + Environment.NewLine;
                        }

                        //  Time for the actual update...

                        if (doUpdate) {
                            newEdge.idEdgeConnector = IdCounter.incrementCounter();
                            newEdge.diagramPage = currentDiagramPage.idDiagramPage;
                            newEdge.reference = connector.reference.ToUpper();
                            newEdge.pin = match.Groups[6].Value.ToUpper();
                            newEdge.order = order;
                            edgeConnectorTable.insert(newEdge);
                        }

                        message += (doUpdate ? "Added" : "Adding") +
                            " Edge Connector " + connector.reference +
                            " to page " + currentPage.name +
                            (doUpdate ? " Database ID=" + newEdge.idEdgeConnector : "") +
                            Environment.NewLine;
                    }
                }
            }

            if(doUpdate) {
                if(errors) {
                    db.CancelTransaction();
                }
                else {
                    db.CommitTransaction();

                    //  Repopulate the grid with the updated information.

                    populateDialog();
                }
            }

            return message;
        }

        //  Method to delete edge connectors - originating either from direct deletes,
        //  or via modifies (which are processed as delete/re-adds)

        private string deleteConnectors(bool doUpdate, string reference) {

            string message = "";

            List<Edgeconnector> connectorsToDelete = edgeConnectorTable.getWhere(
                "WHERE diagramPage='" + currentDiagramPage.idDiagramPage + "'" +
                " AND reference='" + reference + "'" +
                " ORDER BY edgeconnector.order");

            foreach(Edgeconnector connector in connectorsToDelete) {
                if(doUpdate) {
                    edgeConnectorTable.deleteByKey(connector.idEdgeConnector);
                }
                message += (doUpdate ? "Deleted" : "Deleting") +
                    " Edge Connector " + reference +
                    " (Database ID " + connector.idEdgeConnector + ")" +
                    " from page " + currentPage.name + Environment.NewLine;
            }

            return (message);

        }

    }

    //  This class is used to hold information about a single
    //  edge connection, as displayed in the data grid view.
    //  The idea is that this will allow us to use an array.

    public class EdgeConnections {

        public string reference { get; set; }
        public string pin1 { get; set; }
        public string pin2 { get; set; }
        public string pin3 { get; set; }
        public string pin4 { get; set; }
        public string pin5 { get; set; }
        public string pin6 { get; set; }
        public string pin7 { get; set; }
        public string pin8 { get; set; }
        public string pin9 { get; set; }
        public string pin10 { get; set; }

        public void setSlots(string[] slots) {
            pin1 = pin2 = pin3 = pin4 = pin5 =
                pin6 = pin7 = pin8 = pin9 = pin10 = "";
            for(int i = 0; i < slots.Length; ++i) { 
                switch(i) {
                    case 0:
                        pin1 = slots[i];
                        break;
                    case 1:
                        pin2 = slots[i];
                        break;
                    case 2:
                        pin3 = slots[i];
                        break;
                    case 3:
                        pin4 = slots[i];
                        break;
                    case 4:
                        pin5 = slots[i];
                        break;
                    case 5:
                        pin6 = slots[i];
                        break;
                    case 6:
                        pin7 = slots[i];
                        break;
                    case 7:
                        pin8 = slots[i];
                        break;
                    case 8:
                        pin9 = slots[i];
                        break;
                    case 9:
                        pin10 = slots[i];
                        break;
                }
            }
        }

        public string [] getSlots() {
            return new string [] {
                pin1, pin2, pin3, pin4,
                pin5, pin6, pin7, pin8,
                pin9, pin10,
            };
        }
    }
}
