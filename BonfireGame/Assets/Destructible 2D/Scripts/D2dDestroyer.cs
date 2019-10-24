using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dDestroyer))]
	public class D2dDestroyer_Editor : D2dEditor<D2dDestroyer>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Life < 0.0f));
				DrawDefault("Life");
			EndError();

			Separator();

			DrawDefault("Fade");

			if (Any(t => t.Fade == true))
			{
				BeginIndent();
					BeginError(Any(t => t.FadeDuration <= 0.0f));
						DrawDefault("FadeDuration");
					EndError();
				EndIndent();
			}

			Separator();

			DrawDefault("Shrink");

			if (Any(t => t.Shrink == true))
			{
				BeginIndent();
					BeginError(Any(t => t.ShrinkDuration <= 0.0f));
						DrawDefault("ShrinkDuration");
					EndError();
				EndIndent();
			}

			Separator();

			DrawDefault("RandomizeOnEnable");

			if (Any(t => t.RandomizeOnEnable == true))
			{
				BeginIndent();
					BeginError(Any(t => t.LifeMin < 0.0f || t.LifeMin > t.LifeMax));
						DrawDefault("LifeMin");
					EndError();

					BeginError(Any(t => t.LifeMax < 0.0f || t.LifeMin > t.LifeMax));
						DrawDefault("LifeMax");
					EndError();
				EndIndent();
			}
		}
	}
}
#endif

namespace Destructible2D
{
	// This component will automatically destroy the current GameObject after a certain amount of time
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Destroyer")]
	public class D2dDestroyer : MonoBehaviour
	{
		[Tooltip("The amount of seconds it takes for this GameObject to get destroyed if it falls below the MinAlphaCount")]
		public float Life = 3.0f;

		[Tooltip("Should the destructible attached to this GameObject fade out?")]
		public bool Fade;

		[Tooltip("The amount of seconds it takes for the fade animation to complete")]
		public float FadeDuration = 1.0f;

		[Tooltip("Should this GameObject shrink to 0?")]
		public bool Shrink;

		[Tooltip("The amount of seconds it takes for the shrink animation to complete")]
		public float ShrinkDuration = 1.0f;

		[Tooltip("Should these settings get randomized when this component is enabled?")]
		public bool RandomizeOnEnable;

		[Tooltip("The minimum randomized Life value")]
		public float LifeMin = 3.0f;
		
		[Tooltip("The minimum randomized Life value")]
		public float LifeMax = 5.0f;

		[SerializeField]
		private Color startColor;

		[SerializeField]
		private Vector3 startLocalScale;

		[System.NonSerialized]
		private D2dDestructible destructible;

		protected virtual void OnEnable()
		{
			if (RandomizeOnEnable == true)
			{
				Life = Random.Range(LifeMin, LifeMax);
			}
		}

		protected virtual void Update()
		{
			Life -= Time.deltaTime;

			if (Life > 0.0f)
			{
				if (Fade == true)
				{
					UpdateFade();
				}

				if (Shrink == true)
				{
					UpdateShrink();
				}
			}
			else
			{
				D2dHelper.Destroy(gameObject);
			}
		}

		private void UpdateFade()
		{
			if (FadeDuration > 0.0f)
			{
				if (destructible == null) destructible = GetComponent<D2dDestructible>();

				if (destructible != null)
				{
					if (FadeDuration > 0.0f && Life < FadeDuration)
					{
						if (startColor == default(Color))
						{
							startColor = destructible.Color;
						}

						var finalColor = startColor;

						finalColor.a *= Life / FadeDuration;

						destructible.Color = finalColor;
					}
				}
			}
		}

		private void UpdateShrink()
		{
			if (ShrinkDuration > 0.0f)
			{
				if (startLocalScale == default(Vector3))
				{
					startLocalScale = transform.localScale;
				}

				// Setting a zero scale might cause issues, so don't
				if (startLocalScale != Vector3.zero)
				{
					var finalScale = startLocalScale;

					finalScale *= Life / FadeDuration;

					transform.localScale = finalScale;
				}
			}
		}
	}
}
