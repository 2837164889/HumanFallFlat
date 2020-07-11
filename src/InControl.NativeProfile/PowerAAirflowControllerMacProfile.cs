namespace InControl.NativeProfile
{
	public class PowerAAirflowControllerMacProfile : Xbox360DriverMacProfile
	{
		public PowerAAirflowControllerMacProfile()
		{
			base.Name = "PowerA Airflow Controller";
			base.Meta = "PowerA Airflow Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 5604,
					ProductID = 16138
				}
			};
		}
	}
}
