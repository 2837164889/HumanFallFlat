namespace InControl.NativeProfile
{
	public class HoriDOA4ArcadeStickMacProfile : Xbox360DriverMacProfile
	{
		public HoriDOA4ArcadeStickMacProfile()
		{
			base.Name = "Hori DOA4 Arcade Stick";
			base.Meta = "Hori DOA4 Arcade Stick on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 3853,
					ProductID = 10
				}
			};
		}
	}
}
