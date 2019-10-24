using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dClickToExplode))]
	public class D2dClickToExplode_Editor : D2dEditor<D2dClickToExplode>
	{
		protected override void OnInspector()
		{
			DrawDefault("Requires");
			DrawDefault("Intercept");
			DrawDefault("ExplosionPrefab");
			DrawDefault("FractureCount");
			DrawDefault("Force");
		}
	}
}
#endif

namespace Destructible2D
{
	// This component fractures the Destructible under the mouse and then spawns a prefab there
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Click To Explode")]
	public class D2dClickToExplode : MonoBehaviour
	{
		[Tooltip("The key you must hold down to spawn")]
		public KeyCode Requires = KeyCode.Mouse0;

		[Tooltip("The z position the prefab should spawn at")]
		public float Intercept;

		[Tooltip("The prefab that gets spawned under the mouse when clicking")]
		public GameObject ExplosionPrefab;

		[Tooltip("The amount of times you want the clicked object to fracture")]
		public int FractureCount = 5;

		[Tooltip("The amount of outward force added to each fractured part")]
		public float Force;

		// Stores the point of the last explosion in world space so it can be used in OnEndSplit
		private Vector2 explosionPosition;

		protected virtual void Update()
		{
			if (FractureCount <= 0) return;

			// Required key is down?
			if (Input.GetKeyDown(Requires) == true)
			{
				// Main camera exists?
				var mainCamera = Camera.main;

				if (mainCamera != null)
				{
					// Get screen ray of mouse position
					explosionPosition = D2dHelper.ScreenToWorldPosition(Input.mousePosition, Intercept, mainCamera);

					var collider = Physics2D.OverlapPoint(explosionPosition);

					if (collider != null)
					{
						var destructible = collider.GetComponentInParent<D2dDestructible>();

						if (destructible != null)
						{
							// Register split event
							destructible.OnEndSplit.AddListener(OnEndSplit);

							// Split via fracture
							D2dQuadFracturer.Fracture(destructible, FractureCount, 0.5f);

							// Unregister split event
							destructible.OnEndSplit.RemoveListener(OnEndSplit);

							// Spawn explosion prefab?
							if (ExplosionPrefab != null)
							{
								var worldRotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)); // Random rotation around Z axis

								Instantiate(ExplosionPrefab, explosionPosition, worldRotation);
							}
						}
					}
				}
			}
		}

		private void OnEndSplit(List<D2dDestructible> clones)
		{
			// Go through all clones in the clones list
			for (var i = clones.Count - 1; i >= 0; i--)
			{
				var clone     = clones[i];
				var rigidbody = clone.GetComponent<Rigidbody2D>();

				// Does this clone have a Rigidbody2D?
				if (rigidbody != null)
				{
					// Get the local point of the explosion that called this split event
					var localPoint = (Vector2)clone.transform.InverseTransformPoint(explosionPosition);

					// Get the vector between this point and the center of the destructible's current rect
					var vector = clone.AlphaRect.center - localPoint;

					// Apply relative force
					rigidbody.AddRelativeForce(vector * Force, ForceMode2D.Impulse);
				}
			}
		}
	}
}