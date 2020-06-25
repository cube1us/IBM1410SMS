using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Windows.Forms;
using MySQLFramework;

namespace IBM1410SMS
{
    class ImportBuses : Importer
    {

        ImportStartupForm.Disposition disposition;
        Hashtable csvColumnNames = new Hashtable();

        public ImportBuses(string fileName,
            ImportStartupForm.Disposition disposition, bool testMode) : base(fileName) {

            this.disposition = disposition;

            Table<Bussignals> busSignalsTable;
            Table<Machine> machineTable;
            Table<Sheetedgeinformation> sheetEdgeInformationTable;

            bool header = true;
            Bussignals busSignal = null;
            List<string> usedBitNames = null;
            int machineKey = 0;
            string currentMachineName = "";
            int lineCount = 0;
            bool skippingBits = false;
            string lastUsedBit = "";
            bool cancelTransaction = false;
            int tildeIndex = -1;

            DBSetup db = DBSetup.Instance;
            machineTable = db.getMachineTable();
            busSignalsTable = db.getBusSignalsTable();
            sheetEdgeInformationTable = db.getSheetEdgeInformationTable();

            List<string> csvColumns;


            if (testMode) {
                logMessage("NOTE:  TEST MODE.  NO DATA WILL BE WRITTEN");
            }

            while ((csvColumns = getCSVColumns()).Count > 0) {

                ++lineCount;

                //  Process the header line.

                if (header) {
                    header = false;
                    int columnIndex = 0;
                    foreach (string s in csvColumns) {
                        if(csvColumnNames.Contains(s)) {
                            MessageBox.Show("Invalid or duplicate column name " +
                                s + ", column " + columnIndex.ToString(), 
                                "Invalid Column Name",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        csvColumnNames.Add(s, columnIndex);
                        ++columnIndex;
                    }

                    header = false;

                    //  CHeck for required columns.  Column order doesn't matter.

                    string missingColumns = "";
                    missingColumns += checkColumn("Machine");
                    missingColumns += checkColumn("Signal");
                    missingColumns += checkColumn("BITS");
                    missingColumns += checkColumn("BUSName");

                    if (missingColumns.Length > 0) {
                        MessageBox.Show("One or more input columns are " +
                            "missing: \n" + missingColumns,
                            "Missing Column(s)",
                            MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        closeLog();
                        return;
                    }

                    //  Columns were OK, so start up the transaction.

                    if (!testMode) {
                        db.BeginTransaction();
                    }
                    continue;
                }

                //  Subsequent lines

                string machineName = csvColumns[(int)csvColumnNames["Machine"]];
                string signalName = csvColumns[(int)csvColumnNames["Signal"]];
                string bitsUsed = csvColumns[(int)csvColumnNames["BITS"]];
                string busName = csvColumns[(int)csvColumnNames["BUSName"]];

                //  Machine name was blank, or the bits used field says "MANUAL"
                //  then skip the line.

                if (machineName.Length <= 0 || machineName.Substring(0, 1) == " ") {
                    continue;
                }

                if (bitsUsed.ToUpper() == "MANUAL") {
                    continue;
                }

                //  Get the machine for this row

                List<Machine> machineList = machineTable.getWhere(
                    "WHERE machine.name='" + machineName + "'");
                if (machineList.Count > 0) {
                    machineKey = machineList[0].idMachine;
                    currentMachineName = machineName;
                }
                else {
                    logMessage("ERROR: Input Line " + lineCount + " unknown machine name: " +
                        machineName);
                    cancelTransaction = true;
                    continue;
                }

                //  Check to make sure that the signal name has exactly one
                //  tilde (~) placeholder character

                if(signalName.Count(c => (c == '~' )) != 1) {
                    logMessage("ERROR: Input Line " + lineCount + " Signal name '" +
                        signalName + "' does not have " +
                        "exactly one tidle (~) character.");
                    cancelTransaction = true;
                }

                tildeIndex = signalName.IndexOf('~');

                //  Decode the bits used field

                //  If we see a [, then the bits that appear until the matching ]
                //  are available on the bus bit vector, but not used, so  we don't 
                //  genereate a busSignals entry for it.  This is so that bits
                //  like the 1410 CWBA8421 always come out in the same place.

                usedBitNames = new List<string>();
                lastUsedBit = "";
                skippingBits = false;

                for (int bitIndex = 0; bitIndex < bitsUsed.Length; ++bitIndex) {

                    string currentBit = bitsUsed.Substring(bitIndex, 1);

                    //  A W in the bits used field means word mark.

                    if (currentBit == "W" && machineName.Substring(0, 2) == "14") {
                        currentBit = "WM";
                    }

                    //  A "[" means that we start skipping bits.

                    if (currentBit == "[") {
                        if (skippingBits) {
                            logMessage("ERROR: Input Line " + lineCount +
                                " nested or unmatched brackets [] in " +
                                bitsUsed);
                            cancelTransaction = true;
                            continue;
                        }
                        skippingBits = true;
                        continue;
                    }

                    //  If skipping bits, a ] terminates the skip, otherwise
                    //  we just add to the count of bits we need for the bit vector.

                    if (skippingBits) {
                        if (currentBit == "]") {
                            skippingBits = false;
                        }
                        else {
                            usedBitNames.Add("-");      // Add a placeholder bit
                        }
                        continue;
                    }

                    //  If not skipping bits, a ] is in error.

                    if (currentBit == "]") {
                        MessageBox.Show("ERROR: Input Line " + lineCount +
                            " unmatched bracket ] in " +
                            bitsUsed);
                        cancelTransaction = true;
                        continue;
                    }

                    //  Normal bit name....

                    usedBitNames.Add(currentBit);   // Add a real bit
                    lastUsedBit = currentBit;
                }

                //  Make sure we did not end while skipping

                if (skippingBits) {
                    logMessage("ERROR: Input Line " + lineCount +
                        " unmtached brackets [] in " + bitsUsed);
                    cancelTransaction = true;
                    continue;
                }

                if(bitsUsed.Length == 0) {
                    logMessage("ERROR: Input Line " + lineCount +
                        " no used bits.");
                    cancelTransaction = true;
                    continue;
                }

                string bitsUsedString = "";
                foreach(string s in usedBitNames) {
                    bitsUsedString += s;
                }
                logMessage("Input Line " + lineCount + " Bits " +
                    bitsUsed + ", Used Bits: " + bitsUsedString);

                //  Generate the bus database entries here.

                int bitPosition = usedBitNames.Count;
                foreach(string s in usedBitNames) {
                    if(s == "-") {
                        //  Skip this bit position numberr
                        --bitPosition;
                        continue;
                    }

                    busSignal = new Bussignals();
                    busSignal.signalName = signalName.Substring(0, tildeIndex) +
                        s + signalName.Substring(tildeIndex + 1);
                    busSignal.busName = busName;
                    busSignal.busBit = bitPosition - 1;
                    busSignal.machine = machineKey;

                    --bitPosition;

                    //logMessage("DEBUG: Line " + lineCount + " signal " +
                    //    busSignal.signalName + " is bus " + busSignal.busName +
                    //    " bit " + bitPosition.ToString());

                    //  Check to see if there is actually a signal in the
                    //  sheetedgeinformation table that matches - warn if not.

                    List<Sheetedgeinformation> sheetEdgeInformation =
                        sheetEdgeInformationTable.getWhere(
                            "WHERE signalName = '" + busSignal.signalName + "'");

                    if(sheetEdgeInformation == null || sheetEdgeInformation.Count < 1) {
                        logMessage("WARNING: Line " + lineCount + " ALD signal " +
                            busSignal.signalName + " is not in sheet edge information " +
                            "table.");
                    }

                    //  Check to see if this signal already exists.  If so, 
                    //  use the overwrite/merge/skip setting as appropriate.  (In
                    //  this case, merge isn't really relevant).

                    List<Bussignals> busSignalList = busSignalsTable.getWhere(
                        "WHERE signalName = '" + busSignal.signalName + "'");

                    if(busSignalList != null && busSignalList.Count == 1) {
                        if(disposition == ImportStartupForm.Disposition.SKIP ||
                            disposition == ImportStartupForm.Disposition.MERGE) {
                            //  SKIP or MERGE - do not overwrite
                            logMessage("WARNING: Line " + lineCount + " signal " +
                                busSignal.signalName + " already exists, " +
                                "dialog set to SKIP or MERGE -- signal ignored.");
                            continue;
                        }
                        else {
                            //  OVERWRITE
                            logMessage("WARNING: Line " + lineCount + " signal " +
                                busSignal.signalName + " already exists, " +
                                "dialog set to OVERWRITE - will be overwritten");
                            busSignal.idBusSignal = busSignalList[0].idBusSignal;
                            if(!testMode && !cancelTransaction) {
                                busSignalsTable.update(busSignal);
                            }
                        }
                    }
                    else {
                        //  INSERT NEW
                        if(!testMode && !cancelTransaction) {
                            busSignal.idBusSignal = IdCounter.incrementCounter();
                            busSignalsTable.insert(busSignal);
                        }
                    }
                }

            }

            if(!testMode && cancelTransaction) {
                db.CancelTransaction();
                logMessage("ERROR: Transaction Cancelled due to previous errors.");
            }
            else if(!testMode) {
                db.CommitTransaction();
            }

            displayLog();

        }

        private string checkColumn(string column) {
            return csvColumnNames[column] == null ? column + ", " : "";
        }

    }
}
