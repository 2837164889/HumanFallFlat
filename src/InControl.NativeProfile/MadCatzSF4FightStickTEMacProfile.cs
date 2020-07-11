namespace InControl.NativeProfile
{
	public class MadCatzSF4FightStickTEMacProfile : Xbox360DriverMacProfile
	{
		public MadCatzSF4FightStickTEMacProfile()
		{
			base.Name = "Mad Catz SF4 Fight Stick TE";
			base.Meta = "Mad Catz SF4 Fight Stick TE on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 1848,
					ProductID = 18232
				}
			};
		}
	}
}
