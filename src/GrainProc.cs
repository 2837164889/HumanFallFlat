using System;
using UnityEngine;

public class GrainProc : MonoBehaviour
{
	public AudioClip clip;

	private System.Random RandomNumber = new System.Random();

	private float[] samples;

	private int dataChannels;

	private const int grains = 50;

	[Range(0f, 1f)]
	public float pos;

	[Range(-0.5f, 2f)]
	public float tune = 1f;

	[Range(0f, 1f)]
	public float density = 1f;

	[Range(0f, 1f)]
	public float volume = 1f;

	public bool play;

	private float grainsize = 441f;

	private float[] grainStarts = new float[50];

	private float[] grainPhases = new float[50];

	private float[] grainSpeed = new float[50];

	private float[] grainVolume = new float[50];

	private int idx;

	private void Start()
	{
		dataChannels = clip.channels;
		samples = new float[clip.samples * clip.channels];
		clip.GetData(samples, 0);
		for (int i = 0; i < 50; i++)
		{
			StartGrain(i);
		}
	}

	private void OnAudioFilterRead(float[] data, int channels)
	{
		pos += 0.0001f;
		if (pos > 1f)
		{
			pos = 0f;
		}
		if (samples == null)
		{
			return;
		}
		for (int i = 0; i < data.Length; i += channels)
		{
			float num = 0f;
			for (int j = 0; j < 50; j++)
			{
				if (grainPhases[j] >= 0f)
				{
					int num2 = Mathf.FloorToInt(grainStarts[j] + grainPhases[j] * grainsize) * dataChannels;
					if (num2 < samples.Length)
					{
						num += samples[num2] * Mathf.Clamp01((0.5f - Mathf.Abs(0.5f - grainPhases[j])) * 4f) * grainVolume[j];
					}
				}
				grainPhases[j] += grainSpeed[j];
				if (grainPhases[j] >= 1f)
				{
					StartGrain(j);
				}
			}
			data[i] = num;
			if (channels == 2)
			{
				data[i + 1] = data[i];
			}
		}
	}

	private void StartGrain(int s)
	{
		grainPhases[s] = (1f - density) * -5f * (float)RandomNumber.NextDouble();
		grainStarts[s] = Mathf.Clamp01((float)(1.0 - 2.0 * RandomNumber.NextDouble()) * 0.01f + pos) * ((float)samples.Length - grainsize) / (float)dataChannels;
		grainSpeed[s] = 1f / grainsize * tune;
		grainVolume[s] = ((!play) ? 0f : volume);
	}
}
