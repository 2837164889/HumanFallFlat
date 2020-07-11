namespace InControl.NativeProfile
{
	public class MadCatzNeoFightStickMacProfile : Xbox360DriverMacProfile
	{
		public MadCatzNeoFightStickMacProfile()
		{
			base.Name = "Mad Catz Neo Fight Stick";
			base.Meta = "Mad Catz Neo Fight Stick on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 61498
				}
			};
		}
	}
}
