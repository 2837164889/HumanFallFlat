namespace InControl
{
	[AutoDiscover]
	public class AppleTVRemoteProfile : UnityInputDeviceProfile
	{
		public AppleTVRemoteProfile()
		{
			base.Name = "Apple TV Remote";
			base.Meta = "Apple TV Remote on tvOS";
			base.DeviceClass = InputDeviceClass.Remote;
			base.DeviceStyle = InputDeviceStyle.AppleMFi;
			base.IncludePlatforms = new string[1]
			{
				"AppleTV"
			};
			JoystickRegex = new string[1]
			{
				"Remote"
			};
			base.LowerDeadZone = 0.05f;
			base.UpperDeadZone = 0.95f;
			base.ButtonMappings = new InputControlMapping[3]
			{
				new InputControlMapping
				{
					Handle = "TouchPad Click",
					Target = InputControlType.Action1,
					Source = UnityInputDeviceProfile.Button14
				},
				new InputControlMapping
				{
					Handle = "Play/Pause",
					Target = InputControlType.Action2,
					Source = UnityInputDeviceProfile.Button15
				},
				new InputControlMapping
				{
					Handle = "Menu",
					Target = InputControlType.Menu,
					Source = UnityInputDeviceProfile.Button0
				}
			};
			base.AnalogMappings = new InputControlMapping[11]
			{
				UnityInputDeviceProfile.LeftStickLeftMapping(UnityInputDeviceProfile.Analog0),
				UnityInputDeviceProfile.LeftStickRightMapping(UnityInputDeviceProfile.Analog0),
				UnityInputDeviceProfile.LeftStickUpMapping(UnityInputDeviceProfile.Analog1),
				UnityInputDeviceProfile.LeftStickDownMapping(UnityInputDeviceProfile.Analog1),
				new InputControlMapping
				{
					Handle = "TouchPad X",
					Target = InputControlType.TouchPadXAxis,
					Source = UnityInputDeviceProfile.Analog0,
					Raw = true
				},
				new InputControlMapping
				{
					Handle = "TouchPad Y",
					Target = InputControlType.TouchPadYAxis,
					Source = UnityInputDeviceProfile.Analog1,
					Raw = true
				},
				new InputControlMapping
				{
					Handle = "Orientation X",
					Target = InputControlType.TiltX,
					Source = UnityInputDeviceProfile.Analog15,
					Passive = true
				},
				new InputControlMapping
				{
					Handle = "Orientation Y",
					Target = InputControlType.TiltY,
					Source = UnityInputDeviceProfile.Analog16,
					Passive = true
				},
				new InputControlMapping
				{
					Handle = "Orientation Z",
					Target = InputControlType.TiltZ,
					Source = UnityInputDeviceProfile.Analog17,
					Passive = true
				},
				new InputControlMapping
				{
					Handle = "Acceleration X",
					Target = InputControlType.Analog0,
					Source = UnityInputDeviceProfile.Analog18,
					Passive = true
				},
				new InputControlMapping
				{
					Handle = "Acceleration Y",
					Target = InputControlType.Analog1,
					Source = UnityInputDeviceProfile.Analog19,
					Passive = true
				}
			};
		}
	}
}
