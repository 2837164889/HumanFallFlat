namespace InControl
{
	[AutoDiscover]
	public class AmazonFireTVRemoteProfile : UnityInputDeviceProfile
	{
		public AmazonFireTVRemoteProfile()
		{
			base.Name = "Amazon Fire TV Remote";
			base.Meta = "Amazon Fire TV Remote on Amazon Fire TV";
			base.DeviceClass = InputDeviceClass.Remote;
			base.DeviceStyle = InputDeviceStyle.AmazonFireTV;
			base.IncludePlatforms = new string[1]
			{
				"Amazon AFT"
			};
			JoystickNames = new string[2]
			{
				string.Empty,
				"Amazon Fire TV Remote"
			};
			base.ButtonMappings = new InputControlMapping[3]
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
				},
				new InputControlMapping
				{
					Handle = "Menu",
					Target = InputControlType.Menu,
					Source = UnityInputDeviceProfile.MenuKey
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
