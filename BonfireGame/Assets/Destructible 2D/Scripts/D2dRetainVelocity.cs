using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dRetainVelocity))]
	public class D2dRetainVelocity_Editor : D2dEditor<D2dRetainVelocity>
	{
		protected override void OnInspector()
		{
		}
	}
}
#endif

namespace Destructible2D
{
	// This component causes the Rigidbody2D velocity to carry over after a Destructible has been split
	[DisallowMultipleComponent]
	[RequireComponent(typeof(D2dDestructible))]
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Retain Velocity")]
	public class D2dRetainVelocity : MonoBehaviour
	{
		[System.NonSerialized]
		private Rigidbody2D body;

		[System.NonSerialized]
		private D2dDestructible destructible;

		[System.NonSerialized]
		private float angularVelocity;

		[System.NonSerialized]
		private Vector2 velocity;

		protected virtual void OnEnable()
		{
			if (destructible              == null) destructible              = GetComponent<D2dDestructible>();
			if (destructible.OnStartSplit == null) destructible.OnStartSplit = new D2dEvent();
			if (destructible.OnEndSplit   == null) destructible.OnEndSplit   = new D2dDestructibleListEvent();

			destructible.OnStartSplit.AddListener(StartSplit);
			destructible.OnEndSplit  .AddListener(EndSplit);
		}

		protected virtual void OnDisable()
		{
			destructible.OnStartSplit.RemoveListener(StartSplit);
			destructible.OnEndSplit  .RemoveListener(EndSplit);
		}

		protected virtual void StartSplit()
		{
			if (body == null) body = GetComponent<Rigidbody2D>();

			velocity        = body.velocity;
			angularVelocity = body.angularVelocity;
		}

		protected virtual void EndSplit(List<D2dDestructible> clones)
		{
			for (var i = clones.Count - 1; i >= 0; i--)
			{
				var clone = clones[i];

				if (clone.gameObject != gameObject)
				{
					var cloneRigidbody2D = clone.GetComponent<Rigidbody2D>();

					if (cloneRigidbody2D != null)
					{
						cloneRigidbody2D.velocity        = velocity;
						cloneRigidbody2D.angularVelocity = angularVelocity;
					}
				}
			}
		}
	}
}
