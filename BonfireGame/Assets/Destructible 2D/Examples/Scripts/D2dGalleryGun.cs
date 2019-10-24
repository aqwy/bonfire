using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dGalleryGun))]
	public class D2dGalleryGun_Editor : D2dEditor<D2dGalleryGun>
	{
		protected override void OnInspector()
		{
			DrawDefault("MoveScale");
			DrawDefault("MoveSpeed");
			DrawDefault("MuzzlePrefab");
			DrawDefault("BulletPrefab");
		}
	}
}
#endif

namespace Destructible2D
{
	// This component handles the gun in the shooting gallery demo scene
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Gun")]
	public class D2dGalleryGun : MonoBehaviour
	{
		[Tooltip("How much the mouse position relates to the gun position")]
		public float MoveScale = 0.25f;
		
		[Tooltip("How quickly the gun moves to its atrget position")]
		public float MoveSpeed = 5.0f;
		
		[Tooltip("The prefab spawned at the muzzle of the gun when shooting")]
		public GameObject MuzzlePrefab;
		
		[Tooltip("The prefab spawned at the mouse position when shooting")]
		public GameObject BulletPrefab;
		
		protected virtual void Update()
		{
			var localPosition = transform.localPosition;
			var targetX       = (Input.mousePosition.x - Screen.width  / 2) * MoveScale;
			var targetY       = (Input.mousePosition.y - Screen.height / 2) * MoveScale;
			
			localPosition.x = D2dHelper.Dampen(localPosition.x, targetX, MoveSpeed, Time.deltaTime);
			localPosition.y = D2dHelper.Dampen(localPosition.y, targetY, MoveSpeed, Time.deltaTime);
			
			transform.localPosition = localPosition;
			
			// Left click?
			if (Input.GetMouseButtonDown(0) == true)
			{
				var mainCamera = Camera.main;
				
				if (MuzzlePrefab != null)
				{
					Instantiate(MuzzlePrefab, transform.position, Quaternion.identity);
				}
				
				if (BulletPrefab != null && mainCamera != null)
				{
					var position = mainCamera.ScreenToWorldPoint(Input.mousePosition);
					
					Instantiate(BulletPrefab, position, Quaternion.identity);
				}
			}
		}
	}
}