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
    public partial class EditLogicLevelsForm : Form
    {
        DBSetup db = DBSetup.Instance;
        Table<Logiclevels> logicLevelsTable;

        List<Logiclevels> logicLevelsList;
        List<Logiclevels> deletedLogicLevelsList;
        BindingList<Logiclevels> logicLevelsBindingList;

        public EditLogicLevelsForm() {
            InitializeComponent();

            logicLevelsTable = db.getLogicLevelsTable();

            //  This form has no pull down lists.

            populateLogicLevelsTable();

        }


        private void populateLogicLevelsTable() {

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            //  Clear out the existing grid...

            logicLevelsDataGridView.DataSource = null;

            //  Get a list of all of the existing IBM logic functions.

            logicLevelsList = logicLevelsTable.getAll();

            //  Build a binding list from that list...

            logicLevelsBindingList =
                new BindingList<Logiclevels>(logicLevelsList);
            logicLevelsBindingList.AllowNew = true;
            logicLevelsBindingList.AllowRemove = true;
            logicLevelsBindingList.AllowEdit = true;
            logicLevelsDataGridView.DataSource = logicLevelsBindingList;

            //  Hide columns that the user does not need to see / should not change.

            logicLevelsDataGridView.Columns["idLogicLevels"].Visible = false;
            logicLevelsDataGridView.Columns["modified"].Visible = false;

            //  Make the title row upper case.

            logicLevelsDataGridView.Columns["logicLevel"].HeaderText =
                "Logic Level Name";
            logicLevelsDataGridView.Columns["logicZeroTenths"].HeaderText =
                "Zero (Tenths/V)";
            logicLevelsDataGridView.Columns["logicOneTenths"].HeaderText =
                "One (Tenths/V)";
            logicLevelsDataGridView.Columns["circuitType"].HeaderText =
                "Circuit Type";
            logicLevelsDataGridView.Columns["dotFunctionLogic"].HeaderText =
                "Logic for DOT Functions";



            //  Set the column widths

            logicLevelsDataGridView.Columns["logicLevel"].Width = 10 * 5;
            logicLevelsDataGridView.Columns["logicZeroTenths"].Width = 10 * 7;
            logicLevelsDataGridView.Columns["logicOneTenths"].Width = 10 * 7;
            logicLevelsDataGridView.Columns["circuitType"].Width = 10 * 7;
            logicLevelsDataGridView.Columns["dotFunctionLogic"].Width = 23 * 7;


            //  Finally, throw away any deleted entries...

            deletedLogicLevelsList = new List<Logiclevels>();

        }

        private void logicLevelsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {

            //  If this is the title row, ignore it...
            if (e.RowIndex < 0) {
                return;
            }

            //  Mark the edited row as changed, if it exists.

            Logiclevels changedLogicLevel =
                (Logiclevels)logicLevelsDataGridView.Rows[e.RowIndex].DataBoundItem;
            changedLogicLevel.modified = true;

        }

        private void logicLevelsDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {

            //  Clear out the error text...

            DataGridViewRow row =
                logicLevelsDataGridView.Rows[e.RowIndex];
            row.ErrorText = "";

            //  If this is a new row, don't try to validate it...

            if (logicLevelsDataGridView.Rows[e.RowIndex].IsNewRow) {
                return;
            }

            string columnName = logicLevelsDataGridView.Columns[e.ColumnIndex].Name;

            if (string.Compare(columnName, "logicLevel") == 0) {
                if (e.FormattedValue == null || e.FormattedValue.ToString().Length == 0 ||
                    e.FormattedValue.ToString().Length > 2) {
                    row.ErrorText = "Logic Level (name) is required, and has a max " +
                        "length of 2 characters.";
                    e.Cancel = true;
                }
            }

            //  These columns must be integers, -999 to +999
            if (string.Compare(columnName, "logicZeroTenths") == 0 ||
                string.Compare(columnName, "logicOneTenths") == 0) {
                int v = -9999;
                if (!Int32.TryParse(e.FormattedValue.ToString(), out v) ||
                    v < -999 || v > 999) {
                    row.ErrorText = columnName +
                        " is required, and must be an integer -999 - +999 (tenths/V)";
                    e.Cancel = true;
                }
            }

            if (String.Compare(columnName, "circuitType") == 0) {
                if (e.FormattedValue == null || e.FormattedValue.ToString().Length == 0 ||
                    e.FormattedValue.ToString().Length > 16) {
                    row.ErrorText = columnName +
                        " is required, and has a max length of 16.";
                    e.Cancel = true;
                }
            }

            if(String.Compare(columnName,"dotFunctionLogic") == 0) {
                if (e.FormattedValue.ToString().Length > 8) {
                    row.ErrorText = columnName +
                        " has a max length of 8.";
                    e.Cancel = true;
                }

            }
        }

        private void logicLevelsDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {

            string referringTables = "";

            Logiclevels changedLogicLevels =
                (Logiclevels)e.Row.DataBoundItem;

            //  If the ID of this row is not filled in, then we can assume
            //  it was a enw row that the user decided not toa dd.

            if (changedLogicLevels.idLogicLevels == 0) {
                return;
            }

            //  So, the users wants to delete an existing row.  First, we need to make
            //  sure that there are not any references to it.

            List<Cardgate> cardGateList;
            List<Diagramblock> diagramBlockList;
            Table<Cardgate> cardGateTable = db.getCardGateTable();
            Table<Diagramblock> diagramBlockTable = db.getDiagramBlockTable();

            cardGateList = cardGateTable.getWhere(
                "WHERE inputLevel='" +
                changedLogicLevels.idLogicLevels +
                "' OR outputLevel='" +
                changedLogicLevels.idLogicLevels + "'");

            if (cardGateList.Count > 0) {
                referringTables += "Card Gate, ";
            }

            diagramBlockList = diagramBlockTable.getWhere(
                "WHERE inputMode='" +
                changedLogicLevels.idLogicLevels +
                "' OR outputMode='" +
                changedLogicLevels.idLogicLevels + "'");

            if (diagramBlockList.Count > 0) {
                referringTables += "Diagram Block, ";
            }


            if (referringTables.Length > 0) {
                MessageBox.Show("ERROR: This Logic Level is referenced " +
                    "by one or more entries in Table(s) " +
                    referringTables +
                    "and cannot be removed.",
                    "Logic Level entry referenced by other tables",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
            else {
                deletedLogicLevelsList.Add(changedLogicLevels);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e) {

            //  On cancel, just close the form.

            this.Close();

        }

        private void applyButton_Click(object sender, EventArgs e) {
            string message = "";
            bool areChanges = false;

            //  Run through the deleted logic function list.

            foreach (Logiclevels lv in deletedLogicLevelsList) {
                message += "Deleting Logic Level " + lv.logicLevel +
                    " (Database ID: " + lv.idLogicLevels + ")\n";
                areChanges = true;
            }

            //  Next run through the list looking for adds and changes...

            foreach (Logiclevels lv in logicLevelsList) {
                if (lv.idLogicLevels == 0) {
                    message += "Adding Logic Level " + lv.logicLevel + "\n";
                    areChanges = true;
                }
                else if (lv.modified) {
                    message += "Changing Logic Level w/Database ID: " +
                        lv.idLogicLevels + " named " + lv.logicLevel + "\n";
                    areChanges = true;
                }
            }

            //  If there are no changes, tell the user and quit.

            if (!areChanges) {
                MessageBox.Show("No Logic Level changes were completed",
                    "No IBM Logic Levels updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult status = MessageBox.Show("Confirm that you wish to make " +
                "the following changes to the list of Logic Levels:\n\n" +
                message,
                "Confirm IBM Logic Level changes",
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

                message = "IBM Logic Levels Updated:\n\n";

                db.BeginTransaction();

                //  First we do the deletes (in case they delete and then re-add
                //  the IBM Logic Function with the same label...

                foreach (Logiclevels lv in deletedLogicLevelsList) {
                    logicLevelsTable.deleteByKey(lv.idLogicLevels);
                    message += "Logic Level " + lv.logicLevel + " removed.\n";
                }

                //  Next, we do the adds and changes...

                foreach (Logiclevels lv in logicLevelsList) {

                    //  If there is no existing ID, this must be an Add
                    if (lv.idLogicLevels == 0) {
                        lv.idLogicLevels = IdCounter.incrementCounter();
                        logicLevelsTable.insert(lv);
                        message += "Logic Level " + lv.logicLevel +
                            " added.   ID=" + lv.idLogicLevels + "\n";
                    }
                    else if (lv.modified) {
                        logicLevelsTable.update(lv);
                        message += "Logic Levels " + lv.logicLevel +
                            " updated.  ID=" + lv.idLogicLevels + "\n";
                    }
                }

                db.CommitTransaction();

                MessageBox.Show(message, "IBM Logic Levels updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                //  And close the form....

                this.Close();

            }
        }
    }
}
