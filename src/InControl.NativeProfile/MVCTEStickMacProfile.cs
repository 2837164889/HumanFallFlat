namespace InControl.NativeProfile
{
	public class MVCTEStickMacProfile : Xbox360DriverMacProfile
	{
		public MVCTEStickMacProfile()
		{
			base.Name = "MVC TE Stick";
			base.Meta = "MVC TE Stick on Mac";
			Matchers = new NativeInputDeviceMatcher[2]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 61497
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 1848,
					ProductID = 46904
				}
			};
		}
	}
}
