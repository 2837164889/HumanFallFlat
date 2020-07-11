namespace InControl.NativeProfile
{
	public class Xbox360DriverMacProfile : NativeInputDeviceProfile
	{
		public Xbox360DriverMacProfile()
		{
			base.Name = null;
			base.Meta = null;
			base.DeviceClass = InputDeviceClass.Controller;
			base.DeviceStyle = InputDeviceStyle.Xbox360;
			base.IncludePlatforms = new string[1]
			{
				"OS X"
			};
			base.ButtonMappings = new InputControlMapping[15]
			{
				new InputControlMapping
				{
					Handle = "A",
					Target = InputControlType.Action1,
					Source = NativeInputDeviceProfile.Button(11)
				},
				new InputControlMapping
				{
					Handle = "B",
					Target = InputControlType.Action2,
					Source = NativeInputDeviceProfile.Button(12)
				},
				new InputControlMapping
				{
					Handle = "X",
					Target = InputControlType.Action3,
					Source = NativeInputDeviceProfile.Button(13)
				},
				new InputControlMapping
				{
					Handle = "Y",
					Target = InputControlType.Action4,
					Source = NativeInputDeviceProfile.Button(14)
				},
				new InputControlMapping
				{
					Handle = "DPad Up",
					Target = InputControlType.DPadUp,
					Source = NativeInputDeviceProfile.Button(0)
				},
				new InputControlMapping
				{
					Handle = "DPad Down",
					Target = InputControlType.DPadDown,
					Source = NativeInputDeviceProfile.Button(1)
				},
				new InputControlMapping
				{
					Handle = "DPad Left",
					Target = InputControlType.DPadLeft,
					Source = NativeInputDeviceProfile.Button(2)
				},
				new InputControlMapping
				{
					Handle = "DPad Right",
					Target = InputControlType.DPadRight,
					Source = NativeInputDeviceProfile.Button(3)
				},
				new InputControlMapping
				{
					Handle = "Left Bumper",
					Target = InputControlType.LeftBumper,
					Source = NativeInputDeviceProfile.Button(8)
				},
				new InputControlMapping
				{
					Handle = "Right Bumper",
					Target = InputControlType.RightBumper,
					Source = NativeInputDeviceProfile.Button(9)
				},
				new InputControlMapping
				{
					Handle = "Left Stick Button",
					Target = InputControlType.LeftStickButton,
					Source = NativeInputDeviceProfile.Button(6)
				},
				new InputControlMapping
				{
					Handle = "Right Stick Button",
					Target = InputControlType.RightStickButton,
					Source = NativeInputDeviceProfile.Button(7)
				},
				new InputControlMapping
				{
					Handle = "Back",
					Target = InputControlType.Back,
					Source = NativeInputDeviceProfile.Button(5)
				},
				new InputControlMapping
				{
					Handle = "Start",
					Target = InputControlType.Start,
					Source = NativeInputDeviceProfile.Button(4)
				},
				new InputControlMapping
				{
					Handle = "Guide",
					Target = InputControlType.System,
					Source = NativeInputDeviceProfile.Button(10)
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
