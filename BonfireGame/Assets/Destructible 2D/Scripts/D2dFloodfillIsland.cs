using UnityEngine;
using System.Collections.Generic;

namespace Destructible2D
{
	public static partial class D2dFloodfill
	{
		public class Island
		{
			public int MinX;

			public int MinY;

			public int MaxX;

			public int MaxY;

			public int Count;

			public List<Line> Lines = new List<Line>();

			private static D2dDistanceField distanceField = new D2dDistanceField();

			public void Clear()
			{
				Lines.Clear();
			}

			public void Submit(D2dDistanceField baseField, D2dSplitGroup splitGroup, D2dRect baseRect, D2dRect rect)
			{
				distanceField.Transform(rect, this);

				for (var y = rect.MinY; y < rect.MaxY; y++)
				{
					for (var x = rect.MinX; x < rect.MaxX; x++)
					{
						var cell     = distanceField.Cells[x - rect.MinX + (y - rect.MinY) * rect.SizeX];
						var baseCell = baseField.Cells[x - baseRect.MinX + (y - baseRect.MinY) * baseRect.SizeX];

						if (cell.D == baseCell.D)
						{
							splitGroup.AddPixel(x, y);
						}
					}
				}
			}
		}
	}
}