namespace InControl.NativeProfile
{
	public class LogitechDriveFXRacingWheelMacProfile : Xbox360DriverMacProfile
	{
		public LogitechDriveFXRacingWheelMacProfile()
		{
			base.Name = "Logitech DriveFX Racing Wheel";
			base.Meta = "Logitech DriveFX Racing Wheel on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 1133,
					ProductID = 51875
				}
			};
		}
	}
}
