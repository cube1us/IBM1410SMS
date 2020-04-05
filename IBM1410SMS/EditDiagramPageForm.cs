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

    //  Really, it ought to be possible to combine this form and the
    //  EditCardLocationPageForm into a super class with two sub-classes,
    //  but I decided it wasn't worth the trouble.

    public partial class EditDiagramPageForm : Form
    {

        DBSetup db = DBSetup.Instance;

        Table<Machine> machineTable;
        Table<Volumeset> volumeSetTable;
        Table<Volume> volumeTable;
        Table<Page> pageTable;
        Table<Cardlocationpage> cardLocationPageTable;
        Table<Diagrampage> diagramPageTable;
        Table<Diagramecotag> diagramEcoTagTable;
        Table<Eco> ecoTable;
        Table<Diagramblock> diagramBlockTable;
        Table<Sheetedgeinformation> sheetEdgeInformationTable;
        Table<Connection> connectionTable;
        Table<Cardlocationblock> cardLocationBlockTable;
        Table<Tiedown> tieDownTable;
        Table<Edgeconnector> edgeConnectorTable;
        Table<Dotfunction> dotFunctionTable;
        Table<Cableedgeconnectionpage> cableEdgeConnectionPageTable;

        List<Machine> machineList;
        List<Volumeset> volumeSetList;
        List<Volume> volumeList;
        List<Page> pageList;

        Machine currentMachine = null;
        Volumeset currentVolumeSet = null;
        Volume currentVolume = null;
        Page currentPage = null;
        Diagrampage currentDiagramPage = null;

        List<Diagramecotag> diagramEcoTagList;
        List<Diagramecotag> deletedDiagramEcoTagList;
        BindingList<Diagramecotag> diagramEcoTagBindingList;

        List<Sheetedgeinformation> sheetEdgeInformationList;
        List<Sheetedgeinformation> deletedSheetEdgeInformationList;
        BindingList<Sheetedgeinformation> sheetEdgeInformationBindingList;

        bool pageModified = false;
        bool populatingDialog = false;

        public EditDiagramPageForm() {
            InitializeComponent();

            populatingDialog = true;

            machineTable = db.getMachineTable();
            volumeSetTable = db.getVolumeSetTable();
            volumeTable = db.getVolumeTable();
            pageTable = db.getPageTable();
            cardLocationPageTable = db.getCardLocationPageTable();
            diagramPageTable = db.getDiagramPageTable();
            diagramEcoTagTable = db.getDiagramEcoTagTable();
            ecoTable = db.getEcoTable();
            diagramBlockTable = db.getDiagramBlockTable();
            sheetEdgeInformationTable = db.getSheetEdgeInformationTable();
            connectionTable = db.getConnectionTable();
            cardLocationBlockTable = db.getCardLocationBlockTable();
            tieDownTable = db.getTieDownTable();
            edgeConnectorTable = db.getEdgeConnectorTable();
            dotFunctionTable = db.getDotFunctionTable();
            cableEdgeConnectionPageTable = db.getCableEdgeConnectionPageTable();

            machineList = machineTable.getAll();

            //  Fill in the machine combo box, and remember which machine
            //  we started out with.

            machineComboBox.DataSource = machineList;
            string lastMachine = Parms.getParmValue("machine");
            if (lastMachine.Length != 0) {
                currentMachine = machineList.Find(x => x.idMachine.ToString() == lastMachine);
            }

            if (currentMachine == null || currentMachine.idMachine == 0) {
                currentMachine = machineList[0];
            }
            else {
                machineComboBox.SelectedItem = currentMachine;
            }

            //  Same for the volume set list - which is not tied to machine

            volumeSetList = volumeSetTable.getAll();
            volumeSetComboBox.DataSource = volumeSetList;

            string lastVolumeSet = Parms.getParmValue("volume set");
            if (lastVolumeSet.Length > 0) {
                currentVolumeSet = volumeSetList.Find(x => x.idVolumeSet.ToString() ==
                    lastVolumeSet);
            }

            if (currentVolumeSet == null || currentVolumeSet.idVolumeSet == 0) {
                currentVolumeSet = volumeSetList[0];
            }
            else {
                // volumeSetComboBox.SelectedItem = currentVolumeSet;
            }

            //  I don't understand why, but I found that if I tried to
            //  point to the "correct" entry, the datagrid view gets 
            //  messed up, with missing data in some columns, and
            //  columns that should be removed were not getting removed.

            volumeSetComboBox.SelectedIndex = -1;

            // Then populate the other combo boxes

            populateVolumeComboBox(currentVolumeSet, currentMachine);

            populatingDialog = false;
        }

        //  Method to fill in the volume combo box

        private void populateVolumeComboBox(Volumeset volumeSet, Machine machine) {

            string lastVolume = Parms.getParmValue("volume");

            currentVolume = null;

            //  If there is no volume set then this combo box must be empty
            //  as well.

            if (volumeSet == null) {
                volumeComboBox.Items.Clear();
            }
            else {
                //  Set up the volume list combo box.
                volumeList = volumeTable.getWhere(
                    "WHERE volume.set='" + volumeSet.idVolumeSet + 
                    "' ORDER BY volume.order");

                volumeComboBox.DataSource = volumeList;

                //  If we have a list, then see if the last volume we used is in
                //  that list.  If so, then select it.

                if (volumeList.Count > 0) {
                    currentVolume = volumeList.Find(x => x.idVolume.ToString() == lastVolume);
                }
                else {
                    volumeComboBox.Items.Clear();
                }
                if(currentVolume == null || currentVolume.idVolume == 0) {
                    currentVolume = volumeList[0];

                }

                volumeComboBox.SelectedItem = currentVolume;
            }

            //  Even if there are no volumes, populate the page combo box.
            //  It will know to create an empty page combo box...

            populatePageComboBox(machine, currentVolume);
        }


        //  Method to fill in the Page combo box.

        private void populatePageComboBox(Machine machine, Volume volume) {

            string lastPage = Parms.getParmValue("page");

            //  If there is no machine or no volume, then this combo box
            //  has to be left empty.

            if (machine == null || volume == null) {
                pageList = new List<Page>();
                currentPage = null;
                currentDiagramPage = null;
                pageComboBox.Items.Clear();
                return;
            }

            //  Get the (potential) list of pages for this machine and volume

            pageList = pageTable.getWhere(
                "WHERE machine='" + machine.idMachine + "' AND volume='" +
                volume.idVolume + "' ORDER BY page.name");

            //  But not all of those are ALD diagram pages.  Some may
            //  be card location pages or cable/edge connector pages.  
            //  Remove those from the list...

            //  (NOTE:  Pages which are NOT diagram pages nor currently
            //  spoken for as card location pages or cable/edge connector
            //  pages remain in the list - they may become diagram pages via this form).

            List<Page> pagesToRemoveList = new List<Page>();
            foreach (Page p in pageList) {
                List<Cardlocationpage> cardLocationPageList =
                    cardLocationPageTable.getWhere(
                    "WHERE cardlocationpage.page='" + p.idPage + "'");
                if (cardLocationPageList.Count > 0) {
                    pagesToRemoveList.Add(p);
                }
                List<Cableedgeconnectionpage> cableEdgeConnectionPageList =
                    cableEdgeConnectionPageTable.getWhere(
                    "WHERE cableedgeconnectionpage.page='" + p.idPage + "'");
                if (cableEdgeConnectionPageList.Count > 0) {
                    pagesToRemoveList.Add(p);
                }
            }
            foreach (Page p in pagesToRemoveList) {
                pageList.Remove(p);
            }

            pageComboBox.DataSource = pageList;

            //  If the list is not empty, set the current page (and the
            //  dialog, later) to the one that the user last used, if it matches,
            //  otherwise set it to the first entry.

            if (pageList.Count > 0) {
                currentPage = pageList.Find(x => x.idPage.ToString() == lastPage);
                if(currentPage == null || currentPage.idPage == 0) {
                    currentPage = pageList[0];
                }

                pageComboBox.SelectedItem = currentPage;
            }
            else {
                //  Otherwise clear the dialog.
                currentPage = null;
                currentDiagramPage = null;
            }

            populateDialog(currentPage);
        }

        private void populateDialog(Page page) {

            int rowIndex;

            //  First clear everything out.

            nameTextBox.Clear();
            partTextBox.Clear();
            titleTextBox.Clear();
            stampTextBox.Clear();
            commentTextBox.Clear();
            pageModified = false;

            //  Clear out the data grid views.

            ecosDataGridView.Columns.Clear();
            ecosDataGridView.DataSource = null;
            sheetEdgeDataGridView.Columns.Clear();
            sheetEdgeDataGridView.DataSource = null;

            //  Forget anything that we may have been remembering to delete.

            deletedDiagramEcoTagList = new List<Diagramecotag>();
            deletedSheetEdgeInformationList = new List<Sheetedgeinformation>();

            //  If the page is null, enter "add" mode, and return.
            //  Otherwise, we are in update/remove mode.

            if (page == null) {
                removeButton.Visible = false;
                editEdgeConnectionsButton.Visible = false;
                editDiagramBlocksButton.Visible = false;
                sheetEdgeInformationList = new List<Sheetedgeinformation>();
                diagramEcoTagList = new List<Diagramecotag>();
                addApplyButton.Text = "Add";
                currentDiagramPage = null;
                return;
            }
            else {
                removeButton.Visible = true;
                addApplyButton.Text = "Apply";
            }

            //  See if there is a matching DiagramPage to this page yet.
            //  If not, then clear any existing diagram page entity object.
            //  (And more than one is a database integrity problem!)

            List<Diagrampage> diagramPageList =
                diagramPageTable.getWhere(
                    "WHERE diagrampage.page='" + page.idPage + "'");

            if (diagramPageList.Count == 0) {
                currentDiagramPage = null;
                return;
            }
            else if(diagramPageList.Count > 1) {
                MessageBox.Show("ERROR: There are more than one Diagram page " +
                    "corresponding to Page " + page.name + " (Database ID " +
                    page.idPage + ") For machine " + currentMachine.name +
                    " in Volume " + currentVolume.name,
                    "Multiple Diagram Pages Found",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                currentDiagramPage = null;
                return;
            }
            else {
                //  Exactly one, so it becomes the current page.
                currentDiagramPage = diagramPageList[0];
            }

            editEdgeConnectionsButton.Visible = true;
            editDiagramBlocksButton.Visible = true;
            populatingDialog = true; 

            //  Now populate the page data.  (Diagram pages do not have
            //  any simple fields of their own; just lists of stuff)

            nameTextBox.Text = page.name;
            partTextBox.Text = page.part;
            titleTextBox.Text = page.title;
            stampTextBox.Text = page.stamp;
            commentTextBox.Text = page.comment;

            //  If there is a current diagram page, then fill in some
            //  more info...

            if(currentDiagramPage != null) {

                noHDLGenerationCheckBox.Checked = (currentDiagramPage.noHDLGeneration == 1);

                diagramEcoTagList = diagramEcoTagTable.getWhere(
                    "WHERE diagramPage='" + currentDiagramPage.idDiagramPage +
                    "' ORDER BY diagramecotag.name");

                sheetEdgeInformationList = new List<Sheetedgeinformation>();

                sheetEdgeInformationList = sheetEdgeInformationTable.getWhere(
                    "WHERE diagramPage='" + currentDiagramPage.idDiagramPage +
                    "' ORDER BY rightSide, leftSide, sheetedgeinformation.row");

                diagramEcoTagBindingList = new BindingList<Diagramecotag>(
                    diagramEcoTagList);
                diagramEcoTagBindingList.AllowEdit = true;
                diagramEcoTagBindingList.AllowNew = true;
                diagramEcoTagBindingList.AllowRemove = true;

                sheetEdgeInformationBindingList =
                    new BindingList<Sheetedgeinformation>(
                        sheetEdgeInformationList);
                sheetEdgeInformationBindingList.AllowEdit = true;
                sheetEdgeInformationBindingList.AllowNew = true;
                sheetEdgeInformationBindingList.AllowRemove = true;

                ecosDataGridView.DataSource = diagramEcoTagBindingList;
                sheetEdgeDataGridView.DataSource = sheetEdgeInformationBindingList;

                //  Hide columns the user does not need to see.

                ecosDataGridView.Columns["idDiagramECOTag"].Visible = false;
                ecosDataGridView.Columns["diagramPage"].Visible = false;
                ecosDataGridView.Columns["modified"].Visible = false;

                sheetEdgeDataGridView.Columns["idSheetEdgeInformation"].Visible = false;
                sheetEdgeDataGridView.Columns["diagramPage"].Visible = false;
                sheetEdgeDataGridView.Columns["modified"].Visible = false;

                //  Set up the simple columns' headers and widths

                ecosDataGridView.Columns["name"].HeaderText = "Tag";
                ecosDataGridView.Columns["name"].Width = 4 * 8;

                sheetEdgeDataGridView.Columns["row"].HeaderText = "Row";
                sheetEdgeDataGridView.Columns["row"].Width = 4 * 8;
                sheetEdgeDataGridView.Columns["signalName"].HeaderText =
                    "Signal Name";
                sheetEdgeDataGridView.Columns["signalName"].Width = 30 * 8;
                sheetEdgeDataGridView.Columns["signalName"].DefaultCellStyle.Font =                  
                    new Font("Courier New", 10);

                //  The rest of the columns are special, as combo boxes
                //  or dates.

                //  The ECO could be a combo box, but it would get quite
                //  long, so instead it is a text box.  The data gets filled
                //  in later...

                DataGridViewTextBoxColumn ecoNameColumn =
                    new DataGridViewTextBoxColumn();
                ecoNameColumn.HeaderText = "E.C. No.";
                ecoNameColumn.HeaderCell.Style.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;
                ecoNameColumn.Width = 10 * 8;
                ecoNameColumn.Name = "eco";
                ecosDataGridView.Columns.Remove("eco");
                ecosDataGridView.Columns.Insert(4, ecoNameColumn);

                //  Date also has to be a text box, unfortunately.

                DataGridViewTextBoxColumn ecoDateColumn =
                    new DataGridViewTextBoxColumn();
                ecoDateColumn.HeaderText = "Date";
                ecoDateColumn.HeaderCell.Style.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;
                ecoDateColumn.Width = 10 * 8;
                ecoDateColumn.Name = "date";
                ecosDataGridView.Columns.Remove("date");
                ecosDataGridView.Columns.Insert(5, ecoDateColumn);

                //  The Sheet Edge Information has special columns too...

                DataGridViewCheckBoxColumn leftSideColumn =
                    new DataGridViewCheckBoxColumn();
                leftSideColumn.DataPropertyName = "leftSide";
                leftSideColumn.HeaderText = "Left (In)";
                leftSideColumn.TrueValue = 1;
                leftSideColumn.FalseValue = 0;
                leftSideColumn.Width = 5 * 8;
                leftSideColumn.HeaderCell.Style.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;
                sheetEdgeDataGridView.Columns.Remove("leftSide");
                sheetEdgeDataGridView.Columns.Insert(0, leftSideColumn);

                DataGridViewCheckBoxColumn rightSideColumn =
                    new DataGridViewCheckBoxColumn();
                rightSideColumn.DataPropertyName = "rightSide";
                rightSideColumn.HeaderText = "Right (Out)";
                rightSideColumn.TrueValue = 1;
                rightSideColumn.FalseValue = 0;
                rightSideColumn.Width = 5 * 8;
                rightSideColumn.HeaderCell.Style.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;
                sheetEdgeDataGridView.Columns.Remove("rightSide");
                sheetEdgeDataGridView.Columns.Insert(1, rightSideColumn);

                DataGridViewTextBoxColumn countColumn =
                    new DataGridViewTextBoxColumn();
                countColumn.HeaderText = "Count";
                countColumn.Name = "count";
                countColumn.Width = 2 * 8;
                countColumn.ReadOnly = true;
                sheetEdgeDataGridView.Columns.Add(countColumn);

                //  Fill in the ECO text box columns, and reset the modified tags.

                rowIndex = 0;
                foreach(Diagramecotag diagramEcoTag in diagramEcoTagList) {
                    Eco eco = ecoTable.getByKey(diagramEcoTag.eco);
                    ((DataGridViewTextBoxCell)
                        (ecosDataGridView.Rows[rowIndex].Cells["eco"])).Value =
                        eco.eco;
                    ((DataGridViewTextBoxCell)
                        (ecosDataGridView.Rows[rowIndex].Cells["date"])).Value =
                        diagramEcoTag.date.ToString("MM/dd/yy");

                    diagramEcoTag.modified = false;
                    ++rowIndex;
                }

                //  Fill in the sheet edge counts, and reset the sheet edge modified tags as well...

                rowIndex = 0;
                foreach(Sheetedgeinformation edgeInfo in sheetEdgeInformationList) {
                    List<Connection> connections = connectionTable.getWhere(
                        "WHERE fromEdgeSheet='" + edgeInfo.idSheetEdgeInformation + "'" +
                        " OR toEdgeSheet='" + edgeInfo.idSheetEdgeInformation + "'");
                    ((DataGridViewTextBoxCell)
                        (sheetEdgeDataGridView.Rows[rowIndex].Cells["count"])).Value =
                        connections.Count.ToString();
                    edgeInfo.modified = false;

                    //  Also, issue a warning if the sheet edge appears as the destination 
                    //  in more than one sheet.

                    List<Sheetedgeinformation> leftSideList = sheetEdgeInformationTable.getWhere(
                        "WHERE signalname='" + edgeInfo.signalName + "'" +
                        " AND rightSide='1'");
                    if(leftSideList.Count > 1) {
                        String warning = "WARNING:  Signal " + edgeInfo.signalName +
                            " appears on the right side of " + leftSideList.Count +
                            " diagrams: ";
                        foreach (Sheetedgeinformation leftSide in leftSideList) {
                            warning += Helpers.getDiagramPageName(leftSide.diagramPage) + " ";
                        }
                        MessageBox.Show(warning, "Signal output from multiple sheets",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }


                    ++rowIndex;                    
                }
            }

            populatingDialog = false;
        }

        
        //  Method to check for changes and to confirm if users wishes to
        //  discard them when certain combo boxes change.

        private DialogResult checkForModifications() {
            if (pageModified == true) {
                DialogResult status = MessageBox.Show("Current Page had changes. " +
                    "Are you sure you wish " +
                    "to discard them?", "Discard Page Changes?",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return status;
            }

            pageModified = false;
            return DialogResult.OK;
        }


        private void machineComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            //  If there are modifications on the current page, confirm
            //  user wants to discard them...

            if (checkForModifications() == DialogResult.Cancel) {
                return;
            }

            currentMachine = machineList[machineComboBox.SelectedIndex];

            //  Repopulate the other affected combo boxes.

            if (!populatingDialog) {
                populateVolumeComboBox(currentVolumeSet, currentMachine);
            }
        }

        private void volumeSetComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            //  If we are in the constructor, ignore.

            if (volumeSetComboBox.SelectedIndex < 0) {
                return;
            }

            //  Check if there are modifications, and if so, if the user wants
            //  to discard them.

            if (checkForModifications() == DialogResult.Cancel) {
                return;
            }

            currentVolumeSet = volumeSetList[volumeSetComboBox.SelectedIndex];

            //  Repopulate the other affected combo boxes.


            if (!populatingDialog) {
                populateVolumeComboBox(currentVolumeSet, currentMachine);
            }

        }

        private void volumeComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            //  Check if there are modifications, and if so, if the user wants
            //  to discard them.

            if (checkForModifications() == DialogResult.Cancel) {
                return;
            }

            currentVolume = volumeList[volumeComboBox.SelectedIndex];
            if (!populatingDialog) {
                populatePageComboBox(currentMachine, currentVolume);
            }
        }

        private void pageComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            if(pageComboBox.SelectedIndex < 0) {
                return;
            }
            
            //  If there is a current page, and if there are modifications, 
            //  confirm that the user wishes to discard them...

            if (currentPage != null &&
                checkForModifications() == DialogResult.Cancel) {
                return;
            }

            currentPage = pageList[pageComboBox.SelectedIndex];

            if (!populatingDialog) {
                populateDialog(currentPage);
            }

        }

        private void ecosDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
            string sv = e.FormattedValue.ToString();
            string message = "";
            DateTime junk;

            //  Skip if we are on a header row or if the columns are not all in place.

            if (populatingDialog ||
                e.RowIndex < 0 || ecosDataGridView.Rows[e.RowIndex].IsNewRow) {
                return;
            }

            if(e.ColumnIndex == ecosDataGridView.Columns["name"].Index) {
                if(string.IsNullOrEmpty(sv) ||
                    sv.Length > 1) {
                    message = "Missing or Invalid Tag (single character)";
                    e.Cancel = true;
                }
            }
            else if(e.ColumnIndex == ecosDataGridView.Columns["eco"].Index) {
                if (string.IsNullOrEmpty(sv) ||
                    sv.Length > 10) {
                    message = "Missing or Invalid E.C. Number";
                    e.Cancel = true;
                }
            }
            else if (e.ColumnIndex == ecosDataGridView.Columns["date"].Index) {
                if (string.IsNullOrEmpty(sv) ||
                    !DateTime.TryParse(sv, out junk)) { 
                    message = "Missing or Invalid E.C. Date";
                    e.Cancel = true;
                }
            }

            ecosDataGridView.Rows[e.RowIndex].ErrorText = message;
        }

        private void ecosDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {

            //  Changing the title row, or one that has just been deleted doesn't count.

            if (e.RowIndex < 0 ||
                e.RowIndex >= ecosDataGridView.Rows.Count) {
                return;
            }

            //  As with bottom notes, changing a block does not necessarily mean
            //  we will be updating the containing card location.

            Diagramecotag changedEco =
                (Diagramecotag)ecosDataGridView.Rows[e.RowIndex].DataBoundItem;
            changedEco.modified = true;
        }


        private void ecosDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {

            //  Need to check the diagram block table for references before
            //  a user can delete an ECO tag...

            Diagramecotag changedDiagramEco = (Diagramecotag)e.Row.DataBoundItem;

            //  If this is a new one, then their cannot be any references.
            if(changedDiagramEco.idDiagramECOTag == 0) {
                return;
            }

            List<Diagramblock> diagramBlockList = diagramBlockTable.getWhere(
                "WHERE eco='" + changedDiagramEco.idDiagramECOTag + "'");
            if(diagramBlockList.Count > 0) {
                MessageBox.Show("ERROR: This entry is referenced " +
                    "by one or more entries in the Diagram Block table, " +
                    "and cannot be removed.",
                    "ECO Tag entry referenced by other entries",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
            else {
                deletedDiagramEcoTagList.Add(changedDiagramEco);
            }
        }

        private void sheetEdgeDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            //  Changing the title row, or one that has just been deleted doesn't count.

            if (e.RowIndex < 0 ||
                e.RowIndex >= sheetEdgeDataGridView.Rows.Count) {
                return;
            }

            //  As with bottom notes, changing a block does not necessarily mean
            //  we will be updating the containing card location.

            Sheetedgeinformation changedSheetEdge =
                (Sheetedgeinformation)sheetEdgeDataGridView.Rows[e.RowIndex].DataBoundItem;
            changedSheetEdge.modified = true;
        }


        private void sheetEdgeDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {

            //  Need to check the connection  table for references before
            //  a user can delete sheet edge information

            Sheetedgeinformation changedSheetEdge = 
                (Sheetedgeinformation)e.Row.DataBoundItem;

            //  If this is a new one, then their cannot be any references.
            if (changedSheetEdge.idSheetEdgeInformation == 0) {
                return;
            }

            //  Search the connection table according to whether this is an
            //  incoming or outgoing signal...

            List<Connection> connectionList = connectionTable.getWhere(
                "WHERE fromEdgeSheet='" +
                    changedSheetEdge.idSheetEdgeInformation + "'" +
                " OR fromEdgeOriginSheet='" +
                    changedSheetEdge.idSheetEdgeInformation + "'" +
                " OR toEdgeSheet='" +
                    changedSheetEdge.idSheetEdgeInformation + "'" +
                " OR toEdgeDestinationSheet='" +
                    changedSheetEdge.idSheetEdgeInformation + "'");

            if (connectionList.Count > 0) {
                MessageBox.Show("ERROR: This Sheet Edge entry is referenced " +
                    "by one or more entries in the connection table, " +
                    "and cannot be removed.",
                    "Sheet Edge Information entry referenced by other entries",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
            else {
                deletedSheetEdgeInformationList.Add(changedSheetEdge);
            }

        }

        private void sheetEdgeDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
            string sv = e.FormattedValue.ToString();
            string message = "";

            //  Skip if we are on a header row or if the columns are not all in place.

            if (populatingDialog ||
                e.RowIndex < 0 || sheetEdgeDataGridView.Rows[e.RowIndex].IsNewRow) {
                return;
            }

            //  Changed to allow any single character for a "row" name.

            if (e.ColumnIndex == sheetEdgeDataGridView.Columns["row"].Index) {
                if (string.IsNullOrEmpty(sv) || sv.Length > 1) { 
                    // Array.IndexOf(Helpers.validDiagramRows,sv.ToUpper()) < 0) { 
                    message = "Missing or Invalid Diagram Row";
                    e.Cancel = true;
                }
            }

            else if (e.ColumnIndex == sheetEdgeDataGridView.Columns["signalName"].Index) {
                if (string.IsNullOrEmpty(sv) ||
                    sv.Length > 45) {
                    message = "Missing or invalid Signal Name (1-45 characters)";
                    e.Cancel = true;
                }
            }

            sheetEdgeDataGridView.Rows[e.RowIndex].ErrorText = message;
        }



        private void cancelButton_Click(object sender, EventArgs e) {
            //  If the user cancels, we obey!
            this.Close();
        }

        private void editEdgeConnectionsButton_Click(object sender, EventArgs e) {

            EditEdgeConnectorsForm editEdgeConnectorsForm = new EditEdgeConnectorsForm(
                currentMachine, currentVolumeSet, currentVolume, currentDiagramPage);

            //  Save the form parameters for later use.

            saveParams(true);

            //  Then invoke the edge connections dialog.

            editEdgeConnectorsForm.ShowDialog();

        }

        private void editDiagramBlocksButton_Click(object sender, EventArgs e) {
            EditDiagramBlocksForm EditDiagramBlocksForm = new EditDiagramBlocksForm(
                currentMachine, currentVolumeSet, currentVolume, currentDiagramPage);

            //  Save the form parameters for later use.

            saveParams(true);

            //  Then invoke the ALD diagram blocks dialog

            EditDiagramBlocksForm.ShowDialog();

            //  Then refresh our own dialog to pick up the changed sheet edge signal counts.

            populateDialog(currentPage);
        }


        private void addApplyButton_Click(object sender, EventArgs e) {

            bool errors = false;
            int rowIndex = 0;
            string message;


            if (nameTextBox.Text.Length == 0 ||
                titleTextBox.Text.Length == 0 ||
                partTextBox.Text.Length == 0 ||
                currentVolume == null) {
                MessageBox.Show("Error:  Volume, Name, Part and Title are required.",
                    "Volume, Name, Part and Title Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (currentPage == null || currentPage.idPage == 0) {

                //  If we have no current real page, we should be in Add mode...

                if(addApplyButton.Text.CompareTo("Add") != 0) {
                    throw new Exception("Diagram Page Add/Apply Button Click: " +
                        "currentPage is null or ID is 0.  Button text expected to be " +
                        "Add, but button text is " +
                        addApplyButton.Text);

                }

                if (currentPage == null) {
                    currentPage = new Page();
                    //  Set name to not-null, but will also not compare to text box.
                    currentPage.name = "";
                }
            }

            else if (addApplyButton.Text.CompareTo("Apply") == 0) {
                if (currentPage == null || currentPage.idPage == 0) {
                    //  Something is rotten in Denmark.
                    throw new Exception("Diagram Page Add/Apply Button Click: " +
                        "currentPage is null or ID is 0 and button text is " +
                        addApplyButton.Text);
                }
            }

            //  Validate that any Sheet Edge Information Entries have
            //  leftSide or rightSide (but not both).

            rowIndex = 0;
            foreach(Sheetedgeinformation edgeInfo in sheetEdgeInformationList) {
                if(edgeInfo.leftSide + edgeInfo.rightSide != 1) {
                    sheetEdgeDataGridView.Rows[rowIndex].ErrorText =
                        "Must select ONE of left side OR right side";
                    errors = true;
                }
                else if(edgeInfo.row == null || edgeInfo.row.Length != 1 || 
                    edgeInfo.signalName == null || edgeInfo.signalName.Length == 0) {
                    sheetEdgeDataGridView.Rows[rowIndex].ErrorText =
                        "Row and Signal Name cannot be empty.";
                    errors = true;
                }
                ++rowIndex;
            }

            if(errors) {
                MessageBox.Show("Error:  There are one or more errors in " +
                    "Sheet Edge Information that must be corrected before " +
                    "proceeding.",
                    "Sheet Edge Information Error(s)",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //  Find out what would change, make sure there wer no errors, and
            //  if none, let the user confirm the update.

            errors = (applyOrCheckUpdate(false, out message) == false);

            if (errors) {
                MessageBox.Show("Error:  There are one or more errors " +
                    " that must be corrected before proceeding.",
                    "Error(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (message.Length <= 0) {
                MessageBox.Show("No updates to the ALD diagram page found" +
                    message, "No Diagram Page Updates",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                return;
            }

            DialogResult status = MessageBox.Show("Confirm the following Adds/Deletes/Updates: \n\n" +
                message, "Confirm Adds/Deletes/Updates",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            if (status == DialogResult.OK) {

                applyOrCheckUpdate(true, out message);

                MessageBox.Show("The following Adds/Deletes/Updates have been applied: \n\n" +
                    message, "Adds/Deletes/Updates Applied",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private bool applyOrCheckUpdate(bool doUpdate, out string message) {

            bool changePageComboBox = false;

            string tempMessage;
            DateTime tempDate;
            string action = "";
            string addAction = doUpdate ? "Added" : "Adding";
            string updateAction = doUpdate ? "Updated" : "Updating";
            string deleteAction = doUpdate ? "Deleted" : "Deleting";
            message = "";
            int rowIndex;

            //  We have to validate new rows (the existing ones are validated
            //  in the _CellValidating() method.

            rowIndex = 0;
            foreach (Diagramecotag diagramECOTag in diagramEcoTagList) {

                //  If this row was not modified, skip it.

                if (!diagramECOTag.modified) {
                    continue;
                }

                //  Otherwise, validate it.

                tempMessage = "";
                DateTime junk;
                String namev =
                    ecosDataGridView.Rows[rowIndex].Cells["name"].FormattedValue.ToString();
                String ecov =
                    ecosDataGridView.Rows[rowIndex].Cells["eco"].FormattedValue.ToString();
                String datev =
                    ecosDataGridView.Rows[rowIndex].Cells["date"].FormattedValue.ToString();
                if (string.IsNullOrEmpty(namev) || namev.Length > 1) {
                    tempMessage = "Missing or Invalid Tag (single character)";
                }
                else if (string.IsNullOrEmpty(ecov) || ecov.Length > 10) {
                    tempMessage = "Missing or Invalid E.C. Number";
                }
                else if (string.IsNullOrEmpty(datev) || !DateTime.TryParse(datev, out junk)) {
                    tempMessage = "Missing or Invalid E.C. Date";
                }
                if (tempMessage.Length > 0) {
                    ecosDataGridView.Rows[rowIndex].ErrorText = tempMessage;
                    message = "Errors in ECO table found.";
                }
                ++rowIndex;
            }

            if (message.Length > 0) {
                MessageBox.Show(message, "Errors Found",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                message = "";
                return (false);
            }

            //  Validation complete, so now we can go to work, "inside out"

            if (currentDiagramPage == null) {
                currentDiagramPage = new Diagrampage();
            }

            //  Fill in the objects from the form data.

            if(currentPage.machine != currentMachine.idMachine ||
                currentPage.volume != currentVolume.idVolume ||
                currentPage.part != partTextBox.Text ||
                currentPage.title != titleTextBox.Text ||
                currentPage.name != nameTextBox.Text ||
                currentPage.stamp != stampTextBox.Text ||
                currentPage.comment != commentTextBox.Text ) {
                currentPage.modified = true;
            }

            if((currentDiagramPage.noHDLGeneration == 1) != noHDLGenerationCheckBox.Checked) {
                currentDiagramPage.modified = true;
            }

            currentPage.machine = currentMachine.idMachine;
            currentPage.volume = currentVolume.idVolume;
            currentPage.part = Importer.zeroPadPartNumber(partTextBox.Text);
            currentPage.title = titleTextBox.Text.ToUpper();
            if (doUpdate && currentPage.name.CompareTo(nameTextBox.Text) != 0) {
                currentPage.name = nameTextBox.Text;
                changePageComboBox = true;
            }
            currentPage.stamp = stampTextBox.Text;
            currentPage.comment = commentTextBox.Text;
            currentDiagramPage.page = currentPage.idPage;
            currentDiagramPage.noHDLGeneration = (noHDLGenerationCheckBox.Checked ? 1 : 0);

            //  Start a transaction...

            if (doUpdate) {
                db.BeginTransaction();
            }

            //  Next, if we are adding a page, do that, so that we have
            //  a key for the Diagram Page.

            if (currentPage.idPage == 0) {
                if (doUpdate) {
                    currentPage.idPage = IdCounter.incrementCounter();
                    pageTable.insert(currentPage);
                    currentPage.modified = false;
                }
                currentPage.name = nameTextBox.Text;
                message = addAction + " Page " + currentPage.name +
                    (doUpdate ? " Database ID=" + currentPage.idPage + "\n" : "") +
                    message + "\n";
                changePageComboBox = true;
            }

            //  Next, if we are adding a Diagram Page, take care of that.

            if (currentDiagramPage.idDiagramPage == 0) {
                if (doUpdate) {
                    currentDiagramPage.idDiagramPage = IdCounter.incrementCounter();
                    currentDiagramPage.page = currentPage.idPage;
                    diagramPageTable.insert(currentDiagramPage);
                    currentDiagramPage.modified = false;
                }
                message += addAction + " Diagram Page " +
                    currentPage.name +
                    (doUpdate ? " Database ID=" + currentDiagramPage.idDiagramPage : "") +
                    "\n";
            }

            //  If there are any modified flags for pages that are not new,
            //  then we need to do updates.  The data has already been filled in.

            if (currentDiagramPage.modified) {
                if (doUpdate) {
                    diagramPageTable.update(currentDiagramPage);
                    currentDiagramPage.modified = false;
                }
                message = updateAction + " Diagram Page " +
                    currentDiagramPage.idDiagramPage + " (Database ID " +
                    currentDiagramPage.idDiagramPage +
                    ")\n" + message;
            }

            if (currentPage.modified) {
                if (doUpdate) {
                    pageTable.update(currentPage);
                    currentPage.modified = false;
                }
                message = updateAction + " Page " + currentPage.name + " (Database ID " +
                    currentPage.idPage + " )\n" + message;
            }

            //  Now we are ready to process any ECO Tag changes.  First,
            //  the deletions.

            foreach (Diagramecotag diagramEcoTag in deletedDiagramEcoTagList) {
                if (doUpdate) {
                    diagramEcoTagTable.deleteByKey(diagramEcoTag.idDiagramECOTag);
                }
                message += deleteAction + " Diagram ECO Tag " + diagramEcoTag.name +
                    " (Database ID " + diagramEcoTag.idDiagramECOTag +
                    ")\n";
            }

            //  Then the adds/updates

            rowIndex = 0;
            foreach (Diagramecotag diagramEcoTag in diagramEcoTagList) {
                if (diagramEcoTag.idDiagramECOTag == 0 ||
                    diagramEcoTag.modified) {
                    diagramEcoTag.name = diagramEcoTag.name.ToUpper();
                    if (doUpdate) {                     //  A new one.  We have already validated the data...
                        diagramEcoTag.diagramPage = currentDiagramPage.idDiagramPage;
                    }
                    //  The name field (tag) should already be filled in.
                    string ecoNumber =
                        ecosDataGridView.Rows[rowIndex].Cells["eco"].FormattedValue.ToString();
                    diagramEcoTag.eco = Helpers.getOrAddEcoKey(doUpdate, currentMachine,
                        ecoNumber, out tempMessage);
                    message += tempMessage;
                    DateTime.TryParse(
                        ecosDataGridView.Rows[rowIndex].Cells["date"].FormattedValue.ToString(),
                        out tempDate);
                    diagramEcoTag.date = tempDate;
                    if (diagramEcoTag.idDiagramECOTag == 0) {
                        action = addAction;
                        if (doUpdate) {
                            diagramEcoTag.idDiagramECOTag = IdCounter.incrementCounter();
                            diagramEcoTagTable.insert(diagramEcoTag);
                        }
                    }
                    else {
                        addAction = "Updated";
                        if (doUpdate) {
                            diagramEcoTagTable.update(diagramEcoTag);
                        }
                    }
                    message += addAction + " Diagram ECO Tag " + diagramEcoTag.name +
                        (doUpdate ? " Database ID=" + diagramEcoTag.idDiagramECOTag : "")
                        + "\n";
                }
                ++rowIndex;
            }

            //  Then a similar process for Sheet Edge Information

            foreach (Sheetedgeinformation edge in deletedSheetEdgeInformationList) {
                sheetEdgeInformationTable.deleteByKey(edge.idSheetEdgeInformation);
                message += deleteAction + " Sheet " + (edge.leftSide > 0 ? "Left" : "Right") +
                    " edge information entry, row " + edge.row +
                    ", Signal " + edge.signalName +
                    " (Database ID " + edge.idSheetEdgeInformation + ")\n";
            }

            rowIndex = 0;
            foreach (Sheetedgeinformation edge in sheetEdgeInformationList) {
                if (edge.idSheetEdgeInformation == 0 || edge.modified) {
                    edge.diagramPage = currentDiagramPage.idDiagramPage;
                    //  Translate all row names and signal names to upper case
                    edge.signalName = edge.signalName.ToUpper();
                    edge.row = edge.row.ToUpper();
                    //  The other fields should already be filled in.
                    if (edge.idSheetEdgeInformation == 0) {
                        action = addAction;
                        if (doUpdate) {
                            edge.idSheetEdgeInformation = IdCounter.incrementCounter();
                            sheetEdgeInformationTable.insert(edge);
                        }
                    }
                    else {
                        action = updateAction;
                        if (doUpdate) {
                            sheetEdgeInformationTable.update(edge);
                        }
                    }
                    message += action + " Sheet " + (edge.leftSide > 0 ? "Left" : "Right") +
                        " edge information entry, row " + edge.row +
                        ", Signal " + edge.signalName +
                        (doUpdate ? " Database ID " + edge.idSheetEdgeInformation + ")" : "") +
                        "\n";
                }
            }

            //  Close out the transaction...

            if (doUpdate) {
                db.CommitTransaction();
            }

            //  Update the page combo box if we added a page, and arrange to have it
            //  selected, as well, and then also save the current machine, volume set,
            //  volume and page.

            if (doUpdate) {
                if (changePageComboBox) {

                    //  Remember the current (possibly new) page, because populatePageComboBox
                    //  completely rebuilds the list, and sets currentPage to the first entry.

                    Page newPage = currentPage;

                    populatingDialog = true;
                    populatePageComboBox(currentMachine, currentVolume);

                    //  Then select the new or updated page.

                    pageComboBox.SelectedItem = pageList.Find(x => x.idPage == newPage.idPage);

                    populatingDialog = false;
                }
                else {
                    populateDialog(currentPage);
                }

                saveParams(true);
            }

            return (true);
        }

        private void newPageButton_Click(object sender, EventArgs e) {

            //  First, check before we throw anything away.

            if (checkForModifications() == DialogResult.Cancel) {
                return;
            }

            //  Most of the real work is done by populate dialog...

            currentPage = null;
            currentDiagramPage = null;
            populateDialog(currentPage);

        }

        private void removeButton_Click(object sender, EventArgs e) {

            string message = "";

            //  First, check before we throw anything away.

            if (checkForModifications() == DialogResult.Cancel) {
                return;
            }

            //  Once in a great while, we find a typo diagrampage/page, 
            //  and need to be able to remove it.

            //  First, make sure we have something to remove.

            if (currentDiagramPage == null || currentDiagramPage.idDiagramPage == 0) {
                if(currentPage == null || currentPage.idPage == 0) {
                    MessageBox.Show("There is no existing page or diagram page to remove.",
                        "No Page/DiagramPage to remove.",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                //  OK, so there is no current diagram page, but we do have a page.
                //  Is there a matching diagram page somewhere?

                List<Diagrampage> diagramPages = diagramPageTable.getWhere(
                    "WHERE diagrampage.page='" + currentPage.idPage + "'");

                if(diagramPages.Count < 1) {
                    MessageBox.Show("There is no existing diagram page corresponding to " +
                        "page " + currentPage.name + " (Database ID " +
                        currentPage.idPage + ").",
                        "No DiagramPage to remove.",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                currentDiagramPage = diagramPages[0];
            }

            //  Make sure that there is one and only one Diagram page associated with this 
            //  page.

            if(currentDiagramPage.page != currentPage.idPage) {
                MessageBox.Show("Page Database ID mismatch! " +
                    "Current DiagramPage page Database ID=" + currentDiagramPage.page +
                    ", Current page Database ID=" + currentPage.idPage,
                    "Page/Diagram Page Database ID mismatch",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<Diagrampage> diagramPageList = diagramPageTable.getWhere(
                "WHERE diagrampage.page='" + currentPage.idPage + "'");

            if(diagramPageList.Count != 1) {
                MessageBox.Show("Multiple Diagram Pages for page " + 
                    currentPage.name + " (Database ID " + currentPage.idPage +
                    ") were found.",
                    "Multiple Diagram Pages",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //  See if there are any references to this diagram page...

            string whereClause = "WHERE diagramPage='" + currentDiagramPage.idDiagramPage + "'";

            List<Cardlocationblock> cardLocationBlockList =
                cardLocationBlockTable.getWhere(whereClause);
            List<Diagramblock> diagramBlockList = diagramBlockTable.getWhere(whereClause);
            List<Tiedown> tieDownList = tieDownTable.getWhere(whereClause);
            List<Diagramecotag> ecoTagList = diagramEcoTagTable.getWhere(whereClause);
            List<Edgeconnector> edgeList = edgeConnectorTable.getWhere(whereClause);
            List<Sheetedgeinformation> sheetEdgeList =
                sheetEdgeInformationTable.getWhere(whereClause);
            List<Dotfunction> dotFunctionList = dotFunctionTable.getWhere(whereClause);

            if(cardLocationBlockList.Count > 0 ||
                diagramBlockList.Count > 0 ||
                tieDownList.Count > 0 ||
                ecoTagList.Count > 0 ||
                edgeList.Count > 0 ||
                sheetEdgeList.Count > 0 || 
                dotFunctionList.Count > 0) {
                message = "The following reference counts were found: " + Environment.NewLine +
                    "Card Location Blocks: " + cardLocationBlockList.Count + Environment.NewLine +
                    "Diagram Blocks:       " + diagramBlockList.Count + Environment.NewLine +
                    "Tie Downs:            " + tieDownList.Count + Environment.NewLine +
                    "ECO Tags:             " + ecoTagList.Count + Environment.NewLine +
                    "Edge Connections:     " + edgeList.Count + Environment.NewLine +
                    "Sheet Edge Signals:   " + sheetEdgeList.Count + Environment.NewLine +
                    "DOT Functions:        " + dotFunctionList.Count + Environment.NewLine +
                    Environment.NewLine +
                    "Diagram Page cannot be removed";
                MessageBox.Show(message, "Diagram Page cannot be removed.",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;            
            }

            message = "Please confirm removal of page " + currentPage.name +
                " (Database ID " + currentPage.idPage +
                "), and corresponding Diagram Page (Database ID " +
                currentDiagramPage.idDiagramPage + ")";

            DialogResult status = MessageBox.Show(message, "Confirm Page Removal",
                MessageBoxButtons.OKCancel,MessageBoxIcon.Question);

            if(status == DialogResult.Cancel) {
                return;
            }

            if(status == DialogResult.OK) {
                db.BeginTransaction();
                diagramPageTable.deleteByKey(currentDiagramPage.idDiagramPage);
                pageTable.deleteByKey(currentPage.idPage);
                db.CommitTransaction();
                MessageBox.Show("Page " + currentPage.name +
                    " and associated Diagram Page have been deleted.");
                currentPage = null;
                currentDiagramPage = null;
                populatePageComboBox(currentMachine, currentVolume);
            }

            //  Save the various settings from the form for reuse later, except
            //  for the page, which no longer exists.

            saveParams(false);

            return;            
        }

        //  Method to save dialog parameters after significant actions, so they
        //  will be the same after returning to this form and available for other
        //  forms.

        void saveParams(bool setPage) {
            Parms.setParmValue("machine", currentMachine.idMachine.ToString());
            Parms.setParmValue("volume set", currentVolumeSet.idVolumeSet.ToString());
            Parms.setParmValue("volume", currentVolume.idVolume.ToString());
            if (setPage) {
                Parms.setParmValue("page", currentPage.idPage.ToString());
            }
        }

        private void EditDiagramPageForm_Shown(object sender, EventArgs e) {
            volumeSetComboBox.SelectedItem = currentVolumeSet;
        }
    }
}