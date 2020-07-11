namespace InControl.NativeProfile
{
	public class RazerOnzaTEControllerMacProfile : Xbox360DriverMacProfile
	{
		public RazerOnzaTEControllerMacProfile()
		{
			base.Name = "Razer Onza TE Controller";
			base.Meta = "Razer Onza TE Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[2]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 64768
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 5769,
					ProductID = 64768
				}
			};
		}
	}
}
