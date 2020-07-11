namespace InControl
{
	[AutoDiscover]
	public class PlayStation4Profile : UnityInputDeviceProfile
	{
		public PlayStation4Profile()
		{
			string text = "®";
			base.Name = "DUALSHOCK" + text + "4 wireless controller";
			base.Meta = "DUALSHOCK" + text + "4 wireless controller on PlayStation" + text + "4 system";
			base.DeviceClass = InputDeviceClass.Controller;
			base.DeviceStyle = InputDeviceStyle.PlayStation4;
			base.IncludePlatforms = new string[1]
			{
				"PS4"
			};
			JoystickRegex = new string[1]
			{
				"controller"
			};
			base.ButtonMappings = new InputControlMapping[10]
			{
				new InputControlMapping
				{
					Handle = "cross button",
					Target = InputControlType.Action1,
					Source = UnityInputDeviceProfile.Button0
				},
				new InputControlMapping
				{
					Handle = "circle button",
					Target = InputControlType.Action2,
					Source = UnityInputDeviceProfile.Button1
				},
				new InputControlMapping
				{
					Handle = "square button",
					Target = InputControlType.Action3,
					Source = UnityInputDeviceProfile.Button2
				},
				new InputControlMapping
				{
					Handle = "triangle button",
					Target = InputControlType.Action4,
					Source = UnityInputDeviceProfile.Button3
				},
				new InputControlMapping
				{
					Handle = "L1 button",
					Target = InputControlType.LeftBumper,
					Source = UnityInputDeviceProfile.Button4
				},
				new InputControlMapping
				{
					Handle = "R1 button",
					Target = InputControlType.RightBumper,
					Source = UnityInputDeviceProfile.Button5
				},
				new InputControlMapping
				{
					Handle = "touch pad button",
					Target = InputControlType.TouchPadButton,
					Source = UnityInputDeviceProfile.Button6
				},
				new InputControlMapping
				{
					Handle = "OPTIONS button",
					Target = InputControlType.Options,
					Source = UnityInputDeviceProfile.Button7
				},
				new InputControlMapping
				{
					Handle = "L3 button",
					Target = InputControlType.LeftStickButton,
					Source = UnityInputDeviceProfile.Button8
				},
				new InputControlMapping
				{
					Handle = "R3 button",
					Target = InputControlType.RightStickButton,
					Source = UnityInputDeviceProfile.Button9
				}
			};
			base.AnalogMappings = new InputControlMapping[14]
			{
				new InputControlMapping
				{
					Handle = "left stick left",
					Target = InputControlType.LeftStickLeft,
					Source = UnityInputDeviceProfile.Analog0,
					SourceRange = InputRange.ZeroToMinusOne,
					TargetRange = InputRange.ZeroToOne
				},
				new InputControlMapping
				{
					Handle = "left stick right",
					Target = InputControlType.LeftStickRight,
					Source = UnityInputDeviceProfile.Analog0,
					SourceRange = InputRange.ZeroToOne,
					TargetRange = InputRange.ZeroToOne
				},
				new InputControlMapping
				{
					Handle = "left stick up",
					Target = InputControlType.LeftStickUp,
					Source = UnityInputDeviceProfile.Analog1,
					SourceRange = InputRange.ZeroToMinusOne,
					TargetRange = InputRange.ZeroToOne
				},
				new InputControlMapping
				{
					Handle = "left stick down",
					Target = InputControlType.LeftStickDown,
					Source = UnityInputDeviceProfile.Analog1,
					SourceRange = InputRange.ZeroToOne,
					TargetRange = InputRange.ZeroToOne
				},
				new InputControlMapping
				{
					Handle = "right stick left",
					Target = InputControlType.RightStickLeft,
					Source = UnityInputDeviceProfile.Analog3,
					SourceRange = InputRange.ZeroToMinusOne,
					TargetRange = InputRange.ZeroToOne
				},
				new InputControlMapping
				{
					Handle = "right stick right",
					Target = InputControlType.RightStickRight,
					Source = UnityInputDeviceProfile.Analog3,
					SourceRange = InputRange.ZeroToOne,
					TargetRange = InputRange.ZeroToOne
				},
				new InputControlMapping
				{
					Handle = "right stick up",
					Target = InputControlType.RightStickUp,
					Source = UnityInputDeviceProfile.Analog4,
					SourceRange = InputRange.ZeroToMinusOne,
					TargetRange = InputRange.ZeroToOne
				},
				new InputControlMapping
				{
					Handle = "right stick down",
					Target = InputControlType.RightStickDown,
					Source = UnityInputDeviceProfile.Analog4,
					SourceRange = InputRange.ZeroToOne,
					TargetRange = InputRange.ZeroToOne
				},
				new InputControlMapping
				{
					Handle = "left button",
					Target = InputControlType.DPadLeft,
					Source = UnityInputDeviceProfile.Analog5,
					SourceRange = InputRange.ZeroToMinusOne,
					TargetRange = InputRange.ZeroToOne
				},
				new InputControlMapping
				{
					Handle = "right button",
					Target = InputControlType.DPadRight,
					Source = UnityInputDeviceProfile.Analog5,
					SourceRange = InputRange.ZeroToOne,
					TargetRange = InputRange.ZeroToOne
				},
				new InputControlMapping
				{
					Handle = "up button",
					Target = InputControlType.DPadUp,
					Source = UnityInputDeviceProfile.Analog6,
					SourceRange = InputRange.ZeroToOne,
					TargetRange = InputRange.ZeroToOne
				},
				new InputControlMapping
				{
					Handle = "down button",
					Target = InputControlType.DPadDown,
					Source = UnityInputDeviceProfile.Analog6,
					SourceRange = InputRange.ZeroToMinusOne,
					TargetRange = InputRange.ZeroToOne
				},
				new InputControlMapping
				{
					Handle = "L2 button",
					Target = InputControlType.LeftTrigger,
					Source = UnityInputDeviceProfile.Analog7
				},
				new InputControlMapping
				{
					Handle = "R2 button",
					Target = InputControlType.RightTrigger,
					Source = UnityInputDeviceProfile.Analog2,
					Invert = true
				}
			};
		}
	}
}
