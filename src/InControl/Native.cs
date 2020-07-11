using System;
using System.Runtime.InteropServices;

namespace InControl
{
	internal static class Native
	{
		private const string LibraryName = "InControlNative";

		[DllImport("InControlNative", EntryPoint = "InControl_Init")]
		public static extern void Init(NativeInputOptions options);

		[DllImport("InControlNative", EntryPoint = "InControl_Stop")]
		public static extern void Stop();

		[DllImport("InControlNative", EntryPoint = "InControl_GetVersionInfo")]
		public static extern void GetVersionInfo(out NativeVersionInfo versionInfo);

		[DllImport("InControlNative", EntryPoint = "InControl_GetDeviceInfo")]
		public static extern bool GetDeviceInfo(uint handle, out NativeDeviceInfo deviceInfo);

		[DllImport("InControlNative", EntryPoint = "InControl_GetDeviceState")]
		public static extern bool GetDeviceState(uint handle, out IntPtr deviceState);

		[DllImport("InControlNative", EntryPoint = "InControl_GetDeviceEvents")]
		public static extern int GetDeviceEvents(out IntPtr deviceEvents);

		[DllImport("InControlNative", EntryPoint = "InControl_SetHapticState")]
		public static extern void SetHapticState(uint handle, byte motor0, byte motor1);

		[DllImport("InControlNative", EntryPoint = "InControl_SetLightColor")]
		public static extern void SetLightColor(uint handle, byte red, byte green, byte blue);

		[DllImport("InControlNative", EntryPoint = "InControl_SetLightFlash")]
		public static extern void SetLightFlash(uint handle, byte flashOnDuration, byte flashOffDuration);
	}
}
