namespace InControl.NativeProfile
{
	public class ThrustmasterGPXControllerMacProfile : Xbox360DriverMacProfile
	{
		public ThrustmasterGPXControllerMacProfile()
		{
			base.Name = "Thrustmaster GPX Controller";
			base.Meta = "Thrustmaster GPX Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[2]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 1103,
					ProductID = 45862
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 9414,
					ProductID = 23298
				}
			};
		}
	}
}
