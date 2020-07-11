namespace InControl.NativeProfile
{
	public class HoriEX2ControllerMacProfile : Xbox360DriverMacProfile
	{
		public HoriEX2ControllerMacProfile()
		{
			base.Name = "Hori EX2 Controller";
			base.Meta = "Hori EX2 Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[3]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 3853,
					ProductID = 13
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 62721
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 9414,
					ProductID = 21760
				}
			};
		}
	}
}
