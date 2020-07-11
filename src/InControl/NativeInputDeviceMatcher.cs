using System;
using System.Text.RegularExpressions;

namespace InControl
{
	public class NativeInputDeviceMatcher
	{
		public ushort? VendorID;

		public ushort? ProductID;

		public uint? VersionNumber;

		public NativeDeviceDriverType? DriverType;

		public NativeDeviceTransportType? TransportType;

		public string[] NameLiterals;

		public string[] NamePatterns;

		internal bool Matches(NativeDeviceInfo deviceInfo)
		{
			bool result = false;
			if (VendorID.HasValue)
			{
				if (VendorID.Value != deviceInfo.vendorID)
				{
					return false;
				}
				result = true;
			}
			if (ProductID.HasValue)
			{
				if (ProductID.Value != deviceInfo.productID)
				{
					return false;
				}
				result = true;
			}
			if (VersionNumber.HasValue)
			{
				if (VersionNumber.Value != deviceInfo.versionNumber)
				{
					return false;
				}
				result = true;
			}
			if (DriverType.HasValue)
			{
				if (DriverType.Value != deviceInfo.driverType)
				{
					return false;
				}
				result = true;
			}
			if (TransportType.HasValue)
			{
				if (TransportType.Value != deviceInfo.transportType)
				{
					return false;
				}
				result = true;
			}
			if (NameLiterals != null && NameLiterals.Length > 0)
			{
				int num = NameLiterals.Length;
				for (int i = 0; i < num; i++)
				{
					if (string.Equals(deviceInfo.name, NameLiterals[i], StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
				return false;
			}
			if (NamePatterns != null && NamePatterns.Length > 0)
			{
				int num2 = NamePatterns.Length;
				for (int j = 0; j < num2; j++)
				{
					if (Regex.IsMatch(deviceInfo.name, NamePatterns[j], RegexOptions.IgnoreCase))
					{
						return true;
					}
				}
				return false;
			}
			return result;
		}
	}
}
