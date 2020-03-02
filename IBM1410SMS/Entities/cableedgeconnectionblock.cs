using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySQLFramework;

namespace IBM1410SMS
{
	[MyTable]
	public class Cableedgeconnectionblock
	{

		public bool modified {get; set;} = false;

		[MyColumn(Key=true)] public int idCableEdgeConnectionBlock { get; set; }
		[MyColumn] public int cableEdgeConnectionPage { get; set; }
		[MyColumn] public string diagramRow { get; set; }
		[MyColumn] public int diagramColumn { get; set; }
		[MyColumn] public int ecotag { get; set; }
		[MyColumn] public string topNote { get; set; }
		[MyColumn] public int cardSlot { get; set; }
		[MyColumn] public string originNote { get; set; }
		[MyColumn] public string connectionType { get; set; }
		[MyColumn] public string destNote { get; set; }
		[MyColumn] public int explicitDestination { get; set; }
		[MyColumn] public int impliedDestination { get; set; }
	}
}
