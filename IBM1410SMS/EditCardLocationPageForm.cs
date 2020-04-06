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
    public partial class EditCardLocationPageForm : Form
    {

        DBSetup db = DBSetup.Instance;

        Table<Machine> machineTable;
        Table<Volumeset> volumeSetTable;
        Table<Volume> volumeTable;
        Table<Panel> panelTable;
        Table<Eco> ecoTable;
        Table<Page> pageTable;
        Table<Cardlocation> cardLocationTable;
        Table<Cardlocationpage> cardLocationPageTable;
        Table<Diagrampage> diagramPageTable;
        Table<Cardlocationbottomnote> cardLocationBottomNoteTable;
        Table<Cardlocationblock> cardLocationBlockTable;
        Table<Cableedgeconnectionpage> cableEdgeConnectionPageTable;

        List<Machine> machineList;
        List<Volumeset> volumeSetList;
        List<Volume> volumeList;
        List<Eco> ecoList;
        List<Page> pageList;
        List<Panel> panelList;

        Machine currentMachine = null;
        Volumeset currentVolumeSet = null;
        Volume currentVolume = null;
        Panel currentPanel = null;
        Page currentPage = null;
        Cardlocationpage currentCardLocationPage = null;

        bool pageModified = false;
        bool populatingDialog = false;

        string panelLabel = "Panel";
        string frameLabel = "Frame";
        string gateLabel = "Gate";

        public EditCardLocationPageForm() {
            InitializeComponent();

            machineTable = db.getMachineTable();
            volumeSetTable = db.getVolumeSetTable();
            volumeTable = db.getVolumeTable();
            pageTable = db.getPageTable();
            ecoTable = db.getEcoTable();
            panelTable = db.getPanelTable();
            cardLocationPageTable = db.getCardLocationPageTable();
            cardLocationTable = db.getCardLocationTable();
            diagramPageTable = db.getDiagramPageTable();
            cardLocationBottomNoteTable = db.getCardLocationBottomNoteTable();
            cardLocationBlockTable = db.getCardLocationBlockTable();
            cableEdgeConnectionPageTable = db.getCableEdgeConnectionPageTable();

            //  Populate the (static) Sheets combo box...

            for(int i=1; i <= 4; ++i) {
                sheetsComboBox.Items.Add(i);
            }

            machineList = machineTable.getAll();

            //  Fill in the machine combo box, and remember which machine
            //  we started out with.

            machineComboBox.DataSource = machineList;

            //  Retrieve the last machine we worked with, and select it, if any.

            string lastMachine = Parms.getParmValue("machine");
            if (lastMachine.Length > 0) {
                currentMachine = machineList.Find(
                    x => x.idMachine.ToString() == lastMachine);
            }

            if (currentMachine == null || currentMachine.idMachine == 0) {
                currentMachine = machineList[0];
            }

            machineComboBox.SelectedItem = currentMachine;

            //  Same for the volume set list - which is not tied to machine

            volumeSetList = volumeSetTable.getAll();

            volumeSetComboBox.DataSource = volumeSetList;

            string lastVolumeSet = Parms.getParmValue("volume set");
            if (lastVolumeSet.Length > 0) {
                currentVolumeSet = volumeSetList.Find(
                    x => x.idVolumeSet.ToString() == lastVolumeSet);
            }

            if (currentVolumeSet == null || currentVolumeSet.idVolumeSet == 0) {
                currentVolumeSet = volumeSetList[0];
            }

            volumeSetComboBox.SelectedItem = currentVolumeSet;

            // Then populate the other combo boxes

            populateEcoComboBoxes(currentMachine);
            populateVolumeComboBox(currentVolumeSet,currentMachine);
            populatePanelComboBox(currentMachine);
        }

        //  Method to fill in the volume combo box

        private void populateVolumeComboBox(Volumeset volumeSet, Machine machine) {

            currentVolume = null;
            
            //  If there is no volume set then this combo box must be empty
            //  as well.

            if (volumeSet == null) {
                volumeComboBox.Items.Clear();
            }
            else {
                //  Set up the volume list combo box.
                volumeList = volumeTable.getWhere(
                    "WHERE volume.set='" + volumeSet.idVolumeSet + "' ORDER BY volume.order");
                if (volumeList.Count > 0) {
                    currentVolume = volumeList[0];
                }
                else {
                    currentVolume = null;
                    volumeComboBox.Items.Clear();
                }

                volumeComboBox.DataSource = volumeList;

                //  Retrieve the last volume we worked with, and select it, if
                //  it is relevant.

                string lastVolume = Parms.getParmValue("volume");
                if(lastVolume.Length > 0) {
                    currentVolume = volumeList.Find(x => x.idVolume.ToString() == lastVolume);
                }
                if(currentVolume == null || currentVolume.idVolume == 0) {
                    currentVolume = volumeList[0];
                }
                volumeComboBox.SelectedItem = currentVolume;

            }

            //  Even if there are no volumes, populate the page combo box.
            //  It will know to create an empty page combo box...

            populatePageComboBox(machine, currentVolume);
        }

        //  Method to fill in the Page combo box.

        private void populatePageComboBox(Machine machine, Volume volume) {

            currentPage = null;

            //  If there is no machine or no volume, then this combo box
            //  has to be left empty.

            if(machine == null || volume == null) {
                pageList = new List<Page>();
                currentCardLocationPage = null;
                pageComboBox.Items.Clear();
                return;
            }

            //  Get the (potential) list of pages for this machine and volume

            pageList = pageTable.getWhere(
                "WHERE machine='" + machine.idMachine + "' AND volume='" +
                volume.idVolume + "' ORDER BY name");

            //  But not all of those are card location pages.  Some may
            //  be diagram pages or cable/edge pages.  Remove those from the list...

            //  (NOTE:  Pages which are NOT card location pages nor currently
            //  spoken for as diagram pages or cable/edge connection pages
            //  remain in the list - they may become diagram pages via this form).

            //  Also, while we are at it, if we find a page that matches the last
            //  page we used, select it.

            List<Page> pagesToRemoveList = new List<Page>();

            foreach(Page p in pageList) {
                List<Diagrampage> diagramPageList = diagramPageTable.getWhere(
                    "WHERE diagrampage.page='" + p.idPage + "'");
                if(diagramPageList.Count > 0) {
                    pagesToRemoveList.Add(p);
                }
                List<Cableedgeconnectionpage> cableEdgeConnectionPageList = 
                    cableEdgeConnectionPageTable.getWhere(
                        "WHERE cableedgeconnectionpage.page='" + p.idPage + "'");
                if (cableEdgeConnectionPageList.Count > 0) {
                    pagesToRemoveList.Add(p);
                }
            }

            foreach (Page p in pagesToRemoveList) {
                pageList.Remove(p);
            }

            pageComboBox.DataSource = pageList;

            if (pageList.Count > 0) {
                currentPage = pageList[0];
                //  If the list is not empty, set the current page (and the
                //  dialog, later) to the one we last used, or, if there is no match,
                //  to the first page in the list.

                string lastPage = Parms.getParmValue("page");
                if (lastPage.Length > 0) {
                    currentPage = pageList.Find(x => x.idPage.ToString() == lastPage);
                }
                if (currentPage == null || currentPage.idPage == 0) {
                    currentPage = pageList[0];
                }
                pageComboBox.SelectedItem = currentPage;
            }
            else {
                //  Otherwise clear the dialog.
                currentPage = null;
                currentCardLocationPage = null;
                pageComboBox.SelectedIndex = -1;
            }

            //  Sheets defaults to 1.

            sheetsComboBox.SelectedItem = 1;

            populateDialog(currentPage);
        }

        //  Method to set up the panel combo box.

        private void populatePanelComboBox(Machine machine) {

            //  Start with an empty panel list, and an empty string list to
            //  go with it.

            panelList = new List<Panel>();
            panelComboBox.Items.Clear();
            currentPanel = null;
            int selectedPanel = 0;


            Table<Frame> frameTable = db.getFrameTable();
            Table<Machinegate> machineGateTable = db.getMachineGateTable();

            //  Iterate through all of the Frames, (machine) gates and panels
            //  building the combo box as text, and keep a list of panel
            //  entities that match to use later.

            //  While we are at it, see if any of them match the last panel we
            //  we used, and if so, select it.

            //  This could have been a join -- I *know*.

            string lastPanel = Parms.getParmValue("panel");

            int index = 0;
            foreach(Frame frame in
                frameTable.getWhere("" + "WHERE machine='" + 
                    machine.idMachine + "' ORDER BY frame.name")) {
                foreach(Machinegate gate in
                    machineGateTable.getWhere("WHERE frame='" + 
                        frame.idFrame + "' ORDER BY machinegate.name")) {
                    foreach(Panel panel in panelTable.getWhere(
                        "WHERE gate='" + gate.idGate + "' ORDER BY panel")) {
                        panelList.Add(panel);
                        panelComboBox.Items.Add(frameLabel + ": " + frame.name + " " +
                            gateLabel + ": " + gate.name + " " + panelLabel + ": " + panel.panel);
                        if(panel.idPanel.ToString() == lastPanel) {
                            selectedPanel = index;
                        }
                        ++index;
                    }
                }
            }

            //  If the list is not empty, set the combo box to the first
            //  panel in the list.

            if(panelList.Count > 0) {
                panelComboBox.SelectedIndex = selectedPanel;
                currentPanel = panelList[selectedPanel];
            }

            //  Sheets defaults to 1.

            sheetsComboBox.SelectedItem = 1;
        }

        //  Method to populate the two ECO combo boxes.

        private void populateEcoComboBoxes(Machine machine) {

            //  Get the list of ECOs for this machine...

            ecoList = ecoTable.getWhere("WHERE machine='" +
                machine.idMachine + "'");
            ecoComboBox.DataSource = ecoList;

            //  Give the previous ECO combo box a separate binding context, 
            //  so that changeing ECO does not change PreviousECO and vice-versa.

            previousEcoComboBox.BindingContext = new BindingContext();
            previousEcoComboBox.DataSource = ecoList;
            ecoComboBox.SelectedIndex = -1;
            previousEcoComboBox.SelectedIndex = -1;
        }

        //  Method to populate the page data in the dialog.

        private void populateDialog(Page page) {

            //  First clear everything out.

            nameTextBox.Clear();
            partTextBox.Clear();
            titleTextBox.Clear();
            stampTextBox.Clear();
            ecoComboBox.SelectedIndex = -1;
            previousEcoComboBox.SelectedIndex = -1;
            runTextBox.Clear();
            sheetsComboBox.SelectedItem = 1;
            pageModified = false;

            //  If the page is null, enter "add" mode, and return.
            //  Otherwise, we are in update/remove mode.

            if (page == null) {
                removeButton.Visible = false;
                addApplyButton.Text = "Add";
                currentCardLocationPage = null;
                return;
            }
            else {
                removeButton.Visible = true;
                addApplyButton.Text = "Apply";
            }
            
            //  See if there is a matching cardLocationPage to this page yet...
            //  If not, then clear any existing card location page entity object.
            //  And more than one is a database integrity problem...

            List<Cardlocationpage> cardLocationPageList = 
                cardLocationPageTable.getWhere("WHERE cardlocationpage.page='" + 
                page.idPage + "'");
            if(cardLocationPageList.Count == 0) {
                currentCardLocationPage = null;
            }
            else if(cardLocationPageList.Count > 1) {
                MessageBox.Show("ERROR: There are more than one cardLocationPage " +
                    "rows corresponding to Page " + page.name + " (Database ID " +
                    page.idPage + ") For machine " + currentMachine.name +
                    " in Volume " + currentVolume.name,
                    "Multiple Card Location Pages Found",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                currentCardLocationPage = null;
            }
            else {
                //  There is exactly one, so set the current page to that one.
                currentCardLocationPage = cardLocationPageList[0];
            }

            populatingDialog = true;

            //  Now populate the page data, and, if available, the 
            //  Card Location Page data

            nameTextBox.Text = page.name;
            partTextBox.Text = page.part;
            titleTextBox.Text = page.title;
            stampTextBox.Text = page.stamp;

            //  If there is a current Card Location Page row, then fill in the rest.

            if(currentCardLocationPage != null) {

                //  If there is a panel, set that as the current panel in the
                //  combo box.

                if(currentCardLocationPage.panel != 0) {
                    int panelIndex = panelList.FindIndex(x => x.idPanel ==
                    currentCardLocationPage.panel);
                    if(panelIndex >= 0) {
                        panelComboBox.SelectedIndex = panelIndex;
                    }
                }

                //  For ECO, we have to find the matching entry in our eco list
                //  for this machine.  Also applies to previous ECO.

                if (currentCardLocationPage.eco != 0) {
                    Eco eco = ecoTable.getByKey(currentCardLocationPage.eco);

                    //  Did we find a match on ECO?  If so, select that entry.

                    if (eco.idECO == currentCardLocationPage.eco) {
                        //  Set the selected item.  But we must use the original object,
                        //  not the new one we just obtained.
                        ecoComboBox.SelectedItem =
                            ecoList.Find(x => x.idECO == eco.idECO);
                    }
                    else {

                        //  This card location page has an ECO.  Unfortunatetly,
                        //  it isn't in the ECO database!  Integrity error.
                        //  Issue a warning, but allow user to proceed, so that
                        //  they can fix it.

                        MessageBox.Show("WARNING: Card Location Page " + page.name +
                            " (Database ID " +
                            currentCardLocationPage.idCardLocationPage +
                            ") For machine " + currentMachine.name +
                            ", " + panelLabel + (currentPanel != null ? currentPanel.panel : "") +
                            ",Volume " + currentVolume.name +
                            " currently has an ECO database ID " +
                            currentCardLocationPage.eco +
                            " which was not found in the database.",
                            "Unknown ECO database ID ",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ecoComboBox.SelectedIndex = -1;
                    }
                }
                else {

                    //  No ECO, so leave the combo box blank at first.

                    ecoComboBox.SelectedIndex = -1;
                }

                //  Now do the same for previous ECO...

                if (currentCardLocationPage.previousECO != 0) {
                    Eco eco = ecoTable.getByKey(currentCardLocationPage.previousECO);

                    if (eco.idECO == currentCardLocationPage.previousECO) {
                        previousEcoComboBox.SelectedItem =
                            ecoList.Find(x => x.idECO == eco.idECO);
                    }
                    else {
                        MessageBox.Show("WARNING: Card Location Page " + page.name +
                            " (Database ID " +
                            currentCardLocationPage.idCardLocationPage +
                            ") For machine " + currentMachine.name +
                            "," + panelLabel + (currentPanel != null ? currentPanel.panel : "") +
                            ",Volume " + currentVolume.name +
                            " currently has a Previous ECO, database ID " +
                            currentCardLocationPage.previousECO +
                            " which was not found in the database.",
                            "Unknown ECO database ID ",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        previousEcoComboBox.SelectedIndex = -1;
                    }
                }
                else {
                    previousEcoComboBox.SelectedItem = -1;
                }

                //  Set the run field, if present.  Otherwise leave it blank.

                if (currentCardLocationPage.run > 0) {
                    runTextBox.Text = currentCardLocationPage.run.ToString();
                }

                //  Show the current number of sheets, if present.

                if(currentCardLocationPage.sheets > 0 &&
                   currentCardLocationPage.sheets <= sheetsComboBox.Items.Count) {
                    sheetsComboBox.SelectedItem = currentCardLocationPage.sheets;
                }

                //  Show the current panel, too.  In this case, the combo box
                //  is constructed strings, so we need to find the index, and
                //  use that to select the correct item.

                if (currentCardLocationPage.panel != 0) {
                    panelComboBox.SelectedIndex = panelList.FindIndex(
                        x => x.idPanel == currentCardLocationPage.panel);
                }

                populatingDialog = false;

            }
        }

        //  Method to handle user changing machines (which changes everything)

        private void machineComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            //  If there are modifications on the current page, confirm
            //  user wants to discard them...

            if(checkForModifications() == DialogResult.Cancel) {
                return;
            }

            currentMachine = machineList[machineComboBox.SelectedIndex];

            frameLabel = currentMachine.frameLabel;
            gateLabel = currentMachine.gateLabel;
            if(gateLabel != null && gateLabel.Equals(frameLabel)) {
                gateLabel = gateLabel + "(Gate)";
            }
            panelLabel = currentMachine.panelLabel;
            selectPanelLabel.Text = panelLabel + ":";

            //  Repopulate the other affected combo boxes.

            if (!populatingDialog) {
                populateVolumeComboBox(currentVolumeSet, currentMachine);
                populateEcoComboBoxes(currentMachine);
                populatePanelComboBox(currentMachine);
            }
        }

        //  Method to handle user changing volume sets, which also changes
        //  the available volumes, which in turn changes available pages.

        private void volumeSetComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            //  Check if there are modifications, and if so, if the user wants
            //  to discard them.

            if(checkForModifications() == DialogResult.Cancel) {
                return;
            }

            currentVolumeSet = volumeSetList[volumeSetComboBox.SelectedIndex];

            //  Repopulate the other affected combo boxes.


            if (!populatingDialog) {
                populateVolumeComboBox(currentVolumeSet, currentMachine);
            }
        }

        //  Method to handle the user changing volumes, which changes what 
        //  pages are available as well.

        private void volumeComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            //  Check if there are modifications, and if so, if the user wants
            //  to discard them.

            if (checkForModifications() == DialogResult.Cancel) {
                return;
            }

            currentVolume = volumeList[volumeComboBox.SelectedIndex];
            if (!populatingDialog) {
                populatePageComboBox(currentMachine, currentVolume);
            }
        }


        //  Page selection has changed...

        private void pageComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            if(pageComboBox.SelectedIndex < 0) {
                return;
            }

            //  If there is a current page, and if there are modifications, 
            //  confirm that the user wishes to discard them...

            if(currentPage != null && 
                checkForModifications() == DialogResult.Cancel) {
                return;
            }

            currentPage = pageList[pageComboBox.SelectedIndex];

            if (!populatingDialog) {
                populateDialog(currentPage);
            }
        }

        //  User wishes to delete a page...

        private void removeButton_Click(object sender, EventArgs e) {

            string message = "";

            if(currentPage == null) {
                throw new Exception("CardLocationPage remove button click: " +
                    "currentPage is null");
            }

            //  Check to make sure no card locations refer to this (card location) page...

            if(currentPage.idPage > 0 && currentCardLocationPage != null &&
                cardLocationTable.getWhere(
                    "WHERE cardlocation.page='" + currentCardLocationPage.idCardLocationPage +
                     "'").Count > 0) {
                MessageBox.Show(
                    "Page Remove: There are still one or more Card Locations " +
                    "which refer to this page.",
                    "Locations Refer to this page",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //  If there is a current Card location page, check that its
            //  database ID matches.  If not, it is a database integrity
            //  error, and in this case, we do NOT want to proceed.

            if (currentCardLocationPage != null) {
                if (currentCardLocationPage.page != currentPage.idPage) {
                    MessageBox.Show("CardLocationPage Remove, " +
                        "Current Card Location Page page database ID " +
                        currentCardLocationPage.page + " does not match page " +
                        "database ID " + currentPage.idPage,
                        "Mismatching Page Database IDs",
                        MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }

                message = "Remove Card Location Page (Database ID " +
                    currentCardLocationPage.idCardLocationPage + ") and \n";
            }

            //  Issue a confirmation message.

            message += "Remove Page " + currentPage.name +
                " (Database ID " + currentPage.idPage + ")" +
                "\nin volume " + currentVolume.idVolume +
                " for machine " + currentMachine.name;

            DialogResult status = MessageBox.Show(
                "Please confirm that you wish to:\n" + message,
                "Confirm Page/Card Location Page Removal",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if(status == DialogResult.Cancel) {
                return;
            }
            else if(status == DialogResult.OK) {

                //  OK, then.  We'll delete the card location page and the
                //  page entries.

                if(currentCardLocationPage != null) {
                    cardLocationPageTable.deleteByKey(
                        currentCardLocationPage.idCardLocationPage);
                }
                pageTable.deleteByKey(currentPage.idPage);
            }

            //  Results speak for themselves.  ;)

            MessageBox.Show("Page(s) removed:\n" + message,
                "Pages Removed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            currentPage = null;
            currentCardLocationPage = null;

            //  Repopulate the page combo box and the current page dialog
            //  This will select the first entry in the remaining list for the combo box

            populatePageComboBox(currentMachine, currentVolume);

            //  Then reset the form.

            //  currentPage = null;
            //  currentCardLocationPage = null;
            populateDialog(currentPage);
            pageModified = false;

        }

        //  Shared method to check for changes, warn the user, and return
        //  whether they want to discard the changes or not.

        private DialogResult checkForModifications() {
            if (pageModified == true) {
                DialogResult status = MessageBox.Show("Current Page had changes. " +
                    "Are you sure you wish " +
                    "to discard them?", "Discard Page Changes?",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return status;
            }

            pageModified = false;
            return DialogResult.OK;
        }

        //  If the user clicks cancel, end the dialog.

        private void cancelButton_Click(object sender, EventArgs e) {
            this.Close();
        }

        //  This method does the "heavy lifting" of an add or update.

        private void addApplyButton_Click(object sender, EventArgs e) {

            Eco eco = null;
            Eco previousEco = null;
            string message = "";
            int run = -1;
            bool changePageComboBox = false;
            bool refreshECOComboBox = false;

            //  Make sure required fields are filled in / selected, 
            //  regardless of whether this is an add or an update.

            if (nameTextBox.Text.Length == 0 ||
                partTextBox.Text.Length == 0 ||
                titleTextBox.Text.Length == 0 ||
                currentVolume == null) {
                MessageBox.Show("Error:  Volume, Name, Part and Title are required.",
                    "Volume, Name, Part and Title Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //  Also make sure a panel and ECO are selected.

            if (panelComboBox.Text.Length == 0 || ecoComboBox.Text.Length == 0) {
                MessageBox.Show("Error: " + panelLabel + " and ECO are required.",
                    panelLabel + " and ECO Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //  This button may be for an add, or applying updates,
            //  depending upon the context.  Make sure that we don't
            //  have a screwy logic error, and throw an exception if
            //  we do.

            if(currentPage == null || currentPage.idPage == 0) {

                //  If the current page is null or new, then we should be in Add mode.

                if(addApplyButton.Text.CompareTo("Add") != 0) {
                    throw new Exception("Card Location Page add/Apply Button Click: " +
                        "currentPage is null or ID is 0.  Button text expected to be " +
                        "Add, but button text is " +
                        addApplyButton.Text);
                }

                //  If the page is new, but not in range for this volume, warn the user.

                if (nameTextBox.Text.CompareTo(currentVolume.firstPage) < 0 ||
                    nameTextBox.Text.CompareTo(currentVolume.lastPage) > 0) {
                    DialogResult result = MessageBox.Show(
                        "Warning:  New page name is not in range for this volume.",
                        "Page name range warning", MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning);
                    if (result == DialogResult.Cancel) {
                        return;
                    }
                }

                if (currentPage == null) {
                    currentPage = new Page();

                    //  Set name to blank so that the compare we do later will
                    //  not blow up, and will also not match...
                    currentPage.name = "";

                }
            }
            else if(addApplyButton.Text.CompareTo("Apply") == 0) {
                if(currentPage == null || currentPage.idPage == 0) {
                    //  Something is rotten in Denmark.
                    throw new Exception("Card Location Page add/Apply Button Click: " +
                        "currentPage is null or ID is 0 and button text is " +
                        addApplyButton.Text);
                }
            }

            //  Validation passed, so now on to work, inside out / bottom up

            if(currentCardLocationPage == null) {
                currentCardLocationPage = new Cardlocationpage();
            }

            message = "";

            db.BeginTransaction();

            //  First work on the ECO and previous ECO, which may need to be
            //  added to the database.  (We do NOT remove any - there may be
            //  other references.  Leave that to the EditECOsForm.

            if(ecoComboBox.SelectedIndex >= 0) {
                currentCardLocationPage.eco = ecoList[ecoComboBox.SelectedIndex].idECO;
            }
            else if(ecoComboBox.Text.Length > 0) {
                //  New ECO
                eco = new Eco();
                eco.idECO = IdCounter.incrementCounter();
                eco.description = "";
                eco.machine = currentMachine.idMachine;
                eco.modified = true;
                eco.eco = ecoComboBox.Text;
                ecoTable.insert(eco);
                currentCardLocationPage.eco = eco.idECO;
                ecoList.Add(eco);
                message += "ECO " + eco.eco + " (Database ID " + eco.idECO +
                    " ) added.\n";
                refreshECOComboBox = true;
            }

            //  Then something similar for the previous ECO, but this one isn't
            //  always required.

            if(previousEcoComboBox.SelectedIndex >= 0) {
                currentCardLocationPage.previousECO =
                    ecoList[previousEcoComboBox.SelectedIndex].idECO;
            }
            else if(previousEcoComboBox.Text.Length > 0) {

                //  Check to see if it is the same as one we may have just added.
                if (previousEcoComboBox.Text.CompareTo(ecoComboBox.Text) == 0) {
                    currentCardLocationPage.previousECO = eco.idECO;
                }
                else {
                    //  Otherwise add it

                    previousEco = new Eco();
                    previousEco.idECO = IdCounter.incrementCounter();
                    previousEco.description = "";
                    previousEco.machine = currentMachine.idMachine;
                    previousEco.modified = true;
                    previousEco.eco = previousEcoComboBox.Text;
                    ecoTable.insert(previousEco);
                    currentCardLocationPage.previousECO = previousEco.idECO;
                    ecoList.Add(previousEco);
                    message += "ECO " + previousEco.eco + " (Database ID " +
                        previousEco.idECO + " ) added.\n";
                    refreshECOComboBox = true;
                }
            }
            else {
                //  Previous ECO removed...
                currentCardLocationPage.previousECO = 0;
            }

            //  Fill in the objects from the form data

            currentPage.machine = currentMachine.idMachine;
            currentPage.volume = currentVolume.idVolume;
            currentPage.part = Importer.zeroPadPartNumber(partTextBox.Text);
            currentPage.title = titleTextBox.Text.ToUpper();
            if (currentPage.name.CompareTo(nameTextBox.Text) != 0) {
                currentPage.name = nameTextBox.Text;
                changePageComboBox = true;
            }
            currentPage.stamp = stampTextBox.Text;

            currentCardLocationPage.page = currentPage.idPage;
            //  ECO/previous ECO were set earlier
            currentCardLocationPage.panel = currentPanel.idPanel;
            if (int.TryParse(runTextBox.Text, out run)) {
                currentCardLocationPage.run = run;
            }
            else {
                currentCardLocationPage.run = 0;
            }

            if (sheetsComboBox.SelectedIndex > 0) {
                currentCardLocationPage.sheets = (int)sheetsComboBox.SelectedItem;
            }
            else {
                currentCardLocationPage.sheets = 1;
            }

            //  Next, if we are adding a page, take care of that next, so we
            //  have a key to use for the card location page database row.

            if (currentPage.idPage == 0) {
                currentPage.idPage = IdCounter.incrementCounter();
                pageTable.insert(currentPage);
                currentPage.modified = false;
                message = "Page " + currentPage.name + " (Database ID " +
                    currentPage.idPage + " ) added.\n" + message;
                changePageComboBox = true;
            }
            else {
                currentPage.modified = true;
            }

            //  Next, if we are adding the card Location page, take care of that.

            if (currentCardLocationPage.idCardLocationPage == 0) {
                currentCardLocationPage.idCardLocationPage = 
                    IdCounter.incrementCounter();
                currentCardLocationPage.page = currentPage.idPage;
                cardLocationPageTable.insert(currentCardLocationPage);
                currentCardLocationPage.modified = false;
                message = "Card Location Page " + 
                    currentCardLocationPage.idCardLocationPage + " (Database ID " +
                    currentCardLocationPage.idCardLocationPage + 
                    " ) added.\n" + message;
            }
            else {
                currentCardLocationPage.modified = true;
            }

            //  If there are any modified flags, then we need to do updates.
            //  The data has already been filled in.

            if (currentCardLocationPage.modified) {
                cardLocationPageTable.update(currentCardLocationPage);
                currentCardLocationPage.modified = false;
                message = "Card Location Page " +
                    currentCardLocationPage.idCardLocationPage + " (Database ID " +
                    currentCardLocationPage.idCardLocationPage + 
                    " ) updated.\n" + message;
            }

            if(currentPage.modified) {
                pageTable.update(currentPage);
                currentPage.modified = false;
                message = "Page " + currentPage.name + " (Database ID " +
                    currentPage.idPage + " ) updated.\n" + message;

            }
            
            //  Save some of the dialog selections for later use in this and other
            //  dialogs.

            saveSettings();

            db.CommitTransaction();

            MessageBox.Show(message, "Add/Updates applied",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            //  If we added a new page or changed a page name,
            //  repopulate the page combo box.

            //  Also, in that case, arrange to have it selected as well.

            if(changePageComboBox) {

                //  First, we have to save the current page, because populatePageComboBox
                //  completely rebuilds the list, and sets currentPage to the first entry

                Page newPage = currentPage;
                populatePageComboBox(currentMachine, currentVolume);

                //  Then select the new page or updated page

                pageComboBox.SelectedItem = pageList.Find(x => x.idPage == newPage.idPage);
            }

            //  If we changed the ECO database, then update its combo boxes

            populateEcoComboBoxes(currentMachine);

            //  Then clear out the data fields on the page and redisplay it.

            // currentPage = null;
            // currentCardLocationPage = null;

            populateDialog(currentPage);
            pageModified = false;
        }

        private void panelComboBox_SelectedIndexChanged(object sender, EventArgs e) {

            //  The user might have just selected a new machine, that caused a rebuild
            //  of the associated list.  In that case, ignore this event - the currentPanel
            //  will be set during that process.

            if(panelList == null || panelComboBox.SelectedIndex < 0) {
                currentPanel = null;
                return;
            }

            //  Make the new selected panel current.

            currentPanel = panelList[panelComboBox.SelectedIndex];

            //  Find the (first) corresponding page, if any, rather than leaving
            //  an arbitrary page selected, if we can.

            List<Cardlocationpage> clpList = cardLocationPageTable.getWhere(
                "WHERE panel = '" + currentPanel.idPanel + "'");

            if(clpList == null || clpList.Count == 0) {
                return;
            }

            //  If we have a matching page in the page list, make that one current.

            Page page = pageList.Find(x => x.idPage == clpList[0].page);

            //  If there is already a current page and it is different from the
            //  one we are about to select, and if there are modifications,
            //  confirm that the user wishes to discard them.

            if(page != null && page != pageComboBox.SelectedItem &&
                checkForModifications() == DialogResult.Cancel) {
                return;
            }

            pageComboBox.SelectedItem = page;
        }

        private void ecoComboBox_TextChanged(object sender, EventArgs e) {
            // Console.WriteLine("ECO Text Changed: " + ecoComboBox.Text);
        }

        private void newPageButton_Click(object sender, EventArgs e) {

            //  First, check before we throw anything away.

            if (checkForModifications() == DialogResult.Cancel) {
                return;
            }

            //  Most of the real work is done by populate dialog...

            currentPage = null;
            currentCardLocationPage = null;
            populateDialog(currentPage);
        }

        //  A force purge will remove the card location page and everything
        //  that directly or indirectly refers to it.  Used to clean up an
        //  import gone badly wrong.

        private void purgeButton_Click(object sender, EventArgs e) {

            int cardLocationsPurged = 0;
            int notesPurged = 0;
            int blocksPurged = 0;

            DialogResult result;

            if(currentPage.idPage != currentCardLocationPage.page) {
                MessageBox.Show("Inconsistent page ID for current card location page",
                    "Inconsisten Page ID", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            result = MessageBox.Show("WARNING!!! A Force Purge of page " +
                currentPage.name +
                " will remove the card location " +
                "page and ALL direct and indirect references from the " +
                "cardLocation, cardLocationBottomNote and cardLocationBlock " +
                "tables.  Are you sure you want to proceed?",
                "WARNING - MULTIPLE TABLE PURGE",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if(result == DialogResult.Cancel) {
                return;
            }

            //  Okie dokie.  Here we go.

            db.BeginTransaction();

            List<Cardlocation> purgeCardLocationList = cardLocationTable.getWhere(
                "WHERE cardlocation.page='" + 
                currentCardLocationPage.idCardLocationPage + "'");

            foreach(Cardlocation cardlocation in purgeCardLocationList) {

                List<Cardlocationbottomnote> notes = cardLocationBottomNoteTable.getWhere(
                    "WHERE cardLocation='" + cardlocation.idCardLocation + "'");

                foreach(Cardlocationbottomnote note in notes) {
                    cardLocationBottomNoteTable.deleteByKey(note.idCardLocationBottomNote);
                    ++notesPurged;
                }

                List<Cardlocationblock> blocks = cardLocationBlockTable.getWhere(
                    "WHERE cardLocation='" + cardlocation.idCardLocation + "'");

                foreach (Cardlocationblock block in blocks) {
                    cardLocationBlockTable.deleteByKey(block.idCardLocationBlock);
                    ++blocksPurged;
                }

                cardLocationTable.deleteByKey(cardlocation.idCardLocation);
                ++cardLocationsPurged;
            }

            //  Tell the user the results before we commit.

            result = MessageBox.Show("Purge will remove " + cardLocationsPurged +
                " Card locations, along with " +
                notesPurged + " Card Location Bottom Notes and " +
                blocksPurged + " Card Location Blocks.  Do you wish to proceed or cancel?",
                "FINAL CONFIRMATION",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);

            if(result == DialogResult.Cancel) {
                db.CancelTransaction();
                return;
            }

            //  Remember some values for later use for this and other dialogs.

            saveSettings();

            db.CommitTransaction();

            MessageBox.Show("Purge completed.", "Purge Completed",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //  Method to remember some values for later use for this and other dialogs

        private void saveSettings() {
            Parms.setParmValue("machine", currentMachine.idMachine.ToString());
            Parms.setParmValue("volume set", currentVolumeSet.idVolumeSet.ToString());
            Parms.setParmValue("volume", currentVolume.idVolume.ToString());
            Parms.setParmValue("page", currentPage.idPage.ToString());
            Parms.setParmValue("panel", currentPanel.idPanel.ToString());
        }
    }
}
