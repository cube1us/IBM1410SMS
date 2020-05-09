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
            public Connection connection { get; set; } = null;
            public int fromDiagramBlock { get; set; } = 0;
            public string fromRow { get; set; } = "";
            public string fromColumn { get; set; }  = "";
            public int fromGate { get; set; } = 0;
            public string fromPin { get; set; } = "";
            public string fromLoadPin { get; set; } = "";
            public int cardType { get; set; } = 0;
            public bool fromResistor { get; set; } = false;
        }

        //  Class to store information about a given DOT function (Wired OR/AND)

       class DotDetail
        {
            public int dotFunctionKey = 0;
            public string page { get; set; } = "";
            public string row { get; set; } = "";
            public string column { get; set; } = "";
            public string logicFunction { get; set; } = "";
            public bool hasEdgeOutput { get; set; } = false;
            public bool hasGateOutput { get; set; } = false;
            public List<DotConnection> connections { get; set; } = new List<DotConnection>();
        }

        //  Class to store information about connections that eminate from a given pin

        class PinDetail
        {
            public string pin { get; set; } = "";
            public int loadPins { get; set; }  = 0;
            public bool input { get; set; } = false;
            public bool output { get; set; } = false;
            public List<Connection> connectionsFrom { get; set; } = new List<Connection>();
            public List<Connection> connectionsTo { get; set; } = new List<Connection>();
        }

        //  Class to store information about a Logic Block

        class BlockDetail
        {
            public Diagramblock diagramBlock { get; set; } = null;
            public string page { get; set; } = "";
            public List<PinDetail> pinList { get; set; }  = new List<PinDetail>();
        }

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
        Table<Gatepin> gatePinTable;

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
        Hashtable sheetEdgeHash = null;
   
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
            gatePinTable = db.getGatePinTable();

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
                logLevel = 0;
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
            sheetEdgeHash = new Hashtable();

            Hashtable dotFunctionsByPage = new Hashtable();
            Hashtable diagramBlocksByPage = new Hashtable();

            ArrayList sortedPages = null;

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
                    // diagramBlockHash.Add(diagramBlock.idDiagramBlock, diagramBlock);
                    BlockDetail detail = new BlockDetail();
                    detail.diagramBlock = diagramBlock;
                    Diagrampage dp = (Diagrampage)diagramPageHash[diagramBlock.diagramPage];
                    detail.page = (String)pageHash[dp.page];
                    diagramBlockHash.Add(diagramBlock.idDiagramBlock, detail);
                }
            }

            //  A list of sheet edge information signals, too...

            List<Sheetedgeinformation> sheetEdgeInformationList =
                sheetEdgeInformationTable.getAll();
            foreach(Sheetedgeinformation se in sheetEdgeInformationList) {
                if(diagramPageHash.ContainsKey(se.diagramPage)) {
                    sheetEdgeHash.Add(se.idSheetEdgeInformation, se);
                }
            }


            //  And, finally, a list of related connections (exluding sheet edge
            //  connections, which are handled in the Signals report.)

            List<Connection> connectionList = connectionTable.getAll();
            foreach (Connection connection in connectionList) {
                if(diagramBlockHash.ContainsKey(connection.fromDiagramBlock) ||
                   dotFunctionHash.ContainsKey(connection.fromDotFunction) ||
                   diagramBlockHash.ContainsKey(connection.toDiagramBlock) ||
                   dotFunctionHash.ContainsKey(connection.toDotFunction) ||
                   sheetEdgeHash.ContainsKey(connection.fromEdgeSheet) ||
                   sheetEdgeHash.ContainsKey(connection.toEdgeSheet)) {
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

            logMessage("*** Begin DOT Function Connection Checks ***");

            //  DOT Function checks

            //  Go through the connections finding the ones with DOT function 
            //  destinations.  Also check that it isn't fed by another DOT function

            //  For those that are dot function destinations, create a DOT function detail
            //  entry (if not already present) and add this connection to its list
            //  of connections.

            foreach(int connectionKey in connectionHash.Keys) {
                Connection connection = (Connection) connectionHash[connectionKey];

                if (connection.toDotFunction > 0) {

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
                            ((BlockDetail)diagramBlockHash[connection.fromDiagramBlock]).diagramBlock;

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
                            detail.hasEdgeOutput = false;
                            dotDetailHash.Add(dotFunction.idDotFunction, detail);
                        }

                        //  Add a new connection to the detail entry.

                        DotDetail dotDetail = 
                            (DotDetail) dotDetailHash[dotFunction.idDotFunction];
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
                        if(diagramBlock.symbol == "R") {
                            dotConnection.fromResistor = true;
                        }
                        dotDetail.connections.Add(dotConnection);
                    }
                }

                //  Note if this DOT function has a connection to a sheet edge.
                //  (In those cases, we suppress error messages if the load count is 0)
                //  But we do not remember the connection itself.

                if((connection.fromDotFunction > 0 && connection.toEdgeSheet > 0) ||
                   (connection.fromDotFunction > 0 && connection.toDiagramBlock > 0)) {
                    Dotfunction dotFunction =
                        (Dotfunction)dotFunctionHash[connection.fromDotFunction];

                    //  Might be a new DOT function we have to add.

                    if (!dotDetailHash.ContainsKey(dotFunction.idDotFunction)) {
                        DotDetail detail = new DotDetail();
                        detail.dotFunctionKey = dotFunction.idDotFunction;
                        detail.page = getDiagramPageName(dotFunction.diagramPage);
                        detail.row = dotFunction.diagramRowTop;
                        detail.column = dotFunction.diagramColumnToLeft.ToString();
                        detail.logicFunction = dotFunction.logicFunction;
                        detail.connections = new List<DotConnection>();
                        dotDetailHash.Add(dotFunction.idDotFunction, detail);
                        detail.hasEdgeOutput = (connection.toEdgeSheet > 0);
                        detail.hasGateOutput = (connection.toDiagramBlock > 0);
                    }
                    else {
                        DotDetail dotDetail = 
                            (DotDetail)dotDetailHash[dotFunction.idDotFunction];
                        dotDetail.hasEdgeOutput = (connection.toEdgeSheet > 0);
                        dotDetail.hasGateOutput = (connection.toDiagramBlock > 0);
                    }
                }
            }

            //  Now spin through all of the DOT functions we just assembled, to see
            //  if there are any cases where we have multiple loads (i.e., loads +
            //  NON open collector gates > 1)

            //  Report them in page order!

            foreach(DotDetail detail in dotDetailHash.Values) {
                if(!dotFunctionsByPage.Contains(detail.page)) {
                    dotFunctionsByPage.Add(detail.page, new List<DotDetail>());
                }
                ((List<DotDetail>)dotFunctionsByPage[detail.page]).Add(detail);
            }

            sortedPages = new ArrayList(dotFunctionsByPage.Keys);
            sortedPages.Sort();

            foreach (string page in sortedPages) {
                foreach (DotDetail detail in (List<DotDetail>)dotFunctionsByPage[page]) {
                    int loadCount = 0;
                    bool hasSwitch = false;
                    bool hasResistor = false;
                    List<DotConnection> connections = detail.connections;
                    foreach (DotConnection dotConnection in connections) {

                        //  Is a load pin involved?  If so, count it.

                        if (dotConnection.fromLoadPin.Length > 0) {
                            ++loadCount;
                        }

                        //  Is the gate open collector?  If not, count it.

                        Diagramblock diagramBlock =
                            ((BlockDetail)diagramBlockHash[dotConnection.fromDiagramBlock]).diagramBlock;
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

                        if(dotConnection.fromResistor) {
                            hasResistor = true;
                        }
                        else if (isCardASwitch(dotConnection.cardType)) {
                            //  Switches are treated as special, and also not as loads
                            hasSwitch = true;
                        }
                        else if (gate.openCollector == 0) {
                            //  Not open collector - so count it.
                            ++loadCount;
                        }
                    }

                    //  If the load count is 0 and it isn't a switch or a DOT function
                    //  whose output goes to a sheet edge, OR, if the load count is more
                    //  than 1 (meaning 2 non-open collector gates), issue a warning.

                    if ((!(hasSwitch || detail.hasEdgeOutput || detail.hasGateOutput ||
                        hasResistor) && loadCount == 0) || loadCount > 1) {
                        logMessage("Unexpected DOT Function load count of " + 
                            loadCount.ToString() + " [Expected to be 1]");
                        logMessage("   " + getDotFunctionInfo(detail.dotFunctionKey) + " (" +
                            connections.Count.ToString() + " connections)");
                        logMessage("");
                    }
                }

            }

            logMessage("*** End of DOT Function Check ***");
            logMessage("");

            logMessage("*** Logic Block Connection Check ***");

            //  Diagram block checks

            //  Go through the connections finding the ones that originate from
            //  diagram logic blocks, and create a list of connections to and from 
            //  each pin of each lbock.  Also note the load pins.

            //  TODO:  This also MIGHT be the right area to handle extended blocks?

            foreach(int connectionKey in connectionHash.Keys) {
                Connection connection = (Connection)connectionHash[connectionKey];
                if(connection.fromDiagramBlock > 0) {

                    logDebug(2, "DEBUG: FROM Logic block "     +
                            getDiagramBlockInfo(connection.fromDiagramBlock));

                    BlockDetail detail = 
                        (BlockDetail)diagramBlockHash[connection.fromDiagramBlock];
                    if(detail == null) {
                        logConnection(connection);
                        logMessage("   Error:  Invalid fromDiagramBlock");
                    }
                    // else if(connection.fromEdgeSheet > 0 || connection.toEdgeSheet > 0) {
                        //  Ignore connections from to or from edge signals
                    // }
                    PinDetail pinDetail = 
                        detail.pinList.Find(x => x.pin == connection.fromPin);
                    if(pinDetail == null) {
                        logDebug(2, "DEBUG: Adding pin Detail for Card type " +
                            Helpers.getCardTypeType(detail.diagramBlock.cardType) +
                            " FROM pin " + connection.fromPin);
                        pinDetail = new PinDetail();
                        detail.pinList.Add(pinDetail);

                        //  Track down the pin
                        List<Gatepin> gatePinList = gatePinTable.getWhere(
                            "WHERE cardGate = '" + detail.diagramBlock.cardGate +
                            "' AND pin = '" + connection.fromPin + "'");
                        if(gatePinList.Count != 1) {
                            logDebug(2, "DEBUG: Card type " +
                                Helpers.getCardTypeType(detail.diagramBlock.cardType) +
                                " Gate (" + detail.diagramBlock.cardGate + ")" +
                                " Pin " + connection.fromPin + " returned " +
                                gatePinList.Count.ToString() + " matches [expected 1]");
                        }
                        if (gatePinList.Count == 1) {
                            Gatepin gatePin = gatePinList[0];
                            pinDetail.input = (gatePin.input > 0);
                            pinDetail.output = (gatePin.output > 0);
                            if(pinDetail.input) {
                                logDebug(2, "DEBUG:   Pin is INPUT");
                            }
                            if(pinDetail.output) {
                                logDebug(2, "DEBUG:   Pin is OUTPUT");
                            }
                        }

                        pinDetail.pin = connection.fromPin;
                        pinDetail.loadPins = 0;
                        pinDetail.connectionsFrom = new List<Connection>();
                        pinDetail.connectionsTo = new List<Connection>();
                    }
                    if (connection.fromLoadPin != null && connection.fromLoadPin.Length > 0) {
                        ++pinDetail.loadPins;
                    }
                    logDebug(2, "DEBUG: Adding connection FROM pin " + connection.fromPin);
                    pinDetail.connectionsFrom.Add(connection);
                }

                if (connection.toDiagramBlock > 0) {
                    BlockDetail detail =
                        (BlockDetail)diagramBlockHash[connection.toDiagramBlock];
                    if (detail == null) {
                        logConnection(connection);
                        logMessage("   Error:  Invalid toDiagramBlock");
                    }
                    // else if (connection.fromEdgeSheet > 0 || connection.toEdgeSheet > 0) {
                        //  Ignore connections from to or from edge signals
                    // }
                    PinDetail pinDetail =
                        detail.pinList.Find(x => x.pin == connection.toPin);
                    if (pinDetail == null) {
                        logDebug(2, "DEBUG: Adding pin Detail for card type " +
                             Helpers.getCardTypeType(detail.diagramBlock.cardType) +
                             " TO pin " + connection.toPin);

                        pinDetail = new PinDetail();
                        detail.pinList.Add(pinDetail);

                        //  Track down the pin
                        List<Gatepin> gatePinList = gatePinTable.getWhere(
                            "WHERE cardGate = '" + detail.diagramBlock.cardGate +
                            "' AND pin = '" + connection.toPin + "'");

                        if (gatePinList.Count != 1) {
                            logDebug(2, "Card type " +
                                Helpers.getCardTypeType(detail.diagramBlock.cardType) +
                                " Gate (" + detail.diagramBlock.cardGate + ")" +
                                " Pin " + connection.fromPin + " returned " +
                                gatePinList.Count.ToString() + " matches [expected 1]");
                        }

                        if (gatePinList.Count == 1) {
                            Gatepin gatePin = gatePinList[0];
                            pinDetail.input = (gatePin.input > 0);
                            pinDetail.output = (gatePin.output > 0);
                            if(pinDetail.input) {
                                logDebug(2, "DEBUG:   pin is INPUT");
                            }
                            if(pinDetail.output) {
                                logDebug(2, "DEBUG:   pin is OUTPUT");
                            }
                        }

                        pinDetail.pin = connection.toPin;
                        pinDetail.loadPins = 0;
                        pinDetail.connectionsFrom = new List<Connection>();
                        pinDetail.connectionsTo = new List<Connection>();
                    }
                    logDebug(2, "DEBUG: Adding connection TO pin " + connection.toPin);
                    pinDetail.connectionsTo.Add(connection);
                }
            }

            //  Now spin through all of the logic blocks we just assembled, to check
            //  for anomolous connections - in page order.

            foreach(BlockDetail detail in diagramBlockHash.Values) {
                if(!diagramBlocksByPage.Contains(detail.page)) {
                    diagramBlocksByPage.Add(detail.page, new List<BlockDetail>());                
                }
                ((List<BlockDetail>)diagramBlocksByPage[detail.page]).Add(detail);
            }

            sortedPages = new ArrayList(diagramBlocksByPage.Keys);
            sortedPages.Sort();

            foreach(string page in sortedPages) {

                logDebug(1, "Testing Logic blocks on page " + page);

                foreach(BlockDetail detail in (List<BlockDetail>)diagramBlocksByPage[page]) {

                    int inputsCount = 0;
                    int outputsCount = 0;

                    logDebug(1, "Testing Logic Block " +
                        getDiagramBlockInfo(detail.diagramBlock.idDiagramBlock));
                    logDebug(2, "   Pin list has " + detail.pinList.Count.ToString() +
                        " entries.");

                    Cardgate gate = (Cardgate)cardGateHash[detail.diagramBlock.cardGate];
                    if (gate == null) {
                        logMessage("Invalid card gate in connection from logic block, " +
                            getDiagramBlockInfo(detail.diagramBlock.idDiagramBlock));
                        logMessage("");
                        continue;
                    }

                    //  TODO:  If extended, merge with its pair to come up with a merged
                    //  pin list.  THIS MAY NOT BE THE RIGHT PLACE TO DO THAT - perhaps
                    //  do that up above?

                    foreach (PinDetail pinDetail in detail.pinList) {
                        bool toDotFunction = false;

                        inputsCount += pinDetail.connectionsTo.Count;
                        outputsCount += pinDetail.connectionsFrom.Count;

                        logDebug(2, "DEBUG: Pin " + pinDetail.pin + " of logic block " +
                            getDiagramBlockInfo(detail.diagramBlock.idDiagramBlock));
                        logDebug(2, "   With " + pinDetail.connectionsTo.Count.ToString() +
                            " inputs and " + pinDetail.connectionsFrom.Count.ToString() +
                            " outputs");

                        //  An input only pin should not be an output, and vice versa

                        if(pinDetail.connectionsFrom.Count > 0 && !pinDetail.output) {
                            string msgtype = pinDetail.input ? "input only" : "incorrect";
                            logMessage("Outputs from " + msgtype + " pin " +
                                pinDetail.pin + ", " +
                                getDiagramBlockInfo(detail.diagramBlock.idDiagramBlock));
                        }

                        if(pinDetail.connectionsTo.Count > 0 && !pinDetail.input) {
                            string msgtype = pinDetail.output ? "output only" : "incorrect";
                            logMessage("Input to " + msgtype + " pin " +
                                pinDetail.pin + ", " +
                                getDiagramBlockInfo(detail.diagramBlock.idDiagramBlock));
                        }

                        //  Flag any connections that are not open collector with a load
                        //  pin specified.

                        if (gate.openCollector == 0 && pinDetail.loadPins > 0) {
                            logMessage("Non open collector output with load pin from " +
                                "logic block pin " + pinDetail.pin + ", " +
                                getDiagramBlockInfo(detail.diagramBlock.idDiagramBlock));
                        }

                        //  Note if this gate has any connections to DOT functions

                        foreach (Connection connection in pinDetail.connectionsFrom) {
                            if(connection.toDotFunction > 0) {
                                toDotFunction = true;
                            }
                        }

                        //  A block should not connect to anything else if it connects
                        //  to a DOT function.

                        if(toDotFunction && pinDetail.connectionsFrom.Count > 1) {
                            logMessage("Outputs to DOT function and other outputs " +
                                "from logic block pin " + pinDetail.pin + ", " + 
                                getDiagramBlockInfo(detail.diagramBlock.idDiagramBlock));
                        }

                        //  A given input pin should only have one input.

                        if(pinDetail.connectionsTo.Count > 1) {
                            logMessage("Inputs from more than one output " +
                                "to logic block pin " + pinDetail.pin + ", " +
                                getDiagramBlockInfo(detail.diagramBlock.idDiagramBlock));
                        }
                    }

                    //  Note if this block has no inputs or no outputs (except that
                    //  if a diagram block is symbol "L" for load, or "LAMP", 
                    //  no outputs is OK)

                    if(inputsCount == 0) {
                        logMessage("NO inputs to logic block at " +
                            getDiagramBlockInfo(detail.diagramBlock.idDiagramBlock));
                    }
                    if(outputsCount == 0 && detail.diagramBlock.symbol != "L"  &&
                        detail.diagramBlock.symbol != "LAMP") {
                        logMessage("NO outputs from logic block at " +
                            getDiagramBlockInfo(detail.diagramBlock.idDiagramBlock));
                    }

                }
            }

            logMessage("*** End of Logic Block Connection Check ***");
            logMessage("");

            logMessage("End of Report.");

            Parms.setParmValue("machine", currentMachine.idMachine.ToString());
            Parms.setParmValue("report output directory", directoryTextBox.Text);
            Parms.setParmValue("connection report log level", logLevel.ToString());
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
                    message += "DOT Function " + 
                        getDotFunctionInfo(connection.fromDotFunction);
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

            Diagramblock diagramBlock =
                ((BlockDetail)diagramBlockHash[diagramBlockKey]).diagramBlock;
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

        //  Write a message for debug, depending up on log level

        private void logDebug(int level, string message) {
            if(level <= logLevel) {
                logMessage(message);
            }
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
