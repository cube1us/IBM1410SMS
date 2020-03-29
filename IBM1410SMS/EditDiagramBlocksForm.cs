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
    public partial class EditDiagramBlocksForm : Form
    {
        DBSetup db = DBSetup.Instance;

        Table<Page> pageTable;
        Table<Diagramblock> diagramBlockTable;
        Table<Dotfunction> dotFunctionTable;
        Table<Diagramecotag> diagramECOtagTable;
        Table<Cardlocationblock> cardLocationBlockTable;
        Table<Cardlocation> cardLocationTable;
        Table<Cardgate> cardGateTable;
        Table<Connection> connectionTable;

        Page currentPage;
        Diagrampage currentDiagramPage;
        Machine currentMachine;
        Volumeset currentVolumeSet;
        Volume currentVolume;
        string machinePrefix;

        List<Diagramblock> diagramBlockList;
        List<Dotfunction> dotFunctionList;
        List<Diagramecotag> diagramECOTagList;
        List<Cardlocationblock> cardLocationBlockList;
        List<Cardlocation> cardLocationList;

        Size logicBlockButtonSize = new Size(54, 60);
        Size dotFunctionButtonSize = new Size(14, 20);
        Color transparentColor = Color.FromArgb(0, 255, 255, 255);
        Color lightColor = Color.FromArgb(0, 32, 32, 32);

        public EditDiagramBlocksForm(Machine machine,
            Volumeset volumeSet,
            Volume volume,
            Diagrampage diagramPage) {
            InitializeComponent();

            pageTable = db.getPageTable();
            diagramBlockTable = db.getDiagramBlockTable();
            dotFunctionTable = db.getDotFunctionTable();
            diagramECOtagTable = db.getDiagramEcoTagTable();
            cardLocationTable = db.getCardLocationTable();
            cardLocationBlockTable = db.getCardLocationBlockTable();
            cardGateTable = db.getCardGateTable();
            connectionTable = db.getConnectionTable();

            //  Fill in constant data.

            currentMachine = machine;
            currentVolumeSet = volumeSet;
            currentVolume = volume;
            currentPage = pageTable.getByKey(diagramPage.page);
            currentDiagramPage = diagramPage;
            machinePrefix = currentMachine.name.Length >= 4 ?
                currentMachine.name.Substring(0, 2) : "";

            populateDialog();
        }

        private void populateDialog() {

            //  Get lists of the logic blocks, dot functions
            //  and ECO tags on the diagram.

            diagramBlockList = diagramBlockTable.getWhere(
                "WHERE diagramPage='" + currentDiagramPage.idDiagramPage + "'" +
                " ORDER BY diagramRow, diagramColumn");

            dotFunctionList = dotFunctionTable.getWhere(
                "WHERE diagramPage='" + currentDiagramPage.idDiagramPage + "'" +
                " ORDER by diagramRowTop, diagramColumnToLeft");

            diagramECOTagList = diagramECOtagTable.getWhere(
                "WHERE diagramPage='" + currentDiagramPage.idDiagramPage + "'" +
                " ORDER by diagramecotag.name");

            //  Get the card locations and card location blocks that are
            //  associated with this page, so we can use them to pre-populate
            //  data.

            cardLocationBlockList = cardLocationBlockTable.getWhere(
                "WHERE diagramPage='" + currentDiagramPage.idDiagramPage + "'" +
                " AND cardlocationblock.ignore='0'" +
                " ORDER BY diagramRow, diagramColumn");

            cardLocationList = cardLocationTable.getWhere(
                "WHERE cardlocation.page='" + currentPage.idPage + "'" +
                " AND crossedOut=0");

            //  Populate the read only data.

            machineTextBox.ReadOnly = true;
            machineTextBox.Text = currentMachine.name;
            volumeTextBox.ReadOnly = true;
            volumeTextBox.Text = currentVolumeSet.machineType + "/" +
                currentVolumeSet.machineSerial + " Volume: " +
                currentVolume.name;
            pageTextBox.ReadOnly = true;
            pageTextBox.Text = currentPage.name;

            //  Set the sizes of the control table such that it gets
            //  scroll bars...

            tableLayoutPanel1.MaximumSize = new Size(
                tableLayoutPanel1.Width, 700);
            tableLayoutPanel1.AutoScroll = true;

            //  Fill in the control table with buttons for logic blocks
            //  and for dot functions.

            //  Clear out any existing controls

            tableLayoutPanel1.Controls.Clear();

            //  Populate the logic block and dot function buttons and labels

            for (int row = 0; row < tableLayoutPanel1.RowCount; ++row) {
                for (int col = 0; col < tableLayoutPanel1.ColumnCount; ++col) {

                    //  Row headers

                    if (row == 0 || row == tableLayoutPanel1.RowCount - 1) {
                        if (col != tableLayoutPanel1.ColumnCount - 1 && col % 2 == 1) {
                            Label label = new Label();
                            label.Anchor = AnchorStyles.None;
                            label.Text = (Helpers.maxDiagramColumn - col / 2).ToString();
                            label.TextAlign = ContentAlignment.MiddleCenter;
                            label.Font = new Font(label.Font, FontStyle.Bold);
                            tableLayoutPanel1.Controls.Add(label, col, row);
                        }
                        continue;
                    }

                    //  Column headers

                    if (col == 0 || col == tableLayoutPanel1.ColumnCount - 1) {
                        Label label = new Label();
                        label.Anchor = AnchorStyles.None;
                        label.Text = Helpers.validDiagramRows[row - 1];
                        label.TextAlign = ContentAlignment.MiddleCenter;
                        label.Font = new Font(label.Font, FontStyle.Bold);
                        tableLayoutPanel1.Controls.Add(label, col, row);
                        continue;
                    }

                    string rowName = Helpers.validDiagramRows[row - 1];

                    //  Odd columns get a big (logic block) button

                    if (col % 2 == 1) {
                        Button button = new Button();
                        button.Anchor = AnchorStyles.None;
                        button.Size = logicBlockButtonSize;
                        button.Text = "";
                        button.Font = new Font(button.Font.Name, button.Font.SizeInPoints - 2,
                            button.Font.Style);
                        button.Click += new EventHandler(logicBlockButtonClick);
                        button.Paint += new PaintEventHandler(logicBlockButtonPaint);
                        tableLayoutPanel1.Controls.Add(button, col, row);

                        int columnNumber = Helpers.maxDiagramColumn - (col / 2);
                        string ecoTagLetter = ".";
                        int inputConnections = 0;
                        int outputConnections = 0;
                        string gateConnectionsString = "";

                        //  See if we have a logic block for this button.
                        //  If so, populate the button with the available information.

                        Diagramblock diagramBlock = diagramBlockList.Find(
                            b => b.diagramRow == rowName && b.diagramColumn == columnNumber);

                        if (diagramBlock != null && diagramBlock.idDiagramBlock > 0) {
                            CardSlotInfo cardSlotInfo = Helpers.getCardSlotInfo(
                                diagramBlock.cardSlot);
                            string cardTypeName = Helpers.getCardTypeType(
                                diagramBlock.cardType);

                            if (diagramBlock.eco != 0) {
                                Diagramecotag ecoTag = diagramECOTagList.Find(
                                    t => t.idDiagramECOTag == diagramBlock.eco);
                                if (ecoTag != null && ecoTag.idDiagramECOTag != 0) {
                                    ecoTagLetter = ecoTag.name;
                                }
                                else {
                                    ecoTagLetter = "?";
                                }
                            }

                            if(diagramBlock.cardGate != 0) {
                                Cardgate cardGate = cardGateTable.getByKey(diagramBlock.cardGate);
                                inputConnections = connectionTable.getWhere(
                                    "WHERE toDiagramBlock='" + diagramBlock.idDiagramBlock + "'").Count;
                                outputConnections = connectionTable.getWhere(
                                    "WHERE fromDiagramBlock='" + diagramBlock.idDiagramBlock + "'").Count;
                                gateConnectionsString = Environment.NewLine +
                                    "G" + cardGate.number + ":" + inputConnections.ToString() + "/" +
                                    outputConnections.ToString();
                            }

                            //  Generalize: The following may need work for machines other than 1410

                            button.Text = /* Helpers.getTwoCharMachineName(cardSlotInfo.machineName)  + */
                                currentMachine.aldMachineType +
                                cardSlotInfo.frameName + ecoTagLetter +
                                Environment.NewLine + cardSlotInfo.panelName +
                                cardSlotInfo.row + cardSlotInfo.column.ToString("D2") +
                                Environment.NewLine + cardTypeName + 
                                gateConnectionsString;

                            //  Data coming from the diagrmBlock table is in bold.
                            button.Font = new Font(button.Font, FontStyle.Bold);
                        }

                        //  If it wasn't in the diagram block table, try the card location
                        //  and card location block tables...

                        else {

                            Cardlocationblock clb = cardLocationBlockList.Find(
                                b => b.diagramRow == rowName && b.diagramColumn == columnNumber);

                            if (clb == null || clb.idCardLocationBlock == 0) {
                                continue;
                            }

                            Cardlocation cardLocation = cardLocationTable.getByKey(
                                clb.cardLocation);

                            if(cardLocation.idCardLocation == 0) {
                                continue;
                            }

                            CardSlotInfo cardSlotInfo = Helpers.getCardSlotInfo(
                                cardLocation.cardSlot);
                            string cardTypeName = Helpers.getCardTypeType(
                                cardLocation.type);

                            if (clb.diagramECO != 0) {
                                Diagramecotag ecoTag = diagramECOTagList.Find(
                                    t => t.eco == clb.diagramECO);
                                if (ecoTag != null && ecoTag.idDiagramECOTag != 0) {
                                    ecoTagLetter = ecoTag.name;
                                }
                                else {
                                    ecoTagLetter = "?";
                                }
                            }

                            button.Text = /* Helpers.getTwoCharMachineName(currentMachine.name) + */
                                currentMachine.aldMachineType +
                                cardSlotInfo.frameName + ecoTagLetter +
                                Environment.NewLine + cardSlotInfo.panelName +
                                cardSlotInfo.row + cardSlotInfo.column.ToString("D2") +
                                Environment.NewLine + cardTypeName;

                            //  Data coming from the Card Location Table is NOT bold,
                            //  and is white-ish.
                            button.ForeColor = System.Drawing.Color.SlateGray;
                        }

                        continue;
                    }

                    //  Even, interior, columns get a really small (Dot Function) button

                    if (col % 2 == 0) {

                        int columnNumber = Helpers.maxDiagramColumn - (col / 2) + 1;

                        Dotfunction dotFunction = dotFunctionList.Find(
                            dot => dot.diagramRowTop != null &&
                            dot.diagramRowTop == rowName &&
                            dot.diagramColumnToLeft == columnNumber);

                        Button button = new Button();
                        button.Anchor = AnchorStyles.None;
                        button.Size = dotFunctionButtonSize;
                        if(dotFunction != null && dotFunction.idDotFunction != 0 &&
                            dotFunction.logicFunction.Length > 0) {
                            button.Text = dotFunction.logicFunction;
                        }
                        else {
                            button.Text = "-";
                        }
                        button.FlatStyle = FlatStyle.Flat;
                        button.FlatAppearance.BorderSize = 0;
                        button.FlatAppearance.BorderColor = transparentColor;
                        button.TabStop = false;
                        button.Click += new EventHandler(dotFunctionButtonClick);
                        tableLayoutPanel1.Controls.Add(button, col, row);
                    }
                }
            }

        }

        private void logicBlockButtonClick(object sender, EventArgs e) {

            Button clickedButton = (Button)sender;

            TableLayoutPanelCellPosition cell = tableLayoutPanel1.GetCellPosition(
                clickedButton);

            string rowName = Helpers.validDiagramRows[cell.Row - 1];
            int columnNumber = Helpers.maxDiagramColumn - (cell.Column / 2);

            /*
            Console.WriteLine("Button at cell " + tableLayoutPanel1.GetCellPosition(
                clickedButton) + ", Drawing Row Name: " + rowName +
                ", Drawing Column " + columnNumber);
            */

            Diagramblock diagramBlock = diagramBlockList.Find(
                x => x.diagramRow != null && x.diagramRow == rowName &&
                x.diagramColumn == columnNumber);

            Cardlocation cardLocation = null;
            if(diagramBlock == null || diagramBlock.idDiagramBlock == 0) {
                Cardlocationblock clb = cardLocationBlockList.Find(
                    b => b.diagramRow == rowName && b.diagramColumn == columnNumber);

                if (clb != null && clb.idCardLocationBlock != 0) {
                    cardLocation = cardLocationTable.getByKey(
                        clb.cardLocation);
                }
            }
            
            EditDiagramLogicBlockForm EditDiagramLogicBlockForm = new EditDiagramLogicBlockForm(
                diagramBlock, currentMachine, currentVolumeSet, currentVolume,
                currentDiagramPage, rowName, columnNumber, cardLocation);

            EditDiagramLogicBlockForm.ShowDialog();
            populateDialog();
        }

        private void dotFunctionButtonClick(object sender, EventArgs e) {
            Button clickedButton = (Button)sender;

            TableLayoutPanelCellPosition cell = tableLayoutPanel1.GetCellPosition(
                clickedButton);

            string rowName = Helpers.validDiagramRows[cell.Row - 1];
            int columnNumber = Helpers.maxDiagramColumn - (cell.Column / 2) + 1;
        
            /*
            Console.WriteLine("Button at cell " + tableLayoutPanel1.GetCellPosition(
                clickedButton) + ", Drawing Row Name: " + rowName + 
                ", Drawing Column " + columnNumber);
            */

            Dotfunction dotFunction = dotFunctionList.Find(
                x => x.diagramRowTop != null && x.diagramRowTop == rowName && 
                x.diagramColumnToLeft == columnNumber);

            EditDotFunctionForm EditDotFunctionForm = new EditDotFunctionForm(
                dotFunction, currentMachine, currentVolumeSet, currentVolume,
                currentDiagramPage, rowName, columnNumber);

            EditDotFunctionForm.ShowDialog();

            //  We have to repopulate the dialog with the (perhaps) updated
            //  dot function.

            populateDialog();
        }

        private void logicBlockButtonPaint(object sender, PaintEventArgs e) {
            Button drawingButton = (Button)sender;
            ControlPaint.DrawBorder(e.Graphics, drawingButton.ClientRectangle,
                  SystemColors.ControlLightLight, 3, ButtonBorderStyle.Outset,
                  SystemColors.ControlLightLight, 3, ButtonBorderStyle.Outset,
                  SystemColors.ControlLightLight, 3, ButtonBorderStyle.Outset,
                  SystemColors.ControlLightLight, 3, ButtonBorderStyle.Outset);
        }
    

        private void closeButton_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
