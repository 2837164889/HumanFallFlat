namespace InControl.NativeProfile
{
	public class BatarangControllerMacProfile : Xbox360DriverMacProfile
	{
		public BatarangControllerMacProfile()
		{
			base.Name = "Batarang Controller";
			base.Meta = "Batarang Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 5604,
					ProductID = 16144
				}
			};
		}
	}
}
