using HumanAPI;
using UnityEngine;

public class SignalSetVolume : MonoBehaviour
{
	public SignalBase triggerSignal;

	private AudioSource source;

	private Sound2 sound;

	public float minVolume = 1f;

	public float maxVolume = 1f;

	public float minPitch = 1f;

	public float maxPitch = 1f;

	private void OnEnable()
	{
		sound = GetComponent<Sound2>();
		source = GetComponent<AudioSource>();
		triggerSignal.onValueChanged += SignalChanged;
		SignalChanged(triggerSignal.value);
	}

	protected void OnDisable()
	{
		triggerSignal.onValueChanged -= SignalChanged;
	}

	private void SignalChanged(float val)
	{
		if (sound != null)
		{
			sound.SetPitch(Mathf.Lerp(minPitch, maxPitch, val));
			sound.SetVolume(Mathf.Lerp(minVolume, maxVolume, val));
		}
		else
		{
			source.pitch = Mathf.Lerp(minPitch, maxPitch, val);
			source.volume = Mathf.Lerp(minVolume, maxVolume, val);
		}
	}
}
