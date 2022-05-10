﻿/* 
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

using System.IO;


namespace IBM1410SMS
{
    class GenerateGroupHDLLogicVHDL : GenerateGroupHDLLogic
    {

        protected class STD_LOGIC_VECTOR  {
            public string signalName { get; set; } = "";
            public string declaration { get; set; } = "";
            public int highIndex = -1;
            public int lowIndex = 9999;
        }

        const string bufferPrefix = "XX_";
        const string lampVector = "LAMP_VECTOR";
        const string switchVector = "SWITCH_VECTOR";

        public GenerateGroupHDLLogicVHDL(bool generateTestBench) :
            base(generateTestBench) {

        }

        public override string generateHDLExtension() {
            return ("vhdl");
        }

        //  Class to generate HDL prefix information - standard intro comments,
        //  library declarations, etc.

        public override void generateHDLPrefix(string name, string machineName) {

            string templateLine;

            foreach(StreamWriter stream in outputStreams) {
                stream.WriteLine("-- " + (stream == testBenchFile ? "Test Bench" : "") +
                    "VHDL for IBM SMS ALD group " + name);
                stream.WriteLine("-- Title: " + name);
                stream.WriteLine("-- IBM Machine Name " + machineName);
                stream.WriteLine("-- Generated by GenerateHDL on " +
                    DateTime.Now.ToString());
                stream.WriteLine();
            }

            //  Include the "template" file here.  If not available, write out some
            //  pretty standard defaults.

            if (templateFile == null) {
                foreach (StreamWriter stream in outputStreams) {
                    stream.WriteLine("library IEEE;");
                    stream.WriteLine("use IEEE.STD_LOGIC_1164.ALL;");
                    stream.WriteLine();
                }
            }
            else {
                while ((templateLine = templateFile.ReadLine()) != null) {
                    foreach (StreamWriter stream in outputStreams) {
                        stream.WriteLine(templateLine);
                    }
                }

                templateFile.Close();
            }


            //outFile.WriteLine("library IEEE;");
            //outFile.WriteLine("use IEEE.STD_LOGIC_1164.ALL;");
            //outFile.WriteLine();
            //outFile.WriteLine("library xil_defaultlib;");
            //outFile.WriteLine("use xil_defaultlib.all;");
            //outFile.WriteLine();
        }

        public override void generateHDLentity(
            string entityName, List<string> inputs, List<string> outputs,
            List<Bussignals> busSignalsList, List<SwitchInfo> switchList,
            Boolean generateLampsAndSwitches, Boolean generateCSharpIndices
            ) {

            bool firstOutput = true;
            // int switchVectorBits = 0;
            // int lampVectorBits = 0;
            List<string> lampNames = new List<string>();

            outFile.WriteLine("entity " + entityName + " is");

            //  Generate the start of the test bench declaration

            if (testBenchFile != null) {
                testBenchFile.WriteLine("entity " + entityName + testBenchSuffix + " is");
                testBenchFile.WriteLine("end " + entityName + testBenchSuffix + ";");
                testBenchFile.WriteLine();
                testBenchFile.WriteLine("architecture behavioral of " +
                    entityName + testBenchSuffix + " is");
                testBenchFile.WriteLine();
                testBenchFile.WriteLine(
                    "\t-- Component Declaration for the Unit Under Test (UUT)");
                testBenchFile.WriteLine();
                testBenchFile.WriteLine("\tcomponent " + entityName);
            }

            foreach(StreamWriter stream in outputStreams) {
                stream.WriteLine("\t    Port (");
                stream.Write("\t\t" + SystemClockName + ": in STD_LOGIC");
                foreach (string signal in inputs) {

                    //  if the signal matches the list of bussed signals, it gets special
                    //  treatment and turns into STD_LOGIC_VECTOR.  It is then "ripped"
                    //  in instantiations of individual pages to map to the particular
                    //  bit in the bus.

                    List<Bussignals> bsList = busSignalsList.FindAll(x => x.busName == signal);
                    string signalName = generateSignalName(signal);

                    if (bsList == null || bsList.Count == 0) {
                        stream.Write(";" + Environment.NewLine + "\t\t" + signalName + ": in STD_LOGIC");
                    }
                    else {
                        STD_LOGIC_VECTOR stdLogicVector =  generateStdLogicVector(bsList, signalName);
                        stream.Write(";" + Environment.NewLine + "\t\t" + 
                            signalName + ": in " + stdLogicVector.declaration);
                    }
                }

                foreach(SwitchInfo switchEntry in switchList) {
                    stream.Write(";" + Environment.NewLine + "\t\t" +
                        generateSignalName(switchEntry.switchName) + ": in ");
                    if(switchEntry.rotaryCount == 0) {
                        stream.Write("STD_LOGIC");
                    }
                    else {
                        stream.Write("STD_LOGIC_VECTOR(" +
                            (switchEntry.rotaryCount - 1).ToString() + " downTo 0)");
                    }
                }

                foreach (string signal in outputs) {

                    //  Same special treatment for output bus signals for for input bus signals.
                    //  Since a given signal (and thus a given bus bit) can only ever originate on
                    //  a single sheet, we can used named association, just like we did for inputs.

                    List<Bussignals> bsList = busSignalsList.FindAll(x => x.busName == signal);
                    string signalName = generateSignalName(signal);

                    if (bsList == null || bsList.Count == 0) {
                        stream.Write(";" + Environment.NewLine + "\t\t" + signalName + ": out STD_LOGIC");
                    }
                    else {
                        STD_LOGIC_VECTOR stdLogicVector = generateStdLogicVector(bsList, signalName);
                        stream.Write(";" + Environment.NewLine + "\t\t" +
                            signalName + ": out " + stdLogicVector.declaration);
                    }
                }

                stream.WriteLine(");");
            }

            outFile.WriteLine("end " + entityName + ";");
            outFile.WriteLine();

            //  For the test bench only, generate the signal declarations to
            //  match the inputs and outputs of the unit under test.

            //  Then also output the "BEGIN" and the instantiation.

            if (testBenchFile != null) {
                testBenchFile.WriteLine("\tend component;");
                testBenchFile.WriteLine();
                testBenchFile.WriteLine("\t-- Inputs");
                testBenchFile.WriteLine();

                //  Now generate any signals that the test bench might need

                //  First, the inputs, with initializers.  Signals that start "M"
                //  are active low, so they get initialized to '1'

                testBenchFile.WriteLine("\tsignal " + SystemClockName + ": STD_LOGIC := '0';");

                //  Here again, bussed signals get special treatent.  Instead of being
                //  STD_LOGIC, they are STD_LOGIC_VECTOR and are initialized with a 

                foreach (string signal in inputs) {
                    string signalName = generateSignalName(signal);
                    string initialValue = signalName.Substring(0, 1) == "M" ? "1" : "0";

                    List<Bussignals> bsList = busSignalsList.FindAll(x => x.busName == signal);

                    if (bsList == null || bsList.Count == 0) {
                        testBenchFile.WriteLine("\tsignal " + signalName + ": STD_LOGIC := '" +
                         initialValue + "';");
                    }
                    else {
                        STD_LOGIC_VECTOR stdLogicVector =
                            generateStdLogicVector(bsList, signalName);
                        testBenchFile.WriteLine("\tsignal " + signalName + ": " +
                            stdLogicVector.declaration + " := \"" +
                                String.Concat(Enumerable.Repeat(initialValue, stdLogicVector.highIndex -
                                    stdLogicVector.lowIndex + 1)) + "\";");
                    }
                }

                //  And, here come the switches yet again...

                //  Note that outside of the individual page logic, switches are
                //  always active high.

                foreach(SwitchInfo switchEntry in switchList) {
                    testBenchFile.Write("\tsignal " +
                        generateSignalName(switchEntry.switchName));
                    if(switchEntry.rotaryCount == 0) {
                        testBenchFile.WriteLine(": STD_LOGIC := '0';");
                        if(switchEntry.notes == null || !switchEntry.notes.Contains("NOVECTOR")) {
                            ++switchVectorBits;
                        }
                    }
                    else {
                        testBenchFile.WriteLine(": STD_LOGIC_VECTOR(" +
                            (switchEntry.rotaryCount - 1).ToString() + " downTo 0) := \"" +
                            String.Concat(Enumerable.Repeat("0", switchEntry.rotaryCount)) +
                            "\";");
                        if(switchEntry.notes == null || !switchEntry.notes.Contains("NOVECTOR")) {
                            switchVectorBits += switchEntry.rotaryCount;
                        }
                    }
                }

                testBenchFile.WriteLine();


                //  Then the outputs

                testBenchFile.WriteLine("\t-- Outputs");
                testBenchFile.WriteLine();

                //  Once again, if this is a bussed signal, it gets special treatment, but
                //  without the initialization.

                foreach (string signal in outputs) {
                    string signalName = generateSignalName(signal);
                    string initialValue = signalName.Substring(0, 1) == "M" ? "1" : "0";
                    Boolean isLamp = signal.StartsWith("LAMP_") | signal.StartsWith("LAMPS_") |
                        signal.StartsWith("LAMPS ");

                    logMessage("Generating output entry for signal " + signal);

                    List<Bussignals> bsList = busSignalsList.FindAll(x => x.busName == signal);

                    if (bsList == null || bsList.Count == 0) {
                        testBenchFile.WriteLine("\tsignal " + signalName + ": STD_LOGIC;");
                        if(isLamp) {
                            lampNames.Add(signalName);
                            ++lampVectorBits;
                        }
                    }
                    else {
                        STD_LOGIC_VECTOR stdLogicVector =
                            generateStdLogicVector(bsList, signalName);
                        testBenchFile.WriteLine("\tsignal " + signalName + ": " +
                            stdLogicVector.declaration + ";");

                        if (isLamp) {
                            logMessage("Adding bussed lamps " + signalName + " " +
                                stdLogicVector.highIndex + " down to " + stdLogicVector.lowIndex);
                            lampNames.Add(signalName);
                            lampVectorBits += (stdLogicVector.highIndex - stdLogicVector.lowIndex + 1);
                        }
                    }
                }

                //  We want lampVectorBits to be evenly divisible by the number of bits sent
                //  per character to host - currently 7

                logMessage("DEBUG: LampVectorBits BEFORE rounding: " + lampVectorBits.ToString());
                lampVectorBits = ((lampVectorBits + 6) / 7) * 7;

                //  If we are generating lamp and switch vectors, add those signal declarations

                if (generateLampsAndSwitches) {
                    testBenchFile.WriteLine();
                    testBenchFile.WriteLine("\tsignal " + lampVector + ": STD_LOGIC_VECTOR (" +
                        (lampVectorBits - 1).ToString() + " downTo 0);");
                    testBenchFile.WriteLine("\tsignal " + switchVector + ": STD_LOGIC_VECTOR (" +
                        (switchVectorBits - 1).ToString() + " downTo 0);");

                }

                if(generateCSharpIndices) {
                    cSharpFile.WriteLine();
                    cSharpFile.WriteLine("\tpublic const int lampVectorBits = " + lampVectorBits.ToString() + ";");
                    cSharpFile.WriteLine("\tpublic const int switchVectorBits = " + switchVectorBits.ToString() + ";");
                    cSharpFile.WriteLine();
                }

                //  Write out the template or saved user *declaration* lines, which must
                //  preceed "begin"

                testBenchFile.WriteLine();

                foreach (string line in savedTestBenchDeclLines) {
                    testBenchFile.WriteLine(line);
                }

                //  Write out the BEGIN and the instantiation of the unit under test

                testBenchFile.WriteLine();
                testBenchFile.WriteLine("\tbegin");
                testBenchFile.WriteLine();
                testBenchFile.WriteLine("\t-- Instantiate the Unit Under Test (UUT)");
                testBenchFile.WriteLine();
                testBenchFile.WriteLine("\tUUT: " + entityName + " port map(");

                testBenchFile.WriteLine("\t\t" + SystemClockName + " => " +
                        SystemClockName + ",");

                foreach (string signal in inputs) {
                    string signalName = generateSignalName(signal);
                    testBenchFile.WriteLine("\t\t" + signalName + " => " + signalName + ",");
                }

                foreach(SwitchInfo switchEntry in switchList) {
                    string signalName = generateSignalName(switchEntry.switchName);
                    testBenchFile.WriteLine("\t\t" + signalName + " => " + signalName + ",");
                }


                firstOutput = true;

                foreach (string signal in outputs) {
                    string signalName = generateSignalName(signal);
                    if (!firstOutput) {
                        testBenchFile.WriteLine(",");
                    }
                    testBenchFile.Write("\t\t" + signalName + " => " + signalName);
                    firstOutput = false;
                }

                //  Write out the trailing );

                testBenchFile.WriteLine(");");
                testBenchFile.WriteLine();
            }

        }

        public override void generateHDLArchitecturePrefix(string name) {
            outFile.WriteLine();
            outFile.WriteLine("ARCHITECTURE structural of " + name + " is");
        }

        public override void generateHDLSignalList(
            List<string> signals, List<string> bufferSignals, 
            List<Bussignals> busSignalsList, List<string> busOutputList) {

            outFile.WriteLine();
            foreach(string signal in signals) {
                outFile.WriteLine("\t signal " +
                    generateSignalName(signal) + ": STD_LOGIC;");
            }
            if (signals.Count > 0) {
                outFile.WriteLine();
            }

            //  Since we are generating VHDL, also generate a signal for
            //  anything that appears as an output in the port map, but is
            //  also used internally.

            foreach(string signal in bufferSignals) {
                outFile.WriteLine("\t signal " + bufferPrefix +
                    generateSignalName(signal) + ": STD_LOGIC;");
            }
            if(bufferSignals.Count > 0) {
                outFile.WriteLine();
            }

            outFile.WriteLine("BEGIN");
            outFile.WriteLine();

            //  Now we also generate assignments for the buffer signals which are
            //  NOT part of a bus.

            foreach(string signal in bufferSignals) {
                Bussignals bs = busSignalsList.Find(x => x.signalName == signal);
                if(bs == null || !busOutputList.Contains(bs.busName)) {
                    outFile.WriteLine("\t" + generateSignalName(signal) + " <= ");
                    outFile.WriteLine("\t\t" + bufferPrefix + generateSignalName(signal) +
                        ";");
                }
            }

            if(bufferSignals.Count > 0) {
                outFile.WriteLine();
            }

            //  Then we generate assignments to aggregate OUTPUT busses

            foreach(string busName in busOutputList) {
                List<Bussignals> busSignals = busSignalsList.FindAll(
                    x => x.busName == busName);
                if(busSignals == null) {
                    logMessage("ERROR:  Generating bus assignment - bus " +
                        busName + " does not appear in busSignalsList");
                    continue;
                }

                //  These need to be in REVERSE bit order.  Also, we need to fill
                //  in any unused bit slots with 0's.

                bool first = true;
                outFile.WriteLine("\t" + generateSignalName(busName) + " <= (");
                int lastBit = -1;

                for (int i = busSignals.Count-1; i >= 0; --i) {
                    Bussignals bs = busSignals[i];
                    int thisBit = busSignals[i].busBit;
                    if(!first) {
                        outFile.WriteLine(",");
                        //  Fill in any unused bits with 0's
                        while(thisBit < lastBit-1) {
                            outFile.WriteLine("\t\t'0',");
                            --lastBit;
                        }
                    }
                    first = false;
                    outFile.Write("\t\t" + bufferPrefix + 
                        generateSignalName(bs.signalName));
                    lastBit = thisBit;
                }

                outFile.WriteLine(");");
                outFile.WriteLine("");
            }

        }

        //  Method to generate assignments to the individual switch signals from
        //  the switch vector.  (To combine these with switches from the FPGA 
        //  development board, comment out this assignment, and manually enter 
        //  a signal assignment with an "or".

        public override void generateHDLSwitchAssignments(List<SwitchInfo> switchList,
            bool generateLampsAndSwitches, bool generateCSharpIndices) {
            int currentBit = switchVectorBits - 1;

            switchList.Sort((x, y) => x.switchName.CompareTo(y.switchName));

            testBenchFile.WriteLine();
            testBenchFile.WriteLine("-- Assign switches from switch vector received from PC host");
            testBenchFile.WriteLine();

            foreach (SwitchInfo switchInfo in switchList) {
                if(switchInfo.notes == null || !switchInfo.notes.Contains("NOVECTOR")) {
                    if(switchInfo.rotaryCount <= 1) {
                        if(generateLampsAndSwitches) {
                            testBenchFile.WriteLine("\t" + generateSignalName(switchInfo.switchName) + " <= " +
                                switchVector + "(" + currentBit.ToString() + "); -- " +
                                switchInfo.pageName);
                        }
                        if(generateCSharpIndices) {
                            cSharpFile.WriteLine("\tpublic const int " + generateSignalName(switchInfo.switchName) + "_INDEX = " +
                                currentBit.ToString() + ";\t// " + switchInfo.pageName);
                        }
                        --currentBit;
                    }
                    else {
                        if(generateLampsAndSwitches) {
                            testBenchFile.WriteLine("\t" + generateSignalName(switchInfo.switchName) + " <= " +
                                switchVector + "(" + currentBit.ToString() +
                                " downto " + (currentBit - (switchInfo.rotaryCount - 1)).ToString() +
                                "); -- " + switchInfo.pageName);
                        }
                        if (generateCSharpIndices) {
                            cSharpFile.WriteLine("\tpublic const int " + generateSignalName(switchInfo.switchName) + "_INDEX = " +
                                (currentBit - (switchInfo.rotaryCount - 1)).ToString() +
                                ");\t// " + switchInfo.pageName);
                            cSharpFile.WriteLine("\tpublic const int " + generateSignalName(switchInfo.switchName) + "_LEN = " +
                                switchInfo.rotaryCount.ToString() + ";\t// " + switchInfo.pageName);

                        }

                        currentBit -= switchInfo.rotaryCount;
                    }
                }
            }

            testBenchFile.WriteLine();

        }

        //  Method to generate assignments to the lamp vector from the individual lamps.

        public override void generateHDLLampAssignments(List<LampInfo> lampList,
            List<Bussignals> busSignalsList, 
            bool generateLampsAndSwitches,
            bool generateCSharpIndices) {
            int currentBit = lampVectorBits - 1;

            lampList.Sort((x, y) => x.lampName.CompareTo(y.lampName));

            testBenchFile.WriteLine();
            testBenchFile.WriteLine("-- Assign lamp vector from lamps for transmission to PC host");
            testBenchFile.WriteLine();

            //  First, process the individual lamps

            foreach (LampInfo lampInfo in lampList) {
                String lampName = lampInfo.lampName;
                List<Bussignals> bsList = busSignalsList.FindAll(x => x.busName == lampInfo.lampName);
                if (bsList.Count <= 1) {
                    if(generateLampsAndSwitches) {
                        testBenchFile.WriteLine("\t" + lampVector + "(" + currentBit.ToString() +
                            ") <= " + generateSignalName(lampName) + ";" +
                            "  -- " + lampInfo.title + " " + lampInfo.pageName);
                    }
                    if(generateCSharpIndices) {
                        cSharpFile.WriteLine("\tpublic const int " + generateSignalName(lampName) + "_INDEX = " +
                            currentBit.ToString() + ";\t// " + lampInfo.title + " " + lampInfo.pageName);
                    }
                    --currentBit;
                }
                else {
                    STD_LOGIC_VECTOR stdLogicVector =
                        generateStdLogicVector(bsList, lampInfo.lampName);
                    int bitCount = stdLogicVector.highIndex - stdLogicVector.lowIndex + 1;
                    if(generateLampsAndSwitches) {
                        testBenchFile.WriteLine("\t" + lampVector + "(" + currentBit.ToString() +
                           " downto " + (currentBit - bitCount + 1).ToString() +
                           ") <= " + generateSignalName(lampName) + ";" +
                           "  -- " + lampInfo.title + " " + lampInfo.pageName);
                    }
                    if(generateCSharpIndices) {
                        cSharpFile.WriteLine("\tpublic const int " + generateSignalName(lampName) + "_INDEX = " +
                            (currentBit - bitCount + 1).ToString() + ";\t// " + lampInfo.title + " " + lampInfo.pageName);
                        cSharpFile.WriteLine("\tpublic const int " + generateSignalName(lampName) + "_LEN = " +
                            bitCount.ToString() + ";\t// " + lampInfo.title + " " + lampInfo.pageName);

                    }
                    currentBit -= bitCount;
                }
            }

            //  Assign 0 to any left over bits that were created so that the lamp vector
            //  size was modulo 7.

            for(; currentBit >= 0; --currentBit) {
                testBenchFile.WriteLine("\t" + lampVector + "(" + currentBit.ToString() +
                    ") <= '0';");
            }

        }

        public override void generateHDLArchitectureSuffix() {

            //  Insert any saved lines (or newly generated template lines) into the
            //  test bench, if any.

            foreach (string line in savedTestBenchLines) {
                testBenchFile.WriteLine(line);
            }

            foreach(StreamWriter stream in outputStreams) {
                stream.WriteLine();
                stream.WriteLine("END;");
            }
        }

        public override void generatePageEntity(string pageName, string pageTitle, 
            List<string> inputs, List<string> outputs, List<Bussignals> busSignalsList,
            List<string> bufferSignals, List<string> internalSignals,
            List<string> busInputSignals, List<string> busOutputSignals, 
            List<SwitchInfo> switchList, bool needsClock) {

            bool firstPort = true;

            outFile.WriteLine("Page_" +
                replacePeriods.Replace(pageName, "_") + ": ENTITY " +
                generateHDLEntityName(pageName, pageTitle));
            outFile.WriteLine("   PORT MAP (");

            if(needsClock) {
                outFile.Write("\t" + SystemClockName + " => " +
                    SystemClockName);
                firstPort = false;
            }

            foreach(string input in inputs) {
                if(!firstPort) {
                    outFile.WriteLine(",");
                }
                outFile.WriteLine("\t" + generateSignalName(input) + " =>");
                outFile.Write("\t\t");

                //  Here again, bussed signals get special treatment, and are "ripped" to map
                //  the signal in the page instantiation to the particular bit in the bussed
                //  input signal -- but only if they are not buffered and not internal signals

                Bussignals bs = busSignalsList.Find(x => x.signalName == input);
                if(bufferSignals.Contains(input)) {
                    outFile.Write(bufferPrefix + generateSignalName(input));
                }
                else if (bs != null && !internalSignals.Contains(input)) {
                    outFile.Write(generateSignalName(bs.busName + "(" + bs.busBit.ToString() + ")"));
                }
                else {
                    outFile.Write(generateSignalName(input));
                }
                firstPort = false;
            }

            foreach(SwitchInfo switchEntry in switchList) {
                string switchName = generateSignalName(switchEntry.switchName);
                if (!firstPort) {
                    outFile.WriteLine(",");
                }
                outFile.WriteLine("\t" + switchName + " =>");
                outFile.Write("\t\t" + switchName);
                firstPort = false;
            }

            foreach (string output in outputs) {
                if (!firstPort) {
                    outFile.WriteLine(",");
                }
                outFile.WriteLine("\t" + generateSignalName(output) + " =>");
                outFile.Write("\t\t");

                /*
                //  Once more, bussed signals get special treatment, and are mapped to the
                //  signal in the page instantiation to the particuar bit in the bussed signal.

                Bussignals bs = busSignalsList.Find(x => x.signalName == output);
                if (bs != null && busOutputList.Contains(bs.busName)) {
                    outFile.Write(generateSignalName(bs.busName + "(" + bs.busBit.ToString() + ")"));
                }
                else {
                    outFile.Write((bufferSignals.Contains(output) ? bufferPrefix : "") +
                        generateSignalName(output));                
                }
                */

                outFile.Write((bufferSignals.Contains(output) ? bufferPrefix : "") +
                    generateSignalName(output));

                firstPort = false;
            }

            outFile.WriteLine();
            outFile.WriteLine("\t);");
            outFile.WriteLine();
        }

        //  Routine to generate an appropriate VHDL declaration for a switch

        public override string generateSwitchEntry(string switchName, 
            bool declaration, int vectorCount) {
            if (vectorCount == 0) {
                return (generateSignalName(switchName) + 
                    (declaration ? ": STD_LOGIC" : ""));
            }
            else {
                return (generateSignalName(switchName) + 
                    (declaration ? ": STD_LOGIC_VECTOR(" +
                    (vectorCount - 1).ToString() + " downTo 0)" : ""));
            }
        }

        //  Routine to generate part of a STD_LOGIC_VECTOR declaration.

        protected STD_LOGIC_VECTOR generateStdLogicVector(List<Bussignals> bsList, string signalName) {
            STD_LOGIC_VECTOR stdLogicVector = new STD_LOGIC_VECTOR();

            foreach (Bussignals bs in bsList) {
                if (bs.busBit > stdLogicVector.highIndex) {
                    stdLogicVector.highIndex = bs.busBit;
                }
                if (bs.busBit < stdLogicVector.lowIndex) {
                    stdLogicVector.lowIndex = bs.busBit;
                }
            }

            stdLogicVector.declaration = "STD_LOGIC_VECTOR (" +
                stdLogicVector.highIndex.ToString() + " downTo " + stdLogicVector.lowIndex.ToString() + ")";

            return (stdLogicVector);
        }
    }
}
