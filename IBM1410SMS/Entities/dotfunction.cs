using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySQLFramework;

namespace IBM1410SMS
{
	[MyTable]
	public class Dotfunction
	{

		public bool modified {get; set;} = false;

		[MyColumn(Key=true)] public int idDotFunction { get; set; }
		[MyColumn] public int diagramPage { get; set; }
		[MyColumn] public string diagramRowTop { get; set; }
		[MyColumn] public int diagramColumnToLeft { get; set; }
		[MyColumn] public string logicFunction { get; set; }
		[MyColumn] public int checkExempt { get; set; }
	}
}
