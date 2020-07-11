namespace InControl
{
	public class NativeButtonSource : InputControlSource
	{
		public int ButtonIndex;

		public NativeButtonSource(int buttonIndex)
		{
			ButtonIndex = buttonIndex;
		}

		public float GetValue(InputDevice inputDevice)
		{
			return (!GetState(inputDevice)) ? 0f : 1f;
		}

		public bool GetState(InputDevice inputDevice)
		{
			NativeInputDevice nativeInputDevice = inputDevice as NativeInputDevice;
			return nativeInputDevice.ReadRawButtonState(ButtonIndex);
		}
	}
}
