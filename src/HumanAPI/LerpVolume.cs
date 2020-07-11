using UnityEngine;

namespace HumanAPI
{
	[AddComponentMenu("Human/Lerp Volume", 10)]
	public class LerpVolume : LerpBase
	{
		[Tooltip("Minimum Value to output")]
		public float from;

		[Tooltip("Maximum value to output")]
		public float to;

		private Sound2 sound;

		private AudioSource unitySound;

		[Tooltip("Use this in order to show the prints coming from the script")]
		public bool showDebug;

		protected override void Awake()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Lerp Volume setting vars ");
			}
			sound = GetComponent<Sound2>();
			if (sound == null)
			{
				unitySound = GetComponent<AudioSource>();
			}
			base.Awake();
		}

		protected override void ApplyValue(float value)
		{
			if (sound != null)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Sound Lerp ");
				}
				float num = Mathf.Lerp(AudioUtils.DBToValue(from), AudioUtils.DBToValue(to), value);
				sound.SetVolume(num);
				if (showDebug)
				{
					Debug.Log(base.name + " Current Volume = " + num);
				}
			}
			else if (unitySound != null)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Audio Source Lerp ");
				}
				float num2 = Mathf.Lerp(AudioUtils.DBToValue(from), AudioUtils.DBToValue(to), value);
				unitySound.volume = num2;
				if (showDebug)
				{
					Debug.Log(base.name + " Current Volume = " + num2);
				}
			}
		}
	}
}
