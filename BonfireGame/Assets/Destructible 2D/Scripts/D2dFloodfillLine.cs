using System.Collections.Generic;

namespace Destructible2D
{
	public static partial class D2dFloodfill
	{
		public class Line
		{
			public bool   Used;
			public int    Y;
			public int    MinX;
			public int    MaxX;
			public Island Island;

			public List<Line> Ups = new List<Line>();
			public List<Line> Dns = new List<Line>();
		}
	}
}