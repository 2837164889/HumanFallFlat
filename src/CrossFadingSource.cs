using UnityEngine;

public class CrossFadingSource : MonoBehaviour
{
	private AudioSource[] sources;

	private int active = 1;

	private int inactive;

	private float fadeTime;

	private float fadeDuration;

	private float fadeInDuration;

	private float fadeOutDuration;

	private float pauseVol = 1f;

	private float pauseSpeed;

	private float activeVolume;

	private float inactiveVolume;

	private AudioClip queuedClip;

	private float queuedVolume;

	private bool queuedLoop;

	public bool isPlaying;

	private bool isSourcePaused;

	public AudioSource activeSource => sources[active];

	private void Awake()
	{
		sources = GetComponentsInChildren<AudioSource>();
	}

	private void Update()
	{
		if (sources.Length <= active)
		{
			Debug.Log(base.name, this);
		}
		if (!sources[active].isPlaying && !isSourcePaused)
		{
			isPlaying = false;
		}
		if (fadeDuration <= 0f && pauseSpeed == 0f)
		{
			return;
		}
		if (pauseSpeed != 0f)
		{
			pauseVol = Mathf.Clamp01(pauseVol + pauseSpeed * Time.deltaTime);
			if (pauseVol == 0f)
			{
				if (sources[inactive].isPlaying)
				{
					sources[inactive].Stop();
				}
				if (sources[active].isPlaying)
				{
					sources[active].Pause();
					isSourcePaused = true;
				}
				pauseSpeed = 0f;
			}
			else if (pauseVol == 1f)
			{
				pauseSpeed = 0f;
			}
			if (fadeDuration == 0f)
			{
				if (sources[active].isPlaying)
				{
					sources[active].volume = activeVolume * pauseVol;
				}
				return;
			}
		}
		isPlaying = true;
		float num = fadeTime;
		fadeTime += Time.deltaTime;
		if (!(fadeTime < 0f))
		{
			if (num < 0f)
			{
				active = inactive;
				inactive = (active + 1) % 2;
				inactiveVolume = activeVolume;
				activeVolume = queuedVolume;
				sources[active].clip = queuedClip;
				sources[active].loop = queuedLoop;
			}
			if (num < fadeDuration - fadeInDuration && fadeTime >= fadeDuration - fadeInDuration)
			{
				sources[active].Play();
			}
			float num2 = Mathf.Clamp01(fadeTime / fadeDuration);
			float f = 1f - Mathf.Clamp01(fadeTime / fadeOutDuration);
			float f2 = 1f - Mathf.Clamp01((fadeDuration - fadeTime) / fadeInDuration);
			sources[active].volume = Mathf.Sqrt(f2) * activeVolume * Mathf.Sqrt(pauseVol);
			sources[inactive].volume = Mathf.Sqrt(f) * inactiveVolume * Mathf.Sqrt(pauseVol);
			if (num2 == 1f)
			{
				sources[inactive].Stop();
				fadeDuration = 0f;
			}
		}
	}

	internal void PlayOneShot(AudioClip clip, float volume)
	{
		sources[active].PlayOneShot(clip, volume);
	}

	public void Play(AudioClip clip, float volume, bool loop)
	{
		sources[active].clip = clip;
		sources[active].volume = volume;
		sources[active].loop = loop;
		sources[active].Play();
		isPlaying = true;
		sources[inactive].Stop();
		fadeDuration = 0f;
		activeVolume = volume;
	}

	public void Stop()
	{
		sources[active].Stop();
		sources[inactive].Stop();
		isPlaying = false;
	}

	public void CrossFade(AudioClip clip, float volume, bool loop, float after, float duration)
	{
		queuedClip = clip;
		queuedVolume = volume;
		queuedLoop = loop;
		fadeOutDuration = (fadeInDuration = (fadeDuration = duration));
		fadeTime = Mathf.Min(-0.001f, 0f - after);
	}

	public void CrossFade(AudioClip clip, float volume, bool loop, float after, float duration, float outDuration, float inDuration)
	{
		queuedClip = clip;
		queuedVolume = volume;
		queuedLoop = loop;
		fadeDuration = duration;
		fadeOutDuration = Mathf.Max(0.001f, outDuration);
		fadeInDuration = Mathf.Max(0.001f, inDuration);
		fadeTime = Mathf.Min(-0.001f, 0f - after);
	}

	public void SetPitch(float pitch)
	{
		AudioSource obj = sources[inactive];
		sources[active].pitch = pitch;
		obj.pitch = pitch;
	}

	public void Pause()
	{
		if (isPlaying)
		{
			pauseSpeed = -1f;
		}
	}

	public void Resume()
	{
		pauseSpeed = 0.33333334f;
		if (pauseVol == 0f && isSourcePaused)
		{
			isSourcePaused = false;
			sources[active].UnPause();
		}
	}
}
