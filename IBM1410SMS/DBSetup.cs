using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySQLFramework;
using MySql.Data.MySqlClient;


/*
 * This class contains relatively static fields for database access,
 * including instantiations for the entities.  It is a singleton, by
 * way of the instance field.
 *
*/

namespace IBM1410SMS
{
    public class DBSetup
    {
        private static readonly DBSetup instance = new DBSetup();
        private MySqlConnection connection = null;
        private MySqlTransaction transaction = null;

        //  A field for each table we use...

        private Table<Machine> machineTable;
        private Table<Idcounter> idCounterTable;
        private Table<Volumeset> volumeSetTable;
        private Table<Volume> volumeTable;
        private Table<Page> pageTable;
        private Table<Feature> featureTable;
        private Table<Eco> ecoTable;
        private Table<Cardtype> cardTypeTable;
        private Table<Diagramblock> diagramBlockTable;
        private Table<Cardlocation> cardLocationTable;
        private Table<Frame> frameTable;
        private Table<Machinegate> machineGateTable;
        private Table<Panel> panelTable;
        private Table<Cardslot> cardSlotTable;
        private Table<Cardlocationpage> cardLocationPageTable;
        private Table<Tiedown> tieDownTable;
        private Table<Edgeconnector> edgeConnectorTable;
        private Table<Cardeco> cardEcoTable;
        private Table<Cardlocationblock> cardLocationBlockTable;
        private Table<Diagramecotag> diagramEcoTagTable;
        private Table<Ibmlogicfunction> ibmLogicFunctionTable;
        private Table<Cardgate> cardGateTable;
        private Table<Logicfunction> logicFunctionTable;
        private Table<Logicfamily> logicFamilyTable;
        private Table<Logiclevels> logicLevelsTable;
        private Table<Cardnote> cardNoteTable;
        private Table<Gatepin> gatePinTable;
        private Table<Cardlocationbottomnote> cardLocationBottomNoteTable;
        private Table<Diagrampage> diagramPageTable;
        private Table<Sheetedgeinformation> sheetEdgeInformationTable;
        private Table<Dotfunction> dotFunctionTable;
        private Table<Connection> connectionTable;
        private Table<Parameters> parametersTable;
        private Table<Cableedgeconnectionpage> cableEdgeConnectionPageTable;
        private Table<Cableedgeconnectionblock> cableEdgeConnectionBlockTable;
        private Table<Cableedgeconnectionecotag> cableEdgeConnectionECOTagTable;
        private Table<Cableimplieddestinations> cableImpliedDestinationsTable;
        private Table<Logiccheckrules> logicCheckRulesTable;

        //  This is the constructor.  It CANNOT do ANYTHING, because it
        //  is statically initialized.

        private DBSetup() {
        }

        public Boolean Init() {

            //  Create the database connection.

            string connectionString = @"server=localhost;userid=collection;
            password=twiddle;database=ibm1410sms";

            //  Open the connection.

            try {
                connection = new MySqlConnection(connectionString);
                connection.Open();
            }
            catch (MySqlException ex) {
                //  TODO: Should there be a dialog box?
                Console.WriteLine("Error: {0}", ex.ToString());
                return (false);
            }

            //  Create the database entitites...

            machineTable = new Table<Machine>(connection);
            idCounterTable = new Table<Idcounter>(connection);
            volumeSetTable = new Table<Volumeset>(connection);
            volumeTable = new Table<Volume>(connection);
            pageTable = new Table<Page>(connection);
            featureTable = new Table<Feature>(connection);
            ecoTable = new Table<Eco>(connection);
            cardTypeTable = new Table<Cardtype>(connection);
            diagramBlockTable = new Table<Diagramblock>(connection);
            cardLocationTable = new Table<Cardlocation>(connection);
            frameTable = new Table<Frame>(connection);
            machineGateTable = new Table<Machinegate>(connection);
            panelTable = new Table<Panel>(connection);
            cardSlotTable = new Table<Cardslot>(connection);
            cardLocationPageTable = new Table<Cardlocationpage>(connection);
            tieDownTable = new Table<Tiedown>(connection);
            edgeConnectorTable = new Table<Edgeconnector>(connection);
            cardLocationBlockTable = new Table<Cardlocationblock>(connection);
            cardEcoTable = new Table<Cardeco>(connection);
            diagramEcoTagTable = new Table<Diagramecotag>(connection);
            ibmLogicFunctionTable = new Table<Ibmlogicfunction>(connection);
            cardGateTable = new Table<Cardgate>(connection);
            logicFunctionTable = new Table<Logicfunction>(connection);
            logicFamilyTable = new Table<Logicfamily>(connection);
            logicLevelsTable = new Table<Logiclevels>(connection);
            cardNoteTable = new Table<Cardnote>(connection);
            gatePinTable = new Table<Gatepin>(connection);
            cardLocationBottomNoteTable = new Table<Cardlocationbottomnote>(connection);
            diagramPageTable = new Table<Diagrampage>(connection);
            sheetEdgeInformationTable = new Table<Sheetedgeinformation>(connection);
            dotFunctionTable = new Table<Dotfunction>(connection);
            connectionTable = new Table<Connection>(connection);
            parametersTable = new Table<Parameters>(connection);
            cableEdgeConnectionPageTable = new Table<Cableedgeconnectionpage>(connection);
            cableEdgeConnectionBlockTable = new Table<Cableedgeconnectionblock>(connection);
            cableEdgeConnectionECOTagTable = new Table<Cableedgeconnectionecotag>(connection);
            cableImpliedDestinationsTable = new Table<Cableimplieddestinations>(connection);
            logicCheckRulesTable = new Table<Logiccheckrules>(connection);

            return (true);
        }

        public static DBSetup Instance {
            get {
                return instance;
            }
        }

        //  Method to return the existing connection.

        public MySqlConnection getConnection() {
            return connection;
        }

        //  Methods to start and commit transactions...

        public void BeginTransaction() {
            transaction = connection.BeginTransaction();
        }

        public void CommitTransaction() {
            if (transaction != null) {
                transaction.Commit();
            }
        }

        public void CancelTransaction() {
            if(transaction != null) {
                transaction.Rollback();
            }
        }


        //  Methods to return Table objects

        public Table<Machine> getMachineTable() {
            return machineTable;
        }

        public Table<Idcounter> getIdCounterTable() {
            return idCounterTable;
        }

        public Table<Volumeset> getVolumeSetTable() {
            return volumeSetTable;
        }

        public Table<Volume> getVolumeTable() {
            return volumeTable;
        }

        public Table<Page> getPageTable() {
            return pageTable;
        }

        public Table<Feature> getFeatureTable() {
            return featureTable;
        }

        public Table<Eco> getEcoTable() {
            return ecoTable;
        }

        public Table<Cardtype> getCardTypeTable() {
            return cardTypeTable;
        }

        public Table<Diagramblock> getDiagramBlockTable() {
            return diagramBlockTable;
        }

        public Table<Cardlocation> getCardLocationTable() {
            return cardLocationTable;
        }

        public Table<Frame> getFrameTable() {
            return frameTable;
        }

        public Table<Machinegate> getMachineGateTable() {
            return machineGateTable;
        }

        public Table<Panel> getPanelTable() {
            return panelTable;
        }

        public Table<Cardslot> getCardSlotTable() {
            return cardSlotTable;
        }

        public Table<Cardlocationpage> getCardLocationPageTable() {
            return cardLocationPageTable;
        }

        public Table<Tiedown> getTieDownTable() {
            return tieDownTable;
        }

        public Table<Edgeconnector> getEdgeConnectorTable() {
            return edgeConnectorTable;
        }

        public Table<Cardeco> getCardEcoTable() {
            return cardEcoTable;
        }

        public Table<Cardlocationblock> getCardLocationBlockTable() {
            return cardLocationBlockTable;
        }

        public Table<Diagramecotag> getDiagramEcoTagTable() {
            return diagramEcoTagTable;
        }

        public Table<Ibmlogicfunction> getIbmLogicFunctionTable() {
            return ibmLogicFunctionTable;
        }

        public Table<Cardgate> getCardGateTable() {
            return cardGateTable;
        }

        public Table<Logicfunction> getLogicFunctionTable() {
            return logicFunctionTable;
        }

        public Table<Logicfamily> getLogicFamilyTable() {
            return logicFamilyTable;
        }

        public Table<Logiclevels> getLogicLevelsTable() {
            return logicLevelsTable;
        }

        public Table<Cardnote> getCardNoteTable() {
            return cardNoteTable;
        }

        public Table<Gatepin> getGatePinTable() {
            return gatePinTable;
        }

        public Table<Cardlocationbottomnote> getCardLocationBottomNoteTable() {
            return cardLocationBottomNoteTable;
        }

        public Table<Diagrampage> getDiagramPageTable() {
            return diagramPageTable;
        }

        public Table<Sheetedgeinformation> getSheetEdgeInformationTable() {
            return sheetEdgeInformationTable;
        }

        public Table<Dotfunction> getDotFunctionTable() {
            return dotFunctionTable;
        }

        public Table<Connection> getConnectionTable() {
            return connectionTable;
        }

        public Table<Parameters> getParametersTable() {
            return parametersTable;
        }

        public Table<Cableedgeconnectionpage> getCableEdgeConnectionPageTable() {
            return cableEdgeConnectionPageTable;
        }

        public Table<Cableedgeconnectionblock> getCableEdgeConnectionBlockTable() {
            return cableEdgeConnectionBlockTable;
        }

        public Table<Cableedgeconnectionecotag> getCableEdgeConnectionECOTagTable()
        {
            return cableEdgeConnectionECOTagTable;
        }

        public Table<Cableimplieddestinations> getCableImpliedDestinationsTable() {
            return cableImpliedDestinationsTable;
        }

        public Table<Logiccheckrules> getLogicCheckRulesTable() {
            return logicCheckRulesTable;
        }
    }
}
