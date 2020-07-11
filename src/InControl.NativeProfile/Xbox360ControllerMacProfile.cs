namespace InControl.NativeProfile
{
	public class Xbox360ControllerMacProfile : Xbox360DriverMacProfile
	{
		public Xbox360ControllerMacProfile()
		{
			base.Name = "Xbox 360 Controller";
			base.Meta = "Xbox 360 Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 3695,
					ProductID = 62721
				}
			};
		}
	}
}
