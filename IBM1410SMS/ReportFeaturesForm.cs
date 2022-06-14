/* 
 *  COPYRIGHT 2018, 2019, 2020, 2021, 2022 Jay R. Jaeger
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

using MySQLFramework;
using System.IO;
using System.Collections;

namespace IBM1410SMS
{
    public partial class ReportFeaturesForm : Form
    {

        //  Class to store information about a given feature's appearance on
        //  a given page.  (This report does not list the actual logic blocks
        //  themselves.)

        class FeatureDetail {
            public int feature = 0;
            public string pageName = "";
            public int count = 0;
        }

        DBSetup db = DBSetup.Instance;

        Table<Machine> machineTable;
        Table<Diagrampage> diagramPageTable;
        Table<Page> pageTable;
        Table<Diagramblock> diagramBlockTable;
        Table<Feature> featureTable;

        List<FeatureDetail> featureDetailList;
        List<Machine> machineList;
        List<Diagrampage> diagramPageList;

        string logFileName = "";
        StreamWriter logFile = null;

        Machine currentMachine = null;

        public ReportFeaturesForm() {
            InitializeComponent();

            machineTable = db.getMachineTable();
            diagramPageTable = db.getDiagramPageTable();
            pageTable = db.getPageTable();
            diagramBlockTable = db.getDiagramBlockTable();
            featureTable = db.getFeatureTable();

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

            //  Also fill in the Report directory text box

            directoryTextBox.Text = Parms.getParmValue("report output directory");

            //  Disable the report button for now.

            reportButton.Enabled = directoryTextBox.Text.Length > 0;

        }

        private void directoryButton_Click(object sender, EventArgs e) {
            FolderBrowserDialog folderBrowserDialog1 =
                new FolderBrowserDialog();

            folderBrowserDialog1.Description =
                "Identify the directory to use for the report: ";

            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK) {
                directoryTextBox.Text = folderBrowserDialog1.SelectedPath;
                reportButton.Enabled = (directoryTextBox.Text.Length > 0);
            }
        }

        private void reportButton_Click(object sender, EventArgs e) {
            logFileName = Path.Combine(directoryTextBox.Text,
                currentMachine.name + "-FeatureReport.txt");
            logFile = new StreamWriter(logFileName, false);

            Parms.setParmValue("report output directory", directoryTextBox.Text);

            logMessage("Feature Report for Machine: " +
                currentMachine.name);

            doFeaturesReport();

            //  Show a box, maybe, eventually.

            logFile.Close();

            reportButton.Enabled = true;

        }

        //  Write a message to the output

        private void logMessage(string message) {
            logFile.WriteLine(message);
            logFile.Flush();
        }

        private void machineComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            currentMachine = (Machine)machineComboBox.SelectedItem;
        }

        private void doFeaturesReport() {
            reportButton.Enabled = false;
            diagramPageList = new List<Diagrampage>();
            featureDetailList = new List<FeatureDetail>();

            //  Build a list of relevant diagram pages for this machine.

            List<Page> pageList = pageTable.getWhere("WHERE machine = '" +
                currentMachine.idMachine + "'");

            foreach (Page page in pageList) {
                List<Diagrampage> dpList = diagramPageTable.getWhere(
                    "WHERE page = '" + page.idPage + "'");
                if (dpList.Count <= 0) {
                    //  This might not be a diagram page
                    continue;
                }
                else if (dpList.Count > 1) {
                    logMessage("ERROR: More than one diagram page found for page " +
                        page.name + "(" + page.idPage.ToString() + ")");
                }
                else {
                    diagramPageList.Add(dpList[0]);
                }
            }

            //  Then build a list of features relevant to this machine

            List<Feature> featureList = featureTable.getWhere("WHERE machine = '" +
                currentMachine.idMachine + "'");

            foreach (Diagrampage diagrampage in diagramPageList) {

                string pageName = Helpers.getDiagramPageName(diagrampage.idDiagramPage);

                //  Build a list of logic blocks for this page where any feature is present.

                List<Diagramblock> blocks = diagramBlockTable.getWhere("Where diagrampage = '" + 
                    diagrampage.idDiagramPage.ToString() + "' and feature != 0");

                if(blocks.Count > 0) {
                    // logMessage("Page " + pageName + " has " + blocks.Count.ToString() + " blocks for feature(s)");
                }

                //  For each block, check if a FeatureDetail entry exists.  If it does,
                //  increment its count, otherwise create it with a coutn of 1.

                foreach(Diagramblock block in blocks) {
                    FeatureDetail detail = featureDetailList.Find(x => x.feature == block.feature &&
                        x.pageName == pageName);
                    if(detail != null) {
                        ++detail.count;
                    }
                    else {
                        detail = new FeatureDetail();
                        detail.count = 1;
                        detail.feature = block.feature;
                        detail.pageName = pageName;
                        featureDetailList.Add(detail);
                        // logMessage("Adding feature Detail for page " + pageName);
                    }                        
                }
            }

            //  Sort the list into page within feature

            featureDetailList.Sort(delegate (FeatureDetail x, FeatureDetail y) {
                if (x.feature > y.feature) return 1;
                else if (x.feature < y.feature) return -1;
                else return x.pageName.CompareTo(y.pageName);
            });

            //  Now do the actual report...

            int lastFeature = -1;
            string message = "";

            // logMessage("DEBUG: " + featureDetailList.Count.ToString() + " feature entries found");
            logMessage("");

            foreach(FeatureDetail detail in featureDetailList) {

                //  Next feature up...

                if(detail.feature != lastFeature) {
                    if(message.Length > 0) {
                        logMessage(message);
                    }
                    if(lastFeature > 0) {
                        logMessage("");
                    }
                    message = "";
                    lastFeature = detail.feature;
                    Feature feature = featureList.Find(x => x.idFeature == detail.feature);
                    logMessage("Pages for feature " + feature.code + " (" + feature.feature + ")");
                }

                if(message.Length > 60) {
                    message += ",";   //  Add comma to end of line
                    logMessage(message);
                    message = "    ";
                }
                else if (message.Length > 0) {
                    message += ", ";
                }
                else {
                    message = "    ";
                }

                message += detail.pageName + "(" + detail.count + ")";
            }

            //  Handle any straggler message...

            if(message.Length > 0) {
                logMessage(message);
            }
        }
    }
}
