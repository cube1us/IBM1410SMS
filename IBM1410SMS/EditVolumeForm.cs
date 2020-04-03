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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MySQLFramework;

namespace IBM1410SMS
{
    public partial class EditVolumeForm : Form
    {

        DBSetup db = DBSetup.Instance;
        Table<Volumeset> volumeSetTable;
        Table<Volume> volumeTable;
        List<Volumeset> volumeSetList;
        List<Volume> volumeList;
        List<Volume> deletedVolumeList;
        BindingList<Volume> volumeBindingList;
        Volumeset currentVolumeSet = null;

        public EditVolumeForm() {

            int selectedVolumeSet = 0;
            InitializeComponent();


            //  Get the entire volume set list from the database, and use
            //  it to populate the combo box.

            volumeSetTable = db.getVolumeSetTable();
            volumeSetList = volumeSetTable.getAll();
            volumeTable = db.getVolumeTable();

            //  Build up the combo box entries.  We don't just use the
            //  list as a datasource becuase we want other context in the
            //  pull down.  

            //  While we are at it, look for the last volume set we worked with.

            string lastVolumeSet = Parms.getParmValue("volume set");

            int index = 0;
            foreach (Volumeset volumeset in volumeSetList) {
                volumeSetComboBox.Items.Add("Machine Type: " + volumeset.machineType +
                    (volumeset.machineSerial.ToString().Length != 0 ?
                        " S/N: " + volumeset.machineSerial : "") +
                    " ID: " + volumeset.idVolumeSet);
                if(volumeset.idVolumeSet.ToString().Equals(lastVolumeSet)) {
                    selectedVolumeSet = index;
                }
                ++index;
            }

            //  Select the appropriate entry.

            volumeSetComboBox.SelectedIndex = selectedVolumeSet;
            currentVolumeSet = volumeSetList[selectedVolumeSet];

            //  Similarly, initialize the volume combo box with volumes from
            //  the selected volume set.

            populateVolumeTable(currentVolumeSet);

        }

        //  Method to populate the volume combo box based on the currently
        //  selected volume set.

        private void populateVolumeTable(Volumeset volumeSet) {

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            //  Clear out the existing grid.

            volumesDataGridView.DataSource = null;

            //  Find the volumes for the specified volume set

            volumeList = volumeTable.getWhere("Where volume.set='" +
                volumeSetList[volumeSetComboBox.SelectedIndex].idVolumeSet + 
                "' ORDER BY volume.order");

            //  Build a new binding list for the DataGridView control,
            //  and adjust the control size appropriately.

            volumeBindingList = new BindingList<Volume>(volumeList);
            volumeBindingList.AllowNew = true;
            volumeBindingList.AllowRemove = true;
            volumeBindingList.AllowEdit = true;
            volumesDataGridView.DataSource = volumeBindingList;

            //  Hide columns the user does not need to see / should not change.

            volumesDataGridView.Columns["idVolume"].Visible = false;
            volumesDataGridView.Columns["set"].Visible = false;
            volumesDataGridView.Columns["modified"].Visible = false;

            //  Make title row upper case

            volumesDataGridView.Columns["name"].HeaderText =
                "Vol. Name";
            volumesDataGridView.Columns["machineSerial"].HeaderText =
                "Machine S/N";
            volumesDataGridView.Columns["firstPage"].HeaderText =
                            "First Page";
            volumesDataGridView.Columns["lastPage"].HeaderText =
                            "Last Page";
            volumesDataGridView.Columns["order"].HeaderText =
                            "Order";


            //  Set the column widths as well...

            volumesDataGridView.Columns["name"].Width = 10 * 16;
            volumesDataGridView.Columns["machineSerial"].Width = 10 * 16;
            volumesDataGridView.Columns["firstPage"].Width = 13 * 8;
            volumesDataGridView.Columns["machineSerial"].Width = 13 * 8;
            volumesDataGridView.Columns["order"].Width = 6 * 8;

            //  Finally, throw away any entries we may have been saving for deletion

            deletedVolumeList = new List<Volume>();
        }

        private void volumeSetComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            //  If the selection in the volume set combo box changes, 
            //  repopulate the data grid view

            currentVolumeSet = volumeSetList[volumeSetComboBox.SelectedIndex];
            populateVolumeTable(currentVolumeSet);
        }

        private void cancelButton_Click(object sender, EventArgs e) {

            //  If the user clicks cancel, just close the form.

            this.Close();
        }

        private void applyButton_Click(object sender, EventArgs e) {

            string message = "";
            bool areChanges = false;

            //  First, handle deleted volumes

            foreach(Volume v in deletedVolumeList) {
                message += "Deleting Volume " + v.name +
                    (v.machineSerial.Length > 0 ? ", Machine S/N: " + v.machineSerial : "") +
                    " (Database ID: " + v.idVolume + ")\n";
                areChanges = true;
            }

            //  Then look for adds and changes.

            foreach(Volume v in volumeList) {
                if(v.idVolume == 0) {
                    message += "Adding Volume " + v.name +
                        ((v.machineSerial != null && v.machineSerial.Length > 0) 
                            ? ", Machine S/N: " + v.machineSerial : "") + "\n";
                    areChanges = true;
                }
                else if (v.modified) {
                    message += "Changing Volume Database ID: " + v.idVolume + 
                        " to Volume " + v.name +
                        (v.machineSerial.Length > 0 ? ", Machine S/N: " + v.machineSerial : "") +
                        "\n";
                    areChanges = true;
                }
            }
            
            if(!areChanges) {
                MessageBox.Show("No Feature changes were completed",
                    "No Volumes updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;

            }


            //  Otherwise, go ahead and ask the user to confirm chanbges

            DialogResult status = MessageBox.Show("Confirm that you wish to " +
                "make the following changes to Volumes in volume set  for machine type " +
                currentVolumeSet.machineType + "\n\n" + message,
                "Confirm Volume Changes",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            //  If the user hits cancel button, just return.   Do NOT close
            //  the dialog, in case they just want to fix something and try again.

            if (status == System.Windows.Forms.DialogResult.Cancel) {
                return;
            }

            //  If they hit OK, then on we go...

            else if (status == DialogResult.OK) {

                db.BeginTransaction();

                message = "Volume(s) Updated:\n\n";

                //  First we do the deletes (in case they delete and then
                //  re-add a feature with the same name.)

                foreach (Volume v in deletedVolumeList) {
                    volumeTable.deleteByKey(v.idVolume);
                    message += "Volume " + v.name + " removed.\n";
                }

                //  Next we do the adds and changes...

                foreach (Volume v in volumeList) {
                    if (v.idVolume == 0) {
                        v.idVolume = IdCounter.incrementCounter();
                        v.set = currentVolumeSet.idVolumeSet;                        
                        volumeTable.insert(v);
                        message += "Voume " + v.name + " added.  ID=" +
                            v.idVolume + "\n";
                    }
                    else if (v.modified) {
                        volumeTable.update(v);
                        message += "Volume " + v.name + " updated.  ID=" +
                            v.idVolume + "\n";
                    }
                }

                //  Remember the volume set we updated, to select it in later dialogs.

                Parms.setParmValue("volume set", currentVolumeSet.idVolumeSet.ToString());

                db.CommitTransaction();

                MessageBox.Show(message, "Volume(s) updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
        }

        private void volumesDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {

            Table<Page> pageTable = db.getPageTable();
            Table<Cardtype> cardTypeTable = db.getCardTypeTable();

            List<Page> pageList;
            List<Cardtype> cardTypeList;

            string referencingTables = "Tables: ";
            Boolean referenced = false;


            Volume changedVolume = (Volume)e.Row.DataBoundItem;

            //  If the ID of this row is not filled in, then we can
            //  assume it was a new row that the user decided not to add

            if(changedVolume.idVolume == 0) {
                return;
            }

            //  So, the users wants to delete an existing row.  
            //  First we need to make sure there are not any references to it.

            //  Get a volume object representing the volume to be removed.


            //  Check to see if this volume is referenced from the other tables.
            //  If so, issue an error.

            string whereClause = "WHERE volume='" + changedVolume.idVolume + "'";
            pageList = pageTable.getWhere(whereClause);
            cardTypeList = cardTypeTable.getWhere(whereClause);

            if (pageList.Count > 0) {
                referencingTables += "page, ";
                referenced = true;
            }

            if (cardTypeList.Count > 0) {
                referencingTables += "cardtype";
                referenced = true;
            }

            //  If there were references, cancel this entry.
            //  Otherwise, add it to the list of entries that we may delete later.

            if (referenced) {
                MessageBox.Show("ERROR: This Volume is referenced " +
                    "by other entries in " + referencingTables + 
                    "and cannot be removed.",
                    "Volume entry referenced by other entries",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
            else {
                deletedVolumeList.Add(changedVolume);
            }
        
        }

        private void volumesDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            
            //  If this is a title row, ignore it.
            if(e.RowIndex < 0) {
                return;
            }

            //  Mark the edited row as changed.

            Volume changedVolume =
                (Volume)volumesDataGridView.Rows[e.RowIndex].DataBoundItem;
            changedVolume.modified = true;
        }

        private void volumesDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {

            //  Clear out the error text.

            DataGridViewRow row =
                volumesDataGridView.Rows[e.RowIndex];
            row.ErrorText = "";

            //  If this is on a new row, ignore it.  Otherwise the user can end
            //  up in a "Catch-22" in cases where they didn't really mean to add
            //  a row, such as by hitting tab after editing the last existing row.

            if (volumesDataGridView.Rows[e.RowIndex].IsNewRow) {
                return;
            }

            string columnName = volumesDataGridView.Columns[e.ColumnIndex].Name;
            if (String.Compare(columnName, "name") == 0) {
                if (e.FormattedValue == null || e.FormattedValue.ToString().Length == 0 ||
                    e.FormattedValue.ToString().Length > 16) {
                    row.ErrorText = columnName + " is required, and " +
                        "has a max length of 8 characters.";
                    e.Cancel = true;
                }
            }

            else if (String.Compare(columnName, "machineSerial") == 0) {
                if (e.FormattedValue != null && 
                    e.FormattedValue.ToString().Length > 45) {
                    row.ErrorText = columnName + 
                        " has a max length of 45 characters.";
                    e.Cancel = true;
                }
            }

            else if (String.Compare(columnName, "order") == 0) {
                int junk;
                if (e.FormattedValue != null &&
                    !int.TryParse(e.FormattedValue.ToString(), out junk)) {
                    row.ErrorText = columnName +
                        " must be an integer";
                    e.Cancel = true;
                }
            }

        }
    }
}
