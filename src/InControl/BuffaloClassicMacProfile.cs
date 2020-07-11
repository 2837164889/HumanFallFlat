namespace InControl
{
	[AutoDiscover]
	public class BuffaloClassicMacProfile : UnityInputDeviceProfile
	{
		public BuffaloClassicMacProfile()
		{
			base.Name = "iBuffalo Classic Controller";
			base.Meta = "iBuffalo Classic Controller on Mac";
			base.DeviceClass = InputDeviceClass.Controller;
			base.IncludePlatforms = new string[1]
			{
				"OS X"
			};
			JoystickNames = new string[1]
			{
				" USB,2-axis 8-button gamepad"
			};
			base.ButtonMappings = new InputControlMapping[8]
			{
				new InputControlMapping
				{
					Handle = "A",
					Target = InputControlType.Action2,
					Source = UnityInputDeviceProfile.Button0
				},
				new InputControlMapping
				{
					Handle = "B",
					Target = InputControlType.Action1,
					Source = UnityInputDeviceProfile.Button1
				},
				new InputControlMapping
				{
					Handle = "X",
					Target = InputControlType.Action4,
					Source = UnityInputDeviceProfile.Button2
				},
				new InputControlMapping
				{
					Handle = "Y",
					Target = InputControlType.Action3,
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
					Handle = "Select",
					Target = InputControlType.Select,
					Source = UnityInputDeviceProfile.Button6
				},
				new InputControlMapping
				{
					Handle = "Start",
					Target = InputControlType.Start,
					Source = UnityInputDeviceProfile.Button7
				}
			};
			base.AnalogMappings = new InputControlMapping[4]
			{
				UnityInputDeviceProfile.DPadLeftMapping(UnityInputDeviceProfile.Analog0),
				UnityInputDeviceProfile.DPadRightMapping(UnityInputDeviceProfile.Analog0),
				UnityInputDeviceProfile.DPadUpMapping(UnityInputDeviceProfile.Analog1),
				UnityInputDeviceProfile.DPadDownMapping(UnityInputDeviceProfile.Analog1)
			};
		}
	}
}
