namespace InControl.NativeProfile
{
	public class HoriRealArcadeProEXMacProfile : Xbox360DriverMacProfile
	{
		public HoriRealArcadeProEXMacProfile()
		{
			base.Name = "Hori Real Arcade Pro EX";
			base.Meta = "Hori Real Arcade Pro EX on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 62724
				}
			};
		}
	}
}
