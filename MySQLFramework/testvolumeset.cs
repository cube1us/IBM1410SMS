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

//	This is a test entity type that was created and edited
//	outside of Visual Studio.

namespace MySQLFramework
{
    [MyTable]
    class TestVolumeSet
    {
        [MyColumn(Key=true)] public int idVolumeSet { get; set; }
        [MyColumn] public string machineType { get; set; }
	    [MyColumn] public string machineSerial { get; set; }

        public TestVolumeSet() {
            idVolumeSet = 0;
            machineType = "";
            machineSerial = "";
        }

    }
}
