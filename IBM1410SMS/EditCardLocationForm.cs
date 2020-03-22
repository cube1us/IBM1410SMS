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
using System.Diagnostics;

using MySQLFramework;


namespace IBM1410SMS
{

    public partial class EditCardLocationForm : Form
    {

        static string[] validDiagramRows = {
            "A", "B", "C", "D", "E", "F", "G", "H", "I",
        };

        static int[] validDiagramColumns = { 1, 2, 3, 4, 5 };

        // Regex bottomNotesPattern = new Regex(@"^((\d+) {0,1}, {0,1}(\d+))*((\d+){0,1}$");
        Regex bottomNotesPattern = new Regex(@"^(\d{1,8}( *, *\d{1,8})*)?$");

        DBSetup db = DBSetup.Instance;

        Table<Machine> machineTable;
        Table<Frame> frameTable;
        Table<Machinegate> machineGateTable;
        Table<Volumeset> volumeSetTable;
        Table<Volume> volumeTable;
        Table<Eco> ecoTable;
        Table<Page> pageTable;
        Table<Cardlocation> cardLocationTable;
        Table<Cardlocationpage> cardLocationPageTable;
        Table<Cardlocationbottomnote> cardLocationBottomNoteTable;
        Table<Cardlocationblock> cardLocationBlockTable;
        Table<Cardslot> cardSlotTable;
        Table<Cardtype> cardTypeTable;
        Table<Feature> featureTable;
        Table<Panel> panelTable;
        Table<Diagrampage> diagramPageTable;
        Table<Cableedgeconnectionpage> cableEdgeConnectionPageTable;

        List<Machine> machineList;
        List<Volumeset> volumeSetList;
        List<Volume> volumeList;
        List<Eco> ecoList;
        List<Page> pageList;
        List<Cardtype> cardTypeList;
        List<Feature> featureList;
        List<Panel> panelList;
        List<string> rowList;
        List<int> columnList;

        Machine currentMachine = null;
        Volume currentVolume = null;
        Panel currentPanel = null;
        Page currentPage = null;
        Eco currentECO = null;

        Cardlocation currentCardLocation = null;
        Cardlocationpage currentCardLocationPage = null;

        List<Cardlocationblock> deletedCardLocationBlockList;
        List<Cardlocationblock> cardLocationBlockList;

        BindingList<Cardlocationblock> cardLocationBlockBindingList;

        bool bottomNotesModified = false;
        bool populatingDataGridView = false;

        public EditCardLocationForm() {
            InitializeComponent();

            machineTable = db.getMachineTable();
            volumeSetTable = db.getVolumeSetTable();
            volumeTable = db.getVolumeTable();
            ecoTable = db.getEcoTable();
            pageTable = db.getPageTable();
            cardLocationTable = db.getCardLocationTable();
            cardLocationPageTable = db.getCardLocationPageTable();
            cardLocationBottomNoteTable = db.getCardLocationBottomNoteTable();
            cardLocationBlockTable = db.getCardLocationBlockTable();
            cardTypeTable = db.getCardTypeTable();
            cardSlotTable = db.getCardSlotTable();
            featureTable = db.getFeatureTable();
            panelTable = db.getPanelTable();
            frameTable = db.getFrameTable();
            machineGateTable = db.getMachineGateTable();
            diagramPageTable = db.getDiagramPageTable();
            cableEdgeConnectionPageTable = db.getCableEdgeConnectionPageTable();

            //  The card row and columns are dependent upon the selected panel.
            
            rowComboBox.DataSource = null;
            columnComboBox.DataSource = null;

            //  The machine combo box is static, so we can fill that one out now.

            machineList = machineTable.getAll();

            //  Fill in the machine combo box, and remember which machine
            //  we started out with.

            machineComboBox.DataSource = machineList;
            currentMachine = machineList[0];
            populateFeatureComboBox(currentMachine);
            populatePanelComboBox(currentMachine);

            //  The card type table is also fixed.  It has a field, but that
            //  volume is for the volume that contains the card's own drawing.

            cardTypeList = cardTypeTable.getWhere("ORDER BY cardtype.type");
            cardTypeComboBox.DataSource = cardTypeList;

            //  Same for the volume list - which is not tied to machine

            volumeSetList = volumeSetTable.getWhere("ORDER BY machineType");

            volumeList = new List<Volume>();
            foreach (Volumeset vs in volumeSetList) {
                List<Volume> tempVolumeList = volumeTable.getWhere(
                    "WHERE volume.set='" + vs.idVolumeSet + "' ORDER BY volume.order");
                foreach (Volume v in tempVolumeList) {
                    volumeList.Add(v);
                    volumeComboBox.Items.Add("Vol. Set " + vs.machineType +
                        ", Vol. " + v.name);
                }
            }

            if (volumeList.Count > 0) {
                currentVolume = volumeList[0];
                volumeComboBox.SelectedIndex = 0;
            }
            else {

                //  If there is no volume list, the use of this dialog is premature...

                currentVolume = null;
            }

            //  Now, start the cascade of filling in the rest.  Those are
            //  separate methods, because they also get called when
            //  selections change.

            populatePageComboBox(currentMachine, currentVolume);
        }

        private void populatePageComboBox(Machine machine, Volume volume) {

            pageComboBox.DataSource = null;

            //  If there is no machine or no volume, then this combo box
            //  has to be left empty.

            if (machine == null || volume == null) {
                pageList = new List<Page>();
                currentPage = null;
                currentCardLocationPage = null;
                return;
            }

            //  Get the (potential) list of pages for this machine and volume
            //  For the card location page chart page.

            pageList = pageTable.getWhere(
                "WHERE machine='" + machine.idMachine + "' AND volume='" +
                volume.idVolume + "' ORDER BY page.name");

            //  TODO:  It would probably be best to remove diagramPages and
            //  cableEdgeConnectionPages from this list.

            pageComboBox.DataSource = pageList;

            //  Oddly, I found that I needed to do the following, even though
            //  it is set in the combo box properties...

            pageComboBox.DisplayMember = "name";

            //  If the list is not empty, set the current page to the first entry.

            if (pageList.Count > 0) {
                currentPage = pageList[0];
            }
            else {
                currentPage = null;
            }

            //  At this point is it handy to find the matching 

            populatePageECOComboBox(currentPage);
            populateDialog();
        }


        //  Populate the Feature list, which also changes if the machine is changed.

        private void populateFeatureComboBox(Machine machine) {
            featureList = featureTable.getWhere("" +
                "WHERE machine='" + machine.idMachine + "' ORDER BY feature.code");

            //  An empty feature is allowed, so add one to the list, at the FRONT

            Feature nullFeature = new Feature();
            nullFeature.code = "";
            nullFeature.idFeature = 0;
            featureList.Insert(0, nullFeature);

            featureComboBox.DataSource = null;
            featureComboBox.DataSource = featureList;
            featureComboBox.DisplayMember = "code";
        }


        //  The Panel List also changes if the machine is changed. 
        //  The panel Combo Box uses a constructed string - access the key using
        //  its index.
        
        private void populatePanelComboBox(Machine machine) {

            //  Clear out the existing combo box entries and the panel list.

            panelComboBox.Items.Clear();
            panelComboBox.ResetText();
            panelList = new List<Panel>();
             
            List<Frame> frameList = frameTable.getWhere(
                "WHERE machine='" + machine.idMachine + "' ORDER BY frame.name");
            foreach(Frame f in frameList) {
                List<Machinegate> machineGateList = machineGateTable.getWhere(
                    "WHERE frame='" + f.idFrame + "' ORDER BY machinegate.name");
                foreach(Machinegate g in machineGateList) {
                    List<Panel> tempPanelList = panelTable.getWhere(
                        "WHERE gate='" + g.idGate + "' ORDER BY panel");
                    foreach(Panel p in tempPanelList) {
                        panelList.Add(p);
                        panelComboBox.Items.Add("Frame: " + f.name +
                            ", Gate: " + g.name + ", Panel: " + p.panel);
                    } 
                }
            }

            if (panelList.Count > 0) {
                panelComboBox.SelectedIndex = 0;
                currentPanel = panelList[0];
            }
            else {
                currentPanel = null;
            }

            populateRowColumnComboBoxes(currentPanel);
        }


        private void populateRowColumnComboBoxes(Panel panel) {

            rowList = new List<string>();
            columnList = new List<int>();              
            List<Cardslot> cardSlotList;

            rowComboBox.DataSource = null;
            columnComboBox.DataSource = null;

            if (panel == null) {
                return;
            }

            //  Because my framework does not allow arbitrary SQL statements,
            //  We use "GROUP BY" as a stand-in for "DISTINCT"

            //  First for the rows...

            cardSlotList = cardSlotTable.getWhere(
                "WHERE panel='" + panel.idPanel + "' GROUP BY cardRow");
            foreach(Cardslot cs in cardSlotList) {
                rowList.Add(cs.cardRow);
            }

            //  Then the columns.

            cardSlotList = cardSlotTable.getWhere(
                "WHERE panel='" + panel.idPanel + "' GROUP BY cardColumn");
            foreach (Cardslot cs in cardSlotList) {
                columnList.Add(cs.cardColumn);
            }

            rowComboBox.DataSource = rowList;
            columnComboBox.DataSource = columnList;
        }

        private void populatePageECOComboBox(Page page) {

            //  These are not all of the ECOs.  Just the ECOs that exist for
            //  the selected page.

            ecoList = new List<Eco>();
            ecoComboBox.DataSource = null;

            if(page == null) {
                return;
            }

            List<Cardlocationpage> cardLocationPageList =
                cardLocationPageTable.getWhere("" +
                    "WHERE cardlocationpage.page='" + page.idPage + "'");
            //  Now make a list of all of the ECOs that appear in this list...
            foreach (Cardlocationpage clp in cardLocationPageList) {
                Eco eco = ecoTable.getByKey(clp.eco);
                ecoList.Add(eco);
            }

            ecoComboBox.DataSource = ecoList;
            ecoComboBox.DisplayMember = "eco";
            ecoComboBox.ValueMember = "idECO";

            if (ecoList.Count > 0) {
                ecoComboBox.SelectedItem = ecoList[0];
                currentECO = ecoList[0];
            }
            else {
                currentECO = null;
            }

        }

        private void machineComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            currentMachine = (Machine) machineComboBox.SelectedItem;
            populatePageComboBox(currentMachine, currentVolume);
            populateFeatureComboBox(currentMachine);
            populatePanelComboBox(currentMachine);
            populateDialog();
        }

        private void volumeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (volumeComboBox.SelectedIndex >= 0) {
                currentVolume = volumeList[volumeComboBox.SelectedIndex];
            }
            else {
                currentVolume = null;
            }
            populatePageComboBox(currentMachine, currentVolume);
            populateDialog();
        }

        private void pageComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            currentPage = (Page) pageComboBox.SelectedItem;
            populatePageECOComboBox(currentPage);
            populateDialog();
        }

        private void ecoComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            currentECO = (Eco) ecoComboBox.SelectedItem;
            populateDialog();
        }

        private void panelComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            if (panelComboBox.SelectedIndex >= 0) {
                currentPanel = panelList[panelComboBox.SelectedIndex];
            }
            else {
                currentPanel = null;
            }
            populateRowColumnComboBoxes(currentPanel);
            populateDialog();
        }

        private void columnComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            populateDialog();
        }


        private void rowComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            populateDialog();
        }



        private void bottomNotesTextBox_Validating(object sender, CancelEventArgs e) {

            if (!bottomNotesPattern.Match(bottomNotesTextBox.Text).Success) {
                errorProvider1.SetError(bottomNotesTextBox, 
                    "Invalid Bottom Notes specification");
                e.Cancel = true;
            }
            else {
                errorProvider1.SetError(bottomNotesTextBox, "");
            }
        }

        private void cardLocationBlockDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {

            string sv = e.FormattedValue.ToString();
            string message = "";
            
            //  Skip if we are on a header row or if the columns are not all in place.

            if(populatingDataGridView ||
                e.RowIndex < 0 || cardLocationBlockDataGridView.Rows[e.RowIndex].IsNewRow) { 
                return;
            }

            if(e.ColumnIndex == cardLocationBlockDataGridView.Columns["diagramRow"].Index) {
                if(string.IsNullOrEmpty(e.FormattedValue.ToString()) ||
                    Array.IndexOf(validDiagramRows, e.FormattedValue.ToString()) < 0) {
                    message = "Invalid Row Value (A-I)";
                    e.Cancel = true;
                }
            }
            else if(e.ColumnIndex == cardLocationBlockDataGridView.Columns["diagramColumn"].Index) {
                int v;
                if(!int.TryParse(sv,out v)) {
                    message = "Invalid Column: Column must be an integer, 1-5";
                    e.Cancel = true;
                }
                else if (Array.IndexOf(validDiagramColumns,v) < 0) {
                    message = "Invalid Column Value: Must be 1-5";
                    e.Cancel = true;
                }
            }
            else if(e.ColumnIndex == cardLocationBlockDataGridView.Columns["page"].Index) {
                if(sv.Length == 0) {
                    message = "Page number is required.";
                    e.Cancel = true;
                }
            }

            //  Set (or clear, if no errors) the message.

            cardLocationBlockDataGridView.Rows[e.RowIndex].ErrorText = message;
        }


        //  Because this DataGridVew has editing controls, unless we take
        //  some special measures, the delete key does not work as intended.
        //  This event handler takes care of that by registering a special
        //  event handler that checks specifically for the delete key.

        private void cardLocationBlockDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e) {
            //  First, remove any existing handler, if present, to avoid
            //  having multiple handlers.

            e.Control.KeyDown -=
                new KeyEventHandler(cardLocationBlockDataGridView_ControlKeyDown);

            //  And then add it back.

            e.Control.KeyDown +=
                new KeyEventHandler(cardLocationBlockDataGridView_ControlKeyDown);
        }

        //  And this is the control key handler...

        void cardLocationBlockDataGridView_ControlKeyDown(object sender,
            KeyEventArgs e) {

            //  Only the delete key gets special handling, and only if the row is
            //  a new row

            if (e.KeyCode == Keys.Delete &&
                cardLocationBlockDataGridView.SelectedRows.Count > 0 &&
                (!cardLocationBlockDataGridView.SelectedRows[0].IsNewRow ||
                cardLocationBlockList[
                    cardLocationBlockDataGridView.SelectedRows[0].Index].idCardLocationBlock != 0)) {

                //  Delete only ONE row.

                deletedCardLocationBlockList.Add(
                    cardLocationBlockList[
                        cardLocationBlockDataGridView.SelectedRows[0].Index]);

                cardLocationBlockDataGridView.Rows.Remove(
                    cardLocationBlockDataGridView.SelectedRows[0]);

                //  Nothing to verify for a delete here...

                e.Handled = true;
            }
        }


        //  Shared method to set the modified flag.

        private void setModified(bool t) {
            if (currentCardLocation != null) {
                currentCardLocation.modified = true;
            }
        }

        private void cardTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            setModified(true);
        }

        private void featureComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            setModified(true);
        }

        private void bottomNotesTextBox_TextChanged(object sender, EventArgs e) {

            //  Here we do NOT set the card location modified flag, because
            //  a bottom note change may not require an update
            //  to the containing Card Location.

            bottomNotesModified = true;
        }

        private void crossedOutCheckBox_CheckedChanged(object sender, EventArgs e) {
            setModified(true);
        }

        private void cardLocationBlockDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {

            //  Changing the title row, or one that has just been deleted doesn't count.

            if (e.RowIndex < 0 || 
                e.RowIndex >= cardLocationBlockDataGridView.Rows.Count) {
                return;
            }

            //  As with bottom notes, changing a block does not necessarily mean
            //  we will be updating the containing card location.

            Cardlocationblock changedBlock =
                (Cardlocationblock)cardLocationBlockDataGridView.Rows[e.RowIndex].DataBoundItem;
            changedBlock.modified = true;

        }


        private void populateDialog() {

            applyButton.Text = "ERROR";
            applyButton.Visible = false;
            deleteButton.Visible = false;
            populatingDataGridView = true;

            currentCardLocation = null;
            currentCardLocationPage = null;

            //  Clear out the existing fields and the datagridview

            bottomNotesTextBox.Text = "";
            bottomNotesModified = false;
            featureComboBox.SelectedIndex = -1;
            crossedOutCheckBox.Checked = false;
            cardLocationBlockDataGridView.Columns.Clear();
            cardLocationBlockDataGridView.DataSource = null;

            //  Forget anything we may have been saving to delete...

            deletedCardLocationBlockList = new List<Cardlocationblock>();

            //  We need to have a current page, panel and ECO level to proceed.

            if (currentPage == null || currentPanel == null || currentECO == null) {
                return;
            }

            //  OK, so the selection fields are valid.  See
            //  if there is a match...

            List<Cardlocationpage> cardLocationPageList =
                cardLocationPageTable.getWhere(
                    "WHERE cardlocationpage.page='" + currentPage.idPage + "'" +
                    " AND panel='" + currentPanel.idPanel + "'" +
                    " AND eco='" + currentECO.idECO + "'");
                
            //  There really should only be one...

            //  If there are none, we can't do anything.

            if(cardLocationPageList.Count == 0) {
                return;
            }

            //  If there are more than one, we would not know which to use...

            if(cardLocationPageList.Count > 1) {
                MessageBox.Show("ERROR:  Multiple Card Location Pages found " +
                    "with page=" + currentPage.name + ", Panel=" +
                    currentPanel.panel + " and ECO=" + currentECO.eco,
                    "Unexpected Multiple Card Location Pages Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            //  Yay!!  There is exactly one.  

            currentCardLocationPage = cardLocationPageList[0];

            //  Now, see if we have a valid card slot and type.

            if(rowComboBox.SelectedIndex < 0 || columnComboBox.SelectedIndex < 0) {
                return;
            }

            List<Cardslot> cardSlotList = cardSlotTable.getWhere(
                "WHERE panel='" + currentPanel.idPanel + "'" +
                " AND cardRow='" + rowList[rowComboBox.SelectedIndex] + "'" +
                " AND cardColumn='" + columnList[columnComboBox.SelectedIndex] + "'");

            //  If there are none, just return.

            if(cardSlotList.Count == 0) {
                return;
            }

            //  More than one is an error.

            if(cardSlotList.Count > 1) {
                MessageBox.Show("ERROR: More than one Card Slot with " +
                    "Panel=" + currentPanel.idPanel +
                    ", Row=" + rowList[rowComboBox.SelectedIndex] +
                    " and Column=" + columnList[columnComboBox.SelectedIndex],
                    "Multiple Card Slots Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            //  OK, now we are ready to search for a card location...

            List<Cardlocation> cardLocationList = cardLocationTable.getWhere(
                "WHERE cardlocation.page='" + 
                currentCardLocationPage.idCardLocationPage + "'" +
                " AND cardSlot='" + cardSlotList[0].idCardSlot + "'");

            //  Again, there can be only one...

            if(cardLocationList.Count == 0) {
                applyButton.Text = "Add";
                applyButton.Visible = true;
                return;
            }

            if(cardLocationList.Count > 1) {
                MessageBox.Show("ERROR: Multiple Card Locations found for " +
                    "page=" + currentPage.name +
                    ", ECO=" + currentECO.eco +
                    ", panel=" + currentPanel.panel +
                    ", Row=" + rowList[rowComboBox.SelectedIndex] +
                    "and Column=" + columnList[columnComboBox.SelectedIndex],
                    "Multiple Card Locations Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            //  GOT ONE!  Fill in the simple fields first.

            applyButton.Visible = true;
            applyButton.Text = "Update";
            currentCardLocation = cardLocationList[0];

            Feature feature = featureTable.getByKey(currentCardLocation.feature);
            featureComboBox.SelectedItem = featureList.Find(
                f => f.idFeature == currentCardLocation.feature);
            crossedOutCheckBox.Checked = (currentCardLocation.crossedOut > 0);

            if (currentCardLocation.type != 0) {
                cardTypeComboBox.SelectedItem = cardTypeList.Find(                    
                    x => x.idCardType == currentCardLocation.type);
            }

            //  Next, fetech the bottom notes, if any.  For compactness, these
            //  are not done using a datagridview.  We map them manually in and
            //  back out during add/update

            bottomNotesTextBox.Text = "";

            List<Cardlocationbottomnote> bottomNoteList =
                cardLocationBottomNoteTable.getWhere(
                    "WHERE cardLocation='" + currentCardLocation.idCardLocation + 
                    "' ORDER BY note");

            foreach(Cardlocationbottomnote bn in bottomNoteList) {
                if(bottomNotesTextBox.Text.Length > 0) {
                    bottomNotesTextBox.Text += ", ";
                }
                bottomNotesTextBox.Text += bn.note;
            }

            //  Next, fill in the data grid view...

            applyButton.Text = "Apply";
            deleteButton.Visible = true;

            cardLocationBlockList = cardLocationBlockTable.getWhere(
                "WHERE cardLocation='" + currentCardLocation.idCardLocation + "'" +
                " ORDER BY diagramColumn, diagramRow");

            cardLocationBlockBindingList = new BindingList<Cardlocationblock>(
                cardLocationBlockList);
            cardLocationBlockBindingList.AllowNew = true;
            cardLocationBlockBindingList.AllowEdit = true;
            cardLocationBlockBindingList.AllowRemove = true;

            cardLocationBlockDataGridView.DataSource = cardLocationBlockBindingList;

            //  Hide columns users does not need to see

            cardLocationBlockDataGridView.Columns["modified"].Visible = false;
            cardLocationBlockDataGridView.Columns["idCardLocationBlock"].Visible = false;
            cardLocationBlockDataGridView.Columns["cardLocation"].Visible = false;
            cardLocationBlockDataGridView.Columns.Remove("cableEdgeConnectionPage");

            //  Now set up each simple column with header text and width - and, for
            //  combo boxes and check boxes, even more.

            cardLocationBlockDataGridView.Columns["note"].HeaderText = "Note";
            cardLocationBlockDataGridView.Columns["note"].Width = 17 * 8;

            //  The rest of the columns are special, as combo boxes or
            //  check boxes.

            //  The diagram page could be a combobox - but it would be very,
            //  very long.  So instead it is a text box.  The data gets filled
            //  in a bit later...

            DataGridViewTextBoxColumn diagramPageColumn =
                new DataGridViewTextBoxColumn();
            diagramPageColumn.HeaderText = "Page";
            diagramPageColumn.Width = 14 * 8;
            diagramPageColumn.Name = "page";
            cardLocationBlockDataGridView.Columns.Remove("diagramPage");
            cardLocationBlockDataGridView.Columns.Insert(3, diagramPageColumn);
            

            //  Diagram row is a combo box made from the Valid Diagram Rows.

            DataGridViewTextBoxColumn diagramRowColumn = 
                new DataGridViewTextBoxColumn();
            diagramRowColumn.DataPropertyName = "diagramRow";
            diagramRowColumn.HeaderText = "Sheet Row";
            diagramRowColumn.Width = 5 * 8;
            diagramRowColumn.Name = "diagramRow";
            cardLocationBlockDataGridView.Columns.Remove("diagramRow");
            cardLocationBlockDataGridView.Columns.Insert(5, diagramRowColumn);

            //  Diagram column is an int, but use a plain old text box column
            //  anyway.  Setting the type to "int" just causes unfriendly
            //  DataError exceptions...

            DataGridViewTextBoxColumn diagramColumnColumn = 
                new DataGridViewTextBoxColumn();
            diagramColumnColumn.DataPropertyName = "diagramColumn";
            diagramColumnColumn.HeaderText = "Sheet Col.";
            diagramColumnColumn.Width = 5 * 8;
            diagramColumnColumn.Name = "diagramColumn";
            cardLocationBlockDataGridView.Columns.Remove("diagramColumn");
            cardLocationBlockDataGridView.Columns.Insert(4, diagramColumnColumn);

            //  ECOs is another text box column, because the combo box pick
            //  list would be enormously long.

            DataGridViewTextBoxColumn diagramECOColumn = 
                new DataGridViewTextBoxColumn();
            // diagramECOColumn.DataPropertyName = "eco";
            diagramECOColumn.HeaderText = "ECO";
            diagramECOColumn.Width = 8 * 8;
            diagramECOColumn.Name = "eco";
            cardLocationBlockDataGridView.Columns.Remove("diagramECO");
            cardLocationBlockDataGridView.Columns.Insert(6, diagramECOColumn);

            //  Then we have three check box columns...

            DataGridViewCheckBoxColumn identifiedOnSheetColumn =
                new DataGridViewCheckBoxColumn();
            identifiedOnSheetColumn.DataPropertyName = "identifiedOnSheet";
            identifiedOnSheetColumn.HeaderText = "On Sheet";
            identifiedOnSheetColumn.TrueValue = 1;
            identifiedOnSheetColumn.FalseValue = 0;
            identifiedOnSheetColumn.Width = 5 * 8;
            identifiedOnSheetColumn.HeaderCell.Style.Alignment = 
                DataGridViewContentAlignment.MiddleCenter;
            cardLocationBlockDataGridView.Columns.Remove("identifiedOnSheet");
            cardLocationBlockDataGridView.Columns.Insert(7, identifiedOnSheetColumn);

            DataGridViewCheckBoxColumn ignoreColumn =
                new DataGridViewCheckBoxColumn();
            ignoreColumn.DataPropertyName = "ignore";
            ignoreColumn.HeaderText = "Ignore";
            ignoreColumn.TrueValue = 1;
            ignoreColumn.FalseValue = 0;
            ignoreColumn.Width = 5 * 8;
            cardLocationBlockDataGridView.Columns.Remove("ignore");
            cardLocationBlockDataGridView.Columns.Insert(8, ignoreColumn);

            DataGridViewCheckBoxColumn missingColumn =
                new DataGridViewCheckBoxColumn();
            missingColumn.DataPropertyName = "missingDiagram";
            missingColumn.HeaderText = "Missing";
            missingColumn.TrueValue = 1;
            missingColumn.FalseValue = 0;
            missingColumn.Width = 7 * 8;
            cardLocationBlockDataGridView.Columns.Remove("missingDiagram");
            cardLocationBlockDataGridView.Columns.Insert(9, missingColumn);

            //  Fill in the text box columns.

            int rowIndex = 0;
            foreach (Cardlocationblock clb in cardLocationBlockList) {
                // Diagrampage diagramPage = diagramPageTable.getByKey(clb.diagramPage);
                // Page tempPage = pageTable.getByKey(diagramPage.page);
                Eco eco = ecoTable.getByKey(clb.diagramECO);
                if (clb.diagramPage > 0) {
                    ((DataGridViewTextBoxCell)
                        (cardLocationBlockDataGridView.Rows[rowIndex].Cells["page"])).Value =
                        getDiagramPageName(clb.diagramPage);
                }
                else if (clb.cableEdgeConnectionPage > 0) {
                    ((DataGridViewTextBoxCell)
                        (cardLocationBlockDataGridView.Rows[rowIndex].Cells["page"])).Value =
                        getDiagramPageName(clb.cableEdgeConnectionPage);
                }
                else {
                    //  If we don't have an ALD diagram or Cable/Edge connection diagram 
                    //  for this entry, then just ignore it.
                    continue;
                }
                ((DataGridViewTextBoxCell)
                    (cardLocationBlockDataGridView.Rows[rowIndex].Cells["eco"])).Value =
                    eco.eco;
                ((DataGridViewTextBoxCell)
                    (cardLocationBlockDataGridView.Rows[rowIndex].Cells["diagramRow"])).Value =
                    clb.diagramRow;
                ((DataGridViewTextBoxCell)
                    (cardLocationBlockDataGridView.Rows[rowIndex].Cells["diagramColumn"])).Value =
                    clb.diagramColumn;
                ++rowIndex;
            }

            //  Finally, reset the modified flags, and allow cell editing again.

            foreach(Cardlocationblock clb in cardLocationBlockList) {
                clb.modified = false;
            }

            currentCardLocation.modified = false;
            populatingDataGridView = false;
        }


        //  Routine to return the name of a diagram page.

        private string getDiagramPageName(int diagramPageKey) {

            int pageKey = 0;

            if(diagramPageKey == 0) {
                return "";
            }

            Diagrampage diagramPage = diagramPageTable.getByKey(diagramPageKey);
            if(diagramPage.idDiagramPage > 0) {
                pageKey = diagramPage.page;
            }
            else {
                Cableedgeconnectionpage cableEdgeConnectionPage =
                    cableEdgeConnectionPageTable.getByKey(diagramPageKey);
                if(cableEdgeConnectionPage.idCableEdgeConnectionPage > 0) {
                    pageKey = cableEdgeConnectionPage.page;
                }
            }


            if(pageKey == 0) {
                return "";
            }

            Page page = pageTable.getByKey(pageKey);
            if(page.idPage == 0) {
                return "";
            }

            return page.name;
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            //  If the user cancels, we obey!
            this.Close();
        }

        private void deleteButton_Click(object sender, EventArgs e) {

            //  Delete is pretty straightforward, as nothing refers to the
            //  CardLocation, CardLocationBottomNote or CardLocationBlock
            //  tables outside of the three tables themselves.  

            //  Also, the existing deletedCardLocationBlock list is irrelevant,
            //  as we are going to delete them all...

            deletedCardLocationBlockList = cardLocationBlockTable.getWhere(
                "WHERE cardLocation='" + currentCardLocation.idCardLocation + "'");

            List<Cardlocationbottomnote> deletedBottomNotesList =
                cardLocationBottomNoteTable.getWhere(
                    "WHERE cardLocation='" + currentCardLocation.idCardLocation + "'");

            DialogResult status =
                MessageBox.Show("Confirm that you wish to DELETE card location diagram " +
                    "block for Row " + rowComboBox.SelectedItem.ToString() +
                    ", Column " + columnComboBox.SelectedItem.ToString() +
                    " (Database ID " + currentCardLocation.idCardLocation + ") " +
                    (deletedBottomNotesList.Count > 0 || 
                     deletedCardLocationBlockList.Count > 0 ?
                        ", including " : "") +
                    (deletedBottomNotesList.Count > 0 ? 
                        deletedBottomNotesList.Count + " bottom notes " : "") +
                    (deletedBottomNotesList.Count > 0 
                     && deletedCardLocationBlockList.Count > 0 ?
                        " and " : "") +
                    (deletedCardLocationBlockList.Count > 0 ? 
                        deletedCardLocationBlockList.Count + " card location blocks" : ""),
                    "Confirm Card Deletion",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            //  If the user hits cancel button, just return.  Do NOT close
            //  the dialog, in case they just want to fix something and
            //  try again.

            if (status == System.Windows.Forms.DialogResult.Cancel) {
                return;
            }

            else if (status == DialogResult.OK) {

                //  A deleting we will go...

                foreach(Cardlocationbottomnote clbn in deletedBottomNotesList) {
                    cardLocationBottomNoteTable.deleteByKey(clbn.idCardLocationBottomNote);
                }

                foreach(Cardlocationblock clb in deletedCardLocationBlockList) {
                    cardLocationBlockTable.deleteByKey(clb.idCardLocationBlock);
                }

                cardLocationTable.deleteByKey(currentCardLocation.idCardLocation);

                MessageBox.Show("Card Location deleted.", "Card Location Deleted",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                //  Clear things out and repopulate the dialog.
                currentCardLocation = null;
                populateDialog();
            }
        }


        private void applyButton_Click(object sender, EventArgs e) {

            string message = "";
            int aborting = 0;

            List<string> addedNotesList = new List<string>();
            List<Cardlocationbottomnote> deletedNotesList = 
                new List<Cardlocationbottomnote>();
            List<Eco> addedECOList = new List<Eco>();
            List<Page> addedPageList = new List<Page>();
            List<Diagrampage> addedDiagramPageList = new List<Diagrampage>();
            List<Cableedgeconnectionpage> addedCableEdgeConnectionPageList =
                new List<Cableedgeconnectionpage>();
            List<Page> machinePageList = new List<Page>();
            List<Diagrampage> machineDiagramPageList = new List<Diagrampage>();
            List<Cableedgeconnectionpage> machineCableEdgeConnectionPageList =
                new List<Cableedgeconnectionpage>();
    

            Cardslot currentCardSlot;

            if(currentCardLocation == null) {
                currentCardLocation = new Cardlocation();
                currentCardLocation.idCardLocation = 0;
                message += "Add New Card Location to Page " +
                    currentPage.name +
                    ", Row " + rowComboBox.SelectedItem.ToString() +
                    ", Column + " + columnComboBox.SelectedItem.ToString() +
                    "\n";
            }

            if(currentCardLocation.modified == true) {
                message += "Update Card Location (Database ID " +
                    currentCardLocation.idCardLocation + ")\n";            
            }

            //  See if we need to do anything with the bottom notes...

            if (bottomNotesModified) {

                //  It would be easier to just delete them all and re-add them,
                //  rather than matching them up like this.  However, if we ever
                //  find out what these are really for, and thus add another column
                //  to the table, then we'd have to start matching them up, anyway.

                //  First, get a list of existing bottom notes

                List<Cardlocationbottomnote> bottomNoteList =
                    cardLocationBottomNoteTable.getWhere(
                        "WHERE cardLocation='" + currentCardLocation.idCardLocation + "'");

                //  Then get a list of the values in the current list.  I tried doing
                //  this with Regex but it turned into a nightmare.

                string[] matches = bottomNotesTextBox.Text.Split(
                    new string[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries);

                //  We use the modified flag to identify those bottom notes that
                //  matched!  When we are done, any that did not have matches from
                //  the textbox go bye bye.  Meanwhile, any that are in the text box
                //  that are not in the original list must necessarily be new.

                foreach (string s in matches) {
                    bool matched = false;
                    foreach(Cardlocationbottomnote clbn in bottomNoteList) {
                        if(clbn.note.CompareTo(s) == 0) {
                            clbn.modified = true;
                            matched = true;
                        }
                    }

                    //  If no match, then this is an add...

                    if (!matched) {
                        message += "Add Bottom note /" + s + "/\n";
                        addedNotesList.Add(s);
                    }
                }

                //  Now see which bottom notes will get the axe...

                deletedNotesList = new List<Cardlocationbottomnote>();
                foreach(Cardlocationbottomnote clbn in bottomNoteList) {
                    if(!clbn.modified) {
                        message += "Delete Bottom note " + clbn.note + "\n";
                        deletedNotesList.Add(clbn);
                    }
                }
            }

            //  Next, we have to check out the card location blocks for
            //  adds, deletes and changes...

            foreach(Cardlocationblock clb in deletedCardLocationBlockList) {
                int pageKey;

                if(clb.idCardLocationBlock > 0) { 
                pageKey = clb.cableEdgeConnectionPage > 0 ? clb.cableEdgeConnectionPage :
                    clb.diagramPage;

                message += "Delete Card Location Block for Page " +
                    getDiagramPageName(pageKey) +
                    ", Row " + clb.diagramRow +
                    ", Column " + clb.diagramColumn + "\n";
                }
            }

            //  Get a list of current pages and diagram pages for this
            //  machine - this is a bigger list than pageList, which is
            //  just the card location pages.

            machinePageList = pageTable.getWhere("" +
                "WHERE machine='" + currentMachine.idMachine + "'");
            foreach (Page mp in machinePageList) {
                machineDiagramPageList.AddRange(diagramPageTable.getWhere(
                    "WHERE diagrampage.page='" + mp.idPage + "'"));
                machineCableEdgeConnectionPageList.AddRange(cableEdgeConnectionPageTable.getWhere(
                    "WHERE cableedgeconnectionpage.page='" + mp.idPage + "'"));
            }

            int rowIndex = 0;
            if(cardLocationBlockList == null) {
                cardLocationBlockList = new List<Cardlocationblock>();
            }
            foreach (Cardlocationblock clb in cardLocationBlockList) {

                //  Fill in the current Card's location in case it is an add.

                clb.cardLocation = currentCardLocation.idCardLocation;

                //  Process adds and modifies, which may mean adding ECOs and
                //  pages.

                if(!clb.modified) {
                    ++rowIndex;
                    continue;
                }

                int diagramColumn = 0;
                DataGridViewRow dgvRow = cardLocationBlockDataGridView.Rows[rowIndex];

                //  Verify the Column is a valid integer, and fill it in.

                if (dgvRow.Cells["diagramColumn"].Value != null) {
                    int.TryParse(dgvRow.Cells["diagramColumn"].Value.ToString(),
                        out diagramColumn);
                }
                else {
                    diagramColumn = 0;
                }
                clb.diagramColumn = diagramColumn;

                //  Check to see if the ECO already exists.  If not, add it
                //  to the list to add.

                string ecoName = "";
                if (dgvRow.Cells["eco"].Value != null) {
                    ecoName = dgvRow.Cells["eco"].Value.ToString();
                }
                if (ecoName.Length > 0) {
                    List<Eco> tempECOList = ecoTable.getWhere(
                        "WHERE machine='" + currentMachine.idMachine + "'" +
                        " AND eco='" + ecoName + "'");
                    if (tempECOList.Count == 0) {
                        message += "Add ECO " + ecoName + "\n";
                        Eco newECO = new Eco();
                        newECO.eco = ecoName;
                        newECO.machine = currentMachine.idMachine;
                        addedECOList.Add(newECO);
                        clb.diagramECO = 0;
                    }
                    else {
                        clb.diagramECO = tempECOList[0].idECO;
                    }
                }

                //  Similar concept with DiagramPage / Cable/Edge Connection page / Page 

                string pageName = "";
                if (dgvRow.Cells["page"].Value != null) {
                    pageName = dgvRow.Cells["page"].Value.ToString();
                }
                if(pageName.Length == 0 ||
                    clb.diagramColumn == 0 || clb.diagramRow == null ||
                    clb.diagramRow.Length == 0) {
                    dgvRow.ErrorText = "Diagram Page, Row and Column are required";
                    return;
                }

                //  See if we have a matching Page and Diagram Page

                Page tempPage = machinePageList.Find(
                    x => x.name.CompareTo(pageName) == 0);
                if(tempPage == null) {
                    tempPage = new Page();
                    tempPage.idPage = 0;
                }

                //  Add a page.  If we can find the right volume using the 
                //  pageName, use that volume.  Otherwise, Use a volume called
                //  "Unassigned" (and add that too, if needed).

                if(tempPage.idPage == 0) {

                    Volume v;

                    tempPage.volume = 0;
                    v = volumeList.Find(
                        x => x.firstPage != null && x.lastPage != null &&
                        pageName.CompareTo(x.firstPage) >= 0 &&
                        pageName.CompareTo(x.lastPage) <= 0);
                    if(v != null && v.idVolume != 0) {
                        tempPage.volume = v.idVolume;
                    }

                    //  Failing that, see if we can find the volume by the
                    //  unassigned volume name.

                    if(tempPage.volume == 0) {
                        v = volumeList.Find(
                            x => x.name == "Unassigned");
                        if(v != null && v.idVolume != 0) {
                            tempPage.volume = v.idVolume;
                        }
                    }

                    //  Failing that, add a new volume...

                    if(tempPage.volume == 0 && aborting == 0) {
                        v = new Volume();
                        v.idVolume = IdCounter.incrementCounter();
                        //  Volume set same as selected volume...
                        v.set = currentVolume.set;
                        v.name = "Unassigned";
                        v.machineSerial = currentVolume.machineSerial;
                        volumeTable.insert(v);
                        tempPage.volume = v.idVolume;
                        message += "Added new volume 'Unassigned' to currently " +
                            "selected volume set.\n";
                    }

                    tempPage.idPage = 0;
                    tempPage.machine = currentMachine.idMachine;
                    tempPage.part = "";
                    tempPage.title = "Added Via Card Location Block Reference";
                    tempPage.stamp = tempPage.title;
                    tempPage.name = pageName;
                    addedPageList.Add(tempPage);
                    message += "Add Page AND diagram or cable/edge connection page " + 
                        pageName + "\n";
                }
                else {

                    //  See if we need to add the appropriate kind of page (ALD
                    //  diagram or Cable/Edge connection)

                    Debug.WriteLine("Current Card Type: /" +
                        currentCardLocation.type + "/");

                    if (Array.IndexOf(Helpers.cableEdgeConnectionCardTypes,
                        ((Cardtype)cardTypeComboBox.SelectedItem).type) >= 0) {
                        Cableedgeconnectionpage tempCableEdgeConnectionPage =
                            machineCableEdgeConnectionPageList.Find(
                                x => x.page == tempPage.idPage);
                        if(tempCableEdgeConnectionPage == null) {
                            MessageBox.Show("Page " + tempPage.name +
                                " is not a cable/edge connection page.  ABORT.",
                                "Page type / card location card type mismatch.",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            aborting = 1;
                        }
                        else { 
                            if(tempCableEdgeConnectionPage.idCableEdgeConnectionPage == 0) {
                                message += "Add Missing Cable/Edge Connection Page for page " +
                                    pageName + "\n";
                                addedCableEdgeConnectionPageList.Add(tempCableEdgeConnectionPage);
                            }
                        }
                    }
                    else {
                        Diagrampage tempDiagramPage = machineDiagramPageList.Find(
                            x => x.page == tempPage.idPage);
                        if(tempDiagramPage == null) {
                            MessageBox.Show("Page " + tempPage.name +
                                " is not a diagram connection page.  ABORT.",
                                "Page type / card location card type mismatch.",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            aborting = 1;
                        }
                        else { 
                            if (tempDiagramPage.idDiagramPage == 0) {
                                tempDiagramPage.page = tempPage.idPage;
                                message += "Add Missing Diagram Page for page " +
                                    pageName + "\n";
                                addedDiagramPageList.Add(tempDiagramPage);
                            }
                        }
                    }
                }

                if(aborting == 1) {
                    message += "Remainder of updates NOT applied due to earlier error.\n";
                    MessageBox.Show(message, "Remaining updates NOT applied.",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //  Finally, see if this Card Location Block is new

                if(clb.idCardLocationBlock == 0) {
                    message += "Add Card Location Block, Page " + pageName +
                        ", Row " + clb.diagramRow + ", Column " +
                        clb.diagramColumn.ToString() + "\n";
                }
                else {
                    message += "Modifying Card Location Block (Database ID " +
                        clb.idCardLocationBlock + ") Page " + pageName +
                        ", Row " + clb.diagramRow + ", Column " +
                        clb.diagramColumn.ToString() + "\n";
                }
                ++rowIndex;
            }

            //  Find the matching card slot entry, and see if we need to add it.

            List<Cardslot> cardSlotList = cardSlotTable.getWhere(
                "WHERE panel='" + currentPanel.idPanel + "'" +
                " AND cardRow='" + rowComboBox.SelectedValue + "'" +
                " AND cardColumn='" + columnComboBox.SelectedValue + "'");

            if(cardSlotList.Count > 0) {
                currentCardSlot = cardSlotList[0];
            }
            else {
                currentCardSlot = new Cardslot();
                currentCardSlot.panel = currentPanel.idPanel;
                currentCardSlot.cardRow = rowComboBox.SelectedValue.ToString();
                int v = 0;
                int.TryParse(columnComboBox.SelectedValue.ToString(), out v);
                currentCardSlot.cardColumn = v;
                currentCardSlot.idCardSlot = 0;
                message += "Add Card Slot Row " + currentCardSlot.cardRow +
                    ", Column " + currentCardSlot.cardColumn +
                    " to Panel " + currentPanel.panel + "\n";
            }

            //  If nothing has changed, let the user know that.

            if (message.Length == 0) {
                MessageBox.Show("No Changes to Card Location have been found",
                    "No Changes Found", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            //  Otherwise get confirmation before we proceed.

            DialogResult status =
                MessageBox.Show("Please confirm that you wish to:\n" + message,
                    "Confirm Card Location Updates",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (status == DialogResult.Cancel) {
                return;
            }
            else if (status != DialogResult.OK) {
                return;
            }

            //  So, user gave the OK...

            message = "";

            //  If we need to add the Card Slot, now is the time.

            if(currentCardSlot.idCardSlot == 0) {
                currentCardSlot.idCardSlot = IdCounter.incrementCounter();
                cardSlotTable.insert(currentCardSlot);
                message += "Added Card Slot For Panel " + currentCardSlot.panel +
                    ", Row " + currentCardSlot.cardRow +
                    ", Column " + currentCardSlot.cardColumn +
                    " Database ID" + currentCardSlot.idCardSlot + "\n";
            }

            currentCardLocation.page = currentCardLocationPage.idCardLocationPage;
            currentCardLocation.cardSlot = currentCardSlot.idCardSlot;
            currentCardLocation.crossedOut =
                crossedOutCheckBox.Checked ? 1 : 0;
            if (featureComboBox.SelectedItem == null) {
                currentCardLocation.feature = 0;
            }
            else {
                currentCardLocation.feature =
                    ((Feature)featureComboBox.SelectedItem).idFeature;
            }
            currentCardLocation.type =
                ((Cardtype)cardTypeComboBox.SelectedItem).idCardType;
                
            //  Add or update the Card Location itself

            if (currentCardLocation.idCardLocation == 0) {
                currentCardLocation.idCardLocation = IdCounter.incrementCounter();
                cardLocationTable.insert(currentCardLocation);
                message += "Added Card Location for Page " +
                    currentPage.name +
                    ", Row " + rowComboBox.SelectedItem.ToString() +
                    ", Column " + columnComboBox.SelectedItem.ToString() + 
                    " Database ID=" + currentCardLocation.idCardLocation + "\n";                        
            }
            else if(currentCardLocation.modified == true) {
                cardLocationTable.update(currentCardLocation);
                message += "Updated Card Location " +
                    "(Database ID " + currentCardLocation.idCardLocation + ")\n";
            }

            //  Then delete any card notes that are to be deleted

            foreach(Cardlocationbottomnote clbn in deletedNotesList) {
                cardLocationBottomNoteTable.deleteByKey(clbn.idCardLocationBottomNote);
                message += "Deleted Bottom Note " + clbn.note +
                    " (Database ID " + clbn.idCardLocationBottomNote + ")\n";
            }

            //  And then the adds (these have nothing to modify...)

            foreach(string s in addedNotesList) {
                Cardlocationbottomnote clbn = new Cardlocationbottomnote();
                clbn.idCardLocationBottomNote = IdCounter.incrementCounter();
                clbn.cardLocation = currentCardLocation.idCardLocation;
                clbn.note = s;
                cardLocationBottomNoteTable.insert(clbn);
                message += "Added Card Location Bottom Note " + clbn.note +
                    " Database ID=" + clbn.idCardLocationBottomNote + "\n";
            }

            //  Add any new ECOs

            foreach(Eco eco in addedECOList) {
                eco.idECO = IdCounter.incrementCounter();
                ecoTable.insert(eco);
                message += "Added ECO " + eco.eco + 
                    " Database ID=" + eco.idECO + "\n";
            }

            //  Add any new pages/diagram pages needed by the card location 
            //  blocks

            //  Add any new pages (and diagram pages or cable/edge connection pages
            //  at the same time)


            foreach(Page page in addedPageList) {
                page.idPage = IdCounter.incrementCounter();
                pageTable.insert(page);

                message += "Added page " + page.name +
                    " Database ID=" + page.idPage +
                    " WITH a ZERO volume key.\n";

                if (Array.IndexOf(Helpers.cableEdgeConnectionCardTypes,
                    ((Cardtype)cardTypeComboBox.SelectedItem).type) >= 0) {
                    Cableedgeconnectionpage newCableEdgeConnectionPage =
                        new Cableedgeconnectionpage();
                    newCableEdgeConnectionPage.idCableEdgeConnectionPage =
                        IdCounter.incrementCounter();
                    newCableEdgeConnectionPage.page = page.idPage;
                    cableEdgeConnectionPageTable.insert(newCableEdgeConnectionPage);
                    message += "Added corresponding cable/edge connection page, " +
                        "Database ID=" + newCableEdgeConnectionPage.idCableEdgeConnectionPage +
                        "\n";
                }
                else {
                    Diagrampage newDiagramPage = new Diagrampage();
                    newDiagramPage.idDiagramPage = IdCounter.incrementCounter();
                    newDiagramPage.page = page.idPage;
                    diagramPageTable.insert(newDiagramPage);
                    message += "Added corresponding diagram page, " +
                        "Database ID=" + newDiagramPage.idDiagramPage + "\n";
                }
            }

            //  There may also have been some missing diagram pages,
            //  so add them now.

            foreach (Diagrampage diagramPage in addedDiagramPageList) {
                Page page = pageTable.getByKey(diagramPage.page);
                diagramPage.idDiagramPage = IdCounter.incrementCounter();
                diagramPageTable.insert(diagramPage);
                message += "Added (missing) diagram page for page " +
                    page.name + " Database ID=" + diagramPage.idDiagramPage + "\n";
            }

            foreach(Cableedgeconnectionpage cableEdgeConnectionPage in
                addedCableEdgeConnectionPageList) {
                Page page = pageTable.getByKey(cableEdgeConnectionPage.page);
                cableEdgeConnectionPage.idCableEdgeConnectionPage =
                    IdCounter.incrementCounter();
                cableEdgeConnectionPageTable.insert(cableEdgeConnectionPage);
                message += "Added (missing) Cable/Edge Connection page for page " +
                    page.name + " Database ID=" + 
                    cableEdgeConnectionPage.idCableEdgeConnectionPage + "\n";
            }

            //  Now re-create the machine page list, machine diagram  and cable/edge
            //  conection page lists, and create a machine ECO list

            machineDiagramPageList = new List<Diagrampage>();
            machinePageList = pageTable.getWhere("" +
                "WHERE machine='" + currentMachine.idMachine + "'");
            foreach (Page mp in machinePageList) {
                machineDiagramPageList.AddRange(diagramPageTable.getWhere(
                    "WHERE diagrampage.page='" + mp.idPage + "'"));
                machineCableEdgeConnectionPageList.AddRange(cableEdgeConnectionPageTable.getWhere(
                    "WHERE cableedgeconnectionpage.page='" + mp.idPage + "'"));
            }

            List<Eco> machineECOList = ecoTable.getWhere(
                "WHERE machine='" + currentMachine.idMachine + "'");

            Debug.WriteLine("Number of clb's to delete: " +
                deletedCardLocationBlockList.Count);

            //  Delete any Card Location Blocks marked for bye-bye

            foreach (Cardlocationblock clb in deletedCardLocationBlockList) {
                int pageKey;

                if(clb.idCardLocationBlock > 0) { 
                    pageKey = clb.cableEdgeConnectionPage > 0 ? clb.cableEdgeConnectionPage :
                        clb.diagramPage;
                    cardLocationBlockTable.deleteByKey(clb.idCardLocationBlock);
                    message += "Deleted Card Location Block, Page " +
                        getDiagramPageName(pageKey) +
                        ", Page Row " + clb.diagramRow +
                        ", Page Column " + clb.diagramColumn +
                        " (Database ID " + clb.idCardLocationBlock + ")\n";
                    }
            }

            //  And then the adds and modifies.  We have already validated
            //  the data...

            Debug.WriteLine("Total number of clbs: " +
                cardLocationBlockList.Count);

            rowIndex = 0;
            foreach (Cardlocationblock clb in cardLocationBlockList) {

                //  If not modified (or new), skip it. 

                if(!clb.modified) {
                    ++rowIndex;
                    continue;
                }

                clb.cardLocation = currentCardLocation.idCardLocation;

                //  Getting the Diagram Page key takes a bit of work.
                //  We don't know if it was changed, so we always have to do this,
                //  whether the page is new or just modified.

                DataGridViewRow dgvRow = cardLocationBlockDataGridView.Rows[rowIndex];
                String pageName = dgvRow.Cells["page"].Value.ToString();

                Page tempPage = machinePageList.Find(
                    x => x.name.CompareTo(pageName) == 0);


                //if(tempPage == null) {
                //    //  Need to look this one up, because we just added it.
                //    //  Note that we don't know the volume (it is probably 0)
                //    List<Page> tempPageList = pageTable.getWhere(
                //        "WHERE machine='" + currentMachine.idMachine + "'" +
                //        " AND page.name='" + pageName + "'");
                //    if(tempPageList.Count != 1) {
                //        MessageBox.Show("Database Error.  Should have one page named " +
                //            pageName + " for machine " + currentMachine.name +
                //            ", but found " + tempPageList.Count + " pages. " +
                //            "will insert a 0 Diagram Page key for this " +
                //            "Card Location Block ",
                //            "Unexpected results from Page search",
                //            MessageBoxButtons.OK, MessageBoxIcon.Error);
                //        tempPage = new Page();
                //        tempPage.idPage = 0;
                //    }
                //}

                //List<Diagrampage> diagramPageList = diagramPageTable.getWhere(
                //    "WHERE diagrampage.page='" + tempPage.idPage + "'");
                //if(diagramPageList.Count != 1) {
                //    if (tempPage.idPage != 0) {
                //        MessageBox.Show("ERROR: Should have one page with database key " +
                //            tempPage.idPage + ", but found " +
                //            diagramPageList.Count + " entries.",
                //            "Unexpected results from Diagram Page search, " +
                //            "Will insert a 0 Diagram Page key for this " +
                //            "Card Location Block",
                //            MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    }
                //    clb.diagramPage = 0;
                //}
                //else {
                //    clb.diagramPage = diagramPageList[0].idDiagramPage;
                //}

                if(tempPage == null) {
                    MessageBox.Show("ERROR: Unable to find expected page with name " +
                        pageName, "ERROR: Can't Find Page",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tempPage = new Page();
                    tempPage.idPage = 0;
                }

                //  TODO  Change to add in cable/edge connection pages

                if(Array.IndexOf(Helpers.cableEdgeConnectionCardTypes,
                    ((Cardtype)cardTypeComboBox.SelectedItem).type) >= 0) {

                    Cableedgeconnectionpage tempCableEdgeConnectionPage =
                        machineCableEdgeConnectionPageList.Find(
                            x => x.page == tempPage.idPage);

                    if(tempCableEdgeConnectionPage == null) {
                        if(tempPage.idPage != 0) {
                            MessageBox.Show("ERROR: Unable to find expected Cable/Edge " +
                                "connection page for page with name " + pageName +
                                " (Database ID " + tempPage.idPage,
                                "ERROR: Can't Find Cable/Edge Connection Page",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            tempCableEdgeConnectionPage = new Cableedgeconnectionpage();
                            tempCableEdgeConnectionPage.idCableEdgeConnectionPage = 0;

                        }
                    }

                    clb.diagramPage = 0;
                    clb.cableEdgeConnectionPage = 
                        tempCableEdgeConnectionPage.idCableEdgeConnectionPage;

                }
                else {

                    Diagrampage tempDiagramPage = machineDiagramPageList.Find(
                        x => x.page == tempPage.idPage);

                    if(tempDiagramPage == null) {
                        if(tempPage.idPage != 0) {
                            MessageBox.Show("ERROR: Unable to find expected DIAGRAM " +
                                "for page with name " +  pageName +
                                " (Database ID " + tempPage.idPage, 
                                "ERROR: Can't Find DIAGRAM Page",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            tempDiagramPage = new Diagrampage();
                            tempDiagramPage.idDiagramPage = 0;
                        }
                    }

                    clb.diagramPage = tempDiagramPage.idDiagramPage;
                    clb.cableEdgeConnectionPage = 0;
                }

                //  Same story for the ECOs.

                if (dgvRow.Cells["eco"].Value != null &&
                    dgvRow.Cells["eco"].Value.ToString().Length > 0) {
                    String ecoName = dgvRow.Cells["eco"].Value.ToString();

                    Eco tempECO = machineECOList.Find(
                        x => x.eco.CompareTo(ecoName) == 0);
                    if (tempECO == null) {
                        MessageBox.Show("ERROR: Unable to find expected ECO " +
                            ecoName,
                            "ERROR: Can't Find ECO",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tempECO = new Eco();
                        tempECO.idECO = 0;
                    }
                    clb.diagramECO = tempECO.idECO;
                }
                else {
                    clb.diagramECO = 0;
                }

                //  Finally, the add or update of the Card Location Block

                if(clb.idCardLocationBlock == 0) {
                    clb.idCardLocationBlock = IdCounter.incrementCounter();
                    cardLocationBlockTable.insert(clb);
                    message += "Added Card Location Block on page " + pageName +
                        ", Row " + clb.diagramRow + ", Column " + clb.diagramColumn +
                        " Database ID=" + clb.idCardLocationBlock + "\n";
                }
                else {
                    cardLocationBlockTable.update(clb);
                    message += "Updated Card Location Block " +
                        " (Database ID " + clb.idCardLocationBlock +
                        " on page " + pageName +
                        ", Row " + clb.diagramRow + ", Column " + clb.diagramColumn +
                        "\n";
                }

                ++rowIndex;
            }

            // Tell the user what we did

            MessageBox.Show(message, "Updates Completed: \n",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            //  And then repopulate the dialog

            populateDialog();

        }


    }
}
