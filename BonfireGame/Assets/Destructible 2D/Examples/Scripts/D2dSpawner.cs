using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dSpawner))]
	public class D2dSpawner_Editor : D2dEditor<D2dSpawner>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Prefab == null));
				DrawDefault("Prefab");
			EndError();
			DrawDefault("SpawnInStart");
		}
	}
}
#endif

namespace Destructible2D
{
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Spawner")]
	public class D2dSpawner : MonoBehaviour
	{
		[Tooltip("The source GameObject you want to spawn")]
		[FormerlySerializedAs("Source")]
		public GameObject Prefab;

		[Tooltip("Should the source get spawned in Start?")]
		public bool SpawnInStart;

		public void SpawnAt(Collision2D collision)
		{
			if (Prefab != null && isActiveAndEnabled == true)
			{
				var contacts = collision.contacts;

				for (var i = contacts.Length - 1; i >= 0; i--)
				{
					Instantiate(Prefab, contacts[i].point, transform.rotation);
				}
			}
		}

		public void SpawnAt(Vector2 position)
		{
			if (Prefab != null && isActiveAndEnabled == true)
			{
				Instantiate(Prefab, position, transform.rotation);
			}
		}

		public void SpawnAt(Vector3 position)
		{
			if (Prefab != null && isActiveAndEnabled == true)
			{
				Instantiate(Prefab, position, transform.rotation);
			}
		}

		public void Spawn()
		{
			if (Prefab != null && isActiveAndEnabled == true)
			{
				Instantiate(Prefab, transform.position, transform.rotation);
			}
		}

		protected virtual void Start()
		{
			if (SpawnInStart == true)
			{
				Spawn();
			}
		}
	}
}