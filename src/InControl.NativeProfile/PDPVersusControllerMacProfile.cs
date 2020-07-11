namespace InControl.NativeProfile
{
	public class PDPVersusControllerMacProfile : Xbox360DriverMacProfile
	{
		public PDPVersusControllerMacProfile()
		{
			base.Name = "PDP Versus Controller";
			base.Meta = "PDP Versus Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 63748
				}
			};
		}
	}
}
