using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dFixtureGroup))]
	public class D2dFixtureGroup_Editor : D2dEditor<D2dFixtureGroup>
	{
		protected override void OnInspector()
		{
			DrawDefault("Fixtures");
			DrawDefault("AutoDestroy");
			
			Separator();
			
			DrawDefault("OnAllFixturesRemoved");
		}
	}
}
#endif

namespace Destructible2D
{
	[DisallowMultipleComponent]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Fixture Group")]
	public class D2dFixtureGroup : MonoBehaviour
	{
		[Tooltip("The fixtures tracked by this group")]
		public List<D2dFixture> Fixtures;

		[Tooltip("Automatically destroy this component if all fixtures are removed?")]
		public bool AutoDestroy = true;

		public D2dEvent OnAllFixturesRemoved;

		public void UpdateFixtures()
		{
			if (Fixtures.Count > 0)
			{
				for (var i = Fixtures.Count - 1; i >= 0; i--)
				{
					var fixture = Fixtures[i];

					if (FixtureIsConnected(fixture) == false)
					{
						Fixtures.RemoveAt(i);
					}
				}

				if (Fixtures.Count == 0)
				{
					if (OnAllFixturesRemoved != null) OnAllFixturesRemoved.Invoke();

					if (AutoDestroy == true)
					{
						D2dHelper.Destroy(this);
					}
				}
			}
		}

		protected virtual void Update()
		{
			UpdateFixtures();
		}

		private bool FixtureIsConnected(D2dFixture fixture)
		{
			if (fixture != null)
			{
				var checkTransform = fixture.transform;

				while (checkTransform != null)
				{
					if (checkTransform == transform)
					{
						return true;
					}

					checkTransform = checkTransform.parent;
				}
			}

			return false;
		}
	}
}
