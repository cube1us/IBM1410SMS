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
    public partial class EditECOsForm : Form
    {
        DBSetup db = DBSetup.Instance;
        Table<Machine> machineTable;
        Table<Eco> ecoTable;
        Table<Cardlocationpage> cardLocationPageTable;
        Table<Cardlocationblock> cardLocationBlockTable;
        Table<Diagramblock> diagramBlockTable;
        Table<Cardeco> cardEcoTable;
        Table<Diagramecotag> diagramEcoTagTable;

        List<Machine> machineList;
        List<Eco> ecoList;
        List<Eco> deletedEcoList;
        BindingList<Eco> ecoBindingList;

        Machine currentMachine = null;

        public EditECOsForm() {
            InitializeComponent();

            machineTable = db.getMachineTable();
            ecoTable = db.getEcoTable();
            cardLocationPageTable = db.getCardLocationPageTable();
            cardLocationBlockTable = db.getCardLocationBlockTable();
            diagramBlockTable = db.getDiagramBlockTable();
            cardEcoTable = db.getCardEcoTable();
            diagramEcoTagTable = db.getDiagramEcoTagTable();

            //  Set up the pull down list with the machines, and start with
            //  the first machine.

            machineList = machineTable.getAll();

            //  Fill in the machine combo box, and remember which machine
            //  we started out with.

            machineComboBox.DataSource = machineList;

            //  Retrieve the last machine we worked with, and select it, if any.

            string lastMachine = Parms.getParmValue("machine");
            if (lastMachine.Length > 0) {
                currentMachine = machineList.Find(
                    x => x.idMachine.ToString() == lastMachine);
            }

            if (currentMachine == null || currentMachine.idMachine == 0) {
                currentMachine = machineList[0];
            }

            machineComboBox.SelectedItem = currentMachine;

            //  Then fill in the ECO table with entries for that machine.

            populateEcoTable(currentMachine);

        }


        //  Shared method to populate the DataGridView table that the user will
        //  edit.

        private void populateEcoTable(Machine machine) {

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            //  Clear out the existing grid...

            ecosDataGridView.DataSource = null;

            //  Find the ECOs for the specified machine

            ecoList = ecoTable.getWhere("WHERE machine='" +
                machine.idMachine + "'" +
                "ORDER BY eco");

            //  Build a new binding list for the DataGridView control,
            //  and adjust the control size and the parent window size
            //  appropriately.

            ecoBindingList = new BindingList<Eco>(ecoList);
            ecoBindingList.AllowNew = true;
            ecoBindingList.AllowRemove = true;
            ecoBindingList.AllowEdit = true;
            ecosDataGridView.DataSource = ecoBindingList;

            //  Hide columns user does not need to see / should not change

            ecosDataGridView.Columns["idECO"].Visible = false;
            ecosDataGridView.Columns["machine"].Visible = false;
            ecosDataGridView.Columns["modified"].Visible = false;

            //  Make title row upper case.
            ecosDataGridView.Columns["eco"].HeaderText = "ECO";

            //  Set the name column to a reasonable length, too.

            ecosDataGridView.Columns["eco"].Width = 10 * 8;

            //  Finally, throw away any entries we may have been saving for deletion...

            deletedEcoList = new List<Eco>();
        }

        private void machineComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            currentMachine = machineList[machineComboBox.SelectedIndex];
            populateEcoTable(currentMachine);
        }

        private void ecosDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {

            //  If this is the title row, ignore it...
            if (e.RowIndex < 0) {
                return;
            }

            //  Mark the edited row as changed.

            Eco changedEco =
                (Eco)ecosDataGridView.Rows[e.RowIndex].DataBoundItem;
            changedEco.modified = true;
        }

        private void ecosDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {

            //  Clear out the error text.

            DataGridViewRow row =
                ecosDataGridView.Rows[e.RowIndex];
            row.ErrorText = "";

            //  If this is on a new row, ignore it.  Otherwise the user can end
            //  up in a "Catch-22" in cases where they didn't really mean to add
            //  a row, such as by hitting tab after editing the last existing row.

            if(ecosDataGridView.Rows[e.RowIndex].IsNewRow) {
                return;
            }

            string columnName = ecosDataGridView.Columns[e.ColumnIndex].Name;
            if (String.Compare(columnName, "eco") == 0) {
                if (e.FormattedValue == null || e.FormattedValue.ToString().Length == 0 ||
                    e.FormattedValue.ToString().Length > 20) {
                    row.ErrorText = columnName + " is required, and " +
                        "has a max length of 20 characters.";
                    e.Cancel = true;
                }
            }

            if (String.Compare(columnName, "description") == 0) {
                if (e.FormattedValue == null &&
                    e.FormattedValue.ToString().Length > 255) {
                    row.ErrorText = columnName + 
                        " has a max length of 255 characters.";
                    e.Cancel = true;
                }
            }

        }

        private void ecosDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {

            Eco changedEco = (Eco)e.Row.DataBoundItem;
            string referencingTables = "";

            //  If the ID on this row is not filled in, then we can assume
            //  it was a new row that the user decided not to add.

            if (changedEco.idECO == 0) {
                return;
            }

            //  So, the users wants to delete an existing row.  First, we need to make
            //  sure that there are not any references to it.

            List<Cardlocationpage> cardLocationPageList;
            List<Cardlocationblock> cardLocationBlockList;
            List<Cardeco> cardEcoList;
            List<Diagramecotag> diagramEcoTagList;

            string whereClause = "WHERE eco='" + changedEco.idECO + "'";
            cardLocationPageList = cardLocationPageTable.getWhere(
                whereClause + " OR previousECO='" + changedEco.idECO + "'");
            cardLocationBlockList = cardLocationBlockTable.getWhere(
                "WHERE diagramECO='" + changedEco.idECO + "'");
            cardEcoList = cardEcoTable.getWhere(whereClause);
            diagramEcoTagList = diagramEcoTagTable.getWhere(whereClause);

            if (cardLocationPageList.Count > 0) {
                referencingTables += "cardLocationPage, ";
            }
            if(cardLocationBlockList.Count > 0) {
                referencingTables += "cardLocationBlock, ";
            }
            if (cardEcoList.Count > 0) {
                referencingTables += "cardECO, ";
            }
            if(diagramEcoTagList.Count > 0) {
                referencingTables += "diagramECOTag, ";
            }

            if(referencingTables.Length > 0) {
                MessageBox.Show("ERROR: This entry is referenced " +
                    "by other entries in " + referencingTables + 
                    "and cannot be removed.",
                    "ECO entry referenced by other entries",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
            else {
                deletedEcoList.Add(changedEco);
            }
        }

        private void applyButton_Click(object sender, EventArgs e) {

            string message = "";
            bool areChanges = false;

            //  Run through the deleted ECO list.

            foreach (Eco eco in deletedEcoList) {//
                message += "Deleting ECO " + eco.eco +
                    " (Database ID: " + eco.idECO + ")\n";
                areChanges = true;
            }

            //  Run through the list looking for adds and changes.

            foreach (Eco eco in ecoList) {
                if (eco.idECO == 0) {
                    message += "Adding ECO " + eco.eco + "\n";
                    areChanges = true;
                }
                else if (eco.modified) {
                    message += "Changing ECO Database ID: " + eco.idECO +
                        " ECO Name: " + eco.eco + "\n";
                    areChanges = true;
                }
            }

            //  If there are not any changes, tell user and quit.

            if (!areChanges) {
                MessageBox.Show("No ECO changes were completed",
                    "No ECOs updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult status = MessageBox.Show("Confirm that you wish to make " +
                "the following changes to ECOs for machine " +
                currentMachine.name + ":\n\n" + message,
                "Confirm ECO changes",
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

                message = "ECOs Updated:\n\n";

                //  First, we do the deletes (in case they delete and then
                //  re-add a ECO with the same name.)

                foreach (Eco eco in deletedEcoList) {
                    ecoTable.deleteByKey(eco.idECO);
                    message += "ECO " + eco.eco + " removed.\n";
                }

                //  Next we do the adds and changes...

                foreach (Eco eco in ecoList) {
                    if (eco.idECO == 0) {
                        eco.idECO = IdCounter.incrementCounter();
                        eco.machine = currentMachine.idMachine;
                        ecoTable.insert(eco);
                        message += "ECO " + eco.eco + " added.  ID=" +
                            eco.idECO + "\n";
                    }
                    else if (eco.modified) {
                        ecoTable.update(eco);
                        message += "ECO " + eco.eco + " updated.  ID=" +
                            eco.idECO + "\n";
                    }
                }

                //  Remember the last machine we worked with, for use in later dialogs

                Parms.setParmValue("machine", currentMachine.idMachine.ToString());

                db.CommitTransaction();

                MessageBox.Show(message, "ECO(s) updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e) {

            //  On cancel, just close the form and return...

            this.Close();
        }
    }
}
