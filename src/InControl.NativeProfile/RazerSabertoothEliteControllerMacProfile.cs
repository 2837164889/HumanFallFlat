namespace InControl.NativeProfile
{
	public class RazerSabertoothEliteControllerMacProfile : Xbox360DriverMacProfile
	{
		public RazerSabertoothEliteControllerMacProfile()
		{
			base.Name = "Razer Sabertooth Elite Controller";
			base.Meta = "Razer Sabertooth Elite Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[2]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 5769,
					ProductID = 65024
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 9414,
					ProductID = 23812
				}
			};
		}
	}
}
