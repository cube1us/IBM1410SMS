using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySQLFramework;

namespace IBM1410SMS
{
	[MyTable]
	public class Logiclevels
	{

		public bool modified {get; set;} = false;

		[MyColumn(Key=true)] public int idLogicLevels { get; set; }
		[MyColumn] public string logicLevel { get; set; }
		[MyColumn] public int logicZeroTenths { get; set; }
		[MyColumn] public int logicOneTenths { get; set; }
		[MyColumn] public string circuitType { get; set; }
		[MyColumn] public string dotFunctionLogic { get; set; }
	}
}
