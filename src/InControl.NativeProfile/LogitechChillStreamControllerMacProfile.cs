namespace InControl.NativeProfile
{
	public class LogitechChillStreamControllerMacProfile : Xbox360DriverMacProfile
	{
		public LogitechChillStreamControllerMacProfile()
		{
			base.Name = "Logitech Chill Stream Controller";
			base.Meta = "Logitech Chill Stream Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 1133,
					ProductID = 49730
				}
			};
		}
	}
}
