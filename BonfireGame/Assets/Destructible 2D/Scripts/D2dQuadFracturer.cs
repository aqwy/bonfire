using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dQuadFracturer))]
	public class D2dQuadFracturer_Editor : D2dEditor<D2dQuadFracturer>
	{
		protected override void OnInspector()
		{
			DrawDefault("RequiredDamage");
			
			BeginError(Any(t => t.RequiredDamageMultiplier <= 1.0f));
				DrawDefault("RequiredDamageMultiplier");
			EndError();
			
			DrawDefault("FractureCount");
			
			BeginError(Any(t => t.FractureCountMultiplier <= 0.0f));
				DrawDefault("FractureCountMultiplier");
			EndError();
			
			Separator();
			
			DrawDefault("RemainingFractures");
			DrawDefault("Irregularity");
		}
	}
}
#endif

namespace Destructible2D
{
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Quad Fracturer")]
	public class D2dQuadFracturer : D2dFracturer
	{
		[Tooltip("This changes how random the fractured shapes will be")]
		[Range(0.0f, 0.5f)]
		public float Irregularity = 0.25f;
		
		// Temp fracture vars
		private static List<D2dQuad>    quads  = new List<D2dQuad>();
		private static List<D2dVector2> points = new List<D2dVector2>();
		private static int quadCount;
		private static int pointCount;
		private static int xMin;
		private static int xMax;
		private static int yMin;
		private static int yMax;
		
		[ContextMenu("Fracture")]
		public override void Fracture()
		{
			base.Fracture();
			
			Fracture(destructible, FractureCount, Irregularity);
		}
		
		public static void Fracture(D2dDestructible destructible, int count, float irregularity)
		{
			if (destructible != null && count > 0)
			{
				D2dSplitGroup.ClearAll();
				{
					var width    = destructible.AlphaWidth;
					var height   = destructible.AlphaHeight;
					var mainQuad = new D2dQuad();

					quadCount  = 1;
					pointCount = 0;
					xMin       = 0;
					xMax       = width - 1;
					yMin       = 0;
					yMax       = height - 1;

					mainQuad.BL = new D2dVector2(xMin, yMin);
					mainQuad.BR = new D2dVector2(xMax, yMin);
					mainQuad.TL = new D2dVector2(xMin, yMax);
					mainQuad.TR = new D2dVector2(xMax, yMax);
					mainQuad.Calculate();

					if (quads.Count > 0)
					{
						quads[0] = mainQuad;
					}
					else
					{
						quads.Add(mainQuad);
					}

					for (var i = 0; i < count; i++)
					{
						SplitLargest();
					}

					if (irregularity > 0.0f)
					{
						FindPoints();
						ShiftPoints(irregularity);
					}

					for (var i = 0; i < quadCount; i++)
					{
						var quad  = quads[i];
						var group = D2dSplitGroup.GetSplitGroup();
					
						group.AddTriangle(quad.BL, quad.BR, quad.TL);
						group.AddTriangle(quad.TR, quad.TL, quad.BR);
					}

					destructible.SplitWhole(D2dSplitGroup.SplitGroups);
				}
				D2dSplitGroup.ClearAll();
			}
		}
		
		private static void FindPoints()
		{
			for (var i = 0; i < quadCount; i++)
			{
				var quad = quads[i];
				
				TryAddPoint(quad.BL);
				TryAddPoint(quad.BR);
				TryAddPoint(quad.TL);
				TryAddPoint(quad.TR);
			}
		}
		
		private static void ShiftPoints(float irregularity)
		{
			for (var i = 0; i < pointCount; i++)
			{
				var point  = points[i];
				var delta  = Random.insideUnitCircle.normalized * FindMaxMovement(point.X, point.Y) * irregularity;
				var deltaX = Mathf.RoundToInt(delta.x);
				var deltaY = Mathf.RoundToInt(delta.y);
				
				if (point.X <= xMin || point.X >= xMax) deltaX = 0;
				if (point.Y <= yMin || point.Y >= yMax) deltaY = 0;
				
				if (deltaX != 0 || deltaY != 0)
				{
					var newPoint = new D2dVector2(point.X + deltaX, point.Y + deltaY);
					
					MovePoints(point, newPoint);
					
					points[i] = newPoint;
				}
			}
		}
		
		private static void MovePoints(D2dVector2 oldPoint, D2dVector2 newPoint)
		{
			for (var i = 0; i < quadCount; i++)
			{
				var quad = quads[i];
				
				TryMovePoint(ref quad.BL, oldPoint, newPoint);
				TryMovePoint(ref quad.BR, oldPoint, newPoint);
				TryMovePoint(ref quad.TL, oldPoint, newPoint);
				TryMovePoint(ref quad.TR, oldPoint, newPoint);
				
				quads[i] = quad;
			}
		}
		
		private static void TryMovePoint(ref D2dVector2 point, D2dVector2 oldPoint, D2dVector2 newPoint)
		{
			if (point.X == oldPoint.X && point.Y == oldPoint.Y)
			{
				point.X = newPoint.X;
				point.Y = newPoint.Y;
			}
		}
		
		private static void TryAddPoint(D2dVector2 newPoint)
		{
			for (var i = 0; i < pointCount; i++)
			{
				var point = points[i];
				
				if (point.X == newPoint.X && point.Y == newPoint.Y)
				{
					return;
				}
			}
			
			if (points.Count > pointCount)
			{
				points[pointCount] = newPoint;
			}
			else
			{
				points.Add(newPoint);
			}
			
			pointCount += 1;
		}
		
		private static int FindMaxMovement(int x, int y)
		{
			var minDistanceSq = int.MaxValue;
			
			for (var i = 0; i < pointCount; i++)
			{
				var point      = points[i];
				var distanceX  = point.X - x;
				var distanceY  = point.Y - y;
				var distanceSq = distanceX * distanceX + distanceY * distanceY;
				
				if (distanceSq > 0)
				{
					minDistanceSq = Mathf.Min(minDistanceSq, distanceSq);
				}
			}
			
			return Mathf.FloorToInt(Mathf.Sqrt(minDistanceSq));
		}
		
		private static void SplitLargest()
		{
			var largestIndex = 0;
			var largestArea  = 0;
			
			for (var i = 0; i < quadCount; i++)
			{
				var quad = quads[i];
				
				if (quad.Area > largestArea)
				{
					largestIndex = i;
					largestArea  = quad.Area;
				}
			}
			
			var first  = new D2dQuad();
			var second = new D2dQuad();
			
			quads[largestIndex].Split(ref first, ref second);
			
			quads[largestIndex] = first;
			
			if (quads.Count > quadCount)
			{
				quads[quadCount] = second;
			}
			else
			{
				quads.Add(second);
			}
			
			quadCount += 1;
		}
	}
}