namespace InControl
{
	[AutoDiscover]
	public class XTR_G2_WindowsUnityProfile : UnityInputDeviceProfile
	{
		public XTR_G2_WindowsUnityProfile()
		{
			base.Name = "KMODEL Simulator XTR G2 FMS Controller";
			base.Meta = "KMODEL Simulator XTR G2 FMS Controller on Windows";
			base.DeviceClass = InputDeviceClass.Controller;
			base.IncludePlatforms = new string[1]
			{
				"Windows"
			};
			JoystickNames = new string[1]
			{
				"KMODEL Simulator - XTR+G2+FMS Controller"
			};
		}
	}
}
