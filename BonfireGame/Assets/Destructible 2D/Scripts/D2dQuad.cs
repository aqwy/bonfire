using UnityEngine;

namespace Destructible2D
{
	[System.Serializable]
	public struct D2dQuad
	{
		public D2dVector2 BL;
		
		public D2dVector2 BR;
		
		public D2dVector2 TL;
		
		public D2dVector2 TR;
		
		public D2dVector2 Size;
		
		public int Area;
		
		public void Calculate()
		{
			var minX = Mathf.Min(Mathf.Min(BL.X, BR.X), Mathf.Min(TL.X, TR.X));
			var minY = Mathf.Min(Mathf.Min(BL.Y, BR.Y), Mathf.Min(TL.Y, TR.Y));
			var maxX = Mathf.Max(Mathf.Max(BL.X, BR.X), Mathf.Max(TL.X, TR.X));
			var maxY = Mathf.Max(Mathf.Max(BL.Y, BR.Y), Mathf.Max(TL.Y, TR.Y));
			
			Size.X = maxX - minX;
			Size.Y = maxY - minY;
			
			Area = Size.X * Size.Y;
		}
		
		public void Split(ref D2dQuad first, ref D2dQuad second)
		{
			// Vertical split
			if (Size.X > Size.Y)
			{
				var TS = TL + (TR - TL) / 2;
				var BS = BL + (BR - BL) / 2;
				
				first.BL = BL;
				first.BR = BS;
				first.TL = TL;
				first.TR = TS;
				
				second.BL = BS;
				second.BR = BR;
				second.TL = TS;
				second.TR = TR;
			}
			// Horizontal split
			else
			{
				var LS = BL + (TL - BL) / 2;
				var RS = BR + (TR - BR) / 2;
				
				first.BL = LS;
				first.BR = RS;
				first.TL = TL;
				first.TR = TR;
				
				second.BL = BL;
				second.BR = BR;
				second.TL = LS;
				second.TR = RS;
			}
			
			first.Calculate();
			
			second.Calculate();
		}
	}
}