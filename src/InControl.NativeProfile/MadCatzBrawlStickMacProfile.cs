namespace InControl.NativeProfile
{
	public class MadCatzBrawlStickMacProfile : Xbox360DriverMacProfile
	{
		public MadCatzBrawlStickMacProfile()
		{
			base.Name = "Mad Catz Brawl Stick";
			base.Meta = "Mad Catz Brawl Stick on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 61465
				}
			};
		}
	}
}
