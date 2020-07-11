using System.Runtime.InteropServices;

namespace InControl
{
	public struct NativeDeviceInfo
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string name;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string location;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string serialNumber;

		public ushort vendorID;

		public ushort productID;

		public uint versionNumber;

		public NativeDeviceDriverType driverType;

		public NativeDeviceTransportType transportType;

		public uint numButtons;

		public uint numAnalogs;

		public bool HasSameVendorID(NativeDeviceInfo deviceInfo)
		{
			return vendorID == deviceInfo.vendorID;
		}

		public bool HasSameProductID(NativeDeviceInfo deviceInfo)
		{
			return productID == deviceInfo.productID;
		}

		public bool HasSameVersionNumber(NativeDeviceInfo deviceInfo)
		{
			return versionNumber == deviceInfo.versionNumber;
		}

		public bool HasSameLocation(NativeDeviceInfo deviceInfo)
		{
			if (string.IsNullOrEmpty(location))
			{
				return false;
			}
			return location == deviceInfo.location;
		}

		public bool HasSameSerialNumber(NativeDeviceInfo deviceInfo)
		{
			if (string.IsNullOrEmpty(serialNumber))
			{
				return false;
			}
			return serialNumber == deviceInfo.serialNumber;
		}
	}
}
