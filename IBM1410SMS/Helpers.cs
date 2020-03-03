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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using MySQLFramework;

namespace IBM1410SMS
{

    /*
     * This class contains relatively static fields for database access,
     * including instantiations for the entities.  It is a singleton, by
     * way of the instance field.
     *
    */


    class Helpers
    {

        internal static string[] validDiagramRows = {
            "A", "B", "C", "D", "E", "F", "G", "H", "I",
        };

        internal static int maxDiagramColumn = 5;

        internal static char[] validPins = {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N',
            'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '1', '2',
            '3', '4', '5', '6', '7', '8', '9'
        };

        //  List of valid possible first/last row names: letters, no I, or O.

        internal static string[] validRows = new string[] { "A", "B", "C", "D", "E", "F", "G",
            "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "W",
            "X", "Y", "Z"};

        //  List of card types that are Cable/Edge Connection card types

        internal static string[] cableEdgeConnectionCardTypes = new string[]
            {"CABL", "CONN"};

        private static readonly Helpers instance = new Helpers();

        //  This is the constructor.  It CANNOT do ANYTHING, because it
        //  is statically initialized.

        private Helpers() {
        }

        //  This returns an instance of the class for things that need it.

        public static Helpers Instance
        {
            get
            {
                return instance;
            }
        }


        //  Method to get the card slot key given the names of the tree above it,
        //  adding nodes along the way, if needed.  It also prepares a message
        //  to display to the user about its actions.

        //  Note:  the Machine in card slot info MUST ALREADY EXIST.

        public static int getOrAddCardSlotKey(bool update, 
            CardSlotInfo cardSlotInfo, out string message) {

            string action = update ? "Added" : "Adding";
            string machineName = cardSlotInfo.machineName;

            int machineKey = 0;
            int frameKey = 0;
            int gateKey = 0;
            int panelKey = 0;
            int slotKey = 0;

            DBSetup db = DBSetup.Instance;

            Table<Machine> machineTable = db.getMachineTable();
            Table<Frame> frameTable = db.getFrameTable();
            Table<Machinegate> machineGateTable = db.getMachineGateTable();
            Table<Panel> panelTable = db.getPanelTable();
            Table<Cardslot> cardSlotTable = db.getCardSlotTable();


            message = "";

            List<Machine> machineList = machineTable.getWhere(
                "WHERE machine.name='" + cardSlotInfo.machineName + "'");
            if(machineList.Count == 0) {
                return 0;
            }
            machineKey = machineList[0].idMachine;

            List<Frame> frameList = frameTable.getWhere(
                "WHERE machine='" + machineKey + "'" +
                " AND frame.name='" + cardSlotInfo.frameName + "'");
            if (frameList.Count == 0) {
                message += action + " Frame " + cardSlotInfo.frameName + " to machine " +
                    machineName;
                if (update) {
                    Frame newFrame = new Frame();
                    newFrame.idFrame = IdCounter.incrementCounter();
                    newFrame.machine = machineKey;
                    newFrame.name = cardSlotInfo.frameName;
                    frameTable.insert(newFrame);
                    message += " Database ID=" + newFrame.idFrame;
                    frameKey = newFrame.idFrame;
                }
                message += "\n";
            }
            else {
                frameKey = frameList[0].idFrame;
            }

            List<Machinegate> machineGateList = machineGateTable.getWhere(
                "WHERE frame='" + frameKey + "'" +
                " AND machinegate.name='" + cardSlotInfo.gateName + "'");
            if (machineGateList.Count == 0) {
                message += action + " Gate " + cardSlotInfo.gateName +
                    " to machine " + machineName + ", frame " + cardSlotInfo.frameName;
                if (update) {
                    Machinegate newGate = new Machinegate();
                    newGate.idGate = IdCounter.incrementCounter();
                    newGate.frame = frameKey;
                    newGate.name = cardSlotInfo.gateName;
                    machineGateTable.insert(newGate);
                    message += " Database ID=" + newGate.idGate;
                    gateKey = newGate.idGate;
                }
                message += "\n";
            }
            else {
                gateKey = machineGateList[0].idGate;
            }

            List<Panel> panelList = panelTable.getWhere(
                "WHERE gate='" + gateKey + "'" +
                " AND panel='" + cardSlotInfo.panelName + "'");
            if (panelList.Count == 0) {
                message += action + " Panel " + cardSlotInfo.panelName +
                    " to machine " + machineName + ", frame " + cardSlotInfo.frameName +
                    ", gate " + cardSlotInfo.gateName;
                if (update) {
                    Panel newPanel = new Panel();
                    newPanel.idPanel = IdCounter.incrementCounter();
                    newPanel.gate = gateKey;
                    newPanel.panel = cardSlotInfo.panelName;
                    panelTable.insert(newPanel);
                    message += " Database ID=" + newPanel.idPanel;
                    panelKey = newPanel.idPanel;
                }
                message += "\n";
            }
            else {
                panelKey = panelList[0].idPanel;
            }

            //  Now we can finally look for the slot...

            List<Cardslot> cardSlotList = cardSlotTable.getWhere(
                "WHERE panel='" + panelKey + "'" +
                " AND cardRow='" + cardSlotInfo.row + "'" +
                " AND cardColumn='" + cardSlotInfo.column.ToString() + "'");
            if (cardSlotList.Count == 0) {
                message += action + " Card Slot for row " + cardSlotInfo.row +
                    ", column " + cardSlotInfo.column + " to machine " + machineName +
                    ", frame " + cardSlotInfo.frameName + ", gate " + cardSlotInfo.gateName +
                    ", panel " + cardSlotInfo.panelName;
                if (update) {
                    Cardslot newSlot = new Cardslot();
                    newSlot.idCardSlot = IdCounter.incrementCounter();
                    newSlot.panel = panelKey;
                    newSlot.cardRow = cardSlotInfo.row;
                    newSlot.cardColumn = cardSlotInfo.column;
                    cardSlotTable.insert(newSlot);
                    message += " Database ID=" + newSlot.idCardSlot;
                    slotKey = newSlot.idCardSlot;
                }
                message += "\n";
            }
            else {
                slotKey = cardSlotList[0].idCardSlot;
            }

            return (slotKey);
        }


        //  Similar method to get a diagram page given the volume set key,
        //  machine key (and name) volume name and page name.   If the page
        //  is not found in any pages of the volume set, it will use the
        //  volume name (adding that, if necessary), and then 
        //  add the page.

        public static int getOrAddDiagramPageKey(bool update, Machine machine, 
            Volumeset volumeSet, string defaultVolumeName, string pageName, 
            out string message) {

            string action = update ? "Added" : "Adding";
            string machineName = machine.name;
            string volumeSetName = volumeSet.machineType + "/" +
                volumeSet.machineSerial;

            int volumeKey = 0;
            int pageKey = 0;
            int diagramKey = 0;

            DBSetup db = DBSetup.Instance;
            Table<Volume> volumeTable = db.getVolumeTable();
            Table<Page> pageTable = db.getPageTable();
            Table<Diagrampage> diagramPageTable = db.getDiagramPageTable();

            message = "";

            if (defaultVolumeName.Length == 0) {
                defaultVolumeName = "UNKNOWN";
            }

            //  The caller might not know which volume contains the page.
            //  So, first get a list of all of the volumes.

            List<Volume> volumeList = volumeTable.getWhere(
                "WHERE volume.set='" + volumeSet.idVolumeSet + "'");

            foreach(Volume v in volumeList) {
                
                //  While we are at it, capture the key associated with the
                //  default volume name.

                if(v.name == defaultVolumeName) {
                    volumeKey = v.idVolume;
                }
                List<Page> pageList = pageTable.getWhere(
                    "WHERE machine='" + machine.idMachine + "'" +
                    " AND volume='" + v.idVolume + "'" +
                    " AND page.name='" + pageName + "'");
                if(pageList.Count > 0) {
                    pageKey = pageList[0].idPage;
                }
            }

            //  If the page was not found, then we have to add it
            //  (and perhaps the volume, as well).

            if (pageKey == 0) {
                if (volumeKey == 0) {
                    message += action + " Volume " + defaultVolumeName +
                        " to volume set " + volumeSetName;
                    if (update) {
                        Volume newVolume = new Volume();
                        newVolume.idVolume = IdCounter.incrementCounter();
                        newVolume.set = volumeSet.idVolumeSet;
                        newVolume.name = defaultVolumeName;
                        newVolume.machineSerial = volumeSet.machineSerial;
                        volumeTable.insert(newVolume);
                        message += " Database ID=" + newVolume.idVolume;
                        volumeKey = newVolume.idVolume;
                    }
                    message += "\n";
                }
                else {
                    volumeKey = volumeList[0].idVolume;
                }

                //  Then add the page...

                message += action + " Page " + pageName +
                    " to volume " + defaultVolumeName +
                    " of volume set " + volumeSetName +
                    " for machine " + machineName;
                if (update) {
                    Page newPage = new Page();
                    newPage.idPage = IdCounter.incrementCounter();
                    newPage.machine = machine.idMachine;
                    newPage.volume = volumeKey;
                    newPage.part = "";
                    newPage.title = "Added by Tie Down Edit Form";
                    newPage.name = pageName;
                    pageTable.insert(newPage);
                    message += " Database ID=" + newPage.idPage;
                    pageKey = newPage.idPage;
                }
                message += "\n";
            }

            //  The final goal -- the diagram page..

            List<Diagrampage> diagramPageList = diagramPageTable.getWhere(
                "WHERE diagrampage.page='" + pageKey + "'");
            if (diagramPageList.Count == 0) {
                message += action + " Diagram Page to page " + pageName;
                if (update) {
                    Diagrampage newDiagram = new Diagrampage();
                    newDiagram.idDiagramPage = IdCounter.incrementCounter();
                    newDiagram.page = pageKey;
                    diagramPageTable.insert(newDiagram);
                    message += " Database ID=" + newDiagram.idDiagramPage;
                    diagramKey = newDiagram.idDiagramPage;
                }
                message += "\n";
            }
            else {
                diagramKey = diagramPageList[0].idDiagramPage;
            }

            return (diagramKey);
        }

        //  Routine to return the name of an existing diagram page.

        public static string getDiagramPageName(int diagramPageKey) {

            DBSetup db = DBSetup.Instance;
            Table<Diagrampage> diagramPageTable = db.getDiagramPageTable();
            Table<Page> pageTable = db.getPageTable();

            if (diagramPageKey == 0) {
                return "";
            }

            Diagrampage diagramPage = diagramPageTable.getByKey(diagramPageKey);
            if (diagramPage.idDiagramPage == 0) {
                return "";
            }

            Page page = pageTable.getByKey(diagramPage.page);
            if (page.idPage == 0) {
                return "";
            }

            return page.name;
        }

        //  Routine to get the machine name, given a page.

        public static string getMachineFromPage(Page page) {

            DBSetup db = DBSetup.Instance;

            Table<Machine> machineTable = db.getMachineTable();
            Machine machine = machineTable.getByKey(page.machine);
            if(machine == null || machine.idMachine == 0) {
                return "Unknown machine!";
            }
            else {
                return machine.name;
            }
        }

        //  Routine to get a feature code, given its key

        public static string getFeatureCode(int featureKey) {

            DBSetup db = DBSetup.Instance;
            Table<Feature> featureTable = db.getFeatureTable();

            if (featureKey == 0) {
                return "";
            }
            Feature feature = featureTable.getByKey(featureKey);
            if (feature.idFeature == 0) {
                return "";
            }
            return feature.code;
        }


        //  Routine to look up a feature key by name.  It will add the feature,
        //  if it is not already there.  It also provides a message to display
        //  to the user.

        public static int getOrAddFeatureKey(bool update, Machine machine,
            string featureName, out string message) {

            message = "";
            int featureKey = 0;

            DBSetup db = DBSetup.Instance;
            Table<Feature> featureTable = db.getFeatureTable();

            List<Feature> featureList = featureTable.getWhere(
                "WHERE machine='" + machine.idMachine + "'" +
                " AND feature.code='" + featureName + "'");
            if (featureList.Count == 0) {
                message += (update ? "Added" : "Adding") +
                    " Feature " + featureName +
                    " to list for machine " + machine.name;
                if (update) {
                    Feature newFeature = new Feature();
                    newFeature.idFeature = IdCounter.incrementCounter();
                    newFeature.machine = machine.idMachine;
                    newFeature.code = featureName;
                    newFeature.feature = "Added by Tie Down Edit Form";
                    featureTable.insert(newFeature);
                    featureKey = newFeature.idFeature;
                }
                message += "\n";
            }
            else {
                featureKey = featureList[0].idFeature;
            }

            return (featureKey);
        }


        //  Routine to get an ECO number, given its key

        public static string getEcoEco(int ecoKey) {

            DBSetup db = DBSetup.Instance;
            Table<Eco> EcoTable = db.getEcoTable();

            if (ecoKey == 0) {
                return "";
            }
            Eco eco = EcoTable.getByKey(ecoKey);
            if (eco.idECO == 0) {
                return "";
            }
            return eco.eco;
        }


        //  Routine to look up an ECO key by name.  It will add the ECO,
        //  if it is not already there.  It also provides a message to display
        //  to the user.

        public static int getOrAddEcoKey(bool update, Machine machine,
            string eco, out string message) {

            message = "";
            int ecoKey = 0;

            DBSetup db = DBSetup.Instance;
            Table<Eco> ecoTable = db.getEcoTable();

            List<Eco> ecoList = ecoTable.getWhere(
                "WHERE machine='" + machine.idMachine + "'" +
                " AND eco='" + eco + "'");
            if (ecoList.Count == 0) {
                message += (update ? "Added" : "Adding") +
                    " ECO " + eco +
                    " to list for machine " + machine.name;
                if (update) {
                    Eco newEco = new Eco();
                    newEco.idECO = IdCounter.incrementCounter();
                    newEco.machine = machine.idMachine;
                    newEco.eco = eco.ToUpper();
                    newEco.description = "Added by ALD Diagram Page Form";
                    ecoTable.insert(newEco);
                    ecoKey = newEco.idECO;
                }
                message += "\n";
            }
            else {
                ecoKey = ecoList[0].idECO;
            }

            return (ecoKey);
        }



        //  Routine to return the name of an existing card type

        public static string getCardTypeType(int cardTypeKey) {

            DBSetup db = DBSetup.Instance;
            Table<Cardtype> cardTypeTable = db.getCardTypeTable();

            if (cardTypeKey == 0) {
                return "";
            }

            Cardtype cardType = cardTypeTable.getByKey(cardTypeKey);
            if (cardType.idCardType == 0) {
                return "";
            }
            return cardType.type;
        }


        //  Routine to get the key of a card type, given its type (name)

        public static int getCardType(string type) {

            DBSetup db = DBSetup.Instance;
            Table<Cardtype> cardTypeTable = db.getCardTypeTable();

            List<Cardtype> cardTypeList = cardTypeTable.getWhere(
                "WHERE cardtype.type='" + type + "'");
            if (cardTypeList.Count > 0) {
                return cardTypeList[0].idCardType;
            }
            else {
                return 0;
            }
        }

        //  Get 2nd two characters of a machine name

        public static string getTwoCharMachineName(string machineName) {

            string val;
            int len = machineName.Length;

            if (len <= 2) {
                val = machineName;
            }
            else {
                val = machineName.Substring(2, len == 3 ? 1 : 2);
            }

            return (val);
        }


        //  Routine to look up Frame, Gate, Panel, Row and Column for
        //  a given existing card slot

        public static CardSlotInfo getCardSlotInfo(int cardSlotKey) {

            DBSetup db = DBSetup.Instance;
            Table<Cardslot> cardSlotTable = db.getCardSlotTable();
            Table<Panel> panelTable = db.getPanelTable();
            Table<Machinegate> machineGateTable = db.getMachineGateTable();
            Table<Frame> frameTable = db.getFrameTable();
            Table<Machine> machineTable = db.getMachineTable();

            CardSlotInfo info = new CardSlotInfo();

            if (cardSlotKey == 0) {
                return (info);
            }

            Cardslot cs = cardSlotTable.getByKey(cardSlotKey);
            Panel panel = panelTable.getByKey(cs.panel);
            Machinegate gate = machineGateTable.getByKey(panel.gate);
            Frame frame = frameTable.getByKey(gate.frame);
            Machine machine = machineTable.getByKey(frame.machine);

            info.machineName = machine.name;
            info.frameName = frame.name;
            info.gateName = gate.name;
            info.panelName = panel.panel;
            info.row = cs.cardRow;
            info.column = cs.cardColumn;

            return (info);
        }

        //  Method to get a list of all of the inputs or all of the outputs
        //  from a given card type.

        public static List<Gatepin> getGatePins(int cardTypeKey, bool isInput) {

            DBSetup db = DBSetup.Instance;

            Table<Cardgate> cardGateTable = db.getCardGateTable();
            Table<Gatepin> gatePinTable = db.getGatePinTable();

            List<Gatepin> gatePinsList = new List<Gatepin>();

            List<Cardgate> cardGateList = cardGateTable.getWhere(
                "WHERE cardType='" + cardTypeKey + "'");

            //  For each gate on the card, get the list of pins,
            //  and add them to our list only if they do not have
            //  a blank pin name and are not already present.
            //  (This would have been perfect as a JOIN, of course...)

            bool intraCardconnection = false;
            foreach (Cardgate gate in cardGateList) {
                List<Gatepin> subsetPinsList = gatePinTable.getWhere(
                    "WHERE cardGate='" + gate.idcardGate + "'" +
                    " AND " + (isInput ? "input='1'" : "output='1'"));
                foreach (Gatepin pin in subsetPinsList) {
                    if (pin.pin.Length > 0 && gatePinsList.FindIndex(
                        x => x.pin == pin.pin) < 0) {
                        gatePinsList.Add(pin);
                    }
                    else if((isInput && pin.inputGate != 0) ||
                        (!isInput && pin.outputGate != 0)) {
                        intraCardconnection = true;
                    }
                }
            }

            //  Next, sort the list...

            gatePinsList.Sort(delegate (Gatepin x, Gatepin y)
            {
                if (x.pin == null && y.pin == null) return 0;
                else if (x.pin == null) return -1;
                else if (y.pin == null) return 1;
                else return x.pin.CompareTo(y.pin);
            });

            //  If there is an intra-Card connection on this card, add the
            //  special pin at the end of the list.

            if(intraCardconnection) {
                Gatepin intraCardPin = new Gatepin();
                intraCardPin.pin = "--";
                intraCardPin.mapPin = "";
                intraCardPin.input = (isInput ? 1 : 0);
                intraCardPin.output = (isInput ? 0 : 1);
                intraCardPin.voltageTenths = 0;
                intraCardPin.inputGate = 0;
                intraCardPin.outputGate = 0;
                intraCardPin.cardGate = 0;
                intraCardPin.dotInput = 0;
                intraCardPin.dotOutput = 0;
                gatePinsList.Add(intraCardPin);
            }

            //  And return it.

            return (gatePinsList);
        }

    }

    internal class CardSlotInfo  {

        internal string machineName { get; set; }
        internal string frameName { get; set; }
        internal string gateName { get; set; }
        internal string panelName { get; set; }
        internal string row { get; set; }
        internal int column { get; set; }       

        public CardSlotInfo() {
            machineName = frameName = gateName = panelName = row = "";
            column = 0;
        }
        
        public CardSlotInfo(string shortName) {

            Match match = Regex.Match(shortName, @"^(....)(.)(.)(.)(\d\d)(.)$");
            int columnNumber = 0;

            machineName = frameName = gateName = panelName = row = "";
            column = 0;

            if (!match.Success) {
                return;
            }

            if(!int.TryParse(match.Groups[5].Value, out columnNumber)) {
                //  This should never actually happen because the 
                //  pattern already validated the column number.
                return;
            }

            machineName = match.Groups[1].Value;
            frameName = match.Groups[2].Value;
            gateName = frameName;
            panelName = match.Groups[3].Value;
            row = match.Groups[4].Value;
            column = columnNumber;
        }

        //  This class has a special ToString method.

        public override string ToString() {

            return machineName + frameName + gateName +
                panelName + row + column.ToString("D2");
        }

    }
}
