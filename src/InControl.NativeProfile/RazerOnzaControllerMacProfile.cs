namespace InControl.NativeProfile
{
	public class RazerOnzaControllerMacProfile : Xbox360DriverMacProfile
	{
		public RazerOnzaControllerMacProfile()
		{
			base.Name = "Razer Onza Controller";
			base.Meta = "Razer Onza Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[2]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 64769
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 5769,
					ProductID = 64769
				}
			};
		}
	}
}
