namespace InControl.NativeProfile
{
	public class RazerStrikeControllerMacProfile : Xbox360DriverMacProfile
	{
		public RazerStrikeControllerMacProfile()
		{
			base.Name = "Razer Strike Controller";
			base.Meta = "Razer Strike Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 5769,
					ProductID = 1
				}
			};
		}
	}
}
