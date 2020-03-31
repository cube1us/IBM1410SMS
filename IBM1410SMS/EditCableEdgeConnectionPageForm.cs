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

    public partial class EditCableEdgeConnectionPageForm : Form
    {

        DBSetup db = DBSetup.Instance;

        Table<Machine> machineTable;
        Table<Volumeset> volumeSetTable;
        Table<Volume> volumeTable;
        Table<Page> pageTable;
        Table<Cardlocationpage> cardLocationPageTable;
        Table<Diagrampage> diagramPageTable;
        Table<Cableedgeconnectionecotag> cableEdgeConnectionEcoTagTable;
        Table<Eco> ecoTable;
        Table<Cableedgeconnectionblock> cableEdgeConnectionBlockTable;
        Table<Connection> connectionTable;
        Table<Cardlocationblock> cardLocationBlockTable;
        Table<Tiedown> tieDownTable;
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
        Cableedgeconnectionpage currentCableEdgeConnectionPage = null;

        List<Cableedgeconnectionecotag> cableEdgeConnectionEcoTagList;
        List<Cableedgeconnectionecotag> deletedCableEdgeconnectionEcoTagList;
        BindingList<Cableedgeconnectionecotag> cableEdgeconnectionEcoTagBindingList;

        bool pageModified = false;
        bool populatingDialog = false;

        public EditCableEdgeConnectionPageForm() {
            InitializeComponent();

            machineTable = db.getMachineTable();
            volumeSetTable = db.getVolumeSetTable();
            volumeTable = db.getVolumeTable();
            pageTable = db.getPageTable();
            cardLocationPageTable = db.getCardLocationPageTable();
            diagramPageTable = db.getDiagramPageTable();
            cableEdgeConnectionEcoTagTable = db.getCableEdgeConnectionECOTagTable();
            ecoTable = db.getEcoTable();
            cableEdgeConnectionBlockTable = db.getCableEdgeConnectionBlockTable();
            connectionTable = db.getConnectionTable();
            cardLocationBlockTable = db.getCardLocationBlockTable();
            tieDownTable = db.getTieDownTable();
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
                volumeSetComboBox.SelectedItem = currentVolumeSet;
            }

            // Then populate the other combo boxes

            populateVolumeComboBox(currentVolumeSet, currentMachine);
        }

        //  Method to fill in the volume combo box

        private void populateVolumeComboBox(Volumeset volumeSet, Machine machine) {

            //  If there is no volume set then this combo box must be empty
            //  as well.

            if (volumeSet == null) {
                currentVolume = null;
                volumeComboBox.Items.Clear();
            }
            else {
                //  Set up the volume list combo box.
                volumeList = volumeTable.getWhere(
                    "WHERE volume.set='" + volumeSet.idVolumeSet + 
                    "' ORDER BY volume.order");
                if (volumeList.Count > 0) {
                    currentVolume = volumeList[0];
                }
                else {
                    currentVolume = null;
                    volumeComboBox.Items.Clear();
                }
                volumeComboBox.DataSource = volumeList;
            }

            //  Even if there are no volumes, populate the page combo box.
            //  It will know to create an empty page combo box...

            populatePageComboBox(machine, currentVolume);
        }


        //  Method to fill in the Page combo box.

        private void populatePageComboBox(Machine machine, Volume volume) {

            //  If there is no machine or no volume, then this combo box
            //  has to be left empty.

            if (machine == null || volume == null) {
                pageList = new List<Page>();
                currentPage = null;
                currentCableEdgeConnectionPage = null;
                pageComboBox.Items.Clear();
                return;
            }

            //  Get the (potential) list of pages for this machine and volume

            pageList = pageTable.getWhere(
                "WHERE machine='" + machine.idMachine + "' AND volume='" +
                volume.idVolume + "' ORDER BY page.name");

            //  But not all of those are Cable/Edge Connection pages.  
            //  Some may be ALD diagram  pages or card location pages
            //  Remove those from the list...

            //  (NOTE:  Pages which are NOT cable/edge pages nor currently
            //  spoken for as diagram pages or card location pages remain in 
            //  the list - they may become Cable/Edge Connection pages via this form).

            List<Page> pagesToRemoveList = new List<Page>();
            foreach (Page p in pageList) {
                List<Cardlocationpage> cardLocationPageList =
                    cardLocationPageTable.getWhere(
                    "WHERE cardlocationpage.page='" + p.idPage + "'");
                if (cardLocationPageList.Count > 0) {
                    pagesToRemoveList.Add(p);
                }
                List<Diagrampage> diagramPageList =
                    diagramPageTable.getWhere(
                    "WHERE diagramPage.page='" + p.idPage + "'");
                if (diagramPageList.Count > 0) {
                    pagesToRemoveList.Add(p);
                }
            }
            foreach (Page p in pagesToRemoveList) {
                pageList.Remove(p);
            }

            //  If the list is not empty, set the current page (and the
            //  dialog, later) to the first entry. 

            if (pageList.Count > 0) {
                currentPage = pageList[0];
            }
            else {
                //  Otherwise clear the dialog.
                currentPage = null;
                currentCableEdgeConnectionPage = null;
            }

            pageComboBox.DataSource = pageList;

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

            //  Forget anything that we may have been remembering to delete.

            deletedCableEdgeconnectionEcoTagList = new List<Cableedgeconnectionecotag>();

            //  If the page is null, enter "add" mode, and return.
            //  Otherwise, we are in update/remove mode.

            if (page == null) {
                removeButton.Visible = false;
                editCableEdgeBlocksButton.Visible = false;
                cableEdgeConnectionEcoTagList = new List<Cableedgeconnectionecotag>();
                addApplyButton.Text = "Add";
                currentCableEdgeConnectionPage = null;
                return;
            }
            else {
                removeButton.Visible = true;
                addApplyButton.Text = "Apply";
            }

            //  See if there is a matching Cable/Edge Location Page to this page yet.
            //  If not, then clear any existing Cable/Edge Connection page entity object.
            //  (And more than one is a database integrity problem!)

            List<Cableedgeconnectionpage> cableEdgeConnectionPageList =
                cableEdgeConnectionPageTable.getWhere(
                    "WHERE cableedgeconnectionpage.page='" + page.idPage + "'");

            if (cableEdgeConnectionPageList.Count == 0) {
                currentCableEdgeConnectionPage = null;
                return;
            }
            else if(cableEdgeConnectionPageList.Count > 1) {
                MessageBox.Show("ERROR: There are more than one Cable/Edge Connection pages " +
                    "corresponding to Page " + page.name + " (Database ID " +
                    page.idPage + ") For machine " + currentMachine.name +
                    " in Volume " + currentVolume.name,
                    "Multiple Cable/Edge Connection Pages Found",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                currentCableEdgeConnectionPage = null;
                return;
            }
            else {
                //  Exactly one, so it becomes the current page.
                currentCableEdgeConnectionPage = cableEdgeConnectionPageList[0];
            }

            editCableEdgeBlocksButton.Visible = true;
            populatingDialog = true; 

            //  Now populate the page data.  (Cable Edge/Connection pages do not have
            //  any simple fields of their own; just lists of stuff)

            nameTextBox.Text = page.name;
            partTextBox.Text = page.part;
            titleTextBox.Text = page.title;
            stampTextBox.Text = page.stamp;
            commentTextBox.Text = page.comment;

            //  If there is a current Cable/Edge Connection page, then fill in some
            //  more info...

            if(currentCableEdgeConnectionPage != null) {

                cableEdgeConnectionEcoTagList = cableEdgeConnectionEcoTagTable.getWhere(
                    "WHERE cableEdgeConnectionPage='" + 
                    currentCableEdgeConnectionPage.idCableEdgeConnectionPage +
                    "' ORDER BY cableedgeconnectionecotag.name");

                cableEdgeconnectionEcoTagBindingList = new BindingList<Cableedgeconnectionecotag>(
                    cableEdgeConnectionEcoTagList);
                cableEdgeconnectionEcoTagBindingList.AllowEdit = true;
                cableEdgeconnectionEcoTagBindingList.AllowNew = true;
                cableEdgeconnectionEcoTagBindingList.AllowRemove = true;

                ecosDataGridView.DataSource = cableEdgeconnectionEcoTagBindingList;

                //  Hide columns the user does not need to see.

                ecosDataGridView.Columns["idCableEdgeConnectionECOTag"].Visible = false;
                ecosDataGridView.Columns["cableEdgeConnectionPage"].Visible = false;
                ecosDataGridView.Columns["modified"].Visible = false;

                //  Set up the simple columns' headers and widths

                ecosDataGridView.Columns["name"].HeaderText = "Tag";
                ecosDataGridView.Columns["name"].Width = 4 * 8;

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

                //  Fill in the ECO text box columns, and reset the modified tags.

                rowIndex = 0;
                foreach(Cableedgeconnectionecotag cableEdgeConnectionEcoTag in 
                        cableEdgeConnectionEcoTagList) {
                    Eco eco = ecoTable.getByKey(cableEdgeConnectionEcoTag.eco);
                    ((DataGridViewTextBoxCell)
                        (ecosDataGridView.Rows[rowIndex].Cells["eco"])).Value =
                        eco.eco;
                    ((DataGridViewTextBoxCell)
                        (ecosDataGridView.Rows[rowIndex].Cells["date"])).Value =
                        cableEdgeConnectionEcoTag.date.ToString("MM/dd/yy");

                    cableEdgeConnectionEcoTag.modified = false;
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

            Cableedgeconnectionecotag changedEco =
                (Cableedgeconnectionecotag)ecosDataGridView.Rows[e.RowIndex].DataBoundItem;
            changedEco.modified = true;
        }


        private void ecosDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {

            //  Need to check the Cable Edge Connection Block table for references before
            //  a user can delete an ECO tag...

            Cableedgeconnectionecotag changedCableEdgeConnectionEco = 
                (Cableedgeconnectionecotag)e.Row.DataBoundItem;

            //  If this is a new one, then their cannot be any references.
            if(changedCableEdgeConnectionEco.idcableEdgeConnectionECOtag == 0) {
                return;
            }

            List<Cableedgeconnectionblock> cableEdgeConnectionBlockList = 
                cableEdgeConnectionBlockTable.getWhere(
                "WHERE ecoTag='" + changedCableEdgeConnectionEco.idcableEdgeConnectionECOtag + "'");
            if(cableEdgeConnectionBlockList.Count > 0) {
                MessageBox.Show("ERROR: This entry is referenced " +
                    "by one or more entries in the Cable Edge Connection Block table, " +
                    "and cannot be removed.",
                    "ECO Tag entry referenced by other entries",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
            else {
                deletedCableEdgeconnectionEcoTagList.Add(changedCableEdgeConnectionEco);
            }
        }


        private void cancelButton_Click(object sender, EventArgs e) {
            //  If the user cancels, we obey!
            this.Close();
        }


        private void editCableEdgeBlocksButton_Click(object sender, EventArgs e) {

            EditCableEdgeConnectionBlocksForm EditCableEdgeConnectionsBlocksForm =
                new EditCableEdgeConnectionBlocksForm(currentMachine,
                currentVolumeSet, currentVolume,
                currentCableEdgeConnectionPage);

            EditCableEdgeConnectionsBlocksForm.ShowDialog();

            populateDialog(currentPage);
        }



        private void addApplyButton_Click(object sender, EventArgs e) {

            bool errors = false;
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
                    throw new Exception("Cable/Edge Connection Page Add/Apply Button Click: " +
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
                    throw new Exception("Cable/Edge Connection Page Add/Apply Button Click: " +
                        "currentPage is null or ID is 0 and button text is " +
                        addApplyButton.Text);
                }
            }

            //  The following may end up being dead code - it was originally for sheet
            //  edge information errors.

            if(errors) {
                MessageBox.Show("Error:  There are one or more errors " +
                    " that must be corrected before proceeding.",
                    "Error(s)",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //  Find out what would change, and let the user confirm.

            applyOrCheckUpdate(false, out message);

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


        private void applyOrCheckUpdate(bool doUpdate, out string message) {

            bool changePageComboBox = false;
            string tempMessage;
            DateTime tempDate;
            string action = "";
            string addAction = doUpdate ? "Added" : "Adding";
            string updateAction = doUpdate ? "Updated" : "Updating";
            string deleteAction = doUpdate ? "Deleted" : "Deleting";
            message = "";
            int rowIndex;

            //  Validation complete, so now we can go to work, "inside out"

            if (currentCableEdgeConnectionPage == null) {
                currentCableEdgeConnectionPage = new Cableedgeconnectionpage();
            }

            //  Fill in the objects from the form data.

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
            currentCableEdgeConnectionPage.page = currentPage.idPage;

            //  Start a transaction...

            if (doUpdate) {
                db.BeginTransaction();
            }

            //  Next, if we are adding a page, do that, so that we have
            //  a key for the Cable/Edge Connection Page.

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
            else {
                //  Rather than check for changes, we will just force the update...
                currentPage.modified = true;
            }

            //  Next, if we are adding a Cable/Edge Connection Page, take care of that.

            if (currentCableEdgeConnectionPage.idCableEdgeConnectionPage == 0) {
                if (doUpdate) {
                    currentCableEdgeConnectionPage.idCableEdgeConnectionPage = 
                        IdCounter.incrementCounter();
                    currentCableEdgeConnectionPage.page = currentPage.idPage;
                    cableEdgeConnectionPageTable.insert(currentCableEdgeConnectionPage);
                    currentCableEdgeConnectionPage.modified = false;
                }
                message += addAction + " Cable/Edge Connection Page " +
                    currentPage.name +
                    (doUpdate ? " Database ID=" + 
                        currentCableEdgeConnectionPage.idCableEdgeConnectionPage : "") +
                        "\n";
            }
            else {
                //  Rather than check for changes, we will just force the update...
                currentCableEdgeConnectionPage.modified = true;
            }

            //  If there are any modified flags, then we need to do updates.
            //  The data has already been filled in.

            if (currentCableEdgeConnectionPage.modified) {
                if (doUpdate) {
                    cableEdgeConnectionPageTable.update(currentCableEdgeConnectionPage);
                    currentCableEdgeConnectionPage.modified = false;
                }
                message = updateAction + " Cable/Edge Connection Page " +
                    currentPage.name + " (Database ID " +
                    currentCableEdgeConnectionPage.idCableEdgeConnectionPage +
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

            foreach (Cableedgeconnectionecotag cableEdgeConnectionEcoTag in 
                    deletedCableEdgeconnectionEcoTagList) {
                if (doUpdate) {
                    cableEdgeConnectionEcoTagTable.deleteByKey(cableEdgeConnectionEcoTag.idcableEdgeConnectionECOtag);
                }
                message += deleteAction + " Cable/Edge Conn ECO Tag " + cableEdgeConnectionEcoTag.name +
                    " (Database ID " + cableEdgeConnectionEcoTag.idcableEdgeConnectionECOtag +
                    ")\n";
            }

            //  Then the adds/updates

            rowIndex = 0;
            foreach (Cableedgeconnectionecotag cableEdgeConnectionEcoTag in cableEdgeConnectionEcoTagList) {
                if (cableEdgeConnectionEcoTag.idcableEdgeConnectionECOtag == 0 ||
                    cableEdgeConnectionEcoTag.modified) {
                    cableEdgeConnectionEcoTag.name = cableEdgeConnectionEcoTag.name.ToUpper();
                    if (doUpdate) {                     //  A new one.  We have already validated the data...
                        cableEdgeConnectionEcoTag.cableEdgeConnectionPage = 
                            currentCableEdgeConnectionPage.idCableEdgeConnectionPage;
                    }
                    //  The name field (tag) should already be filled in.
                    string ecoNumber =
                        ecosDataGridView.Rows[rowIndex].Cells["eco"].FormattedValue.ToString();
                    cableEdgeConnectionEcoTag.eco = Helpers.getOrAddEcoKey(doUpdate, currentMachine,
                        ecoNumber, out tempMessage);
                    message += tempMessage;
                    DateTime.TryParse(
                        ecosDataGridView.Rows[rowIndex].Cells["date"].FormattedValue.ToString(),
                        out tempDate);
                    cableEdgeConnectionEcoTag.date = tempDate;
                    if (cableEdgeConnectionEcoTag.idcableEdgeConnectionECOtag == 0) {
                        action = addAction;
                        if (doUpdate) {
                            cableEdgeConnectionEcoTag.idcableEdgeConnectionECOtag = IdCounter.incrementCounter();
                            cableEdgeConnectionEcoTagTable.insert(cableEdgeConnectionEcoTag);
                        }
                    }
                    else {
                        addAction = "Updated";
                        if (doUpdate) {
                            cableEdgeConnectionEcoTagTable.update(cableEdgeConnectionEcoTag);
                        }
                    }
                    message += addAction + " Cable/Edge Connection ECO Tag " + 
                        cableEdgeConnectionEcoTag.name +
                        (doUpdate ? " Database ID=" + cableEdgeConnectionEcoTag.idcableEdgeConnectionECOtag : "")
                        + "\n";
                }
                ++rowIndex;
            }

            //  Close out the transaction...

            if (doUpdate) {
                db.CommitTransaction();
            }

            //  Update the page combo box if we added a page...

            if (doUpdate) {
                if (changePageComboBox) {

                    //  We have to save the new page, because populatePageComboBox builds
                    //  a nwe list, and sets the selected item. so save it.

                    Page newPage = currentPage;
                    populatingDialog = true;
                    populatePageComboBox(currentMachine, currentVolume);

                    //  Then select the new page using its key.  
                    pageComboBox.SelectedItem = pageList.Find(
                        x => x.idPage == newPage.idPage);

                    populatingDialog = false;
                }
                else {
                    populateDialog(currentPage);
                }
            }
        }

        private void newPageButton_Click(object sender, EventArgs e) {

            //  First, check before we throw anything away.

            if (checkForModifications() == DialogResult.Cancel) {
                return;
            }

            //  Most of the real work is done by populate dialog...

            currentPage = null;
            currentCableEdgeConnectionPage = null;
            populateDialog(currentPage);

        }

        private void removeButton_Click(object sender, EventArgs e) {

            string message = "";

            //  First, check before we throw anything away.

            if (checkForModifications() == DialogResult.Cancel) {
                return;
            }

            //  Once in a great while, we find a typo Cable/Edge Connection page/page, 
            //  and need to be able to remove it.

            //  First, make sure we have something to remove.

            if (currentCableEdgeConnectionPage == null || 
                currentCableEdgeConnectionPage.idCableEdgeConnectionPage == 0) {
                if(currentPage == null || currentPage.idPage == 0) {
                    MessageBox.Show("There is no existing page or cable/edge connection page to remove.",
                        "No Page or Cable/Edge Connectin Page to remove.",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                //  OK, so there is no current Cable/Edge Connection page, but we do have a page.
                //  Is there a matching Cable/Edge Connection page somewhere?

                List<Cableedgeconnectionpage> cableEdgeConnectionPages = 
                    cableEdgeConnectionPageTable.getWhere(
                    "WHERE cableEdgeConnectionPage.page='" + currentPage.idPage + "'");

                if(cableEdgeConnectionPages.Count < 1) {
                    MessageBox.Show("There is no existing Cable/Edge connection page corresponding to " +
                        "page " + currentPage.name + " (Database ID " +
                        currentPage.idPage + ").",
                        "No Cable/Edge Connection Page to remove.",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                currentCableEdgeConnectionPage = cableEdgeConnectionPages[0];
            }

            //  Make sure that there is one and only one Cable/Edge Connection page associated with this 
            //  page.

            if(currentCableEdgeConnectionPage.page != currentPage.idPage) {
                MessageBox.Show("Page Database ID mismatch! " +
                    "Current Cable/Edge Connection Page page Database ID=" + 
                    currentCableEdgeConnectionPage.page +
                    ", Current page Database ID=" + currentPage.idPage,
                    "Page/Cable Edge Connection Page Database ID mismatch",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<Cableedgeconnectionpage> cableEdgeConenctionPageList = 
                cableEdgeConnectionPageTable.getWhere(
                "WHERE cableedgeconnectionpage.page='" + currentPage.idPage + "'");

            if(cableEdgeConenctionPageList.Count != 1) {
                MessageBox.Show("Multiple Cable/Edge Connection Pages for page " + 
                    currentPage.name + " (Database ID " + currentPage.idPage +
                    ") were found.",
                    "Multiple Cable/Edge Connection Pages",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //  See if there are any references to this Cable/Edge connection page...

            string whereClause = "WHERE cableEdgeConnectionPage='" + 
                currentCableEdgeConnectionPage.idCableEdgeConnectionPage + "'";

            List<Cableedgeconnectionblock> cableEdgeConnectionBlockList = 
                cableEdgeConnectionBlockTable.getWhere(whereClause);
            List<Cableedgeconnectionecotag> ecoTagList = 
                cableEdgeConnectionEcoTagTable.getWhere(whereClause);

            if(cableEdgeConnectionBlockList.Count > 0 ||
                ecoTagList.Count > 0 ) {
                message = "The following reference counts were found: " + Environment.NewLine +
                    "Cable/Edge Connection Blocks: " 
                        + cableEdgeConnectionBlockList.Count + Environment.NewLine +
                    "ECO Tags:                     " + ecoTagList.Count + Environment.NewLine +
                    Environment.NewLine +
                    "Cable/Edge Connection Page cannot be removed";
                MessageBox.Show(message, "Cable/Edge Connection Page cannot be removed.",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;            
            }

            message = "Please confirm removal of page " + currentPage.name +
                " (Database ID " + currentPage.idPage +
                "), and corresponding Cable/Edge Connection Page (Database ID " +
                currentCableEdgeConnectionPage.idCableEdgeConnectionPage + ")";

            DialogResult status = MessageBox.Show(message, "Confirm Page Removal",
                MessageBoxButtons.OKCancel,MessageBoxIcon.Question);

            if(status == DialogResult.Cancel) {
                return;
            }

            if(status == DialogResult.OK) {
                db.BeginTransaction();
                cableEdgeConnectionPageTable.deleteByKey(
                    currentCableEdgeConnectionPage.idCableEdgeConnectionPage);
                pageTable.deleteByKey(currentPage.idPage);
                db.CommitTransaction();
                MessageBox.Show("Page " + currentPage.name +
                    " and associated Cable/Edge Connection Page have been deleted.");
                currentPage = null;
                currentCableEdgeConnectionPage = null;
                populatePageComboBox(currentMachine, currentVolume);
            }

            return;            
        }
    }
}