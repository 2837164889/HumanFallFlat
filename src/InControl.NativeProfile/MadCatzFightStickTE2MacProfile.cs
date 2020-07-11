namespace InControl.NativeProfile
{
	public class MadCatzFightStickTE2MacProfile : Xbox360DriverMacProfile
	{
		public MadCatzFightStickTE2MacProfile()
		{
			base.Name = "Mad Catz Fight Stick TE2";
			base.Meta = "Mad Catz Fight Stick TE2 on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 61568
				}
			};
		}
	}
}
