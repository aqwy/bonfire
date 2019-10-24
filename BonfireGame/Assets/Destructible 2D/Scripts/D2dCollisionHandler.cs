using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dCollisionHandler))]
	public class D2dCollisionHandler_Editor : D2dEditor<D2dCollisionHandler>
	{
		private bool showEvents;

		protected override void OnInspector()
		{
			DrawDefault("DebugCollisions");

			Separator();

			DrawDefault("UseFirstOnly");
			DrawDefault("ImpactMask");
			DrawDefault("ImpactThreshold");

			BeginError(Any(t => t.ImpactDelay < 0.0f));
				DrawDefault("ImpactDelay");
			EndError();

			Separator();

			DrawDefault("DamageOnImpact");

			if (Any(t => t.DamageOnImpact == true))
			{
				BeginIndent();
					DrawDefault("DamageDestructible");
					DrawDefault("DamageScale");
				EndIndent();
			}
			
			Separator();

			showEvents = EditorGUI.Foldout(D2dHelper.Reserve(), showEvents, "Events");

			if (showEvents == true)
			{
				DrawDefault("OnImpact");
			}
		}
	}
}
#endif

namespace Destructible2D
{
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Collision Handler")]
	public class D2dCollisionHandler : MonoBehaviour
	{
		[Tooltip("Output debug information about collisions?")]
		public bool DebugCollisions;
		
		[Tooltip("The layers that must hit this collider for damage to get inflicted")]
		public LayerMask ImpactMask = -1;

		[Tooltip("This amount of damage required to register an impact")]
		public float ImpactThreshold = 1.0f;
		
		[Tooltip("The minimum amount of seconds between each impact")]
		public float ImpactDelay;

		[Tooltip("Should this collider inflict damage on the Destructible when it takes impact?")]
		public bool DamageOnImpact;
		
		[Tooltip("The destructible that takes the damage")]
		public D2dDestructible DamageDestructible;

		[Tooltip("The damage will be scaled by this value after it passes the ImpactThreshold")]
		public float DamageScale = 1.0f;

		[Tooltip("If an impact passes the impact threshold, ignore any other impacts? (can happen when complex shapes hit this)")]
		public bool UseFirstOnly = true;
		
		public D2dVector2Event OnImpact;

		private float cooldownTime;

		protected virtual void OnCollisionEnter2D(Collision2D collision)
		{
			if (DebugCollisions == true)
			{
				Debug.Log(name + " hit " + collision.collider.name + " for " + collision.relativeVelocity.magnitude);
			}

			if (ImpactDelay > 0.0f)
			{
				if (Time.time >= cooldownTime)
				{
					cooldownTime = Time.time + ImpactDelay;
				}
				else
				{
					return;
				}
			}

			var collisionMask = 1 << collision.collider.gameObject.layer;

			if ((collisionMask & ImpactMask) != 0)
			{
				var contacts = collision.contacts;

				for (var i = contacts.Length - 1; i >= 0; i--)
				{
					var contact = contacts[i];
					var impact  = collision.relativeVelocity.magnitude;

					if (impact >= ImpactThreshold)
					{
						if (DamageOnImpact == true)
						{
							if (DamageDestructible == null) DamageDestructible = GetComponentInChildren<D2dDestructible>();

							if (DamageDestructible != null)
							{
								DamageDestructible.Damage += impact * DamageScale;
							}
						}

						if (OnImpact != null) OnImpact.Invoke(contact.point);

						if (UseFirstOnly == true)
						{
							break;
						}
					}
				}
			}
		}
	}
}