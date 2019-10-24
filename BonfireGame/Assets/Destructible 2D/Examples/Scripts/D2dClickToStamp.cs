using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dClickToStamp))]
	public class D2dClickToStamp_Editor : D2dEditor<D2dClickToStamp>
	{
		protected override void OnInspector()
		{
			DrawDefault("Requires");
			BeginError(Any(t => t.Layers == 0));
				DrawDefault("Layers");
			EndError();
			BeginError(Any(t => t.StampTex == null));
				DrawDefault("StampTex");
			EndError();
			BeginError(Any(t => t.Size.x == 0.0f || t.Size.y == 0.0f));
				DrawDefault("Size");
			EndError();
			DrawDefault("Angle");
			BeginError(Any(t => t.Hardness == 0.0f));
				DrawDefault("Hardness");
			EndError();
		}
	}
}
#endif

namespace Destructible2D
{
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Click To Stamp")]
	public class D2dClickToStamp : MonoBehaviour
	{
		[Tooltip("The key you must hold down to stamp")]
		public KeyCode Requires = KeyCode.Mouse0;

		[Tooltip("The z position the stamps should spawn at")]
		public float Intercept;

		[Tooltip("The GameObject layers this can stamp")]
		public LayerMask Layers = -1;

		[Tooltip("The shape of the stamp")]
		public Texture2D StampTex;

		[Tooltip("The size of the stamp")]
		public Vector2 Size = Vector2.one;

		[Tooltip("The angle of the stamp")]
		public float Angle;

		[Tooltip("The hardness of the stamp")]
		public float Hardness = 1.0f;

		protected virtual void Update()
		{
			// Required key is down?
			if (Input.GetKeyDown(Requires) == true)
			{
				// Main camera exists?
				var mainCamera = Camera.main;

				if (mainCamera != null)
				{
					// World position of the mouse
					var position = D2dHelper.ScreenToWorldPosition(Input.mousePosition, Intercept, mainCamera);

					// Stamp at that point
					D2dDestructible.StampAll(position, Size, Angle, StampTex, Hardness, Layers);
				}
			}
		}
	}
}