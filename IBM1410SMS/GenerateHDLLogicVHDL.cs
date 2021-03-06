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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

using MySQLFramework;

namespace IBM1410SMS
{
    class GenerateHDLLogicVHDL : GenerateHDLLogic
    {

        private static Regex ROTARYPATTERN { get; } =
            new Regex("^PIN\\d\\d$");

        public GenerateHDLLogicVHDL(
            Page page, bool generateTestBench) : 
            base (page, generateTestBench) {
        }

        public override string generateHDLExtension() {
            return ("vhdl");
        }

        public override void generateHDLNand(List<string> inputs, string output) {
            outFile.WriteLine("\t" + output + " <= NOT(" +
                string.Join(" AND ", inputs) + " );");
        }

        public override void generateHDLNor(List<string> inputs, string output) {
            outFile.WriteLine("\t" + output + " <= NOT(" +
                string.Join(" OR ", inputs) + " );");
        }

        public override void generateHDLOr(List<string> inputs, string output) {
            outFile.WriteLine("\t" + output + " <= " +
                string.Join(" OR ", inputs) + ";");
        }

        public override void generateHDLNot(List<string> inputs, string output) {
            outFile.WriteLine("\t" + output + " <= NOT " + inputs[0] + ";");
        }

        public override void generateHDLEqual(List<string> inputs, string output) {
            outFile.WriteLine("\t" + output + " <= " + inputs[0] + ";");
        }

        public override void generateHDLAnd(List<string> inputs, string output) {
            outFile.WriteLine("\t" + output + " <= " +
                string.Join(" AND ", inputs) + ";");
        }

        public override void generateHDLSignalAssignment(string blockOutput, string outputSignal) {
            outFile.WriteLine("\t" + outputSignal + " <= " + blockOutput + ";");
        }

        public override string generateLogicZero() {
            return ("'0'");
        }

        public override string generateLogicOne() {
            return ("'1'");
        }

        //  Handle generation of VHDL for Special blocks (Logic functions Special and
        //  Trigger, and perhaps more).

        //  NOTE:  Quite a lot of this code would be common with, say Verilog, and
        //  could prehaps be refactored/hoisted into GenerateHDLLogic.

        public override int generateHDLSpecial(LogicBlock block, List<string> inputs,
            List<string> outputs) {

            int temp_errors = 0;
            int inputIndex = 0;
            string mapPin;
            List<string> mapPins = new List<string>();      //  All of the map pins

            if(block.HDLname == null || block.HDLname.Length == 0) {
                logMessage("ERROR: generateHDLSpecial (VHDL): HDLname for block " +
                    "using Special logic function at " +
                    block.getCoordinate() + " is null or zero length.");
                ++temp_errors;
                return (temp_errors);
            }

            //  Build a list of all of the pins in the port map

            foreach(Gatepin pin in block.pins) {
                if(pin.mapPin != null && pin.mapPin.Length > 0 &&
                    !mapPins.Contains(pin.mapPin)) {
                    mapPins.Add(pin.mapPin);
                }
            }

            outFile.WriteLine();
            outFile.WriteLine("\t" + block.HDLname + "_" + block.getCoordinate() + ": " +
                "entity " + block.HDLname);

            //  If this is an oscillator, use the title to figure out the frequency,
            //  and include a generic map for that frequency in KHZ

            if(block.HDLname == "Oscillator" && block.gate.title.Length > 0) {
                string pattern = @"^([0-9\.]+)\s*(kc|khz|mc|mhz)$";
                Match match = Regex.Match(block.gate.title,pattern,RegexOptions.IgnoreCase);
                if (!match.Success || match.Groups.Count != 3) {
                    logMessage(block.logicFunction + " at " +
                        block.getCoordinate() + " unable to decode title of " +
                        block.gate.title);
                    ++temp_errors;
                }
                else {
                    double freq = 0.0f;
                    double multiplier =
                         (match.Groups[2].Captures[0].Value.Substring(0, 1).ToUpper() == "M" ?
                            1.0e6 : 1.0e3);                   
                    double.TryParse(match.Groups[1].Captures[0].Value, out freq);
                    if(freq < 1.0) {
                        logMessage("\tERROR" + block.HDLname + " at " +
                            block.getCoordinate() + " unable to parse frequency of " +
                            match.Groups[1].Captures[0].Value + " from title of " + block.gate.title);
                        ++temp_errors;
                    }
                    else {
                        string clockPeriod = Parms.getParmValue("fpgaclockperiod");
                        if(clockPeriod.Length == 0) {
                            logMessage("\tWARNING:  Parm table fpgaclockperiod not set.  Using 10 ns");
                            clockPeriod = "10";
                            ++temp_errors;
                        }
                        freq = freq * multiplier;
                        logMessage("\tINFO: " + block.HDLname + " at " + block.getCoordinate() +
                            " frequency: " + freq.ToString() + " KHz, Clock Period is " +
                            clockPeriod + " ns");
                        outFile.WriteLine("\t    generic map(FREQUENCY => " + freq.ToString() + 
                            ", CLOCKPERIOD => " + clockPeriod + ")");
                    }
                }
            }

            if(block.HDLname == "SingleShot" && block.gate.title.Length > 0) {
                string pattern = @"^([0-9\.]+)\s*(us|usec|ms|msec)$";
                Match match = Regex.Match(block.gate.title, pattern, RegexOptions.IgnoreCase);
                if (!match.Success || match.Groups.Count != 3) {
                    logMessage(block.logicFunction + " at " +
                        block.getCoordinate() + " unable to decode title of " +
                        block.gate.title);
                    ++temp_errors;
                }
                else {
                    double pulseTime = 0.0f;
                    double multiplier =
                         (match.Groups[2].Captures[0].Value.Substring(0, 1).ToUpper() == "M" ?
                            1.0e6 : 1.0e3);
                    double.TryParse(match.Groups[1].Captures[0].Value, out pulseTime);
                    if (pulseTime < 1.0) {
                        logMessage("\tERROR" + block.HDLname + " at " +
                            block.getCoordinate() + " unable to parse pulse time of " +
                            match.Groups[1].Captures[0].Value + " from title of " + 
                                block.gate.title);
                        ++temp_errors;
                    }
                    else {
                        string clockPeriod = Parms.getParmValue("fpgaclockperiod");
                        if (clockPeriod.Length == 0) {
                            logMessage("\tWARNING:  Parm table fpgaclockperiod not set.  Using 10 ns");
                            clockPeriod = "10";
                            ++temp_errors;
                        }
                        pulseTime = pulseTime * multiplier;
                        logMessage("\tINFO: " + block.HDLname + " at " + 
                            block.getCoordinate() + " pulse time: " + pulseTime.ToString() +
                            " ns, Clock Period is " + clockPeriod + " ns");
                        outFile.WriteLine("\t    generic map(PULSETIME => " + 
                            pulseTime.ToString() + ", CLOCKPERIOD => " + clockPeriod + ")");
                    }
                }
            }

            //  If this is a Delay, it gets implemented as a shift register.
            //  The title should have the delay, otherwise one can fall back on
            //  the component value, as many of these are fixed delays (a few are
            //  configurable)

            if (block.HDLname.Contains("ShiftRegister") && block.gate.title.Length > 0) {
                string pattern = @"^([0-9\.]+)\s*(us|ns)$";

                Match match = Regex.Match(block.gate.title, pattern, RegexOptions.IgnoreCase);
                if (!match.Success || match.Groups.Count != 3) {
                    logMessage(block.logicFunction + " at " +
                        block.getCoordinate() + " unable to decode title of " +
                        block.gate.title);
                    ++temp_errors;
                }
                else {
                    double delay = 0.0f;
                    double multiplier =
                         (match.Groups[2].Captures[0].Value.Substring(0, 1).ToUpper() == "U" ?
                            1.0e3 : 1.0);
                    string number = match.Groups[1].Captures[0].Value;
                    if(number.Substring(0,1) == ".") {
                        number = "0" + number;
                    }
                    double.TryParse(number, out delay);
                    if (delay == 0.0) {
                        logMessage("\tERROR: " + block.HDLname + " at " +
                            block.getCoordinate() + " unable to parse delay time of " +
                            match.Groups[1].Captures[0].Value + " from title of " + block.gate.title);
                        ++temp_errors;
                    }
                    else {
                        string clockPeriod = Parms.getParmValue("fpgaclockperiod");
                        if (clockPeriod.Length == 0) {
                            logMessage("\tWARNING:  Parm table fpgaclockperiod not set.  Using 10 ns");
                            clockPeriod = "10";
                            ++temp_errors;
                        }
                        int intDelay = (int)(delay * multiplier);
                        logMessage("\tINFO: " + block.HDLname + " at " + block.getCoordinate() +
                            " Delay: " + intDelay.ToString() + " ns, Clock Period is " +
                            clockPeriod + " ns");
                        outFile.WriteLine("\t    generic map( DELAY => " + intDelay.ToString() +
                            ", CLOCKPERIOD => " + clockPeriod + ")");
                    }
                }
            }

            outFile.WriteLine("\t    port map (");
            
            //  If this block needs it, give it a clock.

            if(block.logicFunction == "Trigger" || block.HDLname == "Oscillator" ||
                block.HDLname.Contains("ShiftRegister") || block.HDLname == "SingleShot") {
                outFile.WriteLine("\t\tFPGA_CLK => FPGA_CLK,");
            }

            //  First, map the inputs...

            foreach(Connection connection in block.inputConnections) {

                if (connection.toPin == null || connection.toPin.Length == 0) {
                    logMessage("ERROR: Special gate for block at " +
                        block.getCoordinate() + "input from " +
                        inputs[inputIndex] + " has an unnamed input pin -- skipped.");
                    ++temp_errors;
                }
                else {

                    mapPin = null;
                    Gatepin gatePin = null;

                    if((gatePin = block.pins.Find(x => x.pin == connection.toPin)) != null) {
                        mapPin = gatePin.mapPin;
                    }

                    if (mapPin == null || mapPin.Length == 0) {
                        logMessage("ERROR:  Special gate for block at " +
                            block.getCoordinate() + " no mapPin found for input pin "
                            + connection.toPin);
                        ++temp_errors;
                    }
                    else {
                        outFile.WriteLine("\t\t" + mapPin + " => " + 
                            inputs[inputIndex] + "," + "\t" +
                            "-- Pin " + connection.toPin);
                        if(mapPins.Contains(mapPin)) {
                            mapPins.Remove(mapPin);
                        }
                    }
                }

                ++inputIndex;
            }

            //  Then map the outputs.  There will always be at least ONE output.
            //  But, unlike the inputs, we won't know if there will be more
            //  (either outuputs, or open pins), so the comma gets written out
            //  when we have one (other than the first).

            //  Also, unlike the inputs, there can be more than one output from
            //  the same pin.

            bool firstOutputMap = true;
            int outputIndex = 0;
            foreach (Connection connection in block.outputConnections) {

                if (connection.fromPin == null || connection.fromPin.Length == 0) {
                    logMessage("ERROR: Special gate for block at " +
                        block.getCoordinate() + " output from " +
                        outputs[outputIndex] + " has an unnamed output pin -- skipped.");
                    ++temp_errors;
                }
                else {

                    Gatepin mapGatePin = block.pins.Find(x => x.pin == connection.fromPin);
                    if (mapGatePin == null || mapGatePin.idGatePin == 0) {
                        logMessage("ERROR: Special gate for block at " +
                            block.getCoordinate() + " output pin " + connection.fromPin +
                            " not found in gate definition.");
                        mapPin = "";
                    }
                    else {
                        mapPin = mapGatePin.mapPin;
                    }

                    if (mapPin == null || mapPin.Length == 0) {
                        logMessage("ERROR: Special gate for block at " +
                            block.getCoordinate() + " no mapPin found for output pin "
                            + connection.fromPin);
                        ++temp_errors;
                    }
                    else {

                        //  This output pin may have already been mapped, in which case
                        //  we ignore it.

                        if (mapPins.Contains(mapPin)) {
                            if (!firstOutputMap) {
                                outFile.WriteLine(",");
                            }
                            firstOutputMap = false;
                            outFile.Write("\t\t" + mapPin + " => " +
                                outputs[outputIndex]);
                            if (mapPins.Contains(mapPin)) {
                                mapPins.Remove(mapPin);
                            }
                        }
                    }
                }

                ++outputIndex;
            }

            //  If we have any unmapped port map pins, declare them as open.

            foreach(string pin in mapPins) {
                if(!firstOutputMap) {
                    outFile.WriteLine(",");
                }
                firstOutputMap = false;
                outFile.Write("\t\t" + pin + " => OPEN");
            }

            outFile.WriteLine(" );");

            //mapPin = block.pins.Find(x => x.pin == block.outputConnections[0].fromPin).mapPin;

            //if (mapPin == null || mapPin.Length == 0) {
            //    logMessage("ERROR:  Special gate for block at " +
            //        block.getCoordinate() + " no mapPin found for output pin "
            //        + block.outputConnections[0].fromPin);
            //    ++temp_errors;
            //}
            //else {
            //    outFile.WriteLine("\t\t" + mapPin + " => " + output + " );");
            //}

            outFile.WriteLine();
            return (temp_errors);
        }

        //  Switches are generated as inputs, and have some special characteristics.

        public override int generateHDLSwitch(LogicBlock block, List<string> inputs, 
            List<string> outputs) {
            
            int temp_errors = 0;
            string mapPin;
            List<string> mapPins = new List<string>();      // All of the map pins
            string symbol = block.gate.symbol;

            if(symbol != "MOM" && symbol != "TOG" && symbol != "ROT" && symbol != "REL" &&
                 symbol != "ALT") { 
                logMessage("ERROR: Switch for block at " +
                    block.getCoordinate() + " has unknown symbol of " + symbol);
                return (1);
            }

            if((symbol == "MOM" || symbol == "TOG" || symbol == "REL" || symbol == "ALT") && 
                    outputs.Count > 2) {
                logMessage("WARNING: " + symbol + " switch for block at " +
                    block.getCoordinate() + " appears to have more than two outputs ");
                // return (1);
            }

            //  Build a list of all of the pins in the port map
            //  (Only ROT switches will have these.)

            foreach (Gatepin pin in block.pins) {
                if (pin.mapPin != null && pin.mapPin.Length > 0 &&
                    !mapPins.Contains(pin.mapPin)) {
                    mapPins.Add(pin.mapPin);
                }
            }

            //  For switches, any inputs get ignored (at least for now)
            //  as they should be generating logic one or zero.  If we
            //  find one that toggles between one of two signals or some
            //  such, we will have to create a new type besides MOM, TOG, 
            //  REL, ALT or ROT.

            //  If a rotary switch is active high, and contains ONE input
            //  signal, then it will generate an output of (switch AND signal)
            //  TODO

            int outputIndex = 0;
            List<string> usedSwitchPins = new List<string>();
            foreach(Connection connection in block.outputConnections) {

                if (connection.fromPin == null || connection.fromPin.Length == 0) {
                    logMessage("ERROR: Switch for block at " +
                        block.getCoordinate() + " output from " +
                        outputs[outputIndex] + " has an unnamed output pin -- skipped.");
                    ++temp_errors;
                    ++outputIndex;
                    continue;
                }

                Gatepin mapGatePin = block.pins.Find(x => x.pin == connection.fromPin);
                if (mapGatePin == null || mapGatePin.idGatePin == 0) {
                    logMessage("ERROR: Switch for block at " +
                        block.getCoordinate() + " output pin " + connection.fromPin +
                        " not found in gate definition -- skipped.");
                    mapPin = "";
                    ++temp_errors;
                    ++outputIndex;
                    continue;
                }
                else {
                    mapPin = mapGatePin.mapPin.ToUpper();
                }

                //  If this switch pin has already been processed, ignore it
                //  (Currently only for TOG, MOM and REL and ALT - but could easily be
                //  added to ROT as well).

                if(usedSwitchPins.Contains(connection.fromPin)) {
                    ++outputIndex;
                    continue;
                }

                //  Handle rotary switches

                if(symbol == "ROT") {

                    if (mapPin == null || mapPin.Length == 0) {
                        logMessage("ERROR: " + symbol + " switch for block at " +
                            block.getCoordinate() + " no mapPin found for output pin "
                            + connection.fromPin + " -- skipped.");
                        ++temp_errors;
                        ++outputIndex;
                        continue;
                    }

                    if (!ROTARYPATTERN.IsMatch(mapPin)) {
                        logMessage("ERROR: " + symbol + " switch for block at " +
                            block.getCoordinate() + " mapPin of " + mapPin + 
                            " found for output pin " + connection.fromPin + 
                            " is not of the form PIN##");
                        ++temp_errors;
                        ++outputIndex;
                        continue;
                    }

                    int bitNumber = int.Parse(mapPin.Substring(3));

                    //  Handle special one input rotary switch case

                    if(block.switchActiveHigh() && inputs.Count == 1) {
                        outFile.WriteLine("\t" + outputs[outputIndex] + " <= " +
                            generateSignalName(block.getSwitchName()) +
                            "(" + bitNumber + ")" + " AND " +
                            inputs[0] + ";");
                    }
                    else {
                        outFile.WriteLine("\t" + outputs[outputIndex] + " <= " +
                            (block.switchActiveHigh() ? "" : "NOT ") +
                             generateSignalName(block.getSwitchName()) +
                            "(" + bitNumber + ");");
                    }
                }

                if(symbol == "MOM" || symbol == "TOG" || symbol == "REL" || symbol == "ALT") {

                    if(connection.fromPin != "N" && connection.fromPin != "T" &&
                        mapPin != "OUTON" && mapPin != "OUTOFF") {
                        logMessage("ERROR: " + symbol + " switch for block at " +
                            block.getCoordinate() + " output pin " +
                            connection.fromPin + " is not N or T or mapped to " +
                            "OUTON or OUTOFF");
                    }

                    outFile.Write("\t" + outputs[outputIndex] + " <= ");

                    if (mapPin == "OUTON" || mapPin == "OUTOFF") {
                        if ((mapPin == "OUTON" && !block.switchActiveHigh()) ||
                        (mapPin == "OUTOFF" && block.switchActiveHigh())) {
                            outFile.Write(" NOT ");
                        }
                    }
                    else if ((connection.fromPin == "N" && !block.switchActiveHigh()) ||
                        (connection.fromPin == "T" && block.switchActiveHigh())) {
                        outFile.Write(" NOT ");
                    }
                    outFile.WriteLine(generateSignalName(block.getSwitchName()) + ";");

                    //  Mark this output pin as used, so we don't generate the
                    //  switch assignment again.

                    usedSwitchPins.Add(connection.fromPin);
                }

                ++outputIndex;
            }

            return (temp_errors);
        }

        public override int generateHDLDFlipFlop(LogicBlock block) {

            int temp_errors = 0;
            string outputPinName =
                generateOutputPinName(block, block.outputConnections[0], out temp_errors);

            outFile.WriteLine("\t" + LatchPrefix + "_" + block.getCoordinate() + ": " +
                "entity DFlipFlop port map (");
            outFile.WriteLine("\t\tC => " + SystemClockName + ",");
            outFile.WriteLine("\t\tD => " + outputPinName + "_" + LatchPrefix + ",");
            outFile.WriteLine("\t\tQ => " + outputPinName + ",");
            outFile.WriteLine("\t\tQBar => OPEN );");
            outFile.WriteLine();
            return (temp_errors);
        }

        public override void generateHDLArchitectureSuffix() {

            //  Insert any saved lines (or newly generated template lines) into the
            //  test bench, if any.

            foreach(string line in savedTestBenchLines) {
                testBenchFile.WriteLine(line);
            }

            foreach (StreamWriter stream in outputStreams) {
                stream.WriteLine();
                stream.WriteLine("end;");
            }
        }


        //  Class to generate HDL prefix information - standard intro comments,
        //  library declarations, etc.

        public override void generateHDLPrefix() {

            DateTime localTime = DateTime.Now;
            string templateLine;

            foreach (StreamWriter stream in outputStreams) {
                stream.WriteLine("-- " + (stream == testBenchFile ? "Test Bench " : "") +
                    "VHDL for IBM SMS ALD page " + page.name);
                stream.WriteLine("-- Title: " + page.title);
                stream.WriteLine("-- IBM Machine Name " + Helpers.getMachineFromPage(page));
                stream.WriteLine("-- Generated by GenerateHDL at " +
                   localTime.ToString());
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

            // outFile.WriteLine("library xil_defaultlib;");
            // outFile.WriteLine("use xil_defaultlib.all;");
            // outFile.WriteLine();

            //if (needsDFlipFlop) {
            //    outFile.WriteLine("use work.DflipFlop.all;");
            //    outFile.WriteLine();
            //}
        }

        //  Generate the HDL Entity Declaration for both the page VHDL file
        //  and the test bench vhdl file, if present.

        public override int generateHDLEntity(
            bool needsClock,
            List<Sheetedgeinformation> sheetInputsList,
            List<Sheetedgeinformation> sheetOutputsList) {

            bool firstOutput = true;
            int temp_errors = 0;
            this.needsClock = needsClock;

            //  Generate the Entity declaration for both the page and the test bench

            outFile.WriteLine("entity " + getHDLEntityName() + " is");

            //  Generate the start of the test bench declaration

            if(testBenchFile != null) {
                testBenchFile.WriteLine("entity " + getHDLEntityName() +
                    testBenchSuffix + " is");
                testBenchFile.WriteLine("end " + getHDLEntityName() +
                    testBenchSuffix + ";");
                testBenchFile.WriteLine();
                testBenchFile.WriteLine("architecture behavioral of " +
                    getHDLEntityName() + testBenchSuffix + " is");
                testBenchFile.WriteLine();
                testBenchFile.WriteLine(
                    "\t-- Component Declaration for the Unit Under Test (UUT)");
                testBenchFile.WriteLine();
                testBenchFile.WriteLine("\tcomponent " + getHDLEntityName());
            }

            foreach(StreamWriter stream in outputStreams) {
                firstOutput = true;

                stream.WriteLine("\t    Port (");
                if (needsClock) {
                    stream.WriteLine("\t\t" +
                        SystemClockName + ":\t\t in STD_LOGIC;");
                }
                foreach (Sheetedgeinformation signal in sheetInputsList) {
                    stream.WriteLine("\t\t" + generateSignalName(signal.signalName) +
                        ":\t in STD_LOGIC;");
                }

                //  Switches get special handling in the entity declaration

                foreach(LogicBlock lb in logicBlocks) {
                    if(lb.logicFunction == "Switch") {
                        
                        //  If the switch block is an extension, ignore it
                        //  for the entity declaration generation

                        if(lb.ignore) {
                            continue;
                        }

                        string switchName = lb.getSwitchName();
                        switch(lb.gate.symbol) {
                            case "TOG":
                            case "MOM":
                            case "REL":
                            case "ALT":
                                stream.WriteLine("\t\t" + generateSignalName(switchName) +
                                    ":\t in STD_LOGIC;");
                                break;
                            case "ROT":
                                stream.WriteLine("\t\t" + generateSignalName(switchName) +
                                    ":\t in STD_LOGIC_VECTOR(" + (lb.pins.Count - 1).ToString() +
                                    " downTo 0);");
                                break;
                            default:
                                logMessage("ERROR: Unexpected switch symbol: " +
                                    lb.gate.symbol + " at coordinate " +
                                    lb.gate.diagramColumn.ToString() + lb.gate.diagramRow);
                                ++temp_errors;
                                break;
                        }
                    }
                }

                foreach (Sheetedgeinformation signal in sheetOutputsList) {
                    if (!firstOutput) {
                        stream.WriteLine(";");
                    }
                    firstOutput = false;
                    //  Write out signal WITHOUT the ";" so that the LAST 
                    //  one doesn't have one
                    stream.Write("\t\t" + generateSignalName(signal.signalName) +
                        ":\t out STD_LOGIC");
                }

                //  Write out the trailing );
                stream.WriteLine(");");

            }

            outFile.WriteLine("end " + getHDLEntityName() + ";");
            outFile.WriteLine();

            //  For the test bench only, generate the signal declarations to
            //  match the inputs and outputs of the unit under test.

            //  Then also output the "BEGIN" and the instantiation.

            if(testBenchFile != null) {
                testBenchFile.WriteLine("\tend component;");
                testBenchFile.WriteLine();
                testBenchFile.WriteLine("\t-- Inputs");
                testBenchFile.WriteLine();

                //  Now generate any signals that the test bench might need

                //  First, the inputs, with initializers.  Signals that start "M"
                //  are active low, so they get initialized to '1'

                if (needsClock) {
                    testBenchFile.WriteLine("\tsignal " + SystemClockName + ": STD_LOGIC := '0';");
                }

                foreach (Sheetedgeinformation signal in sheetInputsList) {
                    string signalName = generateSignalName(signal.signalName);                    
                    testBenchFile.WriteLine(
                        "\tsignal " + generateSignalName(signal.signalName) +
                        ": STD_LOGIC := '" +
                        (signalName.Substring(0, 1) == "M" ? "1" : "0") + "';");
                }

                //  Then the switch inputs

                foreach (LogicBlock lb in logicBlocks) {
                    if (lb.logicFunction == "Switch") {

                        //  Don't generate these for extensions

                        if(lb.ignore) {
                            continue;
                        }

                        string switchName = lb.getSwitchName();
                        string initValue = "0";
                        switch (lb.gate.symbol) {
                            case "TOG":
                            case "MOM":
                            case "REL":
                            case "ALT":
                                testBenchFile.WriteLine("\tsignal " +
                                    generateSignalName(switchName) +
                                    ": STD_LOGIC := '" + initValue + "';");
                                break;
                            case "ROT":
                                testBenchFile.WriteLine("\tsignal " +
                                    generateSignalName(switchName) +
                                    ": STD_LOGIC_VECTOR(" + (lb.pins.Count - 1).ToString() +
                                    " downTo 0) := \"" +
                                    String.Concat(Enumerable.Repeat(initValue,lb.pins.Count)) +
                                    "\";");
                                break;
                            default:
                                logMessage("ERROR: Unexpected switch symbol: " +
                                    lb.gate.symbol + " at coordinate " +
                                    lb.gate.diagramColumn.ToString() + lb.gate.diagramRow);
                                ++temp_errors;
                                break;
                        }
                    }
                }

                testBenchFile.WriteLine();

                //  Then the outputs

                testBenchFile.WriteLine("\t-- Outputs");
                testBenchFile.WriteLine();

                foreach (Sheetedgeinformation signal in sheetOutputsList) {
                    testBenchFile.WriteLine("\tsignal " + 
                        generateSignalName(signal.signalName) +
                        ": STD_LOGIC;");
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
                testBenchFile.WriteLine("\tUUT: " + getHDLEntityName() + " port map(");

                if(needsClock) {
                    testBenchFile.WriteLine("\t\t" + SystemClockName + " => " +
                        SystemClockName + ",");
                }

                foreach (Sheetedgeinformation signal in sheetInputsList) {
                    string signalName = generateSignalName(signal.signalName);
                    testBenchFile.WriteLine("\t\t" + signalName + " => " +
                        signalName + ",");
                }

                //  Next come the switches, if any.

                foreach (LogicBlock lb in logicBlocks) {
                    if (lb.logicFunction == "Switch") {

                        //  Ignore any extensions

                        if(lb.ignore) {
                            continue;
                        }

                        string switchName = generateSignalName(lb.getSwitchName());
                        testBenchFile.WriteLine("\t\t" + switchName + " => " +
                            switchName + ",");
                    }
                }


                firstOutput = true;
                foreach (Sheetedgeinformation signal in sheetOutputsList) {
                    string signalName = generateSignalName(signal.signalName);
                    if (!firstOutput) {
                        testBenchFile.WriteLine(",");
                    }
                    firstOutput = false;
                    //  Write out signal WITHOUT the "," so that the LAST 
                    //  one doesn't have one
                    testBenchFile.Write("\t\t" + signalName + " => " + signalName);
                }

                //  Write out the trailing );
                testBenchFile.WriteLine(");");
                testBenchFile.WriteLine();
            }

            return (temp_errors);
        }

        public override int generateHDLArchitecturePrefix() {

            int errors = 0;
            int temp_errors = 0;

            outFile.WriteLine("architecture behavioral of " + getHDLEntityName() +
                " is ");
            outFile.WriteLine();
            foreach (LogicBlock block in logicBlocks) {

                if (block.ignore) {
                    continue;
                }

                List<string> processedPins = new List<string>();
                List<string> processedNoPins = new List<string>();

                foreach (Connection connection in block.outputConnections) {

                    //  A DOT function can only have one output "pin"

                    if (block.isDotFunction()) {
                        outFile.WriteLine("\tsignal " +
                            generateOutputPinName(block, connection, out temp_errors) +
                            ": STD_LOGIC;");
                        errors += temp_errors;
                        break;
                    }

                    //  Only gates from here on...

                    //  First handle un-pinned, intra card connections.  
                    //  They are remembered by the diagram block key.

                    if (connection.fromPin == "--") {
                        if(processedNoPins.Contains(connection.fromDiagramBlock.ToString())) {
                            continue;
                        }
                        processedNoPins.Add(connection.fromDiagramBlock.ToString());
                        outFile.WriteLine("\tsignal " +
                            generateOutputPinName(block, connection, out temp_errors) +
                            ": STD_LOGIC;");
                        errors += temp_errors;
                        //  Even these can be latched...
                        if (block.latchOutputs) {
                            outFile.WriteLine("\tsignal " +
                                generateOutputPinName(block, connection, out temp_errors) +
                                "_Latch" + ": STD_LOGIC;");
                            errors += temp_errors;
                        }

                    }
                    else {
                        //  Connection is from an ordinary pin.
                        //  Signal is delcared only once for each gate output pin

                        if (processedPins.Contains(connection.fromPin)) {
                            continue;
                        }

                        processedPins.Add(connection.fromPin);
                        outFile.WriteLine("\tsignal " +
                            generateOutputPinName(block, connection, out temp_errors) +
                            ": STD_LOGIC;");
                        errors += temp_errors;

                        //  If it is latched, we also have to declare that signal as well.

                        if(block.latchOutputs) {
                            outFile.WriteLine("\tsignal " +
                                generateOutputPinName(block, connection, out temp_errors) +
                                "_Latch" + ": STD_LOGIC;");
                            errors += temp_errors;
                        }
                    }
                }
            }

            outFile.WriteLine("");
            outFile.WriteLine("begin");
            outFile.WriteLine("");

            return (errors);
        }
    }
}
