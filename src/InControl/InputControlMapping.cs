using UnityEngine;

namespace InControl
{
	public class InputControlMapping
	{
		public InputControlSource Source;

		public InputControlType Target;

		public bool Invert;

		public float Scale = 1f;

		public bool Raw;

		public bool Passive;

		public bool IgnoreInitialZeroValue;

		public float Sensitivity = 1f;

		public float LowerDeadZone;

		public float UpperDeadZone = 1f;

		public InputRange SourceRange = InputRange.MinusOneToOne;

		public InputRange TargetRange = InputRange.MinusOneToOne;

		private string handle;

		public string Handle
		{
			get
			{
				return (!string.IsNullOrEmpty(handle)) ? handle : Target.ToString();
			}
			set
			{
				handle = value;
			}
		}

		public float MapValue(float value)
		{
			if (Raw)
			{
				value *= Scale;
				value = ((!SourceRange.Excludes(value)) ? value : 0f);
			}
			else
			{
				value = Mathf.Clamp(value * Scale, -1f, 1f);
				value = InputRange.Remap(value, SourceRange, TargetRange);
			}
			if (Invert)
			{
				value = 0f - value;
			}
			return value;
		}
	}
}
