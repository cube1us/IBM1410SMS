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
using System.Threading.Tasks;

using MySQLFramework;

//  This class handles the management of the counter table - fetching
//  the current value, and incrementing/storing the counter

//  Note it is a static class - no instance variables.

//  Note:  We do NOT remember the counter here.  

namespace IBM1410SMS
{
    static class IdCounter
    {

        /*
        public static int getCounter() {

            //  Call the internal routine - no increment.

            return doCounter(false);
        }
        */

        public static int incrementCounter() {

            //  Call the internal routine - increment

            return doCounter(true);
        }

        static private int doCounter(Boolean increment) {

            //  Get a handle to the database setup, and from that, a handle
            //  to the idCounter table.

            DBSetup db = DBSetup.Instance;            //  Grab the database setup
            Table<Idcounter> idCounterTable = db.getIdCounterTable();

            //  Fetch the current counter (it is always key value 1)

            Idcounter counter = idCounterTable.getByKey(1);

            if(counter == null || counter.counter == 0) {
                throw new CounterException("ID Counter is null or 0");
            }

            //  If we are not incrementing, we are all done.

            if (!increment) {
                return counter.counter;
            }

            //  If we are incrementing, increment the value, and update the
            //  database.

            ++counter.counter;
            idCounterTable.update(counter);
            return (counter.counter);
        }
    }
}
