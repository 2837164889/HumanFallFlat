namespace InControl.NativeProfile
{
	public class HoriRealArcadeProEXSEMacProfile : Xbox360DriverMacProfile
	{
		public HoriRealArcadeProEXSEMacProfile()
		{
			base.Name = "Hori Real Arcade Pro EX SE";
			base.Meta = "Hori Real Arcade Pro EX SE on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 3853,
					ProductID = 22
				}
			};
		}
	}
}
