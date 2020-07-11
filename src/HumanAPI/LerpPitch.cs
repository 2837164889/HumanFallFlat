using UnityEngine;

namespace HumanAPI
{
	[AddComponentMenu("Human/Lerp Pitch", 10)]
	public class LerpPitch : LerpBase
	{
		public float from;

		public float to;

		private Sound2 sound;

		private AudioSource unitySound;

		protected override void Awake()
		{
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
				float pitch = Mathf.Lerp(AudioUtils.CentsToRatio(from), AudioUtils.CentsToRatio(to), value);
				sound.SetPitch(pitch);
			}
			else if (unitySound != null)
			{
				float pitch2 = Mathf.Lerp(AudioUtils.CentsToRatio(from), AudioUtils.CentsToRatio(to), value);
				unitySound.pitch = pitch2;
			}
		}
	}
}
