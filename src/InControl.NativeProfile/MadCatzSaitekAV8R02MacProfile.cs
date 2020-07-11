namespace InControl.NativeProfile
{
	public class MadCatzSaitekAV8R02MacProfile : Xbox360DriverMacProfile
	{
		public MadCatzSaitekAV8R02MacProfile()
		{
			base.Name = "Mad Catz Saitek AV8R02";
			base.Meta = "Mad Catz Saitek AV8R02 on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 1848,
					ProductID = 52009
				}
			};
		}
	}
}
