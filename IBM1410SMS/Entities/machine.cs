using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySQLFramework;

namespace IBM1410SMS
{
	[MyTable]
	public class Machine
	{

		public bool modified {get; set;} = false;

		[MyColumn(Key=true)] public int idMachine { get; set; }
		[MyColumn] public string name { get; set; }
		[MyColumn] public string description { get; set; }
		[MyColumn] public string aldMachineType { get; set; }
		[MyColumn] public string frameLabel { get; set; }
		[MyColumn] public string gateLabel { get; set; }
		[MyColumn] public string panelLabel { get; set; }
		[MyColumn] public string rowLabel { get; set; }
		[MyColumn] public string columnLabel { get; set; }
	}
}
