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

using MySQLFramework;

//  Gah.  This class is 99% an identical copy to GeneraateHDLForm.   I was just too lazy to
//  refactor it, I guess.  8P

namespace IBM1410SMS
{
    public partial class GenerateGroupHDLForm : Form
    {

        DBSetup db = DBSetup.Instance;

        Table<Machine> machineTable;
        Table<Volumeset> volumeSetTable;
        Table<Volume> volumeTable;
        Table<Page> pageTable;
        Table<Diagrampage> diagramPageTable;


        List<Machine> machineList;
        List<Volumeset> volumeSetList;
        List<Volume> volumeList;

        Machine currentMachine = null;
        Volumeset currentVolumeSet = null;
        Volume currentVolume = null;

        bool populatingDialog = true;
        string outputFileName = "";

        const string GROUPPARMPREFIX = "GROUP-";

        public GenerateGroupHDLForm() {


            InitializeComponent();

            machineTable = db.getMachineTable();
            volumeSetTable = db.getVolumeSetTable();
            volumeTable = db.getVolumeTable();
            pageTable = db.getPageTable();
            diagramPageTable = db.getDiagramPageTable();

            machineList = machineTable.getAll();

            //  Fill in the machine combo box, and remember which machine
            //  we started out with.

            machineComboBox.DataSource = machineList;

            string lastMachine = Parms.getParmValue("machine");
            if (lastMachine.Length != 0) {
                currentMachine = machineList.Find(x => x.idMachine.ToString() == lastMachine);
            }

            if (currentMachine == null || currentMachine.idMachine == 0) {
                currentMachine = machineList[0];
            }
            else {
                machineComboBox.SelectedItem = currentMachine;
            }

            //  Same for the volume set list - which is not tied to machine

            volumeSetList = volumeSetTable.getAll();
            volumeSetComboBox.DataSource = volumeSetList;

            string lastVolumeSet = Parms.getParmValue("volume set");
            if (lastVolumeSet.Length > 0) {
                currentVolumeSet = volumeSetList.Find(x => x.idVolumeSet.ToString() ==
                    lastVolumeSet);
            }

            if (currentVolumeSet == null || currentVolumeSet.idVolumeSet == 0) {
                currentVolumeSet = volumeSetList[0];
            }
            else {
                volumeSetComboBox.SelectedItem = currentVolumeSet;
            }

            // Then populate the other combo boxes

            populateVolumeComboBox(currentVolumeSet, currentMachine);

            //  Populate the output directory button, if we have one in the parms

            directoryTextBox.Text = Parms.getParmValue("generate output directory");

            //  Populate the pattern of pages we last used.

            pagesTextBox.Text = Parms.getParmValue("generate group page patterns");

            //  Populate the test bench check boxes

            string testBench = Parms.getParmValue("generate group test bench");
            generateTestBenchCheckBox.Checked = (testBench == "Y");

            // testBench = Parms.getParmValue("generate group pages test bench");
            // generatePageTestBenchCheckBox.Checked = (testBench == "Y");

            populatingDialog = false;

        }

        //  Method to fill in the volume combo box

        private void populateVolumeComboBox(Volumeset volumeSet, Machine machine) {

            //  If there is no volume set then this combo box must be empty
            //  as well.

            string lastVolume = Parms.getParmValue("volume");
            currentVolume = null;

            if (volumeSet == null) {
                currentVolume = null;
                volumeComboBox.Items.Clear();
            }
            else {
                //  Set up the volume list combo box.
                volumeList = volumeTable.getWhere(
                    "WHERE volume.set='" + volumeSet.idVolumeSet + "' ORDER BY volume.order");
                if (volumeList.Count > 0) {
                    if (lastVolume.Length > 0) {
                        currentVolume = volumeList.Find(x => x.idVolume.ToString() == lastVolume);
                    }
                    if (currentVolume == null || currentVolume.idVolume == 0) {
                        currentVolume = volumeList[0];
                    }
                }
                else {
                    currentVolume = null;
                    volumeComboBox.Items.Clear();
                }
                volumeComboBox.DataSource = volumeList;
                if (currentVolume != null) {

                    //  For reasons still a mystery to me, I was not sucessful with the
                    //  more obvious predicate of x => x.idVolume == currentVolume.idVolume.
                    //  It never got any its.  ???

                    volumeComboBox.SelectedIndex =
                        volumeList.FindIndex(x => x.idVolume.ToString() == lastVolume);
                }
            }
        }

        //  Check if we should enable the generate button...

        private void checkEnableGenerateButton() {
            if (directoryTextBox.Text.Length > 0 && pagesTextBox.Text.Length > 0 &&
                outputFileName.Length > 0) {
                generateButton.Enabled = true;
            }
            else {
                generateButton.Enabled = false;
            }
        }

        private void generateButton_Click(object sender, EventArgs e) {

            List<string> pagePatterns = pagesTextBox.Text.Split(
                new char[] { ',', ' ' },
                StringSplitOptions.RemoveEmptyEntries).ToList();

            List<Diagrampage> diagramPageList = new List<Diagrampage>();

            List<Page> pageList = new List<Page>();
            List<string> pageNames = new List<string>();

            //  Save the existing parameters.

            Parms.setParmValue("machine", currentMachine.idMachine.ToString());
            Parms.setParmValue("volume set", currentVolumeSet.idVolumeSet.ToString());
            Parms.setParmValue("volume", currentVolume.idVolume.ToString());
            Parms.setParmValue("generate output directory", directoryTextBox.Text);
            Parms.setParmValue("generate group page patterns", pagesTextBox.Text);
            Parms.setParmValue("generate group test bench",
                generateTestBenchCheckBox.Checked ? "Y" : "N");
            Parms.setParmValue(GROUPPARMPREFIX + outputFileName.ToUpper(), pagesTextBox.Text);
            // Parms.setParmValue("generate group pages test bench",
            //    generatePageTestBenchCheckBox.Checked ? "Y" : "N");

            //  And then proceed.

            foreach (string pagePattern in pagePatterns) {
                List<Page> pages = pageTable.getWhere(
                    "WHERE machine='" + currentMachine.idMachine + "'" +
                    " AND volume='" + currentVolume.idVolume + "'" +
                    " AND page.name LIKE '" + pagePattern + "'" + 
                    " ORDER BY page.name");

                if(pages.Count == 0) {
                    MessageBox.Show("No pages were found matching pattern " + pagePattern);
                }

                foreach(Page page in pages) {
                    if(pageList.FindIndex(x => x.idPage == page.idPage) < 0) {
                        pageList.Add(page);
                    }
                    pageNames.Add(page.name);

                    List<Diagrampage> diagramPages = diagramPageTable.getWhere(
                        "WHERE diagrampage.page='" + page.idPage + "'");
                    if(diagramPages.Count != 1) {
                        MessageBox.Show("Warning:  " +
                            (diagramPages.Count == 0 ? "No " : "more than 1 ") +
                            "Diagram page(s) found for page name " + page.name);
                    }
                    diagramPageList.AddRange(diagramPages);
                }
            }

            if(pageList.Count == 0) {
                MessageBox.Show("No pages were found for ANY of the page patterns.");
                return;
            }

            Console.WriteLine("Pages: " + pageNames);

            DialogResult result = MessageBox.Show(
                "A Group Page will be generated to instantiate the following pages: " + 
                Environment.NewLine + 
                string.Join(",",pageNames), "Group Generation Pages",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            if(result == DialogResult.Cancel) {
                return;
            }

            GenerateGroupHDL gen = new GenerateGroupHDL(
                currentMachine, diagramPageList, pageNames, outputFileNameTextBox.Text,
                directoryTextBox.Text, generateTestBenchCheckBox.Checked);

            int errors = gen.generateGroupHDL();

            if(errors > 0) {
                MessageBox.Show("One or more errors occured during group generation.",
                    "Errors Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            Form ImporterLogDisplayDialog =
                new ImporterLogDisplayForm(gen.getLogfileName());
            ImporterLogDisplayDialog.Show();

        }

        private void machineComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            currentMachine = machineList[machineComboBox.SelectedIndex];

            //  Repopulate the other affected combo boxes.

            if (!populatingDialog) {
                populateVolumeComboBox(currentVolumeSet, currentMachine);
            }
        }

        private void volumeSetComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            currentVolumeSet = volumeSetList[volumeSetComboBox.SelectedIndex];

            //  Repopulate the other affected combo boxes.

            if (!populatingDialog) {
                populateVolumeComboBox(currentVolumeSet, currentMachine);
            }

        }

        private void volumeComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            currentVolume = volumeList[volumeComboBox.SelectedIndex];

        }

        private void selectFolderButton_Click(object sender, EventArgs e) {

            FolderBrowserDialog folderBrowserDialog1 =
                new FolderBrowserDialog();

            folderBrowserDialog1.Description =
                "Identify the directory to use for the HDL output files: ";

            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK) {
                directoryTextBox.Text = folderBrowserDialog1.SelectedPath;
                checkEnableGenerateButton();
            }
        }

        private void pagesTextBox_TextChanged(object sender, EventArgs e) {
            checkEnableGenerateButton();
        }

        internal void displayLog() {
        }

        private void outputFileName_TextChanged(object sender, EventArgs e) {
            if(outputFileNameTextBox.Text.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1) {
                MessageBox.Show("The specified output file name is invalid.",
                    "Invalid File Name",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                outputFileName = "";
                generateButton.Enabled = false;
            }
            else {
                outputFileName = outputFileNameTextBox.Text;
                checkEnableGenerateButton();
            }

            //  Recall the set of page names last used to generate this group, if any

            string groupPageNames = Parms.getParmValue(GROUPPARMPREFIX +
                outputFileName.ToUpper());
            if(groupPageNames.Length > 0) {
                pagesTextBox.Text = groupPageNames;
            }
        }
    }
}
