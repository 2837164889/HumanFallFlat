namespace InControl
{
	[AutoDiscover]
	public class XTR55_G2_WindowsNativeProfile : NativeInputDeviceProfile
	{
		public XTR55_G2_WindowsNativeProfile()
		{
			base.Name = "SAILI Simulator XTR5.5 G2 FMS Controller";
			base.Meta = "SAILI Simulator XTR5.5 G2 FMS Controller on Windows";
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
						"SAILI Simulator --- XTR5.5+G2+FMS Controller"
					}
				}
			};
		}
	}
}
