namespace InControl
{
	[AutoDiscover]
	public class XTR_G2_MacNativeProfile : NativeInputDeviceProfile
	{
		public XTR_G2_MacNativeProfile()
		{
			base.Name = "KMODEL Simulator XTR G2 FMS Controller";
			base.Meta = "KMODEL Simulator XTR G2 FMS Controller on OS X";
			base.DeviceClass = InputDeviceClass.Controller;
			base.IncludePlatforms = new string[1]
			{
				"OS X"
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
