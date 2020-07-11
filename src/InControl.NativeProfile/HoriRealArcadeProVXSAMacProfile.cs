namespace InControl.NativeProfile
{
	public class HoriRealArcadeProVXSAMacProfile : Xbox360DriverMacProfile
	{
		public HoriRealArcadeProVXSAMacProfile()
		{
			base.Name = "Hori Real Arcade Pro VX SA";
			base.Meta = "Hori Real Arcade Pro VX SA on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 62722
				}
			};
		}
	}
}
