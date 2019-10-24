using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dHealDamage))]
	public class D2dHealDamage_Editor : D2dEditor<D2dHealDamage>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.DelayPerHeal < 0.0f));
				DrawDefault("DelayPerHeal");
			EndError();
			BeginError(Any(t => t.HealAmount <= 0));
				DrawDefault("HealAmount");
			EndError();
		}
	}
}
#endif

namespace Destructible2D
{
	// This component automatically heals a Destructible object to its initial state
	[RequireComponent(typeof(D2dDestructible))]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Heal Damage")]
	public class D2dHealDamage : MonoBehaviour
	{
		[Tooltip("How many seconds it takes for the Destructible to get healed another step")]
		public float DelayPerHeal = 0.1f;
		
		[Tooltip("How much alpha gets healed per second (Alpha is 0-255)")]
		public int HealAmount = 10;
		
		// The snapshot of the Destrucrtible's initial damage state
		private D2dSnapshot snapshot;
		
		// The cached destructible
		private D2dDestructible destructible;
		
		// The remaining seconds until the next heal
		[SerializeField]
		private float cooldown;
		
		protected virtual void Awake()
		{
			if (destructible == null) destructible = GetComponent<D2dDestructible>();
			
			// Get a snapshot of the current Destructible's alpha data
			snapshot = destructible.GetSnapshot();
		}
		
		protected virtual void Update()
		{
			cooldown -= Time.deltaTime;
			
			if (cooldown <= 0.0f)
			{
				cooldown = DelayPerHeal;
				
				if (destructible == null) destructible = GetComponent<D2dDestructible>();
				
				// Make sure the snapshot matches the current destructible
				if (snapshot.AlphaWidth == destructible.AlphaWidth && snapshot.AlphaHeight == destructible.AlphaHeight)
				{
					destructible.BeginAlphaModifications();
					{
						// Go through all pixels
						for (var y = snapshot.AlphaHeight - 1; y >= 0; y--)
						{
							for (var x = snapshot.AlphaWidth - 1; x >= 0; x--)
							{
								// Find current and snapshot alpha values
								var index    = x + y * snapshot.AlphaWidth;
								var oldAlpha = destructible.AlphaData[index];
								var newAlpha = snapshot.AlphaData[index];
								
								// Are they different?
								if (oldAlpha != newAlpha)
								{
									// Move old alpha toward new alpha
									newAlpha = (byte)Mathf.MoveTowards(oldAlpha, newAlpha, HealAmount);
									
									// Write the new value
									destructible.WriteAlpha(x, y, newAlpha);
								}
							}
						}
					}
					destructible.EndAlphaModifications();
				}
			}
		}
	}
}