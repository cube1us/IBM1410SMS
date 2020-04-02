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
    public partial class EditTieDownsForm : Form
    {

        DBSetup db = DBSetup.Instance;

        Table<Machine> machineTable;
        Table<Frame> frameTable;
        Table<Machinegate> machineGateTable;
        Table<Panel> panelTable;

        Table<Volumeset> volumeSetTable;
        Table<Volume> volumeTable;

        Table<Eco> ecoTable;
        Table<Page> pageTable;
        Table<Cardslot> cardSlotTable;
        Table<Cardtype> cardTypeTable;
        Table<Feature> featureTable;
        Table<Diagrampage> diagramPageTable;
        Table<Tiedown> tieDownTable;

        List<Machine> machineList;
        List<Frame> frameList;
        List<Machinegate> machineGateList;
        List<Panel> panelList;

        List<Volumeset> volumeSetList;
        List<Volume> volumeList;
        List<Eco> ecoList;
        List<Feature> featureList;
        List<Cardtype> cardTypeList;
        List<Tiedown> tieDownList;

        List<Tiedown> deletedTieDownList = new List<Tiedown>();
        BindingList<Tiedown> tieDownBindingList;

        Machine currentMachine = null;
        Volumeset currentVolumeSet = null;

        string frameLabel = "Frame";
        string gateLabel = "Gate";
        string panelLabel = "Panel";
        string rowLabel = "Row";
        string columnLabel = "Column";

        bool populatingDataGridView = false;
        int selectedVolumeSet = 0;


        public EditTieDownsForm() {
            InitializeComponent();

            machineTable = db.getMachineTable();
            frameTable = db.getFrameTable();
            machineGateTable = db.getMachineGateTable();
            panelTable = db.getPanelTable();

            volumeSetTable = db.getVolumeSetTable();
            volumeTable = db.getVolumeTable();

            ecoTable = db.getEcoTable();
            pageTable = db.getPageTable();
            cardTypeTable = db.getCardTypeTable();
            cardSlotTable = db.getCardSlotTable();
            featureTable = db.getFeatureTable();
            diagramPageTable = db.getDiagramPageTable();
            tieDownTable = db.getTieDownTable();

            //  The card type list is static, so we can fill that one in.

            cardTypeList = cardTypeTable.getWhere(
                "ORDER BY cardtype.type");

            //  The machine combo box is static, so we can fill that one out now.

            machineList = machineTable.getWhere(
                "ORDER by machine.name");

            //  Fill in the machine combo box, and remember which machine
            //  we started out with.

            machineComboBox.DataSource = machineList;

            //  Retrieve the last machine we worked with, and select it, if any.

            if (machineList.Count > 0) {

                string lastMachine = Parms.getParmValue("machine");
                if (lastMachine.Length > 0) {
                    currentMachine = machineList.Find(
                        x => x.idMachine.ToString() == lastMachine);
                }

                if (currentMachine == null || currentMachine.idMachine == 0) {
                    currentMachine = machineList[0];
                }

                machineComboBox.SelectedItem = currentMachine;
            }
            else {
                currentMachine = null;
            }

            //  The volume set combo box is also static, but it has both
            //  text fields displayed, and we employ its index for access.

            volumeSetList = volumeSetTable.getWhere(
                "ORDER by machineType, machineSerial");

            //  Also, while we are at it, if the last volume set we used is in
            //  the list, select that one, otherwise select the first one, unless
            //  the list was empty in the first place.

            if (volumeSetList.Count > 0) {
                string lastVolume = Parms.getParmValue("volume set");

                int index = 0;
                foreach (Volumeset vset in volumeSetList) {
                    volumeSetComboBox.Items.Add(
                        "Machine " + vset.machineType +
                        ", S/N " + vset.machineSerial);
                    if (vset.idVolumeSet.ToString() == lastVolume) {
                        selectedVolumeSet = index;
                    }
                    ++index;
                }
                currentVolumeSet = volumeSetList[0];
            }
            else {
                currentVolumeSet = null;
            }

            //  During this constructor, we set the selected index to -1

            //  I don't understand why, but I found that if I tried to
            //  point to the "correct" entry, the datagrid view gets 
            //  messed up, with missing data in some columns, and
            //  columnts that should be removed were not getting removed.

            volumeSetComboBox.SelectedIndex = -1;

            populateTieDowns(currentMachine, currentVolumeSet);
            
        }


        //  Method to populate the data grid view

        private void populateTieDowns(Machine machine, Volumeset volumeSet) {

            populatingDataGridView = true;

            frameLabel = machine.frameLabel;
            gateLabel = machine.gateLabel;
            panelLabel = machine.panelLabel;
            rowLabel = machine.rowLabel;
            columnLabel = machine.columnLabel;

            //  Clear out any existing data grid view

            tieDownsDataGridView.DataSource = null;
            tieDownsDataGridView.Columns.Clear();
            tieDownsDataGridView.Rows.Clear();

            List<Tiedown> deletedTieDownList = new List<Tiedown>();

            //  We have to have a machine and volume set to proceed.

            if(machine == null || volumeSet == null) {
                return;
            }

            //  Populate the ECO and feature lists, which are dependent upon machine.

            ecoList = ecoTable.getWhere(
                "WHERE machine='" + machine.idMachine + "'" +
                " ORDER BY eco");

            featureList = featureTable.getWhere(
                "WHERE machine='" + machine.idMachine + "'" +
                " ORDER BY code");

            //  The lists of frames, machine gates, panels and slots
            //  are also machine dependent.

            frameList = frameTable.getWhere(
                "WHERE machine='" + machine.idMachine + "'" +
                " ORDER BY frame.name");

            machineGateList = new List<Machinegate>();
            foreach(Frame f in frameList) {
                machineGateList.AddRange(
                    machineGateTable.getWhere(
                        "WHERE frame='" + f.idFrame + "'" +
                        " ORDER BY machinegate.name"));
            }

            panelList = new List<Panel>();
            foreach(Machinegate mg in machineGateList) {
                panelList.AddRange(
                    panelTable.getWhere(
                        "WHERE gate='" + mg.idGate + "'" +
                        " ORDER BY panel"));
            }

            //  Next, we have to hunt down all of the diagram pages
            //  that apply to this volume set.  So Volume Set =>
            //  Volume => Page => Diagram Page (=> tie downs)

            volumeList = volumeTable.getWhere(
                "WHERE volume.set='" + volumeSet.idVolumeSet + "'");

            //  Now build a list of all of the pages in all of the volumes.
            //  We don't need to remember which volume they are in at this
            //  point.

            List<Page> pageList = new List<Page>();
            foreach(Volume v in volumeList) {
                pageList.AddRange(pageTable.getWhere(
                    "WHERE machine='" + machine.idMachine + "'" +
                    " AND volume='" + v.idVolume + "'"));
            }

            //  Sort the page list by page name, so that the tie downs
            //  come out in page name order.

            pageList.Sort(
                delegate (Page x, Page y)
                {
                    if (x.name == null && y.name == null) return 0;
                    else if (x.name == null) return -1;
                    else if (y.name == null) return 1;
                    else return (x.name.CompareTo(y.name));
                }
            );

            //  From that build a list of the diagram pages.

            List<Diagrampage> diagramPageList = new List<Diagrampage>();
            foreach(Page p in pageList) {
                diagramPageList.AddRange(diagramPageTable.getWhere(
                    "WHERE diagrampage.page='" + p.idPage + "'"));
            }

            //  FINALLY, we can actually build the list of tie downs
            //  that are related to the selected machine and volume set

            tieDownList = new List<Tiedown>();
            foreach(Diagrampage dp in diagramPageList) {
                tieDownList.AddRange(tieDownTable.getWhere(
                    "WHERE diagramPage='" + dp.idDiagramPage + "'"));
            }

            //  Now we can actually build our datagridview...

            tieDownBindingList = new BindingList<Tiedown>(tieDownList);
            tieDownBindingList.AllowNew = true;
            tieDownBindingList.AllowRemove = true;
            tieDownBindingList.AllowEdit = true;
            tieDownsDataGridView.DataSource = tieDownBindingList;

            //  Hide columns that the user does not ened to see.

            tieDownsDataGridView.Columns["idTieDown"].Visible = false;
            tieDownsDataGridView.Columns["modified"].Visible = false;


            //  Set up header text and width for the simple columns.

            tieDownsDataGridView.Columns["Pin"].HeaderText = "Pin";
            tieDownsDataGridView.Columns["Pin"].Width = 4 * 8;
            tieDownsDataGridView.Columns["Note"].HeaderText = "Note";
            tieDownsDataGridView.Columns["Note"].Width = 20 * 8;
            tieDownsDataGridView.Columns["otherPin"].HeaderText = "Other Pin";
            tieDownsDataGridView.Columns["otherPin"].Width = 6 * 8;

            //  Remove the columns we won't use, last to first.

            tieDownsDataGridView.Columns.Remove("featureWith");
            tieDownsDataGridView.Columns.Remove("featureWithout");
            tieDownsDataGridView.Columns.Remove("cardSlot");
            tieDownsDataGridView.Columns.Remove("cardType");
            tieDownsDataGridView.Columns.Remove("diagramPage");
            tieDownsDataGridView.Columns.Remove("checkMark");

            //  Build and insert our new columns.

            //  In place of the diagramPage column, we have a text column
            //  with the page number.  (a pull down list would be unwiedly).
            //  The text gets filled in later.

            DataGridViewTextBoxColumn diagramPageColumn =
                new DataGridViewTextBoxColumn();
            diagramPageColumn.HeaderText = "Page";
            diagramPageColumn.Width = 8 * 8;
            diagramPageColumn.Name = "page";
            tieDownsDataGridView.Columns.Insert(3, diagramPageColumn);

            //  Same idea with the cardType column.

            DataGridViewTextBoxColumn cardTypeColumn =
                new DataGridViewTextBoxColumn();
            cardTypeColumn.HeaderText = "Card Type";
            cardTypeColumn.Width = 5 * 8;
            cardTypeColumn.Name = "type";
            tieDownsDataGridView.Columns.Insert(4, cardTypeColumn);

            //  Card Slot turns into Five boxes, for frame, gate,
            //  panel, row and column.

            DataGridViewTextBoxColumn frameColumn =
                new DataGridViewTextBoxColumn();
            frameColumn.Name = "frame";
            frameColumn.Width = Math.Max(5,frameLabel.Length) * 8;
            frameColumn.HeaderText = frameLabel;


            DataGridViewTextBoxColumn gateColumn =
                new DataGridViewTextBoxColumn();
            gateColumn.Name = "gate";
            gateColumn.Width = Math.Max(4,gateLabel.Length) * 8;
            gateColumn.HeaderText = gateLabel;


            DataGridViewTextBoxColumn panelColumn =
                new DataGridViewTextBoxColumn();
            panelColumn.HeaderText = panelLabel;
            panelColumn.Width = 5 * 8;
            panelColumn.Name = "panel";

            DataGridViewTextBoxColumn rowColumn =
                new DataGridViewTextBoxColumn();
            rowColumn.HeaderText = rowLabel.Length <= 4 ?
                rowLabel :  rowLabel.Substring(0,3) + ".";
            rowColumn.Width = 4 * 8;
            rowColumn.Name = "row";

            DataGridViewTextBoxColumn columnColumn =
                new DataGridViewTextBoxColumn();
            columnColumn.HeaderText = columnLabel.Length <= 4 ?
                columnLabel : columnLabel.Substring(0, 3) + ".";
            columnColumn.Width = 4 * 8;
            columnColumn.Name = "column";

            tieDownsDataGridView.Columns.Insert(5, frameColumn);
            tieDownsDataGridView.Columns.Insert(6, gateColumn);
            tieDownsDataGridView.Columns.Insert(7, panelColumn);
            tieDownsDataGridView.Columns.Insert(8, rowColumn);
            tieDownsDataGridView.Columns.Insert(9, columnColumn);

            //  As with page and card type, a feature pull down would be
            //  unwieldy, so these turn into text boxes as well...

            DataGridViewTextBoxColumn featureWithoutColumn =
                new DataGridViewTextBoxColumn();
            featureWithoutColumn.HeaderText = "W/O";
            featureWithoutColumn.Width = 5 * 8;
            featureWithoutColumn.Name = "fwithout";
            tieDownsDataGridView.Columns.Insert(10, featureWithoutColumn);

            DataGridViewTextBoxColumn featureWithColumn =
                new DataGridViewTextBoxColumn();
            featureWithColumn.HeaderText = "With";
            featureWithColumn.Width = 5 * 8;
            featureWithColumn.Name = "fwith";
            tieDownsDataGridView.Columns.Insert(11, featureWithColumn);

            //  Checked is a little different, but the same
            //  general idea, of replacing the original auto selection
            //  with a special column type that maps the data back
            //  correctly, because the underlying data is int, not bool

            DataGridViewCheckBoxColumn checkedColumn =
                new DataGridViewCheckBoxColumn();
            checkedColumn.DataPropertyName = "checkMark";
            checkedColumn.HeaderText = "Checked";
            checkedColumn.TrueValue = 1;
            checkedColumn.FalseValue = 0;
            checkedColumn.Width = 5 * 8;
            tieDownsDataGridView.Columns.Insert(12, checkedColumn);

            //  Now we fill in the data.

            int rowIndex = 0;
            foreach(Tiedown tieDown in tieDownList) {
                ((DataGridViewTextBoxCell)
                    (tieDownsDataGridView.Rows[rowIndex].Cells["page"])).Value =
                    Helpers.getDiagramPageName(tieDown.diagramPage);
                ((DataGridViewTextBoxCell)
                    (tieDownsDataGridView.Rows[rowIndex].Cells["type"])).Value =
                    Helpers.getCardTypeType(tieDown.cardType);

                CardSlotInfo cardSlotInfo = Helpers.getCardSlotInfo(tieDown.cardSlot);
                ((DataGridViewTextBoxCell)
                    (tieDownsDataGridView.Rows[rowIndex].Cells["frame"])).Value =
                        cardSlotInfo.frameName;
                ((DataGridViewTextBoxCell)
                    (tieDownsDataGridView.Rows[rowIndex].Cells["gate"])).Value =
                        cardSlotInfo.gateName;
                ((DataGridViewTextBoxCell)
                    (tieDownsDataGridView.Rows[rowIndex].Cells["panel"])).Value =
                        cardSlotInfo.panelName;
                ((DataGridViewTextBoxCell)
                    (tieDownsDataGridView.Rows[rowIndex].Cells["row"])).Value =
                        cardSlotInfo.row;
                ((DataGridViewTextBoxCell)
                    (tieDownsDataGridView.Rows[rowIndex].Cells["column"])).Value =
                        cardSlotInfo.column;  

                ((DataGridViewTextBoxCell)
                    (tieDownsDataGridView.Rows[rowIndex].Cells["fwithout"])).Value =
                    Helpers.getFeatureCode(tieDown.featureWithout);
                ((DataGridViewTextBoxCell)
                    (tieDownsDataGridView.Rows[rowIndex].Cells["fwith"])).Value =
                    Helpers.getFeatureCode(tieDown.featureWith);

                ++rowIndex;
            }

            //  Reset all the modified flags...

            foreach(Tiedown tieDown in tieDownList) {
                tieDown.modified = false;
            }

            populatingDataGridView = false;
        }

        private void machineComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            currentMachine = (Machine)machineComboBox.SelectedItem;
            populateTieDowns(currentMachine, currentVolumeSet);
        }

        private void volumeSetComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (volumeSetComboBox.SelectedIndex >= 0) {
                currentVolumeSet = volumeSetList[volumeSetComboBox.SelectedIndex];
                populateTieDowns(currentMachine, currentVolumeSet);
            }
        }

        private void tieDownsDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {

            //  Skip validation if we are building the datagrid view,
            //  or we are on a header row or a new row, or if the string
            //  value is empty.

            string sv = e.FormattedValue.ToString();
            int v;
            string message = "";

            if(populatingDataGridView || e.RowIndex < 0 || sv.Length == 0 ||
                tieDownsDataGridView.Rows[e.RowIndex].IsNewRow) {
                return;
            }

            //  Different columns get different validations.

            if (e.ColumnIndex == tieDownsDataGridView.Columns["gate"].Index) {
                //  Gate should be numeric <= 99 or a single letter
                if(!(int.TryParse(sv, out v) && v > 0 && v <= 99) && 
                   !(sv.Length == 1 && char.IsLetter(sv[0]))) {
                    message = gateLabel + " must be numeric 1-99 or a single letter";
                    e.Cancel = true;
                }
            }

            else if (e.ColumnIndex == tieDownsDataGridView.Columns["frame"].Index) {
                //  Frame should also be numeric <= 99 or a single letter
                if (!(int.TryParse(sv, out v) && v > 0 && v <= 99) &&
                   !(sv.Length == 1 && char.IsLetter(sv[0]))) {
                    message = frameLabel + " must be numeric 1-99 or a single letter";
                    e.Cancel = true;
                }

            }

            else if (e.ColumnIndex == tieDownsDataGridView.Columns["panel"].Index) {
                //  Panel should be numeric <= 99
                if (!(int.TryParse(sv, out v) && v > 0 && v <= 99) &&
                    !(sv.Length == 1 && char.IsLetter(sv[0]))) { 
                    message = panelLabel + " must be numeric 1-99 or a single letter";
                    e.Cancel = true;
                }
            }

            else if (e.ColumnIndex == tieDownsDataGridView.Columns["row"].Index) {
                if(sv.Length != 1 || 
                    Array.IndexOf(Helpers.validRows,sv.ToUpper()) < 0) {
                    message = rowLabel + " must be a valid Card Slot Row";
                    e.Cancel = true;
                }
            }

            else if (e.ColumnIndex == tieDownsDataGridView.Columns["column"].Index) {
                //  Column should be numeric <= 99
                if (!(int.TryParse(sv, out v) && v > 0 && v <= 99)) {
                    message = columnLabel + " must be numeric 1-99";
                    e.Cancel = true;
                }
            }

            else if (e.ColumnIndex == tieDownsDataGridView.Columns["pin"].Index) {
                if (sv.Length != 1 ||
                    Array.IndexOf(Helpers.validPins, Char.ToUpper(sv[0])) < 0) {
                    message = "Pin must be a valid pin name";
                    e.Cancel = true;
                }
            }

            else if (e.ColumnIndex == tieDownsDataGridView.Columns["otherPin"].Index) {
                if (sv.Length != 1 ||
                    Array.IndexOf(Helpers.validPins, Char.ToUpper(sv[0])) < 0) {
                    message = "Other Pin must be a valid pin name";
                    e.Cancel = true;
                }
            }

            tieDownsDataGridView.Rows[e.RowIndex].ErrorText = message;
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            //  On cancel, close out the form.
            this.Close();
        }

        private void tieDownsDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {
            //  Nothing to check to delte a tie down.  "Just Do It".
            deletedTieDownList.Add((Tiedown)e.Row.DataBoundItem);
        }

        private void tieDownsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {

            //  If we are building the datagrid view, or 
            //  If this is the title row, ignore it...
            if (populatingDataGridView || e.RowIndex < 0) {
                return;
            }      
            //  Otherwise, mark the edited row as changed.
            ((Tiedown)tieDownsDataGridView.Rows[e.RowIndex].DataBoundItem).modified = true;
        }

        private void applyButton_Click(object sender, EventArgs e) {

            int rowIndex = 0;
            bool errors = false;
            bool changes = false;
            string message;

            DataGridViewRow dgvRow;

            //  First, check any of the rows that have been modified for
            //  valid data.

            //  Note:  The datagrid view already validates the data.
            //  All we need to do is check that the errormesage is blank,
            //  and that required data is present.

            foreach (Tiedown t in tieDownList) {

                dgvRow = tieDownsDataGridView.Rows[rowIndex];

                //  Don't edit existing data...

                if(!((Tiedown)dgvRow.DataBoundItem).modified) {
                    ++rowIndex;
                    continue;
                }

                //  Don't overwrite existing edit errors.

                if (dgvRow.ErrorText.Length > 0) {
                    errors = true;
                    ++rowIndex;
                    continue; 
                }

                //  Check for required fields.

                foreach(string field in 
                    new[]{ "page", "frame", "gate", "panel",
                        "row", "column", "type", "pin"}) {
                    if(dgvRow.Cells[field] == null ||
                        dgvRow.Cells[field].FormattedValue.ToString().Length == 0) {
                        dgvRow.ErrorText = "Field " + 
                            tieDownsDataGridView.Columns[field].HeaderText + " is required.";
                        errors = true;
                        break;  //   On to the next row.
                    }

                    //  Check that the card type is valid (we won't add one).

                    if (Helpers.getCardType(dgvRow.Cells["type"].FormattedValue.ToString()) == 0) {
                        dgvRow.ErrorText = "Card Type has not bee defined.";
                        errors = true;
                        break;
                    }

                    //  Check that either with or without exists.

                    if (dgvRow.Cells["fwith"].FormattedValue.ToString().Length == 0 &&
                       dgvRow.Cells["fwithout"].FormattedValue.ToString().Length == 0) {
                        dgvRow.ErrorText = "Feature With and/or Feature Without required.";
                        errors = true;
                        break;
                    }
                }

                ++rowIndex;
            }

            if(errors) {
                MessageBox.Show("Correct Errors before applying update",
                    "Correct Errors",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //  OK.  We passed edits.   Next, build up a message indicating
            //  what will happen, for confirmation.

            changes = applyCheckOrUpdate(false, out message);

            if(!changes) {
                MessageBox.Show("No Tie Downs Were Changed", "No Tie Down Changes",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult result = MessageBox.Show("Confirm Updates to Tie Downs:\n\n" +
                message, "Confirm Updates",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            if(result == DialogResult.Cancel) {
                return;
            }

            //  And now do the actual updates...

            applyCheckOrUpdate(true, out message);
            MessageBox.Show("Updates to Tie Downs:\n\n" +
                message, "Confirm Updates",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            //  After that, we need to repopulate the data grid view...

            populateTieDowns(currentMachine, currentVolumeSet);

        }


        //  Most of the update heaving lifting occurs in this method, which
        //  can be called with update=false to get a message of what it will
        //  actually do during the update.


        private bool applyCheckOrUpdate(bool doUpdate, out string message) {

            bool changes = false;
            string action;
            int rowIndex;
            DataGridViewRow dgvRow;

            message = "";

            if(doUpdate) {
                db.BeginTransaction();
            }

            foreach (Tiedown t in deletedTieDownList) {

                if (t.idTieDown != 0) {
                    action = doUpdate ? "Deleted" : "Deleting";                    
                    if(doUpdate) {
                        tieDownTable.deleteByKey(t.idTieDown);
                    }
                    message += action + " Tie Down for page " +
                        Helpers.getDiagramPageName(t.diagramPage) +
                        ", card type " + Helpers.getCardTypeType(t.cardType) +
                        ", slot " +
                        Helpers.getCardSlotInfo(t.cardSlot).ToString() +
                        "\n";
                    changes = true;
                }
            }

            rowIndex = 0;
            foreach (Tiedown t in tieDownList) {

                int diagramPageKey = 0;
                int cardTypeKey = 0;
                int cardSlotKey = 0;
                int featureWithKey = 0;
                int featureWithoutKey = 0;

                string outMessage;

                //  Don't say anything for those that did not change.
                if (!t.modified) {
                    ++rowIndex;
                    continue;
                }

                dgvRow = tieDownsDataGridView.Rows[rowIndex];

                CardSlotInfo cardSlotInfo = new CardSlotInfo();
                cardSlotInfo.machineName = currentMachine.name;
                cardSlotInfo.frameName =
                    dgvRow.Cells["frame"].FormattedValue.ToString().ToUpper();
                cardSlotInfo.gateName =
                    dgvRow.Cells["gate"].FormattedValue.ToString().ToUpper();
                cardSlotInfo.panelName =
                    dgvRow.Cells["panel"].FormattedValue.ToString().ToUpper();
                cardSlotInfo.row =
                    dgvRow.Cells["row"].FormattedValue.ToString().ToUpper();
                int columnNumber;
                int.TryParse(dgvRow.Cells["column"].FormattedValue.ToString(),
                    out columnNumber);
                cardSlotInfo.column = columnNumber;

                string pageName =
                    dgvRow.Cells["page"].FormattedValue.ToString();

                string cardTypeName =
                    dgvRow.Cells["type"].FormattedValue.ToString().ToUpper();    

                changes = true;
                if(t.idTieDown == 0) {
                    action = doUpdate ? "Added" : "Adding";
                }
                else {
                    action = doUpdate ? "Updated" : "Updating";
                }

                message += action + " Tie down " +
                    " for page " + pageName + ", card type " + cardTypeName +
                    ", slot " + cardSlotInfo.ToString() + "\n";

                cardTypeKey = 
                    Helpers.getCardType(dgvRow.Cells["type"].FormattedValue.ToString());

                //  Let user know if we will be adding frames, gates, panels,
                //  card slots, pages or features, without acutally doing the updates...

                cardSlotKey = 
                    Helpers.getOrAddCardSlotKey(doUpdate, cardSlotInfo,
                        out outMessage);
                message += outMessage;

                diagramPageKey = 
                    Helpers.getOrAddDiagramPageKey(doUpdate, currentMachine,
                    currentVolumeSet, "TIEDOWN", pageName, out outMessage);
                message += outMessage;

                if (dgvRow.Cells["fwith"].FormattedValue.ToString().Length > 0) {
                    featureWithKey = 
                        Helpers.getOrAddFeatureKey(doUpdate, currentMachine,
                            dgvRow.Cells["fwith"].FormattedValue.ToString(),
                            out outMessage);
                    message += outMessage;
                }
                if (dgvRow.Cells["fwithout"].FormattedValue.ToString().Length > 0) {
                    featureWithoutKey = 
                        Helpers.getOrAddFeatureKey(doUpdate, currentMachine,
                            dgvRow.Cells["fwithout"].FormattedValue.ToString(),
                            out outMessage);
                    message += outMessage;
                }

                //  If we are updating, set the relevant fields and do the update.
            
                if(doUpdate) {

                    //  Note:  Transaction already started earlier.

                    t.diagramPage = diagramPageKey;
                    t.cardType = cardTypeKey;
                    t.cardSlot = cardSlotKey;
                    t.featureWith = featureWithKey;
                    t.featureWithout = featureWithoutKey;
                    //  checkMark, Pin, otherPin and Note are directly updated.

                    if (t.idTieDown == 0) {
                        t.idTieDown = IdCounter.incrementCounter();
                        tieDownTable.insert(t);
                    }
                    else {
                        tieDownTable.update(t);
                    }
                    message += "(Database ID " + t.idTieDown + ")";
                }

                ++rowIndex;
            }

            if (doUpdate) {

                Parms.setParmValue("machine", currentMachine.idMachine.ToString());
                Parms.setParmValue("volume set", currentVolumeSet.idVolumeSet.ToString());
                db.CommitTransaction();
            }

            return (changes);

        }

        //  I found that I needed to force the datagrid view to be updated
        //  via the volumeSetComboBox_SelectedIndexChanged method.  Calling
        //  it from inside the constructor did not work properly.  Hence
        //  this method which is called when the form is first shown.

        private void EditTieDownsForm_Shown(object sender, EventArgs e) {
            currentVolumeSet = volumeSetList[selectedVolumeSet];
            volumeSetComboBox.SelectedIndex = selectedVolumeSet;
        }
    }
}
