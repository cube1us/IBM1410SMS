using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySQLFramework;

namespace IBM1410SMS
{
	[MyTable]
	public class Panel
	{

		public bool modified {get; set;} = false;

		[MyColumn(Key=true)] public int idPanel { get; set; }
		[MyColumn] public string panel { get; set; }
		[MyColumn] public int gate { get; set; }
		[MyColumn] public int maxColumn { get; set; }
		[MyColumn] public string validRows { get; set; }
	}
}
