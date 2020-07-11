namespace InControl
{
	[AutoDiscover]
	public class Xbox360WinProfile : UnityInputDeviceProfile
	{
		public Xbox360WinProfile()
		{
			base.Name = "Xbox 360 Controller";
			base.Meta = "Xbox 360 Controller on Windows";
			base.DeviceClass = InputDeviceClass.Controller;
			base.DeviceStyle = InputDeviceStyle.Xbox360;
			base.IncludePlatforms = new string[1]
			{
				"Windows"
			};
			JoystickNames = new string[30]
			{
				"AIRFLO             ",
				"AxisPad",
				"Controller (Afterglow Gamepad for Xbox 360)",
				"Controller (Batarang wired controller (XBOX))",
				"Controller (Gamepad for Xbox 360)",
				"Controller (GPX Gamepad)",
				"Controller (Infinity Controller 360)",
				"Controller (Mad Catz FPS Pro GamePad)",
				"Controller (MadCatz Call of Duty GamePad)",
				"Controller (MadCatz GamePad)",
				"Controller (MLG GamePad for Xbox 360)",
				"Controller (Razer Sabertooth Elite)",
				"Controller (Rock Candy Gamepad for Xbox 360)",
				"Controller (SL-6566)",
				"Controller (Xbox 360 For Windows)",
				"Controller (Xbox 360 Wireless Receiver for Windows)",
				"Controller (Xbox Airflo wired controller)",
				"Controller (XEOX Gamepad)",
				"Cyborg V.3 Rumble Pad",
				"Generic USB Joystick ",
				"MadCatz GamePad (Controller)",
				"Saitek P990 Dual Analog Pad",
				"SL-6566 (Controller)",
				"USB Gamepad ",
				"WingMan RumblePad",
				"XBOX 360 For Windows (Controller)",
				"XEOX Gamepad (Controller)",
				"XEQX Gamepad SL-6556-BK",
				"Controller (<BETOP GAME FOR WINDOWS>)",
				"Controller (Inno GamePad..)"
			};
			LastResortRegex = "360|xbox|catz";
			base.ButtonMappings = new InputControlMapping[10]
			{
				new InputControlMapping
				{
					Handle = "A",
					Target = InputControlType.Action1,
					Source = UnityInputDeviceProfile.Button0
				},
				new InputControlMapping
				{
					Handle = "B",
					Target = InputControlType.Action2,
					Source = UnityInputDeviceProfile.Button1
				},
				new InputControlMapping
				{
					Handle = "X",
					Target = InputControlType.Action3,
					Source = UnityInputDeviceProfile.Button2
				},
				new InputControlMapping
				{
					Handle = "Y",
					Target = InputControlType.Action4,
					Source = UnityInputDeviceProfile.Button3
				},
				new InputControlMapping
				{
					Handle = "Left Bumper",
					Target = InputControlType.LeftBumper,
					Source = UnityInputDeviceProfile.Button4
				},
				new InputControlMapping
				{
					Handle = "Right Bumper",
					Target = InputControlType.RightBumper,
					Source = UnityInputDeviceProfile.Button5
				},
				new InputControlMapping
				{
					Handle = "Left Stick Button",
					Target = InputControlType.LeftStickButton,
					Source = UnityInputDeviceProfile.Button8
				},
				new InputControlMapping
				{
					Handle = "Right Stick Button",
					Target = InputControlType.RightStickButton,
					Source = UnityInputDeviceProfile.Button9
				},
				new InputControlMapping
				{
					Handle = "Back",
					Target = InputControlType.Back,
					Source = UnityInputDeviceProfile.Button6
				},
				new InputControlMapping
				{
					Handle = "Start",
					Target = InputControlType.Start,
					Source = UnityInputDeviceProfile.Button7
				}
			};
			base.AnalogMappings = new InputControlMapping[16]
			{
				UnityInputDeviceProfile.LeftStickLeftMapping(UnityInputDeviceProfile.Analog0),
				UnityInputDeviceProfile.LeftStickRightMapping(UnityInputDeviceProfile.Analog0),
				UnityInputDeviceProfile.LeftStickUpMapping(UnityInputDeviceProfile.Analog1),
				UnityInputDeviceProfile.LeftStickDownMapping(UnityInputDeviceProfile.Analog1),
				UnityInputDeviceProfile.RightStickLeftMapping(UnityInputDeviceProfile.Analog3),
				UnityInputDeviceProfile.RightStickRightMapping(UnityInputDeviceProfile.Analog3),
				UnityInputDeviceProfile.RightStickUpMapping(UnityInputDeviceProfile.Analog4),
				UnityInputDeviceProfile.RightStickDownMapping(UnityInputDeviceProfile.Analog4),
				UnityInputDeviceProfile.DPadLeftMapping(UnityInputDeviceProfile.Analog5),
				UnityInputDeviceProfile.DPadRightMapping(UnityInputDeviceProfile.Analog5),
				UnityInputDeviceProfile.DPadUpMapping2(UnityInputDeviceProfile.Analog6),
				UnityInputDeviceProfile.DPadDownMapping2(UnityInputDeviceProfile.Analog6),
				new InputControlMapping
				{
					Handle = "Left Trigger",
					Target = InputControlType.LeftTrigger,
					Source = UnityInputDeviceProfile.Analog2,
					SourceRange = InputRange.ZeroToOne,
					TargetRange = InputRange.ZeroToOne
				},
				new InputControlMapping
				{
					Handle = "Right Trigger",
					Target = InputControlType.RightTrigger,
					Source = UnityInputDeviceProfile.Analog2,
					SourceRange = InputRange.ZeroToMinusOne,
					TargetRange = InputRange.ZeroToOne
				},
				new InputControlMapping
				{
					Handle = "Left Trigger",
					Target = InputControlType.LeftTrigger,
					Source = UnityInputDeviceProfile.Analog(8),
					SourceRange = InputRange.ZeroToOne,
					TargetRange = InputRange.ZeroToOne
				},
				new InputControlMapping
				{
					Handle = "Right Trigger",
					Target = InputControlType.RightTrigger,
					Source = UnityInputDeviceProfile.Analog(9),
					SourceRange = InputRange.ZeroToOne,
					TargetRange = InputRange.ZeroToOne
				}
			};
		}
	}
}
