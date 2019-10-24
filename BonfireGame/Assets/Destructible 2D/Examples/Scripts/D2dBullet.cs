using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dBullet))]
	public class D2dBullet_Editor : D2dEditor<D2dBullet>
	{
		protected override void OnInspector()
		{
			DrawDefault("IgnoreTag");
			DrawDefault("RaycastMask");
			DrawDefault("ExplosionPrefab");
			DrawDefault("Speed");
			DrawDefault("MaxLength");
			DrawDefault("MaxScale");
		}
	}
}
#endif

namespace Destructible2D
{
	[ExecuteInEditMode]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Bullet")]
	public class D2dBullet : MonoBehaviour
	{
		[Tooltip("The tag this bullet cannot hit")]
		public string IgnoreTag;
		
		[Tooltip("The layers this bullet can hit")]
		public LayerMask RaycastMask = -1;
		
		[Tooltip("The prefab that gets spawned when this bullet hits something")]
		public GameObject ExplosionPrefab;
		
		[Tooltip("The distance this bullet moves each second")]
		public float Speed;
		
		[Tooltip("The maximum length of the bullet trail")]
		public float MaxLength;
		
		[Tooltip("The scale of the bullet after it's scaled up")]
		public Vector3 MaxScale;
		
		private Vector3 oldPosition;
		
		protected virtual void Start()
		{
			oldPosition = transform.position;
		}
		
		protected virtual void FixedUpdate()
		{
			var newPosition  = transform.position;
			var rayLength    = (newPosition - oldPosition).magnitude;
			var rayDirection = (newPosition - oldPosition).normalized;
			var hit          = Physics2D.Raycast(oldPosition, rayDirection, rayLength, RaycastMask);
			
			// Update old position to trail behind 
			if (rayLength > MaxLength)
			{
				rayLength   = MaxLength;
				oldPosition = newPosition - rayDirection * rayLength;
			}
			
			transform.localScale = MaxScale * D2dHelper.Divide(rayLength, MaxLength);
			
			if (hit.collider != null)
			{
				if (string.IsNullOrEmpty(IgnoreTag) == true || hit.collider.tag != IgnoreTag)
				{
					if (ExplosionPrefab != null)
					{
						Instantiate(ExplosionPrefab, hit.point, Quaternion.identity);
					}
					
					Destroy(gameObject);
				}
			}
		}
		
		protected virtual void Update()
		{
			transform.Translate(0.0f, Speed * Time.deltaTime, 0.0f);
		}
		
#if UNITY_EDITOR
		protected virtual void OnDrawGizmos()
		{
			Gizmos.DrawLine(transform.position, transform.TransformPoint(0.0f, -MaxLength, 0.0f));
		}
#endif
	}
}