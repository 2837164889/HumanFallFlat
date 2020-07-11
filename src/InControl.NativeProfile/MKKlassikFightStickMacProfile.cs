namespace InControl.NativeProfile
{
	public class MKKlassikFightStickMacProfile : Xbox360DriverMacProfile
	{
		public MKKlassikFightStickMacProfile()
		{
			base.Name = "MK Klassik Fight Stick";
			base.Meta = "MK Klassik Fight Stick on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 4779,
					ProductID = 771
				}
			};
		}
	}
}
