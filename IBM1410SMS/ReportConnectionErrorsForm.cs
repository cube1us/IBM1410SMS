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
    public partial class ReportConnectionErrorsForm : Form
    {

        //  Class to store information about a connection to a DOT functino

        class DotConnection
        {
            public int fromDiagramBlock = 0;
            public string fromPin = "";
            public int cardType = 0;
        }

        //  Class to store information about a given DOT function (Wired OR/AND)

       class DotDetail
        {
            public string page = "";
            public string row = "";
            public string column = "";
            public string logicFunction = "";
            public List<DotConnection> connections = new List<DotConnection>();
        }

        //  Class to store information about a card gate

        DBSetup db = DBSetup.Instance;

        Table<Machine> machineTable;
        Table<Connection> connectionTable;
        Table<Diagrampage> diagramPageTable;
        Table<Page> pageTable;
        Table<Dotfunction> dotFunctionTable;
        Table<Diagramblock> diagramBlockTable;
        Table<Cardtype> cardTypeTable;
        Table<Cardgate> cardGateTable;

        List<Machine> machineList;

        Hashtable cardGateHash = new Hashtable();
        Hashtable cardTypeHash = new Hashtable();

        Hashtable connectionHash = null;
        Hashtable diagramPageHash = null;
        Hashtable pageHash = null;
        Hashtable dotFunctionHash = null;
        Hashtable diagramBlockHash = null;
   
        Machine currentMachine = null;

        string logFileName = "";
        StreamWriter logFile = null;

        int logLevel = 0;
        const int MAXLOGLEVEL = 3;

        public ReportConnectionErrorsForm() {
            InitializeComponent();

            machineTable = db.getMachineTable();
            connectionTable = db.getConnectionTable();
            dotFunctionTable = db.getDotFunctionTable();
            diagramBlockTable = db.getDiagramBlockTable();
            diagramPageTable = db.getDiagramPageTable();
            pageTable = db.getPageTable();
            cardGateTable = db.getCardGateTable();
            cardTypeTable = db.getCardTypeTable();

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
                logLevel = Int32.Parse(Parms.getParmValue("connection report log level"));
            }
            catch (FormatException) {
                logLevel = 1;
            }

            logLevel = Math.Min(MAXLOGLEVEL, Math.Max(0, logLevel));
            logLevelComboBox.SelectedItem = logLevel;

            //  Get a list of card gates and build a hash from it.

            List<Cardgate> cardGateList = cardGateTable.getAll();
            foreach(Cardgate gate in cardGateList) {
                cardGateHash.Add(gate.idcardGate, gate);
            }

            //  Same for the card types

            List<Cardtype> cardTypeList = cardTypeTable.getAll();
            foreach(Cardtype ct in cardTypeList) {
                cardTypeHash.Add(ct.idCardType, ct);
            }

            //  Disable the report button for now.

            reportButton.Enabled = directoryTextBox.Text.Length > 0;
        }

        private void machineComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            currentMachine = (Machine)machineComboBox.SelectedItem;
        }

        private void logLevelComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            logLevel = logLevelComboBox.SelectedIndex;
        }

        private void doConnectionReport() {

            pageHash = new Hashtable();
            diagramPageHash = new Hashtable();
            dotFunctionHash = new Hashtable();
            diagramBlockHash = new Hashtable();

            List<DotDetail> dotDetalList = new List<DotDetail>();

            //  Build a hash of the pages related to this machine

            List<Page> pageList = pageTable.getAll();
            foreach(Page page in pageList) {
                if(page.machine == currentMachine.idMachine) {
                    pageHash.Add(page.idPage, page.name);
                }
            }

            //  Having that, build a hash of related diagram pages

            List<Diagrampage> diagramPageList = diagramPageTable.getAll();
            foreach(Diagrampage dp in diagramPageList) {
                if(pageHash.ContainsKey(dp.page)) {
                    diagramPageHash.Add(dp.idDiagramPage, dp);
                }
            }

            //  Having that, build a list of related connections and DOT functions
            //  Also build a list of relevant connections

            List<Dotfunction> dotFunctionList = dotFunctionTable.getAll();
            foreach(Dotfunction df in dotFunctionList) {
                if(diagramPageHash.ContainsKey(df.diagramPage)) {
                    dotFunctionHash.Add(df.idDotFunction, df);
                }
            }

            //  And, also a list of related diagram blocks

            List<Diagramblock> diagramBlockList = diagramBlockTable.getAll();
            foreach(Diagramblock diagramBlock in diagramBlockList) {
                if(diagramPageHash.ContainsKey(diagramBlock.diagramPage)) {
                    diagramBlockHash.Add(diagramBlock.idDiagramBlock, diagramBlock);
                }
            }

            //  And, finally, a list of related connections.

            List<Connection> connectionList = connectionTable.getAll();
            foreach (Connection connection in connectionList) {
                if(diagramBlockHash.ContainsKey(connection.fromDiagramBlock) ||
                    dotFunctionHash.ContainsKey(connection.fromDotFunction) ||
                    diagramBlockHash.ContainsKey(connection.toDiagramBlock) ||
                    dotFunctionHash.ContainsKey(connection.toDotFunction))  {
                    connectionHash.Add(connection.idConnection, connection);
                }
            }

            logMessage("End of Report.");
        }

        //  Write a message to the output

        private void logMessage(string message) {
            logFile.WriteLine(message);
            logFile.Flush();
        }

        private void reportButton_Click(object sender, EventArgs e) {

            logFileName = Path.Combine(directoryTextBox.Text,
                currentMachine.name + "-ConnectionReport.txt");
            logFile = new StreamWriter(logFileName, false);
            logLevel = (int)logLevelComboBox.SelectedItem;

            Parms.setParmValue("report output directory", directoryTextBox.Text);
            Parms.setParmValue("connection report log level", logLevel.ToString());

            logMessage("Connection Report for Machine: " +
                currentMachine.name);

            doConnectionReport();

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
    }
}
