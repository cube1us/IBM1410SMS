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
using System.Collections;

namespace IBM1410SMS
{
    public partial class ReportSignalsForm : Form
    {

        //  Class to store information on a given signal reference.

        class SignalDetail {
            public string name = null;
            public string pageName = "";
            public string fromPage { get; set; } = null;
            public string toPage { get; set; } = null;
            public string row { get; set; } = "";
            public string reference { get; set; } = "";
            public string secondReference { get; set; } = "";
            public bool leftSide { get; set; } = false;
            public bool rightSide { get; set; } = false;
        }

        //  Class to track a given signal

        class Signal
        {
            public List<SignalDetail> origins { get; set; } = new List<SignalDetail>();
                //  Source: toEdgeSheet

            public List<SignalDetail> otherInputs { get; set; } = new List<SignalDetail>();
                //  Source: fromEdgeSheet

            public List<SignalDetail> inputs { get; set; } = new List<SignalDetail>();
                //  Source: fromEdgeOriginSheet

            public List<SignalDetail> outputs { get; set; } = new List<SignalDetail>();
                //  Source: toEdgeDestinationSheet
        }

        DBSetup db = DBSetup.Instance;

        Table<Machine> machineTable;
        Table<Connection> connectionTable;
        Table<Sheetedgeinformation> sheetEdgeInformationTable;
        Table<Diagrampage> diagramPageTable;
        Table<Page> pageTable;

        List<Machine> machineList;
        List<Diagrampage> diagramPageList;

        Machine currentMachine = null;

        string logFileName = "";
        StreamWriter logFile = null;

        int logLevel = 1;
        const int MAXLOGLEVEL = 3;


        public ReportSignalsForm() {
            InitializeComponent();

            machineTable = db.getMachineTable();
            connectionTable = db.getConnectionTable();
            sheetEdgeInformationTable = db.getSheetEdgeInformationTable();
            diagramPageTable = db.getDiagramPageTable();
            pageTable = db.getPageTable();

            machineList = machineTable.getAll();

            //  Fill in the log level combo box

            for (int i = 0; i <= MAXLOGLEVEL; ++i) {
                logLevelComboBox.Items.Add(i);
            }

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

            //  Pre-select the last log level used.

            try {
                logLevel = Int32.Parse(Parms.getParmValue("edge report log level"));
            }
            catch (FormatException) {
                logLevel = 1;
            }

            logLevel = Math.Min(MAXLOGLEVEL, Math.Max(0, logLevel));
            logLevelComboBox.SelectedItem = logLevel;

            //  Disable the report button for now.

            reportButton.Enabled = directoryTextBox.Text.Length > 0;

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

        private void reportButton_Click(object sender, EventArgs e) {

            logFileName = Path.Combine(directoryTextBox.Text,
                currentMachine.name + "-SignalReport.txt");
            logFile = new StreamWriter(logFileName, false);
            logLevel = (int)logLevelComboBox.SelectedItem;

            Parms.setParmValue("report output directory", directoryTextBox.Text);
            Parms.setParmValue("edge report log level", logLevel.ToString());

            logMessage("Sheet Edge Connection Report for Machine: " +
                currentMachine.name);

            doSignalReport();

            //  Show a box, maybe, eventually.

            logFile.Close();

        }

        private void machineComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            currentMachine = (Machine)machineComboBox.SelectedItem;
        }

        private void doSignalReport() {

            reportButton.Enabled = false;
            diagramPageList = new List<Diagrampage>();

            Hashtable signalHash = new Hashtable();
            Hashtable signalNameHash = new Hashtable();

            //  Build a list of relevant diagram pages for this machine.

            List<Page> pageList = pageTable.getWhere("WHERE machine = '" +
                currentMachine.idMachine + "'");

            foreach(Page page in pageList) {
                List<Diagrampage> dpList = diagramPageTable.getWhere(
                    "WHERE page = '" + page.idPage + "'");
                if(dpList.Count <= 0) {
                    //  This might not be a diagram page
                    continue;
                }
                else if(dpList.Count > 1) {
                    logMessage("ERROR: More than one diagram page found for page " +
                        page.name + "(" + page.idPage.ToString() + ")");
                }
                else {
                    diagramPageList.Add(dpList[0]);
                }
            }

            logMessage(diagramPageList.Count.ToString() + " Diagram pages found for " +
                "machine " + currentMachine.name);

            int signalCount = 0;
            int connCount = 0;

            List<Sheetedgeinformation> sheetSignals = sheetEdgeInformationTable.getAll();
            foreach(Sheetedgeinformation sig in sheetSignals) {

                //  If this sheet edge information entry isn't in our list of
                //  diagram pages, skip it.

                Diagrampage diagramPage = diagramPageList.Find(
                    x => x.idDiagramPage == sig.diagramPage);
                if(diagramPage == null || diagramPage.idDiagramPage == 0) {
                    continue;
                }

                //  If this signal name is new, add it to our hash tables

                if (!signalNameHash.ContainsKey(sig.signalName)) {
                    Signal signal = new Signal();
                    signalNameHash.Add(sig.signalName, signal);
                }

                //  Add a signal detail entry as well.
                //  NOTE:  This is NOT the same detail object that appears in
                //  signals.  It is just a handy place to store information.

                SignalDetail detail = new SignalDetail();
                detail.leftSide = (sig.leftSide > 0);
                detail.rightSide = (sig.rightSide > 0);
                detail.row = sig.row;
                detail.name = sig.signalName;
                detail.pageName = pageList.Find(x => x.idPage == diagramPage.page).name;
                signalHash.Add(sig.idSheetEdgeInformation, detail);

                ++signalCount;
            }

            List<Connection> connections = connectionTable.getAll();

            foreach(Connection connection in connections) {

                bool foundSignal = false;
                SignalDetail detail = null;
                Signal signal = null;


                if ((detail = (SignalDetail)signalHash[connection.fromEdgeSheet]) != null) {
                    signal = (Signal) signalNameHash[detail.name];
                    if(signal == null) {
                        logMessage("Internal Error: Signal name " + detail.name + 
                            " referenced by connection Database ID " + 
                            connection.idConnection + " fromEdgeSheet was not found.");
                    }

                    SignalDetail newDetail = new SignalDetail();
                    newDetail.name = detail.name;
                    newDetail.pageName = detail.pageName;
                    newDetail.fromPage = null;      // TODO: Origin page?
                    newDetail.toPage = detail.pageName;
                    newDetail.row = detail.row;
                    newDetail.reference = connection.fromEdgeConnectorReference;
                    newDetail.secondReference = "";
                    newDetail.leftSide = detail.leftSide;
                    newDetail.rightSide = detail.rightSide;
                    signal.otherInputs.Add(newDetail);
                    foundSignal = true;
                }

                if ((detail = (SignalDetail)signalHash[connection.fromEdgeOriginSheet]) 
                    != null) {
                    signal = (Signal)signalNameHash[detail.name];
                    if (signal == null) {
                        logMessage("Internal Error: Signal name " + detail.name +
                            " referenced by connection Database ID " +
                            connection.idConnection + " fromEdgeOriginSheet was not found.");
                    }

                    SignalDetail newDetail = new SignalDetail();
                    newDetail.name = detail.name;
                    newDetail.pageName = detail.pageName;
                    newDetail.fromPage = detail.pageName; 
                    newDetail.toPage = null;   //  TODO: Destination Page
                    newDetail.row = detail.row;
                    newDetail.reference = connection.fromEdgeConnectorReference;
                    newDetail.secondReference = "";
                    newDetail.leftSide = detail.leftSide;
                    newDetail.rightSide = detail.rightSide;
                    signal.inputs.Add(newDetail);
                    foundSignal = true;
                }

                if ((detail = (SignalDetail)signalHash[connection.toEdgeSheet]) != null) {
                    signal = (Signal)signalNameHash[detail.name];
                    if (signal == null) {
                        logMessage("Internal Error: Signal name " + detail.name +
                            " referenced by connection Database ID " +
                            connection.idConnection + " toEdgeSheet was not found.");
                    }

                    SignalDetail newDetail = new SignalDetail();
                    newDetail.name = detail.name;
                    newDetail.pageName = detail.pageName;   
                    newDetail.fromPage = detail.pageName;
                    newDetail.toPage = null;    //  TODO: Destination Page
                    newDetail.row = detail.row;
                    newDetail.reference = connection.toEdgeConnectorReference;
                    newDetail.secondReference = connection.toEdge2ndConnectorReference;
                    newDetail.leftSide = detail.leftSide;
                    newDetail.rightSide = detail.rightSide;
                    signal.origins.Add(newDetail);
                    foundSignal = true;
                }

                if ((detail = (SignalDetail)signalHash[connection.toEdgeDestinationSheet]) 
                    != null) {
                    signal = (Signal)signalNameHash[detail.name];
                    if (signal == null) {
                        logMessage("Internal Error: Signal name " + detail.name +
                            " referenced by connection Database ID " +
                            connection.idConnection + 
                            " toEdgeDestinationSheet was not found.");
                    }

                    SignalDetail newDetail = new SignalDetail();
                    newDetail.name = detail.name;
                    newDetail.pageName = detail.pageName;
                    newDetail.fromPage = null;  //  TODO:  From Page
                    newDetail.toPage = detail.pageName;
                    newDetail.row = detail.row;
                    newDetail.reference = connection.toEdgeConnectorReference;
                    newDetail.secondReference = connection.toEdge2ndConnectorReference;
                    newDetail.leftSide = detail.leftSide;
                    newDetail.rightSide = detail.rightSide;
                    signal.outputs.Add(newDetail);
                    foundSignal = true;
                }

                if(foundSignal) {
                    ++connCount;
                }

            }



            logMessage("Found " + signalCount.ToString() + " signals, with " +
                connCount.ToString() + " connections.");

            logMessage("End of Report.");
            reportButton.Enabled = true;
        }

        //  Write a message to the output

        private void logMessage(string message) {
            logFile.WriteLine(message);
            logFile.Flush();
        }

    }
}
