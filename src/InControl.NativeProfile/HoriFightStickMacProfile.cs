namespace InControl.NativeProfile
{
	public class HoriFightStickMacProfile : Xbox360DriverMacProfile
	{
		public HoriFightStickMacProfile()
		{
			base.Name = "Hori Fight Stick";
			base.Meta = "Hori Fight Stick on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 3853,
					ProductID = 13
				}
			};
		}
	}
}
