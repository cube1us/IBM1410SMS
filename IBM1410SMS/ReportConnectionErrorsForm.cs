﻿using System;
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
            public Connection connection { get; set; } = null;
            public int fromDiagramBlock { get; set; } = 0;
            public string fromRow { get; set; } = "";
            public string fromColumn { get; set; }  = "";
            public int fromGate { get; set; } = 0;
            public string fromPin { get; set; } = "";
            public string fromLoadPin { get; set; } = "";
            public int cardType { get; set; } = 0;
        }

        //  Class to store information about a given DOT function (Wired OR/AND)

       class DotDetail
        {
            public int dotFunctionKey = 0;
            public string page { get; set; } = "";
            public string row { get; set; } = "";
            public string column { get; set; } = "";
            public string logicFunction { get; set; } = "";
            public List<DotConnection> connections { get; set; } = new List<DotConnection>();
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
        Table<Sheetedgeinformation> sheetEdgeInformationTable;
        Table<Logicfamily> logicFamilyTable;

        List<Machine> machineList;

        Hashtable cardGateHash = new Hashtable();
        Hashtable cardTypeHash = new Hashtable();
        Hashtable logicFamilyHash = new Hashtable();

        Hashtable connectionHash = null;
        Hashtable diagramPageHash = null;
        Hashtable pageHash = null;
        Hashtable dotFunctionHash = null;
        Hashtable diagramBlockHash = null;
        Hashtable dotDetailHash = null;
   
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
            sheetEdgeInformationTable = db.getSheetEdgeInformationTable();
            logicFamilyTable = db.getLogicFamilyTable();

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

            //  And logic families

            List<Logicfamily> logicFamilyList = logicFamilyTable.getAll();
            foreach (Logicfamily lf in logicFamilyList) {
                logicFamilyHash.Add(lf.idLogicFamily, lf);
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
            connectionHash = new Hashtable();
            dotDetailHash = new Hashtable();

            Hashtable dotFunctionsByPage = new Hashtable();

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

            //  And, finally, a list of related connections (exluding sheet edge
            //  connections, which are handled in the Signals report.)

            List<Connection> connectionList = connectionTable.getAll();
            foreach (Connection connection in connectionList) {
                if(diagramBlockHash.ContainsKey(connection.fromDiagramBlock) ||
                    dotFunctionHash.ContainsKey(connection.fromDotFunction) ||
                    diagramBlockHash.ContainsKey(connection.toDiagramBlock) ||
                    dotFunctionHash.ContainsKey(connection.toDotFunction))  {
                    connectionHash.Add(connection.idConnection, connection);
                }
                if(!Helpers.isValidConnectionType(connection.from)) {
                    logConnection(connection);
                    logMessage("  Invalid FROM connection type: " + connection.from);
                }
                if (!Helpers.isValidConnectionType(connection.to)) {
                    logConnection(connection);
                    logMessage("  Invalid TO connection type: " + connection.to);
                }
            }

            //  DOT Function checks

            //  Go through the connections finding the ones with DOT function 
            //  destinations.  Also check that it isn't fed by another DOT function

            //  For those that are dot function destinations, create a DOT function detail
            //  entry (if not already present) and add this connection to its list
            //  of connections.

            foreach(int connectionKey in connectionHash.Keys) {
                Connection connection = (Connection) connectionHash[connectionKey];
                if(connection.toDotFunction > 0) {

                    Dotfunction dotFunction =
                        (Dotfunction)dotFunctionHash[connection.toDotFunction];

                    if (dotFunction == null) {
                        logConnection(connection);
                        logMessage("   Error: Invalid DOT Function (" +
                            connection.toDotFunction.ToString() + ")");
                    }
                    else if (connection.fromDotFunction > 0) {
                        logConnection(connection);
                        logMessage("   Error: Connection is FROM DOT Function TO DOT Function.");
                    }
                    else if(connection.fromEdgeSheet != 0) {
                        //  Ignore connections from edge signals.
                    }
                    else if(connection.fromDiagramBlock == 0) {
                        logConnection(connection);
                        logMessage("   Connection to DOT function is not from a diagram block?");
                    }
                    else if(!diagramBlockHash.ContainsKey(connection.fromDiagramBlock)) {
                        logConnection(connection);
                        logMessage("   Connection to DOT function from invalid diagram block (" +
                            connection.fromDiagramBlock + ")");
                    }
                    else {

                        Diagramblock diagramBlock = 
                            (Diagramblock) diagramBlockHash[connection.fromDiagramBlock];

                        //  If there is not already a detail entry for this DOT function,
                        //  create one now.

                        if (!dotDetailHash.ContainsKey(dotFunction.idDotFunction)) {                            
                            DotDetail detail = new DotDetail();
                            detail.dotFunctionKey = dotFunction.idDotFunction;
                            detail.page = getDiagramPageName(dotFunction.diagramPage);
                            detail.row = dotFunction.diagramRowTop;
                            detail.column = dotFunction.diagramColumnToLeft.ToString();
                            detail.logicFunction = dotFunction.logicFunction;
                            detail.connections = new List<DotConnection>();
                            dotDetailHash.Add(dotFunction.idDotFunction, detail);
                        }

                        //  Add a new connection to the detail entry.

                        DotDetail dotDetail = (DotDetail) dotDetailHash[dotFunction.idDotFunction];
                        DotConnection dotConnection = new DotConnection();
                        dotConnection.connection = connection;
                        dotConnection.cardType = diagramBlock.cardType;
                        dotConnection.fromDiagramBlock = diagramBlock.idDiagramBlock;
                        dotConnection.fromPin = connection.fromPin;
                        dotConnection.fromRow = diagramBlock.diagramRow;
                        dotConnection.fromColumn = diagramBlock.diagramColumn.ToString();
                        dotConnection.fromGate = diagramBlock.cardGate;
                        dotConnection.fromLoadPin =
                            connection.fromLoadPin == null ? "" : connection.fromLoadPin;                        
                        dotDetail.connections.Add(dotConnection);
                    }
                }
            }

            //  Now spin through all of the DOT functions we just assembled, to see
            //  if there are any cases where we have multiple loads (i.e., loads +
            //  NON open collector gates > 1)

            //  TODO:  Get these sorted by page.


            foreach(DotDetail detail in dotDetailHash.Values) {
                if(!dotFunctionsByPage.Contains(detail.page)) {
                    dotFunctionsByPage.Add(detail.page, new List<DotDetail>());
                }
                ((List<DotDetail>)dotFunctionsByPage[detail.page]).Add(detail);
            }

            ArrayList sortedPages = new ArrayList(dotFunctionsByPage.Keys);
            sortedPages.Sort();


            foreach (string page in sortedPages) {
                foreach (DotDetail detail in (List<DotDetail>)dotFunctionsByPage[page]) {
                    int loadCount = 0;
                    bool hasSwitch = false;
                    List<DotConnection> connections = detail.connections;
                    foreach (DotConnection dotConnection in connections) {

                        //  Is a load pin involved?  If so, count it.

                        if (dotConnection.fromLoadPin.Length > 0) {
                            ++loadCount;
                        }

                        //  Is the gate open collector?  If not, count it.

                        Diagramblock diagramBlock =
                            (Diagramblock)diagramBlockHash[dotConnection.fromDiagramBlock];
                        if (diagramBlock == null) {
                            logConnection(dotConnection.connection);
                            logMessage("   Internal Error: DotConnection Diagram Block not " +
                                "found (" + dotConnection.fromDiagramBlock.ToString() + ")");
                            continue;
                        }
                        Cardgate gate = (Cardgate)cardGateHash[dotConnection.fromGate];
                        if (gate == null) {
                            logConnection(dotConnection.connection);
                            logMessage("   Internal Error:  Card Gate not found (" +
                                dotConnection.fromGate.ToString() + ")");
                            continue;
                        }

                        if (isCardASwitch(dotConnection.cardType)) {
                            //  Switches are treaed as special, and also not as loads
                            hasSwitch = true;
                        }
                        else if (gate.openCollector == 0) {
                            ++loadCount;
                        }

                    }
                    if ((!hasSwitch && loadCount == 0) || loadCount > 1) {
                        logMessage("Invalid DOT Function load count of " + loadCount.ToString() +
                            " [Expected to be 1]");
                        logMessage("   " + getDotFunctionInfo(detail.dotFunctionKey) + " (" +
                            connections.Count.ToString() + " connections)");
                    }
                }

            }

            logMessage("End of Report.");
        }

        //  Log information about a connection

        private void logConnection(Connection connection) {

            string message = "Connection From: ";
            switch (connection.from) {
                case "P":
                    message += "Block " + getDiagramBlockInfo(connection.fromDiagramBlock) +
                        " pin " + connection.fromPin;
                    if(connection.fromLoadPin.Length > 0) {
                        message += " (Load pin " + connection.fromLoadPin + ")";
                    }
                    break;
                case "D":
                    message += "DOT Function " + getDotFunctionInfo(connection.fromDotFunction);
                    break;
                case "E":
                    message += "Edge Signal ";
                    break;
                default:
                    message += "INVALID ";
                    break;
            }

            message += " To: ";
            switch (connection.to) {
                case "P":
                    message += "Block " + getDiagramBlockInfo(connection.toDiagramBlock) +
                        " pin " + connection.toPin;
                    break;
                case "D":
                    message += "DOT Function " + getDotFunctionInfo(connection.toDotFunction);
                    break;
                case "E":
                    message += "Edge Signal ";
                    break;
                default:
                    message += "INVALID ";
                    break;
            }

            logMessage(message);
        }

        //  Return information on a diagram block connection

        private string getDiagramBlockInfo(int diagramBlockKey) {
            string info = "Diagram Block ";

            Diagramblock diagramBlock = (Diagramblock) diagramBlockHash[diagramBlockKey];
            if(diagramBlock == null) {
                return ("Invalid Diagram Block (" + diagramBlockKey.ToString() + ")");
            }            
            info += getDiagramPageName(diagramBlock.diagramPage) + ", row " + 
                diagramBlock.diagramRow + ", column " +  diagramBlock.diagramColumn.ToString();
            return (info);
        }

        private string getDiagramPageName(int diagramPageKey) {
            if(!diagramPageHash.ContainsKey(diagramPageKey)) {
                return (" Invalid Diagram page (" + diagramPageKey.ToString() + ")");
            }
            Diagrampage diagramPage = (Diagrampage)diagramPageHash[diagramPageKey];
            if(!pageHash.ContainsKey(diagramPage.page)) {
                return(" Invalid Page (" + diagramPage.page.ToString() + ")");
            }

            return ((string)pageHash[diagramPage.page]);
        }

        //  Return information on a dot function connection

        private string getDotFunctionInfo(int dotFunctionKey) {
            string info = "";

            Dotfunction dotFunction = (Dotfunction)dotFunctionHash[dotFunctionKey];
            if(dotFunction == null) {
                return ("Invalid DOT Function (" + dotFunctionKey.ToString() + ")");
            }

            Diagrampage diagramPage = (Diagrampage)diagramPageHash[dotFunction.diagramPage];
            info += diagramPage != null ? "Page " + 
                getDiagramPageName(diagramPage.idDiagramPage) : "(Invalid page) ";
            info += ", row " + dotFunction.diagramRowTop + ", column " +
                dotFunction.diagramColumnToLeft.ToString();
            return (info);

        }

        //  Determine if a card type is a switch

        private bool isCardASwitch(int cardTypeKey) {
            Cardtype type = (Cardtype) cardTypeHash[cardTypeKey];
            if(type == null) {
                return false;
            }
            Logicfamily family = (Logicfamily) logicFamilyHash[type.logicFamily];
            if(family == null) {
                return false;
            }
            return (family.name == "SWITCH");
        }

        //  Write a message to the output

        private void logMessage(string message) {
            logFile.WriteLine(message);
            logFile.Flush();
        }

        private void reportButton_Click(object sender, EventArgs e) {

            reportButton.Enabled = false;

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
            reportButton.Enabled = true;
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