using System;
using System.Collections.Generic;
using UnityEngine;

namespace InControl
{
	public class UnityInputDeviceManager : InputDeviceManager
	{
		private const float deviceRefreshInterval = 1f;

		private float deviceRefreshTimer;

		private List<UnityInputDeviceProfileBase> systemDeviceProfiles = new List<UnityInputDeviceProfileBase>(UnityInputDeviceProfileList.Profiles.Length);

		private List<UnityInputDeviceProfileBase> customDeviceProfiles = new List<UnityInputDeviceProfileBase>();

		private string[] joystickNames;

		private int lastJoystickCount;

		private int lastJoystickHash;

		private int joystickCount;

		private int joystickHash;

		public static uint controllerEnableMask = uint.MaxValue;

		private bool JoystickInfoHasChanged => joystickHash != lastJoystickHash || joystickCount != lastJoystickCount;

		public UnityInputDeviceManager()
		{
			AddSystemDeviceProfiles();
			QueryJoystickInfo();
			AttachDevices();
		}

		public override void Update(ulong updateTick, float deltaTime)
		{
			int count = devices.Count;
			for (int i = 0; i < count; i++)
			{
				UnityInputDevice unityInputDevice = devices[i] as UnityInputDevice;
				if (unityInputDevice != null)
				{
					unityInputDevice.Passive = (((int)controllerEnableMask & (1 << unityInputDevice.JoystickId - 1)) == 0);
				}
			}
			deviceRefreshTimer += deltaTime;
			if (deviceRefreshTimer >= 1f)
			{
				deviceRefreshTimer = 0f;
				QueryJoystickInfo();
				if (JoystickInfoHasChanged)
				{
					Logger.LogInfo("Change in attached Unity joysticks detected; refreshing device list.");
					DetachDevices();
					AttachDevices();
				}
			}
		}

		private void QueryJoystickInfo()
		{
			joystickNames = Input.GetJoystickNames();
			joystickCount = joystickNames.Length;
			joystickHash = 527 + joystickCount;
			for (int i = 0; i < joystickCount; i++)
			{
				joystickHash = joystickHash * 31 + joystickNames[i].GetHashCode();
			}
		}

		private void AttachDevices()
		{
			AttachKeyboardDevices();
			AttachJoystickDevices();
			lastJoystickCount = joystickCount;
			lastJoystickHash = joystickHash;
		}

		private void DetachDevices()
		{
			int count = devices.Count;
			for (int i = 0; i < count; i++)
			{
				InputManager.DetachDevice(devices[i]);
			}
			devices.Clear();
		}

		public void ReloadDevices()
		{
			QueryJoystickInfo();
			DetachDevices();
			AttachDevices();
		}

		private void AttachDevice(UnityInputDevice device)
		{
			device.Passive = (((int)controllerEnableMask & (1 << device.JoystickId - 1)) == 0);
			devices.Add(device);
			InputManager.AttachDevice(device);
		}

		private void AttachKeyboardDevices()
		{
			int count = systemDeviceProfiles.Count;
			for (int i = 0; i < count; i++)
			{
				UnityInputDeviceProfileBase unityInputDeviceProfileBase = systemDeviceProfiles[i];
				if (unityInputDeviceProfileBase.IsNotJoystick && unityInputDeviceProfileBase.IsSupportedOnThisPlatform)
				{
					AttachDevice(new UnityInputDevice(unityInputDeviceProfileBase));
				}
			}
		}

		private void AttachJoystickDevices()
		{
			try
			{
				for (int i = 0; i < joystickCount; i++)
				{
					DetectJoystickDevice(i + 1, joystickNames[i]);
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
				Logger.LogError(ex.StackTrace);
			}
		}

		private bool HasAttachedDeviceWithJoystickId(int unityJoystickId)
		{
			int count = devices.Count;
			for (int i = 0; i < count; i++)
			{
				UnityInputDevice unityInputDevice = devices[i] as UnityInputDevice;
				if (unityInputDevice != null && unityInputDevice.JoystickId == unityJoystickId)
				{
					return true;
				}
			}
			return false;
		}

		private void DetectJoystickDevice(int unityJoystickId, string unityJoystickName)
		{
			if (!HasAttachedDeviceWithJoystickId(unityJoystickId) && unityJoystickName.IndexOf("webcam", StringComparison.OrdinalIgnoreCase) == -1 && (!(InputManager.UnityVersion < new VersionInfo(4, 5, 0, 0)) || (Application.platform != 0 && Application.platform != RuntimePlatform.OSXPlayer) || !(unityJoystickName == "Unknown Wireless Controller")) && (!(InputManager.UnityVersion >= new VersionInfo(4, 6, 3, 0)) || (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.WindowsPlayer) || !string.IsNullOrEmpty(unityJoystickName)))
			{
				UnityInputDeviceProfileBase unityInputDeviceProfileBase = null;
				if (unityInputDeviceProfileBase == null)
				{
					unityInputDeviceProfileBase = customDeviceProfiles.Find((UnityInputDeviceProfileBase config) => config.HasJoystickName(unityJoystickName));
				}
				if (unityInputDeviceProfileBase == null)
				{
					unityInputDeviceProfileBase = systemDeviceProfiles.Find((UnityInputDeviceProfileBase config) => config.HasJoystickName(unityJoystickName));
				}
				if (unityInputDeviceProfileBase == null)
				{
					unityInputDeviceProfileBase = customDeviceProfiles.Find((UnityInputDeviceProfileBase config) => config.HasLastResortRegex(unityJoystickName));
				}
				if (unityInputDeviceProfileBase == null)
				{
					unityInputDeviceProfileBase = systemDeviceProfiles.Find((UnityInputDeviceProfileBase config) => config.HasLastResortRegex(unityJoystickName));
				}
				if (unityInputDeviceProfileBase == null)
				{
					UnityInputDevice device = new UnityInputDevice(unityJoystickId, unityJoystickName);
					AttachDevice(device);
					Debug.Log("[InControl] Joystick " + unityJoystickId + ": \"" + unityJoystickName + "\"");
					Logger.LogWarning("Device " + unityJoystickId + " with name \"" + unityJoystickName + "\" does not match any supported profiles and will be considered an unknown controller.");
				}
				else if (!unityInputDeviceProfileBase.IsHidden)
				{
					UnityInputDevice device2 = new UnityInputDevice(unityInputDeviceProfileBase, unityJoystickId, unityJoystickName);
					AttachDevice(device2);
					Logger.LogInfo("Device " + unityJoystickId + " matched profile " + unityInputDeviceProfileBase.GetType().Name + " (" + unityInputDeviceProfileBase.Name + ")");
				}
				else
				{
					Logger.LogInfo("Device " + unityJoystickId + " matching profile " + unityInputDeviceProfileBase.GetType().Name + " (" + unityInputDeviceProfileBase.Name + ") is hidden and will not be attached.");
				}
			}
		}

		private void AddSystemDeviceProfile(UnityInputDeviceProfile deviceProfile)
		{
			if (deviceProfile.IsSupportedOnThisPlatform)
			{
				systemDeviceProfiles.Add(deviceProfile);
			}
		}

		private void AddSystemDeviceProfiles()
		{
			string[] profiles = UnityInputDeviceProfileList.Profiles;
			foreach (string typeName in profiles)
			{
				UnityInputDeviceProfile deviceProfile = (UnityInputDeviceProfile)Activator.CreateInstance(Type.GetType(typeName));
				AddSystemDeviceProfile(deviceProfile);
			}
		}
	}
}
