using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dRandomizeSound))]
	public class D2dRandomizeSound_Editor : D2dEditor<D2dRandomizeSound>
	{
		protected override void OnInspector()
		{
			DrawDefault("PitchMin");
			DrawDefault("PitchMax");
			DrawDefault("Clips");
		}
	}
}
#endif

namespace Destructible2D
{
	// This component randomly changes the sound's pitch and clip
	[DisallowMultipleComponent]
	[RequireComponent(typeof(AudioSource))]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Randomize Sound")]
	public class D2dRandomizeSound : MonoBehaviour
	{
		[Tooltip("The minimum pitch of this sound")]
		public float PitchMin = 0.9f;
		
		[Tooltip("The maximum pitch of this sound")]
		public float PitchMax = 1.1f;
		
		[Tooltip("The audio clips that can be given to this sound")]
		public AudioClip[] Clips;
		
		protected virtual void Awake()
		{
			var audioSource = GetComponent<AudioSource>();
			
			audioSource.pitch = Random.Range(PitchMin, PitchMax);
			
			if (Clips != null && Clips.Length > 0)
			{
				audioSource.clip = Clips[Random.Range(0, Clips.Length)];
			}
			
			audioSource.Play();
		}
		
#if UNITY_EDITOR
		protected virtual void Reset()
		{
			var audioSource = GetComponent<AudioSource>();
			
			audioSource.playOnAwake = false;
			
			Clips = new AudioClip[] { audioSource.clip };
		}
#endif
	}
}