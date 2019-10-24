using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dThrower))]
	public class D2dThrower_Editor : D2dEditor<D2dThrower>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.DelayMin < 0.0f || (t.DelayMin > t.DelayMax)));
				DrawDefault("DelayMin");
			EndError();
			BeginError(Any(t => t.DelayMax < 0.0f || (t.DelayMin > t.DelayMax)));
				DrawDefault("DelayMax");
			EndError();
			BeginError(Any(t => t.SpeedMin < 0.0f || (t.SpeedMin > t.SpeedMax)));
				DrawDefault("SpeedMin");
			EndError();
			BeginError(Any(t => t.SpeedMax < 0.0f || (t.SpeedMin > t.SpeedMax)));
				DrawDefault("SpeedMax");
			EndError();
			BeginError(Any(t => t.Spread < 0.0f));
				DrawDefault("Spread");
			EndError();
			BeginError(Any(t => t.ThrowPrefabs == null || t.ThrowPrefabs.Length > 0));
				DrawDefault("ThrowPrefabs");
			EndError();
		}
	}
}
#endif

namespace Destructible2D
{
	// This component throws random prefabs upwards
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Thrower")]
	public class D2dThrower : MonoBehaviour
	{
		[Tooltip("The minimum delay between throws in seconds")]
		public float DelayMin = 0.5f;

		[Tooltip("The maximum delay between throws in seconds")]
		public float DelayMax = 2.0f;

		[Tooltip("The minimum speed of the thrown object")]
		public float SpeedMin = 10.0f;

		[Tooltip("The maximum speed of the thrown object")]
		public float SpeedMax = 20.0f;

		[Tooltip("Maximum degrees spread when throwing")]
		public float Spread = 10.0f;

		[Tooltip("The prefabs that can be thrown")]
		public GameObject[] ThrowPrefabs;

		// Seconds until next spawn
		[SerializeField]
		private float cooldown;

		protected virtual void Update()
		{
			cooldown -= Time.deltaTime;

			if (cooldown <= 0.0f)
			{
				cooldown = Random.Range(DelayMin, DelayMax);

				if (ThrowPrefabs != null && ThrowPrefabs.Length > 0)
				{
					var index     = Random.Range(0, ThrowPrefabs.Length);
					var prefab    = ThrowPrefabs[index];
					var instance  = Instantiate(prefab);
					var rigidbody = instance.GetComponent<Rigidbody2D>();

					instance.transform.position = transform.position;

					if (rigidbody != null)
					{
						var angle = Random.Range(-0.5f, 0.5f) * Spread * Mathf.Deg2Rad;
						var speed = Random.Range(SpeedMin, SpeedMax);

						rigidbody.velocity        = new Vector2(Mathf.Sin(angle) * speed, Mathf.Cos(angle) * speed);
						rigidbody.angularVelocity = Random.Range(-180.0f, 180.0f);
					}
				}
			}
		}
	}
}