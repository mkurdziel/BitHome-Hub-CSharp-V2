using System;

namespace BitHome
{
	public struct Triple<T1, T2, T3> {
		public readonly T1 Item1;
		public readonly T2 Item2;
		public readonly T3 Item3;
		public Triple(T1 item1, T2 item2, T3 item3) { Item1 = item1; Item2 = item2; Item3 = item3;} 
	}

	public static class Triple { // for type-inference goodness.
		public static Triple<T1,T2,T3> Create<T1,T2,T3>(T1 item1, T2 item2, T3 item3) { 
			return new Triple<T1,T2,T3>(item1, item2, item3); 
		}
	}
}

