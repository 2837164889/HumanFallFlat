namespace InControl
{
	public class XboxOneInputDevice : InputDevice
	{
		private const uint AnalogLeftStickX = 0u;

		private const uint AnalogLeftStickY = 1u;

		private const uint AnalogRightStickX = 3u;

		private const uint AnalogRightStickY = 4u;

		private const uint AnalogLeftTrigger = 8u;

		private const uint AnalogRightTrigger = 9u;

		private const float LowerDeadZone = 0.2f;

		private const float UpperDeadZone = 0.9f;

		private string[] analogAxisNameForId;

		internal uint JoystickId
		{
			get;
			private set;
		}

		public ulong ControllerId
		{
			get;
			private set;
		}

		public bool IsConnected => false;

		public XboxOneInputDevice(uint joystickId)
			: base("Xbox One Controller")
		{
			JoystickId = joystickId;
			base.SortOrder = (int)joystickId;
			base.Meta = "Xbox One Device #" + joystickId;
			base.DeviceClass = InputDeviceClass.Controller;
			base.DeviceStyle = InputDeviceStyle.XboxOne;
			CacheAnalogAxisNames();
			AddControl(InputControlType.LeftStickLeft, "Left Stick Left", 0.2f, 0.9f);
			AddControl(InputControlType.LeftStickRight, "Left Stick Right", 0.2f, 0.9f);
			AddControl(InputControlType.LeftStickUp, "Left Stick Up", 0.2f, 0.9f);
			AddControl(InputControlType.LeftStickDown, "Left Stick Down", 0.2f, 0.9f);
			AddControl(InputControlType.RightStickLeft, "Right Stick Left", 0.2f, 0.9f);
			AddControl(InputControlType.RightStickRight, "Right Stick Right", 0.2f, 0.9f);
			AddControl(InputControlType.RightStickUp, "Right Stick Up", 0.2f, 0.9f);
			AddControl(InputControlType.RightStickDown, "Right Stick Down", 0.2f, 0.9f);
			AddControl(InputControlType.LeftTrigger, "Left Trigger", 0.2f, 0.9f);
			AddControl(InputControlType.RightTrigger, "Right Trigger", 0.2f, 0.9f);
			AddControl(InputControlType.DPadUp, "DPad Up", 0.2f, 0.9f);
			AddControl(InputControlType.DPadDown, "DPad Down", 0.2f, 0.9f);
			AddControl(InputControlType.DPadLeft, "DPad Left", 0.2f, 0.9f);
			AddControl(InputControlType.DPadRight, "DPad Right", 0.2f, 0.9f);
			AddControl(InputControlType.Action1, "A");
			AddControl(InputControlType.Action2, "B");
			AddControl(InputControlType.Action3, "X");
			AddControl(InputControlType.Action4, "Y");
			AddControl(InputControlType.LeftBumper, "Left Bumper");
			AddControl(InputControlType.RightBumper, "Right Bumper");
			AddControl(InputControlType.LeftStickButton, "Left Stick Button");
			AddControl(InputControlType.RightStickButton, "Right Stick Button");
			AddControl(InputControlType.View, "View");
			AddControl(InputControlType.Menu, "Menu");
		}

		public override void Update(ulong updateTick, float deltaTime)
		{
		}

		public override void Vibrate(float leftMotor, float rightMotor)
		{
		}

		public void Vibrate(float leftMotor, float rightMotor, float leftTrigger, float rightTrigger)
		{
		}

		private string AnalogAxisNameForId(uint analogId)
		{
			return analogAxisNameForId[analogId];
		}

		private void CacheAnalogAxisNameForId(uint analogId)
		{
			analogAxisNameForId[analogId] = "joystick " + JoystickId + " analog " + analogId;
		}

		private void CacheAnalogAxisNames()
		{
			analogAxisNameForId = new string[16];
			CacheAnalogAxisNameForId(0u);
			CacheAnalogAxisNameForId(1u);
			CacheAnalogAxisNameForId(3u);
			CacheAnalogAxisNameForId(4u);
			CacheAnalogAxisNameForId(8u);
			CacheAnalogAxisNameForId(9u);
		}
	}
}
