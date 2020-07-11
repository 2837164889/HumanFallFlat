namespace InControl.NativeProfile
{
	public class MadCatzFightStickTESPlusMacProfile : Xbox360DriverMacProfile
	{
		public MadCatzFightStickTESPlusMacProfile()
		{
			base.Name = "Mad Catz Fight Stick TES Plus";
			base.Meta = "Mad Catz Fight Stick TES Plus on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 61506
				}
			};
		}
	}
}
