using System;
using UnityEngine;

public class AmbienceSource : MonoBehaviour
{
	private AudioSource audiosource;

	private float originalVolume;

	private float transitionSpeed = 10000000f;

	private float transitionPhase;

	public float transitionFrom;

	public float transitionTo;

	private void OnEnable()
	{
		audiosource = GetComponent<AudioSource>();
		originalVolume = audiosource.volume;
		audiosource.volume = 0f;
	}

	private void Update()
	{
		if (!(transitionPhase >= 1f))
		{
			transitionPhase = Mathf.Clamp01(transitionPhase + Time.deltaTime * transitionSpeed);
			audiosource.volume = Mathf.Lerp(transitionFrom, transitionTo, Mathf.Sqrt(transitionPhase));
		}
	}

	internal void FadeVolume(float volume, float duration)
	{
		if (duration == 0f)
		{
			throw new ArgumentException("duration can't be 0", "duration");
		}
		transitionFrom = audiosource.volume;
		transitionTo = originalVolume * volume;
		transitionSpeed = 1f / duration;
		transitionPhase = 0f;
	}
}
