namespace InControl.NativeProfile
{
	public class HORIRealArcadeProVKaiFightingStickMacProfile : Xbox360DriverMacProfile
	{
		public HORIRealArcadeProVKaiFightingStickMacProfile()
		{
			base.Name = "HORI Real Arcade Pro V Kai Fighting Stick";
			base.Meta = "HORI Real Arcade Pro V Kai Fighting Stick on Mac";
			Matchers = new NativeInputDeviceMatcher[2]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 9414,
					ProductID = 21774
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 3853,
					ProductID = 120
				}
			};
		}
	}
}
