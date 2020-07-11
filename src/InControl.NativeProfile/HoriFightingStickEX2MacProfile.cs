namespace InControl.NativeProfile
{
	public class HoriFightingStickEX2MacProfile : Xbox360DriverMacProfile
	{
		public HoriFightingStickEX2MacProfile()
		{
			base.Name = "Hori Fighting Stick EX2";
			base.Meta = "Hori Fighting Stick EX2 on Mac";
			Matchers = new NativeInputDeviceMatcher[3]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 3853,
					ProductID = 10
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 62725
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 3853,
					ProductID = 13
				}
			};
		}
	}
}
