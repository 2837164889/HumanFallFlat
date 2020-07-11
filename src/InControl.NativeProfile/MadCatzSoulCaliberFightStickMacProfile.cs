namespace InControl.NativeProfile
{
	public class MadCatzSoulCaliberFightStickMacProfile : Xbox360DriverMacProfile
	{
		public MadCatzSoulCaliberFightStickMacProfile()
		{
			base.Name = "Mad Catz Soul Caliber Fight Stick";
			base.Meta = "Mad Catz Soul Caliber Fight Stick on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 61503
				}
			};
		}
	}
}
