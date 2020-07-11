namespace InControl.NativeProfile
{
	public class QanbaFightStickPlusMacProfile : Xbox360DriverMacProfile
	{
		public QanbaFightStickPlusMacProfile()
		{
			base.Name = "Qanba Fight Stick Plus";
			base.Meta = "Qanba Fight Stick Plus on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 1848,
					ProductID = 48879
				}
			};
		}
	}
}
