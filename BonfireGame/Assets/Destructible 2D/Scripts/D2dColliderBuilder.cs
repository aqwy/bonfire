using UnityEngine;
using System.Collections.Generic;

namespace Destructible2D
{
	public static class D2dColliderBuilder
	{
		enum Edge
		{
			Left,
			Right,
			Bottom,
			Top
		}

		class Point
		{
			public bool    Used;
			public Vector2 Position;
			public Point   Other;
			public Cell    OppositeCell;
			public int     OppositeIndex;
		}

		class Cell
		{
			public Point[] Points = new Point[4];
		}

		private static List<Cell> cells = new List<Cell>();

		private static int cellCount;
		private static int cellsWidth;
		private static int cellsHeight;

		public static byte[] AlphaData;
		public static int    AlphaWidth;
		public static int    AlphaHeight;
		public static int    MinX;
		public static int    MaxX;
		public static int    MinY;
		public static int    MaxY;

		private static List<Point> points = new List<Point>();

		private static int pointCount;

		private static Cell cell;

		private static D2dLinkedList<Point> lines = new D2dLinkedList<Point>();

		private static byte GetPolyAlpha(int x, int y)
		{
			if (x >= MinX && x < MaxX)
			{
				if (y >= MinY && y < MaxY)
				{
					return AlphaData[x + y * AlphaWidth];
				}
			}

			return 0;
		}

		private static byte GetEdgeAlpha(int x, int y)
		{
			if (x >= 0 && x < AlphaWidth)
			{
				if (y >= 0 && y < AlphaHeight)
				{
					return AlphaData[x + y * AlphaWidth];
				}
			}

			return 0;
		}

		public static void CalculatePolyCells()
		{
			cellsWidth  = MaxX - MinX + 2;
			cellsHeight = MaxY - MinY + 2;
			pointCount  = 0;

			for (var i = cellsWidth * cellsHeight - cells.Count; i > 0; i--) cells.Add(new Cell());

			for (var y = MinY - 1; y <= MaxY; y++)
			{
				var offset = (y - MinY + 1) * cellsWidth - MinX + 1;

				for (var x = MinX - 1; x <= MaxX; x++)
				{
					var bl    = GetPolyAlpha(x    , y    ); var useBl = bl >= 128;
					var br    = GetPolyAlpha(x + 1, y    ); var useBr = br >= 128;
					var tl    = GetPolyAlpha(x    , y + 1); var useTl = tl >= 128;
					var tr    = GetPolyAlpha(x + 1, y + 1); var useTr = tr >= 128;
					var mask  = (useBl ? 1 : 0) + (useBr ? 2 : 0) + (useTl ? 4 : 0) + (useTr ? 8 : 0);
					var index = offset + x;

					cell = cells[index];

					if (mask > 0 && mask < 15)
					{
						var cornerX = x + 0.5f;
						var cornerY = y + 0.5f;

						if (useBl ^ useBr) Build(0, cornerX + (bl - 128) / (float)(bl - br), cornerY);
						if (useTl ^ useTr) Build(1, cornerX + (tl - 128) / (float)(tl - tr), cornerY + 1.0f);
						if (useBl ^ useTl) Build(2, cornerX, cornerY + (bl - 128) / (float)(bl - tl));
						if (useBr ^ useTr) Build(3, cornerX + 1.0f, cornerY + (br - 128) / (float)(br - tr));

						switch (mask)
						{
							case 1: case 14: Link(0, 2, index - cellsWidth, index - 1); break; // BL
							case 2: case 13: Link(0, 3, index - cellsWidth, index + 1); break; // BR
							case 4: case 11: Link(1, 2, index + cellsWidth, index - 1); break; // TL
							case 8: case 7: Link(1, 3, index + cellsWidth, index + 1); break; // TR
							case 3: case 12: Link(2, 3, index - 1, index + 1); break; // B/T
							case 5: case 10: Link(0, 1, index - cellsWidth, index + cellsWidth); break; // L/R
							case 6: Link(1, 2, index + cellsWidth, index - 1); Link(0, 3, index - cellsWidth, index + 1); break; // TL/BR
							case 9: Link(0, 2, index - cellsWidth, index - 1); Link(1, 3, index + cellsWidth, index + 1); break; // BL/TR
						}
					}
				}
			}
		}

		public static void CalculateEdgeCells()
		{
			cellsWidth  = MaxX - MinX + 2;
			cellsHeight = MaxY - MinY + 2;
			pointCount  = 0;

			for (var i = cellsWidth * cellsHeight - cells.Count; i > 0; i--) cells.Add(new Cell());

			for (var y = MinY - 1; y <= MaxY; y++)
			{
				var offset = (y - MinY + 1) * cellsWidth - MinX + 1;

				for (var x = MinX - 1; x <= MaxX; x++)
				{
					var bl    = GetEdgeAlpha(x    , y    ); var useBl = bl >= 128;
					var br    = GetEdgeAlpha(x + 1, y    ); var useBr = br >= 128;
					var tl    = GetEdgeAlpha(x    , y + 1); var useTl = tl >= 128;
					var tr    = GetEdgeAlpha(x + 1, y + 1); var useTr = tr >= 128;
					var mask  = (useBl ? 1 : 0) + (useBr ? 2 : 0) + (useTl ? 4 : 0) + (useTr ? 8 : 0);
					var index = offset + x;

					cell = cells[index];

					if (mask > 0 && mask < 15)
					{
						var cornerX = x + 0.5f;
						var cornerY = y + 0.5f;

						if (useBl ^ useBr) Build(0, cornerX + (bl - 128) / (float)(bl - br), cornerY);
						if (useTl ^ useTr) Build(1, cornerX + (tl - 128) / (float)(tl - tr), cornerY + 1.0f);
						if (useBl ^ useTl) Build(2, cornerX, cornerY + (bl - 128) / (float)(bl - tl));
						if (useBr ^ useTr) Build(3, cornerX + 1.0f, cornerY + (br - 128) / (float)(br - tr));

						var cx = x - MinX + 1;
						var cy = y - MinY + 1;

						switch (mask)
						{
							case 1: case 14: Link(0, 2, cx, cy - 1, cx - 1, cy); break; // BL
							case 2: case 13: Link(0, 3, cx, cy - 1, cx + 1, cy); break; // BR
							case 4: case 11: Link(1, 2, cx, cy + 1, cx - 1, cy); break; // TL
							case 8: case 7: Link(1, 3, cx, cy + 1, cx + 1, cy); break; // TR
							case 3: case 12: Link(2, 3, cx - 1, cy, cx + 1, cy); break; // B/T
							case 5: case 10: Link(0, 1, cx, cy - 1, cx, cy + 1); break; // L/R
							case 6: Link(1, 2, cx, cy + 1, cx - 1, cy); Link(0, 3, cx, cy - 1, cx + 1, cy); break; // TL/BR
							case 9: Link(0, 2, cx, cy - 1, cx - 1, cy); Link(1, 3, cx, cy + 1, cx + 1, cy); break; // BL/TR
						}
					}
				}
			}
		}

		private static readonly int[] opposite = new int[] { 1, 0, 3, 2 };

		private static void Build(int index, float x, float y)
		{
			var point = cell.Points[index] = GetNextPoint();

			point.Position.x = x;
			point.Position.y = y;
			point.OppositeCell = null;
		}

		private static void Link(int edgeA, int edgeB, int xa, int ya, int xb, int yb)
		{
			var pointA = cell.Points[edgeA];
			var pointB = cell.Points[edgeB];

			pointA.Other = pointB;
			pointB.Other = pointA;

			if (xa >= 0 && xa < cellsWidth && ya >= 0 && ya < cellsHeight)
			{
				pointA.OppositeCell  = cells[xa + ya * cellsWidth];
				pointA.OppositeIndex = opposite[edgeA];
			}

			if (xb >= 0 && xb < cellsWidth && yb >= 0 && yb < cellsHeight)
			{
				pointB.OppositeCell  = cells[xb + yb * cellsWidth];
				pointB.OppositeIndex = opposite[edgeB];
			}
		}

		private static void Link(int edgeA, int edgeB, int indexA, int indexB)
		{
			var pointA = cell.Points[edgeA];
			var pointB = cell.Points[edgeB];

			pointA.Other = pointB;
			pointB.Other = pointA;

			pointA.OppositeCell = cells[indexA];
			pointB.OppositeCell = cells[indexB];

			pointA.OppositeIndex = opposite[edgeA];
			pointB.OppositeIndex = opposite[edgeB];
		}

		public static void BuildPoly(D2dPolygonColliderCell cell, Stack<PolygonCollider2D> tempColliders, GameObject child, float weld, float detail)
		{
			// Find the start points of paths
			for (var i = 0; i < pointCount; i++)
			{
				var point = points[i];

				// Build polygon from point
				if (point.Used == false)
				{
					Trace(point);
					WeldLines(weld);
					OptimizeEdges(detail);

					cell.AddPolygon(tempColliders, child, ExtractPoints(1));
				}
			}
		}

		public static void BuildEdge(D2dEdgeColliderCell cell, Stack<EdgeCollider2D> tempColliders, GameObject child, float weld, float detail)
		{
			// Find the start points of paths
			for (var i = 0; i < pointCount; i++)
			{
				var point = points[i];

				// Build path from point
				if (point.Used == false)
				{
					Trace(point);
					WeldLines(weld);
					OptimizeEdges(detail);

					cell.AddPath(tempColliders, child, ExtractPoints(0));
				}
			}
		}

		private static void WeldLines(float threshold)
		{
			if (lines.Count > 2)
			{
				var a = lines.First;
				var b = a.Next;
				var c = b.Next;
				var v = b.Value.Position - a.Value.Position;

				while (c != null)
				{
					var n = c.Value.Position - b.Value.Position;
					var z = n - v;

					if (z.sqrMagnitude < threshold)
					{
						lines.Remove(b);
					}
					else
					{
						v = n;
					}

					b = c;
					c = b.Next;
				}
			}
		}

		private static void OptimizeEdges(float detail)
		{
			if (detail < 1.0f && lines.Count > 2)
			{
				var a = lines.First;
				var b = a.Next;
				var c = b.Next;

				while (c != lines.Last)
				{
					var av  = a.Value.Position;
					var bv  = b.Value.Position;
					var cv  = c.Value.Position;
					var ab  = Vector3.Normalize(bv - av);
					var bc  = Vector3.Normalize(cv - bv);
					var abc = Vector3.Dot(ab, bc);

					if (abc > detail)
					{
						lines.Remove(b);

						b = c;
						c = c.Next;
					}
					else
					{
						a = b;
						b = c;
						c = c.Next;
					}
				}
			}
		}

		public static Vector2[] ExtractPoints(int skip)
		{
			var count = lines.Count - skip;

			if (count > 1)
			{
				var array = new Vector2[count];
				var node  = lines.First;
				var index = 0;

				while (index < count)
				{
					array[index] = node.Value.Position;

					node  = node.Next;
					index += 1;
				}

				return array;
			}

			return null;
		}

		private static void Trace(Point point)
		{
			lines.Clear();

			lines.AddFirst(point);

			while (point.Used == false)
			{
				var other = point.Other;

				lines.AddLast(other);

				point.Used = true;
				other.Used = true;

				if (other.OppositeCell == null)
				{
					return;
				}

				point = other.OppositeCell.Points[other.OppositeIndex];
			}
		}

		private static Point GetNextPoint()
		{
			var point = default(Point);

			if (pointCount >= points.Count)
			{
				point = new Point(); points.Add(point);
			}
			else
			{
				point = points[pointCount];
				point.Used = false;
			}

			pointCount += 1;

			return point;
		}
	}
}