namespace InControl.NativeProfile
{
	public class PowerASpectraIlluminatedControllerMacProfile : Xbox360DriverMacProfile
	{
		public PowerASpectraIlluminatedControllerMacProfile()
		{
			base.Name = "PowerA Spectra Illuminated Controller";
			base.Meta = "PowerA Spectra Illuminated Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 9414,
					ProductID = 21546
				}
			};
		}
	}
}
