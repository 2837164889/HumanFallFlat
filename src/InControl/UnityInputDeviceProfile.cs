using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace InControl
{
	public class UnityInputDeviceProfile : UnityInputDeviceProfileBase
	{
		[SerializeField]
		protected string[] JoystickNames;

		[SerializeField]
		protected string[] JoystickRegex;

		[SerializeField]
		protected string LastResortRegex;

		protected static InputControlSource Button0 = Button(0);

		protected static InputControlSource Button1 = Button(1);

		protected static InputControlSource Button2 = Button(2);

		protected static InputControlSource Button3 = Button(3);

		protected static InputControlSource Button4 = Button(4);

		protected static InputControlSource Button5 = Button(5);

		protected static InputControlSource Button6 = Button(6);

		protected static InputControlSource Button7 = Button(7);

		protected static InputControlSource Button8 = Button(8);

		protected static InputControlSource Button9 = Button(9);

		protected static InputControlSource Button10 = Button(10);

		protected static InputControlSource Button11 = Button(11);

		protected static InputControlSource Button12 = Button(12);

		protected static InputControlSource Button13 = Button(13);

		protected static InputControlSource Button14 = Button(14);

		protected static InputControlSource Button15 = Button(15);

		protected static InputControlSource Button16 = Button(16);

		protected static InputControlSource Button17 = Button(17);

		protected static InputControlSource Button18 = Button(18);

		protected static InputControlSource Button19 = Button(19);

		protected static InputControlSource Analog0 = Analog(0);

		protected static InputControlSource Analog1 = Analog(1);

		protected static InputControlSource Analog2 = Analog(2);

		protected static InputControlSource Analog3 = Analog(3);

		protected static InputControlSource Analog4 = Analog(4);

		protected static InputControlSource Analog5 = Analog(5);

		protected static InputControlSource Analog6 = Analog(6);

		protected static InputControlSource Analog7 = Analog(7);

		protected static InputControlSource Analog8 = Analog(8);

		protected static InputControlSource Analog9 = Analog(9);

		protected static InputControlSource Analog10 = Analog(10);

		protected static InputControlSource Analog11 = Analog(11);

		protected static InputControlSource Analog12 = Analog(12);

		protected static InputControlSource Analog13 = Analog(13);

		protected static InputControlSource Analog14 = Analog(14);

		protected static InputControlSource Analog15 = Analog(15);

		protected static InputControlSource Analog16 = Analog(16);

		protected static InputControlSource Analog17 = Analog(17);

		protected static InputControlSource Analog18 = Analog(18);

		protected static InputControlSource Analog19 = Analog(19);

		protected static InputControlSource MenuKey = new UnityKeyCodeSource(KeyCode.Menu);

		protected static InputControlSource EscapeKey = new UnityKeyCodeSource(KeyCode.Escape);

		protected static InputControlSource MouseButton0 = new UnityMouseButtonSource(0);

		protected static InputControlSource MouseButton1 = new UnityMouseButtonSource(1);

		protected static InputControlSource MouseButton2 = new UnityMouseButtonSource(2);

		protected static InputControlSource MouseXAxis = new UnityMouseAxisSource("x");

		protected static InputControlSource MouseYAxis = new UnityMouseAxisSource("y");

		protected static InputControlSource MouseScrollWheel = new UnityMouseAxisSource("z");

		[SerializeField]
		public VersionInfo MinUnityVersion
		{
			get;
			protected set;
		}

		[SerializeField]
		public VersionInfo MaxUnityVersion
		{
			get;
			protected set;
		}

		public override bool IsJoystick => LastResortRegex != null || (JoystickNames != null && JoystickNames.Length > 0) || (JoystickRegex != null && JoystickRegex.Length > 0);

		public override bool IsSupportedOnThisPlatform => IsSupportedOnThisVersionOfUnity && base.IsSupportedOnThisPlatform;

		private bool IsSupportedOnThisVersionOfUnity
		{
			get
			{
				VersionInfo a = VersionInfo.UnityVersion();
				return a >= MinUnityVersion && a <= MaxUnityVersion;
			}
		}

		public UnityInputDeviceProfile()
		{
			base.Sensitivity = 1f;
			base.LowerDeadZone = 0.2f;
			base.UpperDeadZone = 0.9f;
			MinUnityVersion = VersionInfo.Min;
			MaxUnityVersion = VersionInfo.Max;
		}

		public override bool HasJoystickName(string joystickName)
		{
			if (base.IsNotJoystick)
			{
				return false;
			}
			if (JoystickNames != null && JoystickNames.Contains(joystickName, StringComparer.OrdinalIgnoreCase))
			{
				return true;
			}
			if (JoystickRegex != null)
			{
				for (int i = 0; i < JoystickRegex.Length; i++)
				{
					if (Regex.IsMatch(joystickName, JoystickRegex[i], RegexOptions.IgnoreCase))
					{
						return true;
					}
				}
			}
			return false;
		}

		public override bool HasLastResortRegex(string joystickName)
		{
			if (base.IsNotJoystick)
			{
				return false;
			}
			if (LastResortRegex == null)
			{
				return false;
			}
			return Regex.IsMatch(joystickName, LastResortRegex, RegexOptions.IgnoreCase);
		}

		public override bool HasJoystickOrRegexName(string joystickName)
		{
			return HasJoystickName(joystickName) || HasLastResortRegex(joystickName);
		}

		protected static InputControlSource Button(int index)
		{
			return new UnityButtonSource(index);
		}

		protected static InputControlSource Analog(int index)
		{
			return new UnityAnalogSource(index);
		}

		protected static InputControlMapping LeftStickLeftMapping(InputControlSource analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "Left Stick Left";
			inputControlMapping.Target = InputControlType.LeftStickLeft;
			inputControlMapping.Source = analog;
			inputControlMapping.SourceRange = InputRange.ZeroToMinusOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping LeftStickRightMapping(InputControlSource analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "Left Stick Right";
			inputControlMapping.Target = InputControlType.LeftStickRight;
			inputControlMapping.Source = analog;
			inputControlMapping.SourceRange = InputRange.ZeroToOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping LeftStickUpMapping(InputControlSource analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "Left Stick Up";
			inputControlMapping.Target = InputControlType.LeftStickUp;
			inputControlMapping.Source = analog;
			inputControlMapping.SourceRange = InputRange.ZeroToMinusOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping LeftStickDownMapping(InputControlSource analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "Left Stick Down";
			inputControlMapping.Target = InputControlType.LeftStickDown;
			inputControlMapping.Source = analog;
			inputControlMapping.SourceRange = InputRange.ZeroToOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping RightStickLeftMapping(InputControlSource analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "Right Stick Left";
			inputControlMapping.Target = InputControlType.RightStickLeft;
			inputControlMapping.Source = analog;
			inputControlMapping.SourceRange = InputRange.ZeroToMinusOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping RightStickRightMapping(InputControlSource analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "Right Stick Right";
			inputControlMapping.Target = InputControlType.RightStickRight;
			inputControlMapping.Source = analog;
			inputControlMapping.SourceRange = InputRange.ZeroToOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping RightStickUpMapping(InputControlSource analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "Right Stick Up";
			inputControlMapping.Target = InputControlType.RightStickUp;
			inputControlMapping.Source = analog;
			inputControlMapping.SourceRange = InputRange.ZeroToMinusOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping RightStickDownMapping(InputControlSource analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "Right Stick Down";
			inputControlMapping.Target = InputControlType.RightStickDown;
			inputControlMapping.Source = analog;
			inputControlMapping.SourceRange = InputRange.ZeroToOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping LeftTriggerMapping(InputControlSource analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "Left Trigger";
			inputControlMapping.Target = InputControlType.LeftTrigger;
			inputControlMapping.Source = analog;
			inputControlMapping.SourceRange = InputRange.MinusOneToOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			inputControlMapping.IgnoreInitialZeroValue = true;
			return inputControlMapping;
		}

		protected static InputControlMapping RightTriggerMapping(InputControlSource analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "Right Trigger";
			inputControlMapping.Target = InputControlType.RightTrigger;
			inputControlMapping.Source = analog;
			inputControlMapping.SourceRange = InputRange.MinusOneToOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			inputControlMapping.IgnoreInitialZeroValue = true;
			return inputControlMapping;
		}

		protected static InputControlMapping DPadLeftMapping(InputControlSource analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "DPad Left";
			inputControlMapping.Target = InputControlType.DPadLeft;
			inputControlMapping.Source = analog;
			inputControlMapping.SourceRange = InputRange.ZeroToMinusOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping DPadRightMapping(InputControlSource analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "DPad Right";
			inputControlMapping.Target = InputControlType.DPadRight;
			inputControlMapping.Source = analog;
			inputControlMapping.SourceRange = InputRange.ZeroToOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping DPadUpMapping(InputControlSource analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "DPad Up";
			inputControlMapping.Target = InputControlType.DPadUp;
			inputControlMapping.Source = analog;
			inputControlMapping.SourceRange = InputRange.ZeroToMinusOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping DPadDownMapping(InputControlSource analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "DPad Down";
			inputControlMapping.Target = InputControlType.DPadDown;
			inputControlMapping.Source = analog;
			inputControlMapping.SourceRange = InputRange.ZeroToOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping DPadUpMapping2(InputControlSource analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "DPad Up";
			inputControlMapping.Target = InputControlType.DPadUp;
			inputControlMapping.Source = analog;
			inputControlMapping.SourceRange = InputRange.ZeroToOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}

		protected static InputControlMapping DPadDownMapping2(InputControlSource analog)
		{
			InputControlMapping inputControlMapping = new InputControlMapping();
			inputControlMapping.Handle = "DPad Down";
			inputControlMapping.Target = InputControlType.DPadDown;
			inputControlMapping.Source = analog;
			inputControlMapping.SourceRange = InputRange.ZeroToMinusOne;
			inputControlMapping.TargetRange = InputRange.ZeroToOne;
			return inputControlMapping;
		}
	}
}
