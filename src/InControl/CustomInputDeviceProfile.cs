using System;
using UnityEngine;

namespace InControl
{
	[Obsolete("Custom profiles are deprecated. Use the bindings API instead.", false)]
	public class CustomInputDeviceProfile : UnityInputDeviceProfileBase
	{
		protected static InputControlSource MouseButton0 = new UnityMouseButtonSource(0);

		protected static InputControlSource MouseButton1 = new UnityMouseButtonSource(1);

		protected static InputControlSource MouseButton2 = new UnityMouseButtonSource(2);

		protected static InputControlSource MouseXAxis = new UnityMouseAxisSource("x");

		protected static InputControlSource MouseYAxis = new UnityMouseAxisSource("y");

		protected static InputControlSource MouseScrollWheel = new UnityMouseAxisSource("z");

		public sealed override bool IsJoystick => false;

		public CustomInputDeviceProfile()
		{
			base.Name = "Custom Device Profile";
			base.Meta = "Custom Device Profile";
			base.IncludePlatforms = new string[3]
			{
				"Windows",
				"Mac",
				"Linux"
			};
			base.Sensitivity = 1f;
			base.LowerDeadZone = 0f;
			base.UpperDeadZone = 1f;
		}

		public sealed override bool HasJoystickName(string joystickName)
		{
			return false;
		}

		public sealed override bool HasLastResortRegex(string joystickName)
		{
			return false;
		}

		public sealed override bool HasJoystickOrRegexName(string joystickName)
		{
			return false;
		}

		protected static InputControlSource KeyCodeButton(params KeyCode[] keyCodeList)
		{
			return new UnityKeyCodeSource(keyCodeList);
		}

		protected static InputControlSource KeyCodeComboButton(params KeyCode[] keyCodeList)
		{
			return new UnityKeyCodeComboSource(keyCodeList);
		}
	}
}
