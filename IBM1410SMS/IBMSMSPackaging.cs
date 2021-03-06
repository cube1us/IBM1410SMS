﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;

namespace IBM1410SMS
{

    //  Class to contain SMS package (frame/gate, panel, row and column constraints)
    //  Someday, maybe, this could load from a database.

    //  NOTE:  Currently one should NOT use this class during editing validation.
    //  Instead, use the information in the panels table instead.  Use THIS class
    //  only for the Report* classes for validating interconnects.

    class IBMSMSPackaging {

        private class IBMSMSPanel {
            public string panelName { get; set; } = "";
            public bool specialPanel = false;
            public List<string> validRows { get; set; } = new List<string>();
            public List<string> interconnectRows { get; set; } = new List<string>();
            public List<string> adjacencies { get; set; } = new List<string>();
            public int maxColumn = 0;
        }

        private static List<string> RowsAtoK = new List<string>
            {"A", "B", "C", "D", "E", "F", "G", "H", "J", "K"};

        private static List<string> RowsAtoKPlusYZ = new List<string>
            {"A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "Y", "Z"};

        private static List<string> RowsAtoKPlusZ = new List<string>
            {"A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "Z"};

        private static List<string> RowsAtoF = new List<string>
            { "A", "B", "C", "D", "E", "F"};

        private static List<string> RowsLU = new List<string>
            { "L", "U" };

        private static List<string> RowsAB = new List<string>
            { "A", "B" };

        //  Describe Rack and Panel 2 x 2 (4 total + panel 7)

        private static List<IBMSMSPanel> rackAndPanel4Panels = new List<IBMSMSPanel>
        {
            new IBMSMSPanel {panelName = "1", specialPanel = false,
                validRows = RowsAtoKPlusYZ, interconnectRows = {"Y", "Z"},
                adjacencies = {"2"}, maxColumn = 28 },

            new IBMSMSPanel {panelName = "2", specialPanel = false,
                validRows = RowsAtoKPlusYZ, interconnectRows = {"Y", "Z"},
                adjacencies = {"1"}, maxColumn = 28 },

            new IBMSMSPanel {panelName = "3", specialPanel = false,
                validRows = RowsAtoKPlusYZ, interconnectRows = {"Y", "Z"},
                adjacencies = {"4"}, maxColumn = 28 },

            new IBMSMSPanel {panelName = "4", specialPanel = false,
                validRows = RowsAtoKPlusYZ, interconnectRows = {"Y", "Z"},
                adjacencies = {"3"}, maxColumn = 28 },

            new IBMSMSPanel {panelName = "7", specialPanel = true,
                validRows = RowsLU, adjacencies = {}, interconnectRows = {},
                maxColumn = 52 },

            //  CORE  (really only panel B)

            new IBMSMSPanel {panelName = "0", specialPanel = true,
                validRows = {"C"}, adjacencies = { }, interconnectRows = { },
                maxColumn = 4
            }
        };

        private static List<IBMSMSPanel> ibm1415Panel = new List<IBMSMSPanel>
        {
            new IBMSMSPanel {panelName = "4", specialPanel = true,
                validRows = RowsAB, adjacencies = { }, interconnectRows = { },
                maxColumn = 32
            }
        };


        private static Hashtable machines = new Hashtable()
        {
            {"1411", rackAndPanel4Panels },
            {"1415", ibm1415Panel }
        };

        //  Determine whether or not two panels are adjacent

        public static bool isPanelAdjacent(string machineName, string fromPanel,
            string toPanel) {

            if (machines.ContainsKey(machineName)) {
                List<IBMSMSPanel> panels = (List<IBMSMSPanel>)machines[machineName];
                IBMSMSPanel from = panels.Find(x => x.panelName == fromPanel);
                if (from == null || from.panelName != fromPanel) {
                    return false;
                }
                return (from.adjacencies.Contains(toPanel));
            }

            return false;
        }

        //  Determine if a panel is special (i.e., does not actually contain cards)

        public static bool isSpecialPanel(string machineName, string panelName) {
            if (machines.ContainsKey(machineName)) {
                List<IBMSMSPanel> panels = (List<IBMSMSPanel>)machines[machineName];
                IBMSMSPanel panel = panels.Find(x => x.panelName == panelName);
                if (panel == null || panel.panelName != panelName) {
                    return false;
                }
                return (panel.specialPanel);
            }

            return false;
        }

        //  Determine if a row is an interconnect row

        public static bool isInterconnectRow(string machineName, string panelName,
            string rowName) {
            if(machines.Contains(machineName)) {
                List<IBMSMSPanel> panels = (List<IBMSMSPanel>)machines[machineName];
                IBMSMSPanel panel = panels.Find(x => x.panelName == panelName);
                if (panel == null || panel.panelName != panelName) {
                    return false;
                }
                return (panel.interconnectRows.Contains(rowName));
            }

            return (false);
        }

        //  Determine if a row and column are valid for a given panel

        public static bool isValidRowColumn(string machineName, string panelName,
            string row, int column) {
            if (machines.ContainsKey(machineName)) {
                List<IBMSMSPanel> panels = (List<IBMSMSPanel>)machines[machineName];
                IBMSMSPanel panel = panels.Find(x => x.panelName == panelName);
                if(panel == null || panel.panelName != panelName) {
                    return false;
                }
                return (panel.validRows.Contains(row) && column <= panel.maxColumn);
            }

            return false;
        }
    }

}
