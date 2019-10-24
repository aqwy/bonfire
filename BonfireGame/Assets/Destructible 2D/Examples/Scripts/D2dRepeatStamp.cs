using UnityEngine;

namespace Destructible2D
{
#if UNITY_EDITOR
	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(D2dRepeatStamp))]
	public class D2dRepeatStamp_Editor : D2dEditor<D2dRepeatStamp>
	{
		protected override void OnInspector()
		{
			DrawDefault("Layers");

			Separator();

			DrawDefault("StampTex");
			DrawDefault("Size");
			DrawDefault("Hardness");
			DrawDefault("Delay");
		}
	}
#endif

	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Repeat Stamp")]
	public class D2dRepeatStamp : MonoBehaviour
	{
		[Tooltip("The layers the stamp works on")]
		public LayerMask Layers = -1;

		[Tooltip("The shape of the stamp")]
		public Texture2D StampTex;

		[Tooltip("The size of the stamp in world space")]
		public Vector2 Size = Vector2.one;

		[Tooltip("How hard the stamp is")]
		public float Hardness = 1.0f;

		[Tooltip("The delay between each repeat stamp")]
		public float Delay = 0.25f;

		private float cooldown;

		protected virtual void Update()
		{
			cooldown -= Time.deltaTime;

			if (cooldown <= 0.0f)
			{
				cooldown = Delay;

				var angle = Random.Range(0.0f, 360.0f);

				D2dDestructible.StampAll(transform.position, Size, angle, StampTex, Hardness, Layers);
			}
		}
	}
}