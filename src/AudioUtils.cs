using UnityEngine;

public static class AudioUtils
{
	private static float[] window = new float[882];

	public static float ImpactToVolume(float impact, float unitImpact)
	{
		return Mathf.Pow(impact / unitImpact, 0.75f);
	}

	public static float VelocityToVolume(float velocity, float unitVelocity)
	{
		return Mathf.Sqrt(velocity / unitVelocity);
	}

	public static float CalculateRMS(AudioClip clip)
	{
		float[] array = new float[clip.samples * clip.channels];
		clip.GetData(array, 0);
		return CalculateRMS(array, clip.channels, 0);
	}

	public static float CalculateRMS(float[] samples, int channels, int channel)
	{
		float num = 0f;
		int num2 = samples.Length / channels;
		int num3 = window.Length;
		int num4 = 0;
		float num5 = 0f;
		for (int i = 0; i < num2; i++)
		{
			float num6 = samples[i * channels + channel];
			window[num4] = num6 * num6;
			num += window[num4];
			num4 = (num4 + 1) % num3;
			if (i >= num3)
			{
				if (num > num5)
				{
					num5 = num;
				}
				num -= window[num4];
			}
		}
		if (num2 <= num3)
		{
			return Mathf.Sqrt(num / (float)num2);
		}
		return Mathf.Sqrt(num5 / (float)num3);
	}

	public static float ValueToDB(float value)
	{
		float num = Mathf.Log10(value);
		return 20f * num;
	}

	public static float DBToValue(float decibel, float zeroVolumeDB = -120f)
	{
		if (decibel <= zeroVolumeDB)
		{
			return 0f;
		}
		float p = decibel / 20f;
		return Mathf.Pow(10f, p);
	}

	public static AudioClip Random(this AudioClip[] clips)
	{
		if (clips.Length == 0)
		{
			return null;
		}
		return clips[UnityEngine.Random.Range(0, clips.Length)];
	}

	public static float CentsToRatio(float cents)
	{
		if (cents == 0f)
		{
			return 1f;
		}
		return Mathf.Pow(2f, cents / 1200f);
	}

	public static float RatioToCents(float ratio)
	{
		if (ratio == 1f)
		{
			return 0f;
		}
		return 1200f * Mathf.Log(ratio, 2f);
	}
}
