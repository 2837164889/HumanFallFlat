namespace InControl.NativeProfile
{
	public class MadCatzSF4FightStickRound2TEMacProfile : Xbox360DriverMacProfile
	{
		public MadCatzSF4FightStickRound2TEMacProfile()
		{
			base.Name = "Mad Catz SF4 Fight Stick Round 2 TE";
			base.Meta = "Mad Catz SF4 Fight Stick Round 2 TE on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 61496
				}
			};
		}
	}
}
