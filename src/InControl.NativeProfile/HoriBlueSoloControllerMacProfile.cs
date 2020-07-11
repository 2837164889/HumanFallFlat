namespace InControl.NativeProfile
{
	public class HoriBlueSoloControllerMacProfile : Xbox360DriverMacProfile
	{
		public HoriBlueSoloControllerMacProfile()
		{
			base.Name = "Hori Blue Solo Controller ";
			base.Meta = "Hori Blue Solo Controller\ton Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 64001
				}
			};
		}
	}
}
