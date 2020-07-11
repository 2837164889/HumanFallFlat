namespace InControl.NativeProfile
{
	public class AfterglowPrismaticXboxOneControllerMacProfile : XboxOneDriverMacProfile
	{
		public AfterglowPrismaticXboxOneControllerMacProfile()
		{
			base.Name = "Afterglow Prismatic Xbox One Controller";
			base.Meta = "Afterglow Prismatic Xbox One Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 3695,
					ProductID = 313
				}
			};
		}
	}
}
