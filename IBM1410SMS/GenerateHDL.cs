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
using System.IO;

namespace IBM1410SMS
{
    class GenerateHDL
    {
        const string LatchPrefix = "Latch";
        const string TestBenchTemplate = "TestBenchTemplate";
        const string TestBenchFPGAClock = "TestBenchFPGAClock";
        const string TestBenchFPGAClockTag = "<FPGA CLOCK>";
        const string TestBenchDeclares = "TestBenchDeclares";


        GenerateHDLLogic generator;

        Page page;
        string directory;
        string logPathName = "";
        string outPathName = "";


        Table<Diagrampage> diagramPageTable;
        Table<Diagramblock> diagramBlockTable;
        Table<Sheetedgeinformation> sheetEdgeInformationTable;
        Table<Connection> connectionTable;
        Table<Dotfunction> dotFunctionTable;
        Table<Cardgate> cardGateTable;
        Table<Logicfunction> logicFunctionTable;
        Table<Gatepin> gatePinTable;
        Table<Logiclevels> logicLevelsTable;

        DBSetup db = DBSetup.Instance;

        List<Sheetedgeinformation> sheetInputsList;
        List<Sheetedgeinformation> sheetOutputsList;
        List<LogicBlock> logicBlocks = new List<LogicBlock>();
        List<Logiclevels> logicLevels = null;
        

        Regex replacePeriods = new Regex("\\.");
        Regex replaceTitle = new Regex(" |-|\\.|\\+|\\-");

        List<String> ignoredBlockSymbols = new List<string>()
            {"L", "R", /* "LAMP", */ "CAP" };

        List<string> specialSignalNames = new List<string>()
            {"LOGIC ZERO", "LOGIC ONE", "GROUND"};

        // string VHDLEntityName;

        bool needsClock = false;
        bool generateTestBench = false;

        int temp_errors = 0;


        public GenerateHDL(Page page, string directory, bool generateTestBench) {

            this.page = page;
            this.directory = directory;
            this.generateTestBench = generateTestBench;
            diagramPageTable = db.getDiagramPageTable();
            diagramBlockTable = db.getDiagramBlockTable();
            sheetEdgeInformationTable = db.getSheetEdgeInformationTable();
            connectionTable = db.getConnectionTable();
            dotFunctionTable = db.getDotFunctionTable();
            cardGateTable = db.getCardGateTable();
            logicFunctionTable = db.getLogicFunctionTable();
            gatePinTable = db.getGatePinTable();
            logicLevelsTable = db.getLogicLevelsTable();
        }

        public int generateHDL() {

            List<Diagramblock> blocks;
            List<Dotfunction> dotFunctions;
            List<int> ignoredConnectionIDs = new List<int>();

            logicLevels = logicLevelsTable.getAll();

            //  TODO: Eventually, the class chosen here would depend on what
            //  flavor of HDL the user chose.

            generator = new GenerateHDLLogicVHDL(
                page, generateTestBench);

            //VHDLEntityName =
            //    "ALD_" +
            //    replacePeriods.Replace(page.name, "_") + "_" +
            //    replaceTitle.Replace(page.title, "_");

            // outFile = new StreamWriter(Path.Combine(directory, VHDLEntityName + ".vhdl"), false);
            // logFileName = Path.Combine(directory, VHDLEntityName + ".log");

            logPathName = Path.Combine(directory,
                generator.getHDLEntityName() + ".log");
            outPathName = Path.Combine(directory,
                    generator.getHDLEntityName() + "." +
                    generator.generateHDLExtension());

            try {
                generator.logFile = new StreamWriter(logPathName, false);
            }
            catch(Exception e) {
                return (1);
            }

            try {
                generator.outFile = new StreamWriter(outPathName, false);
            }
            catch (Exception e) {
                logMessage("Cannot open output file " + outPathName +
                    ", aborting: " + e.GetType().Name);
                return (1);
            }

            int errors = 0;
            generator.outputStreams.Add(generator.outFile);

            logMessage("Generating HDL for page " + page.name +
                " " + page.title + " at " + DateTime.Now.ToString());

            //  Is there a template (include, if you will) file to use?
            //  (Is used for both entity and its test bench, if any)

            string templatePath = Path.Combine(directory,
                generator.templateName + "." + generator.generateHDLExtension());
            try {
                generator.templateFile = new StreamReader(templatePath);
            }
            catch (Exception e) {
                logMessage("Unable to open template file " + templatePath +
                    ", using internally generated defaults");
                generator.templateFile = null;
            }

            //  Are we generating or updating a test bench?  If so,
            //  read it in and save the "interesting" parts, if any,
            //  and open the test bench file for writing.

            generator.savedTestBenchLines = new List<string>();
            generator.savedTestBenchDeclLines = new List<string>();

            if (generateTestBench) {

                string testBenchFileName = generator.getHDLEntityName() +
                    generator.testBenchSuffix + "." + generator.generateHDLExtension();
                string testBenchPathName = Path.Combine(directory, testBenchFileName);

                try {
                    StreamReader oldTestBenchFile = new StreamReader(testBenchPathName);
                    string testBenchLine;
                    bool saving = false;
                    bool savingDecl = false;

                    //  "Let us preserve what must be preserved"  (Delores Umbridge)

                    while ((testBenchLine = oldTestBenchFile.ReadLine()) != null) {
                        if(testBenchLine.Contains(GenerateHDLLogic.testBenchUserStart)) {
                            saving = true;
                        }
                        else if(testBenchLine.Contains(GenerateHDLLogic.testBenchDeclStart)) {
                            savingDecl = true;
                        }
                        if (saving) {
                            generator.savedTestBenchLines.Add(testBenchLine);
                        }
                        else if(savingDecl) {
                            generator.savedTestBenchDeclLines.Add(testBenchLine);
                        }
                        if(testBenchLine.Contains(GenerateHDLLogic.testBenchUserEnd)) {
                            saving = false;
                        }
                        else if(testBenchLine.Contains(GenerateHDLLogic.testBenchDeclEnd)) {
                            savingDecl = false;
                        }
                    }

                    oldTestBenchFile.Close();
                    logMessage("Old test bench file " + testBenchFileName + " " +
                        generator.savedTestBenchLines.Count.ToString() +
                        " lines preserved and " +
                        generator.savedTestBenchDeclLines.Count.ToString() +
                        " declaration lines preserved");

                }
                catch (Exception e) {
                    logMessage("No existing test bench file " + testBenchPathName +
                        ", generating default test bench code.");
                }


                try {
                    generator.testBenchFile = new StreamWriter(testBenchPathName);
                    generator.outputStreams.Add(generator.testBenchFile);
                }
                catch(Exception e) {
                    logMessage("Unable to create or open test bench file " +
                        testBenchPathName + " for output.  Test Bench skipped.");
                    generator.testBenchFile = null;
                }
            }

            List<Diagrampage> diagramList = diagramPageTable.getWhere(
                "WHERE diagrampage.page='" + page.idPage + "'");

            if (diagramList.Count != 1) {
                logMessage("Error: " + diagramList.Count + " None or more than one " +
                    "Diagram page(s) found for page " +
                    page.name + " (Database ID " + page.idPage + ")");
                ++errors;
                if (diagramList.Count < 1) {
                    closeFiles();
                    return (errors);
                }
            }

            if(diagramList[0].noHDLGeneration == 1) {
                logMessage("Note: page is marked for no HDL generation - skipped");
                closeFiles();
                return (errors);
            }

            sheetInputsList = sheetEdgeInformationTable.getWhere(
                "WHERE diagramPage='" + diagramList[0].idDiagramPage + "'" +
                " AND leftSide='1'" +
                " ORDER BY sheetedgeinformation.row");

            //  Remove any special names from this list so they don't show up
            //  as signals or entities

            List<Sheetedgeinformation> signalsToDelete = new List<Sheetedgeinformation>();
            foreach(Sheetedgeinformation signal in sheetInputsList) {
                if(specialSignalNames.Contains(signal.signalName)) {
                    signalsToDelete.Add(signal);
                }
            }

            foreach(Sheetedgeinformation signal in signalsToDelete) {
                sheetInputsList.Remove(signal);
                logMessage("NOTE: Special signal " + signal.signalName +
                    " removed from sheet inputs list.");
            }

            if (sheetInputsList.Count < 1) {
                logMessage("WARNING: no sheet inputs found for page " +
                    page.name + " (Database ID " + page.idPage + ")");
            }

            sheetOutputsList = sheetEdgeInformationTable.getWhere(
                "WHERE diagramPage='" + diagramList[0].idDiagramPage + "'" +
                " AND rightSide='1'" +
                " ORDER BY sheetedgeinformation.row");

            if (sheetOutputsList.Count < 1) {
                logMessage("WARNING: no sheet outputs found for page " +
                    page.name + " (Database ID " + page.idPage + ")");
            }

            //  Get the blocks in order by row, so that the top of an extension
            //  shows up first.

            blocks = diagramBlockTable.getWhere("" +
                "WHERE diagramPage='" + diagramList[0].idDiagramPage + "'" +
                "ORDER BY diagramRow ASC, diagramColumn DESC");

            if (blocks.Count < 1) {
                logMessage("Error: No diagram blocks found for page " +
                    page.name + " (Database ID " + page.idPage + ")");
                ++errors;
            }

            //  If there were any messages so far, return with them.

            if (errors > 0) {
                closeFiles();
                return (errors);
            }

            //  OK.  So far so good.

            //  Get a list of the DOT functions...

            dotFunctions = dotFunctionTable.getWhere(
                "WHERE diagramPage='" + diagramList[0].idDiagramPage + "'");

            //  Build the list of logic blocks and their connections.

            foreach (Diagramblock block in blocks) {
                LogicBlock newBlock = new LogicBlock();

                newBlock.type = "G";
                newBlock.gate = block;
                newBlock.dot = null;
                newBlock.latchOutputs = false;
                newBlock.logicFunction = "Unknown";
                newBlock.HDLname = "";
                newBlock.pins = new List<Gatepin>();
                newBlock.inputLevel = "";
                newBlock.outputLevel = "";
                if(block.inputMode != 0) {
                    newBlock.inputLevel = logicLevelsTable.getByKey(block.inputMode).logicLevel;
                }
                if (block.outputMode != 0) {
                    newBlock.outputLevel = logicLevelsTable.getByKey(block.outputMode).logicLevel;
                }


                //  Skip blocks marked for no HDL generation

                if (block.noHDLGeneration == 1) {
                    logMessage("Block at coordinate " + newBlock.getCoordinate() +
                        " is marked for No HDL Generation -- skipped.");
                    continue;
                }

                if (block.cardGate == 0) {
                    logMessage("Error: Zero cardGate Key found for logic block " +
                        newBlock.getCoordinate() + " logic function set to Unknown");
                    ++errors;
                }
                else {
                    Cardgate gate = cardGateTable.getByKey(block.cardGate);
                    if (gate == null || gate.idcardGate != block.cardGate ||
                        gate.logicFunction == 0) {
                        logMessage("Error: Gate not found, or has no logic function " +
                            "for logic block " + newBlock.getCoordinate() +
                            " gate database key " + block.cardGate +
                            ", logic function set to Unknown");
                        ++errors;
                    }
                    else {

                        //  Remember the pins for the corresponding gate...

                        newBlock.pins = gatePinTable.getWhere(
                            "WHERE cardGate='" + gate.idcardGate + "'");

                        Logicfunction fun = logicFunctionTable.getByKey(gate.logicFunction);
                        if (fun == null || fun.idLogicFunction != gate.logicFunction ||
                            fun.name == null || fun.name.Length == 0) {
                            logMessage("Error: matching logic function name not found for " +
                                "logic block " + newBlock.getCoordinate() +
                                "logic function database key " + gate.logicFunction +
                                ", logic function set to Unknown");
                            ++errors;
                        }
                        else {
                            newBlock.logicFunction = fun.name;                            

                            if(newBlock.logicFunction == "Special" ||
                                newBlock.logicFunction == "Trigger"  ||
                                newBlock.logicFunction == "DELAY" ||
                                newBlock.logicFunction == "SS") {
                                if(gate.HDLname == null || gate.HDLname.Length == 0) {
                                    logMessage("Error: Logic function is " +
                                        newBlock.logicFunction + " but there is no " +
                                        "HDLname specified.");
                                    ++errors;                                    
                                }
                                else {
                                    newBlock.HDLname = gate.HDLname;
                                }
                            }
                        }
                    }
                }

                newBlock.inputConnections = connectionTable.getWhere(
                    "WHERE toDiagramBlock='" + block.idDiagramBlock + "'");
                newBlock.outputConnections = connectionTable.getWhere(
                    "WHERE fromDiagramBlock='" + block.idDiagramBlock + "'");

                //  Issue a warning if there are multiple output pins (TOG and ALT blocks
                //  get to have two without a warning).

                List<string> pins = new List<string>();

                foreach (Connection connection in newBlock.outputConnections) {
                    if (pins.IndexOf(connection.fromPin) < 0) {
                        pins.Add(connection.fromPin);
                    }
                }

                if (pins.Count > ((block.symbol == "TOG" || block.symbol == "ALT") ? 2 : 1)) {
                    logMessage(
                        "WARNING: Diagram block at coordinate " +
                        newBlock.getCoordinate() +
                        " has " + pins.Count + " different output pins: " +
                        string.Join(",", pins.ToArray()));
                }


                //  Also issue a warning if the same input pin (other than "--")
                //  appears more than once.

                pins = new List<string>();

                foreach (Connection connection in newBlock.inputConnections) {
                    if (connection.toPin == "--") {
                        continue;
                    }
                    if (pins.IndexOf(connection.toPin) >= 0) {
                        logMessage(
                            "WARNING: Diagram block at coordinate " +
                            newBlock.getCoordinate() +
                            " has more than one input connection to pin " +
                            connection.toPin);
                    }
                    else {
                        pins.Add(connection.toPin);
                    }
                }

                logicBlocks.Add(newBlock);
            }

            //  And then the same for DOT Functions

            foreach (Dotfunction dot in dotFunctions) {
                LogicBlock newBlock = new LogicBlock();

                bool switchFed = true;
                bool minusCFed = true;

                string inputLevel = "";
                string outputLevel = "";
                string logicLevel = "";

                newBlock.type = "D";
                newBlock.gate = null;
                newBlock.dot = dot;
                newBlock.latchOutputs = false;
                if(dot.forcedLogicFunction == null) {
                    dot.forcedLogicFunction = "";
                }

                //  Set the default DOT Function logic function

                newBlock.logicFunction = "OR";  

                newBlock.inputConnections = connectionTable.getWhere(
                    "WHERE toDotFunction='" + dot.idDotFunction + "'");
                newBlock.outputConnections = connectionTable.getWhere(
                    "WHERE fromDotFunction='" + dot.idDotFunction + "'");

                //  If the DOT function logic function is explicitly specified, use that

                if(dot.forcedLogicFunction.Length > 0) {
                    newBlock.logicFunction = dot.forcedLogicFunction;
                    logicBlocks.Add(newBlock);
                    logMessage("Note: DOT Function at " + dot.diagramColumnToLeft +
                        dot.diagramRowTop + " has a forced logic function of " +
                        dot.forcedLogicFunction);
                    continue;  // We are done if the logic function is explicit.
                }

                //  See if we can deduce the logic function based on the
                //  logic levls of the gates connected to it.  If the input(s) and 
                //  output levels are all the same, or if the input level is not
                //  specified but the output is, use the logic function from the
                //  logic levels table.  If it is not specified there, leave the
                //  global default in place.

                //  First, run through the input connections

                foreach (Connection inputConnection in newBlock.inputConnections) {

                    //  If this connection comes from a diagram block, use its
                    //  output logic level

                    if(inputConnection.fromDiagramBlock != 0) {
                        Diagramblock block = blocks.Find(x => x.idDiagramBlock ==
                            inputConnection.fromDiagramBlock);
                        Logiclevels level = logicLevels.Find(x => x.idLogicLevels ==
                            block.outputMode);
                        if(level == null || level.logicLevel == inputLevel) {
                            continue;
                        }
                        if(inputLevel.Length == 0) {
                            inputLevel = level.logicLevel;
                        }
                        else {
                            inputLevel = "MIXED";
                        }
                    }

                    if(inputConnection.fromEdgeSheet != 0) {
                        Sheetedgeinformation edge = sheetEdgeInformationTable.getByKey(
                            inputConnection.fromEdgeSheet);
                        string edgeLevel = "";
                        if (edge.signalName.Length >= 2 &&
                            "+-".Contains(edge.signalName.Substring(0, 1))) {
                            edgeLevel = edge.signalName.Substring(1, 1);
                        }
                        if(edgeLevel == inputLevel) {
                            continue;
                        }
                        if (inputLevel.Length == 0) {
                            inputLevel = edgeLevel;
                        }
                        else {
                            inputLevel = "MIXED";
                        }
                    }
                }

                //  Then, run though the output connections

                foreach (Connection outputConnection in newBlock.outputConnections) {

                    //  If this connection comes from a diagram block, use its
                    //  output logic level

                    if (outputConnection.toDiagramBlock != 0) {
                        Diagramblock block = blocks.Find(x => x.idDiagramBlock ==
                            outputConnection.toDiagramBlock);
                        Logiclevels level = logicLevels.Find(x => x.idLogicLevels ==
                            block.inputMode);
                        if (level == null || level.logicLevel == outputLevel) {
                            continue;
                        }
                        if (outputLevel.Length == 0) {
                            outputLevel = level.logicLevel;
                        }
                        else {
                            outputLevel = "MIXED";
                        }
                    }

                    if (outputConnection.toEdgeSheet != 0) {
                        Sheetedgeinformation edge = sheetEdgeInformationTable.getByKey(
                            outputConnection.toEdgeSheet);
                        string edgeLevel = "";
                        if (edge.signalName.Length >= 2 &&
                            "+-".Contains(edge.signalName.Substring(0, 1))) {
                            edgeLevel = edge.signalName.Substring(1, 1);
                        }
                        if (edgeLevel == outputLevel) {
                            continue;
                        }
                        if (outputLevel.Length == 0) {
                            outputLevel = edgeLevel;
                        }
                        else {
                            inputLevel = "MIXED";
                        }
                    }
                }

                //  Eventually this will be replaced by data from the logic levels table

                if (inputLevel.Length == 0 && outputLevel.Length > 0 && outputLevel != "MIXED") {
                    logicLevel = outputLevel;
                }
                else if (outputLevel.Length == 0 && inputLevel.Length > 0 && inputLevel != "MIXED") {
                    logicLevel = inputLevel;
                }
                else if (outputLevel == inputLevel && outputLevel != "MIXED") {
                    logicLevel = outputLevel;
                }

                if(logicLevel.Length > 0) {
                    Logiclevels l = logicLevels.Find(x => x.logicLevel == logicLevel);
                    if(l != null && l.dotFunctionLogic != null && l.dotFunctionLogic.Length > 0) {
                        newBlock.logicFunction = l.dotFunctionLogic;
                    }
                }

                logMessage("Note: DOT Function at " + dot.diagramColumnToLeft +
                    dot.diagramRowTop + " has input level(s) of " + inputLevel +
                    ", and output level(s) of " + outputLevel + 
                    ", Logic Function set to " + newBlock.logicFunction);

                //  DOT functions are usually OR, voltage wise.  However, there are three
                //  (so far) special cases.  

                //  If a DOT function is fed ONLY from rotary switch(es),
                //  and the switch(es) is active LOW (as rotary switches usually are),
                //  then it turns into an AND.

                //  So, suppose a rotary switch has two outputs, A and B, which
                //  are connected together (via a DOT function - that is not really
                //  a gate dotted output).  Electrically, the output if this virtual
                //  DOT function is 0 if A is 0 or B is 0.  Applying DeMorgan's 
                //  theorum, the output is 1 if (NOT A is 0) AND (NOT B is 0), i.e.,
                //  if A is 1 AND B is 1.  Got that???  ;)

                //  Similary, if the DOT function is fed ONLY from resistors, 
                //  we can assume that there are active low switches feeding them.
                //  The output has to be low if EITHER is a negative voltage, 
                //  same as for the rotary switche example above.

                //  Also, if a DOT function is fed ONLY by -C signals we assume that
                //  if either one is negative voltage, the output is negative voltage.
                //  Logically, this is an OR, but it becomes an AND with respect to 
                //  positive voltage.  First detected on page 12.62.01.1

                foreach (Connection inputConnection in newBlock.inputConnections) {

                    if(minusCFed && inputConnection.fromEdgeSheet != 0) {
                        Sheetedgeinformation edge = sheetEdgeInformationTable.getByKey(
                            inputConnection.fromEdgeSheet);
                        if(edge.signalName.Length < 2 || 
                            edge.signalName.Substring(0,2).ToUpper() != "-C") {
                            // Signal input, not -C (and not R or switch either)
                            minusCFed = false;
                            switchFed = false;
                            break;
                        }
                        else {
                            continue;
                        }
                    }

                    minusCFed = false;  // Got some input other than -C for this DOT function

                    if (inputConnection.fromDiagramBlock == 0) {
                        switchFed = false;
                        break;
                    }

                    Diagramblock block = blocks.Find(x => x.idDiagramBlock ==
                        inputConnection.fromDiagramBlock);

                    if(block == null || block.idDiagramBlock == 0) {
                        logMessage("ERROR: Cannot find matching fromDiagramBLock " +
                            "(" + inputConnection.fromDiagramBlock.ToString() + ") for " +
                            "connection ID (" + inputConnection.idConnection.ToString() +
                            "for DOT function at " + dot.diagramColumnToLeft.ToString() +
                            dot.diagramRowTop);
                        switchFed = false;
                        break;
                    }

                    if ((block.symbol.ToUpper() != "ROT" && block.symbol.ToUpper() != "R") ||
                        (block.notes != null && block.notes.ToUpper().Contains("ACTIVE HIGH"))) {
                        switchFed = false;
                        break;
                    }
                }

                if(switchFed || minusCFed) {
                    logMessage("NOTE: DOT Function at " + dot.diagramColumnToLeft.ToString() +
                        dot.diagramRowTop + " is fed only by rotary switch(es) and/or " +
                        "resistors an/or -C inputs. Changing logic function to AND");
                    newBlock.logicFunction = "AND";
                }

                logicBlocks.Add(newBlock);
            }

            //
            //  Check for cases where another gate and a trigger's output connect 
            //  These serve as DC SETS or DC RESETS depending upon which trigger
            //  output the connec too - by dragging one output to logic 1, the
            //  other necessarily goes to logic 1.
            //

            //  So, process the logic blocks, looking for DOT functions. 
            //  If the DOT function has exactly two inputs, BOTH must be from
            //  diagram blocks, and one of those inputs is the output of a trigger 
            //  (Logic Function is Trigger).
            //
            //  At least for now, it must also have exactly ONE OUTPUT connection
            //
            //  Then do the following:
            //      1.  Locate the Trigger ("T") and the Non Trigger ("N") logic blocks
            //      2.  For gate N, identify its pin "P".
            //      3.  For gate T, identify its pin "T".
            //      3.  Change gate N output to be to the trigger pin T_DOT  
            //          (the HDL for the trigger must support this "extra" pin, 
            //          which will be OR'd to the DC Set or DC Reset input, depending 
            //          on the pin.)
            //      4.  Add this input to the trigger as pin T_DOT from gate N pin P.
            //  Mark the DOT function Logic BLock for removal after the loop is complete.

            List<LogicBlock> dotFunctionLogicBlocksToRemove = new List<LogicBlock>();
            foreach (LogicBlock logicBlock in logicBlocks) {

                //  If it isn't a DOT function, or does not have 2 inputs, or exactly 
                //  one output that is to a Gate (i.e., a Diagram Block), ignore it.

                if (!logicBlock.isDotFunction() || logicBlock.inputConnections.Count != 2
                    || logicBlock.outputConnections.Count != 1 ||
                    logicBlock.outputConnections[0].toDiagramBlock == 0) {
                    continue;
                }
            
                int triggerCount = 0;
                string triggerPin = "";
                LogicBlock triggerBlock = null;
                LogicBlock nonTriggerBlock = null;

                //  Remember the eventual output.

                Connection dotFunctionOuputConection = logicBlock.outputConnections[0];

                //  Is exactly one of the logic blocks a Trigger?

                foreach (Connection connection in logicBlock.inputConnections) {

                    //  If the connection isn't from a diagram block, we are done here.

                    if (connection.fromDiagramBlock == 0) {
                        triggerCount = 0;
                        break;
                    }

                    //  Find the corresponding logic block.  Is it a Trigger?

                    LogicBlock lb = logicBlocks.Find(x => x.gate != null && x.gate.idDiagramBlock ==
                        connection.fromDiagramBlock);

                    if (lb == null) {
                        logMessage("ERROR Processing DOT Function at " + 
                            logicBlock.getCoordinate() +
                            " Connection id " + connection.idConnection.ToString() +
                            " Reference to Diagram Block " + connection.fromDiagramBlock +
                            " Could not be found in logicBlocks list.");
                        ++errors;
                        triggerCount = 0;
                        break;
                    }

                    if (lb.logicFunction == "Trigger") {
                        ++triggerCount;
                        triggerBlock = lb;
                        triggerPin = connection.fromPin;                        
                    }
                    else {
                        nonTriggerBlock = lb;
                    }
                }

                //  Is the trigger count 0?  If so, we are done here....

                if (triggerCount == 0) {
                    continue;
                }

                //  We SHOULD now have a trigger Count of 1, and triggerBlock and
                //  nonTriggerBlock set.

                if (triggerCount > 1) {
                    logMessage("ERROR Processing DOT Function at " +
                        logicBlock.getCoordinate() +
                        " has inputs from > 1 Trigger -- aborting Trigger processing.");
                    continue;
                }

                //  Next, track down the gate that is the output to this DOT function

                LogicBlock destinationBlock = logicBlocks.Find(x => x.gate.idDiagramBlock ==
                    logicBlock.outputConnections[0].toDiagramBlock);

                if (triggerBlock == null || nonTriggerBlock == null || destinationBlock == null) {
                    logMessage("INTERNAL ERROR:  DOT Function Trigger processing: " +
                        "triggerBlock, nonTriggerBlock or destinationBlock is NULL");
                    continue;
                }
                
                //  Log the special case

                logMessage("DOT Function at " + logicBlock.getCoordinate() + 
                    " has one output from a Trigger, one output from Non-Trigger");
                logMessage("   Trigger block pin " + triggerPin + " located at " +
                    triggerBlock.getCoordinate() + ", Non-trigger is located at " +
                    nonTriggerBlock.getCoordinate() + " Output is to " + 
                    destinationBlock.getCoordinate());

                //  Here is where the real work gets done...

                Connection nonTriggerBlockConnection = null;
                Connection triggerBlockConnection = null;
                Connection destinationBlockConnection = null;

                //  First find the trigger and non-trigger output connections to this DOT function,
                //  and the destination gate's connection FROM this DOT function.

                foreach(Connection connection in nonTriggerBlock.outputConnections) {
                    if(connection.toDotFunction == logicBlock.dot.idDotFunction) {
                        nonTriggerBlockConnection = connection;
                    }
                }

                if(nonTriggerBlockConnection == null) {
                    logMessage("   ERROR:  Could not find corresponding connection to this " +
                        "DOT Function from logic block at " + nonTriggerBlock.getCoordinate());
                    continue;
                }

                foreach(Connection connection in triggerBlock.outputConnections) {
                    if(connection.toDotFunction == logicBlock.dot.idDotFunction) {
                        triggerBlockConnection = connection;
                    }
                }

                if (triggerBlockConnection == null) {
                    logMessage("   ERROR:  Could not find corresponding connection to this " +
                        "DOT Function from logic block at " + triggerBlock.getCoordinate());
                    continue;
                }

                foreach(Connection connection in destinationBlock.inputConnections) {
                    if(connection.fromDotFunction == logicBlock.dot.idDotFunction) {
                        destinationBlockConnection = connection;
                    }
                }

                if (destinationBlockConnection == null) {
                    logMessage("   ERROR:  Could not find corresponding connection from this " +
                        "DOT Function to logic block at " + destinationBlock.getCoordinate());
                    continue;
                }

                if(nonTriggerBlockConnection.to != "D" || 
                    nonTriggerBlockConnection.toDotFunction != logicBlock.dot.idDotFunction ||
                    triggerBlockConnection.to != "D" ||
                    triggerBlockConnection.toDotFunction != logicBlock.dot.idDotFunction ||
                    destinationBlockConnection.from != "D" ||
                    destinationBlockConnection.fromDotFunction != logicBlock.dot.idDotFunction) {
                    logMessage("   ERROR: Verification of Trigger/DOT Function connections FAILED.");
                    continue;
                }

                //
                //  Derive a faux pin on the trigger block to use as an input.  It MUST
                //  be defined on the gate, but should be one that cannot actually ever
                //  happen.  In the current application these are restricted to single cards,
                //  so pick the corresponding letter that corresponds to a double card's
                //  second half (S-8) using Valid pins to calculate it.
                //

                string fauxPin = Helpers.validPins[
                    Array.IndexOf(Helpers.validPins, triggerPin[0]) +
                    Array.IndexOf(Helpers.validPins,'S')].ToString();

                logMessage("   Using Trigger faux pin " + fauxPin + " as input side of pin " +
                    triggerPin);

                //  
                //  Change the non-trigger block's output to point to a faux input pin on
                //  the trigger block, and add a reference to it as an input on the trigger block.
                //

                nonTriggerBlockConnection.toDotFunction = 0;
                nonTriggerBlockConnection.toDiagramBlock = triggerBlock.gate.idDiagramBlock;
                nonTriggerBlockConnection.toPin = fauxPin;
                nonTriggerBlockConnection.to = "P";
                triggerBlock.inputConnections.Add(nonTriggerBlockConnection);

                //
                //  Change the trigger block's output to point where the DOT function output
                //  points, and change the destination gate's input to be the trigger block's
                //  output.
                //

                triggerBlockConnection.toDotFunction = 0;
                triggerBlockConnection.toDiagramBlock = destinationBlock.gate.idDiagramBlock;
                triggerBlockConnection.to = "P";
                destinationBlockConnection.fromDotFunction = 0;
                destinationBlockConnection.fromDiagramBlock = triggerBlock.gate.idDiagramBlock;
                destinationBlockConnection.from = "P";
                destinationBlockConnection.fromPin = triggerPin;

                //  Mark the dot function to be removed

                dotFunctionLogicBlocksToRemove.Add(logicBlock);
            }

            //  Finally remove any logic blocks marked for removal before proceeding.

            foreach (LogicBlock lb in dotFunctionLogicBlocksToRemove) {
                logicBlocks.Remove(lb);
            }

            //  Find the Gate type logic blocks that we want to ignore, and capture the
            //  associated connection IDs.

            //  Also, keep an eye out for latches: cases where something in the output
            //  list has a connection back to this block.

            foreach (LogicBlock block in logicBlocks) {

                //  Only gates are important here.  We don't ever mark them ignored,
                //  and if they are in a combinatorial loop (latch), we will end up with
                //  flip flops on either side of the DOT function anyway.

                if (!block.isGate()) {
                    continue;
                }

                //  Check for ignored logic blocks.
                //  Mark those blocks for ignore, and remember all their connections
                //  to be removed later.  

                //  Note:  Resistors are a special case, and are ignored only if they do
                //  not have BOTH an input AND an output connection.

                if (ignoredBlockSymbols.Contains(block.gate.symbol) &&
                        !(block.gate.symbol == "R" &&
                        block.inputConnections.Count > 0 &&
                        block.outputConnections.Count > 0)) {
                    logMessage("Ignoring Logic Block " +
                        block.getCoordinate() + " with symbol " + block.gate.symbol);
                    block.ignore = true;

                    //  Put any connections to or from this block into the list of
                    //  connections to delete later.

                    foreach (Connection connection in block.inputConnections) {
                        if (!ignoredConnectionIDs.Contains(connection.idConnection)) {
                            ignoredConnectionIDs.Add(connection.idConnection);
                        }
                    }

                    foreach (Connection connection in block.outputConnections) {
                        if (!ignoredConnectionIDs.Contains(connection.idConnection)) {
                            ignoredConnectionIDs.Add(connection.idConnection);
                        }
                    }
                }


                //  Check each output connection for the existence of a latch.
                //  Originally, it checked only two levels deep, but I immediately
                //  found one on 11.10.06.1 that was three levels, so I then
                //  generalized the algorithm to look for ANY loop, and then latch
                //  each gate involved.

                //  (Interesting thought experiment:  Would it be sufficient to
                //  just latch any ONE of the gates a given loop?)

                //  If a gate participates in a latch, then we will
                //  generate a D flip flop for ALL outputs of that gate (though
                //  there really should only be one).

                //  The outer loop keeps going, even once a latch is found, for
                //  the destination block cross-check.

                //  We don't check for latches on triggers or single shots, 
                //  since they will already be generated as or with flip flops 
                //  (and therefore we will need a clock input).  Same for
                //  oscillators and shift registers (delays) which also have a clock.

                if (block.gate.symbol == "T" || block.gate.symbol == "SS" ||
                    block.HDLname == "SingleShot" || block.HDLname == "Oscillator" || 
                    block.HDLname.Contains("ShiftRegister")) {
                    needsClock = true;
                    continue;
                }

                //  The latch check algorithm:

                //  Remember the original block we are checking (original block)
                //  Clear the list of blocks on the list
                //  Add the original block to the list
                //  While not latch found and there are entries in the list {
                //      Take an entry off the list (call it current block)
                //      Put the current block in the processed list
                //      foreach output in current block
                //          if it goes to an edge connection, then skip this output
                //          if output goes to original block, marked as latched, and quit
                //          if the output goes to a block that is NOT in the processed list
                //              then put that destination block in the list
                //      end foreach
                //      Mark current block as processed.
                //  end While

                List<LogicBlock> toProcess = new List<LogicBlock>();
                List<LogicBlock> processed = new List<LogicBlock>();
                toProcess.Add(block);

                // logMessage("DEBUG:  Looking for latches for block at " + block.getCoordinate());

                block.latchOutputs = false;

                while (!block.latchOutputs && toProcess.Count > 0) {
                    LogicBlock currentBlock = toProcess[0];
                    toProcess.Remove(currentBlock);

                    //  Don't bother with blocks already marked ignore.

                    if (block.ignore) {
                        continue;
                    }

                    if (block.gate.notes != null && block.gate.notes.Contains("DFLIPFLOP")) {
                        logMessage("Notes indicated to force a  D FF at output of gate at " +
                            block.getCoordinate());
                        block.latchOutputs = true;
                        needsClock = true;
                        generator.needsDFlipFlop = true;
                        continue;
                    }

                    //logMessage("\tDEBUG:  Latch processing " +
                    //    (currentBlock.isGate() ? "Gate" : "DOT Function") + " block at " + 
                    //    currentBlock.getCoordinate());

                    foreach (Connection connection in currentBlock.outputConnections) {

                        if (connection.toEdgeSheet != 0) {
                            continue;
                        }

                        //  Here we DO process DOT functions, as they can be part of
                        //  a loop that we need to detect...

                        LogicBlock destinationBlock = logicBlocks.Find(
                                x => (x.isGate() && x.gate.idDiagramBlock ==
                                    connection.toDiagramBlock) ||
                                     (x.isDotFunction() && x.dot.idDotFunction ==
                                     connection.toDotFunction));

                        //  Sanity check: if the connection destination is not found, or 
                        //  if the destination is not a gate or DOT function, squawk.

                        if (destinationBlock == null ||
                            !(destinationBlock.isGate() || destinationBlock.isDotFunction())) {
                            logMessage("Error: During latch check, " +
                                "no result looking in logicBlocks for " +
                                "Connection Destination " +
                                (connection.toDiagramBlock != 0 ? "Gate" : "") +
                                (connection.toDotFunction != 0 ? "DOT Function" : "") +
                                " database ID " +
                                (connection.toDiagramBlock != 0 ? connection.toDiagramBlock :
                                    connection.toDotFunction) +
                                " Connected from block at " + block.getCoordinate() +
                                " (Database ID=" + block.gate.idDiagramBlock + ")");
                            ++errors;
                            continue;
                        }

                        //  If the destination block is already marked ignore, ignore it...

                        if (destinationBlock.ignore) {
                            continue;
                        }

                        //  If the destination block is a Trigger or a Single Shot, then 
                        //  ignore it, because it would already be breaking the combinatorial
                        //  loop
                        
                        if(destinationBlock.isGate() &&
                            (destinationBlock.gate.symbol == "T" ||
                             destinationBlock.gate.symbol == "SS")) {
                            continue;
                        }


                        //  If we find a connection back to where we started, we have found
                        //  a combinatorial loop.

                        if (connection.toDiagramBlock == block.gate.idDiagramBlock) {
                            block.latchOutputs = true;
                            needsClock = true;
                            generator.needsDFlipFlop = true;
                            logMessage("Found combinatorial loop (need D FF) at output of gate at " +
                                block.getCoordinate());
                            //  Job done!
                            break;
                        }

                        //  Otherwise, if the destination block hasn't already been processed,
                        //  add it to the list of blocks to process.

                        if (processed.FindIndex(
                            x => x.isDotFunction() == destinationBlock.isDotFunction() &&
                            x.isGate() == destinationBlock.isGate() &&
                            x.getCoordinate() == destinationBlock.getCoordinate()) < 0) {
                            //logMessage("\tDEBUG:  Latch adding " +
                            //    (destinationBlock.isGate() ? "Gate" : "DOT Function") + 
                            //    " block at " +
                            //    destinationBlock.getCoordinate() + " to list.");
                            toProcess.Add(destinationBlock);
                        }
                    }

                    //  Mark the current block as processed so we don't run through it again.

                    processed.Add(currentBlock);

                }

            }

            //  END OF LATCH CHECK.


            //  Next, merge any extension blocks together.  They should point to
            //  each other, and if one is identified as a latch, then the merged
            //  entry will be so identified.  Then remove the connections from the
            //  extension, and mark it ignore.  Finally, change all of the 
            //  connections that go TO *other* blocks to be from the master instead
            //  of the extension.

            foreach (LogicBlock block in logicBlocks) {

                //  If the block is being ignored, or is a DOT function, don't bother.

                if (block.ignore || block.type != "G") {
                    continue;
                }

                if (block.gate.extendedTo != 0) {
                    LogicBlock extension = logicBlocks.Find(
                        x => x.isGate() &&
                        x.gate.idDiagramBlock == block.gate.extendedTo);
                    if (extension == null ||
                        extension.gate.idDiagramBlock != block.gate.extendedTo) {
                        logMessage("Error: During extensions merge, " +
                            "No result looking in logicBlocks for " +
                            "extension from block " + block.getCoordinate() +
                            " (Database ID=" + block.gate.idDiagramBlock + ")");
                        ++errors;
                        continue;
                    }
                    else {
                        logMessage("Processing extension from block at " +
                            block.getCoordinate() +
                            " (Database ID=" + block.gate.idDiagramBlock + ")" +
                            " to " + extension.getCoordinate() +
                            " (Database ID=" + extension.gate.idDiagramBlock + ")");

                    }

                    //  The extension block should either have the same symbol as this one,
                    //  or a symbol of "E".

                    if (extension.gate.symbol != block.gate.symbol &&
                        extension.gate.symbol != "E") {
                        logMessage("Warning:  During Extension merge, " +
                            "Extension block symbol " +
                            extension.gate.symbol +
                            " is not E and does not match master block symbol of " +
                            block.gate.symbol);
                    }

                    //  Look through the connections in the extension, and if they are
                    //  already present (can't compare database IDs, as those will
                    //  be different), then skip them.  Otherwise, add them to the
                    //  block in progress.  Then mark this one ignore.

                    bool foundMatchingConnection = false;

                    foreach (Connection c1 in extension.inputConnections) {

                        foundMatchingConnection = false;

                        foreach (Connection c2 in block.inputConnections) {
                            if (inputConnectionMatches(c1, c2)) {
                                foundMatchingConnection = true;
                                //logMessage("DEBUG: Found matching connection on " +
                                //    "extension pin " + c1.toPin + " with master block " +
                                //    "pin " + c2.toPin);
                                break;
                            }

                            //  If the connection does not match, the pins had better be
                            //  different.

                            if (c1.toPin == c2.toPin) {
                                logMessage("Error: During extension merge, " +
                                    "Master and Extension input connections to " +
                                    "same input pin (" + c1.toPin + " with different sources, at " +
                                    "coordinate " + block.getCoordinate());
                                ++errors;
                            }
                        }

                        if (!foundMatchingConnection) {
                            block.inputConnections.Add(c1);
                            logMessage("Copied connection to extension input pin " +
                                c1.toPin + " to master block at " + block.getCoordinate());
                        }
                    }

                    foreach (Connection c1 in extension.outputConnections) {

                        foundMatchingConnection = false;

                        foreach (Connection c2 in block.outputConnections) {
                            if (outputConnectionMatches(c1, c2)) {
                                foundMatchingConnection = true;
                                break;
                            }
                        }

                        if (!foundMatchingConnection) {
                            block.outputConnections.Add(c1);
                            logMessage("Copied connection from extension output pin " +
                                c1.fromPin + " to master block at " + block.getCoordinate());
                        }
                    }

                    //  Also have to merge the pins together, so that if the pins are 
                    //  mapped, we get all of the used pins into the instantiation.

                    //  This works out because a given pin, regardless of which of
                    //  the two possible gates it is on, *should* have a mapName
                    //  in at least one of them.  The only problem would be if a given
                    //  pin has a mapName in one of the gates, but not the other, but
                    //  that should not happen - it would be a data error.

                    foreach(Gatepin pin in extension.pins) {
                        if(pin.mapPin != null &&
                            pin.mapPin.Length > 0 &&
                            block.pins.FindIndex(x => x.pin == pin.pin) < 0) {

                            block.pins.Add(pin);
                            logMessage("Copied mapped pin " + pin.pin + " from extension " +
                                extension.getCoordinate() + " to master block at " +
                                block.getCoordinate());
                        }
                    }

                    //  Then we have to change all of the connections that are from the
                    //  extension TO other blocks to be from the master instead of the extension.

                    foreach(LogicBlock destinationBlock in logicBlocks) {
                        foreach(Connection destinationConnection in destinationBlock.inputConnections) {
                            if(destinationConnection.fromDiagramBlock == extension.gate.idDiagramBlock) {
                                destinationConnection.fromDiagramBlock = block.gate.idDiagramBlock;
                                logMessage("Moved connection from extension " + extension.getCoordinate() +
                                    " pin " + destinationConnection.fromPin + 
                                    " to be from master at " + block.getCoordinate());
                            }
                        }
                    }

                    //  Finally, set the extension to ignore.

                    extension.ignore = true;
                }
            }

            //  Next, look at the output connections ONLY, and if there is an output to an edge,
            //  keep only the first one.  Also, the pin for connecting to a given edge must be
            //  consistent.  Also, if a connection is from pin M to the same logic block ignore it.

            foreach (LogicBlock block in logicBlocks) {
                if (block.ignore) {
                    continue;
                }

                List<EdgeOutput> edges = new List<EdgeOutput>();

                foreach (Connection connection in block.outputConnections) {

                    //  Ignore connections from pin M that come back to me...

                    if (connection.fromPin == "M" && connection.toDiagramBlock ==
                        block.gate.idDiagramBlock) {
                        ignoredConnectionIDs.Add(connection.idConnection);
                        logMessage("Ignoring Pin M connection from/to block at " +
                            block.getCoordinate());
                        continue;
                    }

                    if (connection.toEdgeSheet != 0) {
                        Sheetedgeinformation edge = sheetEdgeInformationTable.getByKey(
                            connection.toEdgeSheet);
                        if (edge == null || edge.idSheetEdgeInformation != connection.toEdgeSheet) {
                            logMessage("Error: During sheet edge merge, " +
                                "Cannot look up sheet edge information for " +
                                "diagram block at " + block.getCoordinate() +
                                ", connection database key " + connection.idConnection +
                                ", edge database key " + connection.toEdgeSheet);
                            ++errors;
                            continue;
                        }

                        if (edge.rightSide == 0) {
                            logMessage("Error:  During sheet edge merge, " +
                                "Output from pin " + connection.fromPin +
                                "connects a side edge connection " +
                                "named " + edge.signalName +
                                " that is not marked right side, from " +
                                " logic block at " + block.getCoordinate() +
                                ", connection database ID " + connection.idConnection);
                            ++errors;
                            break;
                        }

                        //  Do we already have an entry with this signal name or for this pin?
                        //  If so, check them for consistency.

                        EdgeOutput outputSignal = edges.Find(x => x.signalName == edge.signalName);
                        EdgeOutput outputPin = edges.Find(x => x.pin == connection.fromPin);

                        if (outputSignal != null) {

                            if (outputSignal.pin != connection.fromPin) {
                                logMessage("Error:  During sheet edge merge, " +
                                    "Logic block at " + block.getCoordinate() +
                                    " outputs to signal " + edge.signalName + "" +
                                    " on more than one pin: " + outputSignal.pin +
                                    " vs. " + connection.fromPin);
                                ++errors;
                            }

                            //  So, same signal, same pin.  We can delete this one.

                            else {
                                ignoredConnectionIDs.Add(connection.idConnection);
                            }

                        }

                        else {
                            outputSignal = new EdgeOutput();
                            outputSignal.signalName = edge.signalName;
                            outputSignal.pin = connection.fromPin;
                            edges.Add(outputSignal);

                            if (outputPin != null) {

                                //  If we get here, we already know the signal is different,
                                //  but just to be safe, we check again.

                                if (outputPin.signalName != edge.signalName) {
                                    logMessage("WARNING: During sheet edge merge, " +
                                        "Logic block at " + block.getCoordinate() +
                                        " pin " + outputPin.pin +
                                        " outputs to two different signal names: " +
                                        edge.signalName + " vs. " + outputPin.signalName);
                                }
                            }
                        }
                    }
                }
            }

            //  Now go back and delete any of those connections from any of the blocks
            //  (including DOT Function blocks).  We can skip the blocks marked
            //  ignored.

            foreach (LogicBlock block in logicBlocks) {

                String blockType = block.isGate() ? "Gate" : "DOT Function";

                if (block.ignore) {
                    continue;
                }

                int removedInputs = 0;
                int removedOutputs = 0;

                foreach (int connectionID in ignoredConnectionIDs) {
                    removedInputs +=
                        block.inputConnections.RemoveAll(x => x.idConnection == connectionID);
                    removedOutputs +=
                        block.outputConnections.RemoveAll(x => x.idConnection == connectionID);
                }

                if (removedInputs > 0) {
                    logMessage("Removed " + removedInputs + " inputs to " +
                        (block.type == "G" ? "Gate" : "Dot Function") +
                        " at " + block.getCoordinate() +
                        " from ignored block(s)");
                }
                if (removedOutputs > 0) {
                    logMessage("Removed " + removedOutputs + " outputs from " +
                        (block.type == "G" ? "Gate" : "Dot Function") +
                        " at " + block.getCoordinate() +
                        " to ignored block(s) or identical signal names");
                }

            }

            //  Next up:  Check that any connection that goes TO a DOT function
            //  goes ONLY to that DOT function.  Ditto for from a DOT function.

            //  Also, a given INPUT pin should only appear once.

            foreach (LogicBlock block in logicBlocks) {

                if (block.ignore) {
                    continue;
                }

                foreach (Connection connection in block.inputConnections) {
                    if (connection.fromDotFunction != 0 && block.isDotFunction()) {
                        logMessage("Error: During DOT function check, " +
                            "Input to a DOT function is from a DOT function." +
                            ", dot function at " + block.getCoordinate());
                        ++errors;
                    }
                    else if (!block.isDotFunction() &&
                        connection.toPin != "--" &&
                        block.inputConnections.FindAll(
                        x => x.toPin == connection.toPin).Count > 1) {
                        logMessage("Error: During DOT function check, " +
                            "Input to pin " + connection.toPin +
                            " occurs more than once in this gate, at coordinate " +
                            block.getCoordinate());
                        ++errors;
                    }
                }

                foreach (Connection connection in block.outputConnections) {
                    if (connection.toDotFunction != 0 && block.isDotFunction()) {
                        logMessage("Error: During DOT function check, " +
                            "Output from a DOT function is to a DOT function, " +
                            "coordinate " + block.getCoordinate());
                        ++errors;
                    }
                    else if (connection.toDotFunction != 0 &&
                        block.outputConnections.FindAll(
                           x => x.fromPin == connection.fromPin).Count > 1) {
                        logMessage("Error: During DOT function check, " +
                            "Output from pin " + connection.fromPin +
                            " is to a DOT function, and occurs more than once in this gate, " +
                            "coordinate " + block.getCoordinate());
                        ++errors;
                    }
                }
            }

            //  Another check, now that we have merged extensions.  Check that a given output
            //  signal name only appears once.

            List<string> signalsOut = new List<string>();
            foreach (LogicBlock block in logicBlocks) {
                if (block.ignore) {
                    continue;
                }
                foreach (Connection connection in block.outputConnections) {
                    if (connection.toEdgeSheet != 0) {
                        Sheetedgeinformation edge = sheetEdgeInformationTable.getByKey(
                            connection.toEdgeSheet);
                        if (edge == null || edge.idSheetEdgeInformation != connection.toEdgeSheet) {
                            logMessage("Error: During signals check, " +
                                "Cannot look up sheet edge information for " +
                                "diagram block at " + block.getCoordinate() +
                                ", connection database key " + connection.idConnection +
                                ", edge database key " + connection.toEdgeSheet);
                            ++errors;
                            continue;
                        }

                        if (edge.rightSide == 0) {
                            logMessage("Error:  During signals check, Output connection to " +
                                "side edge connection named " + edge.signalName +
                                " does not appear on right side of sheet. " +
                                " logic block at " + block.getCoordinate() +
                                ", connection database ID " + connection.idConnection);
                            ++errors;
                            break;
                        }
                        if (signalsOut.Contains(edge.signalName)) {
                            logMessage("Error:  During signals check, " +
                                "More than one output connects to signal " +
                                "named " + edge.signalName +
                                " logic block at " + block.getCoordinate() +
                                ", connection database ID " + connection.idConnection);
                            ++errors;
                            break;
                        }
                        signalsOut.Add(edge.signalName);
                    }
                }
            }

            //  Check that each logic block uses only the pins defined for the
            //  associated gate.  Warn if this is violated (but it is not fatal,
            //  except for Special gates - which also check for unmapped pins.

            foreach(LogicBlock block in logicBlocks) {
                if(block.ignore || block.isDotFunction()) {
                    continue;
                }

                //  OK, so this is a gate.  Check it out...

                foreach(Connection connection in block.inputConnections) {
                    if(connection.toPin != null && connection.toPin.Length > 0 &&
                        connection.toPin != "--") {
                        Gatepin pin = block.pins.Find(x => x.pin == connection.toPin);
                        if(pin == null || pin.idGatePin == 0) {
                            logMessage("WARNING: Block at " + block.getCoordinate() +
                                " connection to input pin " + connection.toPin +
                                " -- pin is not defined for this gate.");
                            ++errors;
                        }
                    }
                }

                foreach (Connection connection in block.outputConnections) {
                    if (connection.fromPin != null && connection.fromPin.Length > 0 &&
                        connection.fromPin != "--") {
                        Gatepin pin = block.pins.Find(x => x.pin == connection.fromPin);
                        if (pin == null || pin.idGatePin == 0) {
                            logMessage("WARNING: Block at " + block.getCoordinate() +
                                " connection from output pin " + connection.fromPin +
                                " -- pin is not defined for this gate.");
                            ++errors;
                        }
                    }
                }

                //  Lamps get special handling - we generate a special signal name to add to the
                //  outputs list.

                if(block.logicFunction == "Lamp") {
                    string signalName = "LAMP_" + Helpers.getCardSlotInfo(
                        block.gate.cardSlot).ToSmallString();
                    Sheetedgeinformation lampEdge = new Sheetedgeinformation();
                    lampEdge.idSheetEdgeInformation = -1;
                    lampEdge.signalName = signalName;
                    lampEdge.rightSide = 1;
                    lampEdge.leftSide = 0;
                    lampEdge.row = block.gate.diagramRow;
                    sheetOutputsList.Add(lampEdge);

                    logMessage("Added LAMP signal " + signalName);
                }


            }

            //generateVHDLPrefix();
            //generateVHDLEntity();
            //errors += generateVHDLArchitecturePrefix();

            //  For now, I am forcing needsClock to true, so that a future
            //  "rollup generator" does not have to worry about whether or
            //  not a given page needs a clock.  If that causes problems later,
            //  then the likely alternative would be to add a "needsClock"
            //  column to the diagramPage table when the need is identified
            //  (and allow manual setting/clearing in the associated dialog),
            //  or for the rolloup generator to sort throughall of the logic blocks
            //  involved on the page before calling this page.
           
            needsClock = true;

            //  If there is a test bench to be generated, and if there are no saved lines, 
            //  fill up with our defaults.

            if (generateTestBench && generator.savedTestBenchLines.Count == 0) {

                string testBenchTemplateFileName = TestBenchTemplate + "." +
                    generator.generateHDLExtension();
                string testBenchTemplatePathName = Path.Combine(directory,
                    testBenchTemplateFileName);

                string testBenchFPGAClockFileName = TestBenchFPGAClock + "." +
                    generator.generateHDLExtension();
                string testBenchFGPAClockPathName = Path.Combine(directory,
                    testBenchFPGAClockFileName);

                StreamReader testBenchTemplateStream = null;
                StreamReader testBenchFPGAClockStream = null;

                try {
                    testBenchTemplateStream = new StreamReader(
                        testBenchTemplatePathName);
                }
                catch (Exception e) {
                    logMessage("Unable to open the Test Bench Template File: " +
                        e.GetType().Name + ", file " + testBenchTemplatePathName);
                }

                try {
                    testBenchFPGAClockStream = new StreamReader(
                        testBenchFGPAClockPathName);
                }
                catch (Exception e) {
                    logMessage("Unable to open the Test Bench Clock Template File: " +
                        e.GetType().Name + ", file " + testBenchFGPAClockPathName);
                }

                //  Read the default template file.  If we see a line with the
                //  FPGA clock tag, replace that with the FPGA Clock Template

                if (testBenchTemplateStream != null) {
                    string line;
                    while ((line = testBenchTemplateStream.ReadLine()) != null) {

                        //  Replace the FPGA clock tag with the appropriate template,
                        //  but only an the FPGA clock is actually required.

                        if (needsClock && line.Contains(TestBenchFPGAClockTag) &&
                            testBenchFPGAClockStream != null) {
                            while ((line = testBenchFPGAClockStream.ReadLine())
                                != null) {
                                generator.savedTestBenchLines.Add(line);
                            }
                        }
                        else {
                            //  Add other lines directly
                            generator.savedTestBenchLines.Add(line);
                        }
                    }
                }

                if (testBenchTemplateStream != null) {
                    testBenchTemplateStream.Close();
                }
                if (testBenchFPGAClockStream != null) {
                    testBenchFPGAClockStream.Close();
                }

            }

            //  Now do the same, but for the test bench user delcarations.
            //  There is no FPGA Clock tag here, of course.

            if (generateTestBench && generator.savedTestBenchDeclLines.Count == 0) {

                string testBenchDeclFileName = TestBenchDeclares + "." +
                    generator.generateHDLExtension();
                string testBenchDeclPathName = Path.Combine(directory,
                    testBenchDeclFileName);

                StreamReader testBenchDeclStream = null;

                try {
                    testBenchDeclStream = new StreamReader(
                        testBenchDeclPathName);
                }
                catch (Exception e) {
                    logMessage("Unable to open the Test Bench Declares Template File: " +
                        e.GetType().Name + ", file " + testBenchDeclPathName);
                }

                //  Read the Declarations template file. 

                if (testBenchDeclStream != null) {
                    string line;
                    while ((line = testBenchDeclStream.ReadLine()) != null) {
                        generator.savedTestBenchDeclLines.Add(line);
                    }
                }

                if (testBenchDeclStream != null) {
                    testBenchDeclStream.Close();
                }
            }

            generator.logicBlocks = logicBlocks;
            generator.generateHDLPrefix();
            errors += generator.generateHDLEntity(
                needsClock,
                sheetInputsList,
                sheetOutputsList);
            errors += generator.generateHDLArchitecturePrefix();

            //  It's show time!  Run through the logic blocks, generating HDL statments
            //  for everything but the final output signal assignments.  (I could do those
            //  here, too, but I think it better to have them appear at the end.

            foreach (LogicBlock block in logicBlocks) {

                if (block.ignore) {
                    continue;
                }

                //  Get the list of inputs: signals, and other block's outputs

                List<string> inputNames = new List<string>();

                foreach (Connection inputConnection in block.inputConnections) {

                    //  For Edge connections, we want the signal name...

                    if (inputConnection.fromEdgeSheet != 0) {
                        Sheetedgeinformation edge = sheetEdgeInformationTable.getByKey(
                            inputConnection.fromEdgeSheet);
                        if (edge == null ||
                            edge.idSheetEdgeInformation != inputConnection.fromEdgeSheet) {
                            logMessage("Error: During statement generation, " +
                                "Cannot look up sheet edge information for " +
                                "diagram block at " + block.getCoordinate() +
                                ", connection database key " + inputConnection.idConnection +
                                ", edge database key " + inputConnection.fromEdgeSheet);
                            ++errors;
                            continue;
                        }
                        if (edge.leftSide == 0) {
                            logMessage("Error:  During signals check, Input connection from " +
                                "side edge connection named " + edge.signalName +
                                " is not identified as left side of sheet. " +
                                " logic block at " + block.getCoordinate() +
                                ", connection database ID " + inputConnection.idConnection);
                            ++errors;
                            continue;
                        }

                        //  Issue a warning if this sheet edge signal is to a DOT function,
                        //  and the signal is an input to more than one sheet.

                        if(inputConnection.toDotFunction != 0) {
                            List<Sheetedgeinformation> signals = sheetEdgeInformationTable.getWhere(
                                "WHERE signalName = '" + edge.signalName + "' AND diagramPage <> " +
                                edge.diagramPage.ToString() + " AND leftSide = '1'");
                            if(signals.Count > 0) {
                                logMessage("WARNING: Input connection from " +
                                    "edge connection named " + edge.signalName +
                                    " to a DOT function on page " +
                                    Helpers.getDiagramPageName(edge.diagramPage) +
                                    " coordinate " + block.getCoordinate() +
                                    " is also used as input to other page(s):");
                                string message = "";
                                foreach(Sheetedgeinformation signal in signals) {
                                    message += Helpers.getDiagramPageName(signal.diagramPage) + " ";
                                }
                                logMessage("    " + message);
                            }
                        }

                        inputNames.Add(generator.generateSignalName(edge.signalName));
                    }

                    //  Otherwise, it is coming from a gate or a DOT function...

                    else {
                        //  Find the block the input connection comes from.
                        LogicBlock outputBlock = logicBlocks.Find(x =>
                            (x.isDotFunction() && x.dot.idDotFunction == inputConnection.fromDotFunction) ||
                            (x.isGate() && x.gate.idDiagramBlock == inputConnection.fromDiagramBlock));
                        if (outputBlock == null ||
                            !(outputBlock.isDotFunction() || outputBlock.isGate())) {
                            logMessage("ERROR:  During generation, input is not from edge, and " +
                                "cannot find matching DOT function or Gate, Diagram Block " +
                                block.getCoordinate() +
                                ", connection database ID " + inputConnection.idConnection);
                            ++errors;
                            continue;
                        }
                        inputNames.Add(
                            generator.generateOutputPinName(outputBlock, inputConnection,
                                out temp_errors));
                        errors += temp_errors;
                    }
                }


                //  Next, do something similar for outputs, except that in this
                //  case we do not care what it connects to, just the pin the
                //  connection comes from.

                //  Note that most gates should only have ONE output pin, but some 
                //  (particularly those with logic function "Special") may have more 
                //  than one.

                List<string> outputNames = new List<string>();
                List<string> uniqueOutputNames = new List<string>();

                foreach (Connection outputConnection in block.outputConnections) {

                    //  Note that we add an output name for each connection - and there
                    //  may be multiple for a given pin (unlike an input)
                    //  BUT:  That is OK - the generator for special gates will handle that.

                    string name = generator.generateOutputPinName(block, outputConnection,
                            out temp_errors);
                    outputNames.Add(name);
                    if(!uniqueOutputNames.Contains(name)) {
                        uniqueOutputNames.Add(name);
                    }
                    errors += temp_errors;

                }

                //  For now, while debugging, just display the block, output name(s) and
                //  input name(s)

                //  If this block is marked for latch outputs, put latches on all of its
                //  outputs.

                if (block.latchOutputs) {
                    for(int outputIndex = 0; outputIndex < outputNames.Count; ++ outputIndex) {
                        outputNames[outputIndex] += "_" + LatchPrefix;
                    }
                }

                logMessage("Generating Statement for block at " + block.getCoordinate() +
                    " with " + (block.latchOutputs ? "*latched* " : "") + "output pin(s) of " +
                    String.Join(", ", outputNames));
                logMessage("\tand inputs of " + string.Join(",", inputNames));
                logMessage("\tand logic function of " + block.logicFunction);
                errors += temp_errors;

                //  Actual generate is here...

                if (block.logicFunction != "Lamp" && 
                    block.outputConnections.Count <= 0) {
                    logMessage("\tERROR: Block has no outputs.");
                    ++errors;
                }
                else if (block.logicFunction != "Lamp" && outputNames.Count < 1) {
                    logMessage("\tERROR:  No output names found.");
                }
                else if (inputNames.Count <= 0 && block.logicFunction != "Switch") {
                    logMessage("\tERROR: No input names found.");
                    ++errors;
                }
                else if ((block.logicFunction == "NOT" || 
                        block.logicFunction == "EQUAL" ||
                        block.logicFunction == "Lamp")
                        && inputNames.Count > 1) {
                    logMessage("\tError: NOT, EQUAL or Lamp function cannot have more " +
                        "than one input.");
                    ++errors;
                }
                else if(uniqueOutputNames.Count > 1 &&
                    block.logicFunction != "Special" &&
                    block.logicFunction != "Trigger" &&
                    block.logicFunction != "Switch") {
                    logMessage("\tError: More than one output name, but gate is not marked" +
                        " as logic function Special, Trigger or Switch.");
                    ++errors;
                }
                else if (block.logicFunction == "NAND") {                   
                    generator.generateHDLNand(inputNames, outputNames[0]);
                }
                else if (block.logicFunction == "NOT" && inputNames.Count == 1) {
                    generator.generateHDLNot(inputNames, outputNames[0]);
                }
                else if (block.logicFunction == "OR") {
                    //  These are usually DOT functions...
                    generator.generateHDLOr(inputNames, outputNames[0]);
                }
                else if (block.logicFunction == "EQUAL" ||
                    block.logicFunction == "Resistor") {
                    generator.generateHDLEqual(inputNames, outputNames[0]);
                }
                else if (block.logicFunction == "Lamp" && inputNames.Count == 1) {
                    generator.generateHDLEqual(inputNames,
                        "LAMP_" + Helpers.getCardSlotInfo(block.gate.cardSlot).ToSmallString());
                }
                else if (block.logicFunction == "NOR") {
                    generator.generateHDLNor(inputNames, outputNames[0]);
                }
                else if(block.logicFunction == "AND") {
                    generator.generateHDLAnd(inputNames, outputNames[0]);
                }
                else if (block.logicFunction == "Special" ||
                    block.logicFunction == "Trigger" ||
                    block.HDLname == "Oscillator" ||
                    block.logicFunction == "SS" ||
                    block.HDLname == "SingleShot") { 
                    errors += 
                        generator.generateHDLSpecial(block, inputNames, outputNames);
                }
                else if(block.HDLname != null &&
                    block.HDLname.Contains("ShiftRegister")) {

                    //  Special case:  If delay says "NOTE" then we treat it as
                    //  an EQUAL or a NOT (if it is InvShiftRegister))

                    if(block.gate.title.Contains("NOTE")) {
                        logMessage("\tWARNING: Delay Block with a time of NOTE. " +
                            "EQUAL or NOT will be generated.");
                        if(block.HDLname == "ShiftRegister") {
                            generator.generateHDLEqual(inputNames, outputNames[0]);
                        }
                        else {
                            generator.generateHDLNot(inputNames, outputNames[0]);
                        }
                    }
                    else {
                        generator.generateHDLSpecial(block, inputNames, outputNames);
                    }

                }

                else if(block.logicFunction == "Switch") {
                    generator.generateHDLSwitch(block, inputNames, outputNames);
                }
                else {
                    logMessage("\tERROR:  GenerateHDL does not (yet) know how to generate " +
                        "the listed block with logic function " +
                        block.logicFunction + " and HDLName of " + block.HDLname);
                    ++errors;
                }
            }

            generator.outFile.WriteLine();

            //  Now do those signal assignments

            foreach (LogicBlock block in logicBlocks) {

                if (block.ignore) {
                    continue;
                }

                foreach (Connection connection in block.outputConnections) {
                    if (connection.toEdgeSheet != 0) {
                        Sheetedgeinformation edge = sheetEdgeInformationTable.getByKey(
                            connection.toEdgeSheet);
                        if (edge == null || edge.idSheetEdgeInformation == 0) {
                            logMessage("ERROR:  During generation, Output connection " +
                                "from block " + block.getCoordinate() +
                                " to sheet edge with database ID " +
                                connection.toEdgeSheet + " not found in database.");
                            ++errors;
                            continue;
                        }
                        if (edge.rightSide != 1) {
                            logMessage("ERROR:  During generation, Output connection " +
                                "from block " + block.getCoordinate() +
                                " to edge sheet signal " + edge.signalName +
                                "is not marked right side.");
                            ++errors;
                            continue;
                        }
                        logMessage("Generating output sheet edge signal assignment to " +
                            Environment.NewLine + "\tsignal " +
                            generator.generateSignalName(edge.signalName) + 
                            Environment.NewLine + "\tfrom gate output " +
                            generator.generateOutputPinName(block, connection, out temp_errors));
                        generator.generateHDLSignalAssignment(
                            generator.generateOutputPinName(block, connection, out temp_errors),
                            generator.generateSignalName(edge.signalName));
                        errors += temp_errors;
                    }
                }
            }

            generator.outFile.WriteLine();

            //  Finally, generate the use of any D Flip Flops required due to latches.

            foreach (LogicBlock block in logicBlocks) {

                if (block.ignore || !block.latchOutputs) {
                    continue;
                }

                logMessage("Generating D Flip Flop for block at " + block.getCoordinate());

                errors += generator.generateHDLDFlipFlop(block);
            }



            generator.generateHDLArchitectureSuffix();

            closeFiles();

            return (errors);
        }

        

        public string getLogfileName() {
            return logPathName;
        }

        private void logMessage(string message) {
            generator.logFile.WriteLine(message);
            generator.logFile.Flush();
        }

        private void closeFiles() {
            generator.outFile.Close();
            generator.logFile.Close();
            if(generator.templateFile != null) {
                generator.templateFile.Close();
            }
            if(generator.testBenchFile != null) {
                generator.testBenchFile.Close();
            }
        }

        //private string generateOutputPinName(LogicBlock block, Connection connection,
        //    out int errors) {

        //    errors = 0;

        //    //  A DOT function output is always its coordinate (no defined pins)

        //    if (block.isDotFunction()) {
        //        return("OUT_DOT_" + block.getCoordinate());
        //    }

        //    //  A -- connection is named OUT_From_To

        //    if (connection.fromPin == "--") {
        //        if (connection.toDiagramBlock == 0 || connection.to != "P") {
        //            logMessage("Error: Connection from -- is not to a pin.");
        //            ++errors;
        //            return ("*Invalid*");
        //        }
        //        if (connection.toPin != "--") {
        //            logMessage("Error:  Connection from -- is to named pin " +
        //                connection.toPin);
        //            ++errors;
        //            return ("*Invalid*");
        //        }
        //        LogicBlock destination = logicBlocks.Find(
        //            x => x.gate.idDiagramBlock == connection.toDiagramBlock);
        //        if (destination == null ||
        //            destination.gate.idDiagramBlock != connection.toDiagramBlock) {
        //            logMessage("Error:  Cannot find matching -- diagram block in" +
        //                "Blocks for this page (Database ID=" +
        //                connection.toDiagramBlock + ")");
        //            ++errors;
        //            return ("*Invalid*");
        //        }

        //        return("OUT_" + block.getCoordinate() + "_" + destination.getCoordinate());
        //    }
        //    else {
        //        //  Connection is from an ordinary pin.
        //        return ("OUT_" + block.getCoordinate() + "_" + connection.fromPin);
        //    }


        //}


        //private string generateSignalName(string signal) {
        //    string outString = "";

        //    if(signal.Substring(0,1) == "+") {
        //        outString = "P" + signal.Substring(1);
        //    }
        //    else if(signal.Substring(0,1) == "-") {
        //        outString = "M" + signal.Substring(1);
        //    }
        //    outString = outString.Replace(" ", "_");
        //    outString = outString.Replace(".", "_DOT_");
        //    outString = outString.Replace("+", "_OR_");
        //    outString = outString.Replace("*", "_STAR_");
        //    return (outString);
        //}

        private bool inputConnectionMatches(Connection c1, Connection c2) {
            return (c1.toPin == c2.toPin &&
                c1.from == c2.from &&
                c1.fromDiagramBlock == c2.fromDiagramBlock &&
                c1.fromPin == c2.fromPin &&
                c1.fromDotFunction == c2.fromDotFunction &&
                c1.fromEdgeSheet == c2.fromEdgeSheet &&
                c1.fromEdgeOriginSheet == c2.fromEdgeOriginSheet);
        }

        private bool outputConnectionMatches(Connection c1, Connection c2) {
            return (c1.fromPin == c2.fromPin && 
                c1.to == c2.to &&
                c1.toDiagramBlock == c2.toDiagramBlock &&
                c1.toPin == c2.toPin &&
                c1.toDotFunction == c2.toDotFunction &&
                c1.toEdgeSheet == c2.toEdgeSheet &&
                c1.toEdgeDestinationSheet == c2.toEdgeDestinationSheet);
        }
    }


    //  A class to hold and manipulate a given logic block.

    public class LogicBlock {
        public string type { get; set; }                        //  G for gate, D for Dot Function
        public bool ignore { get; set; } = false;
        public bool latchOutputs { get; set; } = false;         // Needs latched outputs (Latch)
        public Diagramblock gate { get; set; }                  // if a gate
        public string inputLevel { get; set; }                  // if a gate
        public string outputLevel { get; set; }                 // if a gate
        public Dotfunction dot { get; set; }                    // if a DOT function
        public string logicFunction { get; set; }               // NOT, NAND, NOR, EQUAL, Resistor, etc.
        public string HDLname { get; set; }                     // For HDL entity/module name
        public List<Connection> inputConnections { get; set; }
        public List<Connection> outputConnections { get; set; }
        public List<Gatepin> pins { get; set; }

        public bool isGate() {
            return (type == "G");
        }

        public bool isDotFunction() {
            return (type == "D");
        }

        public string getCoordinate() {
            if(gate != null) {
                return gate.diagramColumn.ToString() + gate.diagramRow;                
            }
            else if (dot != null) {
                return dot.diagramColumnToLeft.ToString() + dot.diagramRowTop;
            }
            else {
                return ("??");
            }
        }

        public bool switchActiveHigh() {
            if(logicFunction != "Switch") {
                return (false);
            }

            switch(gate.symbol) {
                case "MOM":
                case "TOG":
                case "ALT":
                    return (true);
                case "ROT":
                case "REL":
                    return (gate.notes != null &&
                        gate.notes.ToUpper().Contains("ACTIVE HIGH"));
                default:
                    return (false);
            }
        }

        public string getSwitchName() {
            if(logicFunction != "Switch") {
                return ("NOT A SWITCH!!!");
            }
            return ("SWITCH " + gate.symbol + " " + gate.title +
                ((inputLevel.Length > 0 || outputLevel.Length > 0) ? 
                    " " + inputLevel + outputLevel : ""));
        }
    }

    //  A class to hold edge outputs by pin and signal, used for
    //  consistency checks.

    class EdgeOutput {
        public string pin { get; set; }
        public string signalName { get; set; }
    }
}
