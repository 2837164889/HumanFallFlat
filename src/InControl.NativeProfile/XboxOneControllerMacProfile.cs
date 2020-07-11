namespace InControl.NativeProfile
{
	public class XboxOneControllerMacProfile : XboxOneDriverMacProfile
	{
		public XboxOneControllerMacProfile()
		{
			base.Name = "Xbox One Controller";
			base.Meta = "Xbox One Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[2]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 9414,
					ProductID = 22042
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 9414,
					ProductID = 21786
				}
			};
		}
	}
}
