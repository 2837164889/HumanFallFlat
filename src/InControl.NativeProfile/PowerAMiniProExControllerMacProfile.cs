namespace InControl.NativeProfile
{
	public class PowerAMiniProExControllerMacProfile : Xbox360DriverMacProfile
	{
		public PowerAMiniProExControllerMacProfile()
		{
			base.Name = "PowerA Mini Pro Ex Controller";
			base.Meta = "PowerA Mini Pro Ex Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[3]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 5604,
					ProductID = 16128
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 9414,
					ProductID = 21274
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 9414,
					ProductID = 21248
				}
			};
		}
	}
}
