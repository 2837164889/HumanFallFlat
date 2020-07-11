using UnityEngine;

namespace HumanAPI
{
	[AddComponentMenu("Human/Lerp Volume", 10)]
	public class LerpVolumeAudioSource : LerpBase
	{
		public float from;

		public float to = 1f;

		[Tooltip("Use this in order to show the prints coming from the script")]
		public bool showDebug;

		private float currentVolume;

		private AudioSource sound;

		protected override void Awake()
		{
			sound = GetComponent<AudioSource>();
			if (sound != null)
			{
				currentVolume = sound.volume;
				if (showDebug && sound != null)
				{
					Debug.Log(base.name + " Found something to play ");
				}
			}
			base.Awake();
		}

		protected override void ApplyValue(float value)
		{
			if (!(sound != null))
			{
				return;
			}
			if (showDebug)
			{
				Debug.Log(base.name + " Value coming in is = " + value);
			}
			if (!sound.isPlaying)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " The Sound is not Playing ");
				}
				sound.Play();
			}
			if (showDebug)
			{
				Debug.Log(base.name + " Setting Volume to " + value);
			}
			currentVolume = value;
			sound.volume = currentVolume;
		}
	}
}
