namespace InControl.NativeProfile
{
	public class PDPBattlefieldXBoxOneControllerMacProfile : XboxOneDriverMacProfile
	{
		public PDPBattlefieldXBoxOneControllerMacProfile()
		{
			base.Name = "PDP Battlefield XBox One Controller";
			base.Meta = "PDP Battlefield XBox One Controller on Mac";
			Matchers = new NativeInputDeviceMatcher[1]
			{
				new NativeInputDeviceMatcher
				{
					VendorID = 3695,
					ProductID = 356
				}
			};
		}
	}
}
