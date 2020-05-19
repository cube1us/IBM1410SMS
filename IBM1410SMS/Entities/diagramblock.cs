using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySQLFramework;

namespace IBM1410SMS
{
	[MyTable]
	public class Diagramblock
	{

		public bool modified {get; set;} = false;

		[MyColumn(Key=true)] public int idDiagramBlock { get; set; }
		[MyColumn] public int extendedTo { get; set; }
		[MyColumn] public int diagramPage { get; set; }
		[MyColumn] public string diagramRow { get; set; }
		[MyColumn] public int diagramColumn { get; set; }
		[MyColumn] public string title { get; set; }
		[MyColumn] public string symbol { get; set; }
		[MyColumn] public int feature { get; set; }
		[MyColumn] public int inputMode { get; set; }
		[MyColumn] public int outputMode { get; set; }
		[MyColumn] public int cardSlot { get; set; }
		[MyColumn] public int eco { get; set; }
		[MyColumn] public int cardType { get; set; }
		[MyColumn] public int cardGate { get; set; }
		[MyColumn] public string blockConfiguration { get; set; }
		[MyColumn] public int flipped { get; set; }
		[MyColumn] public string notes { get; set; }
		[MyColumn] public int noHDLGeneration { get; set; }
		[MyColumn] public int checkExempt { get; set; }
	}
}
