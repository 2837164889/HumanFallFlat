namespace InControl.NativeProfile
{
	public class LogitechG920RacingWheelMacProfile : Xbox360DriverMacProfile
	{
		public LogitechG920RacingWheelMacProfile()
		{
			base.Name = "Logitech G920 Racing Wheel";
			base.Meta = "Logitech G920 Racing Wheel on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 1133,
					ProductID = 49761
				}
			};
		}
	}
}
