namespace InControl.NativeProfile
{
	public class GameStopControllerMacProfile : Xbox360DriverMacProfile
	{
		public GameStopControllerMacProfile()
		{
			base.Name = "GameStop Controller";
			base.Meta = "GameStop Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[4]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 3695,
					ProductID = 1025
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 3695,
					ProductID = 769
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 4779,
					ProductID = 770
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 63745
				}
			};
		}
	}
}
