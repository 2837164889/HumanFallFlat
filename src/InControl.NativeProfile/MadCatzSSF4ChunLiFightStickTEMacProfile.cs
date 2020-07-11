namespace InControl.NativeProfile
{
	public class MadCatzSSF4ChunLiFightStickTEMacProfile : Xbox360DriverMacProfile
	{
		public MadCatzSSF4ChunLiFightStickTEMacProfile()
		{
			base.Name = "Mad Catz SSF4 Chun-Li Fight Stick TE";
			base.Meta = "Mad Catz SSF4 Chun-Li Fight Stick TE on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 61501
				}
			};
		}
	}
}
