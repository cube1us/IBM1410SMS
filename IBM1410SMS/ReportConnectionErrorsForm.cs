using MySQLFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IBM1410SMS
{
    public partial class ReportConnectionErrorsForm : Form
    {

        //  Class to store information about a connection to a DOT function

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
            public bool positive = false;
            public bool negative = false;
            public bool nullPolarity = false;
            public List<Connection> connectionsFrom { get; set; } = new List<Connection>();
            public List<Connection> connectionsTo { get; set; } = new List<Connection>();
        }

        //  Class to store information about a Logic Block

        class BlockDetail
        {
            public Diagramblock diagramBlock { get; set; } = null;
            public string page { get; set; } = "";
            public List<PinDetail> pinList { get; set; }  = new List<PinDetail>();
            public int gateNumber = 0;
            public string cardTypeName = "";
            public bool countedPositiveOuput = false;
            public bool countedNegativeOuput = false;
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

        //  Class to store informationa about how a given gate in a given card type 
        //  is used in the ALDS.  Key is CardTypeName:Gate#.  usage entries are by 
        //  SymbolName:PolarityOut

        class CardTypeGateDetail
        {
            public string logicFunction;
            public Dictionary<string, int> usage;
        }

        //  Table to allow matching the symbol on a logic block and its output sense
        //  (top of block is +, bottom of block is -) against the logic function
        //  defined in the card gate table.  (Eventually this is maybe moved to
        //  helpers, and used during editing?)

        public class LogicCheckEntry
        {
            public string logicFunction;   //  Special, DELAY, NAND, AND, NOR, OR ...
            public string symbol;          //  DLY, * (any)
            public string blockType;       //  and, or, invert, driver, * (any)
            public string firstChar;       //  +, -, "*" (any)
            public string outputSense;     //  + (top of block), - (bottom), * (any)

            public LogicCheckEntry(string logicFunction, string symbol, string blockType,
                string firstChar, string outputSense) {
                this.logicFunction = logicFunction;
                this.symbol = symbol;
                this.blockType = blockType;
                this.firstChar = firstChar;
                this.outputSense = outputSense;
            }
        };

        //  TODO: The following should probably really be in a database, as it
        //  would vary by machine.

        LogicCheckEntry[] logicCheckTable = new LogicCheckEntry[]
        {
             new LogicCheckEntry("NAND",     "*",   "and", "+", "+"),
             new LogicCheckEntry("NAND",     "*",   "or",  "-", "+"),
             new LogicCheckEntry("NOT",      "*",   "inv", "I", "+"),
             new LogicCheckEntry("NAND",     "*",   "inv", "*", "+"),   // NAND gate as Inverter
             new LogicCheckEntry("NOT",      "*",   "drv", "D", "+"),
             new LogicCheckEntry("NOT",      "*",   "and", "+", "+"),   // Inverter with +AA symbol
             new LogicCheckEntry("Trigger",  "T",   "*",   "T", "*"),
             new LogicCheckEntry("Resistor", "R",   "*",   "R", "*"),
             new LogicCheckEntry("Resistor", "L",   "*",   "L", "+"),
             new LogicCheckEntry("Capacitor","CAP", "*",   "C", "*"),
             new LogicCheckEntry("EQUAL",    "*",   "drv", "D", "-"),
             new LogicCheckEntry("EQUAL",    "DL",  "drv", "D", "+"),
             new LogicCheckEntry("EQUAL",    "DE",  "drv", "D", "+"),
             new LogicCheckEntry("AND",      "*",   "and", "+", "-"),
             new LogicCheckEntry("AND",      "*",   "or",  "-", "-"),
             new LogicCheckEntry("NOR",      "*",   "and", "-", "+"),
             new LogicCheckEntry("NOR",      "*",   "or",  "+", "+"),
             new LogicCheckEntry("OR",       "*",   "or",  "+", "-"),
             new LogicCheckEntry("OR",       "*",   "and", "-", "-"),
             new LogicCheckEntry("NOR",      "*",   "inv", "I", "+"),   // NOR gate as Inverter
             new LogicCheckEntry("Special",  "*",   "*",   "*", "*"),
             new LogicCheckEntry("DELAY",    "DLY", "*",   "D", "+"),
             new LogicCheckEntry("Clamp",    "L",   "*",   "L", "+"),
             new LogicCheckEntry("Lamp",     "LAMP","*",   "L", "+"),
             new LogicCheckEntry("Switch",   "TOG", "*",   "T", "*"),
             new LogicCheckEntry("Switch",   "ROT", "*",   "R", "+"),
             new LogicCheckEntry("Switch",   "MOM", "*",   "M", "*"),
             new LogicCheckEntry("SS",       "SS",  "*",   "S", "*"),
             new LogicCheckEntry("NAND",     "G",   "*",   "G", "+"),   // LS Matrix Switch Gate
             new LogicCheckEntry("LSS",      "LSS", "*",   "L", "*"),   // LS matrix Switch
             new LogicCheckEntry("Sense",    "AM",  "*",   "A", "+"),   // Sense Amp.
             new LogicCheckEntry("AND",      "*",   "drv", "D", "-"),   // Inhibit Driver
             new LogicCheckEntry("Trigger",  "TV",  "*",   "T", "*"),
             new LogicCheckEntry("*",        "E",   "*",   "*", "*"),   // Don't check extended blocks
        };

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
        Table<Logicfunction> logicFunctionTable;

        List<Machine> machineList;

        Dictionary<int, Cardgate> cardGateDict = new Dictionary<int, Cardgate>();
        Dictionary<int, Cardtype> cardTypeDict = new Dictionary<int, Cardtype>();
        Dictionary<int, Logicfamily> logicFamilyDict = 
            new Dictionary<int, Logicfamily>();
        Dictionary<int, string> logicFunctionDict = new Dictionary<int, string>();

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
            logicFunctionTable = db.getLogicFunctionTable();

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

            //  And even the logic functions

            List<Logicfunction> logicFunctionList = logicFunctionTable.getAll();
            foreach(Logicfunction lf in logicFunctionList) {
                logicFunctionDict.Add(lf.idLogicFunction, lf.name);
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

            logMessage("*** Begin Report on " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));

            pageDict = new Dictionary<int,string>();
            diagramPageDict = new Dictionary<int, Diagrampage>();
            dotFunctionDict = new Dictionary<int, Dotfunction>();
            diagramBlockDict = new Dictionary<int, BlockDetail>();
            connectionDict = new Dictionary<int, Connection>();
            dotDetailDict = new Dictionary<int, DotDetail>();
            sheetEdgeDict = new Dictionary<int, Sheetedgeinformation>();
            cardSlotGateDict = new Dictionary<string, CardGateDetail>();
            cardSlotUsageDict = new Dictionary<string, CardSlotTypeDetail>();

            Dictionary<string, List<DotDetail>> dotFunctionsByPage =
                new Dictionary<string, List<DotDetail>>();
            Dictionary<string, List<BlockDetail>> diagramBlocksByPage =
                new Dictionary<string, List<BlockDetail>>();
            Dictionary<int, int> internalEdgeConnDict = new Dictionary<int, int>();
            Dictionary<string, CardTypeGateDetail> cardTypeGateUsageDict =
                new Dictionary<string, CardTypeGateDetail>();

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
                    detail.countedPositiveOuput = false;
                    detail.countedNegativeOuput = false;
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

                    string cardGateNumber = cardGate.number.ToString();
                    string cardGateKey = cardSlotString + ":" + cardGateNumber;
                    if(!cardSlotGateDict.TryGetValue(cardGateKey,
                        out cardGateDetail)) {
                        cardGateDetail = new CardGateDetail();
                        cardGateDetail.pageUsage = new List<string>();
                        cardSlotGateDict.Add(cardGateKey, cardGateDetail);
                    }

                    // if(!cardGateDetail.pageUsage.Contains(detail.page)) {
                        cardGateDetail.pageUsage.Add(detail.page);
                    // }        

                    //  Create CardGateUsageDict entry if it does not exist, and
                    //  remember the card gate number associated with this logic block
                    //  to use later.

                    detail.gateNumber = cardGate.number;
                    detail.cardTypeName = cardTypeType;

                    string cardTypeGateUsageKey = cardTypeType + ":" + cardGateNumber;
                    if(!cardTypeGateUsageDict.ContainsKey(cardTypeGateUsageKey)) {
                        CardTypeGateDetail cardTypeGateDetail = new CardTypeGateDetail();
                        cardTypeGateDetail.logicFunction =
                            logicFunctionDict[cardGate.logicFunction];
                        cardTypeGateDetail.usage = new Dictionary<string, int>();
                        cardTypeGateUsageDict.Add(cardTypeGateUsageKey, cardTypeGateDetail);
                    }
                }
            }

            //  Report any cases where a given card gate appears twice in the
            //  ALD's, and that any cases where the card type isn't the same.

            logMessage("*** Begin Duplicate Card Slot:Gate usage Report on " +
                 DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));
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



            logMessage("*** End Duplicate Card Slot:Gate usage Report on " +
                DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));
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

            logMessage("*** Begin DOT Function Connection Checks *** on " +
                DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));

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
                    //  The following use case is actually legitimate...
                    //else if(connection.fromPin == "--") {
                    //    logConnection(connection);
                    //    logMessage("   Connection to DOT function is from an " +
                    //        "internal -- pin.");
                    //}
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

                //
                //  The following use case is actually legitimate...
                //if(connection.fromDotFunction > 0 && connection.toPin == "--") {
                //    logConnection(connection);
                //    logMessage("   Connection from DOT function is to an internal " +
                //        "-- pin.");
                //}

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
                List<DotDetail> detailList = null;
                if(!dotFunctionsByPage.TryGetValue(detail.page, out detailList)) {
                    detailList = new List<DotDetail>();
                    dotFunctionsByPage.Add(detail.page, detailList);
                }
                detailList.Add(detail);
            }

            sortedPages = new ArrayList(dotFunctionsByPage.Keys);
            sortedPages.Sort();

            foreach (string page in sortedPages) {
                foreach (DotDetail detail in dotFunctionsByPage[page]) {
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

            logMessage("*** End of DOT Function Check *** on " +
                DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));
            logMessage("");

            logMessage("*** Logic Block Connection Checks *** on " +
                DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));

            //  Diagram block checks

            //  Go through the connections finding the ones that originate from
            //  diagram logic blocks, and create a list of connections to and from 
            //  each pin of each lbock.  Also note the load pins.

            //  Sometimes a block has an extension.  If a given pin doesn't match
            //  On a given block, check its extension.

            //  Also, build up the counts in the cardTypeGateUsage dictionary.

            foreach(int connectionKey in connectionDict.Keys) {

                Connection connection = connectionDict[connectionKey];

                //  Check internal connections (pin "--")

                if (connection.fromPin == "--" ^ connection.toPin == "--") {
                    if(connection.fromDiagramBlock > 0 && connection.toDiagramBlock > 0) {
                        logConnection(connection);
                        logMessage("   Block to Block connection has only one internal " +
                            "pin of \"--\" [Expect both]");
                        logMessage("");
                    }
                }

                //  We just check -- to -- connections on the FROM side to avoid
                //  redundant error messages.

                if(connection.fromPin == "--") {

                    //  Block to block internal connection checks

                    if(connection.toPin == "--") {
                        BlockDetail toBlockDetail = 
                            diagramBlockDict[connection.toDiagramBlock];
                        BlockDetail fromBlockDetail =
                            diagramBlockDict[connection.fromDiagramBlock];

                        //  The following should never happen, but...

                        if(toBlockDetail.page != fromBlockDetail.page) {
                            logConnection(connection);
                            logMessage("   Block to Block internal -- connection is on " +
                                "two different pages");
                            logMessage("");
                        }

                        if (toBlockDetail.diagramBlock.cardSlot != 
                            fromBlockDetail.diagramBlock.cardSlot) {
                            logConnection(connection);
                            logMessage("    Block to Block internal -- connection " +
                                "occupies different card slots - From: " +
                                Helpers.getCardSlotInfo(
                                    fromBlockDetail.diagramBlock.cardSlot).ToString() +
                                ", To: " +
                                Helpers.getCardSlotInfo(
                                    toBlockDetail.diagramBlock.cardSlot).ToString());
                            logMessage("");
                        }
                    }

                    //  Block to sheet edge to block internal connection check

                    if(connection.toEdgeSheet != 0) {
                        string signalName = 
                            sheetEdgeDict[connection.toEdgeSheet].signalName;

                        //  Internal signals that cross sheets are rare, so I didn't
                        //  build a dictionary for them.

                        List<Sheetedgeinformation> edges =
                            sheetEdgeInformationTable.getWhere(
                                "WHERE signalName = '" + signalName + "'");

                        if(edges.Count != 2) {
                            logConnection(connection);
                            logMessage("    Internal -- to Edge connection to signal " +
                                signalName + " has " + edges.Count + " instances of " +
                                "the signal name [2 expected]");
                            string message = "";
                            foreach(Sheetedgeinformation se in edges) {
                                message += message.Length == 0 ? "    Page(s): " : ", ";
                                message += diagramPageDict[se.diagramPage].page;
                            }
                            if(message.Length > 0) {
                                logMessage(message);
                            }
                            logMessage("");
                        }

                        if (edges.Count == 2) {

                            // if(connection.idConnection == 232528) {
                            //    logMessage("Breakpoint");
                            // }
                            Sheetedgeinformation otherEdge = 
                                edges[0].idSheetEdgeInformation == 
                                    connection.toEdgeSheet ? edges[1] : edges[0];

                            //  Find the matching connection - there should be only 1.

                            List<Connection> connections = connectionTable.getWhere(
                                "WHERE fromEdgeSheet = '" + 
                                connection.toEdgeDestinationSheet + "'");

                            if(connections.Count != 1) {
                                logConnection(connection);
                                logMessage("    Internal -- to Edge connection to signal " +
                                    signalName + " found " + connections.Count.ToString() +
                                    " toEdgeDestinationSheet connections [1 expected]");
                                logMessage("");
                            }
                            else {
                                if(signalName != otherEdge.signalName) {
                                    logConnection(connection);
                                    logMessage("    Internal --- to Edge connection to " +
                                        "signals do not match, toEdgeSeet is " +
                                        signalName + ", toEdgeDestinationSheet is " +
                                        otherEdge.signalName);
                                    logMessage("");
                                }

                                Connection otherConnection = connections[0];
                                if(connection.toEdgeSheet != 
                                    otherConnection.fromEdgeOriginSheet ||
                                    connection.toEdgeDestinationSheet !=
                                    otherConnection.fromEdgeSheet) {
                                    logConnection(connection);
                                    logConnection(otherConnection);
                                    logDebug(0, "    edge[0] is (" +
                                        edges[0].idSheetEdgeInformation + "), " +
                                        "edge[1] is (" +
                                        edges[1].idSheetEdgeInformation + ")");
                                    logMessage("    The above connections on internal --" +
                                        " pin for signals " +
                                        signalName + " and " + otherEdge.signalName);
                                    logMessage("      do not match up.");
                                    logMessage("");
                                }
                                else {
                                    //  Remember this match for later.
                                    internalEdgeConnDict.Add(connection.toEdgeDestinationSheet
                                        , connection.toEdgeSheet);
                                }
                            }                            
                        }   
                    }
                }

                //  Connections from a diagram block

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
                    logDebug(2, "DEBUG: Adding connection FROM pin " + 
                        connection.fromPin);
                    pinDetail.connectionsFrom.Add(connection);
                    string polarity = "";

                    switch(connection.fromPhasePolarity) {
                        case null:
                            pinDetail.nullPolarity = true;
                            break;
                        case "+":
                            pinDetail.positive = true;
                            polarity = "+";
                            break;
                        case "-":
                            pinDetail.negative = true;
                            polarity = "-";
                            break;
                        default:
                            logMessage("Connection (" + connection.idConnection +
                                ") from dialog block " +
                                getDiagramBlockInfo(connection.fromDiagramBlock) +
                                " has an invalid fromPhasePolarity" );
                            break;
                    }

                    //  Increment the counter that tracks  usage by card type: gate number
                    //  (but skip Extension blocks) -- but a given diagram block can only
                    //  have, at most, one of each.

                    if(fromDetail.diagramBlock.symbol != "E" ||
                        fromDetail.diagramBlock.extendedTo == 0) {

                        //  Only count the + or - output from this logic block if we
                        //  have not already done so.

                        if((pinDetail.negative && !fromDetail.countedNegativeOuput) ||
                            (pinDetail.positive && !fromDetail.countedPositiveOuput)) {

                            string usageKey = fromDetail.diagramBlock.symbol + ":" + polarity;
                            string cardTypeGateUsageKey = fromDetail.cardTypeName + ":" +
                                    fromDetail.gateNumber.ToString();
                            CardTypeGateDetail cardTypeGateDetail =
                                cardTypeGateUsageDict[cardTypeGateUsageKey];
                            if (!cardTypeGateDetail.usage.ContainsKey(usageKey)) {
                                cardTypeGateDetail.usage.Add(usageKey, 1);
                            }
                            else {
                                ++cardTypeGateDetail.usage[usageKey];
                            }

                            //  Flag the + or - output from this logic block so we dont'
                            //  count it again later.

                            if(pinDetail.negative) {
                                fromDetail.countedNegativeOuput = true;
                            }
                            else {
                                fromDetail.countedPositiveOuput = true;
                            }
                        }
                    }
                }

                //  Connections to a diagram block

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

            logMessage("*** Card Type:Gate Symbol:Polarity Usage Report *** on " +
                DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));

            ArrayList cardTypeGateUsageList = new ArrayList(
                cardTypeGateUsageDict.Keys);
            cardTypeGateUsageList.Sort();

            string lastCardType = "";
            foreach (string cardTypeGateUsageKey in cardTypeGateUsageList) {
                CardTypeGateDetail cardTypeGateDetail =
                    cardTypeGateUsageDict[cardTypeGateUsageKey];
                int colonIndex = cardTypeGateUsageKey.IndexOf(':');
                string cardType = cardTypeGateUsageKey.Substring(0, colonIndex);
                if (lastCardType != cardType && lastCardType.Length > 0) {
                    logMessage("");
                }
                if (lastCardType != cardType) {
                    logMessage("Card Type Usage: " + cardType);
                    lastCardType = cardType;
                }

                //  Don't report on onused gates to save space (and confusion)

                if(cardTypeGateDetail.usage.Count == 0) {
                    continue;
                }

                ArrayList cardSymbols = new ArrayList(cardTypeGateDetail.usage.Keys);
                cardSymbols.Sort();
                string symbolMessage = "    Gate:" +
                    cardTypeGateUsageKey.Substring(colonIndex+1) + "[" +
                    cardTypeGateDetail.logicFunction + "]  ";
                string comma = "";

                foreach (string symbol in cardSymbols) {
                    string s = symbol + cardTypeGateDetail.usage[symbol].ToString();
                    if (symbolMessage.Length + s.Length > 72) {
                        logMessage(symbolMessage);
                        symbolMessage = "        ";
                        comma = "";
                    }
                    symbolMessage += comma + s;
                    comma = ", ";
                }
                if (symbolMessage.Length > 8) {
                    logMessage(symbolMessage);
                }
            }

            logMessage("*** End of Card Type:Gate Symbol:Polarity Usage Report *** on " +
                DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));
            logMessage("");

            logMessage("*** Logic Block Pin Connection Checks ***");


            //  Now, spin through the connections looking for cases where an internal
            //  pin is an input FROM an edge connection.  It should already have an
            //  entry in the internalEdgeConnDict dictionary.

            foreach (int connectionKey in connectionDict.Keys) {

                Connection connection = connectionDict[connectionKey];

                if(connection.toPin == "--" && connection.fromEdgeSheet > 0) {
                    if(!internalEdgeConnDict.ContainsKey(connection.fromEdgeSheet)) {
                        logConnection(connection);
                        logMessage("    Connection from Edge TO an internal -- pin " +
                            "does not have a ");
                        logMessage("      corresponding connection FROM an " +
                            "internal -- pin");
                    }
                }
            }


            //  Now spin through all of the logic blocks we just assembled, to check
            //  for anomolous connections at the pin/gate level - in page order.

            foreach (BlockDetail detail in diagramBlockDict.Values) {
                List<BlockDetail> diagramBlocksList;
                if(!diagramBlocksByPage.TryGetValue(detail.page, 
                        out diagramBlocksList)) {
                    diagramBlocksList = new List<BlockDetail>();
                    diagramBlocksByPage.Add(detail.page, diagramBlocksList);
                }
                diagramBlocksList.Add(detail);
            }

            sortedPages = new ArrayList(diagramBlocksByPage.Keys);
            sortedPages.Sort();

            foreach(string page in sortedPages) {

                logDebug(1, "Testing Logic blocks on page " + page);

                foreach(BlockDetail detail in diagramBlocksByPage[page]) {

                    int inputsCount = 0;
                    int outputsCount = 0;
                    string logicBlockSymbol = detail.diagramBlock.symbol;
                    string blockInfo = getDiagramBlockInfo(detail.diagramBlock.idDiagramBlock);
                    string outputSense = "+";

                    logDebug(1, "Testing Logic Block " +
                        getDiagramBlockInfo(detail.diagramBlock.idDiagramBlock));
                    logDebug(2, "   Pin list has " + detail.pinList.Count.ToString() +
                        " entries.");

                    Cardgate gate;
                    if (!cardGateDict.TryGetValue(detail.diagramBlock.cardGate,
                        out gate)) {
                        logMessage("Invalid card gate in connection from logic block, " +
                            blockInfo);
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
                            blockInfo);
                        logDebug(2, "   With " + pinDetail.connectionsTo.Count.ToString() +
                            " inputs and " + pinDetail.connectionsFrom.Count.ToString() +
                            " outputs");

                        //  An input only pin should not be an output, (and the pin is not
                        //  "--"), and vice versa

                        if(pinDetail.connectionsFrom.Count > 0 && !pinDetail.output &&
                            (pinDetail.input || !pinDetail.pin.StartsWith("--"))) {
                            string msgtype = pinDetail.input ? "input only" : "incorrect";
                            logMessage("Outputs from " + msgtype + " pin " +
                                pinDetail.pin + ", " + blockInfo);                                
                        }

                        if(pinDetail.connectionsTo.Count > 0 && !pinDetail.input &&
                            (pinDetail.output || !pinDetail.pin.StartsWith("--"))) {
                            string msgtype = pinDetail.output ? "output only" : "incorrect";
                            logMessage("Input to " + msgtype + " pin " +
                                pinDetail.pin + ", " + blockInfo);                                
                        }

                        //  Flag cases where the fromPhasepolarity is null

                        if(pinDetail.nullPolarity) {
                            logMessage("Output from pin " + pinDetail.pin +
                                ", " + blockInfo +
                                " has at least one connection with NULL polarity.");
                        }

                        //  Also flag cases where a pin has both negative and positive outputs

                        if(pinDetail.positive && pinDetail.negative) {
                            logMessage("Output from pin " + pinDetail.pin +
                                ", " + blockInfo +
                                " has both positive and negative polarity connections.");
                        }

                        if(outputSense == "+" && pinDetail.negative) {
                            outputSense = "-";
                        }

                        //  Flag any connections that are not open collector with a load
                        //  pin specified.

                        if (gate.openCollector == 0 && pinDetail.loadPins > 0) {
                            logMessage("Non open collector output with load pin from " +
                                "logic block pin " + pinDetail.pin + ", " +  blockInfo);
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
                                "from logic block pin " + pinDetail.pin + ", " + blockInfo);
                        }

                        //  A given input pin should only have one input.

                        if(pinDetail.connectionsTo.Count > 1) {
                            logMessage("Inputs from more than one output " +
                                "to logic block pin " + pinDetail.pin + ", " + blockInfo);
                        }
                    }

                    //  Note if this block has no inputs or no outputs (except that
                    //  if a diagram block is symbol "L" for load, or "LAMP", 
                    //  or "R" for resistor, no outputs is OK)

                    if(inputsCount == 0) {
                        logMessage("NO inputs to logic block at " + blockInfo);
                    }

                    //  Maybe the following should look at the card/gate characteristics,
                    //  instead of the ALD symbol??

                    if(outputsCount == 0 && detail.diagramBlock.symbol != "L"  &&
                        detail.diagramBlock.symbol != "LAMP"  &&
                        detail.diagramBlock.symbol != "R" &&
                        detail.diagramBlock.symbol != "CAP") {
                        logMessage("NO outputs from logic block at " + blockInfo);
                    }

                    //  
                    //  Symbol / Gate checks
                    //

                    string firstChar = logicBlockSymbol.Length > 0 ?
                        logicBlockSymbol.Substring(0, 1) : "";
                    string secondChar = logicBlockSymbol.Length >= 2 ?
                        logicBlockSymbol.Substring(1, 1) : "";
                    string thirdChar = logicBlockSymbol.Length >= 3 ?
                        logicBlockSymbol.Substring(2, 1) : "";
                    string fourthChar = logicBlockSymbol.Length >= 4 ?
                        logicBlockSymbol.Substring(3, 1) : "";
                    string logicFunction = gate.logicFunction == 0 ?
                        "NONE" : logicFunctionDict[gate.logicFunction];                    
                    string blockType = "";

                    switch (firstChar) {
                        case "":
                            logMessage("Empty Logic Block Symbol: " + blockInfo);
                            break;
                        case "+":
                        case "-":
                            switch (secondChar) {
                                case "A":
                                    blockType = "and";
                                    break;
                                case "O":
                                    blockType = "or";
                                    break;
                                case "I":
                                    switch(thirdChar) {
                                        case "A":
                                        case "O":
                                            //  Inverter followed by DOT function
                                            blockType = "inv";
                                            break;
                                        case "P":
                                            if(fourthChar != "A" && fourthChar != "O") {
                                                logMessage("Logic Block symbol +/-IP not " +
                                                    "followed by A or O: " + blockInfo);
                                            }
                                            blockType = "inv";
                                            break;
                                        default:
                                            logMessage("Logic block symbol +/-I not " +
                                                "followed by A, O or P " + blockInfo);
                                            break;
                                    }
                                    break;
                                default:
                                    logMessage("Logic Block symbol +/- not followed by " +
                                        "expected characters of A, O, or I " + blockInfo);
                                    break;
                            }
                            break;
                        case "A":
                        case "O":
                            if (logicBlockSymbol != "AM") {        //  AM special case
                                logMessage("Logic Block symbol A or O not preceeded by +/-: " +
                                    blockInfo);
                            }
                            break;
                        case "D":
                            if (logicBlockSymbol != "DLY") {
                                blockType = "drv";
                            }
                            break;
                        case "I":
                            blockType = "inv";
                            if(secondChar != "" && secondChar != "P" && 
                                secondChar != "A" && secondChar != "O") {
                                logMessage("Logic Block symbol I not alone or " +
                                    "followed by A, O or P: " + blockInfo);
                            }
                            break;
                        case "L":
                            blockType = "load";
                            break;
                        default:
                            break;
                    }

                    //  For certain cases, check the symbol against the logic function.
                    //  +A/-O should correspond to NAND circuitry logic, and
                    //  -A/+O should correspond to NOR circuitry logic.
                    //  (If the above seesm backwards, it is because, at least for 
                    //  the IBM 1410, the inverter is assumed to be present w/r/t the
                    //  symbol)
                    //
                    //  FURTHERMORE, if the logic block output has a - sense (i.e.,
                    //  the output line departs from the bottom of the block) it
                    //  reverses things (removes the implied invert - yeah, it is
                    //  that backwards, at least if, on the IBM 1410, you consider
                    //  0V a logic 1 and -V a logic 0.

                    //      TODO:  Implement checks using logicCheckTable

                    //if( logicFunction != "Special" && 
                    //    (logicBlockSymbol != "DLY" || logicFunction != "DELAY") && (

                    //    (andBlock && firstChar == "+" && logicFunction != "NAND") ||
                    //    (andBlock && firstChar == "-" && logicFunction != "NOR" ) ||
                    //    (orBlock && firstChar == "+" && logicFunction != "NOR"  ) ||
                    //    (orBlock && firstChar == "-" && logicFunction != "NAND" ) ||
                    //    (inverter && logicFunction != "NOT" && 
                    //        logicFunction != "NOR" && logicFunction != "NAND")    ||
                    //    (driver && secondChar == "I" && logicFunction != "NOT" )  ||
                    //    (driver && secondChar != "I" && logicFunction != "EQUAL"))
                    //                                                                ) {
                    //    logMessage("Logic Block Symbol / logicFunction mismatch: " +
                    //        "Symbol: " + logicBlockSymbol + " vs. Function: " +
                    //        logicFunction + ": " + blockInfo);
                    //}

                    //  Check this block against a table of valid configurations.
                    //  A "*" in the table matches anything

                    LogicCheckEntry foundEntry = null;
                    foreach(LogicCheckEntry entry in logicCheckTable) {
                        if ((entry.logicFunction == "*" || entry.logicFunction == logicFunction) &&
                            (entry.symbol == "*" || entry.symbol == logicBlockSymbol) &&
                            (entry.blockType == "*" || entry.blockType == blockType) &&
                            (entry.firstChar == "*" || entry.firstChar == firstChar) &&
                            (entry.outputSense == "*" || entry.outputSense == outputSense)) {
                            foundEntry = entry;
                            break;
                        }
                    }

                    if(foundEntry == null) {
                        logMessage("No match on circuit logic / logic block entry for " +
                            blockInfo);
                        logMessage("   For card type " + Helpers.getCardTypeType(
                            detail.diagramBlock.cardType) + " gate " +
                            gate.number.ToString() + " logic function " + logicFunction);
                        logMessage("   Block Symbol " + logicBlockSymbol + 
                            ", Block type: " + blockType + 
                            (outputSense == "-" ? " negative" : " positive") + " output.");
                        logMessage("");
                    }                
                }
            }

            logMessage("*** End of Logic Block Pin Connection Checks *** on " +
                DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));
            logMessage("");

            logMessage("End of Report. " + DateTime.Now.ToString("MM / dd / yyyy hh:mm:ss"));

            Parms.setParmValue("machine", currentMachine.idMachine.ToString());
            Parms.setParmValue("report output directory", directoryTextBox.Text);
            Parms.setParmValue("connection report log level", logLevel.ToString());
        }

        //  Log information about a connection

        private void logConnection(Connection connection) {

            string message = "Connection (" + connection.idConnection + ") From: ";
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
