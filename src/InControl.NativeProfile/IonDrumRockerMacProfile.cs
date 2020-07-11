namespace InControl.NativeProfile
{
	public class IonDrumRockerMacProfile : Xbox360DriverMacProfile
	{
		public IonDrumRockerMacProfile()
		{
			base.Name = "Ion Drum Rocker";
			base.Meta = "Ion Drum Rocker on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 304
				}
			};
		}
	}
}
