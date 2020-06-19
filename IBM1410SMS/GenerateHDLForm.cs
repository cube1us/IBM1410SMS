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

namespace IBM1410SMS
{
    public partial class GenerateHDLForm : Form
    {

        DBSetup db = DBSetup.Instance;

        Table<Machine> machineTable;
        Table<Volumeset> volumeSetTable;
        Table<Volume> volumeTable;
        Table<Page> pageTable;

        List<Machine> machineList;
        List<Volumeset> volumeSetList;
        List<Volume> volumeList;

        Machine currentMachine = null;
        Volumeset currentVolumeSet = null;
        Volume currentVolume = null;

        bool populatingDialog = true;

        public GenerateHDLForm() {
            InitializeComponent();

            machineTable = db.getMachineTable();
            volumeSetTable = db.getVolumeSetTable();
            volumeTable = db.getVolumeTable();
            pageTable = db.getPageTable();

            machineList = machineTable.getAll();

            //  Fill in the machine combo box, and remember which machine
            //  we started out with.

            machineComboBox.DataSource = machineList;

            string lastMachine = Parms.getParmValue("machine");
            if(lastMachine.Length != 0) {
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
            if(lastVolumeSet.Length > 0) {
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

            //  Populate the Test Bench check box

            string testBench = Parms.getParmValue("generate page test bench");
            testBenchCheckBox.Checked = (testBench == "Y");

            populatingDialog = false;
        }

        //  Check if we should enable the generate button...

        private void checkEnableGenerateButton() {
            if(directoryTextBox.Text.Length > 0 && pagesTextBox.Text.Length > 0) {
                generateButton.Enabled = true;
            }
            else {
                generateButton.Enabled = false;
            }
        }


        private void generateButton_Click(object sender, EventArgs e) {

            int generated = 0;
            string testBench = "N";

            List<string> pagePatterns = pagesTextBox.Text.Split(
                new char[] { ',', ' ' },
                StringSplitOptions.RemoveEmptyEntries).ToList();

            //  Save the existing parameters.

            Parms.setParmValue("machine", currentMachine.idMachine.ToString());
            Parms.setParmValue("volume set", currentVolumeSet.idVolumeSet.ToString());
            Parms.setParmValue("volume", currentVolume.idVolume.ToString());
            Parms.setParmValue("generate output directory", directoryTextBox.Text);

            testBench = testBenchCheckBox.Checked ? "Y" : "N";
            Parms.setParmValue("generate page test bench", testBench);

            //  And then proceed.

            foreach(string pagePattern in pagePatterns) {
                List<Page> pages = pageTable.getWhere(
                    "WHERE machine='" + currentMachine.idMachine + "'" +
                    " AND volume='" + currentVolume.idVolume + "'" +
                    " AND page.name LIKE '" + pagePattern + "'" +
                    " ORDER by page.name");

                foreach (Page page in pages) {

                    ++generated;
                    GenerateHDL gen = new GenerateHDL(page, directoryTextBox.Text,
                        testBenchCheckBox.Checked);
                    int errors = gen.generateHDL();

                    Form ImporterLogDisplayDialog =
                        new ImporterLogDisplayForm(gen.getLogfileName());

                    if (errors > 0) {

                        //  Still thinking about this.  If this is just Show()
                        //  then if user clicks OK, it gets closed with no
                        //  chance to scroll because MessageBox takes over.

                        ImporterLogDisplayDialog.ShowDialog();

                        DialogResult status = MessageBox.Show(
                            "One or more errors occured on the page " +
                            page.name + Environment.NewLine +
                            "Do you wish to continue?", "Errors Found.",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                        if (status == DialogResult.Cancel) {
                            break;
                        }

                        // ImporterLogDisplayDialog.Close();
                    }
                    else {                        
                        ImporterLogDisplayDialog.ShowDialog();
                    }
                }
            }

            if(generated == 0) {
                MessageBox.Show("No matching pages were found to generate.",
                    "Nothing to generate", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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
                    if(lastVolume.Length > 0) {
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
                if(currentVolume != null) {

                    //  For reasons still a mystery to me, I was not sucessful with the
                    //  more obvious predicate of x => x.idVolume == currentVolume.idVolume.
                    //  It never got any its.  ???

                    volumeComboBox.SelectedIndex = 
                        volumeList.FindIndex(x => x.idVolume.ToString() == lastVolume);
                }
            }
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

            if(volumeComboBox.SelectedIndex >= 0) {
                currentVolume = volumeList[volumeComboBox.SelectedIndex];
            }

        }

        private void selectFolderButton_Click(object sender, EventArgs e) {

            FolderBrowserDialog folderBrowserDialog1 =
                new FolderBrowserDialog();

            folderBrowserDialog1.Description =
                "Identify the directory to use for the HDL output files: ";

            DialogResult result = folderBrowserDialog1.ShowDialog();
            if(result == DialogResult.OK) {
                directoryTextBox.Text = folderBrowserDialog1.SelectedPath;
                checkEnableGenerateButton();                
            }
        }

        private void pagesTextBox_TextChanged(object sender, EventArgs e) {
            checkEnableGenerateButton();
        }

        internal void displayLog() {
        }

    }
}
