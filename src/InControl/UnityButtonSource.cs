namespace InControl
{
	public class UnityButtonSource : InputControlSource
	{
		public int ButtonIndex;

		public UnityButtonSource(int buttonIndex)
		{
			ButtonIndex = buttonIndex;
		}

		public float GetValue(InputDevice inputDevice)
		{
			return (!GetState(inputDevice)) ? 0f : 1f;
		}

		public bool GetState(InputDevice inputDevice)
		{
			UnityInputDevice unityInputDevice = inputDevice as UnityInputDevice;
			return unityInputDevice.ReadRawButtonState(ButtonIndex);
		}
	}
}
