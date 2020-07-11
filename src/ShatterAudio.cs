using HumanAPI;
using UnityEngine;

public class ShatterAudio : MonoBehaviour
{
	[Tooltip("Sound to make when shattered")]
	public Sound2 shatter;

	[Tooltip("Sound to make when cracked")]
	public Sound2 crack;

	[Tooltip("Audiosource version of the shatter sounds")]
	public AudioSource audioShatter;

	[Tooltip("Audiosource version of the Crack sounds")]
	public AudioSource audioCrack;

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

	public void Shatter(float impact, Vector3 pos)
	{
		float num = impact / unityImpact;
		if (num > compressTreshold)
		{
			num = (num - compressTreshold) / compressRatio + compressTreshold;
		}
		if (num > limitTreshold)
		{
			num = limitTreshold;
		}
		if (shatter != null)
		{
			shatter.PlayOneShot(pos, num);
		}
		if (audioShatter != null)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Audio Shatter Playing ");
			}
			audioShatter.Play();
		}
	}

	public void Crack(float impact, Vector3 pos)
	{
		float num = impact / unityImpact;
		if (num > compressTreshold)
		{
			num = (num - compressTreshold) / compressRatio + compressTreshold;
		}
		if (crack != null)
		{
			crack.PlayOneShot(pos, num);
		}
		if (audioCrack != null)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Audio Crack Playing ");
			}
			SignalScriptNode1 component = GetComponent<SignalScriptNode1>();
			component.sendSignal.value = 1f;
		}
	}
}
