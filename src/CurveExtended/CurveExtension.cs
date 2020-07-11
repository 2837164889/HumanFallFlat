using UnityEngine;

namespace CurveExtended
{
	public static class CurveExtension
	{
		public static void UpdateAllLinearTangents(this AnimationCurve curve)
		{
			for (int i = 0; i < curve.keys.Length; i++)
			{
				UpdateTangentsFromMode(curve, i);
			}
		}

		public static void UpdateTangentsFromMode(AnimationCurve curve, int index)
		{
			if (index >= 0 && index < curve.length)
			{
				Keyframe key = curve[index];
				if (index >= 1)
				{
					key.inTangent = CalculateLinearTangent(curve, index, index - 1);
					curve.MoveKey(index, key);
				}
				if (index + 1 < curve.length)
				{
					key.outTangent = CalculateLinearTangent(curve, index, index + 1);
					curve.MoveKey(index, key);
				}
			}
		}

		private static float CalculateLinearTangent(AnimationCurve curve, int index, int toIndex)
		{
			return (float)(((double)curve[index].value - (double)curve[toIndex].value) / ((double)curve[index].time - (double)curve[toIndex].time));
		}
	}
}
