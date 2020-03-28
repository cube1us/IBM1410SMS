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
    public partial class EditMachineGatesForm : Form
    {
        DBSetup db = DBSetup.Instance;
        Table<Machine> machineTable;
        Table<Frame> frameTable;
        Table<Machinegate> machineGateTable;
        Table<Panel> panelTable;
        List<Machine> machineList;
        List<Frame> frameList;
        List<Machinegate> machineGateList;
        List<Machinegate> deletedMachineGateList;
        BindingList<Machinegate> machineGateBindingList;
        Machine currentMachine = null;
        Frame currentFrame = null;

        string frameLabel = "";
        string gateLabel = "";

        public EditMachineGatesForm() {
            InitializeComponent();

            //  Set up the pull down list with the machines, and start with
            //  the first machine.

            machineTable = db.getMachineTable();
            machineList = machineTable.getAll();
            frameTable = db.getFrameTable();
            machineGateTable = db.getMachineGateTable();
            panelTable = db.getPanelTable();

            //  Fill in the machine combo box, and remember which machine
            //  we started out with.

            machineComboBox.DataSource = machineList;
            currentMachine = machineList[0];

            //  Then do the same for the frame combo box, but just for the
            //  frames in this machine.  This also populates the data grid.

            populateFrameComboBox(currentMachine);
        }

        //  Shared method to populate the DataGridView table that the user
        //  will edit.

        private void populateMachineGateTable(Frame frame) {

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            //  Clear out the existing grid, and clear out any memory of
            //  deleted entries.

            machineGatesDataGridView.DataSource = null;
            deletedMachineGateList = new List<Machinegate>();

            //  If the parameter is null, we are done.  Otherwise, continue on.

            if(frame == null) {
                return;
            }

            //  Find the Machine Gates for the specified Frame (which in
            //  turn is bound to a given machine)

            machineGateList = machineGateTable.getWhere("WHERE frame='" +
                frame.idFrame + "'");

            //  Build a new binding list for the DataGridView control,
            //  and adjust the control size and the parent window size
            //  appropriately.

            machineGateBindingList = new BindingList<Machinegate>(machineGateList);
            machineGateBindingList.AllowNew = true;
            machineGateBindingList.AllowRemove = true;
            machineGateBindingList.AllowEdit = true;
            machineGatesDataGridView.DataSource = machineGateBindingList;

            //  Hide columns user does not need to see / should not change

            machineGatesDataGridView.Columns["idGate"].Visible = false;
            machineGatesDataGridView.Columns["frame"].Visible = false;
            machineGatesDataGridView.Columns["modified"].Visible = false;

            //  Make title row upper case, and set the width of the name forw.

            machineGatesDataGridView.Columns["name"].HeaderText =
                textInfo.ToTitleCase(machineGatesDataGridView.Columns["name"].HeaderText);
            machineGatesDataGridView.Columns["name"].Width = 10 * 8;

        }

        //  Shared method to populate (or not) the Frame combo box (a given
        //  machine might not have any).

        private void populateFrameComboBox(Machine machine) {

            frameList = frameTable.getWhere("WHERE machine='" +
                currentMachine.idMachine + "'");
            frameComboBox.DataSource = frameList;       //  Might be empty!

            //  Apply the appropriate labels based on the selected machine.
            //  Some machines may try and use the same label for both frames and
            //  gates, in which case we tweak it to make sure which which.

            frameLabel = machine.frameLabel;
            gateLabel = machine.gateLabel;
            if(gateLabel == null || gateLabel.Equals(frameLabel)) {
                gateLabel = gateLabel + "(Gate)";
            }

            selectFrameLabel.Text = "Then Select " + frameLabel + ":";
            Text = "Edit " + gateLabel + "s";

            //  Then if the machine has frames, set the current frame as
            //  the first one.

            currentFrame = frameList.Count > 0 ? frameList[0] : null;
            populateMachineGateTable(currentFrame);
        }

        private void machineComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            //  First, change the current machine.

            currentMachine = machineList[machineComboBox.SelectedIndex];

            //  Next, reset the Frames combo box, and re-populate the data grid view.

            populateFrameComboBox(currentMachine);
        }

        private void machineGatesDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {

            //  If this is the title row, ignore it...
            if (e.RowIndex < 0) {
                return;
            }

            //  Mark the edited row as changed.

            Machinegate changedMachineGate =
                (Machinegate)machineGatesDataGridView.Rows[e.RowIndex].DataBoundItem;
            changedMachineGate.modified = true;
        }


        private void machineGatesDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {
            Machinegate changedMachineGate = (Machinegate)e.Row.DataBoundItem;

            //  If the ID on this row is not filled in, then we can assume
            //  it was a new row that the user decided not to add.

            if (changedMachineGate.idGate == 0) {
                return;
            }

            //  So, the users wants to delete an existing row.  First, we need to make
            //  sure that there are not any references to it.

            List<Panel> panelList;

            string whereClause = "WHERE gate='" + changedMachineGate.idGate + "'";
            panelList = panelTable.getWhere(whereClause);

            //  If there were references, cancel this entry.
            //  Otherwise, add it to the list of entries that we may delete later.

            if (panelList.Count > 0) {
                MessageBox.Show("ERROR: Machine Gate " + changedMachineGate.name +
                    ", database ID: " + changedMachineGate.idGate + " is referenced" +
                    " by one or more entries in the Panel table and cannot be removed.",
                    "Machine Gate entry referenced by other entries",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
            else {
                deletedMachineGateList.Add(changedMachineGate);
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

            foreach (Machinegate mg in deletedMachineGateList) {
                message += "Deleting " + gateLabel + " " + mg.name +
                    " (Database ID: " + mg.idGate + ")\n";
                areChanges = true;
            }

            //  Run through the list looking for adds and changes.

            foreach (Machinegate mg in machineGateList) {
                if (mg.idGate == 0) {
                    message += "Adding " + gateLabel + " " + mg.name + "\n";
                    areChanges = true;
                }
                else if (mg.modified) {
                    message += "Changing " + gateLabel + " Database ID: " + mg.idGate +
                        " to " + gateLabel + " name " + mg.name + "\n";
                    areChanges = true;
                }
            }

            //  If there are not any changes, tell user and quit.

            if (!areChanges) {
                MessageBox.Show("No " + gateLabel + " changes were completed",
                    "No " + gateLabel + "s updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult status = MessageBox.Show("Confirm that you wish to make " +
                "the following changes to " + gateLabel + "s for machine " +
                currentMachine.name + ", " + frameLabel + " " + currentFrame.name + 
                ":\n\n" + message, "Confirm " + gateLabel + " changes",
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

                message = gateLabel + "(s) Updated:\n\n";

                db.BeginTransaction();

                //  First, we do the deletes (in case they delete and then
                //  re-add a Frame with the same name.)

                foreach (Machinegate mg in deletedMachineGateList) {
                    machineGateTable.deleteByKey(mg.idGate);
                    message += gateLabel + " " + mg.name + " removed.\n";
                }

                //  Next we do the adds and changes...

                foreach (Machinegate mg in machineGateList) {
                    if (mg.idGate == 0) {
                        mg.idGate = IdCounter.incrementCounter();
                        mg.frame = currentFrame.idFrame;
                        machineGateTable.insert(mg);
                        message += gateLabel + " " + mg.name + " added.  ID=" +
                            mg.idGate + "\n";
                    }
                    else if (mg.modified) {
                        machineGateTable.update(mg);
                        message += gateLabel + " " + mg.name + " updated.  ID=" +
                            mg.idGate + "\n";
                    }
                }

                db.CommitTransaction();

                MessageBox.Show(message, gateLabel + "(s) updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                //  For this one, we don't close the form, so that the user
                //  can select the next Frame.  But we do need to clear the
                //  deleted list, and reset the modified flags

                deletedMachineGateList = new List<Machinegate>();
                foreach(Machinegate mg in machineGateList) {
                    mg.modified = false;
                }
            }

        }

        private void frameComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            //  If/when the user selects a different Frame, then repopulate
            //  the datagrid (which also clears out the deleted machine gate list.

            currentFrame = frameList[frameComboBox.SelectedIndex];
            populateMachineGateTable(currentFrame);
        }


        private void machineGatesDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
            
            //  Clear out the error text.

            DataGridViewRow row =
                machineGatesDataGridView.Rows[e.RowIndex];
            row.ErrorText = "";

            //  If this is on a new row, ignore it.  Otherwise the user can end
            //  up in a "Catch-22" in cases where they didn't really mean to add
            //  a row, such as by hitting tab after editing the last existing row.

            if (machineGatesDataGridView.Rows[e.RowIndex].IsNewRow) {
                return;
            }


            string columnName = machineGatesDataGridView.Columns[e.ColumnIndex].Name;
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
