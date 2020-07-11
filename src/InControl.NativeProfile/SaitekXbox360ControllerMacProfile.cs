namespace InControl.NativeProfile
{
	public class SaitekXbox360ControllerMacProfile : Xbox360DriverMacProfile
	{
		public SaitekXbox360ControllerMacProfile()
		{
			base.Name = "Saitek Xbox 360 Controller";
			base.Meta = "Saitek Xbox 360 Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 1848,
					ProductID = 51970
				}
			};
		}
	}
}
