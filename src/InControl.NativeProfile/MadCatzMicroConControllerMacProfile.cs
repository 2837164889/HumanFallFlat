namespace InControl.NativeProfile
{
	public class MadCatzMicroConControllerMacProfile : Xbox360DriverMacProfile
	{
		public MadCatzMicroConControllerMacProfile()
		{
			base.Name = "Mad Catz MicroCon Controller";
			base.Meta = "Mad Catz MicroCon Controller on Mac";
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
