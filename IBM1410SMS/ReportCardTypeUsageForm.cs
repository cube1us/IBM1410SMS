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
    public partial class ReportCardTypeUsageForm : Form
    {

        //  Class to hold the data in a single report line.

        class cardTypeUsageEntry
        {
            public string cardType { get; set; }
            public string sheet { get; set; }
            public string coordinate { get; set; }
            public string inputLevel { get; set; }
            public string outputLevel { get; set; }
            public string pinsIn { get; set; }
            public string pinsOut { get; set; }
        }

        DBSetup db = DBSetup.Instance;

        Table<Cardtype> cardTypeTable;
        Table<Diagramblock> diagramBlockTable;
        Table<Logiclevels> logicLevelsTable;
        Table<Connection> connectionTable;
        Table<Cardgate> cardGateTable;

        List<Cardtype> cardTypeList;
        List<Diagramblock> diagramBlockList;

        Cardtype currentCardType = null;

        public ReportCardTypeUsageForm()
        {
            InitializeComponent();

            cardTypeTable = db.getCardTypeTable();
            diagramBlockTable = db.getDiagramBlockTable();
            logicLevelsTable = db.getLogicLevelsTable();
            connectionTable = db.getConnectionTable();
            cardGateTable = db.getCardGateTable();

            //  Populate the Polarity Combo Box.

            polarityComboBox.Items.Add("*");
            polarityComboBox.Items.Add("+");
            polarityComboBox.Items.Add("-");
            polarityComboBox.SelectedIndex = 0;

            //  Clear out any old entries.

            cardTypeComboBox.Items.Clear();

            //  Get the list of card types

            cardTypeList = cardTypeTable.getWhere(
                "WHERE idCardType != '0' ORDER BY type");

            Cardtype anyCard = new Cardtype();
            anyCard.idCardType = 0;
            anyCard.name = "ANY TYPE";
            anyCard.type = "ALL";
            cardTypeList.Insert(0, anyCard);

            foreach (Cardtype ct in cardTypeList) {
                cardTypeComboBox.Items.Add(ct.type);
            }
        }

        private void cardTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            if(cardTypeComboBox.SelectedIndex < 0) {
                return;
            }

            currentCardType = cardTypeList[cardTypeComboBox.SelectedIndex];

            // (Re) populate the Card Gate List.

            List<Cardgate> gateList = cardGateTable.getWhere("WHERE cardtype = '" +
                currentCardType.idCardType + "' ORDER BY cardgate.number");
            cardGateComboBox.Items.Clear();
            foreach(Cardgate gate in gateList) {
                cardGateComboBox.Items.Add(gate);
            }

        }

        private void reportButton_Click(object sender, EventArgs e) {

            cardTypeUsageEntry usageEntry;
            Cardgate currentGate = null;

            List<cardTypeUsageEntry> cardTypeUsageList;
            List<Connection> connectionList;
            List<string> pinList;

            if(cardTypeComboBox.SelectedIndex < 0) {
                return;
            }

            reportButton.Enabled = false;

            //  See if the user wants to restrict it by gate number

            if(cardGateComboBox.SelectedIndex >= 0) {
                currentGate = (Cardgate) cardGateComboBox.SelectedItem;
            }


            cardTypeUsageList = new List<cardTypeUsageEntry>();
            currentCardType = cardTypeList[cardTypeComboBox.SelectedIndex];

            //  First, clear out any existing information

            cardTypeUsageDataGridView.DataSource = null;
            cardTypeUsageDataGridView.Rows.Clear();
            cardTypeUsageDataGridView.Refresh();

            //  Then find all of the diagram blocks that refer to this card type.
            //  The first entry in the list (ANY TYPE) means ignore the type.

            diagramBlockList = diagramBlockTable.getWhere(
                currentCardType.idCardType == 0 ?
                "" : "WHERE cardType='" + currentCardType.idCardType + "'");

            foreach (Diagramblock db in diagramBlockList) {

                usageEntry = new cardTypeUsageEntry();
                usageEntry.cardType = Helpers.getCardTypeType(db.cardType);
                usageEntry.sheet = Helpers.getDiagramPageName(db.diagramPage);
                usageEntry.coordinate = db.diagramColumn + db.diagramRow.ToString();
                usageEntry.inputLevel = logicLevelsTable.getByKey(db.inputMode).logicLevel;
                usageEntry.outputLevel = logicLevelsTable.getByKey(db.outputMode).logicLevel;

                string currentPolarity = (string) polarityComboBox.SelectedItem;
                bool polaritySelected =
                    currentPolarity == "*" ? true : false;

                //  Filter by card gate, if selected.

                if(currentGate != null) {
                    if(db.cardGate != currentGate.idcardGate) {
                        continue;
                    }
                }

                //  Filter by symbol, if enteried

                if(symbolTextBox.Text != null && symbolTextBox.Text.Length > 0 &&
                    symbolTextBox.Text != db.symbol) {
                    continue;
                }

                //  Output pins FROM the block

                usageEntry.pinsOut = "";
                pinList = new List<string>();
                connectionList = connectionTable.getWhere(
                    "WHERE fromDiagramBlock='" + db.idDiagramBlock + "'");
                foreach (Connection c in connectionList) {
                    if (c.fromPin != null && c.fromPin.Length > 0 && !pinList.Contains(c.fromPin)) {
                        pinList.Add(c.fromPin);
                    }
                    if (c.fromLoadPin != null && c.fromLoadPin.Length > 0 &&
                            !pinList.Contains(c.fromLoadPin)) {
                        pinList.Add(c.fromLoadPin);
                    }
                    if(!polaritySelected && 
                        (c.fromPhasePolarity != null && 
                        c.fromPhasePolarity == currentPolarity)) {
                        polaritySelected = true;
                    }
                }
                pinList.Sort();
                foreach (string s in pinList) {
                    if (usageEntry.pinsOut.Length > 0) {
                        usageEntry.pinsOut += ",";
                    }
                    usageEntry.pinsOut += s;
                }

                //  Filter by output polarity, if selected

                if(!polaritySelected) {
                    continue;
                }

                //  Input pins TO the block

                usageEntry.pinsIn = "";
                pinList = new List<string>();
                connectionList = connectionTable.getWhere(
                    "WHERE toDiagramBlock='" + db.idDiagramBlock + "'");
                foreach (Connection c in connectionList) {
                    if (c.toPin != null && c.toPin.Length > 0 && !pinList.Contains(c.toPin)) {
                        pinList.Add(c.toPin);
                    }
                }
                pinList.Sort();
                foreach (string s in pinList) {
                    if (usageEntry.pinsIn.Length > 0) {
                        usageEntry.pinsIn += ",";
                    }
                    usageEntry.pinsIn += s;
                }

                cardTypeUsageList.Add(usageEntry);
            }

            if (cardTypeUsageList.Count <= 0) {
                MessageBox.Show("There were no references to card type " +
                    currentCardType.type, "No references found",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                reportButton.Enabled = true;
                return;
            }

            //  Sort by coordinate within sheet.

            cardTypeUsageList.Sort(delegate (cardTypeUsageEntry x, cardTypeUsageEntry y)
            {
                if (x.sheet != y.sheet) return x.sheet.CompareTo(y.sheet);
                else return x.coordinate.CompareTo(y.coordinate);
            });

            cardTypeUsageDataGridView.DataSource = cardTypeUsageList;
            cardTypeUsageDataGridView.Columns["cardType"].HeaderText = "Type";
            cardTypeUsageDataGridView.Columns["sheet"].HeaderText = "Sheet";
            cardTypeUsageDataGridView.Columns["coordinate"].HeaderText = "Coord.";
            cardTypeUsageDataGridView.Columns["inputLevel"].HeaderText = "In Lvl";
            cardTypeUsageDataGridView.Columns["outputLevel"].HeaderText = "Out Lvl";
            cardTypeUsageDataGridView.Columns["pinsIn"].HeaderText = "In Pins";
            cardTypeUsageDataGridView.Columns["pinsOut"].HeaderText = "Out";

            cardTypeUsageDataGridView.Columns["cardType"].Width = 5 * 8;
            cardTypeUsageDataGridView.Columns["sheet"].Width = 10 * 8;
            cardTypeUsageDataGridView.Columns["coordinate"].Width = 7 * 8;
            cardTypeUsageDataGridView.Columns["inputLevel"].Width = 4 * 8;
            cardTypeUsageDataGridView.Columns["outputLevel"].Width = 4 * 8;
            cardTypeUsageDataGridView.Columns["pinsIn"].Width = 12 * 8;
            cardTypeUsageDataGridView.Columns["pinsOut"].Width = 12 * 8;

            reportButton.Enabled = true;
        }
    }
}
