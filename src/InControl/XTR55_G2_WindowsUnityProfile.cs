namespace InControl
{
	[AutoDiscover]
	public class XTR55_G2_WindowsUnityProfile : UnityInputDeviceProfile
	{
		public XTR55_G2_WindowsUnityProfile()
		{
			base.Name = "SAILI Simulator XTR5.5 G2 FMS Controller";
			base.Meta = "SAILI Simulator XTR5.5 G2 FMS Controller on Windows";
			base.DeviceClass = InputDeviceClass.Controller;
			base.IncludePlatforms = new string[1]
			{
				"Windows"
			};
			JoystickNames = new string[1]
			{
				"SAILI Simulator --- XTR5.5+G2+FMS Controller"
			};
		}
	}
}
