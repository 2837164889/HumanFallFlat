namespace InControl.NativeProfile
{
	public class HoriPadUltimateMacProfile : Xbox360DriverMacProfile
	{
		public HoriPadUltimateMacProfile()
		{
			base.Name = "HoriPad Ultimate";
			base.Meta = "HoriPad Ultimate on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 3853,
					ProductID = 144
				}
			};
		}
	}
}
