using System.Collections.Generic;
using UnityEngine;

public class AudioCurves : MonoBehaviour
{
	public class AudioCurve
	{
		private float _near;

		private float _far;

		private float _falloffStart;

		private float _falloff48db;

		private float _falloffFocus;

		private AnimationCurve _animationCurve;

		public AudioCurve(float near, float far, float falloffStart, float falloff48db, float falloffFocus, AnimationCurve animationCurve)
		{
			_near = near;
			_far = far;
			_falloffStart = falloffStart;
			_falloff48db = falloff48db;
			_falloffFocus = falloffFocus;
			_animationCurve = animationCurve;
		}

		public AnimationCurve TestVolumeFalloffFromTo(float near, float far, float falloffStart, float falloff48db, float falloffFocus)
		{
			if (_near == near && _far == far && _falloffStart == falloffStart && _falloff48db == falloff48db && _falloffFocus == falloffFocus)
			{
				return _animationCurve;
			}
			return null;
		}
	}

	private static List<AudioCurve> audioCurvesVolumeFalloffFromTo = new List<AudioCurve>();

	public static AnimationCurve GetExistingVolumeFalloffFromTo(float near, float far, float falloffStart, float falloff48db, float falloffFocus)
	{
		foreach (AudioCurve item in audioCurvesVolumeFalloffFromTo)
		{
			AnimationCurve animationCurve = item.TestVolumeFalloffFromTo(near, far, falloffStart, falloff48db, falloffFocus);
			if (animationCurve != null)
			{
				return animationCurve;
			}
		}
		return null;
	}

	public static void SetExistingVolumeFalloffFromTo(float near, float far, float falloffStart, float falloff48db, float falloffFocus, AnimationCurve animationCurve)
	{
		AudioCurve item = new AudioCurve(near, far, falloffStart, falloff48db, falloffFocus, animationCurve);
		audioCurvesVolumeFalloffFromTo.Add(item);
	}
}
