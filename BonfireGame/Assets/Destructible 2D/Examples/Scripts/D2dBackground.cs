using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dBackground))]
	public class D2dBackground_Editor : D2dEditor<D2dBackground>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Prefab == null));
				DrawDefault("Prefab");
			EndError();
			DrawDefault("TileAxis");
			DrawDefault("OffsetPerSecond");
			DrawDefault("Offset");
			DrawDefault("OverrideSorting");

			if (Any(t => t.OverrideSorting == true))
			{
				BeginIndent();
					DrawDefault("SortingOrder");
				EndIndent();
			}
		}
	}
}
#endif

namespace Destructible2D
{
	// This component automatically spawns tiles based on the main camera's orthographic size
	[ExecuteInEditMode]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Background")]
	public class D2dBackground : MonoBehaviour
	{
		public enum Axes
		{
			Horizontal,
			Vertical,
			HorizontalAndVertical
		}
		
		[Tooltip("The prefab used to render the background tiles")]
		public D2dTile Prefab;
		
		[Tooltip("The axes the background will tile across")]
		public Axes TileAxis = Axes.HorizontalAndVertical;
		
		[Tooltip("Scroll the background?")]
		public Vector2 OffsetPerSecond;
		
		[Tooltip("The current scrolling position")]
		public Vector2 Offset;
		
		[Tooltip("Override the sorting of all background renderers?")]
		public bool OverrideSorting;
		
		[Tooltip("The new sorting order")]
		public int SortingOrder;
		
		// The tiles that have been spawned by this component
		[SerializeField]
		private List<D2dTile> tiles;
		
		protected virtual void Update()
		{
			Offset += OffsetPerSecond * Time.deltaTime;
			
			UpdateTiles();
		}
		
		private void UpdateTiles()
		{
			var tileCount = 0;
			
			if (Prefab != null && Prefab.Size.x > 0.0f && Prefab.Size.y > 0.0f)
			{
				var mainCamera = Camera.main;
				
				if (mainCamera != null && mainCamera.orthographic == true)
				{
					var width  = Mathf.CeilToInt(mainCamera.orthographicSize * mainCamera.aspect / Prefab.Size.x);
					var height = Mathf.CeilToInt(mainCamera.orthographicSize                     / Prefab.Size.y);
					
					if (TileAxis == Axes.Horizontal) height = 0;
					if (TileAxis == Axes.Vertical  ) width  = 0;
					
					for (var y = -height; y <= height; y++)
					{
						for (var x = -width; x <= width; x++)
						{
							// Expand tile array?
							if (tileCount == tiles.Count)
							{
								tiles.Add(null);
							}
							
							// Get tile at this index
							var tile = tiles[tileCount];
							
							// Create tile?
							if (tile == null)
							{
								tile = Instantiate(Prefab);
								
								tile.enabled = false;
								
								tile.transform.SetParent(transform, false);
								
								tiles[tileCount] = tile;
							}
							
							if (OverrideSorting == true)
							{
								tile.UpdateRenderer(SortingOrder);
							}
							
							// Update this tile
							tile.Offset.X = x;
							tile.Offset.Y = y;
							
							tile.UpdatePosition(Offset);
							
							// Increment tile count
							tileCount += 1;
						}
					}
				}
			}
			
			// Remove unused tiles
			for (var i = tiles.Count - 1; i >= tileCount; i--)
			{
				var tile = tiles[i];
				
				if (tile != null)
				{
					D2dHelper.Destroy(tile.gameObject);
				}
				
				tiles.RemoveAt(i);
			}
		}
	}
}