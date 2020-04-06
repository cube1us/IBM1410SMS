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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySQLFramework;

//  Class to hold common stuff for importers.

namespace IBM1410SMS
{
    public class Importer {

        private StreamReader reader;
        private StreamWriter logWriter;
        private FileStream fileStream;
        private string inputLine;
        private string logFileName;

        DBSetup db;

        Table<Volumeset> volumeSetTable;
        Table<Volume> volumeTable;
        Table<Eco> ecoTable;
        Table<Cardtype> cardTypeTable;
        Table<Cardslot> cardSlotTable;
        Table<Feature> featureTable;

        //  The following pattern was found at
        //  https://stackoverflow.com/questions/3268622/regex-to-split-line-csv-file
        //  It works, though it seems to generate an extra empty string entry at the end.


        //                              # Parse CVS line. 
        //  \s*                         # Ignore leading whitespace.
        //  (?:                         # Group of value alternatives.
        //      ""                      # Either a double quoted string,
        //      (?<val>                 # Capture contents between quotes in "Val".
        //          [^""]* (""""[^""]*)*  # Zero or more non-quotes, allowing 
        //      )                       # doubled "" quotes within string.
        //      ""\s*                   # Ignore whitespace following quote.
        //  |   (?<val>[^,]*)           # Or... zero or more non-commas.
        //  )                           # End value alternatives group.
        //  (?:,|$)                     # Match end is comma or EOS

        private string pattern =
            @"\s*(?:""(?<val>[^""]*(""""[^""]*)*)""\s*|(?<val>[^,]*))(?:,|$)";

        DateTime defaultDate = new DateTime(1960, 1, 1);

        public Importer(string fileName) {

            db = DBSetup.Instance;
            volumeSetTable = db.getVolumeSetTable();
            volumeTable = db.getVolumeTable();
            ecoTable = db.getEcoTable();
            cardTypeTable = db.getCardTypeTable();
            cardSlotTable = db.getCardSlotTable();
            featureTable = db.getFeatureTable();

            List<string> resultList = new List<string>();

            //  Open the file even if someone else (like Excel) has it open...

            fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read,
                FileShare.ReadWrite);
            reader = new StreamReader(fileStream);

            int extensionIndex = fileName.LastIndexOf(".csv");
            if(extensionIndex < 0) {
                logFileName = fileName;
            }
            else {
                logFileName = fileName.Remove(extensionIndex, 4);
            }
            logFileName += ".log";
            logWriter = new StreamWriter(logFileName);
        }

        internal string getCSVPatternString() {
            return pattern;
        }

        internal Regex getCSVPattern() {
            return new Regex(getCSVPatternString());
        }

        internal string getLastInputLine() {
            return inputLine;
        }

        internal List<string> getCSVColumns() {

            List<string> resultList = new List<string>();

            inputLine = reader.ReadLine();
            if(inputLine == null) {

                //  List with 0 entries ==> EOF

                reader.Close();
                fileStream.Close();
                return resultList;
            }

            Match match = getCSVPattern().Match(inputLine);
            while (match.Success) {
                resultList.Add(match.Groups["val"].Value);
                match = match.NextMatch();
            }

            return resultList;
        }

        //  Utility method for an importer to get a machine entry by its name.

        internal static int getMachineKeyByName(string name) {
            List<Machine> machineList = new List<Machine>();
            machineList = DBSetup.Instance.getMachineTable().getWhere(
                "WHERE machine.name='" + name + "'");
            return (machineList.Count > 0) ? machineList[0].idMachine : 0;
        }


        //  Utility method for an importer to get a volume entry by name,
        //  and add it if it isn't already there.

        internal int getVolumeKeyByName(string volumeName, string volumeSetName,
            string machineSerial) {

            List<Volume> volumeList = new List<Volume>();
            List<Volumeset> volumeSetList = new List<Volumeset>();
            Volumeset volumeSet = null;

            //  Set some defaults...

            if(volumeSetName.Length == 0) {
                volumeSetName = "Unknown";
            }
            if(volumeName.Length == 0) {
                volumeName = "I";
            }

            //  Fine the volume set.  If none, add it now.

            volumeSetList = volumeSetTable.getWhere(
                "WHERE machineType='" + volumeSetName + "'" +
                (machineSerial.Length > 0 ? 
                    " AND machineSerial='" + machineSerial + "'" : ""));
            if(volumeSetList.Count > 0) {
                volumeSet = volumeSetList[0];
            }
            else {
                volumeSet = new Volumeset();
                volumeSet.idVolumeSet = IdCounter.incrementCounter();
                volumeSet.machineType = volumeSetName;
                volumeSet.machineSerial = machineSerial;
                volumeSetTable.insert(volumeSet);
                logMessage("Added Volume Set for \"Machine Type\" " +
                    volumeSetName + " Database ID=" + volumeSet.idVolumeSet);
            }

            //  Similar situation with the volume...

            volumeList = DBSetup.Instance.getVolumeTable().getWhere(
                "WHERE volume.set='" + volumeSet.idVolumeSet + "'" +
                " AND volume.name='" + volumeName + "'");
            if(volumeList.Count > 0) {
                return volumeList[0].idVolume;
            }
            else {
                Volume newVolume = new Volume();
                newVolume.idVolume = IdCounter.incrementCounter();
                newVolume.set = volumeSet.idVolumeSet;
                newVolume.name = volumeName;
                newVolume.machineSerial = "";
                volumeTable.insert(newVolume);
                logMessage("Added Volume " + volumeName +
                    " Database ID=" + newVolume.idVolume +
                    " to Volume Set " + volumeSetName);
                return (newVolume.idVolume);
            }
        }


        //  Routine to locate a card type given type and possibly 
        //  the part number.  If it does not exist, we will add it.

        internal int getCardType(string type, string partNumber,
            string machineName, string machineSerial, int height) {

            //  First, look up the card type by name (and perhaps
            //  part number).  If we get a hit, just return it.
            //  This search is volume name independent.

            partNumber = zeroPadPartNumber(partNumber);
            List<Cardtype> cardTypeList = cardTypeTable.getWhere(
                "WHERE cardtype.type='" + type + "'" +
                (partNumber.Length > 0 ?
                    " AND part='" + partNumber + "'" : ""));
            if (cardTypeList.Count > 0) {
                return cardTypeList[0].idCardType;
            }

            //  No hits.  In this case, we have to add a card type.
            //  But we need some volume to attach it to, so we will
            //  use a volume name of "SMS".  If we can't find one,
            //  it will get automagically added by this method call.

            int volumeKey = getVolumeKeyByName("SMS", machineName, machineSerial);

            Cardtype newCardType = new Cardtype();
            newCardType.idCardType = IdCounter.incrementCounter();
            newCardType.volume = volumeKey;
            newCardType.type = type;
            newCardType.part = partNumber;
            newCardType.nameType = "CARD ASM TSTR";
            newCardType.name = "Card " + type + " Added via Card Location Import";
            newCardType.height = height;
            newCardType.approvedDate = defaultDate;
            newCardType.designDate = defaultDate;
            newCardType.detailDate = defaultDate;
            newCardType.designCheckDate = defaultDate;
            newCardType.approvalDate = defaultDate;
            newCardType.drawDate = defaultDate;
            newCardType.drawingCheckDate = defaultDate;
            cardTypeTable.insert(newCardType);
            logMessage("Added card type " + type + " Database ID=" +
                newCardType.idCardType);
            return newCardType.idCardType;
        }

        //  Routine to look for a given card slot by name.  If it does
        //  not already exist, we will add it here.

        internal int getCardSlotByName(int panelKey, string rowName,
            int columnNumber) {

            List<Cardslot> cardSlotList = cardSlotTable.getWhere(
                "WHERE panel='" + panelKey + "'" +
                " AND cardRow='" + rowName + "'" +
                " AND cardColumn='" + columnNumber + "'");
            if (cardSlotList.Count == 0) {
                Cardslot cardSlot = new Cardslot();
                cardSlot.idCardSlot = IdCounter.incrementCounter();
                cardSlot.panel = panelKey;
                cardSlot.cardRow = rowName;
                cardSlot.cardColumn = columnNumber;
                cardSlotTable.insert(cardSlot);
                logMessage("Added Card Slot for Row " + rowName +
                    ", Column " + columnNumber +
                    " Database ID=" + cardSlot.idCardSlot +
                    " to panel (Database ID " + panelKey + ")");
                return cardSlot.idCardSlot;
            }
            return cardSlotList[0].idCardSlot;
        }

        //  Shared method to look up a feature by its name, given the
        //  machine and feature code.  If it isn't found, it will be added.

        internal int getFeatureByName(int machineKey, string code) {

            List<Feature> featureList = featureTable.getWhere(
                "WHERE machine='" + machineKey + "'" +
                " AND feature.code='" + code + "'");
            if(featureList.Count > 0) {
                return featureList[0].idFeature;
            }

            Feature feature = new Feature();
            feature.idFeature = IdCounter.incrementCounter();
            feature.machine = machineKey;
            feature.code = code;
            feature.feature = "Added Automatically by Card Location Chart Import";
            featureTable.insert(feature);
            logMessage("Added Feature " + code + " Database ID=" +
                feature.idFeature);
            return feature.idFeature;
        }

        //  Shared method to look up an ECO by its name, given the name and
        //  the database key of the machine.  If it isn't found, it will be
        //  added.

        internal int getECOByName(int machineKey, string eco) {
            List<Eco> ecoList = ecoTable.getWhere(
                "WHERE machine='" + machineKey + "'" +
                " AND eco='" + eco + "'");
            if(ecoList.Count > 0) {
                return (ecoList[0].idECO);
            }

            Eco newECO = new Eco();
            newECO.idECO = IdCounter.incrementCounter();
            newECO.machine = machineKey;
            newECO.eco = eco;
            ecoTable.insert(newECO);
            logMessage("Added ECO " + eco +
                " Database ID=" + newECO.idECO);
            return (newECO.idECO);
        }

        //  Utility method to expand numeric part numbers to 7 characters.

        public static string zeroPadPartNumber(string partNumber) {
            int junk;
            string s = partNumber;

            //  If it isn't an integer, fuhgettaboutit.
            if(partNumber.Length >= 7 ||
                !int.TryParse(partNumber,out junk)) {
                return (partNumber);
            }

            //  Otherwise, insert the appropriate number of zeros.
            partNumber = 
                partNumber.Insert(0, "0000000".Substring(0, 7 - partNumber.Length));
            Console.WriteLine("Zero pad of /" + s + "/ = /" + partNumber + "/");
            return (partNumber);

        }

        //  Utility logging method.

        internal void logMessage(string log) {
            logWriter.WriteLine(log);
        }

        //  Utility method to close the log

        internal void closeLog() {
            logWriter.Close();
        }

        //  Utility method to display the log

        internal void displayLog() {
            closeLog();
            Form ImporterLogDisplayDialog = 
                new ImporterLogDisplayForm(logFileName);
            ImporterLogDisplayDialog.Show();
        }


    }
}
