using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dFlipper))]
	public class D2dFlipper_Editor : D2dEditor<D2dFlipper>
	{
		protected override void OnInspector()
		{
			DrawDefault("Flipped");
			DrawDefault("FlipDelay");
			DrawDefault("OnFlip");
			DrawDefault("OnUnflip");
		}
	}
}
#endif

namespace Destructible2D
{
	// This component flips between two states and fires events based on it
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Flipper")]
	public class D2dFlipper : MonoBehaviour
	{
		[Tooltip("Currently flipped?")]
		public bool Flipped;
		
		[Tooltip("The delay between flipping in seconds")]
		public float FlipDelay = 1.0f;

		[Tooltip("Called when Flipped = 1")]
		public D2dEvent OnFlip;

		[Tooltip("Called when Flipped = 0")]
		public D2dEvent OnUnflip;

		[SerializeField]
		private float cooldown;

		protected virtual void Update()
		{
			cooldown -= Time.deltaTime;

			if (cooldown <= 0.0f)
			{
				cooldown = FlipDelay;

				if (Flipped == true)
				{
					Flipped = false;

					if (OnUnflip != null) OnUnflip.Invoke();
                }
				else
				{
					Flipped = true;

					if (OnFlip != null) OnFlip.Invoke();
				}
            }
        }
	}
}