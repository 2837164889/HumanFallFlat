using UnityEngine;

namespace InControl
{
	public struct InputRange
	{
		public static readonly InputRange None = new InputRange(0f, 0f, InputRangeType.None);

		public static readonly InputRange MinusOneToOne = new InputRange(-1f, 1f, InputRangeType.MinusOneToOne);

		public static readonly InputRange OneToMinusOne = new InputRange(1f, -1f, InputRangeType.OneToMinusOne);

		public static readonly InputRange ZeroToOne = new InputRange(0f, 1f, InputRangeType.ZeroToOne);

		public static readonly InputRange ZeroToMinusOne = new InputRange(0f, -1f, InputRangeType.ZeroToMinusOne);

		public static readonly InputRange OneToZero = new InputRange(1f, 0f, InputRangeType.OneToZero);

		public static readonly InputRange MinusOneToZero = new InputRange(-1f, 0f, InputRangeType.MinusOneToZero);

		public static readonly InputRange ZeroToNegativeInfinity = new InputRange(0f, float.NegativeInfinity, InputRangeType.ZeroToNegativeInfinity);

		public static readonly InputRange ZeroToPositiveInfinity = new InputRange(0f, float.PositiveInfinity, InputRangeType.ZeroToPositiveInfinity);

		public static readonly InputRange Everything = new InputRange(float.NegativeInfinity, float.PositiveInfinity, InputRangeType.Everything);

		private static readonly InputRange[] TypeToRange = new InputRange[10]
		{
			None,
			MinusOneToOne,
			OneToMinusOne,
			ZeroToOne,
			ZeroToMinusOne,
			OneToZero,
			MinusOneToZero,
			ZeroToNegativeInfinity,
			ZeroToPositiveInfinity,
			Everything
		};

		public readonly float Value0;

		public readonly float Value1;

		public readonly InputRangeType Type;

		private InputRange(float value0, float value1, InputRangeType type)
		{
			Value0 = value0;
			Value1 = value1;
			Type = type;
		}

		public InputRange(InputRangeType type)
		{
			Value0 = TypeToRange[(int)type].Value0;
			Value1 = TypeToRange[(int)type].Value1;
			Type = type;
		}

		public bool Includes(float value)
		{
			return !Excludes(value);
		}

		public bool Excludes(float value)
		{
			if (Type == InputRangeType.None)
			{
				return true;
			}
			return value < Mathf.Min(Value0, Value1) || value > Mathf.Max(Value0, Value1);
		}

		public static float Remap(float value, InputRange sourceRange, InputRange targetRange)
		{
			if (sourceRange.Excludes(value))
			{
				return 0f;
			}
			float t = Mathf.InverseLerp(sourceRange.Value0, sourceRange.Value1, value);
			return Mathf.Lerp(targetRange.Value0, targetRange.Value1, t);
		}

		internal static float Remap(float value, InputRangeType sourceRangeType, InputRangeType targetRangeType)
		{
			InputRange sourceRange = TypeToRange[(int)sourceRangeType];
			InputRange targetRange = TypeToRange[(int)targetRangeType];
			return Remap(value, sourceRange, targetRange);
		}
	}
}
