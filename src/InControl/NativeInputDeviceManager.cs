using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UnityEngine;

namespace InControl
{
	public class NativeInputDeviceManager : InputDeviceManager
	{
		public static Func<NativeDeviceInfo, ReadOnlyCollection<NativeInputDevice>, NativeInputDevice> CustomFindDetachedDevice;

		private List<NativeInputDevice> attachedDevices;

		private List<NativeInputDevice> detachedDevices;

		private List<NativeInputDeviceProfile> systemDeviceProfiles;

		private List<NativeInputDeviceProfile> customDeviceProfiles;

		private uint[] deviceEvents;

		public NativeInputDeviceManager()
		{
			attachedDevices = new List<NativeInputDevice>();
			detachedDevices = new List<NativeInputDevice>();
			systemDeviceProfiles = new List<NativeInputDeviceProfile>(NativeInputDeviceProfileList.Profiles.Length);
			customDeviceProfiles = new List<NativeInputDeviceProfile>();
			deviceEvents = new uint[32];
			AddSystemDeviceProfiles();
			NativeInputOptions options = new NativeInputOptions
			{
				enableXInput = InputManager.NativeInputEnableXInput,
				preventSleep = InputManager.NativeInputPreventSleep
			};
			if (InputManager.NativeInputUpdateRate != 0)
			{
				options.updateRate = (ushort)InputManager.NativeInputUpdateRate;
			}
			else
			{
				options.updateRate = (ushort)Mathf.FloorToInt(1f / Time.fixedDeltaTime);
			}
			Native.Init(options);
		}

		public override void Destroy()
		{
			Native.Stop();
		}

		private uint NextPowerOfTwo(uint x)
		{
			if (x < 0)
			{
				return 0u;
			}
			x--;
			x |= x >> 1;
			x |= x >> 2;
			x |= x >> 4;
			x |= x >> 8;
			x |= x >> 16;
			return x + 1;
		}

		public override void Update(ulong updateTick, float deltaTime)
		{
			IntPtr source;
			int num = Native.GetDeviceEvents(out source);
			if (num <= 0)
			{
				return;
			}
			Utility.ArrayExpand(ref deviceEvents, num);
			MarshalUtility.Copy(source, deviceEvents, num);
			int num2 = 0;
			uint num3 = deviceEvents[num2++];
			for (int i = 0; i < num3; i++)
			{
				uint num4 = deviceEvents[num2++];
				StringBuilder stringBuilder = new StringBuilder(256);
				stringBuilder.Append("Attached native device with handle " + num4 + ":\n");
				if (Native.GetDeviceInfo(num4, out NativeDeviceInfo deviceInfo))
				{
					stringBuilder.AppendFormat("Name: {0}\n", deviceInfo.name);
					stringBuilder.AppendFormat("Driver Type: {0}\n", deviceInfo.driverType);
					stringBuilder.AppendFormat("Location ID: {0}\n", deviceInfo.location);
					stringBuilder.AppendFormat("Serial Number: {0}\n", deviceInfo.serialNumber);
					stringBuilder.AppendFormat("Vendor ID: 0x{0:x}\n", deviceInfo.vendorID);
					stringBuilder.AppendFormat("Product ID: 0x{0:x}\n", deviceInfo.productID);
					stringBuilder.AppendFormat("Version Number: 0x{0:x}\n", deviceInfo.versionNumber);
					stringBuilder.AppendFormat("Buttons: {0}\n", deviceInfo.numButtons);
					stringBuilder.AppendFormat("Analogs: {0}\n", deviceInfo.numAnalogs);
					DetectDevice(num4, deviceInfo);
				}
				Logger.LogInfo(stringBuilder.ToString());
			}
			uint num5 = deviceEvents[num2++];
			for (int j = 0; j < num5; j++)
			{
				uint num6 = deviceEvents[num2++];
				Logger.LogInfo("Detached native device with handle " + num6 + ":");
				NativeInputDevice nativeInputDevice = FindAttachedDevice(num6);
				if (nativeInputDevice != null)
				{
					DetachDevice(nativeInputDevice);
				}
				else
				{
					Logger.LogWarning("Couldn't find device to detach with handle: " + num6);
				}
			}
		}

		private void DetectDevice(uint deviceHandle, NativeDeviceInfo deviceInfo)
		{
			NativeInputDeviceProfile nativeInputDeviceProfile = null;
			nativeInputDeviceProfile = (nativeInputDeviceProfile ?? customDeviceProfiles.Find((NativeInputDeviceProfile profile) => profile.Matches(deviceInfo)));
			nativeInputDeviceProfile = (nativeInputDeviceProfile ?? systemDeviceProfiles.Find((NativeInputDeviceProfile profile) => profile.Matches(deviceInfo)));
			nativeInputDeviceProfile = (nativeInputDeviceProfile ?? customDeviceProfiles.Find((NativeInputDeviceProfile profile) => profile.LastResortMatches(deviceInfo)));
			nativeInputDeviceProfile = (nativeInputDeviceProfile ?? systemDeviceProfiles.Find((NativeInputDeviceProfile profile) => profile.LastResortMatches(deviceInfo)));
			NativeInputDevice nativeInputDevice = FindDetachedDevice(deviceInfo) ?? new NativeInputDevice();
			nativeInputDevice.Initialize(deviceHandle, deviceInfo, nativeInputDeviceProfile);
			AttachDevice(nativeInputDevice);
		}

		private void AttachDevice(NativeInputDevice device)
		{
			detachedDevices.Remove(device);
			attachedDevices.Add(device);
			InputManager.AttachDevice(device);
		}

		private void DetachDevice(NativeInputDevice device)
		{
			attachedDevices.Remove(device);
			detachedDevices.Add(device);
			InputManager.DetachDevice(device);
		}

		private NativeInputDevice FindAttachedDevice(uint deviceHandle)
		{
			int count = attachedDevices.Count;
			for (int i = 0; i < count; i++)
			{
				NativeInputDevice nativeInputDevice = attachedDevices[i];
				if (nativeInputDevice.Handle == deviceHandle)
				{
					return nativeInputDevice;
				}
			}
			return null;
		}

		private NativeInputDevice FindDetachedDevice(NativeDeviceInfo deviceInfo)
		{
			ReadOnlyCollection<NativeInputDevice> arg = new ReadOnlyCollection<NativeInputDevice>(detachedDevices);
			if (CustomFindDetachedDevice != null)
			{
				return CustomFindDetachedDevice(deviceInfo, arg);
			}
			return SystemFindDetachedDevice(deviceInfo, arg);
		}

		private static NativeInputDevice SystemFindDetachedDevice(NativeDeviceInfo deviceInfo, ReadOnlyCollection<NativeInputDevice> detachedDevices)
		{
			int count = detachedDevices.Count;
			for (int i = 0; i < count; i++)
			{
				NativeInputDevice nativeInputDevice = detachedDevices[i];
				if (nativeInputDevice.Info.HasSameVendorID(deviceInfo) && nativeInputDevice.Info.HasSameProductID(deviceInfo) && nativeInputDevice.Info.HasSameSerialNumber(deviceInfo))
				{
					return nativeInputDevice;
				}
			}
			for (int j = 0; j < count; j++)
			{
				NativeInputDevice nativeInputDevice2 = detachedDevices[j];
				if (nativeInputDevice2.Info.HasSameVendorID(deviceInfo) && nativeInputDevice2.Info.HasSameProductID(deviceInfo) && nativeInputDevice2.Info.HasSameLocation(deviceInfo))
				{
					return nativeInputDevice2;
				}
			}
			for (int k = 0; k < count; k++)
			{
				NativeInputDevice nativeInputDevice3 = detachedDevices[k];
				if (nativeInputDevice3.Info.HasSameVendorID(deviceInfo) && nativeInputDevice3.Info.HasSameProductID(deviceInfo) && nativeInputDevice3.Info.HasSameVersionNumber(deviceInfo))
				{
					return nativeInputDevice3;
				}
			}
			for (int l = 0; l < count; l++)
			{
				NativeInputDevice nativeInputDevice4 = detachedDevices[l];
				if (nativeInputDevice4.Info.HasSameLocation(deviceInfo))
				{
					return nativeInputDevice4;
				}
			}
			return null;
		}

		private void AddSystemDeviceProfile(NativeInputDeviceProfile deviceProfile)
		{
			if (deviceProfile.IsSupportedOnThisPlatform)
			{
				systemDeviceProfiles.Add(deviceProfile);
			}
		}

		private void AddSystemDeviceProfiles()
		{
			string[] profiles = NativeInputDeviceProfileList.Profiles;
			foreach (string typeName in profiles)
			{
				NativeInputDeviceProfile deviceProfile = (NativeInputDeviceProfile)Activator.CreateInstance(Type.GetType(typeName));
				AddSystemDeviceProfile(deviceProfile);
			}
		}

		public static bool CheckPlatformSupport(ICollection<string> errors)
		{
			if (Application.platform != RuntimePlatform.OSXPlayer && Application.platform != 0 && Application.platform != RuntimePlatform.WindowsPlayer && Application.platform != RuntimePlatform.WindowsEditor)
			{
				return false;
			}
			try
			{
				Native.GetVersionInfo(out NativeVersionInfo versionInfo);
				Logger.LogInfo("InControl Native (version " + versionInfo.major + "." + versionInfo.minor + "." + versionInfo.patch + ")");
			}
			catch (DllNotFoundException ex)
			{
				errors?.Add(ex.Message + Utility.PluginFileExtension() + " could not be found or is missing a dependency.");
				return false;
			}
			return true;
		}

		internal static bool Enable()
		{
			List<string> list = new List<string>();
			if (CheckPlatformSupport(list))
			{
				InputManager.AddDeviceManager<NativeInputDeviceManager>();
				return true;
			}
			foreach (string item in list)
			{
				Debug.LogError("Error enabling NativeInputDeviceManager: " + item);
			}
			return false;
		}
	}
}
