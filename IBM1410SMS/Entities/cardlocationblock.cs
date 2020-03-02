using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySQLFramework;

namespace IBM1410SMS
{
	[MyTable]
	public class Cardlocationblock
	{

		public bool modified {get; set;} = false;

		[MyColumn(Key=true)] public int idCardLocationBlock { get; set; }
		[MyColumn] public int cardLocation { get; set; }
		[MyColumn] public int diagramPage { get; set; }
		[MyColumn] public int cableEdgeConnectionPage { get; set; }
		[MyColumn] public string diagramRow { get; set; }
		[MyColumn] public int diagramColumn { get; set; }
		[MyColumn] public int diagramECO { get; set; }
		[MyColumn] public int identifiedOnSheet { get; set; }
		[MyColumn] public int ignore { get; set; }
		[MyColumn] public int missingDiagram { get; set; }
		[MyColumn] public string note { get; set; }
	}
}
