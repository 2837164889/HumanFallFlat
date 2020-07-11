using UnityEngine;

public class ServoSound : MonoBehaviour
{
	public float crossfadeStart = 0.2f;

	public float crossfadeEnd = 0.2f;

	public AudioClip[] startClips;

	public AudioClip[] loopClips;

	public AudioClip[] endClips;

	public AudioClip[] startClips2;

	public AudioClip[] loopClips2;

	public AudioClip[] endClips2;

	private CrossFadingSource source;

	public float pitch = 1f;

	public float volume = 1f;

	private float stopIn;

	public bool isPlaying;

	public bool secondMedium;

	private void Awake()
	{
		source = GetComponent<CrossFadingSource>();
	}

	public void CrossfadeLoop()
	{
		AudioClip clip = (!secondMedium) ? loopClips[Random.Range(0, loopClips.Length)] : loopClips2[Random.Range(0, loopClips2.Length)];
		source.CrossFade(clip, volume, loop: true, 0f, 0.5f);
	}

	public void Play(float stopIn = 0f)
	{
		isPlaying = true;
		this.stopIn = stopIn;
		AudioClip audioClip = (!secondMedium) ? startClips[Random.Range(0, startClips.Length)] : startClips2[Random.Range(0, startClips2.Length)];
		AudioClip clip = (!secondMedium) ? loopClips[Random.Range(0, loopClips.Length)] : loopClips2[Random.Range(0, loopClips2.Length)];
		source.Play(audioClip, volume, loop: false);
		source.CrossFade(clip, volume, loop: true, audioClip.length - crossfadeStart, crossfadeStart);
		source.SetPitch(pitch);
	}

	public void Stop()
	{
		if (isPlaying)
		{
			isPlaying = false;
			AudioClip clip = (!secondMedium) ? endClips[Random.Range(0, endClips.Length)] : endClips2[Random.Range(0, endClips2.Length)];
			source.CrossFade(clip, volume, loop: false, 0f, crossfadeEnd);
		}
	}

	public void SetPitch(float pitch)
	{
		source.SetPitch(pitch * this.pitch);
	}

	private void Update()
	{
		if (isPlaying && !(stopIn <= 0f))
		{
			stopIn -= Time.deltaTime;
			if (stopIn <= 0f)
			{
				Stop();
			}
		}
	}

	public void PLayOneShot(AudioClip clip)
	{
		source.PlayOneShot(clip, volume);
	}
}
