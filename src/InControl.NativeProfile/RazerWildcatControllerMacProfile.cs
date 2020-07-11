namespace InControl.NativeProfile
{
	public class RazerWildcatControllerMacProfile : Xbox360DriverMacProfile
	{
		public RazerWildcatControllerMacProfile()
		{
			base.Name = "Razer Wildcat Controller";
			base.Meta = "Razer Wildcat Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 5426,
					ProductID = 2563
				}
			};
		}
	}
}
