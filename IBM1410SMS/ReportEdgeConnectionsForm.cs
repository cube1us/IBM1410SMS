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

    public partial class ReportEdgeConnectionsForm : Form
    {

        //  Class to hold information on an edge connection to make life
        //  easier instead of having to chase other tables down all the time.

        class edgeConnectorEntry : IComparable<edgeConnectorEntry>
        {
            public Edgeconnector edgeConnector { get; set; }
            public int cardSlot { get; set; }
            public CardSlotInfo cardSlotInfo { get; set; }
            public string pageName { get; set; }

            public string entryName() {
                return (pageName + "*" + edgeConnector.reference + ":" + 
                    cardSlotInfo.ToSmallString().ToUpper() + edgeConnector.pin);
            }

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

        //  Class to hold tracking information for From/To connections

        class connectionTracker
        {
            public string fromKey { get; set; } = "";
            public string fromPageName { get; set; } = "";

            public int counter { get; set; } = 0;     // Number of times this was encountered

            //  Only warn once for no matching cable/Edge Connector

            public bool connWarning { get; set; } = false;

            public List<string> destinationList { get; set; } = new List<string>();
            public List<string> destinationPageList { get; set; } = new List<string>();
        }

        DBSetup db = DBSetup.Instance;

        Table<Machine> machineTable;
        List<Machine> machineList;
        Hashtable pageNameCache = new Hashtable();
        Hashtable cardSlotCache = new Hashtable();


        Machine currentMachine = null;

        string logFileName = "";
        StreamWriter logFile = null;

        int debug = 1;

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

        //  doEdgeConnectionReport actually generates the report.

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

            List<connectionTracker> slotList = new List<connectionTracker>();
            List<connectionTracker> pinList = new List<connectionTracker>();

            reportButton.Enabled = false;

            //  Build a list of cable/edge connectors that are relevant

            allCableEdgeConnectionBlockList = cableEdgeConnectionBlockTable.getAll();
            foreach (Cableedgeconnectionblock cecb in allCableEdgeConnectionBlockList) {
                if(getCachedCardSlotInfo(cecb.cardSlot).machineName ==
                    currentMachine.name) {
                    cableEdgeConnectionBlockList.Add(cecb);
                }
            }

            if(debug > 0) {
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
                if(getCachedCardSlotInfo(edgeConnector.cardSlot).machineName ==
                    currentMachine.name) {
                    edgeConnectorEntry ece = new edgeConnectorEntry();
                    ece.edgeConnector = edgeConnector;
                    ece.cardSlot = edgeConnector.cardSlot;
                    ece.cardSlotInfo = getCachedCardSlotInfo(edgeConnector.cardSlot);
                    ece.pageName = getCachedPageName(edgeConnector.diagramPage);
                    edgeConnectors.Add(ece);
                }
            }

            if(debug > 1) {
                logMessage("Edge Connectors: " + edgeConnectors.Count.ToString());
                logMessage(DateTime.Now.ToLocalTime().ToString());
            }

            edgeConnectors.Sort();

            if (debug > 1) {
                logMessage("Sorted Edge Connectors: " + edgeConnectors.Count.ToString());
                logMessage(DateTime.Now.ToLocalTime().ToString());
                for (int i = 0; i < 100; ++i) {
                    edgeConnectorEntry e = edgeConnectors[i];
                    logMessage(e.pageName + ":" +
                        e.edgeConnector.reference + ":" +
                        e.edgeConnector.order + ":"+
                        e.cardSlotInfo.ToSmallString().ToUpper() +
                        e.edgeConnector.pin);                        
                }
            }

            //  Run through all but the last of the edge connectors in order
            //  Note that we stop one BEFORE the last one.

            for(int index = 0; index < edgeConnectors.Count() -1; ++index) {

                edgeConnectorEntry entry = edgeConnectors[index];
                edgeConnectorEntry nextOne = edgeConnectors[index + 1];
                bool warning = false;

                //  If the page, machine or the reference changes, we are at the end
                //  of a particuar list, so skip it.

                if(entry.pageName != nextOne.pageName ||
                    entry.edgeConnector.reference != nextOne.edgeConnector.reference ||
                    entry.cardSlotInfo.machineName != nextOne.cardSlotInfo.machineName) { 
                    if(debug > 1) {
                        logMessage("Info: Skipping: " + entry.entryName() + ", Next: " +
                            nextOne.entryName());
                    }
                    continue;
                }

                if (debug > 0) {
                    logMessage("Processing " + entry.entryName() + 
                        " -> " + nextOne.entryName());
                }

                //  Construct the keys we will use to search

                string slotFromKey = entry.cardSlotInfo.ToString().ToUpper();
                string slotToKey = nextOne.cardSlotInfo.ToString().ToUpper();

                //  See if we already have an entry for this FROM (first slot)

                connectionTracker fromSlot = slotList.Find(x => x.fromKey == slotFromKey);

                //  Look for a from/to match by card slot among cable/edge connectors
                //  If none found, issue a warning (but only once for a given from/to

                //  Do NOT warn in some obvious cases:
                //      We have already warned *OR* 
                //      Same Panel  *OR*
                //      Same Gate and is NOT an adjacent panel
                //  Applying DeMorgan's Theorem, we get CHECK IF
                //      We have NOT already warned *AND*
                //      NOT the same Panel  *AND*
                //      (different gate OR adjacent panel)
                //  HOWEVER, if it is the same panel, it cannot be adjacent,
                //  So adjacent panel is a superset of not the same panel
                //  So, we check if the gate name does NOT match, or, if it is
                //  the same gate, the panel is adjacent.  Othewise, the check is skipped.

                if (entry.cardSlotInfo.gateName != nextOne.cardSlotInfo.gateName ||
                    Helpers.isPanelAdjacent(entry.cardSlotInfo.panelName,
                        nextOne.cardSlotInfo.panelName)) {

                    Cableedgeconnectionblock cableMatch = cableEdgeConnectionBlockList.Find(
                        x => x.cardSlot == entry.cardSlot && x.Destination == nextOne.cardSlot);

                    if ((fromSlot == null || !fromSlot.connWarning) &&
                        (cableMatch == null || cableMatch.cardSlot == 0)) {
                        if (debug == 0) {
                            logMessage("Processing " + 
                                entry.entryName() + " -> " +  nextOne.entryName());
                        }
                        logMessage("   Warning:  No Cable/Edge Connector Found");
                        warning = true;
                    }
                }

                //  Look for a matching entry card slot -> card slot


                if(fromSlot == null) {

                    //  No Matching FROM entry

                    if (debug > 0) {
                        logMessage("   Adding new slotList entry for " + slotFromKey +
                            " with destination " + slotToKey);
                    }

                    fromSlot = new connectionTracker();
                    fromSlot.fromKey = slotFromKey;
                    fromSlot.counter = 1;
                    fromSlot.connWarning = warning;
                    fromSlot.destinationList.Add(slotToKey);
                    fromSlot.destinationPageList.Add(nextOne.pageName);
                    fromSlot.fromPageName = entry.pageName;

                    //  Look to see if this destination is already in use somewhere else

                    connectionTracker destTracker = slotList.Find(
                        x => x.destinationList.Contains(slotToKey));

                    if(destTracker != null && destTracker.counter != 0) {
                        if (!warning && debug == 0) {
                            logMessage("Processing " +
                                entry.entryName() + " -> " + nextOne.entryName());
                        }

                        int destTrackerIndex = destTracker.destinationList.IndexOf(
                            slotToKey);

                        logMessage("   WARNING:  This destination already exists for " +
                            "slotList FROM " + destTracker.fromKey);
                        logMessage("   FROM first found on page " +
                            fromSlot.fromPageName + ", TO first found on page " +
                            destTracker.destinationPageList[destTrackerIndex]);

                    }

                    //  Finally, add the new entry

                    slotList.Add(fromSlot);
                }

                else {

                    //  Matching FROM entry

                    if (debug > 0) {
                        logMessage("   Found matching slotList entry for " + slotFromKey +
                            " first found on page " + fromSlot.fromPageName);
                    }

                    //  Does this entry already contain the same destination?
                    //  If so, bump the counter

                    if(fromSlot.destinationList.Contains(slotToKey)) {
                        ++fromSlot.counter;
                    }
                    else {
                        //  This "TO" is new.  Is this "TO" referenced somewhere else?

                        connectionTracker destTracker = slotList.Find(
                            x => x.destinationList.Contains(slotToKey));

                        if (destTracker != null && destTracker.counter != 0) {
                            if (!warning && debug == 0) {
                                logMessage("Processing " +
                                    entry.entryName() + " -> " + nextOne.entryName());
                            }

                            int destTrackerIndex = destTracker.destinationList.IndexOf(
                                slotToKey);
                            logMessage("   WARNING:  This destination already exists for " +
                                "slotList FROM " + destTracker.fromKey);
                            logMessage("   FROM first found on page " +
                                fromSlot.fromPageName + ", TO first found on page " +
                                destTracker.destinationPageList[destTrackerIndex]);
                        }

                        if(debug > 0) {
                            logMessage("   Adding new Destination " + slotToKey +
                                " to slotList entry " + slotFromKey);
                        }
                        fromSlot.destinationList.Add(slotToKey);
                        fromSlot.destinationPageList.Add(nextOne.pageName);
                    }
                }

                if (debug > 0 && index > 100) {
                   break;      // Testing
                }
            }

            reportButton.Enabled = true;

            logMessage("End of Report");
        }

        private CardSlotInfo getCachedCardSlotInfo(int cardSlot) {
            if(!cardSlotCache.ContainsKey(cardSlot)) {
                cardSlotCache[cardSlot] = Helpers.getCardSlotInfo(cardSlot);
            }
            return ((CardSlotInfo)cardSlotCache[cardSlot]);
        }

        private string getCachedPageName(int diagramPage) {
            if(!pageNameCache.ContainsKey(diagramPage)) {
                pageNameCache.Add(diagramPage, Helpers.getDiagramPageName(diagramPage));
            }
            return ((string)pageNameCache[diagramPage]);
        }

        private void logMessage(string message) {
            logFile.WriteLine(message);
            logFile.Flush();
        }

    }
}
