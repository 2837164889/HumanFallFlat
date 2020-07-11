namespace InControl.NativeProfile
{
	public class TrustPredatorJoystickMacProfile : Xbox360DriverMacProfile
	{
		public TrustPredatorJoystickMacProfile()
		{
			base.Name = "Trust Predator Joystick";
			base.Meta = "Trust Predator Joystick on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 2064,
					ProductID = 3
				}
			};
		}
	}
}
