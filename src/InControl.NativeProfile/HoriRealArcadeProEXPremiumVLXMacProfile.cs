namespace InControl.NativeProfile
{
	public class HoriRealArcadeProEXPremiumVLXMacProfile : Xbox360DriverMacProfile
	{
		public HoriRealArcadeProEXPremiumVLXMacProfile()
		{
			base.Name = "Hori Real Arcade Pro EX Premium VLX";
			base.Meta = "Hori Real Arcade Pro EX Premium VLX on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 62726
				}
			};
		}
	}
}
