using HumanAPI;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralDroneMix : MonoBehaviour
{
	public Sound2 music;

	private List<Sound2> activeSounds = new List<Sound2>();

	private List<Sound2> inactiveSounds = new List<Sound2>();

	public float fadeMinDuration = 5f;

	public float fadeMaxDuration = 10f;

	public float eventMinDuration = 10f;

	public float eventMaxDuration = 30f;

	public float duckVolume = 0.2f;

	public float duckIn = 2f;

	public float duckOut = 5f;

	public int minVoices = 1;

	public int maxVoices = 4;

	private float duckPhase = 1f;

	private float duckPhaseSpeed;

	private float unduckAfter = float.MaxValue;

	private float masterPhase;

	private float masterPhaseSpeed;

	private float nextEvent;

	private bool playing;

	private float time;

	public void Duck(Sound2 mainsound)
	{
		if (mainsound == null || mainsound.activeClip == null || mainsound.activeClip.clip == null)
		{
			Duck(1f);
		}
		else
		{
			Duck(mainsound.activeClip.clip.length);
		}
	}

	public void Duck(float length)
	{
		duckPhaseSpeed = -1f / duckIn;
		unduckAfter = length - duckOut;
	}

	private void Awake()
	{
		Sound2[] componentsInChildren = GetComponentsInChildren<Sound2>();
		inactiveSounds.AddRange(componentsInChildren);
	}

	public void Play(float fadeInDuration)
	{
		playing = true;
		masterPhaseSpeed = 1f / fadeInDuration;
		if (activeSounds.Count == 0)
		{
			AddVoice(Random.Range(fadeMinDuration, fadeMaxDuration));
		}
		ApplyMasterVolume();
	}

	public void Stop(float fadeOutDuration)
	{
		masterPhaseSpeed = -1f / fadeOutDuration;
	}

	public void Update()
	{
		if (!playing)
		{
			return;
		}
		for (int i = 0; i < inactiveSounds.Count; i++)
		{
			if (inactiveSounds[i].soundSample == null || !inactiveSounds[i].soundSample.loaded)
			{
				return;
			}
		}
		time += Time.unscaledDeltaTime;
		if (nextEvent < time)
		{
			nextEvent = time + Random.Range(eventMaxDuration, eventMaxDuration);
			float fade = Random.Range(fadeMinDuration, fadeMaxDuration);
			bool flag = false;
			while (!flag)
			{
				float value = Random.value;
				flag = ((!(value < 0.5f)) ? ((!(value < 0.75f)) ? RemoveVoice(fade) : AddVoice(fade)) : CrossFadeVoice(fade));
			}
		}
		masterPhase += masterPhaseSpeed * Time.unscaledDeltaTime;
		ApplyMasterVolume();
		if (masterPhaseSpeed < 0f && masterPhase <= 0f)
		{
			playing = false;
		}
	}

	private void ApplyMasterVolume()
	{
		masterPhase = Mathf.Clamp01(masterPhase);
		float num = Mathf.Sqrt(masterPhase);
		unduckAfter -= Time.unscaledDeltaTime;
		if (unduckAfter <= 0f)
		{
			duckPhaseSpeed = 1f / duckIn;
		}
		duckPhase += duckPhaseSpeed * Time.unscaledDeltaTime;
		duckPhase = Mathf.Clamp01(duckPhase);
		num *= (1f - duckVolume) * Mathf.Sqrt(duckPhase) + duckVolume;
		if (duckPhase == 1f)
		{
			unduckAfter = float.MaxValue;
			duckPhaseSpeed = 0f;
		}
		for (int i = 0; i < activeSounds.Count; i++)
		{
			activeSounds[i].SetVolume(num);
		}
		for (int j = 0; j < inactiveSounds.Count; j++)
		{
			inactiveSounds[j].SetVolume(num);
		}
	}

	private bool AddVoice(float fade)
	{
		//Discarded unreachable code: IL_009e
		if (activeSounds.Count >= maxVoices)
		{
			return false;
		}
		int index;
		do
		{
			index = Random.Range(0, inactiveSounds.Count);
		}
		while (inactiveSounds[index].isPlaying);
		inactiveSounds[index].FadeIn(inactiveSounds[index].soundSample.clips[0], loop: true, 0f, fade);
		activeSounds.Add(inactiveSounds[index]);
		inactiveSounds.RemoveAt(index);
		return true;
	}

	private bool RemoveVoice(float fade)
	{
		if (activeSounds.Count <= minVoices)
		{
			return false;
		}
		int index = Random.Range(0, activeSounds.Count);
		activeSounds[index].FadeOut(fade);
		inactiveSounds.Add(activeSounds[index]);
		activeSounds.RemoveAt(index);
		return true;
	}

	private bool CrossFadeVoice(float fade)
	{
		if (activeSounds.Count >= maxVoices)
		{
			return false;
		}
		if (activeSounds.Count <= minVoices)
		{
			return false;
		}
		RemoveVoice(fade);
		AddVoice(fade);
		return true;
	}
}
