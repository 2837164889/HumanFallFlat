using HumanAPI;
using UnityEngine;

public class SignalServoSound : MonoBehaviour
{
	public SignalBase triggerSignal;

	public ServoSound servoSound;

	public Sound2 sound;

	public bool playOnReverse;

	public float pitchLow = 1f;

	public float pitchHigh = 1f;

	public float treshold = 0.5f;

	private void OnEnable()
	{
		triggerSignal.onValueChanged += SignalChanged;
		if (triggerSignal.value > treshold || (playOnReverse && triggerSignal.value < 0f - treshold))
		{
			sound.SetPitch(Mathf.Lerp(pitchLow, pitchHigh, Mathf.Abs(triggerSignal.value)));
			sound.Play();
		}
	}

	private void OnDisable()
	{
		triggerSignal.onValueChanged -= SignalChanged;
	}

	private void SignalChanged(float val)
	{
		if (triggerSignal.value > treshold || (playOnReverse && triggerSignal.value < 0f - treshold))
		{
			sound.SetPitch(Mathf.Lerp(pitchLow, pitchHigh, Mathf.Abs(triggerSignal.value)));
			if (!sound.isPlaying)
			{
				sound.Play();
			}
		}
		else if (sound.isPlaying)
		{
			sound.Stop();
		}
	}
}
