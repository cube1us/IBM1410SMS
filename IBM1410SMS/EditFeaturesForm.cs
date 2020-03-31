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
    public partial class EditFeaturesForm : Form
    {
        DBSetup db = DBSetup.Instance;
        Table<Feature> featureTable;
        Table<Machine> machineTable;
        Table<Diagramblock> diagramBlockTable;
        Table<Cardlocation> cardLocationTable;
        List<Feature> featureList;
        List<Feature> deletedFeatureList;
        List<Machine> machineList;
        BindingList<Feature> featureBindingList;
        Machine currentMachine = null;

        public EditFeaturesForm() {
            InitializeComponent();

            //  Set up the pull down list with the machines, and start with
            //  the first machine.

            machineTable = db.getMachineTable();
            machineList = machineTable.getAll();
            featureTable = db.getFeatureTable();
            diagramBlockTable = db.getDiagramBlockTable();
            cardLocationTable = db.getCardLocationTable();

            machineComboBox.DataSource = machineList;

            //  Retrieve the last machine we worked with, and select it, if any.

            string lastMachine = Parms.getParmValue("machine");
            if(lastMachine.Length > 0) {
                currentMachine = machineList.Find(
                    x => x.idMachine.ToString().Equals(lastMachine));
            }

            if(currentMachine == null || currentMachine.idMachine == 0) {
                currentMachine = machineList[0];
            }

            machineComboBox.SelectedItem = currentMachine;

            //  Then fill the feature table with entries for that machine

            populateFeatureTable(currentMachine);

        }

        //  Shared method to populate the DataGridView table the user uses to edit.

        private void populateFeatureTable(Machine machine) {

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            //  Clear out the existing grid...

            featuresDataGridView.DataSource = null;

            //  Find the features for the specified machine

            featureList = featureTable.getWhere("WHERE machine='" +
                currentMachine.idMachine + "'");

            //  Build a new binding list for the DataGridView control,
            //  and adjust the control size and the parent window size
            //  appropriately.

            featureBindingList = new BindingList<Feature>(featureList);
            featureBindingList.AllowNew = true;
            featureBindingList.AllowRemove = true;
            featureBindingList.AllowEdit = true;
            featuresDataGridView.DataSource = featureBindingList;

            //  Hide columns user does not need to see / should not change

            featuresDataGridView.Columns["idFeature"].Visible = false;
            featuresDataGridView.Columns["machine"].Visible = false;
            featuresDataGridView.Columns["modified"].Visible = false;

            //  Make title row upper case.
            featuresDataGridView.Columns["code"].HeaderText =
                textInfo.ToTitleCase(featuresDataGridView.Columns["code"].HeaderText);
            featuresDataGridView.Columns["feature"].HeaderText =
                textInfo.ToTitleCase(featuresDataGridView.Columns["feature"].HeaderText);

            //  Set the width of smaller columns

            featuresDataGridView.Columns["code"].Width = 10 * 8;

            //  Finally, throw away any entries we may have been saving for deletion...

            deletedFeatureList = new List<Feature>();

        }

        private void machineComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            currentMachine = machineList[machineComboBox.SelectedIndex];
            populateFeatureTable(currentMachine);
        }


        private void applyButton_Click(object sender, EventArgs e) {

            string message = "";
            bool areChanges = false;

            //  Run through the deleted feature list.

            foreach(Feature f in deletedFeatureList) {
                message += "Deleting Feature " + f.feature + ", Code: " + f.code +
                    " (Database ID: " + f.idFeature + ")\n";
                areChanges = true;
            }
            
            //  Run through the list looking for adds and changes.

            foreach(Feature f in featureList) {
                if(f.idFeature == 0) {
                    message += "Adding Feature " + f.feature + ", Code: " + f.code + "\n";
                    areChanges = true;
                }
                else if (f.modified) {
                    message += "Changing Feature Database ID: " + f.idFeature +
                        " to Feature " + f.feature + ", Code: " + f.code + "\n";
                    areChanges = true;
                }
            }

            //  If there are not any changes, tell user and quit.

            if(!areChanges) {
                MessageBox.Show("No feature changes were completed", 
                    "No Features updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult status = MessageBox.Show("Confirm that you wish to make " +
                "the following changes to features for machine " +
                currentMachine.name + ":\n\n" + message,
                "Confirm Feature changes",
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

                message = "Features Updated:\n\n";

                db.BeginTransaction();

                //  First, we do the deletes (in case they delete and then
                //  re-add a feature with the same name.)

                foreach (Feature f in deletedFeatureList) {
                    featureTable.deleteByKey(f.idFeature);
                    message += "Feature " + f.feature + " removed.\n";
                }

                //  Next we do the adds and changes...

                foreach (Feature f in featureList) {
                    if (f.idFeature == 0) {
                        f.idFeature = IdCounter.incrementCounter();
                        f.machine = currentMachine.idMachine;
                        featureTable.insert(f);
                        message += "Feature " + f.feature + " added.  ID=" +
                            f.idFeature + "\n";
                    }
                    else if (f.modified) {
                        featureTable.update(f);
                        message += "Feature " + f.feature + " updated.  ID=" +
                            f.idFeature + "\n";
                    }
                }

                Parms.setParmValue("machine", currentMachine.idMachine.ToString());

                db.CommitTransaction();

                MessageBox.Show(message,"Features updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
        }

        private void featuresDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {

            //  If this is the title row, ignore it
            if(e.RowIndex < 0) {
                return;
            }

            //  Mark the edited row as changed.

            Feature changedFeature = 
                (Feature) featuresDataGridView.Rows[e.RowIndex].DataBoundItem;
            changedFeature.modified = true;
        }

        private void featuresDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {

            Feature changedFeature = (Feature)e.Row.DataBoundItem;

            //  If the ID on this row is not filled in, then we can assume
            //  it was a new row that the user decided not to add.

            if(changedFeature.idFeature == 0) {
                return;
            }

            //  So, the users wants to delete an existing row.  First, we need to make
            //  sure that there are not any references to it.

            List<Diagramblock> diagramBlockList;
            List<Cardlocation> cardLocationList;

            string referencingTables = "Tables: ";
            Boolean referenced = false;
            string whereClause = "WHERE feature='" + changedFeature.idFeature + "'";
            diagramBlockList = diagramBlockTable.getWhere(whereClause);
            cardLocationList = cardLocationTable.getWhere(whereClause);

            if (diagramBlockList.Count > 0) {
                referencingTables += "diagramBlock, ";
                referenced = true;
            }
            if (cardLocationList.Count > 0) {
                referencingTables += "cardLocation, ";
                referenced = true;
            }

            //  If there were references, cancel this entry.
            //  Otherwise, add it to the list of entries that we may delete later.

            if (referenced) {
                MessageBox.Show("ERROR: Feature " + changedFeature.feature +
                    ", Code: " + changedFeature.code +
                    ", database ID: " + changedFeature.idFeature + " is referenced" +
                    " by other entries in " + referencingTables + " and cannot be removed.",
                    "Feature entry referenced by other entries",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
            else {
                deletedFeatureList.Add(changedFeature);
            }
        }

        //  On cancel, just close the form, without committing ANY changes.

        private void cancelButton_Click(object sender, EventArgs e) {
            this.Close();
        }

        //  Validate values, mostly for length...

        private void featuresDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {

            //  Clear out the error text.

            DataGridViewRow row =
                featuresDataGridView.Rows[e.RowIndex];
            row.ErrorText = "";

            //  If this is on a new row, ignore it.  Otherwise the user can end
            //  up in a "Catch-22" in cases where they didn't really mean to add
            //  a row, such as by hitting tab after editing the last existing row.

            if (featuresDataGridView.Rows[e.RowIndex].IsNewRow) {
                return;
            }


            string columnName = featuresDataGridView.Columns[e.ColumnIndex].Name;
            if(String.Compare(columnName,"code") == 0) {
                if(e.FormattedValue == null || e.FormattedValue.ToString().Length  == 0 ||
                    e.FormattedValue.ToString().Length > 8) {
                    row.ErrorText = columnName + " is required, and " +
                        "has a max length of 8 characters.";
                    e.Cancel = true;
                }
            }
            else if (String.Compare(columnName, "feature") == 0) {
                if (e.FormattedValue == null || e.FormattedValue.ToString().Length == 0 ||
                    e.FormattedValue.ToString().Length > 1023) {
                    row.ErrorText = columnName + " is required, and " + 
                        "has a max length of 1023 characters.";
                    e.Cancel = true;
                }
            }

        }
    }
}
