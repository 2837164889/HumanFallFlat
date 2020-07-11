using HumanAPI;
using UnityEngine;

public class SignalSound : MonoBehaviour
{
	public SignalBase triggerSignal;

	private bool isOn;

	public Sound2 onSound;

	public Sound2 offSound;

	public AudioClip onClip;

	public AudioClip offClip;

	public float onVolume = 1f;

	public float offVolume = 1f;

	public float minDelay = 0.2f;

	private float lastSoundTime;

	public bool playOnOff = true;

	private void OnEnable()
	{
		isOn = triggerSignal.boolValue;
		triggerSignal.onValueChanged += SignalChanged;
	}

	private void OnDisable()
	{
		triggerSignal.onValueChanged -= SignalChanged;
	}

	private void SignalChanged(float val)
	{
		if (triggerSignal.boolValue == isOn)
		{
			return;
		}
		isOn = triggerSignal.boolValue;
		float time = Time.time;
		if (lastSoundTime + minDelay > time)
		{
			return;
		}
		if (isOn && (onClip != null || onSound != null))
		{
			if (onSound != null)
			{
				onSound.PlayOneShot();
			}
			else
			{
				GetComponent<AudioSource>().PlayOneShot(onClip, onVolume);
			}
			lastSoundTime = time;
		}
		else if (playOnOff && !isOn && (offClip != null || onClip != null || offSound != null || onSound != null))
		{
			if (offSound != null)
			{
				offSound.PlayOneShot();
			}
			else if (onSound != null)
			{
				onSound.PlayOneShot();
			}
			else
			{
				GetComponent<AudioSource>().PlayOneShot((!(offClip != null)) ? onClip : offClip, offVolume);
			}
			lastSoundTime = time;
		}
	}
}
