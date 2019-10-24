using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dPlayerCamera))]
	public class D2dPlayerCamera_Editor : D2dEditor<D2dPlayerCamera>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Speed <= 0.0f));
				DrawDefault("Speed");
			EndError();
			BeginError(Any(t => t.Acceleration <= 0.0f));
				DrawDefault("Acceleration");
			EndError();
		}
	}
}
#endif

namespace Destructible2D
{
	// This component causes the current GameObject to follow the target Transform
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Player Camera")]
	public class D2dPlayerCamera : MonoBehaviour
	{
		[Tooltip("How quickly the camera can move per second")]
		public float Speed = 1.0f;

		[Tooltip("How quickly the camera moves to its target location")]
		public float Acceleration = 2.0f;

		[SerializeField]
		private Vector2 velocity;

		protected virtual void Update()
		{
			var h = Input.GetAxisRaw("Horizontal");
			var v = Input.GetAxisRaw("Vertical");

			velocity.x += h * Speed * Time.deltaTime;
			velocity.y += v * Speed * Time.deltaTime;

			velocity = D2dHelper.Dampen2(velocity, Vector2.zero, Acceleration, Time.deltaTime);

			transform.Translate(velocity);
		}
	}
}
