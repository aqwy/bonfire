using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dRandomVelocity))]
	public class D2dRandomVelocity_Editor : D2dEditor<D2dRandomVelocity>
	{
		protected override void OnInspector()
		{
			DrawDefault("MaxLinearSpeed");
			DrawDefault("MaxAngularSpeed");
			DrawDefault("Additive");
		}
	}
}
#endif

namespace Destructible2D
{
	// This component will automatically add velocity to split pieces
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Random Velocity")]
	public class D2dRandomVelocity : MonoBehaviour
	{
		[Tooltip("The maximum speed applied")]
		public float MaxLinearSpeed = 1.0f;
		
		[Tooltip("The maximum speed applied")]
		public float MaxAngularSpeed = 1.0f;

		[Tooltip("Should the new velocity be added to the existing one?")]
		public bool Additive;

		[System.NonSerialized]
		private Rigidbody2D body;
		
		public void RandomizeVelocity()
		{
			if (body == null) body = GetComponent<Rigidbody2D>();

			if (Additive == true)
			{
				body.velocity        += Random.insideUnitCircle * MaxLinearSpeed;
				body.angularVelocity += Random.value            * MaxAngularSpeed;
			}
			else
			{
				body.velocity        = Random.insideUnitCircle * MaxLinearSpeed;
				body.angularVelocity = Random.value            * MaxAngularSpeed;
			}
		}
	}
}