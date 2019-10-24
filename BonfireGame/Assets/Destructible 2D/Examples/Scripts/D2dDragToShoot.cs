using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dDragToShoot))]
	public class D2dDragToShoot_Editor : D2dEditor<D2dDragToShoot>
	{
		protected override void OnInspector()
		{
			DrawDefault("Requires");
			DrawDefault("Intercept");
			BeginError(Any(t => t.IndicatorPrefab == null));
				DrawDefault("IndicatorPrefab");
			EndError();
			BeginError(Any(t => t.ImpactPrefab == null));
				DrawDefault("ImpactPrefab");
			EndError();
			DrawDefault("Thickness");
			DrawDefault("Range");
		}
	}
}
#endif

namespace Destructible2D
{
	// This component allows you to slice all Destructibles under the mouse slice
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Drag To Shoot")]
	public class D2dDragToShoot : MonoBehaviour
	{
		[Tooltip("The key you must hold down to do slicing")]
		public KeyCode Requires = KeyCode.Mouse0;

		[Tooltip("The z position the indicator should spawn at")]
		public float Intercept;

		[Tooltip("The prefab used to show what the slice will look like")]
		public GameObject IndicatorPrefab;

		[Tooltip("The prefab spawned at the impact point of the ray")]
		public GameObject ImpactPrefab;

		[Tooltip("The thickness of the indicator")]
		public float Thickness = 1.0f;

		[Tooltip("The max range the shot")]
		public float Range = 10.0f;

		// Currently slicing?
		[SerializeField]
		private bool down;

		// Mouse position when slicing began
		[SerializeField]
		private Vector3 startMousePosition;

		// Instance of the indicator
		[SerializeField]
		private GameObject indicatorInstance;

		protected virtual void Update()
		{
			// Get main camera
			var mainCamera = Camera.main;

			// Begin dragging
			if (Input.GetKey(Requires) == true && down == false)
			{
				down               = true;
				startMousePosition = Input.mousePosition;
			}

			// Dragging finger released?
			if (Input.GetKey(Requires) == false && down == true)
			{
				down = false;

				// Impact prefab exists?
				if (ImpactPrefab != null)
				{
					// Main camera exists?
					if (mainCamera != null)
					{
						// Find start and end world points
						var startPos = D2dHelper.ScreenToWorldPosition( startMousePosition, Intercept, mainCamera);
						var endPos   = D2dHelper.ScreenToWorldPosition(Input.mousePosition, Intercept, mainCamera);

						// Extend end pos to raycast hit
						CalculateEndPos(startPos, ref endPos);

						// Spawn explosion there
						Instantiate(ImpactPrefab, endPos, Quaternion.identity);
					}
				}
			}

			// Update indicator?
			if (down == true && mainCamera != null && IndicatorPrefab != null)
			{
				if (indicatorInstance == null)
				{
					indicatorInstance = Instantiate(IndicatorPrefab);
				}

				var startPos = D2dHelper.ScreenToWorldPosition( startMousePosition, Intercept, mainCamera);
				var endPos   = D2dHelper.ScreenToWorldPosition(Input.mousePosition, Intercept, mainCamera); CalculateEndPos(startPos, ref endPos);
				var scale    = Vector3.Distance(endPos, startPos);
				var angle    = D2dHelper.Atan2(endPos - startPos) * Mathf.Rad2Deg;

				// Transform the indicator so it lines up with the slice
				indicatorInstance.transform.position   = startPos;
				indicatorInstance.transform.rotation   = Quaternion.Euler(0.0f, 0.0f, -angle);
				indicatorInstance.transform.localScale = new Vector3(Thickness, scale, scale);
			}
			// Destroy indicator?
			else if (indicatorInstance != null)
			{
				Destroy(indicatorInstance.gameObject);
			}
		}

		private void CalculateEndPos(Vector3 startPos, ref Vector3 endPos)
		{
			// Find ray cast point in front of start pos toward end pos
			var vec      = endPos - startPos;
			var hit      = Physics2D.Raycast(startPos, vec.normalized, Range);
			var distance = Range;

			// If the ray hit something, set the hit distance
			if (hit.collider != null)
			{
				distance = hit.distance;
			}

			// Adjust end pos so it's at the correct distance
			endPos = startPos + vec.normalized * distance;
		}
	}
}