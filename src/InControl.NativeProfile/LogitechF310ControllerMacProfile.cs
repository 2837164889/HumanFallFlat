namespace InControl.NativeProfile
{
	public class LogitechF310ControllerMacProfile : Xbox360DriverMacProfile
	{
		public LogitechF310ControllerMacProfile()
		{
			base.Name = "Logitech F310 Controller";
			base.Meta = "Logitech F310 Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 1133,
					ProductID = 49693
				}
			};
		}
	}
}
