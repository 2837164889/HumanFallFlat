namespace InControl
{
	public abstract class UnityInputDeviceProfileBase : InputDeviceProfile
	{
		public abstract bool IsJoystick
		{
			get;
		}

		public bool IsNotJoystick => !IsJoystick;

		public abstract bool HasJoystickName(string joystickName);

		public abstract bool HasLastResortRegex(string joystickName);

		public abstract bool HasJoystickOrRegexName(string joystickName);
	}
}
