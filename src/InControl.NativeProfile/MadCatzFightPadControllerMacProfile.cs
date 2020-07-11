namespace InControl.NativeProfile
{
	public class MadCatzFightPadControllerMacProfile : Xbox360DriverMacProfile
	{
		public MadCatzFightPadControllerMacProfile()
		{
			base.Name = "Mad Catz FightPad Controller";
			base.Meta = "Mad Catz FightPad Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[2]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 61480
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 1848,
					ProductID = 18216
				}
			};
		}
	}
}
