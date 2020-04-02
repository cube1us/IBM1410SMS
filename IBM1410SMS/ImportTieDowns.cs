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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MySQLFramework;

namespace IBM1410SMS
{
    class ImportTieDowns : Importer
    {

        ImportStartupForm.Disposition disposition;
        Hashtable csvColumnNames = new Hashtable();

        public ImportTieDowns(string fileName,
            ImportStartupForm.Disposition disposition, bool testMode) : base(fileName) {

            this.disposition = disposition;

            Table<Machine> machineTable;
            Table<Tiedown> tieDownTable;
            Table<Volumeset> volumeSetTable;
            Table<Volume> volumeTable;
            Table<Page> pageTable;
            Table<Diagrampage> diagramPageTable;
            Table<Frame> frameTable;
            Table<Machinegate> machineGateTable;
            Table<Panel> panelTable;
            Table<Cardslot> cardSlotTable;

            bool header = true;
            int lineNumber = 0;

            string currentMachineName = null;
            string currentMachineSerialName = null;
            string frameName = null;
            string gateName = null;
            string panelName = null;
            string rowName = null;
            string columnName = null;
            int columnNumber = 0;

            int machineKey = 0;
            int volumeSetKey = 0;
            int machinesWithKey;
            int machinesWithoutKey;
            int cardTypeKey;
            int pageKey;
            int diagramPageKey;
            int frameKey;
            int machineGateKey;
            int panelKey;
            int cardSlotKey;

            List<string> csvColumns;
            List<Volume> volumeList = new List<Volume>();

            DBSetup db = DBSetup.Instance;
            machineTable = db.getMachineTable();
            tieDownTable = db.getTieDownTable();
            volumeSetTable = db.getVolumeSetTable();
            volumeTable = db.getVolumeTable();
            pageTable = db.getPageTable();
            diagramPageTable = db.getDiagramPageTable();
            frameTable = db.getFrameTable();
            machineGateTable = db.getMachineGateTable();
            panelTable = db.getPanelTable();
            cardSlotTable = db.getCardSlotTable();


            while ((csvColumns = getCSVColumns()).Count > 0) {

                //  Process the header line.

                if (header) {

                    //  Skip extra headers with nothing in the first column.

                    if(csvColumns[0].Length == 0) {
                        continue;
                    }

                    int columnIndex = 0;
                    foreach (string s in csvColumns) {
                        csvColumnNames.Add(s, columnIndex);
                        ++columnIndex;
                    }

                    header = false;


                    //  Check for required columns.  Column order doesn't matter.

                    string missingColumns = "";
                    missingColumns += checkColumn("Machine");
                    missingColumns += checkColumn("Machine Serial");
                    missingColumns += checkColumn("LOGIC SOURCE");
                    missingColumns += checkColumn("CARD CAP CODE");
                    missingColumns += checkColumn("LOCATION");
                    missingColumns += checkColumn("PIN");
                    missingColumns += checkColumn("OTHER PIN");
                    missingColumns += checkColumn("APPLIES TO MACHINES WITHOUT");
                    missingColumns += checkColumn("CHECKED");
                    missingColumns += checkColumn("APPLIES TO MACHINES WITH");
                    missingColumns += checkColumn("NOTE");


                    if (missingColumns.Length > 0) {
                        MessageBox.Show("One or more input columns are " +
                            "missing: \n" + missingColumns,
                            "Missing Column(s)",
                            MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        closeLog();
                        return;
                    }

                    //  Columns were OK, so start up the transaction.

                    db.BeginTransaction();
                    ++lineNumber;
                    continue;
                }

                string machineName = csvColumns[(int)csvColumnNames["Machine"]];
                string machineSerial =
                    csvColumns[(int)csvColumnNames["Machine Serial"]];
                string logicSource =
                    csvColumns[(int)csvColumnNames["LOGIC SOURCE"]];
                string cardCapCode =
                    csvColumns[(int)csvColumnNames["CARD CAP CODE"]];
                string location = csvColumns[(int)csvColumnNames["LOCATION"]];
                string pinName = csvColumns[(int)csvColumnNames["PIN"]];
                string otherPinName = csvColumns[(int)csvColumnNames["OTHER PIN"]];
                string machinesWithout =
                    csvColumns[(int)csvColumnNames["APPLIES TO MACHINES WITHOUT"]];
                string tieDownChecked =
                    csvColumns[(int)csvColumnNames["CHECKED"]];
                string machinesWith =
                    csvColumns[(int)csvColumnNames["APPLIES TO MACHINES WITH"]];
                string note = csvColumns[(int)csvColumnNames["NOTE"]];


                //  If there is a machine specified, capture it.

                if (machineName.Length > 0) {
                    List<Machine> machineList = machineTable.getWhere(
                        "WHERE machine.name='" + machineName + "'");
                    if (machineList.Count > 0) {
                        machineKey = machineList[0].idMachine;
                        currentMachineName = machineName;
                    }
                }

                if(machineSerial.Length > 0) {
                    currentMachineSerialName = machineSerial;
                }

                //  Decode the location information.  This is computer
                //  specific.


                if(currentMachineName == "1411" || 
                   currentMachineName == "1415" ||
                   currentMachineName == "1414") {
                

                    //  The card location must be specified, and must be 7 
                    //  characters (MM machine, Frame, Gate, Panel, Row and
                    //  column.

                    if (location.Length != 7) {
                        MessageBox.Show("Input line " + lineNumber +
                            ": invalid location " + location +
                            "(Must be 7 chars FFGPRCC for Machine " +
                            currentMachineName + ")");
                        db.CancelTransaction();
                        closeLog();
                        return;
                    }

                    if (location.Substring(0,2) != currentMachineName.Substring(2,2)) {
                        MessageBox.Show("Input Line " + lineNumber +
                            "Location does not begin " +
                            currentMachineName.Substring(2,2) + " for Machine " +
                            currentMachineName + ": " +
                            location);
                        db.CancelTransaction();
                        closeLog();
                        return;
                    }

                    frameName = location.Substring(2, 1);
                    gateName = frameName;
                    panelName = location.Substring(3, 1);
                    rowName = location.Substring(4, 1);
                    columnName = location.Substring(5, 2);
                    if(!int.TryParse(columnName, out columnNumber)) {
                        MessageBox.Show("Input Line " + lineNumber +
                            "Invalid column in location - must be numeric " +
                            location);
                        db.CancelTransaction();
                        closeLog();
                        return;
                    }
                }

                //  ADD LOCATION FOR OTHER TYPES OF COMPUTERS HERE....

                //  By this time, in the first data row, we MUST have
                //  a machine serial and machine.

                if(currentMachineSerialName == null ||
                    currentMachineSerialName == null) {
                    MessageBox.Show("Machine Name and Machine Serial not specified",
                        "Missing Machine Name / Machine Serial",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Stop);
                    db.CancelTransaction();
                    closeLog();
                    return;
                }

                //  If either of the above changed, get a new volume set and
                //  volume list.

                if(machineName.Length > 0 || machineSerial.Length > 0) {

                    List<Volumeset> volumeSetList = volumeSetTable.getWhere(
                        "WHERE machineType='" + currentMachineName + "'" +
                        " AND machineSerial='" + currentMachineSerialName + "'");
                    if(volumeSetList.Count == 0) {
                        Volumeset vs = new Volumeset();
                        vs.idVolumeSet = IdCounter.incrementCounter();
                        vs.machineType = currentMachineName;
                        vs.machineSerial = currentMachineSerialName;
                        volumeSetTable.insert(vs);
                        logMessage("Input Line " + lineNumber +
                            " Added Volume Set Database ID=" +
                            vs.idVolumeSet + " for machine " +
                            currentMachineName + " Serial " +
                            currentMachineSerialName);
                        volumeSetKey = vs.idVolumeSet;
                    }
                    else {
                        volumeSetKey = volumeSetList[0].idVolumeSet;
                    }

                    volumeList = volumeTable.getWhere(
                        "WHERE volume.set='" + volumeSetKey + "'");
                }

                //  Look up the (diagram) page, and add one if not there,
                //  calling it the same name as used by the Card Location
                //  Chart Importer.

                pageKey = 0;
                foreach (Volume volume in volumeList) {
                    List<Page> pageList = pageTable.getWhere(
                        "WHERE machine='" + machineKey + "'" +
                        " AND volume='" + volume.idVolume + "'" +
                        " AND page.name='" + logicSource + "'");
                    if(pageList.Count > 0) {
                        pageKey = pageList[0].idPage;
                        break;
                    }
                }

                //  If none exists, see if we can find a volume that fits
                //  this pages range.  If so, use that volume, otherwise,
                //  add it to a volume called TIEDOWN

                if(pageKey == 0) {
                    int volumeKey = 0;

                    //  First, see if we can find a volume appropriate for
                    //  this page.

                    volumeKey = volumeList.Find(
                        x => x.firstPage != null && x.lastPage != null &&
                        logicSource.CompareTo(x.firstPage) >= 0 &&
                        logicSource.CompareTo(x.lastPage) <= 0).idVolume;

                    //  If that doesn't work, look for the TIEDOWN volume.

                    if (volumeKey == 0) {
                        foreach (Volume volume in volumeList) {
                            if (volume.name == "TIEDOWN") {
                                volumeKey = volume.idVolume;
                                break;
                            }
                        }
                    }

                    //  If that doesn't work we need to add the TIEDOWN volume.

                    if(volumeKey == 0) {
                        //  Create the placeholder volume
                        Volume v = new Volume();
                        v.idVolume = IdCounter.incrementCounter();
                        v.name = "TIEDOWN";
                        v.set = volumeSetKey;
                        volumeKey = v.idVolume;
                        volumeTable.insert(v);
                        logMessage("Input Line " + lineNumber +
                            "Added Volume " + v.name +
                            " Database ID=" + v.idVolume +
                            " to volume set ");
                        //  And rebuild the volume list...
                        volumeList = volumeTable.getWhere(
                            "WHERE volume.set='" + volumeSetKey + "'");

                    }

                    Page currentPage = new Page();
                    currentPage.idPage = IdCounter.incrementCounter();
                    currentPage.volume = volumeKey;
                    currentPage.machine = machineKey;
                    currentPage.part = "";
                    currentPage.title = "Added via ImportTieDowns";
                    currentPage.name = logicSource;
                    currentPage.stamp = "";
                    pageTable.insert(currentPage);
                    pageKey = currentPage.idPage;
                    logMessage("Input Line " + lineNumber +
                        " Added page " + logicSource +
                        ", Database ID=" + currentPage.idPage);
                }

                //  Now, use the page to look up the diagram page (and
                //  add it, if needs be

                List<Diagrampage> diagramPageList = diagramPageTable.getWhere(
                    "WHERE diagrampage.page='" + pageKey + "'");

                if(diagramPageList.Count > 0) {
                    diagramPageKey = diagramPageList[0].idDiagramPage;
                }
                else {
                    Diagrampage dp = new Diagrampage();
                    dp.idDiagramPage = IdCounter.incrementCounter();
                    dp.page = pageKey;
                    dp.noHDLGeneration = 0;
                    diagramPageTable.insert(dp);
                    logMessage("Input Line " + lineNumber +
                        " Added Diagram page " + logicSource +
                        ", Database ID=" + dp.idDiagramPage);
                    diagramPageKey = dp.idDiagramPage;
                }

                //  Look up the card type, and possibly add a new one.

                cardTypeKey = getCardType(cardCapCode, "", currentMachineName,
                    currentMachineSerialName, 1);

                //  Look up the card slot (machine/frame/gate/panel/slot)
                //  Adding entries, if we have to.

                //  (NOTE:  A lot of this code is the same as the
                //  Card Location Chart importer - could have
                //  refactored/promoted it...)

                List<Frame> frameList = frameTable.getWhere(
                    "WHERE machine='" + machineKey + "'" +
                    " AND frame.name='" + frameName + "'");
                
                if(frameList.Count > 0) {
                    frameKey = frameList[0].idFrame;
                }
                else {
                    Frame f = new Frame();
                    f.idFrame = IdCounter.incrementCounter();
                    f.machine = machineKey;
                    f.name = frameName;
                    frameTable.insert(f);
                    logMessage("Input Line " + lineNumber +
                        " Added Frame " + frameName +
                        " Database ID=" + f.idFrame +
                        " to machine " + currentMachineName);
                    frameKey = f.idFrame;
                }

                List<Machinegate> machineGateList = machineGateTable.getWhere(
                    "WHERE frame='" + frameKey + "'" +
                    " AND machinegate.name='" + gateName + "'");
                if(machineGateList.Count > 0) {
                    machineGateKey = machineGateList[0].idGate;
                }
                else {
                    Machinegate mg = new Machinegate();
                    mg.idGate = IdCounter.incrementCounter();
                    mg.frame = frameKey;
                    mg.name = gateName;
                    machineGateTable.insert(mg);
                    logMessage("Input Line " + lineNumber +
                        " Added Gate " + gateName +
                        " Database ID=" + mg.idGate +
                        " to Frame " + frameName + " of machine " +
                        currentMachineName);
                    machineGateKey = mg.idGate;
                }

                List<Panel> panelList = panelTable.getWhere(
                    "WHERE gate='" + machineGateKey + "'" +
                    " AND panel='" + panelName + "'");
                if(panelList.Count > 0) {
                    panelKey = panelList[0].idPanel;
                }
                else {
                    Panel p = new Panel();
                    p.idPanel = IdCounter.incrementCounter();
                    p.gate = machineGateKey;
                    p.panel = panelName;
                    panelTable.insert(p);
                    logMessage("Input Line " + lineNumber +
                        " Added Panel " + panelName +
                        " Database ID=" + p.idPanel +
                        " to gate " + gateName +
                        " of frame " + frameName + " of machine " +
                        currentMachineName);
                    panelKey = p.idPanel;
                }

                cardSlotKey = getCardSlotByName(panelKey,
                    rowName, columnNumber);

                //  Look up feature codes.

                machinesWithKey = (machinesWith.Length > 0) ?
                    getFeatureByName(machineKey, machinesWith) : 0;

                machinesWithoutKey = (machinesWithout.Length > 0) ?
                    getFeatureByName(machineKey, machinesWithout) : 0;

                //  Whew.  Now we can see if this tie down already exists...

                Tiedown currentTieDown = null;

                List<Tiedown> tieDownList = tieDownTable.getWhere(
                    "WHERE diagrampage='" + diagramPageKey + "'" +
                    " AND cardType='" + cardTypeKey + "'" +
                    " AND cardSlot='" + cardSlotKey + "'" +
                    " AND pin='" + pinName + "'");

                if(tieDownList.Count > 0) {
                    if(disposition == ImportStartupForm.Disposition.SKIP) {
                        logMessage("Input Line " + lineNumber +
                            " skipping Tie Down for " +
                            "Logic Source " + logicSource +
                            " Location " + location +
                            " Pin " + pinName +
                            " Due to disposition SKIP.");
                    }
                    else if(disposition == ImportStartupForm.Disposition.MERGE) {
                        currentTieDown = tieDownList[0];
                        if(currentTieDown.featureWith == 0) {
                            currentTieDown.featureWith = machinesWithKey;
                        }
                        if(currentTieDown.featureWithout == 0) {
                            currentTieDown.featureWithout = machinesWithoutKey;
                        }
                        if(currentTieDown.otherPin.Length == 0) {
                            currentTieDown.otherPin = otherPinName;
                        }
                        if(currentTieDown.note.Length == 0) {
                            currentTieDown.note = note;
                        }
                        tieDownTable.update(currentTieDown);
                        logMessage("Input Line " + lineNumber +
                            " Updated Tie Down " +
                            "(Database ID " + currentTieDown.idTieDown + ")" +
                            " for Logic Source " + logicSource +
                            " Location " + location +
                            " Pin " + pinName +
                            " Due to disposition MERGE.");
                    }
                    else {
                        // Overwrite
                        currentTieDown = tieDownList[0];
                        currentTieDown.featureWith = machinesWithKey;
                        currentTieDown.featureWithout = machinesWithoutKey;
                        currentTieDown.otherPin = otherPinName;
                        currentTieDown.checkMark =
                            tieDownChecked.Length > 0 ? 1 : 0;
                        currentTieDown.note = note;
                        tieDownTable.update(currentTieDown);
                        logMessage("Input Line " + lineNumber +
                            " Replaced Tie Down " +
                            "(Database ID " + currentTieDown.idTieDown + ")" +
                            " for Logic Source " + logicSource +
                            " Location " + location +
                            " Pin " + pinName +
                            " Due to disposition OVERWRITE.");
                    }
                }
                else {
                    //  Add a new tie down
                    currentTieDown = new Tiedown();
                    currentTieDown.idTieDown = IdCounter.incrementCounter();
                    currentTieDown.diagramPage = diagramPageKey;
                    currentTieDown.cardType = cardTypeKey;
                    currentTieDown.cardSlot = cardSlotKey;
                    currentTieDown.pin = pinName;
                    currentTieDown.featureWith = machinesWithKey;
                    currentTieDown.featureWithout = machinesWithoutKey;
                    currentTieDown.otherPin = otherPinName;
                    currentTieDown.checkMark =
                        tieDownChecked.Length > 0 ? 1 : 0;
                    currentTieDown.note = note;
                    tieDownTable.insert(currentTieDown);
                    logMessage("Input Line " + lineNumber +
                        " Added Tie Down " +
                        "Database ID " + currentTieDown.idTieDown + 
                        " for Logic Source " + logicSource +
                        " Location " + location +
                        " Pin " + pinName);
                }

                //  Bump the counter, the on to the next one...

                ++lineNumber;
            }

            if (testMode) {
                db.CancelTransaction();
            }
            else {
                db.CommitTransaction();
            }

            displayLog();

        }

        private string checkColumn(string column) {
            return csvColumnNames[column] == null ? column + ", " : "";
        }

    }
}
