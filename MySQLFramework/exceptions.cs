﻿/* 
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

namespace MySQLFramework
{

    //  Exception thrown if a member of the mysqlEntity class does not have
    //  the MyTable attribute

    public class MyTableMissingException : Exception
    {

        public MyTableMissingException() {
        }

        public MyTableMissingException(string message) : base(message) {
        }

        public MyTableMissingException(string message, Exception inner) : base(message, inner) {
        }
    }


    //  Exception thrown if a member of the mysqlEntity class has more than one
    //  column with the MyColumn attribute with the Key value set to true.

    public class MyTableKeyException: Exception { 

        public MyTableKeyException() {
        }

        public MyTableKeyException(string message) : base(message) {
        }

        public MyTableKeyException(string message, Exception inner) : base(message,inner) {
        }
    }
}
