namespace InControl
{
	public abstract class NativeInputDeviceProfile : InputDeviceProfile
	{
		public NativeInputDeviceMatcher[] Matchers;

		public NativeInputDeviceMatcher[] LastResortMatchers;

		public NativeInputDeviceProfile()
		{
			base.Sensitivity = 1f;
			base.LowerDeadZone = 0.2f;
			base.UpperDeadZone = 0.9f;
		}

		internal bool Matches(NativeDeviceInfo deviceInfo)
		{
			return Matches(deviceInfo, Matchers);
		}

		internal bool LastResortMatches(NativeDeviceInfo deviceInfo)
		{
			return Matches(deviceInfo, LastResortMatchers);
		}

		private bool Matches(NativeDeviceInfo deviceInfo, NativeInputDeviceMatcher[] matchers)
		{
			if (Matchers != null)
			{
				int num = Matchers.Length;
				for (int i = 0; i < num; i++)
				{
					if (Matchers[i].Matches(deviceInfo))
					{
						return true;
					}
				}
			}
			return false;
		}

		protected static InputControlSource Button(int index)
		{
			return new NativeButtonSource(index);
		}

		protected static InputControlSource Analog(int index)
		{
			return new NativeAnalogSource(index);
		}

		protected static InputControlMapping LeftStickLeftMapping(int analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "Left Stick Left";
			inputControlMapping.Target = InputControlType.LeftStickLeft;
			inputControlMapping.Source = Analog(analog);
			inputControlMapping.SourceRange = InputRange.ZeroToMinusOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping LeftStickRightMapping(int analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "Left Stick Right";
			inputControlMapping.Target = InputControlType.LeftStickRight;
			inputControlMapping.Source = Analog(analog);
			inputControlMapping.SourceRange = InputRange.ZeroToOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping LeftStickUpMapping(int analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "Left Stick Up";
			inputControlMapping.Target = InputControlType.LeftStickUp;
			inputControlMapping.Source = Analog(analog);
			inputControlMapping.SourceRange = InputRange.ZeroToMinusOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping LeftStickDownMapping(int analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "Left Stick Down";
			inputControlMapping.Target = InputControlType.LeftStickDown;
			inputControlMapping.Source = Analog(analog);
			inputControlMapping.SourceRange = InputRange.ZeroToOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping LeftStickUpMapping2(int analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "Left Stick Up";
			inputControlMapping.Target = InputControlType.LeftStickUp;
			inputControlMapping.Source = Analog(analog);
			inputControlMapping.SourceRange = InputRange.ZeroToOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping LeftStickDownMapping2(int analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "Left Stick Down";
			inputControlMapping.Target = InputControlType.LeftStickDown;
			inputControlMapping.Source = Analog(analog);
			inputControlMapping.SourceRange = InputRange.ZeroToMinusOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping RightStickLeftMapping(int analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "Right Stick Left";
			inputControlMapping.Target = InputControlType.RightStickLeft;
			inputControlMapping.Source = Analog(analog);
			inputControlMapping.SourceRange = InputRange.ZeroToMinusOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping RightStickRightMapping(int analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "Right Stick Right";
			inputControlMapping.Target = InputControlType.RightStickRight;
			inputControlMapping.Source = Analog(analog);
			inputControlMapping.SourceRange = InputRange.ZeroToOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping RightStickUpMapping(int analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "Right Stick Up";
			inputControlMapping.Target = InputControlType.RightStickUp;
			inputControlMapping.Source = Analog(analog);
			inputControlMapping.SourceRange = InputRange.ZeroToMinusOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping RightStickDownMapping(int analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "Right Stick Down";
			inputControlMapping.Target = InputControlType.RightStickDown;
			inputControlMapping.Source = Analog(analog);
			inputControlMapping.SourceRange = InputRange.ZeroToOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping RightStickUpMapping2(int analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "Right Stick Up";
			inputControlMapping.Target = InputControlType.RightStickUp;
			inputControlMapping.Source = Analog(analog);
			inputControlMapping.SourceRange = InputRange.ZeroToOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping RightStickDownMapping2(int analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "Right Stick Down";
			inputControlMapping.Target = InputControlType.RightStickDown;
			inputControlMapping.Source = Analog(analog);
			inputControlMapping.SourceRange = InputRange.ZeroToMinusOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping LeftTriggerMapping(int analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "Left Trigger";
			inputControlMapping.Target = InputControlType.LeftTrigger;
			inputControlMapping.Source = Analog(analog);
			inputControlMapping.SourceRange = InputRange.MinusOneToOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			inputControlMapping.IgnoreInitialZeroValue = true;
			return inputControlMapping;
		}

		protected static InputControlMapping RightTriggerMapping(int analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "Right Trigger";
			inputControlMapping.Target = InputControlType.RightTrigger;
			inputControlMapping.Source = Analog(analog);
			inputControlMapping.SourceRange = InputRange.MinusOneToOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			inputControlMapping.IgnoreInitialZeroValue = true;
			return inputControlMapping;
		}

		protected static InputControlMapping DPadLeftMapping(int analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "DPad Left";
			inputControlMapping.Target = InputControlType.DPadLeft;
			inputControlMapping.Source = Analog(analog);
			inputControlMapping.SourceRange = InputRange.ZeroToMinusOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping DPadRightMapping(int analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "DPad Right";
			inputControlMapping.Target = InputControlType.DPadRight;
			inputControlMapping.Source = Analog(analog);
			inputControlMapping.SourceRange = InputRange.ZeroToOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping DPadUpMapping(int analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "DPad Up";
			inputControlMapping.Target = InputControlType.DPadUp;
			inputControlMapping.Source = Analog(analog);
			inputControlMapping.SourceRange = InputRange.ZeroToMinusOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping DPadDownMapping(int analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "DPad Down";
			inputControlMapping.Target = InputControlType.DPadDown;
			inputControlMapping.Source = Analog(analog);
			inputControlMapping.SourceRange = InputRange.ZeroToOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping DPadUpMapping2(int analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "DPad Up";
			inputControlMapping.Target = InputControlType.DPadUp;
			inputControlMapping.Source = Analog(analog);
			inputControlMapping.SourceRange = InputRange.ZeroToOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping DPadDownMapping2(int analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "DPad Down";
			inputControlMapping.Target = InputControlType.DPadDown;
			inputControlMapping.Source = Analog(analog);
			inputControlMapping.SourceRange = InputRange.ZeroToMinusOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}
	}
}
