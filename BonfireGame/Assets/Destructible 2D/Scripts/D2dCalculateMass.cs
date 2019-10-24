using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dCalculateMass))]
	public class D2dCalculateMass_Editor : D2dEditor<D2dCalculateMass>
	{
		protected override void OnInspector()
		{
			DrawDefault("MassPerSolidPixel");
		}
	}
}
#endif

namespace Destructible2D
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(D2dDestructible))]
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Calculate Mass")]
	public class D2dCalculateMass : MonoBehaviour
	{
		[Tooltip("The amount of mass added to the Rigidbody2D by each solid pixel in the Destructible")]
		public float MassPerSolidPixel = 0.01f;
		
		[System.NonSerialized]
		private Rigidbody2D mainRigidbody2D;
		
		[System.NonSerialized]
		private D2dDestructible destructible;
		
		[System.NonSerialized]
		private float lastSetMass = -1.0f;
		
		protected virtual void OnEnable()
		{
			if (mainRigidbody2D == null) mainRigidbody2D = GetComponent<Rigidbody2D>();
			if (destructible    == null) destructible    = GetComponent<D2dDestructible>();
		}
		
		protected virtual void Update()
		{
			var newMass = destructible.AlphaCount * MassPerSolidPixel;
			
			if (newMass != lastSetMass)
			{
				mainRigidbody2D.mass = lastSetMass = newMass;
			}
		}
	}
}