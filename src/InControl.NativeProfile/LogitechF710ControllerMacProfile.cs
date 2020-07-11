namespace InControl.NativeProfile
{
	public class LogitechF710ControllerMacProfile : Xbox360DriverMacProfile
	{
		public LogitechF710ControllerMacProfile()
		{
			base.Name = "Logitech F710 Controller";
			base.Meta = "Logitech F710 Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 1133,
					ProductID = 49695
				}
			};
		}
	}
}
