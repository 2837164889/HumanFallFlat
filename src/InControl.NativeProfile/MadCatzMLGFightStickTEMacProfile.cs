namespace InControl.NativeProfile
{
	public class MadCatzMLGFightStickTEMacProfile : Xbox360DriverMacProfile
	{
		public MadCatzMLGFightStickTEMacProfile()
		{
			base.Name = "Mad Catz MLG Fight Stick TE";
			base.Meta = "Mad Catz MLG Fight Stick TE on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 61502
				}
			};
		}
	}
}
