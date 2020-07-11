namespace InControl.NativeProfile
{
	public class PDPXbox360ControllerMacProfile : Xbox360DriverMacProfile
	{
		public PDPXbox360ControllerMacProfile()
		{
			base.Name = "PDP Xbox 360 Controller";
			base.Meta = "PDP Xbox 360 Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 3695,
					ProductID = 1281
				}
			};
		}
	}
}
