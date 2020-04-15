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
using System.IO;

namespace IBM1410SMS
{
    public partial class ReportConnectorErrorsForm : Form
    {

        DBSetup db = DBSetup.Instance;

        Table<Machine> machineTable;
        List<Machine> machineList;

        Machine currentMachine = null;

        public ReportConnectorErrorsForm() {
            InitializeComponent();

            machineTable = db.getMachineTable();
            machineList = machineTable.getAll();

            //  Fill in the machine combo box, and remember which machine
            //  we started out with.

            machineComboBox.DataSource = machineList;

            string lastMachine = Parms.getParmValue("machine");
            if (lastMachine.Length != 0) {
                currentMachine = machineList.Find(x => x.idMachine.ToString() == lastMachine);
            }

            if (currentMachine == null || currentMachine.idMachine == 0) {
                currentMachine = machineList[0];
            }
            else {
                machineComboBox.SelectedItem = currentMachine;
            }
        }

        private void reportButton_Click(object sender, EventArgs e) {

            Table<Cableedgeconnectionblock> cableEdgeConnectionBlockTable;
            CardSlotInfo cardSlotInfo;
            CardSlotInfo matchingConnectorInfo;
            List<String> messages = new List<string>();

            List<Cableedgeconnectionblock> cableEdgeConnectionBlockList = null;
            List<Cableedgeconnectionblock> matchingCableEdgeConnectionBlockList = null;

            //  Save current machine for later use by this and other dialogs

            Parms.setParmValue("machine", currentMachine.idMachine.ToString());

            messages.Add("Begin Connector Errors Report for machine: " +
                currentMachine.name);

            //  Loop through all of the cable/edge connection blocks,
            //  looking for ones that are in this machine.  For each of those,
            //  if the card type is CONN, get the destination information.
            //  Verify that it too is CONN, and that it's destination matches
            //  this connectors card slot.  If not, report an error.

            cableEdgeConnectionBlockTable = db.getCableEdgeConnectionBlockTable();
            cableEdgeConnectionBlockList = cableEdgeConnectionBlockTable.getAll();

            foreach(Cableedgeconnectionblock cecb in cableEdgeConnectionBlockList) {

                //  Only process CONN types

                string cecbType = Helpers.getCardTypeType(cecb.connectionType);

                if(!cecbType.Equals("CONN") && !cecbType.Equals("CABL")) {
                    continue;
                }

                //  Get the card slot information.  If it isn't for this machine,
                //  skip it.

                cardSlotInfo = Helpers.getCardSlotInfo(cecb.cardSlot);
                if(!cardSlotInfo.machineName.Equals(currentMachine.name)) {
                    continue;
                }

                //  Get the diagram's page name, for use in messages.

                string diagramPageName = Helpers.getCableEdgeConnectionPageName(
                    cecb.cableEdgeConnectionPage);
                string diagramLoc = "Page:" + diagramPageName + ":" +
                    cecb.diagramColumn.ToString() +
                    cecb.diagramRow +                    
                    ", Slot:" + cardSlotInfo.ToSmallString();

                //  Check for matching destination(s)

                matchingCableEdgeConnectionBlockList =
                    cableEdgeConnectionBlockList.FindAll(x =>
                        x.cardSlot == cecb.Destination);

                if(matchingCableEdgeConnectionBlockList.Count == 0) {
                    //  No matches found -- error, unless this is a CABL
                    if (!cecbType.Equals("CABL")) {
                        messages.Add(diagramLoc + 
                            ": No Matching Destination Connector Found, " +
                            "Expected to be " +
                            Helpers.getCardSlotInfo(cecb.Destination).ToSmallString());
                    }
                }
                else if(matchingCableEdgeConnectionBlockList.Count > 1) {
                    //  More than one match found -- error.
                    string destinationBlocks = "";
                    foreach(Cableedgeconnectionblock matchingCecb in 
                        matchingCableEdgeConnectionBlockList) {
                        matchingConnectorInfo = Helpers.getCardSlotInfo(
                            matchingCecb.cardSlot);
                        if(destinationBlocks.Length > 0) {
                            destinationBlocks += ", ";
                        }
                        destinationBlocks += "(Page:" + 
                            Helpers.getCableEdgeConnectionPageName(
                            matchingCecb.cableEdgeConnectionPage) + ":" +
                            matchingCecb.diagramColumn.ToString() +
                            matchingCecb.diagramRow +  ", Slot:" +
                            matchingConnectorInfo.ToSmallString() + ")";
                    }
                    messages.Add(cardSlotInfo.ToSmallString() +
                        ": Multiple Matching Destination Connectors Found: " +
                        destinationBlocks);
                }
                else {
                    //  Exactly one match
                    Cableedgeconnectionblock matchingCecb =
                        matchingCableEdgeConnectionBlockList[0];
                    matchingConnectorInfo = Helpers.getCardSlotInfo(
                        matchingCecb.cardSlot);
                    if (!Helpers.getCardTypeType(
                        matchingCecb.connectionType).Equals(cecbType)) {
                        //  But match is of wrong card type
                        messages.Add(diagramLoc +
                            ": Matching dest. block " +
                            "(Page:" + Helpers.getCableEdgeConnectionPageName(
                            matchingCecb.cableEdgeConnectionPage) + ":" +
                            matchingCecb.diagramColumn.ToString() +
                            matchingCecb.diagramRow + ", Slot:" +
                            matchingConnectorInfo.ToSmallString() + ")" +
                            " is " + 
                            Helpers.getCardTypeType(matchingCecb.connectionType) +
                            " should be " + cecbType);
                    }
                    if(matchingCecb.Destination != cecb.cardSlot) {
                        //  Match's destination doesn't point back to me.
                        //  (Easier to get just one from DB)
                        CardSlotInfo otherConnectorInfo = Helpers.getCardSlotInfo(
                            matchingCecb.Destination);
                        messages.Add(diagramLoc +
                            ": Matching dest. block at " +
                            "(Page:" + Helpers.getCableEdgeConnectionPageName(
                            matchingCecb.cableEdgeConnectionPage) + ":" +
                            matchingCecb.diagramColumn.ToString() +
                            matchingCecb.diagramRow + ", Slot:" +
                            matchingConnectorInfo.ToSmallString() + ")" +
                            " dest. mismatch: " +
                            otherConnectorInfo.ToSmallString());
                    }
                }
            }

            //  If there were no messages, just display a dialog box

            if(messages.Count == 0) {
                MessageBox.Show("No Connector errors found.", "No Errors",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else {
                string log = "";
                foreach(string message in messages) {
                    log += message + Environment.NewLine;
                }
                Form LogDisplayDialog = 
                    new ImporterLogDisplayForm("Messages", log);
                LogDisplayDialog.ShowDialog();
            }           
        }

        private void machineComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            currentMachine = machineList[machineComboBox.SelectedIndex];
        }
    }
}
