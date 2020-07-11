using HumanAPI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ReverbManager : MonoBehaviour, IReset
{
	public static ReverbManager instance;

	public AudioMixer mainMixer;

	[NonSerialized]
	public List<ReverbZone> zones = new List<ReverbZone>();

	private void OnEnable()
	{
		instance = this;
	}

	public void ZoneEntered(ReverbZone zone)
	{
		zones.Add(zone);
	}

	public void ZoneLeft(ReverbZone zone)
	{
		zones.Remove(zone);
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		zones.Clear();
	}

	private void Update()
	{
		if (Game.instance == null)
		{
			return;
		}
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		float num6 = 0f;
		for (int i = 0; i < Listener.instance.earList.Count; i++)
		{
			Vector3 position = Listener.instance.earList[i].position;
			for (int j = 0; j < zones.Count; j++)
			{
				ReverbZone reverbZone = zones[j];
				if (reverbZone == null)
				{
					zones.RemoveAt(j);
					j--;
					continue;
				}
				float weight = zones[j].GetWeight(position);
				num += weight;
				num2 += weight * reverbZone.level;
				num3 += weight * reverbZone.delay;
				num4 += weight * reverbZone.diffusion;
				num5 += weight * Mathf.Log10(reverbZone.lowPass);
				num6 += weight * Mathf.Log10(reverbZone.highPass);
			}
		}
		if (num != 0f)
		{
			num2 /= num;
			num3 /= num;
			num4 /= num;
			num5 /= num;
			num6 /= num;
			GameAudio.instance.SetReverbLevel(num2);
			mainMixer.SetFloat("ReverbDelay", num3);
			mainMixer.SetFloat("ReverbDiffusion", num4);
			mainMixer.SetFloat("ReverbHighpass", Mathf.Pow(10f, num6));
			mainMixer.SetFloat("ReverbLowpass", Mathf.Pow(10f, num5));
		}
	}
}
