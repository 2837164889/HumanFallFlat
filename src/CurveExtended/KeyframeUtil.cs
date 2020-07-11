using UnityEngine;

namespace CurveExtended
{
	public class KeyframeUtil
	{
		public static Keyframe GetNew(float time, float value, TangentMode leftAndRight)
		{
			return GetNew(time, value, leftAndRight, leftAndRight);
		}

		public static Keyframe GetNew(float time, float value, TangentMode left, TangentMode right)
		{
			Keyframe result = new Keyframe(time, value);
			if (left == TangentMode.Stepped)
			{
				result.inTangent = float.PositiveInfinity;
			}
			if (right == TangentMode.Stepped)
			{
				result.outTangent = float.PositiveInfinity;
			}
			return result;
		}

		public static TangentMode GetKeyTangentMode(int tangentMode, int leftRight)
		{
			if (leftRight == 0)
			{
				return (TangentMode)((tangentMode & 6) >> 1);
			}
			return (TangentMode)((tangentMode & 0x18) >> 3);
		}
	}
}
