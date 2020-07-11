namespace InControl.NativeProfile
{
	public class JoytekXbox360ControllerMacProfile : Xbox360DriverMacProfile
	{
		public JoytekXbox360ControllerMacProfile()
		{
			base.Name = "Joytek Xbox 360 Controller";
			base.Meta = "Joytek Xbox 360 Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 5678,
					ProductID = 48879
				}
			};
		}
	}
}
