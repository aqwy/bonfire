using UnityEngine;
using System.Collections.Generic;

namespace Destructible2D
{
	public static partial class D2dFloodfill
	{
		public enum Layout
		{
			Whole,
			Island,
			Vertical,
			Horizontal
		}

		public static Layout IslandLayout;

		public static List<Island> IslandsA = new List<Island>();

		public static List<Island> IslandsB = new List<Island>();

		public static List<Island> IslandsC = new List<Island>();

		private static List<Island> tempIslands = new List<Island>();

		private static List<Line> lines = new List<Line>();

		private static List<Line> linkedLines = new List<Line>();

		private static int lineCount;

		public static void Clear()
		{
			Clear(tempIslands);
			Clear(IslandsA);
			Clear(IslandsB);
			Clear(IslandsC);
		}

		private static void Clear(List<Island> islands)
		{
			for (var i = islands.Count - 1; i >= 0; i--)
			{
				var island = islands[i];

				island.Clear();

				D2dPool<Island>.Despawn(island);
			}

			islands.Clear();
		}

		private static void CalculateLayout(D2dRect rect, int alphaWidth, int alphaHeight)
		{
			if (rect.MinX == 0 && rect.MaxX == alphaWidth && rect.MinY == 0 && rect.MaxY == alphaHeight)
			{
				IslandLayout = Layout.Whole; return;
			}

			if (rect.MinX == 0 && rect.MaxX == alphaWidth && rect.MinY > 0 && rect.MaxY < alphaHeight)
			{
				IslandLayout = Layout.Vertical; return;
			}

			if (rect.MinY == 0 && rect.MaxY == alphaHeight && rect.MinX > 0 && rect.MaxX < alphaWidth)
			{
				IslandLayout = Layout.Horizontal; return;
			}

			IslandLayout = Layout.Island;
		}

		public static void FastFind(byte[] alphaData, int alphaWidth, int alphaHeight, D2dRect rect)
		{
			Clear();

			if (rect.MinX < 0) rect.MinX = 0;
			if (rect.MinY < 0) rect.MinY = 0;
			if (rect.MaxX > alphaWidth ) rect.MaxX = alphaWidth;
			if (rect.MaxY > alphaHeight) rect.MaxY = alphaHeight;

			lineCount = 0;

			CalculateLayout(rect, alphaWidth, alphaHeight);

			var oldCount = 0;

			for (var y = rect.MinY; y < rect.MaxY; y++)
			{
				var newCount = FastFindLines(alphaData, alphaWidth, y, rect.MinX, rect.MaxX);

				FastLinkLines(lineCount - newCount - oldCount, lineCount - newCount, lineCount);

				oldCount = newCount;
			}

			for (var i = 0; i < lineCount; i++)
			{
				var line = lines[i];

				if (line.Used == false)
				{
					var island = D2dPool<Island>.Spawn() ?? new Island();

					island.MinX  = line.MinX;
					island.MaxX  = line.MaxX;
					island.MinY  = line.Y;
					island.MaxY  = line.Y + 1;
					island.Count = 0;

					// Scan though all connected lines and add to list
					linkedLines.Clear(); linkedLines.Add(line); line.Used = true;

					for (var j = 0; j < linkedLines.Count; j++)
					{
						var linkedLine = linkedLines[j];

						island.MinX   = Mathf.Min(island.MinX, linkedLine.MinX);
						island.MaxX   = Mathf.Max(island.MaxX, linkedLine.MaxX);
						island.MinY   = Mathf.Min(island.MinY, linkedLine.Y    );
						island.MaxY   = Mathf.Max(island.MaxY, linkedLine.Y + 1);
						island.Count += linkedLine.MaxX - linkedLine.MinX;

						AddToScan(linkedLine.Ups);
						AddToScan(linkedLine.Dns);

						linkedLine.Island = island;

						island.Lines.Add(linkedLine);
					}

					// Bridge layout?
					switch (IslandLayout)
					{
						case Layout.Vertical:
							if (island.MinY <= rect.MinY && island.MaxY >= rect.MaxY) IslandLayout = Layout.Island;
						break;

						case Layout.Horizontal:
							if (island.MinX <= rect.MinX && island.MaxX >= rect.MaxX) IslandLayout = Layout.Island;
						break;
					}

					tempIslands.Add(island);
				}
			}

			SortIslands(rect, alphaWidth, alphaHeight);
		}

		private static void SortIslands(D2dRect rect, float alphaWidth, float alphaHeight)
		{
			switch (IslandLayout)
			{
				case Layout.Whole:
				{
					IslandsA.AddRange(tempIslands);
				}
				break;

				case Layout.Island:
				{
					if (rect.MinX == 0) rect.MinX -= 1;
					if (rect.MinY == 0) rect.MinY -= 1;
					if (rect.MaxX == alphaWidth ) rect.MaxX += 1;
					if (rect.MaxY == alphaHeight) rect.MaxY += 1;

					for (var i = tempIslands.Count - 1; i >= 0; i--)
					{
						var island = tempIslands[i];

						if (island.MinX > rect.MinX && island.MinY > rect.MinY && island.MaxX < rect.MaxX && island.MaxY < rect.MaxY)
						{
							IslandsA.Add(island);
						}
						else
						{
							IslandsB.Add(island);
						}
					}
				}
				break;

				case Layout.Horizontal:
				{
					for (var i = tempIslands.Count - 1; i >= 0; i--)
					{
						var island = tempIslands[i];

						if (island.MinX <= rect.MinX)
						{
							IslandsB.Add(island);
						}
						else if (island.MinX <= rect.MinX)
						{
							IslandsC.Add(island);
						}
						else
						{
							IslandsA.Add(island);
						}
					}
				}
				break;

				case Layout.Vertical:
				{
					for (var i = tempIslands.Count - 1; i >= 0; i--)
					{
						var island = tempIslands[i];

						if (island.MinY <= rect.MinY)
						{
							IslandsB.Add(island);
						}
						else if (island.MinY <= rect.MinY)
						{
							IslandsC.Add(island);
						}
						else
						{
							IslandsA.Add(island);
						}
					}
				}
				break;
			}

			tempIslands.Clear();
		}

		private static void AddToScan(List<Line> lines)
		{
			for (var i = lines.Count - 1; i >= 0; i--)
			{
				var line = lines[i];

				if (line.Used == false)
				{
					linkedLines.Add(line); line.Used = true;
				}
			}
		}

		private static void FastLinkLines(int min, int mid, int max)
		{
			for (var i = min; i < mid; i++)
			{
				var oldLine = lines[i];

				for (var j = mid; j < max; j++)
				{
					var newLine = lines[j];

					if (newLine.MinX < oldLine.MaxX && newLine.MaxX > oldLine.MinX)
					{
						oldLine.Ups.Add(newLine);
						newLine.Dns.Add(oldLine);
					}
				}
			}
		}

		private static int FastFindLines(byte[] alphaData, int alphaWidth, int y, int minX, int maxX)
		{
			var line   = default(Line);
			var count  = 0;
			var offset = alphaWidth * y;

			for (var x = minX; x < maxX; x++)
			{
				if (alphaData[offset + x] > 127)
				{
					// Start new line?
					if (line == null)
					{
						line = GetLine(); count += 1;
						line.MinX = line.MaxX = x;
						line.Y = y;
					}

					// Expand line
					line.MaxX += 1;
				}
				// Terminate line?
				else if (line != null)
				{
					line = null;
				}
			}

			return count;
		}

		private static Line GetLine()
		{
			var line = default(Line);

			if (lineCount >= lines.Count)
			{
				line = new Line();

				lines.Add(line);
			}
			else
			{
				line = lines[lineCount];

				line.Used = false;
				line.Ups.Clear();
				line.Dns.Clear();
			}

			lineCount += 1;

			return line;
		}
	}
}