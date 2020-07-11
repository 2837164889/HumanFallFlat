namespace InControl.NativeProfile
{
	public class MadCatzSF4FightStickSEMacProfile : Xbox360DriverMacProfile
	{
		public MadCatzSF4FightStickSEMacProfile()
		{
			base.Name = "Mad Catz SF4 Fight Stick SE";
			base.Meta = "Mad Catz SF4 Fight Stick SE on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 1848,
					ProductID = 18200
				}
			};
		}
	}
}
