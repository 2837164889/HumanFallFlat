namespace InControl.NativeProfile
{
	public class HoriControllerMacProfile : Xbox360DriverMacProfile
	{
		public HoriControllerMacProfile()
		{
			base.Name = "Hori Controller";
			base.Meta = "Hori Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[2]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 21760
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 654
				}
			};
		}
	}
}
