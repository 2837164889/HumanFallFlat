namespace InControl.NativeProfile
{
	public class PDPAfterglowControllerMacProfile : Xbox360DriverMacProfile
	{
		public PDPAfterglowControllerMacProfile()
		{
			base.Name = "PDP Afterglow Controller";
			base.Meta = "PDP Afterglow Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[10]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 3695,
					ProductID = 1043
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 9414,
					ProductID = 64252
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 63751
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 9414,
					ProductID = 64253
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 1118,
					ProductID = 742
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 7085,
					ProductID = 63744
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 3695,
					ProductID = 275
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 3695,
					ProductID = 63744
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 3695,
					ProductID = 531
				},
				new NativeInputDeviceMatcher
				{
					VendorID = 4779,
					ProductID = 769
				}
			};
		}
	}
}
