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
    public partial class EditCardGatePinsForm : Form
    {

        //  Class used to build the list of valid pins tha can be
        //  used for a given gate.  It is used to fill in the pin
        //  combo box in the data grid view.  We need to do this
        //  because the key field in cardGate is an integer, so
        //  we can never store a string in there.  We could have used
        //  the gatePin clas for this, but the usage would be misleading.

        class validGatePin
        {
            public int idGatePin { get; set; }
            public string pin { get; set; }
        }

        DBSetup db = DBSetup.Instance;

        Table<Cardgate> cardGateTable;
        Table<Volume> volumeTable;
        Table<Volumeset> volumeSetTable;
        Table<Cardtype> cardTypeTable;
        Table<Gatepin> gatePinTable;

        List<Volumeset> volumeSetList;
        List<Volume> volumeList;
        List<Cardtype> cardTypeList;
        List<Cardgate> cardGatesList;
        List<Cardgate> ioCardGatesList;
        List<Gatepin> deletedPinsList;
        List<Gatepin> gatePinsList;
        List<validGatePin> gatePinComboBoxList;

        BindingList<Gatepin> gatePinsBindingList;

        Volume currentVolume = null;
        Cardtype currentCardType = null;
        Cardgate currentCardGate = null;

        public EditCardGatePinsForm() {
            InitializeComponent();

            cardGateTable = db.getCardGateTable();
            volumeTable = db.getVolumeTable();
            volumeSetTable = db.getVolumeSetTable();
            cardTypeTable = db.getCardTypeTable();
            gatePinTable = db.getGatePinTable();

            //  Build a list of valid gate pins for the combo box, and
            //  also add a null entry for when the combo box is first 
            //  displayed.  The value is one more than the index 
            //  (to allow for a 0, empty entry)
            //  This list is NOT used for validating pins!

            gatePinComboBoxList = new List<validGatePin>();
            validGatePin vg0 = new validGatePin();
            vg0.idGatePin = 0;
            vg0.pin = "";
            for (int i = 0; i < Helpers.validPins.Length; ++i) {
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

            //  This is a fixed list, so we only need to do it once.

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
                    if (v.idVolume.ToString() == lastVolume) {
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
            //  the current card type (if any), to the last one this users used,
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

            populateGatesComboBox(currentCardType);
        }


        //  Method to fill in the Gates Comobo Box - if there are any.

        private void populateGatesComboBox(Cardtype cardType) {

            //  First, clear out any old entries.

            cardGatesComboBox.Items.Clear();

            //  If there is no card type, then this combo box will
            //  also be empty.

            if (cardType == null) {
                cardGatesList = new List<Cardgate>();
                currentCardGate = null;
                return;
            }

            //  Get the list of existing card gates for this card type

            cardGatesList = cardGateTable.getWhere(
                "WHERE cardType='" + cardType.idCardType + 
                    "' ORDER BY cardgate.number");

            //  Now, fill in the gates combo box.  

            foreach (Cardgate cg in cardGatesList) {
                cardGatesComboBox.Items.Add(cg.number.ToString());
            }

            //  If the list is not empty, select the first entry for now.

            if (cardGatesList.Count > 0) {
                currentCardGate = cardGatesList[0];
                cardGatesComboBox.SelectedIndex = 0;
            }
            else {
                currentCardGate = null;
            }

            //  Finally, popoulate the da
            populatePins(currentCardGate);
        }


        //  Method to populate the Pins Data Grid View.

        private void populatePins(Cardgate cardGate) {

            deletedPinsList = new List<Gatepin>();

            if (cardGate == null) {
                gatePinsList = new List<Gatepin>();
            }
            else {
                gatePinsList = gatePinTable.getWhere("" +
                    "WHERE cardGate='" + cardGate.idcardGate + "' ORDER BY pin");
            }

            //  Clear out the existing data grid view

            pinsDataGridView.DataSource = null;

            //  Create the list of data for the user to edit.

            gatePinsBindingList = new BindingList<Gatepin>(gatePinsList);
            gatePinsBindingList.AllowNew = true;
            gatePinsBindingList.AllowRemove = true;
            gatePinsBindingList.AllowEdit = true;

            pinsDataGridView.DataSource = gatePinsBindingList;

            //  Hide the columns that the user does not need to see.

            pinsDataGridView.Columns["modified"].Visible = false;
            pinsDataGridView.Columns["idGatePin"].Visible = false;
            pinsDataGridView.Columns["cardGate"].Visible = false;

            //  Now set up each column with header text and width - and, for
            //  combo boxes and check boxes, even more.

            pinsDataGridView.Columns["pin"].HeaderText = "Pin";
            pinsDataGridView.Columns["pin"].Width = 4 * 8;
            pinsDataGridView.Columns["voltageTenths"].HeaderText = "Voltage (Tenths)";
            pinsDataGridView.Columns["voltageTenths"].Width = 7 * 8;
            pinsDataGridView.Columns["mapPin"].Width = 10 * 8;
            pinsDataGridView.Columns["mapPin"].HeaderText = "Map To";
            ((DataGridViewTextBoxColumn)
                pinsDataGridView.Columns["mapPin"]).MaxInputLength = 8;

            //  The pin combo box is just a list of valid pins...

            DataGridViewComboBoxColumn pinColumn =
                new DataGridViewComboBoxColumn();
            pinColumn.DataPropertyName = "Pin";
            pinColumn.DataSource = gatePinComboBoxList;
            pinColumn.HeaderText = "Pin";
            pinColumn.ValueMember = "pin";    //  Since we really want char.
            pinColumn.DisplayMember = "pin";
            pinColumn.Width = 4 * 8;
            pinColumn.MaxDropDownItems = 17;
            pinsDataGridView.Columns.Remove("pin");
            pinsDataGridView.Columns.Insert(3, pinColumn);


            //  DataGridView combo boxes to handle child tables take some
            //  work.  They need to have a data source of the list representing
            //  the child table.  Also, they don't handle null well, so the lists
            //  have to be augmented to handle null values.

            //  Also, input vs. output is best displayed as a pair of
            //  check boxes (since a given pin might be both),
            //  but the underyling data is integer.  

            //  The ioGatesList is a copy, with an empty entry added.

            ioCardGatesList = new List<Cardgate>(cardGatesList);
            Cardgate nullIOGate = new Cardgate();
            nullIOGate.idcardGate = 0;
            nullIOGate.number = 0;
            ioCardGatesList.Add(nullIOGate);

            //  Now on to construct the columns for the data grid view

            DataGridViewComboBoxColumn inputGateColumn =
                new DataGridViewComboBoxColumn();
            inputGateColumn.DataPropertyName = "inputGate";
            inputGateColumn.HeaderText = "Input Gate";
            inputGateColumn.DataSource = ioCardGatesList;
            inputGateColumn.ValueMember = "idcardGate";
            inputGateColumn.DisplayMember = "number";
            inputGateColumn.Width = 6 * 8;
            pinsDataGridView.Columns.Remove("inputGate");
            pinsDataGridView.Columns.Insert(4, inputGateColumn);

            DataGridViewComboBoxColumn outputGateColumn =
                new DataGridViewComboBoxColumn();
            outputGateColumn.DataPropertyName = "outputGate";
            outputGateColumn.HeaderText = "Output Gate";
            outputGateColumn.DataSource = ioCardGatesList;
            outputGateColumn.ValueMember = "idcardGate";
            outputGateColumn.DisplayMember = "number";
            outputGateColumn.Width = 6 * 8;
            pinsDataGridView.Columns.Remove("outputGate");
            pinsDataGridView.Columns.Insert(5, outputGateColumn);

            //  Columns input, output, dotOutput and dotInput are all
            //  check box columns.

            DataGridViewCheckBoxColumn inputColumn =
                new DataGridViewCheckBoxColumn();
            inputColumn.DataPropertyName = "input";
            inputColumn.HeaderText = "In";
            inputColumn.TrueValue = 1;
            inputColumn.FalseValue = 0;
            inputColumn.Width = 5 * 8;
            pinsDataGridView.Columns.Remove("input");
            pinsDataGridView.Columns.Insert(6, inputColumn);

            DataGridViewCheckBoxColumn outputColumn =
                new DataGridViewCheckBoxColumn();
            outputColumn.DataPropertyName = "output";
            outputColumn.HeaderText = "Out";
            outputColumn.TrueValue = 1;
            outputColumn.FalseValue = 0;
            outputColumn.Width = 5 * 8;
            pinsDataGridView.Columns.Remove("output");
            pinsDataGridView.Columns.Insert(7, outputColumn);

            DataGridViewCheckBoxColumn dotInputColumn =
                new DataGridViewCheckBoxColumn();
            dotInputColumn.DataPropertyName = "dotInput";
            dotInputColumn.HeaderText = "DOT In";
            dotInputColumn.TrueValue = 1;
            dotInputColumn.FalseValue = 0;
            dotInputColumn.Width = 5 * 8;
            pinsDataGridView.Columns.Remove("dotInput");
            pinsDataGridView.Columns.Insert(8, dotInputColumn);

            DataGridViewCheckBoxColumn dotOutputColumn =
                new DataGridViewCheckBoxColumn();
            dotOutputColumn.DataPropertyName = "dotOutput";
            dotOutputColumn.HeaderText = "DOT Out";
            dotOutputColumn.TrueValue = 1;
            dotOutputColumn.FalseValue = 0;
            dotOutputColumn.Width = 5 * 8;
            pinsDataGridView.Columns.Remove("dotOutput");
            pinsDataGridView.Columns.Insert(9, dotOutputColumn);


        }

        private void volumeComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            //  If a different Volume is selected, then repopulate the Card Type
            //  (and eventually, the gates)

            currentVolume = volumeList[volumeComboBox.SelectedIndex];
            currentCardType = null;
            populateCardTypeComboBox(currentVolume, currentCardType);
        }

        private void cardTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            //  If we have not list of card types, just return.  (The user is using
            //  this dialog prematurely.

            if (cardTypeList == null || cardTypeComboBox.SelectedIndex < 0) {
                currentCardType = null;
                currentCardGate = null;
                return;
            }

            currentCardType = cardTypeList[cardTypeComboBox.SelectedIndex];
            currentCardGate = null;
            populateGatesComboBox(currentCardType);
        }

        private void cardGatesComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            //  If we have no list of gates, or no selection just return. 

            if (cardGatesList == null || cardGatesComboBox.SelectedIndex < 0) {
                currentCardGate = null;
                return;
            }

            currentCardGate = cardGatesList[cardGatesComboBox.SelectedIndex];
            populatePins(currentCardGate);
        }

        private void pinsDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {

            DataGridViewRow row = pinsDataGridView.Rows[e.RowIndex];

            //  If this is on a new row, ignore it.  Otherwise the user can end
            //  up in a "Catch-22" in cases where they didn't really mean to add
            //  a row, such as by hitting tab after editing the last existing row.

            if (pinsDataGridView.Rows[e.RowIndex].IsNewRow) {
                return;
            }

            string columnName = pinsDataGridView.Columns[e.ColumnIndex].Name;

            if (columnName.CompareTo("voltageTenths") == 0) {
                int vc;
                if (e.FormattedValue != null && e.FormattedValue != DBNull.Value &&
                    e.FormattedValue.ToString().Length > 0) {
                    if (!int.TryParse(
                        e.FormattedValue.ToString(), out vc)) {
                        row.ErrorText = columnName + " must be null or an integer.";
                        e.Cancel = true;
                    }
                }
                else {
                    row.ErrorText = "";
                }
            }
        }

        private void pinsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {

            //  If this is the title row, ignore it...
            if (e.RowIndex < 0) {
                return;
            }

            //  Mark the edited row as changed.

            Gatepin changedPin =
                (Gatepin)pinsDataGridView.Rows[e.RowIndex].DataBoundItem;
            changedPin.modified = true;
        }

        private void pinsDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {

            Gatepin changedGatePin = (Gatepin)e.Row.DataBoundItem;

            //  If the ID of this row is not filled in, then we can assume
            //  it was a new row that the user decided not to add.

            if (changedGatePin.idGatePin == 0) {
                return;
            }

            //  If this pin is a defining pin for the gate we cannot delete it...

            Cardgate cardGate = cardGateTable.getByKey(changedGatePin.cardGate);

            //  It shouldn't be null, but check just in case.

            if (cardGate == null) {
                return;
            }

            if (cardGate.definingPin == changedGatePin.idGatePin) {
                MessageBox.Show("ERROR: This Pin is the defining pin " +
                    "for Card Gate " + cardGate.number +
                    " and cannot be removed.",
                    "Pin is defining pin",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
            else {
                deletedPinsList.Add(changedGatePin);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            //  If the user cancels, we obey!
            this.Close();
        }

        private void applyButton_Click(object sender, EventArgs e) {

            string message = "";
            bool areChanges = false;
            bool abortUpdate = false;

            //  If the current Card Gate is null, there can be no changes.

            if (currentCardGate == null) {
                MessageBox.Show("Error:  No Gate Selected.  " +
                    "Changes cannot be processed.", "No Gate Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //  See if there were any deletions

            foreach (Gatepin gp in deletedPinsList) {
                message += "Deleting Pin " + gp.pin +
                    " (Database ID " + gp.idGatePin + ")\n";
                areChanges = true;
            }

            //  Then look at adds and changes...

            int pinRow = 0;
            foreach (Gatepin gp in gatePinsList) {
                if ((gp.pin == null || gp.pin.Length == 0) 
                    && gp.inputGate == 0 && gp.outputGate == 0) {
                    pinsDataGridView.Rows[pinRow].ErrorText =
                        "Pin Required";
                    abortUpdate = true;
                }
                else if(gp.pin == null) {
                    gp.pin = "";
                }
                if (gp.idGatePin == 0) {
                    message += "Adding Pin " + gp.pin + "\n";
                    areChanges = true;
                }
                else if (gp.modified) {
                    message += "Changing Pin, Database ID: " +
                        gp.idGatePin + ", Pin " + gp.pin + "\n";
                    areChanges = true;
                }
                ++pinRow;
            }

            //  If there are no changes, tell the user...

            if (!areChanges || abortUpdate) {
                MessageBox.Show("No Pin changes were completed",
                    "No Pin(s) updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult status =
                MessageBox.Show("Confirm that you wish to make " +
                    "the following changes to Pins for gate  " +
                    currentCardGate.number + ":\n\n" + message,
                    "Confirm Pin changes",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            //  If the user hits cancel button, just return.  Do NOT close
            //  the dialog, in case they just want to fix something and
            //  try again.

            if (status == System.Windows.Forms.DialogResult.Cancel) {
                return;
            }

            //  If they hit OK, proceed with the adds, removes and updates, and
            //  then close the form.

            else if (status == DialogResult.OK) {

                db.BeginTransaction();

                message = "";

                //  First, we do the deletes.

                foreach (Gatepin gp in deletedPinsList) {
                    gatePinTable.deleteByKey(gp.idGatePin);
                    message += "Pin " + gp.pin +
                        " (Database ID " + gp.idGatePin +
                        ") deleted. ";
                }

                //  Then the adds and updates.

                foreach (Gatepin gp in gatePinsList) {

                    //  If it is a new pin, get its key value, and add it.

                    if (gp.idGatePin == 0) {
                        gp.idGatePin = IdCounter.incrementCounter();
                        gp.cardGate = currentCardGate.idcardGate;
                        gatePinTable.insert(gp);
                        message += "Added Pin " + gp.pin +
                            " Database ID " + gp.idGatePin + "\n";
                    }
                    else if (gp.modified) {
                        gatePinTable.update(gp);
                        message += "Updated Pin " + gp.pin +
                            " (Database ID " + gp.idGatePin + ")\n";
                    }
                }

                //  Save the state of the selection combo boxes for later use by
                //  this form or other dialogs

                Parms.setParmValue("SMS volume", currentVolume.idVolume.ToString());
                Parms.setParmValue("SMS card type", currentCardType.idCardType.ToString());

                db.CommitTransaction();

                MessageBox.Show(message, "Pins updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                //  And then refresh the data grid view.

                populatePins(currentCardGate);
            }
        }
    }            
}
