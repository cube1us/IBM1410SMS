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
    public partial class EditMachinesForm : Form
    {

        DBSetup db = DBSetup.Instance;
        Table<Machine> machineTable;
        List<Machine> machineList;
        BindingList<Machine> machineBindingList;
        List<Machine> deletedMachineList = null;

        public EditMachinesForm() {
            InitializeComponent();


            populateDialog();
        }

        //  Method to  populate the rest of the dialog, which for this form,
        //  is simply the data grid!

        private void populateDialog() {

            //  Clear out the existing grid...

            machinesDataGridView.DataSource = null;

            //  Initialize with the list of machines from the database

            machineTable = db.getMachineTable();
            machineList = machineTable.getWhere(
                "ORDER BY machine.name");

            //  Build a new binding list for the DataGridView control,
            //  and adjust the control size and the parent window size
            //  appropriately.

            machineBindingList = new BindingList<Machine>(machineList);
            machineBindingList.AllowNew = true;
            machineBindingList.AllowRemove = true;
            machineBindingList.AllowEdit = true;

            machinesDataGridView.DataSource = machineBindingList;

            //  Hide the id  and modified columns.

            machinesDataGridView.Columns["idMachine"].Visible = false;
            machinesDataGridView.Columns["modified"].Visible = false;

            //  Set the titles and widths.

            machinesDataGridView.Columns["name"].HeaderText = "Machine";
            machinesDataGridView.Columns["name"].Width = 10 * 8;
            machinesDataGridView.Columns["description"].HeaderText = "Description";
            machinesDataGridView.Columns["description"].Width = 40 * 8;
            machinesDataGridView.Columns["aldmachinetype"].Width = 5 * 8;
            machinesDataGridView.Columns["aldmachinetype"].HeaderText = "Type";
            machinesDataGridView.Columns["framelabel"].Width = 8 * 8;
            machinesDataGridView.Columns["framelabel"].HeaderText = "Frame";
            machinesDataGridView.Columns["gatelabel"].Width = 8 * 8;
            machinesDataGridView.Columns["gatelabel"].HeaderText = "Gate";
            machinesDataGridView.Columns["panellabel"].Width = 8 * 8;
            machinesDataGridView.Columns["panellabel"].HeaderText = "Panel";
            machinesDataGridView.Columns["rowlabel"].Width = 8 * 8;
            machinesDataGridView.Columns["rowlabel"].HeaderText = "Row";
            machinesDataGridView.Columns["columnlabel"].Width = 8 * 8;
            machinesDataGridView.Columns["columnlabel"].HeaderText = "Column";

            //  Start a new list of possible deletions.

            deletedMachineList = new List<Machine>();
        }

        private void machinesDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {

            String columnName = machinesDataGridView.Columns[e.ColumnIndex].Name;
            DataGridViewRow row = machinesDataGridView.Rows[e.RowIndex];
            row.ErrorText = "";

            //  Skip if we are building the datagrid view or we are on a header
            //  row or a new row or if the string is empty.

            //  If this is on a new row, ignore it.  Otherwise the user can end
            //  up in a "Catch-22" in cases where they didn't really mean to add
            //  a row, such as by hitting tab after editing the last existing row.

            if (e.RowIndex < 0 || machinesDataGridView.Rows[e.RowIndex].IsNewRow) {
                return;
            }

            // string columnName = machinesDataGridView.Columns[e.ColumnIndex].Name;
            if (String.Compare(columnName, "name") == 0) {
                if (e.FormattedValue == null || e.FormattedValue.ToString().Length == 0 ||
                    e.FormattedValue.ToString().Length > 8) {
                    row.ErrorText = columnName + " is required, and " +
                        "has a max length of 8 characters.";
                    e.Cancel = true;
                }
            }
            else if (String.Compare(columnName, "description") == 0) {
                if (e.FormattedValue != null && e.FormattedValue.ToString().Length > 255) {
                    row.ErrorText = columnName + "has a max length of 255 characters.";
                    e.Cancel = true;
                }
            }
            else if (String.Compare(columnName, "aldMachineType") == 0) {
                if (e.FormattedValue != null && e.FormattedValue.ToString().Length > 2) {
                    row.ErrorText = "ALD Machine Type has a max length of 2 characters.";
                    e.Cancel = true;
                }
            }
            else if (String.Compare(columnName, "frameLabel") == 0) {
                if (e.FormattedValue != null && e.FormattedValue.ToString().Length > 8) {
                    row.ErrorText = "Frame label has a max length of 8 characters.";
                    e.Cancel = true;
                }
            }
            else if (String.Compare(columnName, "gateLabel") == 0) {
                if (e.FormattedValue != null && e.FormattedValue.ToString().Length > 8) {
                    row.ErrorText = "Gate label has a max length of 8 characters.";
                    e.Cancel = true;
                }
            }
            else if (String.Compare(columnName, "panelLabel") == 0) {
                if (e.FormattedValue != null && e.FormattedValue.ToString().Length > 8) {
                    row.ErrorText = "Panel label has a max length of 8 characters.";
                    e.Cancel = true;
                }
            }
            else if (String.Compare(columnName, "rowLabel") == 0) {
                if (e.FormattedValue != null && e.FormattedValue.ToString().Length > 8) {
                    row.ErrorText = "Row label has a max length of 8 characters.";
                    e.Cancel = true;
                }
            }
            else if (String.Compare(columnName, "columnLabel") == 0) {
                if (e.FormattedValue != null && e.FormattedValue.ToString().Length > 8) {
                    row.ErrorText = "Column label has a max length of 8 characters.";
                    e.Cancel = true;
                }
            }
        }

        private void machinesDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {

            //  If this is the title row, ignore it
            if (e.RowIndex < 0) {
                return;
            }

            //  Mark the edited row as changed.

            Machine changedMachine =
                (Machine)machinesDataGridView.Rows[e.RowIndex].DataBoundItem;
            changedMachine.modified = true;
        }

        private void machinesDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {

            List<Page> pageList;
            List<Feature> featureList;
            List<Eco> ecoList;
            List<Frame> frameList;
            Table<Page> pageTable = db.getPageTable();
            Table<Feature> featureTable = db.getFeatureTable();
            Table<Eco> ecoTable = db.getEcoTable();
            Table<Frame> frameTable = db.getFrameTable();

            string referencingTables = "Tables: ";
            Boolean referenced = false;

            //  Get a machine object representing the machine being removed.

            Machine changedMachine = (Machine)e.Row.DataBoundItem;

            //  If the ID of this row is 0, it is a new row, so it is safe to delete.

            if (changedMachine.idMachine == 0) {
                return;
            }

            //  Otherwise, see if there is anything referencing this machine.

            string whereClause = "WHERE machine='" + changedMachine.idMachine + "'";
            pageList = pageTable.getWhere(whereClause);
            featureList = featureTable.getWhere(whereClause);
            ecoList = ecoTable.getWhere(whereClause);
            frameList = frameTable.getWhere(whereClause);

            if (pageList.Count > 0) {
                referencingTables += "page, ";
                referenced = true;
            }
            if (featureList.Count > 0) {
                referencingTables += "feature, ";
                referenced = true;
            }
            if (ecoList.Count > 0) {
                referencingTables += "eco, ";
                referenced = true;
            }
            if (frameList.Count > 0) {
                referencingTables = "frame, ";
                referenced = true;
            }

            //  If the machine is referenced, tell the user, otherwise remember it
            //  for later deletion...

            if (referenced) {
                MessageBox.Show("ERROR: Machine " + changedMachine.name +
                    ", database ID: " + changedMachine.idMachine + " is referenced" +
                    " by other entries in " + referencingTables + " and cannot be removed.",
                    "Machine entry referenced by other entries",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
            else {
                deletedMachineList.Add(changedMachine);
            }
        }


        private void cancelButton_Click(object sender, EventArgs e) {

            //  Just Dew It.....
            this.Close();
        }

        private void applyButton_Click(object sender, EventArgs e) {

            //  Apply adds, deletes, updates

            string message = "";
            bool areChanges = false;

            //  Run through the deleted feature list.

            foreach (Machine machine in deletedMachineList) {
                message += "Deleting Machine " + machine.name +
                    " (Database ID: " + machine.idMachine + ")\n";
                areChanges = true;
            }

            //  Run through the list looking for adds and changes.

            foreach (Machine machine in machineList) {
                if (machine.idMachine == 0) {
                    message += "Adding Machine " + machine.name;
                    areChanges = true;
                }
                else if (machine.modified) {
                    message += "Changing Machine " + machine.name + 
                        " (Database ID: " + machine.idMachine +")\n";
                    areChanges = true;
                }
            }

            //  If there are not any changes, tell user and quit.

            if (!areChanges) {
                MessageBox.Show("No machine changes were completed",
                    "No Machines updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult status = MessageBox.Show("Confirm that you wish to make " +
                "the following changes to these machines: \n\n" + message,
                "Confirm Machine changes",
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

                message = "Machine(s) Updated:\n\n";

                db.BeginTransaction();

                //  First, we do the deletes (in case they delete and then
                //  re-add a feature with the same name.)

                foreach (Machine machine in deletedMachineList) {
                    machineTable.deleteByKey(machine.idMachine);
                    message += "Machine " + machine.name + 
                        " (Database ID " + machine.idMachine + ") removed.\n";
                }

                //  Next we do the adds and changes...

                foreach (Machine machine in machineList) {
                    if (machine.idMachine == 0) {
                        machine.idMachine = IdCounter.incrementCounter();
                        machineTable.insert(machine);
                        message += "Machine " + machine.name + " added.  Database ID=" +
                            machine.idMachine + "\n";
                    }
                    else if (machine.modified) {
                        machineTable.update(machine);
                        message += "Machine " + machine.name +
                            " (Database ID=" + machine.idMachine + ")" +
                            " updated.\n";
                    }
                }

                db.CommitTransaction();

                MessageBox.Show(message, "Machine(s) updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }

        }
    }
}
