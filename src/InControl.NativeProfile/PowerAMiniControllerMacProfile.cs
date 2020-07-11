namespace InControl.NativeProfile
{
	public class PowerAMiniControllerMacProfile : Xbox360DriverMacProfile
	{
		public PowerAMiniControllerMacProfile()
		{
			base.Name = "PowerA Mini Controller";
			base.Meta = "PowerA Mini Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 9414,
					ProductID = 21530
				}
			};
		}
	}
}
