namespace InControl.NativeProfile
{
	public class ThrustmasterFerrari458RacingWheelMacProfile : Xbox360DriverMacProfile
	{
		public ThrustmasterFerrari458RacingWheelMacProfile()
		{
			base.Name = "Thrustmaster Ferrari 458 Racing Wheel";
			base.Meta = "Thrustmaster Ferrari 458 Racing Wheel on Mac";
			Matchers = new NativeInputDeviceMatcher[2]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 9414,
					ProductID = 23296
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 9414,
					ProductID = 23299
				}
			};
		}
	}
}
