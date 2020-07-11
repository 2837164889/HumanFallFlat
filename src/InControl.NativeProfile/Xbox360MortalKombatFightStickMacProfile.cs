namespace InControl.NativeProfile
{
	public class Xbox360MortalKombatFightStickMacProfile : Xbox360DriverMacProfile
	{
		public Xbox360MortalKombatFightStickMacProfile()
		{
			base.Name = "Xbox 360 Mortal Kombat Fight Stick";
			base.Meta = "Xbox 360 Mortal Kombat Fight Stick on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 63750
				}
			};
		}
	}
}
