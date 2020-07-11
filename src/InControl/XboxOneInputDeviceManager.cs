using System.Collections.Generic;
using UnityEngine;

namespace InControl
{
	public class XboxOneInputDeviceManager : InputDeviceManager
	{
		private const int maxDevices = 8;

		private bool[] deviceConnected = new bool[8];

		public XboxOneInputDeviceManager()
		{
			for (uint num = 1u; num <= 8; num++)
			{
				devices.Add(new XboxOneInputDevice(num));
			}
			UpdateInternal(0uL, 0f);
		}

		private void UpdateInternal(ulong updateTick, float deltaTime)
		{
			for (int i = 0; i < 8; i++)
			{
				XboxOneInputDevice xboxOneInputDevice = devices[i] as XboxOneInputDevice;
				if (xboxOneInputDevice.IsConnected != deviceConnected[i])
				{
					if (xboxOneInputDevice.IsConnected)
					{
						InputManager.AttachDevice(xboxOneInputDevice);
					}
					else
					{
						InputManager.DetachDevice(xboxOneInputDevice);
					}
					deviceConnected[i] = xboxOneInputDevice.IsConnected;
				}
			}
		}

		public override void Update(ulong updateTick, float deltaTime)
		{
			UpdateInternal(updateTick, deltaTime);
		}

		public override void Destroy()
		{
		}

		public static bool CheckPlatformSupport(ICollection<string> errors)
		{
			if (Application.platform != RuntimePlatform.XboxOne)
			{
				return false;
			}
			return true;
		}

		internal static bool Enable()
		{
			List<string> list = new List<string>();
			if (CheckPlatformSupport(list))
			{
				InputManager.AddDeviceManager<XboxOneInputDeviceManager>();
				return true;
			}
			foreach (string item in list)
			{
				Logger.LogError(item);
			}
			return false;
		}
	}
}
