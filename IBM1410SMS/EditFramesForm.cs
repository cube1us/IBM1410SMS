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
    public partial class EditFramesForm : Form
    {
        DBSetup db = DBSetup.Instance;
        Table<Machine> machineTable;
        Table<Frame> frameTable;
        Table<Machinegate> machineGateTable;
        List<Machine> machineList;
        List<Frame> frameList;
        List<Frame> deletedFrameList;
        BindingList<Frame> frameBindingList;
        Machine currentMachine = null;

        String frameLabel = "Frame";

        public EditFramesForm() {
            InitializeComponent();

            //  Set up the pull down list with the machines, and start with
            //  the first machine.

            machineTable = db.getMachineTable();
            machineList = machineTable.getAll();
            frameTable = db.getFrameTable();
            machineGateTable = db.getMachineGateTable();

            //  Fill in the machine combo box, and remember which machine
            //  we started out with.

            machineComboBox.DataSource = machineList;
            string lastMachine = Parms.getParmValue("machine");
            if(lastMachine.Length > 0) {
                currentMachine = machineList.Find(
                    x => x.idMachine.ToString() == lastMachine);
            }

            if(currentMachine == null || currentMachine.idMachine == 0) {
                currentMachine = machineList[0];
            }

            machineComboBox.SelectedItem = currentMachine;

            //  Then fill in the frame table with entries for that machine

            populateFrameTable(currentMachine);
        }

        //  Shared method to populate the DataGridView table that the user will edit.

        private void populateFrameTable(Machine machine) {

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            //  Set the appropriate dialog title

            frameLabel = machine.frameLabel;
            Text = "Edit " + frameLabel;

            //  Clear out the existing grid...

            framesDataGridView.DataSource = null;

            //  Find the Frames for the specified machine

            frameList = frameTable.getWhere("WHERE machine='" +
                machine.idMachine + "'");

            //  Build a new binding list for the DataGridView control,
            //  and adjust the control size and the parent window size
            //  appropriately.

            frameBindingList = new BindingList<Frame>(frameList);
            frameBindingList.AllowNew = true;
            frameBindingList.AllowRemove = true;
            frameBindingList.AllowEdit = true;
            framesDataGridView.DataSource = frameBindingList;

            //  Hide columns user does not need to see / should not change

            framesDataGridView.Columns["idFrame"].Visible = false;
            framesDataGridView.Columns["machine"].Visible = false;
            framesDataGridView.Columns["modified"].Visible = false;

            //  Make title row upper case.
            framesDataGridView.Columns["name"].HeaderText =
                textInfo.ToTitleCase(framesDataGridView.Columns["name"].HeaderText);

            //  Set the name column to a reasonable length, too.

            framesDataGridView.Columns["name"].Width = 10 * 8;

            //  Finally, throw away any entries we may have been saving for deletion...

            deletedFrameList = new List<Frame>();
        }

        private void machineComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            currentMachine = machineList[machineComboBox.SelectedIndex];
            populateFrameTable(currentMachine);
        }

        private void framesDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {

            //  If this is the title row, ignore it...
            if(e.RowIndex < 0) {
                return;
            }

            //  Mark the edited row as changed.

            Frame changedFrame =
                (Frame)framesDataGridView.Rows[e.RowIndex].DataBoundItem;
            changedFrame.modified = true;
        }


        private void framesDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {

            Frame changedFrame = (Frame)e.Row.DataBoundItem;

            //  If the ID on this row is not filled in, then we can assume
            //  it was a new row that the user decided not to add.

            if (changedFrame.idFrame == 0) {
                return;
            }

            //  So, the users wants to delete an existing row.  First, we need to make
            //  sure that there are not any references to it.

            List<Machinegate> machineGateList;

            string whereClause = "WHERE frame='" + changedFrame.idFrame + "'";
            machineGateList = machineGateTable.getWhere(whereClause);

            //  If there were references, cancel this entry.
            //  Otherwise, add it to the list of entries that we may delete later.

            if (machineGateList.Count > 0) {
                MessageBox.Show("ERROR: " + frameLabel + changedFrame.name +
                    ", database ID: " + changedFrame.idFrame + " is referenced" +
                    " by entries in the Machine Gate table and cannot be removed.",
                    frameLabel + " entry referenced by other entries",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
            else {
                deletedFrameList.Add(changedFrame);
            }

        }

        //  On cancel, just close the form without committing changes.

        private void cancelButton_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void applyButton_Click(object sender, EventArgs e) {

            string message = "";
            bool areChanges = false;

            //  Run through the deleted Frame list.

            foreach (Frame f in deletedFrameList) {
                message += "Deleting " + frameLabel + " " + f.name + 
                    " (Database ID: " + f.idFrame + ")\n";
                areChanges = true;
            }

            //  Run through the list looking for adds and changes.

            foreach (Frame f in frameList) {
                if (f.idFrame == 0) {
                    message += "Adding " + frameLabel + " " + f.name + "\n";
                    areChanges = true;
                }
                else if (f.modified) {
                    message += "Changing " + frameLabel + " Database ID: " + f.idFrame +
                        " to " + frameLabel + " name " + f.name + "\n";
                    areChanges = true;
                }
            }

            //  If there are not any changes, tell user and quit.

            if (!areChanges) {
                MessageBox.Show("No " + frameLabel + " changes were completed",
                    "No " + frameLabel + "s updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult status = MessageBox.Show("Confirm that you wish to make " +
                "the following changes to " + frameLabel + "s for machine " +
                currentMachine.name + ":\n\n" + message,
                "Confirm " + frameLabel + " changes",
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

                message = frameLabel + "s Updated:\n\n";

                db.BeginTransaction();

                //  First, we do the deletes (in case they delete and then
                //  re-add a Frame with the same name.)

                foreach (Frame f in deletedFrameList) {
                    frameTable.deleteByKey(f.idFrame);
                    message += frameLabel + " " + f.name + " removed.\n";
                }

                //  Next we do the adds and changes...

                foreach (Frame f in frameList) {
                    if (f.idFrame == 0) {
                        f.idFrame = IdCounter.incrementCounter();
                        f.machine = currentMachine.idMachine;
                        frameTable.insert(f);
                        message += frameLabel + " " + f.name + " added.  ID=" +
                            f.idFrame + "\n";
                    }
                    else if (f.modified) {
                        frameTable.update(f);
                        message += frameLabel + " " + f.name + " updated.  ID=" +
                            f.idFrame + "\n";
                    }
                }

                //  Remember the machine we last worked with.

                Parms.setParmValue("machine", currentMachine.idMachine.ToString());

                db.CommitTransaction();

                MessageBox.Show(message, frameLabel + "(s) updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }

        }

        private void framesDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {

            //  Clear out the error text.

            DataGridViewRow row =
                framesDataGridView.Rows[e.RowIndex];
            row.ErrorText = "";

            //  If this is on a new row, ignore it.  Otherwise the user can end
            //  up in a "Catch-22" in cases where they didn't really mean to add
            //  a row, such as by hitting tab after editing the last existing row.

            if (framesDataGridView.Rows[e.RowIndex].IsNewRow) {
                return;
            }

            string columnName = framesDataGridView.Columns[e.ColumnIndex].Name;
            if (String.Compare(columnName, "name") == 0) {
                if (e.FormattedValue == null || e.FormattedValue.ToString().Length == 0 ||
                    e.FormattedValue.ToString().Length > 8) {
                    row.ErrorText = columnName + " is required, and " +
                        "has a max length of 8 characters.";
                    e.Cancel = true;
                }
            }

        }
    }
}
