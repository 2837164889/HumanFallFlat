namespace InControl
{
	public class OuyaEverywhereDeviceManager : InputDeviceManager
	{
		private bool[] deviceConnected = new bool[4];

		public OuyaEverywhereDeviceManager()
		{
			for (int i = 0; i < 4; i++)
			{
				devices.Add(new OuyaEverywhereDevice(i));
			}
		}

		public override void Update(ulong updateTick, float deltaTime)
		{
			for (int i = 0; i < 4; i++)
			{
				OuyaEverywhereDevice ouyaEverywhereDevice = devices[i] as OuyaEverywhereDevice;
				if (ouyaEverywhereDevice.IsConnected != deviceConnected[i])
				{
					if (ouyaEverywhereDevice.IsConnected)
					{
						ouyaEverywhereDevice.BeforeAttach();
						InputManager.AttachDevice(ouyaEverywhereDevice);
					}
					else
					{
						InputManager.DetachDevice(ouyaEverywhereDevice);
					}
					deviceConnected[i] = ouyaEverywhereDevice.IsConnected;
				}
			}
		}

		public static void Enable()
		{
		}
	}
}
