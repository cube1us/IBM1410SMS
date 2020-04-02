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

//  Note:  See datagridCardECO class at the end of this source.
//  It had to be after the Form class, or Visual Studio got confused
//  and was not able to open the form in design view.

namespace IBM1410SMS
{
    public partial class EditCardTypesForm : Form
    {

        DBSetup db = DBSetup.Instance;

        Table<Cardtype> cardTypeTable;
        Table<Cardeco> cardECOTable;
        Table<Cardnote> cardNoteTable;
        Table<Cardlocation> cardLocationTable;
        Table<Volume> volumeTable;
        Table<Volumeset> volumeSetTable;
        Table<Logicfamily> logicFamilyTable;
        Table<Eco> ecoTable;

        Table<Diagramblock> diagramBlockTable;
        Table<Tiedown> tieDownTable;
        Table<Cardgate> cardGateTable;
        Table<Gatepin> gatePinTable;
        Table<Machine> machineTable;

        Cardtype currentCardType = null;
        Volume currentVolume = null;

        List<Cardeco> cardECOList;
        List<datagridCardECO> datagridCardECOList;
        List<datagridCardECO> deletedCardECOList;
        List<Cardnote> cardNoteList;
        List<Cardnote> deletedCardNoteList;
        List<Cardtype> cardTypeList;
        List<Volumeset> volumeSetList;
        List<Volume> volumeList;
        List<Logicfamily> logicFamilyList;
        BindingList<Cardnote> cardNoteBindingList;
        // BindingList<Cardeco> cardECOBindingList;
        BindingList<datagridCardECO> datagridCardECOBindingList;
        

        public EditCardTypesForm() {
            InitializeComponent();

            cardTypeTable = db.getCardTypeTable();
            cardECOTable = db.getCardEcoTable();
            cardNoteTable = db.getCardNoteTable();
            volumeTable = db.getVolumeTable();
            volumeSetTable = db.getVolumeSetTable();
            logicFamilyTable = db.getLogicFamilyTable();
            ecoTable = db.getEcoTable();
            diagramBlockTable = db.getDiagramBlockTable();
            tieDownTable = db.getTieDownTable();
            cardGateTable = db.getCardGateTable();
            machineTable = db.getMachineTable();
            cardLocationTable = db.getCardLocationTable();
            gatePinTable = db.getGatePinTable();

            //  Popoulate the Logic Family list and combo box - they are fixed,
            //  so we only have to di ti once.

            logicFamilyList = logicFamilyTable.getWhere("ORDER BY logicfamily.name");
            logicFamilyComboBox.DataSource = logicFamilyList;

            //  Populate the Volume List and the Volume combo boxes,
            //  which correspond to the volume list, but display a string.
            //  We can use the index later to determine the database key of
            //  the associated volume.
            //  This is also a fixed list, so we only need to do it once.

            //  While we are at it, if we find a volume that matches the last
            //  SMS volume used, select it.

            string lastVolume = Parms.getParmValue("SMS volume");
            int selectedVolume = 0;

            volumeList = new List<Volume>();
            volumeSetList = volumeSetTable.getWhere("ORDER BY machineType");

            int index = 0;
            foreach(Volumeset vs in volumeSetList) {
                List<Volume> tempVolumeList = volumeTable.getWhere(
                    "WHERE volume.set='" + vs.idVolumeSet + "' ORDER BY volume.order");
                foreach(Volume v in tempVolumeList) {
                    volumeList.Add(v);
                    volumeComboBox.Items.Add(
                        "Vol. Set " + vs.machineType + ", Vol. " + v.name);
                    updatedVolumeComboBox.Items.Add(
                        "Vol. Set " + vs.machineType + ", Vol. " + v.name);
                    if(v.idVolume.ToString() == lastVolume) {
                        selectedVolume = index;
                    }
                    ++index;
                }
            }

            if(volumeList.Count > 0) {
                currentVolume = volumeList[selectedVolume];
                volumeComboBox.SelectedIndex = selectedVolume;
                updatedVolumeComboBox.SelectedIndex = selectedVolume;
            }
            else {

                //  If there is no volume list, the use of this dialog is premature...

                currentVolume = null;
            }

            //  Then populate the Card Type combo box (and the rest of the dialog)
            //  Allow it to pick the first card type in the list for this
            //  volume, initially.

            populateCardTypeComboBox(currentVolume, null);

        }


        //  Method to fill in the (existing) Card Type combo box - if there are 
        //  any.

        private void populateCardTypeComboBox(Volume volume, 
            Cardtype selectedCardType) {

            //  Clear out any old entries.

            cardTypeComboBox.Items.Clear();

            //  If there is no volume, then this combo box will also
            //  be empty.  (i.e., entering this dialog was premature).

            if (volume == null) {
                cardTypeList = new List<Cardtype>();
                currentCardType = null;
                return;
            }

            //  Get the list of existing card types for this volume

            cardTypeList = cardTypeTable.getWhere(
                "WHERE volume='" + volume.idVolume + "' ORDER BY cardtype.type");

            //  Now fill in the combo box.  This is another one of those
            //  where the combo box has text entries, and we use the
            //  current index to match up with the cardTypeList.

            int index = 0;
            int cbSelectedCardType = 0;
            string lastCardType = Parms.getParmValue("SMS card type");

            foreach(Cardtype ct in cardTypeList) {
                cardTypeComboBox.Items.Add(ct.type + ", Part No." + ct.part);
                if(ct.idCardType.ToString() == lastCardType) {
                    cbSelectedCardType = index;
                }
                ++index;
            }

            //  If the list is not empty, set the current card type (and
            //  eventually, the dialog) to either the current card type
            //  (if any) or to the first one in the list (if there is no
            //  currently selected card type).

            if (cardTypeList.Count > 0) {
                if (selectedCardType != null) {
                    cardTypeComboBox.SelectedIndex =
                        cardTypeList.IndexOf(
                            cardTypeList.Single(
                                i => i.idCardType == selectedCardType.idCardType));
                }
                else {
                    //  Null card type, so select the one from the parameter table
                    //  (or the first one, if no matchw as found.)

                    currentCardType = cardTypeList[cbSelectedCardType];
                    cardTypeComboBox.SelectedIndex = cbSelectedCardType;
                }
                cardTypeComboBox.Refresh();
            }
            else {
                currentCardType = null;
            }

            //  Populate the rest of the page.
            
            populateDialog(currentCardType);
        }

        //  Method to populate the Form itself

        private void populateDialog(Cardtype cardType) {

            //  Note that the Volume, (Existing) Card Types and Logic Families
            //  are populated elsewhere..

            //  Clear out the existing data grids...

            cardNotesDataGridView.Columns.Clear();
            cardNotesDataGridView.DataSource = null;
            cardECOsDataGridView.Columns.Clear();
            cardECOsDataGridView.DataSource = null;

            //  Throw away any entries we may have been saving for deletion

            deletedCardECOList = new List<datagridCardECO>();
            deletedCardNoteList = new List<Cardnote>();

            //  If the cardType is null, then just set up the defaults.

            if (cardType == null) {
                DateTime defaultDate = DateTime.Parse("1/1/1960");
                partTextBox.Text = "";
                typeTextBox.Text = "";
                nameTextBox.Text = "";
                singleRadioButton.Select();
                approvedByTextBox.ResetText();
                approvedDatePicker.Value = defaultDate;
                holePatternTextBox.ResetText();
                nameTypeTextBox.Text = "CARD ASM TSTR";
                developmentNumberTextBox.ResetText();
                designApproverTextBox.ResetText();
                designDatePicker.Value = defaultDate;
                detailerTextBox.ResetText();
                detailDatePicker.Value = defaultDate;
                designCheckerTextBox.ResetText();
                designCheckDatePicker.Value = defaultDate;
                approverTextBox.ResetText();
                approvalDatePicker.Value = defaultDate;
                modelTypeTextBox.Text = "SMS";
                modelDeviceTextBox.ResetText();
                scaleTextBox.Text = "NONE";
                drawTextBox.ResetText();
                drawDatePicker.Value = defaultDate;
                drawingCheckerTextBox.ResetText();
                drawingCheckDatePicker.Value = defaultDate;
                addApplyButton.Text = "Add";
                deleteButton.Visible = false;
                logicFamilyComboBox.SelectedIndex = 0;

                currentCardType = new Cardtype();
                currentCardType.idCardType = 0;

                cardNoteList = new List<Cardnote>();
                cardECOList = new List<Cardeco>();

            }
            else {

                //  OK, so we have an actual card type to display.
                //  First do the simple fields...

                partTextBox.Text = cardType.part;
                typeTextBox.Text = cardType.type;
                nameTextBox.Text = cardType.name;
                if (cardType.height > 1) {
                    doubleRadioButton.Select();
                }
                else {
                    singleRadioButton.Select();
                }

                approvedByTextBox.Text = cardType.approvedBy;
                approvedDatePicker.Value = cardType.approvedDate;
                holePatternTextBox.Text = cardType.holePattern;
                nameTypeTextBox.Text = cardType.nameType;
                developmentNumberTextBox.Text = cardType.developmentNumber;
                designApproverTextBox.Text = cardType.designApprover;
                designDatePicker.Value = cardType.designDate;
                detailerTextBox.Text = cardType.detailer;
                detailDatePicker.Value = cardType.detailDate;
                designCheckerTextBox.Text = cardType.designChecker;
                designCheckDatePicker.Value = cardType.designCheckDate;
                approverTextBox.Text = cardType.approver;
                approvalDatePicker.Value = cardType.approvalDate;
                modelTypeTextBox.Text = cardType.modelType;
                modelDeviceTextBox.Text = cardType.modelDevice;
                scaleTextBox.Text = cardType.scale;
                drawTextBox.Text = cardType.draw;
                drawDatePicker.Value = cardType.drawDate;
                drawingCheckerTextBox.Text = cardType.drawingChecker;
                drawingCheckDatePicker.Value = cardType.drawingCheckDate;
                logicFamilyComboBox.SelectedValue = cardType.logicFamily;

                addApplyButton.Text = "Apply";
                deleteButton.Visible = true;

                cardNoteList = cardNoteTable.getWhere(
                    "WHERE cardType='" + cardType.idCardType + 
                    "' ORDER BY noteName");

                cardECOList = cardECOTable.getWhere(
                    "WHERE cardType='" + cardType.idCardType + "'");
            }

            //  Next, we fill in the data grid views (even if they end up empty).

            cardNotesDataGridView.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            cardNoteBindingList = new BindingList<Cardnote>(cardNoteList);
            cardNoteBindingList.AllowNew = true;
            cardNoteBindingList.AllowRemove = true;
            cardNoteBindingList.AllowEdit = true;

            cardNotesDataGridView.DataSource = cardNoteBindingList;
            cardNotesDataGridView.Columns["idCardNote"].Visible = false;
            cardNotesDataGridView.Columns["cardType"].Visible = false;
            cardNotesDataGridView.Columns["modified"].Visible = false;

            cardNotesDataGridView.Columns["noteName"].DisplayIndex = 0;
            cardNotesDataGridView.Columns["note"].DisplayIndex = 1;

            cardNotesDataGridView.Columns["noteName"].HeaderText = "Note Name";
            cardNotesDataGridView.Columns["note"].HeaderText = "Note";
            cardNotesDataGridView.Columns["noteName"].MinimumWidth = 8 * 8;
            cardNotesDataGridView.Columns["note"].MinimumWidth = 8 * 8;

            cardNotesDataGridView.Columns["noteName"].FillWeight = 100;
            cardNotesDataGridView.Columns["note"].FillWeight = 400;


            //  The ECO one is a litte more complicated, because the ECO number
            //  is in another table, and it refers to the notes as well.
            //  (Yes, this could have been a view or join)

            //  Furthermore, I tried to use a DataGridViewColumn to hold the
            //  text equivalents of the properites in the Card ECO List that
            //  are keys to other tables (eco, note).  That did not work out.
            //  So instead we have the datagridCardECO subclass and list.

            //  (Perhaps someday this could be a constructor for datagridCardECO)

            datagridCardECOList = new List<datagridCardECO>();
            foreach (Cardeco cardECO in cardECOList) {

                //  First, copy the non-key values.
                datagridCardECO datagridCardECO = new datagridCardECO();
                datagridCardECO.approver = cardECO.approver;
                datagridCardECO.cardType = cardECO.cardType;
                datagridCardECO.date = cardECO.date;
                datagridCardECO.modified = cardECO.modified;
                datagridCardECO.idCardTypeECO = cardECO.idCardTypeECO;

                //  Also copy the note key, as we need it later when
                //  searching list list from the card ECO list

                datagridCardECO.note = cardECO.note;

                //  Get the text equivalents of the ECO # and note name.

                Eco eco = ecoTable.getByKey(cardECO.eco);
                datagridCardECO.textECO = eco.eco;

                Cardnote cardNote = cardNoteTable.getByKey(cardECO.note);
                datagridCardECO.noteName = cardNote.noteName;
                datagridCardECOList.Add(datagridCardECO);
            }

            datagridCardECOBindingList = 
                new BindingList<datagridCardECO>(datagridCardECOList);
            datagridCardECOBindingList.AllowNew = true;
            datagridCardECOBindingList.AllowRemove = true;
            datagridCardECOBindingList.AllowEdit = true;
            cardECOsDataGridView.DataSource = datagridCardECOBindingList;
            cardECOsDataGridView.Columns["idCardTypeECO"].Visible = false;
            cardECOsDataGridView.Columns["eco"].Visible = false;
            cardECOsDataGridView.Columns["cardType"].Visible = false;
            cardECOsDataGridView.Columns["note"].Visible = false;
            cardECOsDataGridView.Columns["modified"].Visible = false;

            cardECOsDataGridView.Columns["date"].HeaderText = "Date";
            cardECOsDataGridView.Columns["approver"].HeaderText = "Approver";
            cardECOsDataGridView.Columns["textECO"].HeaderText = "ECO";
            cardECOsDataGridView.Columns["noteName"].HeaderText = "Note Name";

            cardECOsDataGridView.Columns["textECO"].MinimumWidth = 8 * 8;
            cardECOsDataGridView.Columns["date"].MinimumWidth = 10 * 8;
            cardECOsDataGridView.Columns["approver"].MinimumWidth = 10 * 8;
            cardECOsDataGridView.Columns["noteName"].MinimumWidth = 10 * 8;

            cardECOsDataGridView.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            cardECOsDataGridView.Columns["textECO"].FillWeight = 100;
            cardECOsDataGridView.Columns["date"].FillWeight = 100;
            cardECOsDataGridView.Columns["approver"].FillWeight = 200;
            cardECOsDataGridView.Columns["noteName"].FillWeight = 100;


            cardECOsDataGridView.Columns["textECO"].DisplayIndex = 0;
            cardECOsDataGridView.Columns["date"].DisplayIndex = 1;
            cardECOsDataGridView.Columns["noteName"].DisplayIndex = 2;
            cardECOsDataGridView.Columns["approver"].DisplayIndex = 3;
        }


        //  Field validation methods.  In general, we just set the error message,
        //  rather than cancelling, to give the user the maxiumum flexibility.
        //  (The max length for these is set in the control, we de don't have
        //  to check it.)

        private void partTextBox_Validating(object sender, CancelEventArgs e) {
            string errorMessage = "";
            if(partTextBox.Text.Length < 1 || partTextBox.Text.Length > 10) {
                errorMessage = "Part# is required, 1-10 chars (Usually 7 digits).";
            }
            errorProvider1.SetError(partTextBox,errorMessage);
        }

        private void typeTextBox_Validating(object sender, CancelEventArgs e) {
            string errorMessage = "";
            if(typeTextBox.Text.Length == 0) {
                errorMessage = "Type is required, 1-8 chars.  (Usually 4).";
            }
            errorProvider1.SetError(typeTextBox, errorMessage);
        }

        private void nameTextBox_Validating(object sender, CancelEventArgs e) {
            string errorMessage = "";
            if (nameTextBox.Text.Length == 0) {
                errorMessage = "Name is required, 1-80 Characters";
            }
            errorProvider1.SetError(nameTextBox, errorMessage);
        }

        private void nameTypeTextBox_Validating(object sender, CancelEventArgs e) {
            string errorMessage = "";
            if (nameTypeTextBox.Text.Length == 0) {
                errorMessage = 
                    "Type (Name) is required, 1-45 chars.  (Usually 'CARD ASM TSTR').";
            }
            errorProvider1.SetError(nameTypeTextBox, errorMessage);
        }

        private void cardECOsDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e) {

            DataGridViewRow row =
                cardECOsDataGridView.Rows[e.RowIndex];
            string columnName = cardECOsDataGridView.Columns[e.ColumnIndex].Name;

            //  Ignore header row...

            if (e.RowIndex < 0) {
                return;
            }

            if (String.Compare(columnName, "date") == 0) {
                row.ErrorText = "Invalid Date Format";
                e.Cancel = true;
            }
        }


        private void cardECOsDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {

            //  Ignore header row...

            if(e.RowIndex < 0) {
                return;
            }

            //  Clear out the error text.

            DataGridViewRow row =
                cardECOsDataGridView.Rows[e.RowIndex];
            row.ErrorText = "";

            //  If this is on a new row, ignore it.  Otherwise the user can end
            //  up in a "Catch-22" in cases where they didn't really mean to add
            //  a row, such as by hitting tab after editing the last existing row.

            if (cardECOsDataGridView.Rows[e.RowIndex].IsNewRow) {
                return;
            }

            string columnName = cardECOsDataGridView.Columns[e.ColumnIndex].Name;
            if (String.Compare(columnName, "textECO") == 0) {
                if (e.FormattedValue == null || e.FormattedValue.ToString().Length == 0 ||
                    e.FormattedValue.ToString().Length > 20) {
                    row.ErrorText = columnName + " is required, and " +
                        "has a max length of 20 characters.";
                    e.Cancel = true;
                }
            }
            else if (String.Compare(columnName,"approver") == 0 ||
                     String.Compare(columnName,"noteName") == 0) {
                if (e.FormattedValue.ToString().Length > 8) {
                    row.ErrorText = columnName + 
                        "has a max length of 8 characters.";
                    e.Cancel = true;
                }
            }
            else if(String.Compare(columnName,"date") == 0) {
                DateTime tempDate;
                if(!DateTime.TryParse(e.FormattedValue.ToString(), out tempDate)) {
                    row.ErrorText = "Invalid Date";
                    e.Cancel = true;
                }
            }

            //  Set it to modified, so we know to update it later.

            ((Cardeco)cardECOsDataGridView.Rows[e.RowIndex].DataBoundItem).modified =
                true;

        }

        private void cardECOsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {

            //  Ignore changes to header row...

            if(e.RowIndex < 0) {
                return;
            }

            ((datagridCardECO)cardECOsDataGridView.Rows[e.RowIndex].DataBoundItem).modified = true;
        }

        private void cardNotesDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {

            //  Ignore header...

            if(e.RowIndex < 0) {
                return;
            }
            
            //  Clear out the error text.

            DataGridViewRow row =
                cardNotesDataGridView.Rows[e.RowIndex];
            row.ErrorText = "";

            //  If this is on a new row, ignore it.  Otherwise the user can end
            //  up in a "Catch-22" in cases where they didn't really mean to add
            //  a row, such as by hitting tab after editing the last existing row.

            if (cardNotesDataGridView.Rows[e.RowIndex].IsNewRow) {
                return;
            }

            string columnName = cardNotesDataGridView.Columns[e.ColumnIndex].Name;
            if (String.Compare(columnName, "noteName") == 0) {
                if (e.FormattedValue == null || e.FormattedValue.ToString().Length == 0 ||
                    e.FormattedValue.ToString().Length > 8) {
                    row.ErrorText = columnName + " is required, and " +
                        "has a max length of 8 characters.";
                    e.Cancel = true;
                }
            }
            else if (String.Compare(columnName, "note") == 0) {
                if (e.FormattedValue.ToString().Length > 255) {
                    row.ErrorText = columnName + 
                        "has a max length of 8 characters.";
                    e.Cancel = true;
                }
            }
        }

        private void cardNotesDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {

            //  Ignore changes to the header row...

            if (e.RowIndex < 0) {
                return;
            }

            ((Cardnote)cardNotesDataGridView.Rows[e.RowIndex].DataBoundItem).modified = true;
        }



        private void cardNotesDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {

            int references = 0;

            //  If an ECO entry refers to this note, then tell the user
            //  we cannot delete it (yet)

            Cardnote changedNote = (Cardnote)e.Row.DataBoundItem;

            //  If the ID of this row is not filled in, then we can remove it.
            //  (If there is an ECO that refers to it, it would necessarily also
            //  be new, and if it is indeed added, it will create a note with
            //  this name, but no note text).

            if(changedNote.idCardNote == 0) {
                return;
            }

            //  Now we look at the ECO list for references to this note.
            //  Because the ECO data grid view has all of the data, as the
            //  user currently wants it, we do NOT check the database - the
            //  user might have deleted any ECO(s) that refer to this note,
            //  but had not yet applied the update.

            foreach(datagridCardECO tempDatagridCardECO in datagridCardECOBindingList) {
                if(tempDatagridCardECO.note == changedNote.idCardNote) {
                    ++references;
                }
            }

            if(references > 0) {
                MessageBox.Show("ERROR: This Card Note is referenced by " +
                    references + " card ECOs and cannot be removed.",
                    "Note referenced by ECOs",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
            else {
                deletedCardNoteList.Add(changedNote);
            }
        }

        private void cardECOsDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {

            //  No verification needed when removing a Card ECO - nothing
            //  refers to them, so just remember that the user wishes to
            //  remove it...

            deletedCardECOList.Add((datagridCardECO)e.Row.DataBoundItem);
        }

    
        private void cancelButton_Click(object sender, EventArgs e) {
            //  User hit cancel.  Forget we were ever here...
            this.Close();
        }

        private void deleteButton_Click(object sender, EventArgs e) {

            string referencingTables = "";

            //  For this form, the only thing delete needs to check
            //  is for references to the Card Type table.  If we
            //  delete a Card Type, then the associated Card ECOs
            //  and Notes are also deleted (but not the underlying
            //  ECOs - those are left in place).

            //  If there is no current card type, that is an error.

            if(currentCardType == null) {
                MessageBox.Show("APPLICATION ERROR: Current Card Type is null",
                    "APPLICATION ERROR", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            //  If there is a current card type, but no ID, that is also
            //  an error.

            if (currentCardType.idCardType == 0) {
                MessageBox.Show("APPLICATION ERROR: Current Card Database ID is 0",
                    "APPLICATION ERROR", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            //  Next, see if anything else refers to this card type.

            List<Cardlocation> cardLocationList = cardLocationTable.getWhere(
                "WHERE cardlocation.type='" + currentCardType.idCardType + "'");
            List<Diagramblock> diagramBlockList = diagramBlockTable.getWhere(
                "WHERE cardType='" + currentCardType.idCardType + "'");
            List<Tiedown> tieDownList = tieDownTable.getWhere(
                "WHERE cardType='" + currentCardType.idCardType + "'");
            List<Cardgate> cardGateList = cardGateTable.getWhere(
                "WHERE cardType='" + currentCardType.idCardType + "'");

            if(cardLocationList.Count > 0) {
                referencingTables += "Card Location,";
            }
            if(diagramBlockList.Count > 0) {
                referencingTables += "Diagram Block,";
            }
            if(tieDownList.Count > 0) {
                referencingTables += "Tie Down";
            }

            if (referencingTables.Length > 0) {
                MessageBox.Show(
                "Card Type " + currentCardType.type +
                ", Part# " + currentCardType.part +
                " Cannot be deleted because it is referenced by tables " +
                referencingTables,
                "Card Type Referenced",
                MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            //  OK, so we can proceed to delete this card type.    
            //  Confirm that the user really means to delete it.

            DialogResult status = MessageBox.Show(
                "Confirm that you wish to delete Card Type " +
                currentCardType.type + ", Part# " +
                currentCardType.part +
                (cardGateList.Count > 0 ? 
                    " Including " + cardGateList.Count + " Gates" : ""),
                "Confirm Deletion",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if(status == DialogResult.Cancel) {
                return;
            }
            
            //  Cleared for deletion...

            //  Get a list of the associated Card Type ECOs and Notes.

            List<Cardeco> cardECOsToDelete = cardECOTable.getWhere(
                "WHERE cardType='" + currentCardType.idCardType + "'");
            List<Cardnote> cardNotesToDelete = cardNoteTable.getWhere(
                "WHERE cardType='" + currentCardType.idCardType + "'");

            //  Delete the ECOs and Notes...

            foreach(Cardeco cardECO in cardECOsToDelete) {
                cardECOTable.deleteByKey(cardECO.idCardTypeECO);
            }

            foreach(Cardnote cardNote in cardNotesToDelete) {
                cardNoteTable.deleteByKey(cardNote.idCardNote);
            }

            //  Then the gates and pins...

            foreach(Cardgate cardGate in cardGateList) {
                cardGateTable.deleteByKey(cardGate.idcardGate);
                List<Gatepin> pinList = gatePinTable.getWhere(
                    "WHERE cardGate='" + cardGate.idcardGate + "'");
                foreach (Gatepin pin in pinList) {
                    gatePinTable.deleteByKey(pin.idGatePin);
                }
            }

            //  And then the card type itself...

            cardTypeTable.deleteByKey(currentCardType.idCardType);

            MessageBox.Show("Card Type " + currentCardType.type + 
                ", Part# " + currentCardType.part + " has been deleted, " +
                "inlcuding its ECO references, Notes, Gates and Pins.",
                "Card Type Deleted",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            //  Then reset the current card type to null, and refresh the
            //  dialog - including the existing card type list, and
            //  let it select the first card type in the current volume,
            //  since the one we were working on is now gone.

            currentCardType = null;
            populateCardTypeComboBox(currentVolume, null);
        }

        private void newCardTypeButton_Click(object sender, EventArgs e) {

            //  If the user clicks the New Card Type, forget what may be
            //  there and start fresh.  (I had thought about tracking if
            //  the current one was modified, but eventually decided not
            //  to worry about that).

            currentCardType = null;
            populateDialog(currentCardType);
        }

        private void cardTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            //  Similarly, if the user selects a new card type, just
            //  forget what was there, and start fresh.

            if(cardTypeComboBox.SelectedIndex >= 0) {
                currentCardType = cardTypeList[cardTypeComboBox.SelectedIndex];
            }
            else {
                currentCardType = null;
            }

            populateDialog(currentCardType);
        }

        private void addApplyButton_Click(object sender, EventArgs e) {

            bool volumeChange = false;          //  True if user changed volume.

            //  Here are either adding a new Card Type, or updating an
            //  existing one...

            //  Check that the required fields are present.  If not,
            //  set their error text.

            if(volumeComboBox.SelectedIndex < 0) {
                errorProvider1.SetError(volumeComboBox, "A volume must be selected");
            }

            if(nameTextBox.Text.Length == 0) {
                errorProvider1.SetError(nameTextBox, "A name is required.");
            }

            if(partTextBox.Text.Length == 0) {
                errorProvider1.SetError(partTextBox, "A part number is required.");
            }

            if(typeTextBox.Text.Length == 0) {
                errorProvider1.SetError(typeTextBox, "A Card Type is required");
            }

            if(nameTypeTextBox.Text.Length == 0) {
                errorProvider1.SetError(nameTypeTextBox, "A Name Type is required, " +
                    "Usually CARD ASM TSTR-");
            }

            //  See if there are any existing errors...

            if(errorProvider1.GetError(partTextBox).Length > 0 ||
               errorProvider1.GetError(typeTextBox).Length > 0 ||
               errorProvider1.GetError(nameTextBox).Length > 0 ||
               errorProvider1.GetError(nameTypeTextBox).Length > 0) {
                MessageBox.Show("Data Fields required for Card Type are missing.",
                    "Required Fields Missing",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            //  All is OK.  Time to go to work.

            db.BeginTransaction();
            string message = "";

            //  Fill in the fields from the form.

            currentCardType.type = typeTextBox.Text;
            currentCardType.part = Importer.zeroPadPartNumber(partTextBox.Text);
            currentCardType.name = nameTextBox.Text;
            currentCardType.logicFamily = logicFamilyComboBox.SelectedIndex >= 0 ?
                ((Logicfamily)logicFamilyComboBox.SelectedItem).idLogicFamily : 0;
            currentCardType.height = doubleRadioButton.Checked ? 2 : 1;
            currentCardType.approvedBy = approvedByTextBox.Text;
            currentCardType.approvedDate = approvedDatePicker.Value;
            currentCardType.holePattern = holePatternTextBox.Text;
            currentCardType.nameType = nameTypeTextBox.Text;
            currentCardType.developmentNumber = developmentNumberTextBox.Text;
            currentCardType.designApprover = designApproverTextBox.Text;
            currentCardType.designDate = designDatePicker.Value;
            currentCardType.detailer = detailerTextBox.Text;
            currentCardType.detailDate = detailDatePicker.Value;
            currentCardType.designChecker = designCheckerTextBox.Text;
            currentCardType.designCheckDate = designCheckDatePicker.Value;
            currentCardType.approver = approverTextBox.Text;
            currentCardType.approvalDate = approvalDatePicker.Value;
            currentCardType.modelType = modelTypeTextBox.Text;
            currentCardType.modelDevice = modelDeviceTextBox.Text;
            currentCardType.scale = scaleTextBox.Text;
            currentCardType.draw = drawTextBox.Text;
            currentCardType.drawDate = drawDatePicker.Value;
            currentCardType.drawingChecker = drawingCheckerTextBox.Text;
            currentCardType.drawingCheckDate = drawingCheckDatePicker.Value;

            //  The user may have changed the selected volume, so set/update
            //  that as well.  Also, leave the current Volume item where they
            //  left it.

            //  While we are at it, preserve the current volume so we can
            //  keep displaying the original volume, and also reset the
            //  updatedVolumeComboBox back to its original setting, that
            //  matches the volumeComboBox.

            Volume oldVolume = currentVolume;
            currentVolume = (Volume)volumeList[updatedVolumeComboBox.SelectedIndex];
            if(currentVolume.idVolume != currentCardType.volume &&
                currentCardType.volume != 0) {
                volumeChange = true;
            }
            currentCardType.volume = currentVolume.idVolume;
            currentVolume = oldVolume;
            updatedVolumeComboBox.SelectedIndex = volumeComboBox.SelectedIndex;

            //  If this is a new card type, assign it a database ID, and add it.
            //  otherwise, just update it.  (We need the database ID to put in the
            //  card ECO and card Note tables).

            if (currentCardType.idCardType == 0) {
                currentCardType.idCardType = IdCounter.incrementCounter();
                cardTypeTable.insert(currentCardType);
                message += "Card Type " + currentCardType.type +
                    " added, Database ID = " + currentCardType.idCardType + "\n\n";
            }
            else {
                cardTypeTable.update(currentCardType);
                message += "Card Type + " + currentCardType.type +
                    " (Database ID " + currentCardType.idCardType + ") Updated.\n\n";
            }

            //  Next, we turn our attention to the notes.  We have to do them
            //  first, in case an ECO refers to one of them.

            //  First, delete any we no longer want.

            foreach(Cardnote cardNote in deletedCardNoteList) {
                cardNoteTable.deleteByKey(cardNote.idCardNote);
                message += "Card Note " + cardNote.noteName +
                    " (Database ID " + cardNote.idCardNote + ") removed.\n";
            }

            //  Next, process the Note list looking for inserts and adds...

            foreach(Cardnote cardNote in cardNoteBindingList) {
                cardNote.cardType = currentCardType.idCardType;
                if(cardNote.idCardNote == 0) {
                    cardNote.idCardNote = IdCounter.incrementCounter();
                    cardNoteTable.insert(cardNote);
                    message += "Card Note " + cardNote.noteName +
                        " added, Database ID " + cardNote.idCardNote + "\n";
                }
                else if(cardNote.modified == true) { 
                    cardNoteTable.update(cardNote);
                    message += "Card Note " + cardNote.noteName +
                        " (Database ID " + cardNote.idCardNote + ") updated...\n";
                }
            }

            //  Similar story with the Card ECOs.  But this is more complicated,
            //  because other tables are involved, and because the text name of the ECO
            //  only appears in the data grid view.

            foreach(Cardeco cardECO in deletedCardECOList) {
                cardECOTable.deleteByKey(cardECO.idCardTypeECO);
                //  Maybe fetch the ECO name here?
                message += "Card ECO " +
                    " (Database ID " + cardECO.idCardTypeECO + ") deleted.\n";
            }

            //  Iterate through the Card ECOs.  Here it is handy to use the
            //  superclass in the binding list, as it has the text values
            //  for the ECO and Note references.
 
            foreach(datagridCardECO datagridCardECO in datagridCardECOBindingList) {

                Eco eco;

                //  If this new or modified Card ECO doesn't have a database key from
                //  the ECO table, add it to the ECO table.  If it does have a database 
                //  key, then use that.  Adding the ECO to the ECO table might also 
                //  mean having to create a machine table entry...

                if (datagridCardECO.eco == 0) {
                    List<Eco> ecoList = ecoTable.getWhere("WHERE eco='" +
                        datagridCardECO.textECO + "'");
                    if (ecoList.Count > 0) {
                        eco = ecoList[0];
                    }

                    else {
                        eco = new Eco();

                        //  See if we have a machine that matches this card type's
                        //  modelComputer.  If so, use that machine.  Otherwise, add that 
                        //  to the list of machines - using "SMS" if none was
                        //  entered by the user.

                        string tempMachineName = currentCardType.modelDevice;
                        if (tempMachineName.Length == 0) {
                            tempMachineName = "SMS";
                        }

                        List<Machine> machineList = machineTable.getWhere(
                            "WHERE machine.name='" + tempMachineName + "'");

                        if (machineList.Count >= 1) {
                            eco.machine = machineList[0].idMachine;
                        }
                        else {
                            Machine tempMachine = new Machine();
                            tempMachine.idMachine = IdCounter.incrementCounter();
                            tempMachine.name = tempMachineName;
                            machineTable.insert(tempMachine);
                            message += "Machine Name " + tempMachine.name +
                                " added to Machine table, Database ID " +
                                tempMachine.idMachine + "\n";
                            eco.machine = tempMachine.idMachine;
                        }

                        //  Insert the newly minted ECO...

                        eco.idECO = IdCounter.incrementCounter();
                        eco.eco = datagridCardECO.textECO;
                        eco.description = "";
                        ecoTable.insert(eco);
                        message += "ECO " + eco.eco + " added to ECO table, " +
                            "Database ID " + eco.idECO + "\n";
                    }

                    datagridCardECO.eco = eco.idECO;
                }
                else {
                    eco = ecoTable.getByKey(datagridCardECO.eco);
                }

                //  See if this Card ECO has a note.  If so, then retrieve
                //  the name of that note from the Card Note list.  If it is
                //  the same as a noteName in the binding list, then compare
                //  the key to see if it changed.  If it did, then the ECO
                //  needs to be updated (if it wasn't new).  If the Note was
                //  note found, then add it now.

                if (datagridCardECO.noteName != null &&
                    datagridCardECO.noteName.Length > 0) {
                    bool foundNote = false;
                    foreach (Cardnote cardNote in cardNoteBindingList) {
                        if (string.Compare(cardNote.noteName,datagridCardECO.noteName) == 0) {
                            foundNote = true;
                            if (datagridCardECO.note != cardNote.idCardNote) {
                                datagridCardECO.note = cardNote.idCardNote;
                                datagridCardECO.modified = true;
                            }
                        }
                    }

                    if (!foundNote) {
                        //  It wasn't there, so add it now.  We don't bother with
                        //  adding it to the binding list, because that will get
                        //  refreshed when we are done applying the updates.
                        Cardnote tempCardNote = new Cardnote();
                        tempCardNote.idCardNote = IdCounter.incrementCounter();
                        tempCardNote.cardType = currentCardType.idCardType;
                        tempCardNote.noteName = datagridCardECO.noteName;                            
                        tempCardNote.note = "SMS - Added via Card Type " +
                            currentCardType.type;
                        cardNoteTable.insert(tempCardNote);
                        message += "Added Card Note " + datagridCardECO.noteName +
                            " automatically.  Database ID " + 
                            tempCardNote.idCardNote + "\n";
                        datagridCardECO.note = tempCardNote.idCardNote;
                    }
                }
                
                //  Finally, insert or update the Card ECO

                if(datagridCardECO.idCardTypeECO == 0) {
                    datagridCardECO.idCardTypeECO = IdCounter.incrementCounter();
                    datagridCardECO.cardType = currentCardType.idCardType;
                    cardECOTable.insert(datagridCardECO);
                    message += "Added Card ECO " + eco.eco +
                        " Database ID " + datagridCardECO.idCardTypeECO + "\n";
                }
                else if(datagridCardECO.modified) {
                    cardECOTable.update(datagridCardECO);
                    message += "Updated Card ECO " + eco.eco +
                        " (Database ID " + datagridCardECO.idCardTypeECO + ")\n";

                }
            }

            //  Save the combo box selectsion

            Parms.setParmValue("SMS volume", currentVolume.idVolume.ToString());
            Parms.setParmValue("SMS card type", currentCardType.idCardType.ToString());

            //  Tell the user what we did to him/her...

            db.CommitTransaction();

            MessageBox.Show(message, "Card Type Updates",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            //  Finally, repopulate the card type combo box (and the dialog).
            //  If the user changed the card volume, set the current card type
            //  to null as well.

            if(volumeChange) {
                currentCardType = null;
            }
            populateCardTypeComboBox(currentVolume, currentCardType);
        
        }

        private void volumeComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            //  Sync the "CHANGED" volume combo box with this one, since
            //  we will be discarding the current card type anyway.
             
            updatedVolumeComboBox.SelectedIndex = volumeComboBox.SelectedIndex;
            
            //  Then update the card type list.

            currentVolume = volumeList[volumeComboBox.SelectedIndex];
            currentCardType = null;
            populateCardTypeComboBox(currentVolume, currentCardType);
        }
    }


    //  Classes for displaying card ECOs in a data grid view.
    //  (Before I did this, if I set the value of columns I added, 
    //  things did not work out.

    //  Hated to do this, but nothing else I tried worked.  At least
    //  the class is trivial.

    public class datagridCardECO : Cardeco
    {
        public string textECO { get; set; }
        public string noteName { get; set; }
    }

}
