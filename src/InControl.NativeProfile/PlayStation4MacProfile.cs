namespace InControl.NativeProfile
{
	public class PlayStation4MacProfile : NativeInputDeviceProfile
	{
		public PlayStation4MacProfile()
		{
			base.Name = "PlayStation 4 Controller";
			base.Meta = "PlayStation 4 Controller on Mac";
			base.DeviceClass = InputDeviceClass.Controller;
			base.DeviceStyle = InputDeviceStyle.PlayStation4;
			base.IncludePlatforms = new string[1]
			{
				"OS X"
			};
			Matchers = new NativeInputDeviceMatcher[3]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 1356,
					ProductID = 1476
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 1356,
					ProductID = 2508
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 1356,
					ProductID = 2976
				}
			};
			base.ButtonMappings = new InputControlMapping[16]
			{
				new InputControlMapping
				{
					Handle = "Cross",
					Target = InputControlType.Action1,
					Source = NativeInputDeviceProfile.Button(1)
				},
				new InputControlMapping
				{
					Handle = "Circle",
					Target = InputControlType.Action2,
					Source = NativeInputDeviceProfile.Button(2)
				},
				new InputControlMapping
				{
					Handle = "Square",
					Target = InputControlType.Action3,
					Source = NativeInputDeviceProfile.Button(0)
				},
				new InputControlMapping
				{
					Handle = "Triangle",
					Target = InputControlType.Action4,
					Source = NativeInputDeviceProfile.Button(3)
				},
				new InputControlMapping
				{
					Handle = "DPad Up",
					Target = InputControlType.DPadUp,
					Source = NativeInputDeviceProfile.Button(14)
				},
				new InputControlMapping
				{
					Handle = "DPad Down",
					Target = InputControlType.DPadDown,
					Source = NativeInputDeviceProfile.Button(15)
				},
				new InputControlMapping
				{
					Handle = "DPad Left",
					Target = InputControlType.DPadLeft,
					Source = NativeInputDeviceProfile.Button(16)
				},
				new InputControlMapping
				{
					Handle = "DPad Right",
					Target = InputControlType.DPadRight,
					Source = NativeInputDeviceProfile.Button(17)
				},
				new InputControlMapping
				{
					Handle = "Left Bumper",
					Target = InputControlType.LeftBumper,
					Source = NativeInputDeviceProfile.Button(4)
				},
				new InputControlMapping
				{
					Handle = "Right Bumper",
					Target = InputControlType.RightBumper,
					Source = NativeInputDeviceProfile.Button(5)
				},
				new InputControlMapping
				{
					Handle = "Left Stick Button",
					Target = InputControlType.LeftStickButton,
					Source = NativeInputDeviceProfile.Button(10)
				},
				new InputControlMapping
				{
					Handle = "Right Stick Button",
					Target = InputControlType.RightStickButton,
					Source = NativeInputDeviceProfile.Button(11)
				},
				new InputControlMapping
				{
					Handle = "Share",
					Target = InputControlType.Share,
					Source = NativeInputDeviceProfile.Button(8)
				},
				new InputControlMapping
				{
					Handle = "Options",
					Target = InputControlType.Options,
					Source = NativeInputDeviceProfile.Button(9)
				},
				new InputControlMapping
				{
					Handle = "System",
					Target = InputControlType.System,
					Source = NativeInputDeviceProfile.Button(12)
				},
				new InputControlMapping
				{
					Handle = "Touchpad Button",
					Target = InputControlType.TouchPadButton,
					Source = NativeInputDeviceProfile.Button(13)
				}
			};
			base.AnalogMappings = new InputControlMapping[10]
			{
				NativeInputDeviceProfile.LeftStickLeftMapping(0),
				NativeInputDeviceProfile.LeftStickRightMapping(0),
				NativeInputDeviceProfile.LeftStickUpMapping(1),
				NativeInputDeviceProfile.LeftStickDownMapping(1),
				NativeInputDeviceProfile.RightStickLeftMapping(2),
				NativeInputDeviceProfile.RightStickRightMapping(2),
				NativeInputDeviceProfile.RightStickUpMapping(3),
				NativeInputDeviceProfile.RightStickDownMapping(3),
				NativeInputDeviceProfile.LeftTriggerMapping(4),
				NativeInputDeviceProfile.RightTriggerMapping(5)
			};
		}
	}
}
