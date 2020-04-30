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
    public partial class IBM1410SMSForm : Form
    {

        public Table<Machine> machineTable = null;

        public IBM1410SMSForm() {
            InitializeComponent();

            //  Set up the singleton DBSetup, and initialize it.

            DBSetup dbSetup = DBSetup.Instance;
            dbSetup.Init();
        }

        private void editFeaturesMenuItem_Click(object sender, EventArgs e) {
            Form EditFeaturesDialog = new EditFeaturesForm();
            EditFeaturesDialog.ShowDialog();
        }

        private void editVolumeSetsMenuItem_Click(object sender, EventArgs e) {
            Form EditVolumeSetsDialog = new EditVolumeSetsForm();
            EditVolumeSetsDialog.ShowDialog();
        }

        private void editVolumeMenuItem_Click(object sender, EventArgs e) {
            Form EditVolumeDialog = new EditVolumeForm();
            EditVolumeDialog.ShowDialog();
        }

        private void editMachinesMenuItem_Click(object sender, EventArgs e) {
            Form EditMachinesDialog = new EditMachinesForm();
            EditMachinesDialog.ShowDialog();
        }

        private void editFramesMenuItem_Click(object sender, EventArgs e) {
            Form EditFramesDialog = new EditFramesForm();
            EditFramesDialog.ShowDialog();
        }

        private void editMachineGatesMenuItem_Click(object sender, EventArgs e) {
            Form EditMachineGatesDialog = new EditMachineGatesForm();
            EditMachineGatesDialog.ShowDialog();
        }

        private void editPanelsMenuItem_Click(object sender, EventArgs e) {
            Form EditPanelsDialog = new EditPanelsForm();
            EditPanelsDialog.ShowDialog();
        }

        private void editEcosMenuItem_Click(object sender, EventArgs e) {
            Form EditEcosDialog = new EditECOsForm();
            EditEcosDialog.ShowDialog();
        }

        private void cardLocationPagesToolStripMenuItem_Click(object sender, EventArgs e) {
            Form CardLocationPageDialog = new EditCardLocationPageForm();
            CardLocationPageDialog.ShowDialog();
        }

        private void IBMLogicFunctionsMenuItem_Click(object sender, EventArgs e) {
            Form IBMLogicFunctionsDialog = new EditIBMLogicFunctionForm();
            IBMLogicFunctionsDialog.ShowDialog();
        }

        private void standardLogicFunctionsMenuItem_Click(object sender, EventArgs e) {
            Form LogicFunctionsDialog = new EditLogicFunctionsForm();
            LogicFunctionsDialog.ShowDialog();
        }

        private void iBMLogicFamiliesToolStripMenuItem_Click(object sender, EventArgs e) {
            Form LogicFamiliesDialog = new EditLogicFamiliesForm();
            LogicFamiliesDialog.ShowDialog();
        }

        private void logicVoltageLevelsToolStripMenuItem_Click(object sender, EventArgs e) {
            Form LogicLevelsDialog = new EditLogicLevelsForm();
            LogicLevelsDialog.ShowDialog();
        }

        private void editSMSCardTypesToolStripMenuItem_Click(object sender, EventArgs e) {
            Form EditCardTypesDialog = new EditCardTypesForm();
            EditCardTypesDialog.ShowDialog();
        }

        private void editSMSCardGatesToolStripMenuItem_Click(object sender, EventArgs e) {
            Form EditCardGatesDialog = new EditCardGatesForm();
            EditCardGatesDialog.ShowDialog();
        }

        private void editSMSCardPinsToolStripMenuItem_Click(object sender, EventArgs e) {
            Form EditCardPinsDialog = new EditCardGatePinsForm();
            EditCardPinsDialog.ShowDialog();
        }

        private void cardLocationShartsToolStripMenuItem_Click(object sender, EventArgs e) {
            Form EditCardLocationForm = new EditCardLocationForm();
            EditCardLocationForm.ShowDialog();
        }

        private void tieDownsMenuItem_Click(object sender, EventArgs e) {
            Form EditTieDownsForm = new EditTieDownsForm();
            EditTieDownsForm.ShowDialog();
        }

        private void aldDiagramPagesMenuItem_Click(object sender, EventArgs e) {
            EditDiagramPageForm EditDiagramPageForm = new EditDiagramPageForm();
            EditDiagramPageForm.ShowDialog();
        }


        private void importFeaturesMenuItem_Click(object sender, EventArgs e) {
            ImportStartupForm ImportStartupForm = new ImportStartupForm();
            DialogResult status = ImportStartupForm.ShowDialog();
            if (status != DialogResult.Cancel) {
                ImportFeatures junk = new ImportFeatures(
                ImportStartupForm.fileName, ImportStartupForm.disposition,
                    ImportStartupForm.testMode);
            }
        }

        private void importSMSCardsMenuItem_Click(object sender, EventArgs e) {
            ImportStartupForm ImportStartupForm = new ImportStartupForm();
            DialogResult status = ImportStartupForm.ShowDialog();
            if (status != DialogResult.Cancel) {
                ImportCardTypes junk = new ImportCardTypes(
                ImportStartupForm.fileName, ImportStartupForm.disposition,
                ImportStartupForm.testMode);
            }
        }

        private void importLocationPagesToolStripMenuItem_Click(object sender, EventArgs e) {
            ImportStartupForm ImportStartupForm = new ImportStartupForm();
            DialogResult status = ImportStartupForm.ShowDialog();
            if (status != DialogResult.Cancel) {
                ImportCardLocationPages junk = new ImportCardLocationPages(
                ImportStartupForm.fileName, ImportStartupForm.disposition,
                ImportStartupForm.testMode);
            }
        }

        private void importCardLocationChartMenuItem_Click(object sender, EventArgs e) {
            ImportStartupForm ImportStartupForm = new ImportStartupForm();
            DialogResult status = ImportStartupForm.ShowDialog();
            if (status != DialogResult.Cancel) {
                ImportCardLocationChart junk = new ImportCardLocationChart(
                ImportStartupForm.fileName, ImportStartupForm.disposition,
                ImportStartupForm.testMode);
            }
        }

        private void importTieDownsMenuItem_Click(object sender, EventArgs e) {
            ImportStartupForm ImportStartupForm = new ImportStartupForm();
            DialogResult status = ImportStartupForm.ShowDialog();
            if (status != DialogResult.Cancel) {
                ImportTieDowns junk = new ImportTieDowns(
                ImportStartupForm.fileName, ImportStartupForm.disposition,
                ImportStartupForm.testMode);
            }
        }

        private void generateMenuItem_Click(object sender, EventArgs e) {

            GenerateHDLForm GenerateHDLForm = new GenerateHDLForm();
            GenerateHDLForm.ShowDialog();
        }

        private void generateGroupToolStripMenuItem_Click(object sender, EventArgs e) {

            GenerateGroupHDLForm generateGroupHDLForm = new GenerateGroupHDLForm();
            generateGroupHDLForm.ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form AboutBox = new AboutBox();
            AboutBox.ShowDialog();
        }

        private void cardTypeReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form ReportCardTypeUsageForm = new ReportCardTypeUsageForm();
            ReportCardTypeUsageForm.ShowDialog();

        }

        private void cableEdgeConnectionPagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditCableEdgeConnectionPageForm EditCableEdgeConnectionPageForm = 
                new EditCableEdgeConnectionPageForm();
            EditCableEdgeConnectionPageForm.ShowDialog();

        }

        private void CONNectorCheckToolStripMenuItem_Click(object sender, EventArgs e) {
            ReportConnectorErrorsForm ReportConnectorErrorsForm =
                new ReportConnectorErrorsForm();
            ReportConnectorErrorsForm.ShowDialog();
        }

        private void edgeConnectionCheckToolStripMenuItem_Click(object sender, EventArgs e) {
            ReportEdgeConnectionsForm ReportEdgeConnectionsForm =
                new ReportEdgeConnectionsForm();
            ReportEdgeConnectionsForm.ShowDialog();
        }

        private void signalReportToolStripMenuItem_Click(object sender, EventArgs e) {
            ReportSignalsForm ReportSignalsForm =
                new ReportSignalsForm();
            ReportSignalsForm.ShowDialog();
        }
    }
}
