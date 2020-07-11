namespace InControl.NativeProfile
{
	public class TSZPelicanControllerMacProfile : Xbox360DriverMacProfile
	{
		public TSZPelicanControllerMacProfile()
		{
			base.Name = "TSZ Pelican Controller";
			base.Meta = "TSZ Pelican Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 3695,
					ProductID = 513
				}
			};
		}
	}
}
