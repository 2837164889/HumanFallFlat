namespace InControl.NativeProfile
{
	public class HoriXbox360GemPadExMacProfile : Xbox360DriverMacProfile
	{
		public HoriXbox360GemPadExMacProfile()
		{
			base.Name = "Hori Xbox 360 Gem Pad Ex";
			base.Meta = "Hori Xbox 360 Gem Pad Ex on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 9414,
					ProductID = 21773
				}
			};
		}
	}
}
