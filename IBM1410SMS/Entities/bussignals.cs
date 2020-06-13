using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySQLFramework;

namespace IBM1410SMS
{
	[MyTable]
	public class Bussignals
	{

		public bool modified {get; set;} = false;

		[MyColumn(Key=true)] public string signalName { get; set; }
		[MyColumn] public string busName { get; set; }
		[MyColumn] public int busBit { get; set; }
	}
}
