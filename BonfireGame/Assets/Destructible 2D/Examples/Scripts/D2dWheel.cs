using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dWheel))]
	public class D2dWheel_Editor : D2dEditor<D2dWheel>
	{
		protected override void OnInspector()
		{
			DrawDefault("Speed");
			DrawDefault("GripDampening");
			DrawDefault("Friction");
		}
	}
}
#endif

namespace Destructible2D
{
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Wheel")]
	public class D2dWheel : MonoBehaviour
	{
		[Tooltip("The current rotational speed of the wheel")]
		public float Speed;

		[Tooltip("How quickly the wheel matches the ground speed")]
		public float GripDampening = 1.0f;

		[Tooltip("How quickly the wheels slow down")]
		public float Friction = 0.1f;
		
		// Has oldPosition been set?
		[SerializeField]
		private bool oldPositionSet;

		// Stores the old position
		[SerializeField]
		private Vector2 oldPosition;

		// The rigidbody this wheel is attached to
		[System.NonSerialized]
		private Rigidbody2D body;

		public void AddTorque(float amount)
		{
			Speed += amount;
		}

		protected virtual void FixedUpdate()
		{
			if (body == null) body = GetComponentInParent<Rigidbody2D>();

			if (body != null)
			{
				if (oldPositionSet == false)
				{
					oldPositionSet = true;
					oldPosition    = transform.position;
				}

				var newPosition   = (Vector2)transform.position;
				var deltaPosition = newPosition - oldPosition;
				var deltaSpeed    = deltaPosition.magnitude / Time.fixedDeltaTime;
				var expectedSpeed = deltaSpeed * Vector2.Dot(transform.up, body.transform.up);

				oldPosition = newPosition;

				// Match ground speed
				Speed = D2dHelper.Dampen(Speed, expectedSpeed, GripDampening, Time.fixedDeltaTime);
			
				// Apply speed difference
				var deltaWheel = (Vector2)transform.up * Speed * Time.fixedDeltaTime;

				body.AddForceAtPosition(deltaWheel - deltaPosition, transform.position, ForceMode2D.Impulse);
			
				// Slow wheel down
				Speed = D2dHelper.Dampen(Speed, 0.0f, Friction, Time.fixedDeltaTime);
			}
		}
	}
}