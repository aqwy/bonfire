using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dEdgeCollider))]
	public class D2dEdgeCollider_Editor : D2dCollider_Editor<D2dEdgeCollider>
	{
		protected override void OnInspector()
		{
			var regenerate = false;

			DrawDefault("CellSize", ref regenerate);
			DrawDefault("Detail", ref regenerate);
			DrawDefault("Weld", ref regenerate);

			if (regenerate == true) DirtyEach(t => t.Regenerate());

			base.OnInspector();
		}
	}
}
#endif

namespace Destructible2D
{
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Edge Collider")]
	public class D2dEdgeCollider : D2dCollider
	{
		[Tooltip("The size of each collider cell")]
		[D2dPopup(8, 16, 32, 64, 128, 256)]
		public int CellSize = 64;

		[Tooltip("How many vertices should remain in the collider shapes")]
		[Range(0.5f, 1.0f)]
		public float Detail = 0.9f;

		[Tooltip("The minimum distance between vertices")]
		[Range(0.001f, 1.0f)]
		public float Weld = 0.01f;

		[SerializeField]
		private int expectedCellSize;

		[SerializeField]
		private int expectedWidth;

		[SerializeField]
		private int expectedHeight;

		[SerializeField]
		private int cellWidth;

		[SerializeField]
		private int cellHeight;

		[SerializeField]
		private D2dEdgeColliderCell[] cells;

		private static Stack<EdgeCollider2D> tempColliders = new Stack<EdgeCollider2D>();

		public override void UpdateColliderSettings()
		{
			if (cells != null)
			{
				for (var i = cells.Length - 1; i >= 0; i--)
				{
					var cell = cells[i];

					if (cell != null)
					{
						cell.UpdateColliderSettings(IsTrigger, Material);
					}
				}
			}
		}

		protected override void OnAlphaDataReplaced()
		{
			base.OnAlphaDataReplaced();
			
			Rebuild();
		}

		protected override void OnAlphaDataModified(D2dRect rect)
		{
			base.OnAlphaDataModified(rect);

			if (CellSize <= 0)
			{
				Mark(); Sweep(); return;
			}

			if (destructible.AlphaWidth != expectedWidth || destructible.AlphaHeight != expectedHeight || cells == null || cells.Length != cellWidth * cellHeight || CellSize != expectedCellSize)
			{
				Rebuild(); return;
			}

			var cellXMin = rect.MinX / CellSize;
			var cellYMin = rect.MinY / CellSize;
			var cellXMax = (rect.MaxX + 1) / CellSize;
			var cellYMax = (rect.MaxY + 1) / CellSize;

			cellXMin = Mathf.Clamp(cellXMin, 0, cellWidth  - 1);
			cellXMax = Mathf.Clamp(cellXMax, 0, cellWidth  - 1);
			cellYMin = Mathf.Clamp(cellYMin, 0, cellHeight - 1);
			cellYMax = Mathf.Clamp(cellYMax, 0, cellHeight - 1);

			for (var cellY = cellYMin; cellY <= cellYMax; cellY++)
			{
				var offset = cellY * cellWidth;

				for (var cellX = cellXMin; cellX <= cellXMax; cellX++)
				{
					var index = cellX + offset;

					RebuildCell(ref cells[index], cellX, cellY);
				}
			}

			Sweep();
		}

		protected override void OnAlphaDataSubset(D2dRect rect)
		{
			base.OnAlphaDataSubset(rect);

			Rebuild();
		}

		protected override void OnStartSplit()
		{
			base.OnStartSplit();
			
			Mark();
			Sweep();
		}

		private void Mark()
		{
			tempColliders.Clear();

			if (cells != null)
			{
				for (var i = cells.Length - 1; i >= 0; i--)
				{
					var cell = cells[i];

					if (cell != null)
					{
						cell.Clear(tempColliders);

						cells[i] = D2dEdgeColliderCell.Add(cell);
					}
				}
			}
		}

		private void Sweep()
		{
			while (tempColliders.Count > 0)
			{
				D2dHelper.Destroy(tempColliders.Pop());
			}
		}

		private void Rebuild()
		{
			Mark();
			{
				if (CellSize > 0)
				{
					expectedCellSize = CellSize;
					expectedWidth    = destructible.AlphaWidth;
					expectedHeight   = destructible.AlphaHeight;
					cellWidth        = (expectedWidth  + CellSize - 1) / CellSize;
					cellHeight       = (expectedHeight + CellSize - 1) / CellSize;
					cells            = new D2dEdgeColliderCell[cellWidth * cellHeight];

					for (var cellY = 0; cellY < cellHeight; cellY++)
					{
						var offset = cellY * cellWidth;

						for (var cellX = 0; cellX < cellWidth; cellX++)
						{
							RebuildCell(ref cells[cellX + offset], cellX, cellY);
						}
					}
				}
			}
			Sweep();
		}

		private void RebuildCell(ref D2dEdgeColliderCell cell, int cellX, int cellY)
		{
			var xMin = CellSize * cellX;
			var yMin = CellSize * cellY;
			var xMax = Mathf.Min(CellSize + xMin, destructible.AlphaWidth );
			var yMax = Mathf.Min(CellSize + yMin, destructible.AlphaHeight);

			if (xMin > 0) xMin += 1;
			if (yMin > 0) yMin += 1;
			if (xMax < destructible.AlphaWidth ) xMax -= 1;
			if (yMax < destructible.AlphaHeight) yMax -= 1;

			D2dColliderBuilder.AlphaData   = destructible.AlphaData;
			D2dColliderBuilder.AlphaWidth  = destructible.AlphaWidth;
			D2dColliderBuilder.AlphaHeight = destructible.AlphaHeight;
			D2dColliderBuilder.MinX        = xMin;
			D2dColliderBuilder.MinY        = yMin;
			D2dColliderBuilder.MaxX        = xMax;
			D2dColliderBuilder.MaxY        = yMax;

			D2dColliderBuilder.CalculateEdgeCells();

			if (cell == null)
			{
				cell = D2dEdgeColliderCell.Get();
			}

			cell.Clear(tempColliders);

			D2dColliderBuilder.BuildEdge(cell, tempColliders, child, Weld, Detail);

			cell.UpdateColliderSettings(IsTrigger, Material);
		}
	}
}