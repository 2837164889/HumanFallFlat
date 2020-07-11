namespace InControl
{
	[AutoDiscover]
	public class NexusPlayerRemoteProfile : UnityInputDeviceProfile
	{
		public NexusPlayerRemoteProfile()
		{
			base.Name = "Nexus Player Remote";
			base.Meta = "Nexus Player Remote";
			base.DeviceClass = InputDeviceClass.Remote;
			base.IncludePlatforms = new string[1]
			{
				"Android"
			};
			JoystickNames = new string[1]
			{
				"Google Nexus Remote"
			};
			base.ButtonMappings = new InputControlMapping[2]
			{
				new InputControlMapping
				{
					Handle = "A",
					Target = InputControlType.Action1,
					Source = UnityInputDeviceProfile.Button0
				},
				new InputControlMapping
				{
					Handle = "Back",
					Target = InputControlType.Back,
					Source = UnityInputDeviceProfile.EscapeKey
				}
			};
			base.AnalogMappings = new InputControlMapping[4]
			{
				UnityInputDeviceProfile.DPadLeftMapping(UnityInputDeviceProfile.Analog4),
				UnityInputDeviceProfile.DPadRightMapping(UnityInputDeviceProfile.Analog4),
				UnityInputDeviceProfile.DPadUpMapping(UnityInputDeviceProfile.Analog5),
				UnityInputDeviceProfile.DPadDownMapping(UnityInputDeviceProfile.Analog5)
			};
		}
	}
}
