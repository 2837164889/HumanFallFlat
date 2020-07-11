namespace InControl.NativeProfile
{
	public class HoriRealArcadeProHayabusaMacProfile : Xbox360DriverMacProfile
	{
		public HoriRealArcadeProHayabusaMacProfile()
		{
			base.Name = "Hori Real Arcade Pro Hayabusa";
			base.Meta = "Hori Real Arcade Pro Hayabusa on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 3853,
					ProductID = 99
				}
			};
		}
	}
}
