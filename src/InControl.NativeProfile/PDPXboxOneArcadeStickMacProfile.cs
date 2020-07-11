namespace InControl.NativeProfile
{
	public class PDPXboxOneArcadeStickMacProfile : XboxOneDriverMacProfile
	{
		public PDPXboxOneArcadeStickMacProfile()
		{
			base.Name = "PDP Xbox One Arcade Stick";
			base.Meta = "PDP Xbox One Arcade Stick on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 3695,
					ProductID = 348
				}
			};
		}
	}
}
