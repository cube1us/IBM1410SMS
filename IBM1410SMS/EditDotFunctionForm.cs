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
    public partial class EditDotFunctionForm : Form
    {

        DBSetup db = DBSetup.Instance;

        Table<Page> pageTable;
        Table<Dotfunction> dotFunctionTable;
        Table<Connection> connectionTable;
        Table<Diagramblock> diagramBlockTable;
        Table<Sheetedgeinformation> sheetEdgeInformationTable;

        Page currentPage;
        Diagrampage currentDiagramPage;
        Machine currentMachine;
        Volumeset currentVolumeSet;
        Volume currentVolume;
        string machinePrefix;

        List<Connection> connectionList;

        Dotfunction currentDotFunction;

        public EditDotFunctionForm(
            Dotfunction dotFunction,
            Machine machine,
            Volumeset volumeSet,
            Volume volume,
            Diagrampage diagramPage,
            string diagramRow, int diagramColumn) {

            InitializeComponent();

            pageTable = db.getPageTable();
            dotFunctionTable = db.getDotFunctionTable();
            connectionTable = db.getConnectionTable();
            diagramBlockTable = db.getDiagramBlockTable();
            sheetEdgeInformationTable = db.getSheetEdgeInformationTable();

            //  Fill in constant data.

            currentMachine = machine;
            currentVolumeSet = volumeSet;
            currentVolume = volume;
            currentPage = pageTable.getByKey(diagramPage.page);
            currentDiagramPage = diagramPage;
            machinePrefix = currentMachine.name.Length >= 4 ?
                currentMachine.name.Substring(0, 2) : "";

            machineTextBox.ReadOnly = true;
            machineTextBox.Text = currentMachine.name;
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

            //  Set up the Edit button click handlers.

            connectionsDataGridView.CellClick += new DataGridViewCellEventHandler(
                    editButton_Click);

            if (dotFunction ==  null) {
                dotFunction = new Dotfunction();
                dotFunction.diagramPage = diagramPage.idDiagramPage;
                dotFunction.diagramRowTop = diagramRow;
                dotFunction.diagramColumnToLeft = diagramColumn;
                dotFunction.logicFunction = "A";
                dotFunction.checkExempt = 0;
                dotFunction.forcedLogicFunction = "";
                deleteButton.Visible = false;
            }
            else {
                deleteButton.Visible = true;                
            }

            //  Legacy database rows may not have all fields set...

            if (dotFunction.forcedLogicFunction == null) {
                dotFunction.forcedLogicFunction = "";
            }

            if(dotFunction.logicFunction == "A") {
                andRadioButton.Select();
            }
            else {
                orRadioButton.Select();
            }

            exemptCheckBox.Checked = (dotFunction.checkExempt != 0);
            forceLogicFunction.Text = dotFunction.forcedLogicFunction;

            currentDotFunction = dotFunction;

            populateConnections();
        }

        private void populateConnections() {
            //  Populate the connections data grid view.  Inputs
            //  (those whose fromDotFunction is 0) come first.

            List<Connection> connectionsList = null;
            if (currentDotFunction.idDotFunction != 0) {

                connectionsList = connectionTable.getWhere(
                    "WHERE fromDotFunction='" + currentDotFunction.idDotFunction + "'" +
                    " OR toDotFunction='" + currentDotFunction.idDotFunction + "'" +
                    " ORDER BY fromDotFunction");
            }
            else {
                connectionsList = new List<Connection>();
            }

            //  (Class ConnectionInfo is defined in EditConnectionsForm)

            List<ConnectionInfo> connectionInfoList = new List<ConnectionInfo>();

            foreach (Connection connection in connectionsList) {

                ConnectionInfo info = new ConnectionInfo();

                info.info3 = "";
                info.info4 = "";
                info.edit = "(N/A)";
                info.idConnection = connection.idConnection;

                if (connection.toDotFunction != 0) {
                    //  Input TO DOT function
                    info.connectionType = connection.from;
                    info.inputOutput = "I";
                    switch (connection.from) {
                        case "P":
                            Diagramblock block = diagramBlockTable.getByKey(
                                connection.fromDiagramBlock);
                            info.info1 = block.diagramColumn.ToString() + block.diagramRow;
                            info.info2 = connection.fromPin;
                            info.info3 = connection.fromPhasePolarity;
                            break;
                        case "D":
                            Dotfunction dot = dotFunctionTable.getByKey(
                                connection.fromDotFunction);
                            info.info1 = dot.diagramColumnToLeft.ToString() +
                                dot.diagramRowTop;
                            info.info2 = dot.logicFunction;
                            break;
                        case "E":
                            info.edit = "Edit";
                            Sheetedgeinformation edge = sheetEdgeInformationTable.getByKey(
                                connection.fromEdgeOriginSheet);
                            info.info1 = Helpers.getDiagramPageName(edge.diagramPage);
                            edge = sheetEdgeInformationTable.getByKey(
                                connection.fromEdgeSheet);
                            if (connection.fromEdgeConnectorReference != null &&
                                connection.fromEdgeConnectorReference.Length > 0) {
                                info.info2 = "*" + connection.fromEdgeConnectorReference;
                            }
                            info.info4 = edge.row + " / " + edge.signalName;
                            break;
                        default:
                            info.info1 = "???";
                            info.info2 = "???";
                            break;
                    }
                }
                else {
                    //  Output FROM DOT function
                    info.pin = connection.fromPin + connection.fromLoadPin;
                    info.connectionType = connection.to;
                    info.inputOutput = "O";
                    switch (connection.to) {
                        case "P":
                            Diagramblock block = diagramBlockTable.getByKey(
                                connection.toDiagramBlock);
                            info.info1 = block.diagramColumn.ToString() + block.diagramRow;
                            info.info2 = connection.toPin;
                            if (connection.toEdgeConnectorReference != null &&
                                connection.toEdgeConnectorReference.Length > 0) {
                                info.info4 = "*" + connection.toEdgeConnectorReference;
                            }
                            if (connection.toEdge2ndConnectorReference != null &&
                                connection.toEdge2ndConnectorReference.Length > 0) {
                                info.info4 += ", *" + connection.toEdge2ndConnectorReference;
                            }
                            break;
                        case "D":
                            Dotfunction dot = dotFunctionTable.getByKey(
                                connection.toDotFunction);
                            info.info1 = dot.diagramColumnToLeft.ToString() +
                                dot.diagramRowTop;
                            info.info2 = dot.logicFunction;
                            break;
                        case "E":
                            info.edit = "Edit";
                            Sheetedgeinformation edge = sheetEdgeInformationTable.getByKey(
                                connection.toEdgeDestinationSheet);
                            info.info1 = Helpers.getDiagramPageName(edge.diagramPage);
                            edge = sheetEdgeInformationTable.getByKey(
                                connection.toEdgeSheet);
                            if (connection.toEdgeConnectorReference != null &&
                                connection.toEdgeConnectorReference.Length > 0) {
                                info.info2 = "*" + connection.toEdgeConnectorReference;
                            }
                            if (connection.toEdge2ndConnectorReference != null &&
                                connection.toEdge2ndConnectorReference.Length > 0) {
                                info.info2 += ", *" + connection.toEdge2ndConnectorReference;
                            }
                            info.info4 = edge.row + " / " + edge.signalName;
                            break;
                        default:
                            info.info1 = "???";
                            info.info2 = "???";
                            break;
                    }
                }

                connectionInfoList.Add(info);
            }

            //  Now build the datagridview itself!

            //  Note:  In reality, types "D" and "E" should never occur on input,
            //  and type "D" should never occur on output.
            //  So, set the headers accordingly.  Also, the "Pin" 
            //  column for the DOT connection itself must always be blank.

            //   Point wand, "obliviate"
            connectionsDataGridView.DataSource = null;
            connectionsDataGridView.Rows.Clear();
            connectionsDataGridView.Columns.Clear();
            connectionsDataGridView.DataSource = connectionInfoList;

            DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
            buttonColumn.Name = "EditButton";
            buttonColumn.HeaderText = "Edit";
            // buttonColumn.Text = "Edit";
            buttonColumn.UseColumnTextForButtonValue = false;
            buttonColumn.DataPropertyName = "edit";
            buttonColumn.Width = 40;
            connectionsDataGridView.Columns.Insert(0, buttonColumn);

            connectionsDataGridView.Columns["idConnection"].Visible = false;
            connectionsDataGridView.Columns["pin"].Visible = false;
            connectionsDataGridView.Columns["edit"].Visible = false;
            connectionsDataGridView.Columns["inputOutput"].Width = 40;
            connectionsDataGridView.Columns["inputOutput"].HeaderText = "In/Out";
            // connectionsDataGridView.Columns["pin"].HeaderText = "Pin";
            // connectionsDataGridView.Columns["pin"].Width = 30;
            connectionsDataGridView.Columns["connectionType"].HeaderText = "Type";
            connectionsDataGridView.Columns["connectionType"].Width = 40;
            connectionsDataGridView.Columns["info1"].HeaderText = "Coord.";
            connectionsDataGridView.Columns["info1"].Width = 60;
            connectionsDataGridView.Columns["info2"].HeaderText = "Pin/*Ref";
            connectionsDataGridView.Columns["info2"].Width = 50;
            connectionsDataGridView.Columns["info3"].HeaderText = "Polarity";
            connectionsDataGridView.Columns["info3"].Width = 45;
            connectionsDataGridView.Columns["info4"].HeaderText = "Row / Signal Name";
            connectionsDataGridView.Columns["info4"].Width = 200;
        }

        private void editButton_Click(object sender, DataGridViewCellEventArgs e) {

            if (e.RowIndex < 0 ||
                e.ColumnIndex != connectionsDataGridView.Columns["EditButton"].Index) {
                return;
            }

            int connectionKey = (int)connectionsDataGridView.Rows[e.RowIndex].Cells["idConnection"].Value;

            //  If  this is not a connection to or from an Edge, ignore the click.
            
            if ((string)connectionsDataGridView.Rows[e.RowIndex].Cells["connectionType"].Value != "E") {
                return;
            }

            EditDotFunctionEdgeConnectionForm EditDotFunctionEdgeConnectionForm = 
                new EditDotFunctionEdgeConnectionForm(
                    currentDotFunction, currentMachine, currentVolumeSet, currentVolume,
                    currentDiagramPage, connectionKey);

            EditDotFunctionEdgeConnectionForm.ShowDialog();
            populateConnections();
        }


        private void cancelButton_Click(object sender, EventArgs e) {
            //   Buh bye...
            this.Close();
        }

        private void deleteButton_Click(object sender, EventArgs e) {

            //  Warn the user if there are any connections that refer
            //  to this dot function.

            if(currentDotFunction.idDotFunction == 0) {
                MessageBox.Show("DOT Function is new - nothing to delete",
                    "New DOT Function",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                //  Nothing to do, so close the dialog...
                this.Close();
            }

            connectionList = connectionTable.getWhere(
                "WHERE fromDotFunction='" + currentDotFunction.idDotFunction + "'" +
                " OR toDotFunction='" + currentDotFunction.idDotFunction + "'");

            if (connectionList.Count > 0) {
                DialogResult status = MessageBox.Show("DOT Function " +
                    "(Database ID " + currentDotFunction.idDotFunction +
                    ") is referred to by " +
                    connectionList.Count + " connections.  Proceed with delete " +
                    "(including the connections)?", "DOT Function in use",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (status != DialogResult.OK) {
                    //  User hit cancel.  Close the update.
                    this.Close();
                    return;
                }
            }

            string message = "Deleted DOT Function at Column " +
                diagramColumnTextBox.Text +
                ", Row " + diagramRowTextBox.Text +
                " (Database ID " + currentDotFunction.idDotFunction +
                ") Deleted.\n\n";

            db.BeginTransaction();
            dotFunctionTable.deleteByKey(currentDotFunction.idDotFunction);
            foreach(Connection connection in connectionList) {
                connectionTable.deleteByKey(connection.idConnection);
                message += "Connection (Database ID " +
                    connection.idConnection + ") deleted.\n";
            }

            db.CommitTransaction();
            MessageBox.Show(message, "Update Completed",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }


        private void addOutputButton_Click(object sender, EventArgs e) {

            //  If the current DOT function has not yet been created, create it now...

            if(currentDotFunction.idDotFunction == 0) {
                applyButton_Click(sender, e);
            }

            //  Then go to the editing form...

            EditDotFunctionEdgeConnectionForm EditDotFunctionEdgeConnectionForm =
                new EditDotFunctionEdgeConnectionForm(
                    currentDotFunction, currentMachine, currentVolumeSet, currentVolume,
                    currentDiagramPage, 0);

            EditDotFunctionEdgeConnectionForm.ShowDialog();

            populateConnections();
        }


        private void applyButton_Click(object sender, EventArgs e) {

            string action = "Updated";

            currentDotFunction.logicFunction =
                andRadioButton.Checked ? "A" : "O";
            currentDotFunction.checkExempt = exemptCheckBox.Checked ? 1 : 0;
            currentDotFunction.forcedLogicFunction = forceLogicFunction.Text;
            if(currentDotFunction.idDotFunction == 0) {
                action = "Added";
                currentDotFunction.idDotFunction = IdCounter.incrementCounter();
                dotFunctionTable.insert(currentDotFunction);
            }
            else {
                dotFunctionTable.update(currentDotFunction);
            }

            MessageBox.Show(action + " DOT Function at Column " +
                    diagramColumnTextBox.Text +
                    ", Row " + diagramRowTextBox.Text +
                    " (Database ID " + currentDotFunction.idDotFunction +
                    ")");
            this.Close();
        }
    }
}
