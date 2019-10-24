using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dDragToSlice))]
	public class D2dDragToSlice_Editor : D2dEditor<D2dDragToSlice>
	{
		protected override void OnInspector()
		{
			DrawDefault("Requires");
			DrawDefault("Intercept");
			BeginError(Any(t => t.IndicatorPrefab == null));
				DrawDefault("IndicatorPrefab");
			EndError();
			BeginError(Any(t => t.StampTex == null));
				DrawDefault("StampTex");
			EndError();
			BeginError(Any(t => t.Hardness == 0.0f));
				DrawDefault("Hardness");
			EndError();
			BeginError(Any(t => t.Thickness == 0.0f));
				DrawDefault("Thickness");
			EndError();
		}
	}
}
#endif

namespace Destructible2D
{
	// This component allows you to slice all Destructibles under the mouse slice
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Drag To Slice")]
	public class D2dDragToSlice : MonoBehaviour
	{
		[Tooltip("The key you must hold down to do slicing")]
		public KeyCode Requires = KeyCode.Mouse0;

		[Tooltip("The z position the indicator should spawn at")]
		public float Intercept;

		[Tooltip("The prefab used to show what the slice will look like")]
		public GameObject IndicatorPrefab;

		[Tooltip("The shape of the slice when it stamps the Destructibles in the scene")]
		public Texture2D StampTex;

		[Tooltip("How hard the stamp should be")]
		public float Hardness = 1.0f;

		[Tooltip("The thickness of the slice line")]
		public float Thickness = 1.0f;

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
			// Get the main camera
			var mainCamera = Camera.main;

			// Begin dragging
			if (Input.GetKey(Requires) == true && down == false)
			{
				down               = true;
				startMousePosition = Input.mousePosition;
			}

			// End dragging
			if (Input.GetKey(Requires) == false && down == true)
			{
				down = false;

				// Main camera exists?
				if (mainCamera != null)
				{
					var endMousePosition = Input.mousePosition;
					var startPos         = D2dHelper.ScreenToWorldPosition(startMousePosition, Intercept, mainCamera);
					var endPos           = D2dHelper.ScreenToWorldPosition(  endMousePosition, Intercept, mainCamera);

					D2dDestructible.SliceAll(startPos, endPos, Thickness, StampTex, Hardness);
				}
			}

			// Update indicator?
			if (down == true && mainCamera != null && IndicatorPrefab != null)
			{
				if (indicatorInstance == null)
				{
					indicatorInstance = Instantiate(IndicatorPrefab);
				}

				var startPos   = D2dHelper.ScreenToWorldPosition( startMousePosition, Intercept, mainCamera);
				var currentPos = D2dHelper.ScreenToWorldPosition(Input.mousePosition, Intercept, mainCamera);
				var scale      = Vector3.Distance(currentPos, startPos);
				var angle      = D2dHelper.Atan2(currentPos - startPos) * Mathf.Rad2Deg;

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
	}
}