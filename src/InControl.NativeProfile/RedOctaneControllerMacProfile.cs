namespace InControl.NativeProfile
{
	public class RedOctaneControllerMacProfile : Xbox360DriverMacProfile
	{
		public RedOctaneControllerMacProfile()
		{
			base.Name = "Red Octane Controller";
			base.Meta = "Red Octane Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[2]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 5168,
					ProductID = 63489
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 5168,
					ProductID = 672
				}
			};
		}
	}
}
