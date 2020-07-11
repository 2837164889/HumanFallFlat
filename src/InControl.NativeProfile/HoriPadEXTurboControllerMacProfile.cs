namespace InControl.NativeProfile
{
	public class HoriPadEXTurboControllerMacProfile : Xbox360DriverMacProfile
	{
		public HoriPadEXTurboControllerMacProfile()
		{
			base.Name = "Hori Pad EX Turbo Controller";
			base.Meta = "Hori Pad EX Turbo Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 3853,
					ProductID = 12
				}
			};
		}
	}
}
