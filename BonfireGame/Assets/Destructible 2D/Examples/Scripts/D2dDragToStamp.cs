using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dDragToStamp))]
	public class D2dDragToStamp_Editor : D2dEditor<D2dDragToStamp>
	{
		protected override void OnInspector()
		{
			DrawDefault("Requires");
			DrawDefault("Intercept");
			BeginError(Any(t => t.Layers == 0));
				DrawDefault("Layers");
			EndError();
			BeginError(Any(t => t.StampTex == null));
				DrawDefault("StampTex");
			EndError();
			BeginError(Any(t => t.Size.x == 0.0f || t.Size.y == 0.0f));
				DrawDefault("Size");
			EndError();
			BeginError(Any(t => t.Stretch <= 0.0f));
				DrawDefault("Stretch");
			EndError();
			BeginError(Any(t => t.Hardness == 0.0f));
				DrawDefault("Hardness");
			EndError();
			DrawDefault("Delay");
		}
	}
}
#endif

namespace Destructible2D
{
	// This component allows you to stamp all Destructibles under the mouse while down
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Drag To Slice")]
	public class D2dDragToStamp : MonoBehaviour
	{
		[Tooltip("The key you must hold down to do slicing")]
		public KeyCode Requires = KeyCode.Mouse0;

		[Tooltip("The z position the indicator should spawn at")]
		public float Intercept;

		[Tooltip("The GameObject layers this can stamp")]
		public LayerMask Layers = -1;

		[Tooltip("The shape of the stamp")]
		public Texture2D StampTex;

		[Tooltip("The size of the stamp")]
		public Vector2 Size = Vector2.one;

		[Tooltip("The stretch factorwhen connecting stamps")]
		public float Stretch = 1.0f;

		[Tooltip("How hard the stamp should be")]
		public float Hardness = 1.0f;

		[Tooltip("The delay between each repeat stamp")]
		public float Delay = 0.25f;

		private float cooldown;

		// Currently slicing?
		[SerializeField]
		private bool down;

		// Mouse position when slicing began
		[SerializeField]
		private Vector3 lastMousePosition;

		// Mouse movement angle
		[SerializeField]
		private float lastAngle;

		protected virtual void Update()
		{
			var mousePosition = Input.mousePosition;

			// Required key is down?
			if (Input.GetKey(Requires) == true)
			{
				if (down == true)
				{
					cooldown -= Time.deltaTime;

					if (cooldown <= 0.0f)
					{
						cooldown = Delay;

						Stamp(lastMousePosition, mousePosition);

						lastMousePosition = mousePosition;
					}
				}
				else
				{
					down     = true;
					cooldown = Delay;

					Stamp(mousePosition, mousePosition);

					lastMousePosition = mousePosition;
				}
			}
			else if (down == true)
			{
				down = false;

				if (mousePosition != lastMousePosition)
				{
					Stamp(lastMousePosition, mousePosition);
				}
			}
		}

		private void Stamp(Vector2 from, Vector2 to)
		{
			// Main camera exists?
			var mainCamera = Camera.main;

			if (mainCamera != null)
			{
				if (from != to)
				{
					var delta = to - from;

					lastAngle = -Mathf.Atan2(delta.x, delta.y) * Mathf.Rad2Deg;
				}

				var positionA = D2dHelper.ScreenToWorldPosition(from, Intercept, mainCamera);
				var positionB = D2dHelper.ScreenToWorldPosition(  to, Intercept, mainCamera);
				var positionM = (positionA + positionB) * 0.5f;
				var length    = Vector3.Distance(positionA, positionB) * Stretch;

				if (length < Size.y)
				{
					length = Size.y;
				}

				var size = new Vector2(Size.x, length);

				// Stamp at that point
				D2dDestructible.StampAll(positionM, size, lastAngle, StampTex, Hardness, Layers);
			}
		}
	}
}