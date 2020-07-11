namespace InControl.NativeProfile
{
	public class PDPTitanfall2XboxOneControllerMacProfile : XboxOneDriverMacProfile
	{
		public PDPTitanfall2XboxOneControllerMacProfile()
		{
			base.Name = "PDP Titanfall 2 Xbox One Controller";
			base.Meta = "PDP Titanfall 2 Xbox One Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 3695,
					ProductID = 357
				}
			};
		}
	}
}
