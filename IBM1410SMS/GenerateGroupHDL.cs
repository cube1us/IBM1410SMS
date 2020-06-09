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
using System.Threading.Tasks;

using MySQLFramework;
using System.IO;

namespace IBM1410SMS
{
    class GenerateGroupHDL
    {
        const string TestBenchTemplate = "TestBenchTemplate";
        const string TestBenchFPGAClock = "TestBenchFPGAClock";
        const string TestBenchFPGAClockTag = "<FPGA CLOCK>";
        const string TestBenchDeclares = "TestBenchDeclares";

        Machine machine;
        List<Diagrampage> diagramPageList;
        List<string> pageNames;
        string directory;
        string outFileName;
        string logFileName;

        bool generateTestBench;
        // bool generatePagesTestBench;

        Table<Diagrampage> diagramPageTable;
        Table<Sheetedgeinformation> sheetEdgeInformationTable;
        Table<Page> pageTable;
        Table<Diagramblock> diagramBlockTable;
        Table<Cardgate> cardGateTable;
        Table<Logicfunction> logicFunctionTable;

        DBSetup db = DBSetup.Instance;

        GenerateGroupHDLLogic generator;

        //  TODO:  For now, forcing needsclock to true - maybe change later.

        bool needsClock = true;

        // StreamWriter outFile;
        // StreamWriter logFile;

        public GenerateGroupHDL(
            Machine machine,
            List<Diagrampage> diagramPageList,
            List<string> pageNames, 
            string outFileName, 
            string directory,
            bool generateTestBench) {

            this.machine = machine;
            this.diagramPageList = diagramPageList;
            this.pageNames = pageNames;
            this.directory = directory;
            this.outFileName = outFileName;
            this.generateTestBench = generateTestBench;
            // this.generatePagesTestBench = generatePagesTestBench;

            diagramPageTable = db.getDiagramPageTable();
            sheetEdgeInformationTable = db.getSheetEdgeInformationTable();
            pageTable = db.getPageTable();
            diagramBlockTable = db.getDiagramBlockTable();
            cardGateTable = db.getCardGateTable();
            logicFunctionTable = db.getLogicFunctionTable();
        }

        public int generateGroupHDL() {


            //  TODO: Eventually, the class chosen here would depend on what
            //  flavor of HDL the user chose.

            generator = new GenerateGroupHDLLogicVHDL(generateTestBench);

            List<string> inputList = new List<string>();
            List<string> outputList = new List<string>();

            List<string> removedInputSignals = new List<string>();
            List<string> removedOutputSignals = new List<string>();

            List<string> internalSignals = new List<string>();
            List<string> allSignals = new List<string>();
            List<string> bufferSignals = new List<string>();

            int errors = 0;

            logFileName = Path.Combine(directory, outFileName + ".log");
            string outPathName = Path.Combine(directory, outFileName +
                    "." + generator.generateHDLExtension());

            try {
                generator.logFile = new StreamWriter(Path.Combine(logFileName), false);
            }
            catch(Exception e) {
                return (1);
            }

            try {
                generator.outFile = new StreamWriter(outPathName, false);
            }
            catch (Exception e) {
                generator.logMessage("Cannot open output file " +  outPathName +
                    ", aborting: " + e.GetType().Name);
                return (1);
            }

            generator.outputStreams.Add(generator.outFile);
            errors = 0;

            generator.logMessage("Generating HDL for group named " + outFileName +
                "at " + DateTime.Now.ToString() + " containing pages: ");
            string temp = "";
            foreach(string pageName in pageNames) {
                if(temp.Length + pageName.Length > 60) {
                    generator.logMessage(temp);
                    temp = "";
                }
                temp = temp + (temp.Length == 0 ? "\t" : ", ") + pageName;
            }
            if(temp.Length > 0) {
                generator.logMessage(temp);
            }

            //  Is there a template (include, if you will) file to use?
            //  (Is used for both entity and its test bench, if any)

            string templatePath = Path.Combine(directory,
                generator.templateName + "." + generator.generateHDLExtension());
            try {
                generator.templateFile = new StreamReader(templatePath);
            }
            catch (Exception e) {
                generator.logMessage("Unable to open template file " + templatePath +
                    ", " + e.GetType().Name + ", using internally generated defaults");
                generator.templateFile = null;
            }

            //  Are we generating or updating a test bench?  If so,
            //  read it in and save the "interesting" parts, if any,
            //  and open the test bench file for writing.

            generator.savedTestBenchLines = new List<string>();
            generator.savedTestBenchDeclLines = new List<string>();

            if (generateTestBench) {

                string testBenchFileName = outFileName +
                    generator.testBenchSuffix + "." + generator.generateHDLExtension();
                string testBenchPathName = Path.Combine(directory, testBenchFileName);

                //  "Let us preserve what must be preserved".

                try {
                    StreamReader oldTestBenchFile = new StreamReader(testBenchPathName);
                    string testBenchLine;
                    bool saving = false;
                    bool savingDecl = false;

                    while ((testBenchLine = oldTestBenchFile.ReadLine()) != null) {
                        if (testBenchLine.Contains(GenerateHDLLogic.testBenchUserStart)) {
                            saving = true;
                        }
                        else if(testBenchLine.Contains(GenerateHDLLogic.testBenchDeclStart)) {
                            savingDecl = true;
                        }
                        if (saving) {
                            generator.savedTestBenchLines.Add(testBenchLine);
                        }
                        else if (savingDecl) {
                            generator.savedTestBenchDeclLines.Add(testBenchLine);
                        }
                        if (testBenchLine.Contains(GenerateHDLLogic.testBenchUserEnd)) {
                            saving = false;
                        }
                        else if(testBenchLine.Contains(GenerateHDLLogic.testBenchDeclEnd)) {
                            savingDecl = false;
                        }
                    }
                    oldTestBenchFile.Close();
                    generator.logMessage("Old test bench file " + testBenchFileName + " " +
                        generator.savedTestBenchLines.Count.ToString() +
                        " lines preserved and " +
                        generator.savedTestBenchDeclLines.Count.ToString() +
                        " declaration lines preserved");
                }
                catch (Exception e) {
                    generator.logMessage("No existing test bench file " + testBenchPathName +
                        ", " + e.GetType().Name + " , generating default test bench code.");
                }


                try {
                    generator.testBenchFile = new StreamWriter(testBenchPathName);
                    generator.outputStreams.Add(generator.testBenchFile);
                }
                catch (Exception e) {
                    generator.logMessage("Unable to create or open test bench file " +
                        testBenchPathName + " for output.  Test Bench skipped. " +
                        e.GetType().Name);
                    generator.testBenchFile = null;
                }
            }

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
                    generator.logMessage("Unable to open the Test Bench Template File: " +
                        e.GetType().Name + ", file " + testBenchTemplatePathName);
                }

                try {
                    testBenchFPGAClockStream = new StreamReader(
                        testBenchFGPAClockPathName);
                }
                catch (Exception e) {
                    generator.logMessage("Unable to open the Test Bench Clock Template File: " +
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
                    generator.logMessage(
                        "Unable to open the Test Bench Declares Template File: " +
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


            //  Get lists of all of the input and output signals for all of the pages
            //  together.

            generator.logMessage("Building lists of signals on " +
                diagramPageList.Count + " pages...");

            foreach(Diagrampage page in diagramPageList) {
                List<Sheetedgeinformation> signals =
                    sheetEdgeInformationTable.getWhere(
                        "WHERE diagramPage='" + page.idDiagramPage + "'");

                generator.logMessage("Found " + signals.Count +
                    " signals on page " + Helpers.getDiagramPageName(page.idDiagramPage));

                foreach(Sheetedgeinformation signal in signals) {
                    if(signal.leftSide == 1 &&
                        !inputList.Contains(signal.signalName)) {
                        inputList.Add(signal.signalName);
                    }
                    if(signal.rightSide == 1 &&
                        !outputList.Contains(signal.signalName)) {
                        outputList.Add(signal.signalName);
                    }
                    if(!allSignals.Contains(signal.signalName)) {
                        allSignals.Add(signal.signalName);
                    }
                }                
            }

            generator.logMessage("Found " + inputList.Count +
                " unique input signals and " + outputList.Count +
                " unique output signals, (" + allSignals.Count +
                " total unique signals)");

            //  For each input signal used somewhere in the group, check to see
            //  if it originates outside the group.  If so, leave it alone.  If 
            //  not, then move it to the remove list.

            generator.logMessage("Determining sources for all input signals...");

            foreach(string signal in inputList) {
                bool external = false;
                List<Sheetedgeinformation> edgeList =
                    sheetEdgeInformationTable.getWhere(
                        "WHERE signalName='" + signal + "'" +
                        " AND rightSide='1'");
                if(edgeList.Count == 0) {
                    generator.logMessage("ERROR:  Cannot find source of signal named " +
                        signal);
                    ++errors;
                }
                else if(edgeList.Count > 1) {
                    generator.logMessage("ERROR:  More than one source found for signal " +
                        "named " + signal);
                    ++errors;
                }
                foreach(Sheetedgeinformation edge in edgeList) {
                    string pageName = Helpers.getDiagramPageName(edge.diagramPage);
                    if(!pageNames.Contains(pageName)) {
                        external = true;
                        break;
                    }
                }

                generator.logMessage("INFO:  Signal " + signal + " originates " +
                    (external ? "outside" : "inside") + " the group.");

                if(!external) {
                    removedInputSignals.Add(signal);
                }
            }

            //  Now, something similar for output signals, to see if they are used by
            //  any pages outside the group.

            generator.logMessage("Determining destinations for all output signals...");

            foreach (string signal in outputList) {
                bool external = false;

                List<Sheetedgeinformation> edgeList =
                    sheetEdgeInformationTable.getWhere(
                        "WHERE signalName='" + signal + "'" +
                        " AND leftSide='1'");

                if (edgeList.Count == 0) {
                    generator.logMessage("WARNING:  Cannot find any uses of signal named " +
                        signal);
                }

                foreach (Sheetedgeinformation edge in edgeList) {
                    string pageName = Helpers.getDiagramPageName(edge.diagramPage);
                    if (!pageNames.Contains(pageName)) {
                        external = true;
                        break;
                    }
                }

                generator.logMessage("INFO:  Signal " + signal + " is used " +
                    (external ? "outside" : "only inside") + " the group.");

                if (!external) {
                    removedOutputSignals.Add(signal);
                }
            }

            //  Remove any output signals that do not travel outside the group...

            generator.logMessage("Removing " + removedOutputSignals.Count +
                " output signals that do not have destinations outside the group...");

            foreach (string signal in removedOutputSignals) {
                outputList.Remove(signal);
            }

            //  Now, remove the input signals that originate from inside the group...
            //  But, while we are at it, if a signal is also an output signal that
            //  travels outside the group, then remember that 
            //  (older VHDL does not allow reading an output signal)

            //  (This issue cannot crop up with input signals that originate outside
            //  the group, as such a signal would necessarily not originate from any
            //  entity inside the group).

            generator.logMessage("Removing " + removedInputSignals.Count +
                " input signals that originate inside the group...");

            foreach(string signal in removedInputSignals) {
                inputList.Remove(signal);
                if(outputList.Contains(signal)) {
                    generator.logMessage("Signal " + signal +
                        " is output from the group, but also used as an input " +
                        "inside the group.");
                    bufferSignals.Add(signal);
                }
            }

            //
            //  Lamps are output signals too - but they don't appear in the database
            //  so spin all the sheets looking for lamps.
            //

            foreach(Diagrampage page in diagramPageList) {

                int tempErrors = 0;
                List<string> lampNames = getLampNames(page, out tempErrors);
                foreach (string lamp in lampNames) {
                    generator.logMessage("Page " + Helpers.getDiagramPageName(page.idDiagramPage) +
                        " generates Lamp Output " + lamp);
                    outputList.Add(lamp);
                }
                errors += tempErrors;

            }

            //  The signals (wires for Verilog fans) will be any signal in the
            //  orignal full list that is not in what remains of either the input
            //  or the output list.

            generator.logMessage("Generating list of internal signals/wires ...");

            foreach(string signal in allSignals) {
                if(!inputList.Contains(signal) && !outputList.Contains(signal)) {
                    internalSignals.Add(signal);
                }
            }

            generator.logMessage(internalSignals.Count + " internal signals/wires found.");

            generator.logMessage("Generating HDL prefixes...");

            //  Generate the stuff at the beginning of the file

            generator.generateHDLPrefix(outFileName, machine.name);

            //  Generate the entity / module definition

            generator.generateHDLentity(outFileName, inputList, outputList);

            //  Generate the beginning of the actual HDL

            generator.generateHDLArchitecturePrefix(outFileName);

            //  Generate the list of (internal) signals / wires

            generator.generateHDLSignalList(internalSignals, bufferSignals);

            //  Generate the individual page instantiations...

            foreach(Diagrampage page in diagramPageList) {
                List<string> pageInputNames = new List<string>();
                List<string> pageOutputNames = new List<string>();
                List<Sheetedgeinformation> pageInputs =
                    sheetEdgeInformationTable.getWhere(
                        "WHERE diagramPage='" + page.idDiagramPage + "'" +
                        " AND leftSide='1'" +
                        " ORDER BY sheetedgeinformation.row");
                List<Sheetedgeinformation> pageOutputs = 
                    sheetEdgeInformationTable.getWhere(
                        "WHERE diagramPage='" + page.idDiagramPage + "'" +
                        " AND rightSide='1'" +
                        " ORDER BY sheetedgeinformation.row");
                foreach(Sheetedgeinformation edge in pageInputs) {
                    pageInputNames.Add(edge.signalName);
                }
                foreach(Sheetedgeinformation edge in pageOutputs) {
                    pageOutputNames.Add(edge.signalName);
                }
                Page thisPage = pageTable.getByKey(page.page);
                if(thisPage == null || thisPage.idPage == 0) {
                    generator.logMessage(
                        "GenerateGroupHDL: Cannot find page for diagram page with " +
                        "database key of " + page.idDiagramPage + "referring to page " +
                        "with key of " + page.page);
                    ++errors;
                    continue;
                }

                //  Search the page for LAMPS (yes, again...)

                int tempErrors = 0;
                List<string> lampNames = getLampNames(page, out tempErrors);
                foreach(string lamp in lampNames) {
                    pageOutputNames.Add(lamp);
                }
                errors += tempErrors;

                //  Generate the page entity.  For now, just force needsClock to true.

                generator.logMessage("Generating HDL associated with page " + thisPage.name +
                    " (" + thisPage.title + ")");

                generator.generatePageEntity(thisPage.name, thisPage.title,
                    pageInputNames, pageOutputNames, bufferSignals, needsClock);


            }


            //  Generate anything needed at the end

            generator.generateHDLArchitectureSuffix();

            //  Close out the files and logs, and return.

            closeFiles();
            return (errors);
        }

        public List<string> getLampNames(Diagrampage page, out int errors) {
            errors = 0;
            List<string> outputList = new List<string>();

            List<Diagramblock> logicBlocks = diagramBlockTable.getWhere(
                "WHERE diagramPage = '" + page.idDiagramPage + "' " +
                "ORDER BY diagramRow ASC, diagramColumn DESC");

            foreach (Diagramblock block in logicBlocks) {

                string pageCoordinate = Helpers.getDiagramPageName(page.idDiagramPage) +
                        " coordinate " + block.diagramColumn.ToString() +
                        block.diagramRow;

                Cardgate cardGate = cardGateTable.getByKey(block.cardGate);
                if (cardGate == null || cardGate.idcardGate == 0) {
                    generator.logMessage("ERROR:  Invalid Card Gate Key (" +
                        block.cardGate + ") for DiagramBlock " +
                        "on page " + pageCoordinate);
                    ++errors;
                    continue;
                }

                Logicfunction logicFunction = logicFunctionTable.getByKey(
                    cardGate.logicFunction);
                if (logicFunction == null || logicFunction.idLogicFunction == 0) {
                    generator.logMessage("ERROR:  Invalid Logic Function Key (" +
                        cardGate.logicFunction + "), Gate Key (" + cardGate.idcardGate +
                        ") for DiagramBlock " + "on page " + pageCoordinate);
                    ++errors;
                    continue;
                }

                //  Whew.  Now, is it a Lamp?

                if (logicFunction.name != "Lamp") {
                    continue;
                }

                //  Yes.  Fudge up a signal and add it to the list.

                outputList.Add("LAMP_" +
                    Helpers.getCardSlotInfo(block.cardSlot).ToSmallString());
            }

            return (outputList);
        }

        public void closeFiles() {
            generator.outFile.Close();
            generator.logFile.Close();
            if (generator.templateFile != null) {
                generator.templateFile.Close();
            }
            if (generator.testBenchFile != null) {
                generator.testBenchFile.Close();
            }
        }

        public string getLogfileName() {
            return logFileName;
        }

    }
}
