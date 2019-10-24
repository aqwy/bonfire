using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dExplosion))]
	public class D2dExplosion_Editor : D2dEditor<D2dExplosion>
	{
		protected override void OnInspector()
		{
			DrawDefault("Mask");

			Separator();

			DrawDefault("Stamp");

			if (Any(t => t.Stamp == true))
			{
				BeginIndent();
					DrawDefault("StampTex");
					DrawDefault("StampSize");
					DrawDefault("StampHardness");
					DrawDefault("StampRandomDirection");
				EndIndent();
			}

			Separator();

			DrawDefault("Raycast");

			if (Any(t => t.Raycast == true))
			{
				BeginIndent();
					DrawDefault("RaycastRadius");
					DrawDefault("RaycastCount");
					DrawDefault("ForcePerRay");
					DrawDefault("DamagePerRay");
				EndIndent();
			}
		}
	}
}
#endif

namespace Destructible2D
{
	// This component will stamp and damage any nearby Destructibles, add physics forces to nearby rigidbody2Ds, and destroy the current GameObject after a set time
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Explosion")]
	public class D2dExplosion : MonoBehaviour
	{
		[Tooltip("The layers the explosion should work on")]
		public LayerMask Mask = -1;

		[Tooltip("Sould the explosion stamp a shape?")]
		public bool Stamp = true;

		[Tooltip("The shape of the stamp")]
		public Texture2D StampTex;

		[Tooltip("The size of the explosion stamp in world space")]
		public Vector2 StampSize = new Vector2(1.0f, 1.0f);
		
		[Tooltip("How hard the stamp is")]
		public float StampHardness = 1.0f;
		
		[Tooltip("Randomly rotate the stamp?")]
		public bool StampRandomDirection = true;
		
		[Tooltip("Should the explosion cast rays?")]
		public bool Raycast = true;
		
		[Tooltip("The size of the explosion raycast sphere")]
		public float RaycastRadius = 1.0f;

		[Tooltip("The amount of raycasts sent out")]
		public int RaycastCount = 32;
		
		[Tooltip("The amount of force added to objects that the raycasts hit")]
		public float ForcePerRay = 1.0f;
		
		[Tooltip("The amount of damage added to objects that the raycasts hit")]
		public float DamagePerRay = 1.0f;
		
		protected virtual void Start()
		{
			if (Stamp == true)
			{
				var stampPosition = transform.position;
				var stampAngle    = StampRandomDirection == true ? Random.Range(-180.0f, 180.0f) : 0.0f;
			
				D2dDestructible.StampAll(stampPosition, StampSize, stampAngle, StampTex, StampHardness, Mask);
			}

			if (Raycast == true && RaycastCount > 0)
			{
				var angleStep = 360.0f / RaycastCount;
				
				// Add damage?
				if (DamagePerRay != 0.0f)
				{
					for (var i = 0; i < RaycastCount; i++)
					{
						var angle     = i * angleStep;
						var direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
						var hit       = Physics2D.Raycast(transform.position, direction, RaycastRadius, Mask);
						var collider  = hit.collider;
					
						// Make sure the raycast hit something, and that it wasn't a trigger
						if (collider != null && collider.isTrigger == false)
						{
							var strength     = 1.0f - hit.fraction; // Do less damage if the hit point is far from the explosion
							var destructible = collider.GetComponentInParent<D2dDestructible>();
							
							if (destructible != null)
							{
								destructible.Damage += DamagePerRay * strength;
							}
						}
					}
				}

				// Add force?
				if (ForcePerRay != 0.0f)
				{
					for (var i = 0; i < RaycastCount; i++)
					{
						var angle     = i * angleStep;
						var direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
						var hit       = Physics2D.Raycast(transform.position, direction, RaycastRadius, Mask);
						var collider  = hit.collider;
					
						// Make sure the raycast hit something, and that it wasn't a trigger
						if (collider != null && collider.isTrigger == false)
						{
							var strength    = 1.0f - hit.fraction; // Do less damage if the hit point is far from the explosion
							var rigidbody2D = collider.attachedRigidbody;
							
							if (rigidbody2D != null)
							{
								var force = direction * ForcePerRay * strength;
								
								rigidbody2D.AddForceAtPosition(force, hit.point);
							}
						}
					}
				}
			}
		}
	}
}