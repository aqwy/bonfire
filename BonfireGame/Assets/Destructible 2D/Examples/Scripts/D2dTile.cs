using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dTile))]
	public class D2dTile_Editor : D2dEditor<D2dTile>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Size.x <= 0.0f || t.Size.y <= 0.0f));
				DrawDefault("Size");
			EndError();
			DrawDefault("Offset");
		}
	}
}
#endif

namespace Destructible2D
{
	// This component causes the current GameObject to follow the main camera on the x/y axis
	[ExecuteInEditMode]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Tile")]
	public class D2dTile : MonoBehaviour
	{
		[Tooltip("The size of this tile in local coordinates")]
		public Vector2 Size;
		
		[Tooltip("The position offset of this tile in local coordinates")]
		public D2dVector2 Offset;
		
		[System.NonSerialized]
		private Renderer mainRenderer;
		
		public void UpdatePosition(Vector2 offset)
		{
			// Main camera exists?
			var mainCamera = Camera.main;
			
			if (mainCamera != null)
			{
				// Valid size?
				if (Size.x > 0.0f && Size.y > 0.0f)
				{
					var position = transform.localPosition;
					var observer = mainCamera.transform.position - (Vector3)offset;
				
					position.x = Mathf.RoundToInt(observer.x / Size.x + Offset.X) * Size.x + offset.x;
					position.y = Mathf.RoundToInt(observer.y / Size.y + Offset.Y) * Size.y + offset.y;
				
					transform.localPosition = position;
				}
			}
		}
		
		public void UpdateRenderer(int sortingOrder)
		{
			if (mainRenderer == null) mainRenderer = GetComponent<Renderer>();
			
			if (mainRenderer != null)
			{
				mainRenderer.sortingOrder = sortingOrder;
			}
		}
		
		protected virtual void Update()
		{
			UpdatePosition(Vector3.zero);
		}
	}
}