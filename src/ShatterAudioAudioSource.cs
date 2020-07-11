using UnityEngine;

public class ShatterAudioAudioSource : MonoBehaviour
{
	[Tooltip("Array pof sounds to make when shattered")]
	public AudioClip[] shatterAudioClips;

	[Tooltip("Sound to make when cracked")]
	public AudioClip[] crackAudioClips;

	private AudioSource currentAudioSource;

	private int currentArrayEntry;

	private int currentArrayLength;

	[Tooltip("Max vol the sound should play at")]
	public float volume = 1f;

	[Tooltip("Unused Var")]
	public float pitch = 1f;

	[Tooltip("Unused Var")]
	public float volumeCrack = 0.5f;

	[Tooltip("Unused Var")]
	public float pitchCrack = 1f;

	[Tooltip("Used to determine the impact needed to shatter")]
	public float unityImpact = 100f;

	private float compressTreshold = 0.5f;

	private float limitTreshold = 1f;

	private float compressRatio = 8f;

	[Tooltip("Use this in order to show the prints coming from the script")]
	public bool showDebug;

	private void Start()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Started ");
		}
		currentAudioSource = GetComponent<AudioSource>();
	}

	public void Shatter(float impact, Vector3 pos)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Shatter " + shatterAudioClips.Length);
		}
		if (shatterAudioClips.Length == 0)
		{
			return;
		}
		if (showDebug)
		{
			Debug.Log(base.name + " There is something in the Shatter Array  ");
		}
		float num = impact / unityImpact;
		if (num > compressTreshold)
		{
			num = (num - compressTreshold) / compressRatio + compressTreshold;
			if (showDebug)
			{
				Debug.Log(base.name + " volume > compressTreshold ");
				Debug.Log(base.name + " Volume = " + num);
			}
		}
		if (num > limitTreshold)
		{
			num = limitTreshold;
			if (showDebug)
			{
				Debug.Log(base.name + " volume > limitTreshold ");
				Debug.Log(base.name + " Volume = " + num);
			}
		}
		currentArrayLength = shatterAudioClips.Length;
		RandomArrayEntry();
		currentAudioSource.clip = shatterAudioClips[currentArrayEntry];
		currentAudioSource.volume = num;
		currentAudioSource.Play();
	}

	public void Crack(float impact, Vector3 pos)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Crack ");
		}
		if (crackAudioClips.Length != 0)
		{
			float num = impact / unityImpact;
			if (num > compressTreshold)
			{
				num = (num - compressTreshold) / compressRatio + compressTreshold;
			}
			if (showDebug)
			{
				Debug.Log(base.name + " There is something in the Crack Array  ");
			}
			currentArrayLength = crackAudioClips.Length;
			RandomArrayEntry();
			currentAudioSource.clip = crackAudioClips[currentArrayEntry];
			currentAudioSource.volume = num;
			currentAudioSource.Play();
		}
	}

	public void RandomArrayEntry()
	{
		currentArrayEntry = Random.Range(1, currentArrayLength);
		if (showDebug)
		{
			Debug.Log(base.name + " current Array Entry = " + currentArrayEntry);
			Debug.Log(base.name + " Array Length = " + currentArrayLength);
		}
	}
}
