namespace InControl.NativeProfile
{
	public class LogitechF510ControllerMacProfile : Xbox360DriverMacProfile
	{
		public LogitechF510ControllerMacProfile()
		{
			base.Name = "Logitech F510 Controller";
			base.Meta = "Logitech F510 Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 1133,
					ProductID = 49694
				}
			};
		}
	}
}
