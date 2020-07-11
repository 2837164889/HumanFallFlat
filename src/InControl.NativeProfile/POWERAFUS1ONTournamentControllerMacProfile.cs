namespace InControl.NativeProfile
{
	public class POWERAFUS1ONTournamentControllerMacProfile : Xbox360DriverMacProfile
	{
		public POWERAFUS1ONTournamentControllerMacProfile()
		{
			base.Name = "POWER A FUS1ON Tournament Controller";
			base.Meta = "POWER A FUS1ON Tournament Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 9414,
					ProductID = 21399
				}
			};
		}
	}
}
