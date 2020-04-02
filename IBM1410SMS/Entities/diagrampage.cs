using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySQLFramework;

namespace IBM1410SMS
{
	[MyTable]
	public class Diagrampage
	{

		public bool modified {get; set;} = false;

		[MyColumn(Key=true)] public int idDiagramPage { get; set; }
		[MyColumn] public int page { get; set; }
		[MyColumn] public int noHDLGeneration { get; set; }
	}
}
