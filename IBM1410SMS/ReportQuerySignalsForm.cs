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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Collections;
using MySQLFramework;

namespace IBM1410SMS
{
    public partial class ReportQuerySignalsForm : Form
    {

        //  Class to store information on a given signal reference

        //class SignalReference
        //{
        //    public string pageName { get; set; } = "";
        //    public string row { get; set; } = "";
        //    public string side { get; set; } = "L";
        //}

        //class SignalInstance
        //{
        //    public List<string> = null;
        //}


        DBSetup db = DBSetup.Instance;

        Table<Machine> machineTable;
        Table<Sheetedgeinformation> sheetEdgeInformationTable;
        Table<Diagrampage> diagramPageTable;
        Table<Page> pageTable;

        List<Machine> machineList;
        List<Diagrampage> diagramPageList;

        Machine currentMachine = null;

    
        public ReportQuerySignalsForm() {
            InitializeComponent();

            machineTable = db.getMachineTable();
            sheetEdgeInformationTable = db.getSheetEdgeInformationTable();
            diagramPageTable = db.getDiagramPageTable();
            pageTable = db.getPageTable();

            machineList = machineTable.getAll();

            //  Fill in the machine combo box, and remember which machine
            //  we started out with.

            machineComboBox.DataSource = machineList;

            string lastMachine = Parms.getParmValue("machine");
            if (lastMachine.Length != 0) {
                currentMachine = machineList.Find(
                    x => x.idMachine.ToString() == lastMachine);
            }

            if (currentMachine == null || currentMachine.idMachine == 0) {
                currentMachine = machineList[0];
            }
            else {
                machineComboBox.SelectedItem = currentMachine;
            }


        }

        private void reportButton_Click(object sender, EventArgs e) {

            List<String> messages = new List<string>();
            diagramPageList = new List<Diagrampage>();

            //  Key is signal name:side

            Dictionary<string, List<string>> signalDict =
                new Dictionary<string, List<string>>();

            //  Save current machine for later use by this and other dialogs

            Parms.setParmValue("machine", currentMachine.idMachine.ToString());

            messages.Add("Begin Signal Query Report for machine: " +
                currentMachine.name);
            messages.Add("Signal pattern: " + signalTextBox.Text);

            //  Find the relevant pages

            List<Page> pageList = pageTable.getWhere("WHERE machine = '" +
                currentMachine.idMachine + "'");

            //  And then the relevant diagram pages

            foreach (Page page in pageList) {
                List<Diagrampage> dpList = diagramPageTable.getWhere(
                    "WHERE page = '" + page.idPage + "'");
                if (dpList.Count <= 0) {
                    //  This might not be a diagram page
                    continue;
                }
                else if (dpList.Count > 1) {
                    MessageBox.Show("Warning: More than one diagram page found for " +
                        "page " + page.name + "(" + page.idPage.ToString() + ")",
                        "More than one diagram page for page",
                        MessageBoxButtons.OKCancel,MessageBoxIcon.Warning);
                }
                else {
                    diagramPageList.Add(dpList[0]);
                }
            }

            //  Now, get the signals

            List<Sheetedgeinformation> seiList = sheetEdgeInformationTable.getWhere(
                "WHERE signalName LIKE '" + signalTextBox.Text + "'");
            foreach(Sheetedgeinformation sei in seiList) {

                //  Is this one relevant to this machine?  If not, skip it.

                Diagrampage dp = diagramPageList.Find(
                    x => x.idDiagramPage == sei.diagramPage);
                if(dp == null || dp.idDiagramPage == 0) {
                    continue;
                }

                //  Got one

                string pageName = pageList.Find(
                    x => x.idPage == dp.page).name;
                List<string> signals;

                //  If this is a first for this signal:side, create a new
                //  dictionary entry.

                string signalKey = sei.signalName + ":" +
                    (sei.leftSide > 0 ? "U" : "O");
                signalDict.TryGetValue(signalKey, out signals);
                if(signals == null) {
                    signals = new List<string>();
                    signalDict.Add(signalKey, signals);
                }

                //  Add this one to the list.

                signals.Add(pageName);
            }

            //  Get a list of keys in the dictionary, sorted by signal name : Side
            //  So, the left side comes first.  ;)

            ArrayList sortedSignals = new ArrayList(signalDict.Keys);
            sortedSignals.Sort();

            string references = "";
            string lastSignalName = "";
            foreach(string signal in sortedSignals) {

                string signalName = signal.Split(
                    new char[] { ':' })[0];
                string side = signal.Substring(signal.Length - 1);
                switch(side) {
                    case "O":
                        side = "*";
                        break;
                    case "U":
                        side = "";
                        break;                    
                }

                if (signalName != lastSignalName) {
                    if(lastSignalName.Length > 0) {
                        if (references.Length > 4) {
                            messages.Add(references);
                        }
                        messages.Add("");
                    }
                    lastSignalName = signalName;
                    messages.Add("Signal: " + signalName);
                    references = "    ";
                }

                List<string> pages = signalDict[signal];
                foreach(string page in pages) {
                    if(references.Length > 72) {
                        messages.Add(references);
                        references = "    ";
                    }
                    if(references.Length > 4) {
                        references += ", ";
                    }
                    references += page + side;
                }
            }

            if(references.Length > 4) {
                messages.Add(references);
            }

            //  Display the results

            if (messages.Count < 3) {
                MessageBox.Show("No Signals found.", "No Signals Found",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else {
                string log = "";
                foreach (string message in messages) {
                    log += message + Environment.NewLine;
                }
                Form LogDisplayDialog =
                    new ImporterLogDisplayForm("Messages", log);
                LogDisplayDialog.ShowDialog();
            }
        }

        private void machineComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            currentMachine = (Machine)machineComboBox.SelectedItem;
        }
    }
}
