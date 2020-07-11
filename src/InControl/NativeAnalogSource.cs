namespace InControl
{
	public class NativeAnalogSource : InputControlSource
	{
		public int AnalogIndex;

		public NativeAnalogSource(int analogIndex)
		{
			AnalogIndex = analogIndex;
		}

		public float GetValue(InputDevice inputDevice)
		{
			NativeInputDevice nativeInputDevice = inputDevice as NativeInputDevice;
			return nativeInputDevice.ReadRawAnalogValue(AnalogIndex);
		}

		public bool GetState(InputDevice inputDevice)
		{
			return Utility.IsNotZero(GetValue(inputDevice));
		}
	}
}
