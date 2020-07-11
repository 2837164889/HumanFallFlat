namespace InControl.NativeProfile
{
	public class MadCatzMicroControllerMacProfile : Xbox360DriverMacProfile
	{
		public MadCatzMicroControllerMacProfile()
		{
			base.Name = "Mad Catz Micro Controller";
			base.Meta = "Mad Catz Micro Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 1848,
					ProductID = 18230
				}
			};
		}
	}
}
