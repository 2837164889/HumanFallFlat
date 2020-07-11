namespace InControl.NativeProfile
{
	public class KonamiDancePadMacProfile : Xbox360DriverMacProfile
	{
		public KonamiDancePadMacProfile()
		{
			base.Name = "Konami Dance Pad";
			base.Meta = "Konami Dance Pad on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 4779,
					ProductID = 4
				}
			};
		}
	}
}
