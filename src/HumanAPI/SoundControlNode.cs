using UnityEngine;

namespace HumanAPI
{
	public class SoundControlNode : Node
	{
		public NodeInput input;

		public Sound2 sound;

		public AudioSource unitySound;

		public bool affectVolume = true;

		public float volumeMin;

		public float volumeMax = 1f;

		public bool affectLowPassFilter;

		public float lowPassMin = 22000f;

		public float lowPassMax = 22000f;

		public bool affectPitch;

		public float pitchMin = 0.5f;

		public float pitchMax = 1.5f;

		private void Awake()
		{
			if (sound == null)
			{
				sound = GetComponent<Sound2>();
			}
			else if (unitySound == null)
			{
				unitySound = GetComponentInChildren<AudioSource>();
			}
		}

		public override void Process()
		{
			base.Process();
			if (sound != null)
			{
				if (affectVolume)
				{
					float volume = Mathf.Lerp(volumeMin, volumeMax, input.value);
					sound.SetVolume(volume);
				}
				if (affectLowPassFilter)
				{
					float lowPass = Mathf.Lerp(lowPassMin, lowPassMax, input.value);
					sound.SetLowPass(lowPass);
				}
				if (affectPitch)
				{
					float pitch = Mathf.Lerp(pitchMin, pitchMax, input.value);
					sound.SetPitch(pitch);
				}
			}
			else if (unitySound != null)
			{
				if (affectVolume)
				{
					float volume2 = Mathf.Lerp(volumeMin, volumeMax, input.value);
					unitySound.volume = volume2;
				}
				if (affectLowPassFilter)
				{
					float cutoffFrequency = Mathf.Lerp(lowPassMin, lowPassMax, input.value);
					AudioLowPassFilter component = unitySound.GetComponent<AudioLowPassFilter>();
					component.cutoffFrequency = cutoffFrequency;
				}
				if (affectPitch)
				{
					float pitch2 = Mathf.Lerp(pitchMin, pitchMax, input.value);
					unitySound.pitch = pitch2;
				}
			}
		}
	}
}
