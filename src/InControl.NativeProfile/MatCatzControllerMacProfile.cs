namespace InControl.NativeProfile
{
	public class MatCatzControllerMacProfile : Xbox360DriverMacProfile
	{
		public MatCatzControllerMacProfile()
		{
			base.Name = "Mat Catz Controller";
			base.Meta = "Mat Catz Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 61462
				}
			};
		}
	}
}
