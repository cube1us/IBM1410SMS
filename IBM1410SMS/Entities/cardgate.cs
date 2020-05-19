using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySQLFramework;

namespace IBM1410SMS
{
	[MyTable]
	public class Cardgate
	{

		public bool modified {get; set;} = false;

		[MyColumn(Key=true)] public int idcardGate { get; set; }
		[MyColumn] public int cardType { get; set; }
		[MyColumn] public int number { get; set; }
		[MyColumn] public int definingPin { get; set; }
		[MyColumn] public int transistorNumber { get; set; }
		[MyColumn] public int positiveLogicFunction { get; set; }
		[MyColumn] public int negativeLogicFunction { get; set; }
		[MyColumn] public int logicFunction { get; set; }
		[MyColumn] public string HDLname { get; set; }
		[MyColumn] public int latchGate { get; set; }
		[MyColumn] public int openCollector { get; set; }
		[MyColumn] public int inputLevel { get; set; }
		[MyColumn] public int outputLevel { get; set; }
		[MyColumn] public int? componentValue { get; set; }
		[MyColumn] public int noOutExempt { get; set; }
	}
}
