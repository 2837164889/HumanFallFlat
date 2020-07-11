namespace InControl.NativeProfile
{
	public class GuitarHeroControllerMacProfile : Xbox360DriverMacProfile
	{
		public GuitarHeroControllerMacProfile()
		{
			base.Name = "Guitar Hero Controller";
			base.Meta = "Guitar Hero Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 5168,
					ProductID = 18248
				}
			};
		}
	}
}
