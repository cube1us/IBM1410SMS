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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using MySQLFramework;

namespace IBM1410SMS
{
    class ImportCardLocationChart : Importer
    {

        Table<Page> pageTable;
        Table<Diagrampage> diagramPageTable;
        Table<Cardlocationpage> cardLocationPageTable;
        Table<Cardlocation> cardLocationTable;
        Table<Cardlocationbottomnote> cardLocationBottomNoteTable;
        Table<Cardlocationblock> cardLocationBlockTable;
        Table<Cardtype> cardTypeTable;
        Table<Cardslot> cardSlotTable;

        Table<Frame> frameTable;
        Table<Machinegate> machineGateTable;
        Table<Panel> panelTable;
        Table<Volumeset> volumeSetTable;
        Table<Volume> volumeTable;

        Cardlocationpage cardLocationPage = null;

        Hashtable csvColumnNames = new Hashtable();
        ImportStartupForm.Disposition disposition;

        static int[] validDiagramColumns = { 1, 2, 3, 4, 5 };

        int[] validColumnList = Enumerable.Range(1, 50).ToList().ToArray();

        string unassignedSheetVolume = "Unassigned Sheet";

        public ImportCardLocationChart(string fileName,
            ImportStartupForm.Disposition disposition,
            bool testMode) : base(fileName) {

            this.disposition = disposition;
            bool header = true;
            List<string> csvColumns;
            List<Volume> volumeList = null;

            int machineKey = 0;
            int panelKey = 0;
            int cardLocationKey = 0;

            int ecoKey = 0;
            int specialEcoKey = 0;
            int runNumber = 0;
            int columnNumber = 0;
            int sheetCoordinateColumn = 0;
            int lineNumber = 1;

            string machineSerial = null;
            string volumeName = null;
            string drawingPart = null;
            string ecoName = null;
            string pageName = null;
            string run = null;
            string machineName = null;
            string frameName = null;

            Page currentPage;

            DBSetup db = DBSetup.Instance;

            pageTable = db.getPageTable();
            diagramPageTable = db.getDiagramPageTable();
            cardLocationPageTable = db.getCardLocationPageTable();
            cardLocationTable = db.getCardLocationTable();
            cardLocationBottomNoteTable = db.getCardLocationBottomNoteTable();
            cardLocationBlockTable = db.getCardLocationBlockTable();
            cardTypeTable = db.getCardTypeTable();
            cardSlotTable = db.getCardSlotTable();

            frameTable = db.getFrameTable();
            machineGateTable = db.getMachineGateTable();
            panelTable = db.getPanelTable();
            volumeSetTable = db.getVolumeSetTable();
            volumeTable = db.getVolumeTable();


            Regex bottomNotePattern = new Regex(
                @"\s*(?:(?<val>\d+)|(?<val>\*))(?:\s+|$)");

            //  \s*                     Ignore leading whitespace
            //  (?:                     Group of value alternatives
            //      (?<val>\d+)         Either a group of digits
            //      | (?<val>\*)        Or an asterisk
            //  )                       End alternatives group
            //  (?:\s+,$)               Followed by white space or end of string

            while ((csvColumns = getCSVColumns()).Count > 0) {

                //  Snag the header line so we can search for columns by name.
                //  That way, if we reorder columns, insert a column, etc.,
                //  the code will still work.

                if (header) {
                    header = false;
                    int columnIndex = 0;
                    foreach (string s in csvColumns) {
                        if (s.Length > 0) {
                            csvColumnNames.Add(s, columnIndex);
                        }
                        ++columnIndex;
                    }

                    //  Check for required columns...  Column order does not
                    //  matter.  (Only column Volume is optional)

                    string missingColumns = "";
                    missingColumns += checkColumn("Machine Serial");
                    missingColumns += checkColumn("Volume");
                    missingColumns += checkColumn("Drawing P/N");
                    missingColumns += checkColumn("E.C.");
                    missingColumns += checkColumn("Page");
                    missingColumns += checkColumn("Run");
                    missingColumns += checkColumn("Machine");
                    missingColumns += checkColumn("Frame");
                    missingColumns += checkColumn("Gate");
                    missingColumns += checkColumn("Panel");
                    missingColumns += checkColumn("Row");
                    missingColumns += checkColumn("Column");
                    missingColumns += checkColumn("Double Card Height");
                    missingColumns += checkColumn("Card Type");
                    missingColumns += checkColumn("Card P/N");
                    missingColumns += checkColumn("MF 1 Feature Code");
                    missingColumns += checkColumn("Crossed Out");
                    missingColumns += checkColumn("Sheet");
                    missingColumns += checkColumn("Sheet Coordinate");
                    missingColumns += checkColumn("Ignore");
                    missingColumns += checkColumn("Special EC");
                    missingColumns += checkColumn("Bottom Note?");
                    missingColumns += checkColumn("Notes");

                    if (missingColumns.Length > 0) {
                        MessageBox.Show("Input Line " + lineNumber + 
                            ": One or more input columns are missing: \n" + 
                            missingColumns,
                            "Missing Column(s)",
                            MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        closeLog();
                        return;
                    }

                    //  And then on to the first real data row.  
                    //  NOTE:  Wait for first row of actual data before starting
                    //  transaction to make sure we have the info we need.

                    ++lineNumber;
                    continue;
                }

                //  We are past the headers, so get the column data, but skip
                //  certain ones after the first row.

                if (cardLocationPage == null) {
                    machineSerial = csvColumns[(int)csvColumnNames["Machine Serial"]];
                    volumeName = csvColumns[(int)csvColumnNames["Volume"]];
                    drawingPart = csvColumns[(int)csvColumnNames["Drawing P/N"]];
                    ecoName = csvColumns[(int)csvColumnNames["E.C."]];
                    pageName = csvColumns[(int)csvColumnNames["Page"]];
                    run = csvColumns[(int)csvColumnNames["Run"]];
                    machineName = csvColumns[(int)csvColumnNames["Machine"]];
                    frameName = csvColumns[(int)csvColumnNames["Frame"]];
                }

                //  Always get the latest data for the following columns...

                string gateName = csvColumns[(int)csvColumnNames["Gate"]];
                string panelName = csvColumns[(int)csvColumnNames["Panel"]];
                string rowName = csvColumns[(int)csvColumnNames["Row"]];
                string columnName = csvColumns[(int)csvColumnNames["Column"]];
                string doubleHeight = csvColumns[(int)csvColumnNames["Double Card Height"]];
                string cardTypeName = csvColumns[(int)csvColumnNames["Card Type"]];
                string cardPartNumber = csvColumns[(int)csvColumnNames["Card P/N"]];
                string featureName = csvColumns[(int)csvColumnNames["MF 1 Feature Code"]];
                string crossedOut = csvColumns[(int)csvColumnNames["Crossed Out"]];
                string sheetName = csvColumns[(int)csvColumnNames["Sheet"]];
                string sheetCoordinate = csvColumns[(int)csvColumnNames["Sheet Coordinate"]];
                string ignore = csvColumns[(int)csvColumnNames["Ignore"]];
                string specialEC = csvColumns[(int)csvColumnNames["Special EC"]];
                string bottomNotes = csvColumns[(int)csvColumnNames["Bottom Note?"]];
                string notes = csvColumns[(int)csvColumnNames["Notes"]];

                //  Some columns must ALWAYS have data

                if(rowName.Length == 0 || columnName.Length == 0 ||
                    cardTypeName.Length == 0 || 
                    (ignore.Length == 0 && crossedOut.Length == 0 &&
                        (sheetName.Length == 0 || sheetCoordinate.Length == 0)) ) {
                    MessageBox.Show("Row, Column, Card Type, Sheet or " + 
                        "Sheet Coordinate not specified at line " + lineNumber, 
                        "Required Column Empty",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    db.CancelTransaction();
                    closeLog();
                    return;
                }

                //  Check that the card column and first character of
                //  sheet coordinate are integers, and within range.

                if(!int.TryParse(columnName, out columnNumber) ||
                    Array.IndexOf(validColumnList,columnNumber) < 0) {
                    MessageBox.Show("Input Line " + lineNumber + "" +
                        ": Card Column must be an integer [1-50]: (" +
                        columnName + ")" + " [" + columnNumber + "]",
                        "Invalid Card Column",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    db.CancelTransaction();
                    closeLog();
                    return;
                }

                sheetCoordinateColumn = 0;
                if(sheetCoordinate.Length > 0 && ( 
                    !int.TryParse(sheetCoordinate.Substring(0,1), 
                    out sheetCoordinateColumn) ||
                    Array.IndexOf(validDiagramColumns,sheetCoordinateColumn) < 0)) {
                    MessageBox.Show("Input Line " + lineNumber + "" +
                        ": First char of sheet coordinate must be an integer [1-5]: (" +
                        sheetCoordinate + ")",
                        "Invalid Sheet Coordinate Column",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    db.CancelTransaction();
                    closeLog();
                    return;
                }

                //  Check that card row and sheet row are valid as well...

                if(Array.IndexOf(Helpers.validRows,rowName) < 0) {
                    MessageBox.Show("Input Line " + lineNumber + "" +
                        ": Card Row invalid (" +
                        rowName + ")",
                        "Invalid Card Row",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    db.CancelTransaction();
                    closeLog();
                    return;
                }

                if(sheetCoordinate.Length > 0 && 
                    Array.IndexOf(
                        Helpers.validDiagramRows,sheetCoordinate.Substring(1,1)) < 0) {
                    MessageBox.Show("Input Line " + lineNumber + "" +
                        ": Sheet Row invalid (" +
                        sheetCoordinate + ")",
                        "Invalid Sheet Row",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    db.CancelTransaction();
                    closeLog();
                    return;
                }

                //  The first data row must have certain fields filled in.

                if (cardLocationPage == null) {

                    //  Start a single transaction for all of this.

                    db.BeginTransaction();

                    machineKey = getMachineKeyByName(machineName);
                    if (machineKey == 0) { 
                        MessageBox.Show("ERROR:  Machine named " + machineName +
                            "not found in database.  Aborting import.",
                            "Machine Not Found",
                            MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        db.CancelTransaction();
                        closeLog();
                        return;
                    }

                    //  Get the key for the specified volume in this row.

                    int volumeKey = getVolumeKeyByName(volumeName, machineName,
                        machineSerial);

                    //  Also, create a list of volumes for this machine to use
                    //  to look up the volumes for sheet refereces.

                    volumeList = getVolumeList(machineSerial, machineName);

                    if (pageName.Length == 0) {
                        MessageBox.Show("ERROR: Page Name not specified.",
                            "Page Name Not Specified",
                            MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        db.CancelTransaction();
                        closeLog();
                        return;

                    }

                    if (drawingPart.Length == 0 || ecoName.Length == 0) {
                        MessageBox.Show("ERROR: Drawing P/N or E.C. not specified.",
                            "Drawing P/N Not Specified",
                            MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        db.CancelTransaction();
                        closeLog();
                        return;
                    }
                    
                    if(run.Length == 0 || gateName.Length == 0 ||
                        panelName.Length == 0 || frameName.Length == 0) {
                        MessageBox.Show("ERROR: Run, Gate, Frame, or Panel not specified",
                            "Run, Gate or Panel Not Specified",
                            MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        db.CancelTransaction();
                        closeLog();
                        return;
                    }

                    if(!int.TryParse(run, out runNumber)) {
                        MessageBox.Show("ERROR: Run must be an integer number",
                            "Run Must Be an Integer",
                            MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        db.CancelTransaction();
                        closeLog();
                        return;
                    }

                    //  Get the key to the specified ECO (which will be added
                    //  if not already present.

                    ecoKey = getECOByName(machineKey, ecoName);

                    //  See if this page already exists.  If not, we will add it.

                    List<Page> pageList = pageTable.getWhere(
                        "WHERE machine='" + machineKey + "'" +
                        " AND volume='" + volumeKey + "'" +
                        " AND part='" + drawingPart + "'" +
                        " AND page.name='" + pageName + "'");

                    if(pageList.Count > 0) {
                        currentPage = pageList[0];
                    }
                    else {
                        currentPage = new Page();
                        currentPage.idPage = IdCounter.incrementCounter();
                        currentPage.volume = volumeKey;
                        currentPage.machine = machineKey;
                        currentPage.part = drawingPart;
                        currentPage.title = "Added via ImportCardLocationChart";
                        currentPage.name = pageName;
                        currentPage.stamp = "";
                        pageTable.insert(currentPage);
                        logMessage("Input Line " + lineNumber + 
                            " Added page " + pageName +
                            ", Database ID=" + currentPage.idPage);
                    }

                    //  Similarly, if the card location page does not already
                    //  exist, we will add it now.

                    List<Cardlocationpage> clpList = cardLocationPageTable.getWhere(
                        "WHERE cardlocationpage.page='" + currentPage.idPage + "'" +
                        " AND eco='" + ecoKey + "'");
                
                    if(clpList.Count > 0) {
                        cardLocationPage = clpList[0];
                        panelKey = cardLocationPage.panel;
                    }
                    else {
                        panelKey = 0;
                    }

                    if(panelKey == 0) { 

                        //  Find the panel.  It is required.

                        List<Frame> frameList = frameTable.getWhere("" +
                            "WHERE machine='" + machineKey + "'" +
                            " AND frame.name='" + frameName + "'");
                        foreach(Frame frame in frameList) {
                            List<Machinegate> gateList = machineGateTable.getWhere("" +
                                "WHERE frame='" + frame.idFrame + "'" +
                                " AND machinegate.name='" + gateName + "'");
                            foreach(Machinegate gate in gateList) {
                                List<Panel> panelList = panelTable.getWhere(
                                    "WHERE gate='" + gate.idGate + "'" +
                                    " AND panel='" + panelName + "'");
                                if(panelList.Count > 0) {
                                    panelKey = panelList[0].idPanel;
                                }
                            }
                        }

                        if(panelKey == 0) {
                            MessageBox.Show("Input Line " + lineNumber + "" +
                                " No Database entry found for Machine " +
                                machineName + ", Frame " + frameName +
                                ", Gate " + gateName + ", Panel " + panelName,
                                "No Panel Entry Found",
                                MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            db.CancelTransaction();
                            closeLog();
                            return;
                        }

                        //  Add a new card location page...
                        cardLocationPage = new Cardlocationpage();
                        cardLocationPage.idCardLocationPage = IdCounter.incrementCounter();
                        cardLocationPage.page = currentPage.idPage;
                        cardLocationPage.eco = ecoKey;
                        cardLocationPage.panel = panelKey;
                        cardLocationPage.run = runNumber;
                        cardLocationPage.previousECO = 0;       // Previous ECO not in CSV
                        cardLocationPage.sheets = 2;            // These take two sheets each.
                        cardLocationPageTable.insert(cardLocationPage);
                        logMessage("Input Line " + lineNumber + 
                            " Added Card Location Page " + pageName +
                            " for Machine " + machineName + 
                            ", Frame " + frameName + ", Gate " + gateName + 
                            ", Panel " + panelName);
                    }

                    //  Now, if the panel in the (pre existing) 
                    //  card location page is 0, replace it with the new one.

                    if(cardLocationPage.panel == 0) {
                        cardLocationPage.panel = panelKey;
                        cardLocationPageTable.update(cardLocationPage);
                        logMessage("Input Line " + lineNumber +
                            "Updated Card Location Page (Database ID" +
                            cardLocationPage.idCardLocationPage +
                            " for sheet " + sheetName +
                            " to add panel key");
                    }
                    
                }        //  End of First Row Data gathering.

                //  Gather Card Location data for every row but
                //  the title row.

                int cardSlotKey = getCardSlotByName(panelKey, rowName,
                    columnNumber);

                if(specialEC.Length > 0) {
                    specialEcoKey = getECOByName(machineKey, specialEC);
                }
                else {
                    specialEcoKey = 0;
                }

                //  See if this Card Location already exists.  If so,
                //  handle according to disposition.  If not, add it.

                int cardTypeKey = getCardType(cardTypeName, cardPartNumber,
                    machineName, machineSerial,
                    doubleHeight.Length > 0 ? 2 : 1);

                List<Cardlocation> cardLocationList = cardLocationTable.getWhere(
                    "WHERE cardlocation.page='" + cardLocationPage.idCardLocationPage + 
                    "'" + " AND cardSlot='" + cardSlotKey + "'");

                if(cardLocationList.Count > 0) {
                    Cardlocation cardLocation = cardLocationList[0];
                    cardLocationKey = cardLocation.idCardLocation;
                    if(disposition == ImportStartupForm.Disposition.SKIP) {
                        //  Do nothing in this case.
                    }
                    else if (disposition == ImportStartupForm.Disposition.MERGE) { 
                        //  The only optional field to merge in is feature...
                        if(cardLocation.feature == 0 &&
                            featureName.Length > 0) {
                            cardLocation.feature = getFeatureByName(
                                machineKey, featureName);
                        }
                    }
                    else {
                        //  Overwrite...  Page and Slot would not change.
                        cardLocation.type = cardTypeKey;
                        if (featureName.Length > 0) {
                            cardLocation.feature = getFeatureByName(
                                machineKey, featureName);
                        }
                        cardLocation.crossedOut = 
                            crossedOut.Length > 0 ? 1 : 0;
                        cardLocationTable.update(cardLocation);
                    }
                }
                else {
                    //  Add a new card location...
                    Cardlocation newCardLocation = new Cardlocation();
                    newCardLocation.idCardLocation = IdCounter.incrementCounter();
                    newCardLocation.page = cardLocationPage.idCardLocationPage;
                    newCardLocation.cardSlot = cardSlotKey;
                    newCardLocation.type = cardTypeKey;
                    newCardLocation.crossedOut =
                        crossedOut.Length > 0 ? 1 : 0;
                    if (featureName.Length > 0) {
                        newCardLocation.feature = getFeatureByName(machineKey,
                            featureName);
                    }
                    else {
                        newCardLocation.feature = 0;

                    }

                    cardLocationTable.insert(newCardLocation);
                    logMessage("Input Line " + lineNumber + 
                        " Added Card Location Database ID=" +
                        newCardLocation.idCardLocation + 
                        " for card type " +
                        cardTypeName + " at slot " + rowName + " " +
                        columnName);
                    cardLocationKey = newCardLocation.idCardLocation;
                }

                //  Now add any bottom notes that are not already there.
                //  In the usual case where the bottom notes are repeated in
                //  the input data for all of the sheet references for a given
                //  card location, this will be executed each time, but will 
                //  only add a given note once.

                Cardlocationbottomnote newNote = null;

                Match match = bottomNotePattern.Match(bottomNotes);
                
                //  If it didn't match at all, then put up to 8 chars in as is.
                //  In this case, there can be only one such note.

                if(!match.Success && bottomNotes.Length > 0) {
                    string tempNote = (bottomNotes.Length > 10) ? bottomNotes.Substring(0, 10) :
                        bottomNotes;
                    List<Cardlocationbottomnote> bnl =
                        cardLocationBottomNoteTable.getWhere(
                            "WHERE cardLocation='" + cardLocationKey + "'" +
                            " AND note='" + tempNote + "'");
                    if (bnl.Count == 0) {
                        newNote = new Cardlocationbottomnote();
                        newNote.idCardLocationBottomNote =
                            IdCounter.incrementCounter();
                        newNote.cardLocation = cardLocationKey;
                        newNote.note = bottomNotes;
                        cardLocationBottomNoteTable.insert(newNote);
                        logMessage("Input Line " + lineNumber +
                            " Added string bottom note " + tempNote +
                            " Database ID=" + newNote.idCardLocationBottomNote +
                            " for card type " + cardTypeName +
                            " at card row " + rowName + " column " + columnName);
                    }
                }

                //  Otherwise, handle the parsed bits...

                while (match.Success) {
                    string note = match.Groups["val"].Value;
                    //  See if this note already exists.  If not, add it.
                    List<Cardlocationbottomnote> bnl =
                        cardLocationBottomNoteTable.getWhere(
                            "WHERE cardLocation='" + cardLocationKey + "'" +
                            " AND note='" + note + "'");
                    if(bnl.Count == 0) {
                        newNote = new Cardlocationbottomnote();
                        newNote.idCardLocationBottomNote =
                            IdCounter.incrementCounter();
                        newNote.cardLocation = cardLocationKey;
                        newNote.note = note;
                        cardLocationBottomNoteTable.insert(newNote);
                        logMessage("Input Line " + lineNumber + 
                            " Added bottom note " + note +
                            " Database ID=" + newNote.idCardLocationBottomNote +
                            " for card type " + cardTypeName +
                            " at card row " + rowName + " column " + columnName);
                    }
                    match = match.NextMatch();
                }

                //  Finally, add the Card Location Block sheet reference...
                //  Unless the sheet is already there, we won't know the
                //  volume.  In such cases we will use a made up volume.
                //  We need to restrict the search to the volume set in
                //  play for this import - not just by machine.

                //  Don't bother with this if there is no sheet reference.

                if(sheetName.Length == 0) {
                    ++lineNumber;
                    continue;
                }

                int sheetKey = 0;

                Diagrampage newDiagramPage = null;

                foreach(Volume v in volumeList) {
                    List<Page> pageList = pageTable.getWhere(
                        "WHERE machine='" + machineKey + "'" +
                        " AND volume='" + v.idVolume + "'" +
                        " AND page.name='" + sheetName + "'");
                    if(pageList.Count > 0) {
                        //  We found the right page.
                        //  Now find the corresponding diagram page.
                        //  If it isn't there, add it now.
                        List<Diagrampage> diagramPageList = diagramPageTable.getWhere(
                            "WHERE diagrampage.page='" + pageList[0].idPage + "'");
                        if(diagramPageList.Count > 0) {
                            sheetKey = diagramPageList[0].idDiagramPage;
                            break;
                        }
                        else {
                            newDiagramPage = new Diagrampage();
                            newDiagramPage.idDiagramPage = IdCounter.getCounter();
                            newDiagramPage.noHDLGeneration = 0;
                            newDiagramPage.page = pageList[0].idPage;
                            diagramPageTable.insert(newDiagramPage);
                            logMessage("Input Line " + lineNumber + 
                                " Added Diagram Page " +
                                " Database ID=" + newDiagramPage.idDiagramPage +
                                " for page " + sheetName);
                            sheetKey = newDiagramPage.idDiagramPage;
                            break;
                        }
                    }
                }

                //  If we didn't find the sheet, add it now.  If possible, find
                //  which volume the sheet should be in using firstPage/lastPage.
                //  If that fails, use our special unassigned Sheet Volume name.
                //  If we use the latter, also add the volume, if necessary.

                if (sheetKey == 0) {

                    int volumeKey = 0;
                    Volume useVolume = 
                        volumeList.Find(x => x.firstPage != null && 
                            x.lastPage != null && 
                            x.firstPage.CompareTo(sheetName) <= 0 &&
                            x.lastPage.CompareTo(sheetName) >= 0);
                    if (useVolume == null || useVolume.idVolume == 0) {
                        volumeKey = getVolumeKeyByName(unassignedSheetVolume,
                            machineName, machineSerial);
                        //  Since the above might add a volume, rebuild the volume list 
                        //  for next time.
                        volumeList = getVolumeList(machineSerial, machineName);
                    }
                    else {
                        volumeKey = useVolume.idVolume;
                    }


                    Page newPage = new Page();
                    newPage.idPage = IdCounter.incrementCounter();
                    newPage.machine = machineKey;
                    newPage.volume = volumeKey;
                    newPage.part = "";
                    newPage.title = "Added via Card Location Chart Import";
                    newPage.name = sheetName;
                    newPage.stamp = "";
                    pageTable.insert(newPage);

                    newDiagramPage = new Diagrampage();
                    newDiagramPage.idDiagramPage = IdCounter.incrementCounter();
                    newDiagramPage.page = newPage.idPage;
                    newDiagramPage.noHDLGeneration = 0;
                    diagramPageTable.insert(newDiagramPage);
                    sheetKey = newDiagramPage.idDiagramPage;

                    logMessage("Input Line " + lineNumber + 
                        " Added page and diagram page for sheet " +
                        sheetName +
                        ", page Database ID=" + newPage.idPage +
                        ", diagram page Database ID=" + newDiagramPage.idDiagramPage);
                }

                //  See if we have an existing card location block...

                List<Cardlocationblock> clbList = cardLocationBlockTable.getWhere(
                    "WHERE cardLocation='" + cardLocationKey + "'" +
                    " AND diagramPage='" + sheetKey + "'" + 
                    " AND diagramRow='" + 
                        (sheetCoordinate.Length > 0 ? 
                        sheetCoordinate.Substring(1, 1) : "") + "'" +
                    " AND diagramColumn='" + sheetCoordinateColumn + "'");

                if(clbList.Count > 0) {
                    Cardlocationblock clb = clbList[0];
                    if(disposition == ImportStartupForm.Disposition.SKIP) {
                        //  Do thing for skip.
                        logMessage("Input Line " + lineNumber + 
                            " Existing Card Location Block " +
                            "(Database ID " + clb.idCardLocationBlock +
                            ") for Card Type " + cardTypeName + 
                            " referencing sheet " + sheetName +
                            " at row " + rowName +
                            " and column " + columnName +
                            " not updated due to disposition SKIP");
                    }
                    else if(disposition == ImportStartupForm.Disposition.MERGE) {
                        if(clb.diagramECO == 0 && specialEcoKey > 0) {
                            clb.diagramECO = specialEcoKey;
                        }
                        //  Ignore the boolean flags on a merge...
                        if(clb.note.Length == 0) {
                            clb.note = notes;
                        }
                        if (sheetCoordinate.Length > 0) {
                            if (clb.diagramRow.Length == 0) {
                                clb.diagramRow = sheetCoordinate.Substring(1, 1);
                            }
                            if (clb.diagramColumn == 0) {
                                clb.diagramColumn = sheetCoordinateColumn;
                            }
                        }
                        cardLocationBlockTable.update(clb);
                        logMessage("Input Line " + lineNumber + 
                            " Existing Card Location Block " +
                            "(Database ID " + clb.idCardLocationBlock +
                            ") for Card Type " + cardTypeName +
                            " referencing sheet " + sheetName +
                            " at row " + rowName +
                            " and column " + columnName +
                            " updated due to disposition MERGE");
                    }
                    else {
                        //  Overwrite
                        if (sheetCoordinate.Length > 0) {
                            clb.diagramRow = sheetCoordinate.Substring(1, 1);
                            clb.diagramColumn = sheetCoordinateColumn;
                        }
                        else {
                            clb.diagramRow = "";
                            clb.diagramColumn = 0;
                        }
                        clb.diagramECO = specialEcoKey;
                        clb.identifiedOnSheet =
                            notes.IndexOf("using sheet",
                                StringComparison.CurrentCultureIgnoreCase) >= 0 ?
                                1 : 0;
                        clb.ignore = ignore.Length > 0 ? 1 : 0;
                        clb.missingDiagram = 
                            notes.IndexOf("missing", 
                            StringComparison.CurrentCultureIgnoreCase) >= 0 ?
                                1 : 0;
                        clb.note = notes;
                        cardLocationBlockTable.update(clb);
                        logMessage("Input Line " + lineNumber + 
                            " Existing Card Location Block " +
                            "(Database ID " + clb.idCardLocationBlock +
                            ") for Card Type " + cardTypeName +
                            " referencing sheet " + sheetName +
                            " at row " + rowName +
                            " and column " + columnName +
                            " replaced due to disposition OVERWRITE");
                    }
                }
                else {

                    //  Add a brand new card location block entry

                    Cardlocationblock newClb = new Cardlocationblock();
                    newClb.idCardLocationBlock = IdCounter.incrementCounter();
                    newClb.cardLocation = cardLocationKey;
                    newClb.diagramPage = sheetKey;

                    if (sheetCoordinate.Length > 0) {
                        newClb.diagramRow = sheetCoordinate.Substring(1, 1);
                        newClb.diagramColumn = sheetCoordinateColumn;
                    }
                    else {
                        newClb.diagramRow = "";
                        newClb.diagramColumn = 0;
                    }
                    newClb.diagramECO = specialEcoKey;
                    newClb.identifiedOnSheet =
                        notes.IndexOf("using sheet",
                            StringComparison.CurrentCultureIgnoreCase) >= 0 ?
                            1 : 0;
                    newClb.ignore = ignore.Length > 0 ? 1 : 0;
                    newClb.missingDiagram =
                        notes.IndexOf("missing",
                        StringComparison.CurrentCultureIgnoreCase) >= 0 ?
                            1 : 0;
                    newClb.note = notes;
                    cardLocationBlockTable.insert(newClb);
                    logMessage("Input Line " + lineNumber +
                        " Added Card Location Block" +
                        " Database ID=" + newClb.idCardLocationBlock +
                        " for Card Type " + cardTypeName +
                        " referencing sheet " + sheetName +
                        " row " + newClb.diagramRow +
                        ", column " + newClb.diagramColumn);
                }
                ++lineNumber;
            }

            //  We are all done...

            if(testMode) {
                db.CancelTransaction();
            }
            else {
                db.CommitTransaction();
            }

            displayLog();            
        }

        //  Routine to get a volume list.  We have it here, because it needs
        //  to be done at the beginning, and if we later add a volume to
        //  contain sheets where we don't yet know the volume.

        private List<Volume> getVolumeList(string machineSerial, string machineType) {
            List<Volumeset> volumeSetList = volumeSetTable.getWhere(
                "WHERE machineType='" + machineType + "'" +
                " AND machineSerial='" + machineSerial + "'");

            //  If there is a volume set, get all of the volumes in that set.

            if (volumeSetList.Count > 0) {
                return (volumeTable.getWhere(
                    "WHERE volume.set='" + volumeSetList[0].idVolumeSet + "'"));
            }
            else {

                //  If there is no volume set, call getVolumeKeyByName using
                //  our special made up name to create it, and the volume set,
                //  then return the (one and only) newly created volume.

                int volumeKey = getVolumeKeyByName(unassignedSheetVolume,
                    machineType, machineSerial);
                List<Volume> volumeList = new List<Volume>();
                volumeList.Add(volumeTable.getByKey(volumeKey));
                return (volumeList);
            }
        } 

        //  Utility routine to check if a required column name is present, and
        //  return the column name follwed by a space if it is NOT present

        private string checkColumn(string column) {
            return csvColumnNames[column] == null ? column + ", " : "";
        }

    }       
}
