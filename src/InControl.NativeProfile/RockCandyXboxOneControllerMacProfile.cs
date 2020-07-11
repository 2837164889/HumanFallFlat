namespace InControl.NativeProfile
{
	public class RockCandyXboxOneControllerMacProfile : XboxOneDriverMacProfile
	{
		public RockCandyXboxOneControllerMacProfile()
		{
			base.Name = "Rock Candy Xbox One Controller";
			base.Meta = "Rock Candy Xbox One Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[3]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 3695,
					ProductID = 326
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 3695,
					ProductID = 582
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 3695,
					ProductID = 838
				}
			};
		}
	}
}
