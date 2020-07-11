namespace InControl.NativeProfile
{
	public class APlayControllerMacProfile : Xbox360DriverMacProfile
	{
		public APlayControllerMacProfile()
		{
			base.Name = "A Play Controller";
			base.Meta = "A Play Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 9414,
					ProductID = 64251
				}
			};
		}
	}
}
