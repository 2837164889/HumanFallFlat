namespace InControl.NativeProfile
{
	public class RazerAtroxArcadeStickMacProfile : Xbox360DriverMacProfile
	{
		public RazerAtroxArcadeStickMacProfile()
		{
			base.Name = "Razer Atrox Arcade Stick";
			base.Meta = "Razer Atrox Arcade Stick on Mac";
			Matchers = new NativeInputDeviceMatcher[2]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 5426,
					ProductID = 2560
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 9414,
					ProductID = 20480
				}
			};
		}
	}
}
