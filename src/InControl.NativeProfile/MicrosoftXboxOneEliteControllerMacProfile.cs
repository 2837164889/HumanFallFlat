namespace InControl.NativeProfile
{
	public class MicrosoftXboxOneEliteControllerMacProfile : XboxOneDriverMacProfile
	{
		public MicrosoftXboxOneEliteControllerMacProfile()
		{
			base.Name = "Microsoft Xbox One Elite Controller";
			base.Meta = "Microsoft Xbox One Elite Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 1118,
					ProductID = 739
				}
			};
		}
	}
}
