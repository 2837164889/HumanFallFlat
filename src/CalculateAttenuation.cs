using CurveExtended;
using UnityEngine;

public class CalculateAttenuation : MonoBehaviour
{
	public float falloffStart = 1f;

	public float falloffPower = 0.5f;

	public float lpStart = 2f;

	public float lpPower = 0.5f;

	public float spreadNear = 0.5f;

	public float spreadFar;

	public float spatialNear = 0.5f;

	public float spatialFar = 1f;

	public void Generate()
	{
		AudioSource[] components = GetComponents<AudioSource>();
		foreach (AudioSource audioSource in components)
		{
			audioSource.rolloffMode = AudioRolloffMode.Custom;
			audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, VolumeFalloff(falloffStart / audioSource.maxDistance, falloffPower));
			audioSource.SetCustomCurve(AudioSourceCurveType.Spread, Spread(spreadNear, spreadFar, falloffStart / audioSource.maxDistance, falloffPower));
			audioSource.SetCustomCurve(AudioSourceCurveType.SpatialBlend, Spread(spatialNear, spatialFar, falloffStart / audioSource.maxDistance, falloffPower));
		}
		AudioLowPassFilter component = GetComponent<AudioLowPassFilter>();
		if (component != null)
		{
			component.customCutoffCurve = LowPassFalloff(lpStart / components[0].maxDistance, lpPower);
		}
	}

	public static AnimationCurve VolumeFalloffFromTo(float near, float far, float falloffStart, float falloff48db, float falloffFocus)
	{
		AnimationCurve animationCurve = new AnimationCurve();
		animationCurve.AddKey(KeyframeUtil.GetNew(0f, near, TangentMode.Linear));
		if (falloff48db <= falloffStart)
		{
			animationCurve.AddKey(KeyframeUtil.GetNew(1f, near, TangentMode.Linear));
		}
		else if (falloffFocus == 1f)
		{
			float num = falloffStart / (falloffStart + falloff48db);
			float num2 = 1f;
			for (int i = 0; i < 16; i++)
			{
				float time = num + (1f - num) * (float)i / 16f;
				animationCurve.AddKey(KeyframeUtil.GetNew(time, Mathf.Lerp(far, near, num2), TangentMode.Linear));
				num2 /= Mathf.Sqrt(2f);
			}
			animationCurve.AddKey(KeyframeUtil.GetNew(1f, far, TangentMode.Linear));
		}
		else
		{
			float f = Mathf.Pow(2f, 1f - falloffFocus);
			float num3 = falloffStart / (falloffStart + falloff48db);
			float num4 = (1f - Mathf.Pow(f, 8f) * num3) / (1f - Mathf.Pow(f, 8f));
			float num5 = num3 - num4;
			float num6 = 1f;
			for (int j = 0; j < 16; j++)
			{
				animationCurve.AddKey(KeyframeUtil.GetNew(num4 + num5, Mathf.Lerp(far, near, num6), TangentMode.Linear));
				num5 *= Mathf.Sqrt(f);
				num6 /= Mathf.Sqrt(2f);
			}
			animationCurve.AddKey(KeyframeUtil.GetNew(1f, far, TangentMode.Linear));
		}
		animationCurve.UpdateAllLinearTangents();
		return animationCurve;
	}

	public static AnimationCurve VolumeFalloff(float falloffPoint, float falloffPower)
	{
		AnimationCurve animationCurve = new AnimationCurve();
		animationCurve.AddKey(KeyframeUtil.GetNew(0f, 1f, TangentMode.Linear));
		if (falloffPoint >= 1f || falloffPower == 1f)
		{
			animationCurve.AddKey(KeyframeUtil.GetNew(1f, 1f, TangentMode.Linear));
		}
		else
		{
			for (float num = 0f; num < 10f; num += 0.5f)
			{
				float num2 = falloffPoint * Mathf.Pow(2f, num);
				if (num2 > 1f)
				{
					break;
				}
				float value = Mathf.Pow(falloffPower, num) * Mathf.InverseLerp(1f, falloffPoint, num2);
				animationCurve.AddKey(KeyframeUtil.GetNew(num2, value, TangentMode.Linear));
			}
			animationCurve.AddKey(KeyframeUtil.GetNew(1f, 0f, TangentMode.Linear));
		}
		animationCurve.UpdateAllLinearTangents();
		return animationCurve;
	}

	public static AnimationCurve LowPassFalloff(float falloffPoint, float falloffPower)
	{
		AnimationCurve animationCurve = new AnimationCurve();
		animationCurve.AddKey(KeyframeUtil.GetNew(0f, 1f, TangentMode.Linear));
		if (falloffPoint > 29.9f && falloffPower > 0.99f)
		{
			animationCurve.UpdateAllLinearTangents();
			return animationCurve;
		}
		if (falloffPoint >= 1f || falloffPower == 1f)
		{
			animationCurve.AddKey(KeyframeUtil.GetNew(1f, 1f, TangentMode.Linear));
		}
		else
		{
			float num = 1f / falloffPoint;
			for (float num2 = 0f; num2 < 10f; num2 += 0.5f)
			{
				float num3 = falloffPoint * Mathf.Pow(2f, num2);
				if (num3 > 1f)
				{
					break;
				}
				float value = Mathf.Pow(falloffPower, num2);
				animationCurve.AddKey(KeyframeUtil.GetNew(num3, value, TangentMode.Linear));
			}
		}
		animationCurve.UpdateAllLinearTangents();
		return animationCurve;
	}

	public static AnimationCurve Spread(float near, float far, float falloffPoint, float falloffPower)
	{
		AnimationCurve animationCurve = new AnimationCurve();
		animationCurve.AddKey(KeyframeUtil.GetNew(0f, near, TangentMode.Linear));
		if (falloffPoint >= 1f || falloffPower == 1f)
		{
			animationCurve.AddKey(KeyframeUtil.GetNew(1f, near, TangentMode.Linear));
		}
		else
		{
			float num = 1f / falloffPoint;
			for (float num2 = 0f; num2 < 10f; num2 += 0.5f)
			{
				float num3 = falloffPoint * Mathf.Pow(2f, num2);
				if (num3 >= 1f)
				{
					break;
				}
				float value = Mathf.Lerp(far, near, Mathf.Pow(falloffPower, num2));
				animationCurve.AddKey(KeyframeUtil.GetNew(num3, value, TangentMode.Linear));
			}
			animationCurve.AddKey(KeyframeUtil.GetNew(1f, far, TangentMode.Linear));
		}
		animationCurve.UpdateAllLinearTangents();
		return animationCurve;
	}
}
