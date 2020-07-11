namespace InControl
{
	public class OuyaEverywhereDevice : InputDevice
	{
		private const float LowerDeadZone = 0.2f;

		private const float UpperDeadZone = 0.9f;

		public int DeviceIndex
		{
			get;
			private set;
		}

		public bool IsConnected => false;

		public OuyaEverywhereDevice(int deviceIndex)
			: base("OUYA Controller")
		{
			DeviceIndex = deviceIndex;
			base.SortOrder = deviceIndex;
			base.Meta = "OUYA Everywhere Device #" + deviceIndex;
			AddControl(InputControlType.LeftStickLeft, "Left Stick Left");
			AddControl(InputControlType.LeftStickRight, "Left Stick Right");
			AddControl(InputControlType.LeftStickUp, "Left Stick Up");
			AddControl(InputControlType.LeftStickDown, "Left Stick Down");
			AddControl(InputControlType.RightStickLeft, "Right Stick Left");
			AddControl(InputControlType.RightStickRight, "Right Stick Right");
			AddControl(InputControlType.RightStickUp, "Right Stick Up");
			AddControl(InputControlType.RightStickDown, "Right Stick Down");
			AddControl(InputControlType.LeftTrigger, "Left Trigger");
			AddControl(InputControlType.RightTrigger, "Right Trigger");
			AddControl(InputControlType.DPadUp, "DPad Up");
			AddControl(InputControlType.DPadDown, "DPad Down");
			AddControl(InputControlType.DPadLeft, "DPad Left");
			AddControl(InputControlType.DPadRight, "DPad Right");
			AddControl(InputControlType.Action1, "O");
			AddControl(InputControlType.Action2, "A");
			AddControl(InputControlType.Action3, "Y");
			AddControl(InputControlType.Action4, "U");
			AddControl(InputControlType.LeftBumper, "Left Bumper");
			AddControl(InputControlType.RightBumper, "Right Bumper");
			AddControl(InputControlType.LeftStickButton, "Left Stick Button");
			AddControl(InputControlType.RightStickButton, "Right Stick Button");
			AddControl(InputControlType.Menu, "Menu");
		}

		public void BeforeAttach()
		{
		}

		public override void Update(ulong updateTick, float deltaTime)
		{
		}
	}
}
