namespace InControl.NativeProfile
{
	public class MicrosoftXboxControllerMacProfile : Xbox360DriverMacProfile
	{
		public MicrosoftXboxControllerMacProfile()
		{
			base.Name = "Microsoft Xbox Controller";
			base.Meta = "Microsoft Xbox Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[7]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = ushort.MaxValue,
					ProductID = ushort.MaxValue
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 1118,
					ProductID = 649
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 1118,
					ProductID = 648
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 1118,
					ProductID = 645
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 1118,
					ProductID = 514
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 1118,
					ProductID = 647
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 1118,
					ProductID = 648
				}
			};
		}
	}
}
