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

        //  Class to store information about a given card slot as used
        //  in an ALD

        class CardGateDetail
        {
            public List<string> pageUsage = new List<string>();
        }
       
        class CardSlotTypeDetail
        {
            public List<string> cardTypes = new List<string>();
            public List<string> firstPage = new List<string>();
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
        Table<Cardslot> cardSlotTable;

        List<Machine> machineList;

        Dictionary<int, Cardgate> cardGateDict = new Dictionary<int, Cardgate>();
        Dictionary<int, Cardtype> cardTypeDict = new Dictionary<int, Cardtype>();
        Dictionary<int, Logicfamily> logicFamilyDict = 
            new Dictionary<int, Logicfamily>();

        Dictionary<int,Connection> connectionDict = null;
        Dictionary<int,Diagrampage> diagramPageDict = null;
        Dictionary<int,string> pageDict = null;
        Dictionary<int,Dotfunction> dotFunctionDict = null;
        Dictionary<int,BlockDetail> diagramBlockDict = null;
        Dictionary<int,DotDetail> dotDetailDict = null;
        Dictionary<int,Sheetedgeinformation> sheetEdgeDict = null;
        Dictionary<string,CardGateDetail> cardSlotGateDict = null;
        Dictionary<string, CardSlotTypeDetail> cardSlotUsageDict = null;
   
        Machine currentMachine = null;
        Boolean reporting = false;          //  If true, the working label will blink

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
            cardSlotTable = db.getCardSlotTable();

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
                cardGateDict.Add(gate.idcardGate, gate);
            }

            //  Same for the card types

            List<Cardtype> cardTypeList = cardTypeTable.getAll();
            foreach(Cardtype ct in cardTypeList) {
                cardTypeDict.Add(ct.idCardType, ct);
            }

            //  And logic families

            List<Logicfamily> logicFamilyList = logicFamilyTable.getAll();
            foreach (Logicfamily lf in logicFamilyList) {
                logicFamilyDict.Add(lf.idLogicFamily, lf);
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

            pageDict = new Dictionary<int,string>();
            diagramPageDict = new Dictionary<int, Diagrampage>();
            dotFunctionDict = new Dictionary<int, Dotfunction>();
            diagramBlockDict = new Dictionary<int, BlockDetail>();
            connectionDict = new Dictionary<int, Connection>();
            dotDetailDict = new Dictionary<int, DotDetail>();
            sheetEdgeDict = new Dictionary<int, Sheetedgeinformation>();
            cardSlotGateDict = new Dictionary<string, CardGateDetail>();
            cardSlotUsageDict = new Dictionary<string, CardSlotTypeDetail>();

            Hashtable dotFunctionsByPage = new Hashtable();
            Hashtable diagramBlocksByPage = new Hashtable();

            ArrayList sortedPages = null;

            List<DotDetail> dotDetalList = new List<DotDetail>();

            //  Build a hash of the pages related to this machine

            List<Page> pageList = pageTable.getAll();
            foreach(Page page in pageList) {
                if(page.machine == currentMachine.idMachine) {
                    pageDict.Add(page.idPage, page.name);
                }
            }

            //  Having that, build a hash of related diagram pages

            List<Diagrampage> diagramPageList = diagramPageTable.getAll();
            foreach(Diagrampage dp in diagramPageList) {
                if(pageDict.ContainsKey(dp.page)) {
                    diagramPageDict.Add(dp.idDiagramPage, dp);
                }
            }

            //  Having that, build a list of related connections and DOT functions
            //  Also build a list of relevant connections

            List<Dotfunction> dotFunctionList = dotFunctionTable.getAll();
            foreach(Dotfunction df in dotFunctionList) {
                if(diagramPageDict.ContainsKey(df.diagramPage)) {
                    dotFunctionDict.Add(df.idDotFunction, df);
                }
            }

            //  And, also a list of related diagram blocks.  While we are at it,
            //  Fill in the dictionary of CardSlot:Gate values with pages that use
            //  that gate ("There can only be one"), and card types, by slot.

            List<Diagramblock> diagramBlockList = diagramBlockTable.getWhere(
                "ORDER BY diagramColumn, diagramRow");
            foreach(Diagramblock diagramBlock in diagramBlockList) {
                Diagrampage dp;
                CardGateDetail cardGateDetail;
                string cardSlotString = "";

                if (diagramPageDict.TryGetValue(diagramBlock.diagramPage, out dp)) {

                    BlockDetail detail = new BlockDetail();
                    detail.diagramBlock = diagramBlock;
                    detail.page = pageDict[dp.page];
                    diagramBlockDict.Add(diagramBlock.idDiagramBlock, detail);
                    CardSlotInfo cardSlot = Helpers.getCardSlotInfo(
                        diagramBlock.cardSlot);
                    cardSlotString = cardSlot.ToString();

                    CardSlotTypeDetail cardSlotTypeDetail;
                    string cardTypeType = cardTypeDict[diagramBlock.cardType].type;
                    if (!cardSlotUsageDict.TryGetValue(cardSlotString,
                            out cardSlotTypeDetail)) {
                        cardSlotTypeDetail = new CardSlotTypeDetail();
                        cardSlotTypeDetail.cardTypes = new List<string>();
                        cardSlotTypeDetail.firstPage = new List<string>();
                        cardSlotUsageDict.Add(cardSlotString, cardSlotTypeDetail);
                    }
                    if (!cardSlotTypeDetail.cardTypes.Contains(cardTypeType)) {
                        cardSlotTypeDetail.cardTypes.Add(cardTypeType);
                        cardSlotTypeDetail.firstPage.Add(detail.page);
                    }

                    //  (If a gate has a symbol of "E" and an extension field, 
                    //  ignore it for card gate re-use checks.)

                    if (diagramBlock.symbol == "E" && diagramBlock.extendedTo != 0) {
                        continue;
                    }

                    Cardgate cardGate = cardGateTable.getByKey(
                        diagramBlock.cardGate);
                    if(cardGate.number == 0) {
                        logMessage("Invalid gate number of 0 " +
                            getDiagramBlockInfo(diagramBlock.idDiagramBlock));
                    }

                    string cardGateKey = 
                        cardSlotString + ":" + cardGate.number.ToString();
                    if(!cardSlotGateDict.TryGetValue(cardGateKey,
                        out cardGateDetail)) {
                        cardGateDetail = new CardGateDetail();
                        cardGateDetail.pageUsage = new List<string>();
                        cardSlotGateDict.Add(cardGateKey, cardGateDetail);
                    }

                    // if(!cardGateDetail.pageUsage.Contains(detail.page)) {
                        cardGateDetail.pageUsage.Add(detail.page);
                    // }        
                }
            }

            //  Report any cases where a given card gate appears twice in the
            //  ALD's, and that any cases where the card type isn't the same.

            logMessage("*** Begin Duplicate Card Slot:Gate usage Report.");
            ArrayList cardSlotGateArrayList = new ArrayList(
                cardSlotGateDict.Keys);
            cardSlotGateArrayList.Sort();
            foreach(string cardSlotGate in cardSlotGateArrayList) {
                CardGateDetail cardGateDetail = cardSlotGateDict[cardSlotGate];
                bool reportPages = false;

                if(cardGateDetail.pageUsage.Count != 1) {
                    logMessage("Unexpected card gate use count of " +
                        cardGateDetail.pageUsage.Count.ToString() + 
                        " for cardslot:gate of " + cardSlotGate);
                    reportPages = true;
                }

                if(reportPages) {
                    string pageNames = "";
                    for (int i = 0; i < cardGateDetail.pageUsage.Count; ++i) {
                        pageNames += (i > 0 ? ", " : "") +
                            cardGateDetail.pageUsage[i];
                    }
                    if (pageNames.Length > 0) {
                        logMessage("   Pages: " + pageNames);
                    }
                    logMessage("");
                }
            }

            ArrayList cardSlotArrayList = new ArrayList(cardSlotUsageDict.Keys);
            cardSlotArrayList.Sort();
            foreach(string cardSlotName in cardSlotArrayList) {
                CardSlotTypeDetail cardSlotTypeDetail = 
                    cardSlotUsageDict[cardSlotName];
                if (cardSlotTypeDetail.cardTypes.Count != 1) {
                    logMessage("Duplicate or missing card types numbering " +
                        cardSlotTypeDetail.cardTypes.Count.ToString() +
                        " for cardslot " + cardSlotName);
                    string cardTypes = "";
                    for (int i = 0; i < cardSlotTypeDetail.cardTypes.Count; ++i) {
                        cardTypes += (i > 0 ? ", " : "") +
                            cardSlotTypeDetail.cardTypes[i] +
                            " (Page " + cardSlotTypeDetail.firstPage[i] + ")";
                    }
                    if (cardTypes.Length > 0) {
                        logMessage("   Types (first pages): " + cardTypes);
                        logMessage("");
                    }
                }
            }



            logMessage("*** End Duplicate Card Slot:Gate usage Report.");
            logMessage("");

            //  A list of sheet edge information signals, too...

            List<Sheetedgeinformation> sheetEdgeInformationList =
                sheetEdgeInformationTable.getAll();
            foreach(Sheetedgeinformation se in sheetEdgeInformationList) {
                if(diagramPageDict.ContainsKey(se.diagramPage)) {
                    sheetEdgeDict.Add(se.idSheetEdgeInformation, se);
                }
            }


            //  And, finally, a list of related connections (exluding sheet edge
            //  connections, which are handled in the Signals report.)

            List<Connection> connectionList = connectionTable.getAll();
            foreach (Connection connection in connectionList) {
                if(diagramBlockDict.ContainsKey(connection.fromDiagramBlock) ||
                   dotFunctionDict.ContainsKey(connection.fromDotFunction) ||
                   diagramBlockDict.ContainsKey(connection.toDiagramBlock) ||
                   dotFunctionDict.ContainsKey(connection.toDotFunction) ||
                   sheetEdgeDict.ContainsKey(connection.fromEdgeSheet) ||
                   sheetEdgeDict.ContainsKey(connection.toEdgeSheet)) {
                    connectionDict.Add(connection.idConnection, connection);
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

            foreach(int connectionKey in connectionDict.Keys) {
                Connection connection = connectionDict[connectionKey];
                BlockDetail diagramBlockDetail;

                if (connection.toDotFunction > 0) {

                    Dotfunction dotFunction;
                    if(!dotFunctionDict.TryGetValue(connection.toDotFunction,
                        out dotFunction)) {
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
                    else if(!diagramBlockDict.TryGetValue(connection.fromDiagramBlock,
                        out diagramBlockDetail)) {
                        logConnection(connection);
                        logMessage("   Connection to DOT function from invalid diagram block (" +
                            connection.fromDiagramBlock + ")");

                    }
                    else {

                        //  At this point, diagramBlockDetail is filled in!

                        Diagramblock diagramBlock = diagramBlockDetail.diagramBlock;
                        DotDetail dotDetail;

                        //  If there is not already a detail entry for this DOT function,
                        //  create one now.

                        if(!dotDetailDict.TryGetValue(dotFunction.idDotFunction,
                            out dotDetail)) {
                            dotDetail = new DotDetail();
                            dotDetail.dotFunctionKey = dotFunction.idDotFunction;
                            dotDetail.page = getDiagramPageName(dotFunction.diagramPage);
                            dotDetail.row = dotFunction.diagramRowTop;
                            dotDetail.column = dotFunction.diagramColumnToLeft.ToString();
                            dotDetail.logicFunction = dotFunction.logicFunction;
                            dotDetail.connections = new List<DotConnection>();
                            dotDetail.hasEdgeOutput = false;
                            dotDetail.hasGateOutput = false;
                            dotDetailDict.Add(dotFunction.idDotFunction, dotDetail);
                        }

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
                        dotFunctionDict[connection.fromDotFunction];
                    DotDetail dotDetail;

                    //  Might be a new DOT function we have to add.

                    if (!dotDetailDict.TryGetValue(dotFunction.idDotFunction,
                        out dotDetail)) {
                        dotDetail = new DotDetail();
                        dotDetail.dotFunctionKey = dotFunction.idDotFunction;
                        dotDetail.page = getDiagramPageName(dotFunction.diagramPage);
                        dotDetail.row = dotFunction.diagramRowTop;
                        dotDetail.column = dotFunction.diagramColumnToLeft.ToString();
                        dotDetail.logicFunction = dotFunction.logicFunction;
                        dotDetail.connections = new List<DotConnection>();
                        dotDetail.hasEdgeOutput = false;
                        dotDetail.hasGateOutput = false;
                        dotDetailDict.Add(dotFunction.idDotFunction, dotDetail);
                    }

                    if(connection.toEdgeSheet > 0) {
                        dotDetail.hasEdgeOutput = true;
                    }
                    if(connection.toDiagramBlock > 0) {
                        dotDetail.hasGateOutput = true;
                    }
                }
            }

            //  Now spin through all of the DOT functions we just assembled, to see
            //  if there are any cases where we have multiple loads (i.e., loads +
            //  NON open collector gates > 1)

            //  Report them in page order!

            foreach(DotDetail detail in dotDetailDict.Values) {
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
                    BlockDetail diagramBlockDetail;

                    List<DotConnection> connections = detail.connections;
                    foreach (DotConnection dotConnection in connections) {

                        //  Is a load pin involved?  If so, count it.

                        if (dotConnection.fromLoadPin.Length > 0) {
                            ++loadCount;
                        }

                        //  Is the gate open collector?  If not, count it.

                        if(!diagramBlockDict.TryGetValue(dotConnection.fromDiagramBlock,
                            out diagramBlockDetail)) {
                            logConnection(dotConnection.connection);
                            logMessage("   Internal Error: DotConnection Diagram Block not " +
                                "found (" + dotConnection.fromDiagramBlock.ToString() + ")");
                            continue;
                        }
                        Diagramblock diagramBlock = diagramBlockDetail.diagramBlock;

                        Cardgate gate;
                        if(!cardGateDict.TryGetValue(dotConnection.fromGate, out gate)) {
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

            //  Sometimes a block has an extension.  If a given pin doesn't match
            //  On a given block, check its extension.

            foreach(int connectionKey in connectionDict.Keys) {

                //  Give up control                

                Connection connection = connectionDict[connectionKey];

                if ((connection.fromPin == "--" ^ connection.toPin == "--") &&
                    connection.fromDiagramBlock > 0 && connection.toDiagramBlock > 0) {
                    logConnection(connection);
                    logMessage("Block to Block connection has only one internal " +
                        "pin of \"--\" [Expect both]");
                }

                if(connection.fromDiagramBlock > 0) {
                    BlockDetail fromDetail;
                    BlockDetail fromExtendedDetail;

                    logDebug(2, "DEBUG: FROM Logic block "     +
                            getDiagramBlockInfo(connection.fromDiagramBlock));

                    if(!diagramBlockDict.TryGetValue(connection.fromDiagramBlock,
                        out fromDetail)) {
                        logConnection(connection);
                        logMessage("   Error:  Invalid fromDiagramBlock");
                        continue;
                    }

                    // if(connection.fromEdgeSheet > 0 || connection.toEdgeSheet > 0) {
                        //  Ignore connections from to or from edge signals
                        //continue;
                    // }

                    PinDetail pinDetail = 
                        fromDetail.pinList.Find(x => x.pin == connection.fromPin);
                    if(pinDetail == null) {
                        logDebug(2, "DEBUG: Adding pin Detail for Card type " +
                            Helpers.getCardTypeType(fromDetail.diagramBlock.cardType) +
                            " FROM pin " + connection.fromPin);
                        pinDetail = new PinDetail();
                        fromDetail.pinList.Add(pinDetail);

                        //  Track down the pin
                        List<Gatepin> gatePinList = gatePinTable.getWhere(
                            "WHERE cardGate = '" + fromDetail.diagramBlock.cardGate +
                            "' AND pin = '" + connection.fromPin + "'");

                        //  If there was not exactly one match report it unless
                        //  there were no matches and it is an internal connectin (pin "--")

                        if(gatePinList.Count != 1 &&
                            !(gatePinList.Count == 0 && connection.fromPin == "--")) {
                            if(logLevel >= 2) {
                                logConnection(connection);
                            }
                            logDebug(2, "DEBUG: Card type " +
                                Helpers.getCardTypeType(fromDetail.diagramBlock.cardType) +
                                " Gate (" + fromDetail.diagramBlock.cardGate + ")" +
                                " Pin " + connection.fromPin + " returned " +
                                gatePinList.Count.ToString() + " matches [expected 1]");
                        }

                        //  If no match was found, and this gate has an extension, 
                        //  use the information from the extension to decide if this
                        //  pin is an input or an output (and therefore valid.)

                        if(gatePinList.Count == 0 && 
                            connection.fromPin != "--" && 
                            fromDetail.diagramBlock.extendedTo != 0) {
                            logDebug(2, "DEBUG: Trying extended block for FROM connection.");
                            if(!diagramBlockDict.TryGetValue(fromDetail.diagramBlock.extendedTo,
                                out fromExtendedDetail)) {
                                logConnection(connection);
                                logMessage("   Error:  Invalid ExtendedTo in fromDiagramBlock");
                            }
                            else {
                                gatePinList = gatePinTable.getWhere(
                                    "WHERE cardGate = '" + 
                                    fromExtendedDetail.diagramBlock.cardGate +
                                    "' AND pin = '" + connection.fromPin + "'");
                                if (gatePinList.Count != 1) {
                                    if(logLevel >= 2) {
                                        logConnection(connection);
                                    }
                                    logDebug(2, "DEBUG: Extended Block Card type " +
                                        Helpers.getCardTypeType(
                                            fromExtendedDetail.diagramBlock.cardType) +
                                        " Gate (" + fromExtendedDetail.diagramBlock.cardGate + ")" +
                                        " Pin " + connection.fromPin + " returned " +
                                        gatePinList.Count.ToString() + " matches [expected 1]");
                                }
                                else {
                                    logDebug(2, "DEBUG: Found 1 match on extended block.");
                                }
                            }
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

                        //  If the pin is not --, just use its name.  Otherwise,
                        //  if it is --, qualify the name with the logic block on
                        //  the OTHER end of the connection.

                        pinDetail.pin = connection.fromPin;
                        if (connection.fromPin == "--") {
                            pinDetail.pin += connection.toDiagramBlock.ToString();
                            pinDetail.output = true;
                            pinDetail.input = false;
                        }
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
                    BlockDetail toDetail;
                    BlockDetail toExtendedDetail;

                    logDebug(2, "DEBUG: TO Logic block " +
                        getDiagramBlockInfo(connection.toDiagramBlock));

                    if(!diagramBlockDict.TryGetValue(connection.toDiagramBlock,
                        out toDetail)) {
                        logConnection(connection);
                        logMessage("   Error:  Invalid toDiagramBlock");
                        continue;
                    }
                    // else if (connection.fromEdgeSheet > 0 || connection.toEdgeSheet > 0) {
                        //  Ignore connections from to or from edge signals
                    // }
                    PinDetail pinDetail =
                        toDetail.pinList.Find(x => x.pin == connection.toPin);
                    if (pinDetail == null) {
                        logDebug(2, "DEBUG: Adding pin Detail for card type " +
                             Helpers.getCardTypeType(toDetail.diagramBlock.cardType) +
                             " TO pin " + connection.toPin);

                        pinDetail = new PinDetail();
                        toDetail.pinList.Add(pinDetail);

                        //  Track down the pin
                        List<Gatepin> gatePinList = gatePinTable.getWhere(
                            "WHERE cardGate = '" + toDetail.diagramBlock.cardGate +
                            "' AND pin = '" + connection.toPin + "'");

                        if (gatePinList.Count != 1) {
                            logDebug(2, "Card type " +
                                Helpers.getCardTypeType(toDetail.diagramBlock.cardType) +
                                " Gate (" + toDetail.diagramBlock.cardGate + ")" +
                                " Pin " + connection.fromPin + " returned " +
                                gatePinList.Count.ToString() + " matches [expected 1]");
                        }

                        //  If there was not exactly one match report it unless
                        //  there were no matches and it is an internal connectin (pin "--")

                        if (gatePinList.Count != 1 &&
                            !(gatePinList.Count == 0 && connection.toPin == "--")) {
                            if (logLevel >= 2) {
                                logConnection(connection);
                            }
                            logDebug(2, "DEBUG: Card type " +
                                Helpers.getCardTypeType(toDetail.diagramBlock.cardType) +
                                " Gate (" + toDetail.diagramBlock.cardGate + ")" +
                                " Pin " + connection.fromPin + " returned " +
                                gatePinList.Count.ToString() + " matches [expected 1]");
                        }

                        //  If no match was found, and this gate has an extension, 
                        //  use the information from the extension to decide if this
                        //  pin is an input or an output (and therefore valid.)

                        if (gatePinList.Count == 0 &&
                            connection.toPin != "--" &&
                            toDetail.diagramBlock.extendedTo != 0) {
                            logDebug(2, "DEBUG: Trying extended block for TO connection.");
                            if(!diagramBlockDict.TryGetValue(toDetail.diagramBlock.extendedTo,
                                out toExtendedDetail)) {
                                logConnection(connection);
                                logMessage("   Error:  Invalid ExtendedTo in toDiagramBlock");
                            }
                            else {
                                gatePinList = gatePinTable.getWhere(
                                    "WHERE cardGate = '" +
                                    toExtendedDetail.diagramBlock.cardGate +
                                    "' AND pin = '" + connection.toPin + "'");
                                if (gatePinList.Count != 1) {
                                    if (logLevel >= 2) {
                                        logConnection(connection);
                                    }
                                    logDebug(2, "DEBUG: Extended Block Card type " +
                                        Helpers.getCardTypeType(
                                            toExtendedDetail.diagramBlock.cardType) +
                                        " Gate (" + toExtendedDetail.diagramBlock.cardGate + ")" +
                                        " Pin " + connection.toPin + " returned " +
                                        gatePinList.Count.ToString() + " matches [expected 1]");
                                }
                                else {
                                    logDebug(2, "DEBUG: Found 1 match on extended block.");
                                }
                            }
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

                        //  If the pin is not --, just use its name.  Otherwise,
                        //  if it is --, qualify the name with the logic block on
                        //  the OTHER end of the connection.

                        pinDetail.pin = connection.toPin;
                        if (connection.toPin == "--") {
                            pinDetail.pin += connection.toDiagramBlock.ToString();
                            pinDetail.output = false;
                            pinDetail.input = true;
                        }

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

            foreach(BlockDetail detail in diagramBlockDict.Values) {
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

                    Cardgate gate;
                    if (!cardGateDict.TryGetValue(detail.diagramBlock.cardGate,
                        out gate)) {
                        logMessage("Invalid card gate in connection from logic block, " +
                            getDiagramBlockInfo(detail.diagramBlock.idDiagramBlock));
                        logMessage("");
                        continue;
                    }

                    //  Count inputs and outputs from any extension block

                    if (detail.diagramBlock.extendedTo != 0) {
                        BlockDetail extendedDetail =
                            (BlockDetail)diagramBlockDict[detail.diagramBlock.extendedTo];
                        if (extendedDetail != null) {
                            foreach (PinDetail extendedPinDetail in extendedDetail.pinList) {
                                inputsCount += extendedPinDetail.connectionsTo.Count;
                                outputsCount += extendedPinDetail.connectionsFrom.Count;
                            }
                        }
                    }

                    //  Then look at this block itself in more detail

                    foreach (PinDetail pinDetail in detail.pinList) {
                        bool toDotFunction = false;

                        inputsCount += pinDetail.connectionsTo.Count;
                        outputsCount += pinDetail.connectionsFrom.Count;

                        logDebug(2, "DEBUG: Pin " + pinDetail.pin + " of logic block " +
                            getDiagramBlockInfo(detail.diagramBlock.idDiagramBlock));
                        logDebug(2, "   With " + pinDetail.connectionsTo.Count.ToString() +
                            " inputs and " + pinDetail.connectionsFrom.Count.ToString() +
                            " outputs");

                        //  An input only pin should not be an output, (and the pin is not
                        //  "--"), and vice versa

                        if(pinDetail.connectionsFrom.Count > 0 && !pinDetail.output &&
                            (pinDetail.input || !pinDetail.pin.StartsWith("--"))) {
                            string msgtype = pinDetail.input ? "input only" : "incorrect";
                            logMessage("Outputs from " + msgtype + " pin " +
                                pinDetail.pin + ", " +
                                getDiagramBlockInfo(detail.diagramBlock.idDiagramBlock));
                        }

                        if(pinDetail.connectionsTo.Count > 0 && !pinDetail.input &&
                            (pinDetail.output || !pinDetail.pin.StartsWith("--"))) {
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
                    //  or "R" for resistor, no outputs is OK)

                    if(inputsCount == 0) {
                        logMessage("NO inputs to logic block at " +
                            getDiagramBlockInfo(detail.diagramBlock.idDiagramBlock));
                    }

                    //  Maybe the following should look at the card/gate characteristics,
                    //  instead of the ALD symbol??

                    if(outputsCount == 0 && detail.diagramBlock.symbol != "L"  &&
                        detail.diagramBlock.symbol != "LAMP"  &&
                        detail.diagramBlock.symbol != "R" &&
                        detail.diagramBlock.symbol != "CAP") {
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
                    if(connection.fromLoadPin != null && connection.fromLoadPin.Length > 0) {
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
                ((BlockDetail)diagramBlockDict[diagramBlockKey]).diagramBlock;
            if(diagramBlock == null) {
                return ("Invalid Diagram Block (" + diagramBlockKey.ToString() + ")");
            }            
            info += getDiagramPageName(diagramBlock.diagramPage) + ", row " + 
                diagramBlock.diagramRow + ", column " +  diagramBlock.diagramColumn.ToString();
            return (info);
        }

        private string getDiagramPageName(int diagramPageKey) {
            string page;
            Diagrampage diagramPage;

            if(!diagramPageDict.TryGetValue(diagramPageKey, out diagramPage)) {
                return (" Invalid Diagram page (" + diagramPageKey.ToString() + ")");
            }
            
            if(pageDict.TryGetValue(diagramPage.page,out page)) {
                return pageDict[diagramPage.page];
            }
            else {
                return(" Invalid Page (" + diagramPage.page.ToString() + ")");
            }

        }

        //  Return information on a dot function connection

        private string getDotFunctionInfo(int dotFunctionKey) {
            string info = "";
            Diagrampage diagramPage;
            Dotfunction dotFunction;

            if(!dotFunctionDict.TryGetValue(dotFunctionKey, out dotFunction)) {
                return ("Invalid DOT Function (" + dotFunctionKey.ToString() + ")");
            }

            if(diagramPageDict.TryGetValue(dotFunction.diagramPage, out diagramPage)) {
                info += "Page " + getDiagramPageName(diagramPage.idDiagramPage);
            }
            else {
                info += "(Invalid page) ";
            }
            info += ", row " + dotFunction.diagramRowTop + ", column " +
                dotFunction.diagramColumnToLeft.ToString();
            return (info);

        }

        //  Determine if a card type is a switch

        private bool isCardASwitch(int cardTypeKey) {
            Cardtype type;
            if(!cardTypeDict.TryGetValue(cardTypeKey, out type)) { 
                return false;
            }
            Logicfamily family;
            if(logicFamilyDict.TryGetValue(type.logicFamily,out family)) {
                return (family.name == "SWITCH");
            }
            else {
                return false;
            }
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

        private async void reportButton_Click(object sender, EventArgs e) {
            if(!reportButton.Enabled) {
                return;
            }
            reportButton.Enabled = false;
            reporting = true;
            workingLabel.Visible = true;

            logFileName = Path.Combine(directoryTextBox.Text,
                currentMachine.name + "-ConnectionReport.txt");
            logFile = new StreamWriter(logFileName, false);
            logLevel = (int)logLevelComboBox.SelectedItem;

            Parms.setParmValue("report output directory", directoryTextBox.Text);
            Parms.setParmValue("connection report log level", logLevel.ToString());

            logMessage("Connection Report for Machine: " +
                currentMachine.name);


            workingBlink();
            await Task.Run(() => doConnectionReport());


            //  Show a box, maybe, eventually.

            logFile.Close();
            reporting = false;
            workingLabel.Visible = false;
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

        //  Routine to blink the working label

        private async void workingBlink() {
            while(reporting) {
                if(InvokeRequired) {
                    Invoke((Action)workingBlink);
                }
                else {
                    workingLabel.Visible = !workingLabel.Visible;
                }
                await Task.Delay(750);
            }
        }
    }
}
