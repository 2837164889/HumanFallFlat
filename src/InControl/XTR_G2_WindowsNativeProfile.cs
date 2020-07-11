namespace InControl
{
	[AutoDiscover]
	public class XTR_G2_WindowsNativeProfile : NativeInputDeviceProfile
	{
		public XTR_G2_WindowsNativeProfile()
		{
			base.Name = "KMODEL Simulator XTR G2 FMS Controller";
			base.Meta = "KMODEL Simulator XTR G2 FMS Controller on Windows";
			base.DeviceClass = InputDeviceClass.Controller;
			base.IncludePlatforms = new string[1]
			{
				"Windows"
			};
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 2971,
					ProductID = 16402,
					NameLiterals = new string[1]
					{
						"KMODEL Simulator - XTR+G2+FMS Controller"
					}
				}
			};
		}
	}
}
