namespace InControl.NativeProfile
{
	public class RockCandyXbox360ControllerMacProfile : Xbox360DriverMacProfile
	{
		public RockCandyXbox360ControllerMacProfile()
		{
			base.Name = "Rock Candy Xbox 360 Controller";
			base.Meta = "Rock Candy Xbox 360 Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[2]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 3695,
					ProductID = 543
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 9414,
					ProductID = 64254
				}
			};
		}
	}
}
