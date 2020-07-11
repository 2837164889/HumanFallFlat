namespace InControl.NativeProfile
{
	public class MadCatzCODControllerMacProfile : Xbox360DriverMacProfile
	{
		public MadCatzCODControllerMacProfile()
		{
			base.Name = "Mad Catz COD Controller";
			base.Meta = "Mad Catz COD Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 61477
				}
			};
		}
	}
}
