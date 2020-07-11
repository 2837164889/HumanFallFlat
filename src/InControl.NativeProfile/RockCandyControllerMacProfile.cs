namespace InControl.NativeProfile
{
	public class RockCandyControllerMacProfile : Xbox360DriverMacProfile
	{
		public RockCandyControllerMacProfile()
		{
			base.Name = "Rock Candy Controller";
			base.Meta = "Rock Candy Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 3695,
					ProductID = 287
				}
			};
		}
	}
}
