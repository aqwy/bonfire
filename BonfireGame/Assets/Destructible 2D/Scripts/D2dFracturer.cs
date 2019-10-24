using UnityEngine;
using System.Collections.Generic;

namespace Destructible2D
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(D2dDestructible))]
	public abstract class D2dFracturer : MonoBehaviour
	{
		[Tooltip("Every time a Destructible is fractured this decreases by 1, if it reaches 0 then this component will be removed")]
		public int RemainingFractures = 1;

		[Tooltip("The amount of damage required by the Destructible for it to fracture")]
		public float RequiredDamage = 100.0f;

		[Tooltip("Each time the Destructible is fractured, the RequiredDamage will by multiplied by this")]
		public float RequiredDamageMultiplier = 2.0f;

		[Tooltip("The amount of times the Destructible should be fractured in two")]
		public int FractureCount = 5;

		[Tooltip("Each time the Destructible is fractured, the FractureCount will by multiplied by this")]
		public float FractureCountMultiplier = 0.75f;
		
		[System.NonSerialized]
		protected D2dDestructible destructible;
		
		[ContextMenu("Fracture")]
		public virtual void Fracture()
		{
			RemainingFractures -= 1;
			RequiredDamage     *= RequiredDamageMultiplier;
			FractureCount       = Mathf.CeilToInt(FractureCount * FractureCountMultiplier);

			if (destructible == null) destructible = GetComponent<D2dDestructible>();
		}

		public void UpdateFracture()
		{
			if (RemainingFractures > 0)
			{
				if (destructible == null) destructible = GetComponent<D2dDestructible>();

				if (destructible.Damage >= RequiredDamage)
				{
					Fracture();
				}
			}
		}

		protected virtual void OnEnable()
		{
			if (destructible                 == null) destructible                 = GetComponent<D2dDestructible>();
			if (destructible.OnDamageChanged == null) destructible.OnDamageChanged = new D2dFloatFloatEvent();

			destructible.OnDamageChanged.AddListener(OnDamageChanged);
		}

		protected virtual void OnDisable()
		{
			destructible.OnDamageChanged.RemoveListener(OnDamageChanged);
		}

		private void OnDamageChanged(float oldDamage, float newDamage)
		{
			UpdateFracture();
		}
	}
}
