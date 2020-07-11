using HumanAPI;
using System.Collections.Generic;
using UnityEngine;

public class SampleLibrary : MonoBehaviour
{
	public AudioSourcePoolId poolId;

	public float levelDB = -12f;

	private float volumeAdjust;

	private float compressorTreshold;

	public int maxVoices = 3;

	public int maxVoicesPerEntity = 3;

	public float blockTime;

	public bool dontBlockLouder;

	public float rndVolumeMin = 1f;

	public float rndVolumeMax = 1f;

	public float rndPitchMin = 1f;

	public float rndPitchMax = 1f;

	public float compressorTresholdDB;

	public float compressorRate = 8f;

	private float lastSampleTime;

	public float unitImpact = 40f;

	public float unitVelocity = 10f;

	public ImpactVolumeMix volumeMix = ImpactVolumeMix.Lerp;

	public float lerpImpactWeight = 0.5f;

	public bool isSlide;

	public SampleLibrary slideLibrary;

	public AudioClip[] clips;

	public float[] clipsRms;

	private List<float> polyphonyBlockUntil = new List<float>();

	private List<float> playingLoudness = new List<float>();

	private float blockUntil;

	private void OnEnable()
	{
		if (clipsRms.Length == 0)
		{
			volumeAdjust = 0f;
		}
		else
		{
			volumeAdjust = AudioUtils.DBToValue(levelDB) / clipsRms[0];
		}
		compressorTreshold = AudioUtils.DBToValue(compressorTresholdDB);
	}

	private bool CanPlay(float volume)
	{
		if (volumeAdjust == 0f)
		{
			return false;
		}
		if (volume < 0.001f)
		{
			return false;
		}
		float time = Time.time;
		bool flag = true;
		for (int num = polyphonyBlockUntil.Count - 1; num >= 0; num--)
		{
			if (polyphonyBlockUntil[num] <= time)
			{
				polyphonyBlockUntil.RemoveAt(num);
				playingLoudness.RemoveAt(num);
			}
			else if (volume < playingLoudness[num])
			{
				flag = false;
			}
		}
		if (!dontBlockLouder || !flag)
		{
			if (blockUntil > time)
			{
				return false;
			}
			if (polyphonyBlockUntil.Count >= maxVoices)
			{
				return false;
			}
		}
		return true;
	}

	public bool PlayRMS(AudioChannel channel, Vector3 pos, float rms, float pitch)
	{
		float num = rms;
		float num2 = pitch;
		rms *= volumeAdjust;
		rms = ((rndVolumeMin != rndVolumeMax) ? (rms * Random.Range(rndVolumeMin, rndVolumeMax)) : (rms * rndVolumeMin));
		if (!CanPlay(rms))
		{
			return false;
		}
		pitch = ((rndPitchMin != rndPitchMax) ? (pitch * Random.Range(rndPitchMin, rndPitchMax)) : (pitch * rndPitchMin));
		int num3 = Random.Range(0, clips.Length);
		AudioClip audioClip = clips[num3];
		float volumeScale = rms / clipsRms[num3];
		AudioSource audioSource = AudioSourcePool.Allocate(poolId, null, pos);
		audioSource.pitch = pitch;
		audioSource.outputAudioMixerGroup = AudioRouting.GetChannel(channel);
		audioSource.volume = 1f;
		audioSource.PlayOneShot(audioClip, volumeScale);
		float time = Time.time;
		if (blockTime > 0f)
		{
			blockUntil = Mathf.Max(blockUntil, time + blockTime);
		}
		if (maxVoices > 0)
		{
			polyphonyBlockUntil.Add(time + audioClip.length);
			playingLoudness.Add(rms);
		}
		return true;
	}
}
