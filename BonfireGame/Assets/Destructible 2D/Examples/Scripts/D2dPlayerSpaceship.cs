using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dPlayerSpaceship))]
	public class D2dPlayerSpaceship_Editor : D2dEditor<D2dPlayerSpaceship>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.ShootDelay < 0.0f));
				DrawDefault("ShootDelay");
			EndError();
			DrawDefault("LeftGun");
			DrawDefault("RightGun");
			DrawDefault("LeftThruster");
			DrawDefault("RightThruster");
		}
	}
}
#endif

namespace Destructible2D
{
	// This component allows you to control a spaceship
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Player Spaceship")]
	public class D2dPlayerSpaceship : MonoBehaviour
	{
		[Tooltip("Minimum time between each shot in seconds")]
		public float ShootDelay = 0.1f;

		[Tooltip("The left gun")]
		public D2dGun LeftGun;

		[Tooltip("The right gun")]
		public D2dGun RightGun;

		[Tooltip("The left thruster")]
		public D2dThruster LeftThruster;
		
		[Tooltip("The right thruster")]
		public D2dThruster RightThruster;
		
		// Cached rigidbody of this spaceship
		[System.NonSerialized]
		private Rigidbody2D body;
		
		// Seconds until next shot is available
		[SerializeField]
		private float cooldown;

		protected virtual void Update()
		{
			cooldown -= Time.deltaTime;

			// Does the player want to shoot?
			if (Input.GetButton("Jump") == true)
			{
				// Can we shoot?
				if (cooldown <= 0.0f)
				{
					cooldown = ShootDelay;

					// Shoot left gun?
					if (LeftGun != null && LeftGun.CanShoot == true)
					{
						LeftGun.Shoot();
					}
					// Shoot right gun?
					else if (RightGun != null && RightGun.CanShoot == true)
					{
						RightGun.Shoot();
					}
				}
			}
			
			if (LeftThruster != null)
			{
				LeftThruster.Throttle = Input.GetAxisRaw("Vertical") + Input.GetAxisRaw("Horizontal") * 0.5f;
			}

			if (RightThruster != null)
			{
				RightThruster.Throttle = Input.GetAxisRaw("Vertical") - Input.GetAxisRaw("Horizontal") * 0.5f;
			}
		}
	}
}