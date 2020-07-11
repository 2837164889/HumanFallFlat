namespace InControl.NativeProfile
{
	public class NaconGC100XFControllerMacProfile : Xbox360DriverMacProfile
	{
		public NaconGC100XFControllerMacProfile()
		{
			base.Name = "Nacon GC-100XF Controller";
			base.Meta = "Nacon GC-100XF Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 4553,
					ProductID = 22000
				}
			};
		}
	}
}
