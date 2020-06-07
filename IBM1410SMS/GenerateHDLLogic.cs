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

//  This class is the super class (mostly or entirely virtual) that encapsulates
//  HDL language generation, to make the GenerateHDL class a bit cleaner.

//  I didn't use an Interface, because I wanted to be able implement some things
//  in the superclass.

namespace IBM1410SMS
{
    public abstract class GenerateHDLLogic
    {

        public const string testBenchUserStart = "START USER TEST BENCH";
        public const string testBenchUserEnd = "END USER TEST BENCH";

        public StreamWriter outFile { get; set; }
        public StreamWriter logFile { get; set; }
        public StreamWriter testBenchFile { get; set; }
        public StreamReader templateFile { get; set; }
        public List<StreamWriter> outputStreams { get; set; } =
            new List<StreamWriter>();

        public List<string> savedTestBenchLines { get; set; }
        public List<LogicBlock> logicBlocks { get; set; }
        protected Page page { get; set; }

        public bool needsDFlipFlop { get; set; }
        public bool generateTestBench { get; set; }
        public bool needsClock { get; set; }

        protected string LatchPrefix { get; set; } = "Latch";
        protected string SystemClockName { get; set; } = "FPGA_CLK";
        public string testBenchSuffix { get; set; } = "_tb";
        public string templateName { get; set; } = "HDLTemplate";

        Regex replacePeriods = new Regex("\\.");
        Regex replaceTitle = new Regex(" |-|\\.|\\+|\\-|\\*");

        public GenerateHDLLogic(
            Page page, bool generateTestBench) {

            this.page = page;
            this.generateTestBench = generateTestBench;
            testBenchFile = null;
            templateFile = null;
            needsClock = false;
        }

        //  Method to return the proper extension to use.

        public abstract string generateHDLExtension();

        //  Basic logic methods

        public abstract void generateHDLNand(List<string> inputs, string output);
        public abstract void generateHDLNor(List<string> inputs, string output);
        public abstract void generateHDLOr(List<string> inputs, string output);
        public abstract void generateHDLNot(List<string> inputs, string output);
        public abstract void generateHDLEqual(List<string> inputs, string output);
        public abstract void generateHDLSignalAssignment(string blockOutput, string outputSignal);
        public abstract int generateHDLSpecial(LogicBlock block, List<string> inputs,
           List<string> outputs);

        //  Method to generate the statements for a D Flip Flop used on a latch
        //  output.  Returns error count.

        public abstract int generateHDLDFlipFlop(LogicBlock block);

        //  Method to generate HDL prefix information - standard intro comments,
        //  library declarations, etc.

        public abstract void generateHDLPrefix();

        //  Method to generate the "entity" declaration, if required.

        public abstract void generateHDLEntity(
            bool needsClock,
            List<Sheetedgeinformation> sheetInputsList,
            List<Sheetedgeinformation> sheetOutputsList);

        //  Method to generate "architecture" prefix, if required.  
        //  Returns number of errors.

        public abstract int generateHDLArchitecturePrefix();

        //  Method to generate statements at the end of the "architecture" if required.

        public abstract void generateHDLArchitectureSuffix();

        //  Method to generate the pin name of the output from a logic block.
        //  This name is presumed/hoped to be HDL language independent.

        public string generateOutputPinName(LogicBlock block, 
            Connection connection, out int errors) {

            errors = 0;

            //  A DOT function output is always its coordinate (no defined pins)

            if (block.isDotFunction()) {
                return ("OUT_DOT_" + block.getCoordinate());
            }

            //  Lamps get special handling as well - the output pin name is always
            //  OUT_LAMP_<coordinate>

            if(block.logicFunction == "Lamp") {
                return ("OUT_LAMP_" + block.getCoordinate());
            }

            //  A -- connection is named OUT_From_NoPin

            if (connection.fromPin == "--") {
                if (connection.toDiagramBlock == 0 || connection.to != "P") {
                    logMessage("Error: Connection from -- is not to a pin.");
                    ++errors;
                    return ("*Invalid*");
                }
                if (connection.toPin != "--") {
                    logMessage("Error:  Connection from -- is to named pin " +
                        connection.toPin);
                    ++errors;
                    return ("*Invalid*");
                }
                LogicBlock destination = logicBlocks.Find(
                    x => x.gate.idDiagramBlock == connection.toDiagramBlock);
                if (destination == null ||
                    destination.gate.idDiagramBlock != connection.toDiagramBlock) {
                    logMessage("Error:  Cannot find matching -- diagram block in" +
                        "Blocks for this page (Database ID=" +
                        connection.toDiagramBlock + ")");
                    ++errors;
                    return ("*Invalid*");
                }

                // return ("OUT_" + block.getCoordinate() + "_" + destination.getCoordinate());
                return ("OUT_" + block.getCoordinate() + "_" + "NoPin");
            }
            else {
                //  Connection is from an ordinary pin.
                return ("OUT_" + block.getCoordinate() + "_" + connection.fromPin);
            }
        }

        //  Method to produce the name to use for a signal.  As with the method above,
        //  it is hoped/presumed that this one will be HDL language independent.

        public string generateSignalName(string signal) {
            string outString = "";

            if (signal.Substring(0, 1) == "+") {
                outString = "P" + signal.Substring(1);
            }
            else if (signal.Substring(0, 1) == "-") {
                outString = "M" + signal.Substring(1);
            }
            else {
                outString = signal;
            }

            outString = outString.Replace(" ", "_");
            outString = outString.Replace(".", "_DOT_");
            outString = outString.Replace("+", "_OR_");
            outString = outString.Replace("*", "_STAR_");
            outString = outString.Replace("-", "_");

            //  VHDL won't allow consecutive underscores (which can happen for
            //  inputs like ... + ... , so...

            outString = outString.Replace("__", "_");

            return (outString);
        }

        //  Method to get the HDL entity name

        public string getHDLEntityName() {
            return
                "ALD_" +
                replacePeriods.Replace(page.name, "_") + "_" +
                replaceTitle.Replace(page.title, "_");
        }

        public void logMessage(string message) {
            logFile.WriteLine(message);
            logFile.Flush();
        }
    }
}
