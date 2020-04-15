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

    public partial class ReportEdgeConnectionsForm : Form
    {

        //  Class to hold information on an edge connection to make life
        //  easier instead of having to chase other tables down all the time.

        class edgeConnectorEntry : IComparable<edgeConnectorEntry>
        {
            public Edgeconnector edgeConnector { get; set; }
            public CardSlotInfo cardSlotInfo { get; set; }
            public string pageName { get; set; }

            public int CompareTo(edgeConnectorEntry other) {

                int c;

                if (other == null) {
                    return 1;
                }

                //  There should always be a page name, card slot and
                //  edgeconnector, but, just in case:

                if(pageName == null && other.pageName == null) {
                    return 0;
                }
                else if(pageName == null) {
                    return -1;
                }
                else if(other.pageName == null) {
                    return 1;
                }

                if ((c = pageName.CompareTo(other.pageName)) != 0) {
                    return c;
                }

                if ((c = edgeConnector.reference.CompareTo(other.edgeConnector.reference))
                    != 0) {
                    return c;
                }

                return (edgeConnector.order.CompareTo(other.edgeConnector.order));

                //  There MUST be a card slot and other fields.

                /*
                 *  NOT comparing on card slot.
                 *  
                if((c = cardSlotInfo.machineName.CompareTo(other.cardSlotInfo.machineName))
                    != 0) {
                    return c;
                }
                if((c = cardSlotInfo.frameName.CompareTo(other.cardSlotInfo.frameName)) != 0) {
                    return c;
                }
                if((c = cardSlotInfo.gateName.CompareTo(other.cardSlotInfo.gateName)) != 0) {
                    return c;
                }
                if((c = cardSlotInfo.panelName.CompareTo(other.cardSlotInfo.panelName)) != 0) {
                    return c;
                }
                if((c = cardSlotInfo.row.CompareTo(other.cardSlotInfo.row)) != 0) {
                    return c;
                }
                if((c = cardSlotInfo.column.CompareTo(other.cardSlotInfo.column)) != 0) {
                    return c;
                }
                */
            }
        }

        DBSetup db = DBSetup.Instance;

        Table<Machine> machineTable;
        List<Machine> machineList;

        Machine currentMachine = null;

        string logFileName = "";
        StreamWriter logFile = null;

        bool debug = true;

        public ReportEdgeConnectionsForm() {
            InitializeComponent();
            machineTable = db.getMachineTable();
            machineList = machineTable.getAll();

            //  Fill in the machine combo box, and remember which machine
            //  we started out with.

            machineComboBox.DataSource = machineList;

            string lastMachine = Parms.getParmValue("machine");
            if (lastMachine.Length != 0) {
                currentMachine = machineList.Find(
                    x => x.idMachine.ToString() == lastMachine);
            }

            if (currentMachine == null || currentMachine.idMachine == 0) {
                currentMachine = machineList[0];
            }
            else {
                machineComboBox.SelectedItem = currentMachine;
            }

            //  Also fill in the Report directory text box

            directoryTextBox.Text = Parms.getParmValue("report output directory");

            //  Disable the report button for now.

            reportButton.Enabled = directoryTextBox.Text.Length > 0;
        }

        private void reportButton_Click(object sender, EventArgs e) {

            logFileName = Path.Combine(directoryTextBox.Text,
                currentMachine.name + "-EdgeConnectionReport.txt");
            logFile = new StreamWriter(logFileName, false);

            Parms.setParmValue("report output directory", directoryTextBox.Text);
            logMessage("Sheet Edge Connection Report for Machine: " +
                currentMachine.name);

            doEdgeConnectionReport();

            //  Show a box, maybe, eventually.

            logFile.Close();
        }

        private void directoryButton_Click(object sender, EventArgs e) {
            FolderBrowserDialog folderBrowserDialog1 =
                new FolderBrowserDialog();

            folderBrowserDialog1.Description =
                "Identify the directory to use for the report: ";

            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK) {
                directoryTextBox.Text = folderBrowserDialog1.SelectedPath;
                reportButton.Enabled = (directoryTextBox.Text.Length > 0);
            }

        }

        private void doEdgeConnectionReport() {

            Table<Edgeconnector> edgeConnectorTable = db.getEdgeConnectorTable();
            Table<Diagrampage> diagrampageTable = db.getDiagramPageTable();
            Table<Cableedgeconnectionblock> cableEdgeConnectionBlockTable =
                db.getCableEdgeConnectionBlockTable();

            List<Edgeconnector> allEdgeConnectorList = null;
            List<edgeConnectorEntry> edgeConnectors = new List<edgeConnectorEntry>();

            List<Cableedgeconnectionblock> allCableEdgeConnectionBlockList = null;
            List<Cableedgeconnectionblock> cableEdgeConnectionBlockList =
                new List<Cableedgeconnectionblock>();

            reportButton.Enabled = false;

            //  Build a list of cable/edge connectors that are relevant

            allCableEdgeConnectionBlockList = cableEdgeConnectionBlockTable.getAll();
            foreach (Cableedgeconnectionblock cecb in allCableEdgeConnectionBlockList) {
                if(Helpers.getCardSlotInfo(cecb.cardSlot).machineName ==
                    currentMachine.name) {
                    cableEdgeConnectionBlockList.Add(cecb);
                }
            }

            if(debug) {
                logMessage("Cable/Edge Connections: " +
                    cableEdgeConnectionBlockList.Count.ToString());
                logMessage(DateTime.Now.ToLocalTime().ToString());
            }

            //  Next, build a list of edgeConnector entries that are relevant
            //  Really, this should include all related machines.   However, as
            //  other machines are really at the beginng or end of a chain, it 
            //  should be OK, he said, hopefully.

            allEdgeConnectorList = edgeConnectorTable.getAll();
            foreach(Edgeconnector edgeConnector in allEdgeConnectorList) {
                if(Helpers.getCardSlotInfo(edgeConnector.cardSlot).machineName ==
                    currentMachine.name) {
                    edgeConnectorEntry ece = new edgeConnectorEntry();
                    ece.edgeConnector = edgeConnector;
                    ece.cardSlotInfo = Helpers.getCardSlotInfo(edgeConnector.cardSlot);
                    ece.pageName = Helpers.getDiagramPageName(edgeConnector.diagramPage);
                    edgeConnectors.Add(ece);
                }
            }

            if(debug) {
                logMessage("Edge Connectors: " + edgeConnectors.Count.ToString());
                logMessage(DateTime.Now.ToLocalTime().ToString());
            }

            edgeConnectors.Sort();

            if (debug) {
                logMessage("Sorted Edge Connectors: " + edgeConnectors.Count.ToString());
                logMessage(DateTime.Now.ToLocalTime().ToString());
                for (int i = 0; i < 100; ++i) {
                    edgeConnectorEntry e = edgeConnectors[i];
                    logMessage(e.pageName + ":" +
                        e.edgeConnector.reference + ":" +
                        e.edgeConnector.order + ":"+
                        e.cardSlotInfo.ToSmallString() +
                        e.edgeConnector.pin);                        
                }
            }




            reportButton.Enabled = true;

            logMessage("End of Report");

        }

        private void logMessage(string message) {
            logFile.WriteLine(message);
            logFile.Flush();
        }

    }
}
