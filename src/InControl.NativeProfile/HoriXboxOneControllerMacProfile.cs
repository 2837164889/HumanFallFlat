namespace InControl.NativeProfile
{
	public class HoriXboxOneControllerMacProfile : XboxOneDriverMacProfile
	{
		public HoriXboxOneControllerMacProfile()
		{
			base.Name = "Hori Xbox One Controller";
			base.Meta = "Hori Xbox One Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 3853,
					ProductID = 103
				}
			};
		}
	}
}
