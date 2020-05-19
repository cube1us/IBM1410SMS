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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using MySQLFramework;

//  This class handles activities related to card slot coordinate ranges.
//  Coordinates are composed of rows (up to two letters) and columns (integers).
//  It is also set up to make dealing with the datagridview much easier.

namespace IBM1410SMS
{
    class PanelRowColumn {

        //  Valid row specification

        public static Regex ROWPATTERN { get; } =
            new Regex("^[ABCDEFGHJKLMNPQRSTUVWXYZ]{1,2}$");
        public static int MAXCOL { get; } = 99;

        public int idPanel { get; set; }
        public string panel { get; set; }
        public int gate { get; set; }
        public bool modified { get; set; }

        public string minRow { get; set; }
        public string maxRow { get; set; }
        public int minCol { get; set; }
        public int maxCol { get; set; }

        public bool validCoordinate {get;}
        public string validRows { get; set; }

        public PanelRowColumn() {

        }

        //  Constructor to set the data from a panel entity, and
        //  the range from  parameters directly.

        public PanelRowColumn(Panel panel, string minRow, string maxRow,
            int minCol, int maxCol, string validRows) {


            //  First, copy the info from the specified entity.

            idPanel = panel.idPanel;
            this.panel = panel.panel;
            gate = panel.gate;
            modified = panel.modified;

            //  Check that the secified row and column are valid.

            validCoordinate = false;

            if(!ROWPATTERN.IsMatch(minRow)) {
                throw new ArgumentException(String.Format("{0} is " +
                    "not A-Z or AA-ZZ",minRow),"minRow");
            }
            if (!ROWPATTERN.IsMatch(maxRow)) {
                throw new ArgumentException(String.Format("{0} is " +
                    "not A-Z or AA-ZZ",maxRow), "maxRow");
            }
            if(minCol < 1 || minCol > MAXCOL) {
                throw new ArgumentException(String.Format("{0} is " +
                    "not in range of 1-{1}", minCol, MAXCOL), "minCol");

            }
            if (maxCol < 1 || maxCol > MAXCOL) {
                throw new ArgumentException(String.Format("{0} is " +
                    "not in range of 1-99", maxCol), "maxCol");
            }
            if(String.Compare(minRow,maxRow,false) > 0) {
                throw new ArgumentException(String.Format("{0} is greater than {1}",
                    minRow, maxRow), "minRow/MaxRow");
            }
            if(minCol > maxCol) {
                throw new ArgumentException(String.Format("{0} is greater than {1}",
                    minCol, maxCol), "minCol/maxCol");
            }

            this.minRow = minRow;
            this.maxRow = maxRow;
            this.minCol = minCol;
            this.maxCol = maxCol;
            this.validRows = validRows;
            validCoordinate = true;
        }


        //  Constructor to set the range that exists for a given Panel
        //  in the database.

        public PanelRowColumn(Panel panel) {

            DBSetup db = DBSetup.Instance;
            Table<Cardslot> cardSlotTable = db.getCardSlotTable();
            List<Cardslot> cardSlotList = cardSlotTable.getWhere(
                "WHERE panel='" + panel.idPanel + "'");
            List<string> validRowsSorted;


            idPanel = panel.idPanel;
            this.panel = panel.panel;
            gate = panel.gate;
            modified = panel.modified;

            minRow = "A";
            maxRow = "";
            minCol = 1;
            maxCol = -1;

            //  If there are not entries, then create a default.

            if(cardSlotList.Count <= 0) {
                maxRow = minRow;
                maxCol = minCol;
                validCoordinate = true;
                return;
            }

            //  Othrewise, run through them and capture the min and max.

            minRow = "ZZ";
            minCol = 100;
            validCoordinate = false;

            foreach (Cardslot cs in cardSlotList) {
                if (String.Compare(cs.cardRow, minRow, false) < 0) {
                    minRow = cs.cardRow;
                }
                if (String.Compare(cs.cardRow, maxRow, false) > 0) {
                    maxRow = cs.cardRow;
                }
                if (cs.cardColumn < minCol) {
                    minCol = cs.cardColumn;
                }
                if (cs.cardColumn > maxCol) {
                    maxCol = cs.cardColumn;
                }

            }

            if(panel.validRows != null) {
                validRowsSorted = new List<string>(
                    panel.validRows.Split(new char[] { ',' }));
                validRowsSorted.Sort();
            }
            else {
                validRowsSorted = new List<string>();
            }

            validRows = String.Join(",",validRowsSorted);

            //  Now, make sure that they were all valid, so we don't
            //  have a GIGO issue...

            if (String.Compare(minRow,"ZZ",false) == 0) {
                throw new ArgumentOutOfRangeException(
                    String.Format("No valid minimum Row value in cardslot table " +
                    "for panel ID {0}", panel.idPanel));
            }

            if (maxRow.Length == 0 || String.Compare(maxRow,"ZZ",false) > 0) {
                throw new ArgumentOutOfRangeException(
                    String.Format("No valid maximum Row value in cardslot table " +
                    "for panel ID {0}", panel.idPanel));
            }

            if (minCol >= 100 || minCol < 1) {
                throw new ArgumentOutOfRangeException(
                    String.Format("No valid minimum Column value in cardslot table " +
                    "for panel ID {0}", panel.idPanel));
            }

            if (maxCol <= 0 || maxCol > MAXCOL) {
                throw new ArgumentOutOfRangeException(
                    String.Format("No valid maximum Column value in cardslot table " +
                    "for panel ID {0}", panel.idPanel));
            }
            validCoordinate = true;
        }

    }
}
