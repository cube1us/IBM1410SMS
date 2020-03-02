using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySQLFramework;

namespace IBM1410SMS
{
	[MyTable]
	public class Cableedgeconnectionpage
	{

		public bool modified {get; set;} = false;

		[MyColumn(Key=true)] public int idCableEdgeConnectionPage { get; set; }
		[MyColumn] public int page { get; set; }
	}
}
