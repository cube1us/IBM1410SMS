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
    public partial class EditCardGatesForm : Form
    {

        //  Class used to build the list of valid pins tha can be
        //  used for a given gate.  It is used to fill in the pin
        //  combo box in the data grid view.  We need to do this
        //  because the key field in cardGate is an integer, so
        //  we can never store a string in there.  We could have used
        //  the gatePin clas for this, but the usage would be misleading.

        class validGatePin {
            public int idGatePin { get; set; }
            public string pin { get; set; }
        }

        DBSetup db = DBSetup.Instance;

        Table<Cardgate> cardGateTable;
        Table<Volume> volumeTable;
        Table<Volumeset> volumeSetTable;
        Table<Cardtype> cardTypeTable;
        Table<Gatepin> gatePinTable;
        Table<Ibmlogicfunction> ibmLogicFunctionTable;
        Table<Logicfunction> logicFunctionTable;
        Table<Logiclevels> logicLevelsTable;
        Table<Diagramblock> diagramBlockTable;

        List<Volumeset> volumeSetList;
        List<Volume> volumeList;
        List<Cardtype> cardTypeList;
        List<Ibmlogicfunction> ibmLogicFunctionList;
        List<Logicfunction> logicFunctionList;
        List<Logiclevels> logicLevelsList;
        List<Cardgate> cardGatesList;
        List<Cardgate> latchGatesList;
        List<Cardgate> deletedGatesList;
        List<validGatePin> gatePinComboBoxList;

        BindingList<Cardgate> cardGatesBindingList;

        Volume currentVolume = null;
        Cardtype currentCardType = null;

        public EditCardGatesForm() {
            InitializeComponent();

            cardGateTable = db.getCardGateTable();
            volumeTable = db.getVolumeTable();
            volumeSetTable = db.getVolumeSetTable();
            cardTypeTable = db.getCardTypeTable();
            gatePinTable = db.getGatePinTable();
            ibmLogicFunctionTable = db.getIbmLogicFunctionTable();
            logicFunctionTable = db.getLogicFunctionTable();
            logicLevelsTable = db.getLogicLevelsTable();
            diagramBlockTable = db.getDiagramBlockTable();

            //  Populate the fixed lists: IBM logic function
            //  and (standard) logic functions.

            ibmLogicFunctionList = ibmLogicFunctionTable.getWhere(
                "ORDER BY label");
            logicFunctionList = logicFunctionTable.getWhere(
                "ORDER BY logicfunction.name");
            logicLevelsList = logicLevelsTable.getWhere(
                "ORDER BY logiclevel");

            //  Because of the way DataGridViewComboBox does NOT handle
            //  nulls well, add a corresponding entry to deal with that
            //  to each list.

            Ibmlogicfunction nullIBMLogicFunction = new Ibmlogicfunction();
            nullIBMLogicFunction.idIBMLogicFunction = 0;
            nullIBMLogicFunction.label = "";
            ibmLogicFunctionList.Add(nullIBMLogicFunction);

            Logicfunction nullLogicFunction = new Logicfunction();
            nullLogicFunction.idLogicFunction = 0;
            nullLogicFunction.name = "";
            logicFunctionList.Add(nullLogicFunction);

            Logiclevels nullLogicLevel = new Logiclevels();
            nullLogicLevel.idLogicLevels = 0;
            nullLogicLevel.logicLevel = "";
            logicLevelsList.Add(nullLogicLevel);

            //  Build a list of valid gate pins for the combo box, and
            //  as with the above lists, also add a null entry for when
            //  the combo box is first displayed.  The value is one
            //  more than the index (to allow for a 0, empty entry)
            //  This list is NOT used for validating pins!

            gatePinComboBoxList = new List<validGatePin>();
            validGatePin vg0 = new validGatePin();
            vg0.idGatePin = 0;
            vg0.pin = "";
            for(int i=0; i < Helpers.validPins.Length; ++i) {
                validGatePin vg = new validGatePin();
                vg.idGatePin = i + 1;
                vg.pin = Helpers.validPins[i].ToString();
                gatePinComboBoxList.Add(vg);
            }
            gatePinComboBoxList.Add(vg0);


            //  Populate the Volume List and the Volume combo box,
            //  which corresponds to the volume list, but displays a string.
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
            foreach (Volumeset vs in volumeSetList) {
                List<Volume> tempVolumeList = volumeTable.getWhere(
                    "WHERE volume.set='" + vs.idVolumeSet + "' ORDER BY volume.order");
                foreach (Volume v in tempVolumeList) {
                    volumeList.Add(v);
                    volumeComboBox.Items.Add("Vol. Set " + vs.machineType +
                        ", Vol. " + v.name);
                    if(v.idVolume.ToString() == lastVolume) {
                        selectedVolume = index;
                    }
                    ++index;
                }
            }

            if (volumeList.Count > 0) {
                currentVolume = volumeList[selectedVolume];
                volumeComboBox.SelectedIndex = selectedVolume;
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


        //  Method to fill in the the Card Type combo box - if there are 
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

            foreach (Cardtype ct in cardTypeList) {
                cardTypeComboBox.Items.Add(ct.type + ", Part No." + ct.part);
            }

            //  If the list is not empty, set the current card type to either 
            //  the current card type (if any), to the last one this user used,
            //  if a match is found, or to the first one in the list 
            //  (if there is no currently selected card type).
            
            string lastCardType = Parms.getParmValue("SMS card type");
            int cbSelectedCardType = cardTypeList.IndexOf(
                cardTypeList.Find(x => x.idCardType.ToString() == lastCardType));
            if (cbSelectedCardType < 0 && cardTypeList.Count > 0) {
                cbSelectedCardType = 0;
            }

            if (cardTypeList.Count > 0) {
                if (selectedCardType != null) {
                    cardTypeComboBox.SelectedIndex =
                        cardTypeList.IndexOf(
                            cardTypeList.Single(
                                i => i.idCardType == selectedCardType.idCardType));
                    cardTypeComboBox.Refresh();
                }
                else {
                    currentCardType = cardTypeList[cbSelectedCardType];
                    cardTypeComboBox.SelectedIndex = cbSelectedCardType;
                }
            }
            else {
                currentCardType = null;
            }

            populateGates(currentCardType);
        }


        //  Method to populate the Gates Data Grid View

        private void populateGates(Cardtype cardType) {

            deletedGatesList = new List<Cardgate>();

            if (cardType == null) {
                cardGatesList = new List<Cardgate>();
            }
            else {
                cardGatesList = cardGateTable.getWhere(
                    "WHERE cardType='" + cardType.idCardType + 
                    "' ORDER BY cardgate.number");
            }

            //  Clear out the existing data grid view

            cardGatesDataGridView.DataSource = null;

            //  Next, we need to go through the card gates list, and change
            //  the defining Pin values from the key to the values in our
            //  valiation list.  Also handle nulls for defining pin, which
            //  the combo box does not handle well.

            foreach(Cardgate cardGate in cardGatesList) {
                if(cardGate.definingPin != 0) {
                    Gatepin gatePin = gatePinTable.getByKey(cardGate.definingPin);
                    if(gatePin.pin != null) {
                        //  Match up the pin, and add one for our combo box
                        //  value offset.
                        cardGate.definingPin = 
                            Array.IndexOf(Helpers.validPins, gatePin.pin[0])+1;
                    }
                    else {
                        cardGate.definingPin = 0;
                    }
                }

            }

            //  Handle null for the input and output levels as well.
            
            //  Create the list of data for user to edit...

            cardGatesBindingList = new BindingList<Cardgate>(cardGatesList);
            cardGatesBindingList.AllowNew = true;
            cardGatesBindingList.AllowRemove = true;
            cardGatesBindingList.AllowEdit = true;

            cardGatesDataGridView.DataSource = cardGatesBindingList;

            //  Hide columns the user does not need to see.

            cardGatesDataGridView.Columns["modified"].Visible = false;
            cardGatesDataGridView.Columns["idcardGate"].Visible = false;
            cardGatesDataGridView.Columns["cardType"].Visible = false;

            //  Now set up each column with header text and width - and, for
            //  combo boxes and check boxes, even more.

            cardGatesDataGridView.Columns["number"].HeaderText = "Gate #";
            cardGatesDataGridView.Columns["number"].Width = 5 * 8;
            cardGatesDataGridView.Columns["transistorNumber"].HeaderText = "T#";
            cardGatesDataGridView.Columns["transistorNumber"].Width = 4 * 8;
            cardGatesDataGridView.Columns["componentValue"].HeaderText = "Comp. Value";
            cardGatesDataGridView.Columns["componentValue"].Width = 6 * 8;
            cardGatesDataGridView.Columns["HDLname"].Width = 16 * 8;
            cardGatesDataGridView.Columns["HDLname"].HeaderText = "HDL Name";

            //  DataGridView combo boxes to handle child tables take some
            //  work.  They need to have a data source of the list representing
            //  the child table.  Also, they don't handle null well, so the lists
            //  have to be augmented to handle null values.

            //  Also, the open collector is best displayed as a check box,
            //  but the underyling data is integer.  

            //  Defining Pin is a special case, because we might want to
            //  *create* a pin using this dialog.  So it needs to use
            //   a separate list.

            DataGridViewComboBoxColumn definingPinColumn =
                new DataGridViewComboBoxColumn();
            definingPinColumn.DataPropertyName = "definingPin";
            definingPinColumn.DataSource = gatePinComboBoxList;
            definingPinColumn.HeaderText = "Pin";
            definingPinColumn.ValueMember = "idGatePin";
            definingPinColumn.DisplayMember = "pin";
            definingPinColumn.Width = 4 * 8;
            definingPinColumn.MaxDropDownItems = 17;
            cardGatesDataGridView.Columns.Remove("definingPin");
            cardGatesDataGridView.Columns.Insert(4, definingPinColumn);

            //  Create the Combo Box Columns in place of the originals

            DataGridViewComboBoxColumn positiveLogicFunctionColumn =
                new DataGridViewComboBoxColumn();
            positiveLogicFunctionColumn.DataPropertyName = "positiveLogicFunction";
            positiveLogicFunctionColumn.HeaderText = "IBM+";
            positiveLogicFunctionColumn.DataSource = ibmLogicFunctionList;
            positiveLogicFunctionColumn.ValueMember = "idIBMLogicFunction";
            positiveLogicFunctionColumn.DisplayMember = "label";
            positiveLogicFunctionColumn.Width = 6 * 8;
            cardGatesDataGridView.Columns.Remove("positiveLogicFunction");
            cardGatesDataGridView.Columns.Insert(6,positiveLogicFunctionColumn);

            DataGridViewComboBoxColumn negativeLogicFunctionColumn =
                new DataGridViewComboBoxColumn();
            negativeLogicFunctionColumn.DataPropertyName = "negativeLogicFunction";
            negativeLogicFunctionColumn.HeaderText = "IBM-";
            negativeLogicFunctionColumn.DataSource = ibmLogicFunctionList;
            negativeLogicFunctionColumn.ValueMember = "idIBMLogicFunction";
            negativeLogicFunctionColumn.DisplayMember = "label";
            negativeLogicFunctionColumn.Width = 6 * 8;
            cardGatesDataGridView.Columns.Remove("negativeLogicFunction");
            cardGatesDataGridView.Columns.Insert(7, negativeLogicFunctionColumn);

            DataGridViewComboBoxColumn logicFunctionColumn =
                new DataGridViewComboBoxColumn();
            logicFunctionColumn.DataPropertyName = "logicFunction";
            logicFunctionColumn.HeaderText = "Logic Fun.";
            logicFunctionColumn.DataSource = logicFunctionList;
            logicFunctionColumn.ValueMember = "idLogicFunction";
            logicFunctionColumn.DisplayMember = "name";
            logicFunctionColumn.Width = 9 * 8;
            cardGatesDataGridView.Columns.Remove("logicFunction");
            cardGatesDataGridView.Columns.Insert(8, logicFunctionColumn);

            //  The latch gates list is a copy - with an empty entry added,
            //  to use for this combo box.

            latchGatesList = new List<Cardgate>(cardGatesList);
            Cardgate nullLatchGate = new Cardgate();
            nullLatchGate.idcardGate = 0;
            nullLatchGate.number = 0;
            latchGatesList.Add(nullLatchGate);

            //  Use that list to build the new combo box column.

            DataGridViewComboBoxColumn latchGateColumn =
                new DataGridViewComboBoxColumn();
            latchGateColumn.DataPropertyName = "latchGate";
            latchGateColumn.HeaderText = "Latch";
            latchGateColumn.DataSource = latchGatesList;
            latchGateColumn.ValueMember = "idcardGate";
            latchGateColumn.DisplayMember = "number";
            latchGateColumn.Width = 6 * 8;
            cardGatesDataGridView.Columns.Remove("latchGate");
            cardGatesDataGridView.Columns.Insert(9, latchGateColumn);

            //  Open collector is a little different, but the same
            //  general idea, of replacing the original auto selection
            //  with a special column type that maps the data back
            //  correctly.  (By default, a DataGridViewCheckBoxColumn
            //  expect an underlying bool, I think).

            DataGridViewCheckBoxColumn openCollectorColumn =
                new DataGridViewCheckBoxColumn();
            openCollectorColumn.DataPropertyName = "openCollector";
            openCollectorColumn.HeaderText = "Open Coll.";
            openCollectorColumn.TrueValue = 1;
            openCollectorColumn.FalseValue = 0;
            openCollectorColumn.Width = 5 * 8;
            cardGatesDataGridView.Columns.Remove("openCollector");
            cardGatesDataGridView.Columns.Insert(10, openCollectorColumn);

            //  And then the last two combo box columns.

            DataGridViewComboBoxColumn inputLevelColumn =
                new DataGridViewComboBoxColumn();
            inputLevelColumn.DataPropertyName = "inputLevel";
            inputLevelColumn.HeaderText = "In Lvl";
            inputLevelColumn.DataSource = logicLevelsList;
            inputLevelColumn.ValueMember = "idLogicLevels";
            inputLevelColumn.DisplayMember = "logicLevel";
            inputLevelColumn.Width = 6 * 8;
            cardGatesDataGridView.Columns.Remove("inputLevel");
            cardGatesDataGridView.Columns.Insert(11, inputLevelColumn);

            DataGridViewComboBoxColumn outputLevelColumn =
                new DataGridViewComboBoxColumn();
            outputLevelColumn.DataPropertyName = "outputLevel";
            outputLevelColumn.HeaderText = "Out Lvl";
            outputLevelColumn.DataSource = logicLevelsList;
            outputLevelColumn.ValueMember = "idLogicLevels";
            outputLevelColumn.DisplayMember = "logicLevel";
            outputLevelColumn.Width = 6 * 8;
            cardGatesDataGridView.Columns.Remove("outputLevel");
            cardGatesDataGridView.Columns.Insert(12, outputLevelColumn);

            //  And the No Ouput Exempt Column

            DataGridViewCheckBoxColumn noOutputExemptColumn =
                new DataGridViewCheckBoxColumn();
            noOutputExemptColumn.DataPropertyName = "noOutExempt";
            noOutputExemptColumn.HeaderText = "No Out Exempt";
            noOutputExemptColumn.TrueValue = 1;
            noOutputExemptColumn.FalseValue = 0;
            noOutputExemptColumn.Width = 6 * 8;
            cardGatesDataGridView.Columns.Remove("noOutExempt");
            cardGatesDataGridView.Columns.Insert(13, noOutputExemptColumn);
        }

        private void volumeComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            //  If a different Volume is selected, then repopulate the Card Type
            //  (and eventually, the gates)

            currentVolume = volumeList[volumeComboBox.SelectedIndex];
            currentCardType = null;
            populateCardTypeComboBox(currentVolume, currentCardType);
        }


        private void cardTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            currentCardType = cardTypeList[cardTypeComboBox.SelectedIndex];
            populateGates(currentCardType);
        }


        private void cardGatesDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {

            DataGridViewRow row =
                  cardGatesDataGridView.Rows[e.RowIndex];
            row.ErrorText = "";

            //  If this is on a new row, ignore it.  Otherwise the user can end
            //  up in a "Catch-22" in cases where they didn't really mean to add
            //  a row, such as by hitting tab after editing the last existing row.

            if (cardGatesDataGridView.Rows[e.RowIndex].IsNewRow) {
                return;
            }

            string columnName = cardGatesDataGridView.Columns[e.ColumnIndex].Name;

            //  We are only interested in the text box colunmns.

            switch (columnName) {
                case "number":
                case "transistorNumber":
                    int v;
                    if (!int.TryParse(
                        e.FormattedValue.ToString(), out v)) {
                        row.ErrorText = columnName + " must be an integer (or 0).";
                        e.Cancel = true;
                    }
                    break;
                case "componentValue":
                    int vc;
                    if (e.FormattedValue != null && e.FormattedValue != DBNull.Value &&
                        e.FormattedValue.ToString().Length > 0) {
                        if (!int.TryParse(
                            e.FormattedValue.ToString(), out vc)) {
                            row.ErrorText = columnName + " must be null or an integer.";
                            e.Cancel = true;
                        }
                    }
                    break;
                // case "Pin":
                case "definingPin":
                    string changedPin = e.FormattedValue.ToString();
                    if (changedPin.Length != 1 || !Helpers.validPins.Contains(changedPin[0])) {
                        row.ErrorText = columnName + "must be a 1 character valid Pin";
                        e.Cancel = true;
                    }
                    break;
            }
        }


        //  Because this DataGridVew has editing controls, unless we take
        //  some special measures, the delete key does not work as intended.
        //  This event handler takes care of that by registering a special
        //  event handler that checks specifically for the delete key.

        private void cardGatesDataGridView_EditingControlShowing(object sender, 
            DataGridViewEditingControlShowingEventArgs e) {

            //  First, remove any existing handler, if present, to avoid
            //  having multiple handlers.

            e.Control.KeyDown -= 
                new KeyEventHandler(cardGatesDataGridView_ControlKeyDown);

            //  And then add it back.

            e.Control.KeyDown +=
                new KeyEventHandler(cardGatesDataGridView_ControlKeyDown);
        }

        //  And this is the control key handler...

        void cardGatesDataGridView_ControlKeyDown(object sender,
            KeyEventArgs e) {

            //  Only the delete key gets special handling.

            if(e.KeyCode == Keys.Delete &&
                cardGatesDataGridView.SelectedRows.Count > 0  &&
                !cardGatesDataGridView.SelectedRows[0].IsNewRow) { 
                if (isDeleteRowOK((Cardgate)cardGatesDataGridView.SelectedRows[0].DataBoundItem)) {
                    cardGatesDataGridView.Rows.Remove(
                        cardGatesDataGridView.SelectedRows[0]);

                    //  Then make sure we don't actually delete a character.  We do this
                    //  by telling Windows to ignore the delete key itself.  
                    e.Handled = true;
                }
            }
        }


        private void cardGatesDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {

            //  If this is the title row, ignore it...
            if (e.RowIndex < 0) {
                return;
            }

            //  Mark the edited row as changed.

            Cardgate changedGate =
                (Cardgate)cardGatesDataGridView.Rows[e.RowIndex].DataBoundItem;
            changedGate.modified = true;
        }


        private void cardGatesDataGridView_UserDeletingRow(object sender, 
            DataGridViewRowCancelEventArgs e) {

            Cardgate changedCardGate = (Cardgate)e.Row.DataBoundItem;

            //  If it is NOT ok, then Cancel should be TRUE.

            if (changedCardGate != null) {
                e.Cancel = !isDeleteRowOK(changedCardGate);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            //  If the user cancels, we obey!

            this.Close();
        }

        //  Routine to check if a request to delete a row is OK.
        //  If is OK (and if the row is not a new one), then it adds the row
        //  to the list of rows to be deleted and returns true.

        private bool isDeleteRowOK(Cardgate changedCardGate) {
            string referencingTables = "";

            if(changedCardGate == null) {
                return false;
            }

            //  If the ID of this row is not filled in, then we can assume
            //  it was a enw row that the user decided not to add

            if (changedCardGate.idcardGate == 0) {
                return(true);
            }

            //  So, the user wants to delete an existing row.  First, we need
            //  to check that there are not any references to it.  The references
            //  from pins in this gate can be ignored (when we delete the gate, 
            //  we delete the pins, but we do have to worry about references from
            //  OTHER gates' pins, as well as references from other Gates as
            //  Latches with this gate.

            List<Cardgate> latchReferences = cardGateTable.getWhere(
                "WHERE latchGate='" + changedCardGate.idcardGate + "'");

            List<Gatepin> pinReferences = gatePinTable.getWhere(
                "WHERE inputGate='" + changedCardGate.idcardGate +
                "' OR outputGate='" + changedCardGate.idcardGate + "'");

            List<Diagramblock> diagramBlockReferences = diagramBlockTable.getWhere(
                "WHERE cardGate='" + changedCardGate.idcardGate + "'");

            if (latchReferences.Count > 0) {
                referencingTables += "cardGate (Latch), ";
            }

            if (pinReferences.Count > 0) {
                referencingTables += "gatePin (Other Gates), ";
            }

            if(diagramBlockReferences.Count > 0) {
                referencingTables += "diagramBlock (Logic Blocks)";
            }

            if (referencingTables.Length > 0) {
                MessageBox.Show("ERROR: This Card Gate is referenced " +
                    "by other entries in table(s)" + referencingTables +
                    "and cannot be removed.",
                    "Card Gate entry referenced by other entries",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false);
            }
            else {
                deletedGatesList.Add(changedCardGate);
                return (true);
            }

        }

        private void applyButton_Click(object sender, EventArgs e) {

            string message = "";
            bool areChanges = false;
            bool abortUpdate = false;

            //  If the current card type is null, there can be no changes...

            if(currentCardType == null) {
                MessageBox.Show("Error:  No Card Type Selected.  " +
                    "Changes cannot be processed.", "No Card Type Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //  First run through the deleted Card Gate List

            foreach(Cardgate cardGate in deletedGatesList) {
                message += "Deleting Gate " + cardGate.number +
                    " (Database ID: " + cardGate.idcardGate + ")\n";
                areChanges = true;
            }

            //  Then run through the list looking for adds and changes...

            int gateRow = 0;
            foreach(Cardgate cardGate in cardGatesList) {

                if(cardGate.definingPin == 0) {
                    cardGatesDataGridView.Rows[gateRow].ErrorText =
                        "Defining Pin required.";
                    abortUpdate = true;
                }
                if(cardGate.idcardGate == 0 && cardGate.number != 0) {
                    message += "Adding Card Gate " + cardGate.number + "\n";
                    areChanges = true;
                }
                else if(cardGate.modified) {
                    message += "Changing Card Gate, Database ID: " +
                        cardGate.idcardGate + ", Number " + cardGate.number +
                        "\n";
                    areChanges = true;
                }

                //  The HDLname column was added later, so initialize it.

                if(cardGate.HDLname == null) {
                    cardGate.HDLname = "";
                }
            }

            //  If there are no changes, tell the user...

            if (!areChanges || abortUpdate) {
                MessageBox.Show("No Card Gate changes were completed",
                    "No Card Gate(s) updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult status = 
                MessageBox.Show("Confirm that you wish to make " +
                    "the following changes to Card Gates for card  " +
                    currentCardType.type + ":\n\n" + message,
                    "Confirm Card Gate changes",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            //  If the user hits cancel button, just return.  Do NOT close
            //  the dialog, in case they just want to fix something and
            //  try again.

            if (status == System.Windows.Forms.DialogResult.Cancel) {
                return;
            }

            //  If they hit OK, proceed with the adds, removes and updates, 
            //  but keep the form open.

            else if (status == DialogResult.OK) {

                db.BeginTransaction();

                message = "";

                //  First, we do the deletes.

                foreach (Cardgate cardGate in deletedGatesList) {

                    //  First delete the Card Gate itself.  Since there
                    //  can be circular references between the Card Gate
                    //  Table and the Gate Pin table (due to "definingPin"),
                    //  the order of deleting is a Catch-22 referential
                    //  integrity wise.  This order makes the messaging
                    //  easier.

                    cardGateTable.deleteByKey(cardGate.idcardGate);
                    message += "Card Gate " + cardGate.number +
                        " (Database ID " + cardGate.idcardGate +
                        ") deleted. ";

                    //  We have already checked pin references when the
                    //  user went to delete in the first place, so go
                    //  ahead and delete them...

                    int deletedPins = 0;
                    List<Gatepin> gatePinList = gatePinTable.getWhere(
                        "WHERE cardGate='" + cardGate.idcardGate + "'");
                    foreach(Gatepin gatePin in gatePinList) {
                        gatePinTable.deleteByKey(gatePin.idGatePin);
                        ++deletedPins;
                    }

                    message += deletedPins > 0 ? ", including " + deletedPins +
                        " pins.\n" : "\n";
                }

                //  Then the adds/changes

                foreach (Cardgate cardGate in cardGatesList) {

                    bool newCardGate = false; 

                    //  If this will be a new Card Gate, get its key now, so
                    //  we can use it to add the defining pin.  As a result,
                    //  we also need a flag to remember that this is an add...

                    if(cardGate.idcardGate == 0) {
                        cardGate.idcardGate = IdCounter.incrementCounter();
                        newCardGate = true;
                    }

                    //  See if we need to add the defining pin.  Note
                    //  that at this point, the defining pin is NOT the database
                    //  key, but is instead an index into validPins[].  This can
                    //  occur during either an add or an udpated.  Also, note that
                    //  the defining pin from the combo box has a +1 offset.

                    List<Gatepin> gatePinList = gatePinTable.getWhere(
                        "WHERE pin='" + Helpers.validPins[cardGate.definingPin-1] + "'");

                    Gatepin gatePin;
                    string pinMessage = "";
                    if(gatePinList.Count == 0) {
                        //  Need to add the defining pin
                        gatePin = new Gatepin();
                        //  The pin number has a +1 offset.  Remove that here.
                        gatePin.pin = Helpers.validPins[cardGate.definingPin-1].ToString();
                        gatePin.idGatePin = IdCounter.incrementCounter();
                        gatePin.cardGate = cardGate.idcardGate;
                        gatePinTable.insert(gatePin);
                        pinMessage = "Defining Pin " + gatePin.pin + " added.\n";
                    }
                    else {
                        gatePin = gatePinList[0];
                    }

                    //  Set the defining pin (back to) the database ID

                    cardGate.definingPin = gatePin.idGatePin;

                    //  Update the Card Gate for this Gate, just in case.

                    cardGate.cardType = currentCardType.idCardType;

                    //  Hopefully, the DataGridView already filled in the rest
                    //  of the data.

                    if (newCardGate) {
                        cardGateTable.insert(cardGate);
                        message += "Added Card Gate " + cardGate.number +
                            " Database ID " + cardGate.idcardGate + "\n";
                    }
                    else if (cardGate.modified) {
                        cardGateTable.update(cardGate);
                        message += "Updated Card Gate " + cardGate.number +
                            " (Database ID" + cardGate.idcardGate + "\n";
                    }

                    message += pinMessage;
                }

                //  Save the state of the selection combo boxes for later use by
                //  this form or other dialogs

                Parms.setParmValue("SMS volume", currentVolume.idVolume.ToString());
                Parms.setParmValue("SMS card type", currentCardType.idCardType.ToString());

                db.CommitTransaction();

                MessageBox.Show(message, "Card Gate(s) updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                populateGates(currentCardType);
            }

        }
    }
}
