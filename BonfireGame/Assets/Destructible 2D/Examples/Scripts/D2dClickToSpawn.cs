using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dClickToSpawn))]
	public class D2dClickToSpawn_Editor : D2dEditor<D2dClickToSpawn>
	{
		protected override void OnInspector()
		{
			DrawDefault("Requires");
			DrawDefault("Intercept");
			BeginError(Any(t => t.Prefab == null));
				DrawDefault("Prefab");
			EndError();
		}
	}
}
#endif

namespace Destructible2D
{
	// This component spawns a prefab under the mouse when you click
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Click To Spawn")]
	public class D2dClickToSpawn : MonoBehaviour
	{
		[Tooltip("The key you must hold down to spawn")]
		public KeyCode Requires = KeyCode.Mouse0;

		[Tooltip("The z position the prefab should spawn at")]
		public float Intercept;
		
		[Tooltip("The prefab that gets spawned under the mouse when clicking")]
		public GameObject Prefab;
		
		protected virtual void Update()
		{
			// Required key is down?
			if (Input.GetKeyDown(Requires) == true)
			{
				// Prefab exists?
				if (Prefab != null)
				{
					// Main camera exists?
					var mainCamera = Camera.main;

					if (mainCamera != null)
					{
						// World position of the mouse
						var position = D2dHelper.ScreenToWorldPosition(Input.mousePosition, Intercept, mainCamera);

						// Get a random rotation around the Z axis
						var rotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
						
						// Spawn prefab here
						Instantiate(Prefab, position, rotation);
					}
				}
			}
		}
	}
}