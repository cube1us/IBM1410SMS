using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySQLFramework;

namespace IBM1410SMS
{
	[MyTable]
	public class Logiccheckrules
	{

		public bool modified {get; set;} = false;

		[MyColumn(Key=true)] public int idLogicCheckRule { get; set; }
		[MyColumn] public string machineName { get; set; }
		[MyColumn] public int priority { get; set; }
		[MyColumn] public string logicFunction { get; set; }
		[MyColumn] public string diagramBlockSymbol { get; set; }
		[MyColumn] public string logicBlockType { get; set; }
		[MyColumn] public char char1 { get; set; }
		[MyColumn] public char char2 { get; set; }
		[MyColumn] public char lastChar { get; set; }
		[MyColumn] public char outputPolarity { get; set; }
		[MyColumn] public string comment { get; set; }
	}
}
