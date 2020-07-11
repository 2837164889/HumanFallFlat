namespace InControl.NativeProfile
{
	public class EASportsControllerMacProfile : Xbox360DriverMacProfile
	{
		public EASportsControllerMacProfile()
		{
			base.Name = "EA Sports Controller";
			base.Meta = "EA Sports Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 3695,
					ProductID = 305
				}
			};
		}
	}
}
