namespace InControl.NativeProfile
{
	public class PowerAMiniXboxOneControllerMacProfile : XboxOneDriverMacProfile
	{
		public PowerAMiniXboxOneControllerMacProfile()
		{
			base.Name = "Power A Mini Xbox One Controller";
			base.Meta = "Power A Mini Xbox One Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 9414,
					ProductID = 21562
				}
			};
		}
	}
}
