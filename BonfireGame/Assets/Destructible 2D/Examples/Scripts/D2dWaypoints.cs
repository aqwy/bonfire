using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dWaypoints))]
	public class D2dWaypoints_Editor : D2dEditor<D2dWaypoints>
	{
		protected override void OnInspector()
		{
			DrawDefault("Acceleration");
			DrawDefault("MaximumSpeed");
			DrawDefault("SpeedBoost");
			DrawDefault("MinimumDistance");
			BeginError(Any(t => t.Points == null || t.Points.Length == 0));
				DrawDefault("Points");
			EndError();
		}
	}
}
#endif

namespace Destructible2D
{
	// This component automatically moves the current GameObject between waypoints
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Waypoints")]
	public class D2dWaypoints : MonoBehaviour
	{
		[Tooltip("The rate at which this GameObject accelerates toward its current target")]
		public float Acceleration = 5.0f;

		[Tooltip("The maximum speed at which this GameObject can move toward its current target")]
		public float MaximumSpeed = 2.0f;

		[Tooltip("The extra acceleration given to stop this gameObject orbiting its target")]
		public float SpeedBoost = 2.0f;

		[Tooltip("If this gameObject gets within this distance of its current target then it will switch target")]
		public float MinimumDistance = 1.0f;

		[Tooltip("The  points this GameObject will randomly move between")]
		public Vector2[] Points;

		[SerializeField]
		private Vector2 targetPoint;
		
		[System.NonSerialized]
		private Rigidbody2D body;

		protected virtual void Awake()
		{
			ChangeTargetPoint();
        }

		protected virtual void FixedUpdate()
		{
			var currentPoint = (Vector2)transform.position;
			var vector       = targetPoint - currentPoint;

			if (vector.magnitude <= MinimumDistance)
			{
				ChangeTargetPoint();

				vector = targetPoint - currentPoint;
			}

			// Limit target speed
			if (vector.magnitude > MaximumSpeed)
			{
				vector = vector.normalized * MaximumSpeed;
			}
			
			// Acceleration
			if (body == null) body = GetComponent<Rigidbody2D>();

			body.velocity = D2dHelper.Dampen2(body.velocity, vector * SpeedBoost, Acceleration, Time.deltaTime);
		}

		private void ChangeTargetPoint()
		{
			if (Points != null && Points.Length > 0)
			{
				var newIndex = Random.Range(0, Points.Length);

				targetPoint = Points[newIndex];
			}
		}

#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			if (Points != null)
			{
				foreach (var point in Points)
				{
					Gizmos.DrawLine(point, transform.position);
				}
			}
		}
#endif
	}
}