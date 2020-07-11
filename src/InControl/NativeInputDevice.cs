using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace InControl
{
	public class NativeInputDevice : InputDevice
	{
		private const int maxUnknownButtons = 20;

		private const int maxUnknownAnalogs = 20;

		private short[] buttons;

		private short[] analogs;

		private NativeInputDeviceProfile profile;

		private int skipUpdateFrames;

		private int numUnknownButtons;

		private int numUnknownAnalogs;

		internal uint Handle
		{
			get;
			private set;
		}

		internal NativeDeviceInfo Info
		{
			get;
			private set;
		}

		public override bool IsSupportedOnThisPlatform => profile == null || profile.IsSupportedOnThisPlatform;

		public override bool IsKnown => profile != null;

		internal override int NumUnknownButtons => numUnknownButtons;

		internal override int NumUnknownAnalogs => numUnknownAnalogs;

		internal NativeInputDevice()
		{
		}

		internal void Initialize(uint deviceHandle, NativeDeviceInfo deviceInfo, NativeInputDeviceProfile deviceProfile)
		{
			Handle = deviceHandle;
			Info = deviceInfo;
			profile = deviceProfile;
			base.SortOrder = (int)(1000 + Handle);
			NativeDeviceInfo info = Info;
			numUnknownButtons = Math.Min((int)info.numButtons, 20);
			NativeDeviceInfo info2 = Info;
			numUnknownAnalogs = Math.Min((int)info2.numAnalogs, 20);
			NativeDeviceInfo info3 = Info;
			buttons = new short[info3.numButtons];
			NativeDeviceInfo info4 = Info;
			analogs = new short[info4.numAnalogs];
			base.AnalogSnapshot = null;
			ClearInputState();
			ClearControls();
			if (IsKnown)
			{
				string name = profile.Name;
				if (name == null)
				{
					NativeDeviceInfo info5 = Info;
					name = info5.name;
				}
				base.Name = name;
				string text = profile.Meta;
				if (text == null)
				{
					NativeDeviceInfo info6 = Info;
					text = info6.name;
				}
				base.Meta = text;
				base.DeviceClass = profile.DeviceClass;
				base.DeviceStyle = profile.DeviceStyle;
				int analogCount = profile.AnalogCount;
				for (int i = 0; i < analogCount; i++)
				{
					InputControlMapping inputControlMapping = profile.AnalogMappings[i];
					InputControl inputControl = AddControl(inputControlMapping.Target, inputControlMapping.Handle);
					inputControl.Sensitivity = Mathf.Min(profile.Sensitivity, inputControlMapping.Sensitivity);
					inputControl.LowerDeadZone = Mathf.Max(profile.LowerDeadZone, inputControlMapping.LowerDeadZone);
					inputControl.UpperDeadZone = Mathf.Min(profile.UpperDeadZone, inputControlMapping.UpperDeadZone);
					inputControl.Raw = inputControlMapping.Raw;
					inputControl.Passive = inputControlMapping.Passive;
				}
				int buttonCount = profile.ButtonCount;
				for (int j = 0; j < buttonCount; j++)
				{
					InputControlMapping inputControlMapping2 = profile.ButtonMappings[j];
					InputControl inputControl2 = AddControl(inputControlMapping2.Target, inputControlMapping2.Handle);
					inputControl2.Passive = inputControlMapping2.Passive;
				}
			}
			else
			{
				base.Name = "Unknown Device";
				NativeDeviceInfo info7 = Info;
				base.Meta = info7.name;
				for (int k = 0; k < NumUnknownButtons; k++)
				{
					AddControl((InputControlType)(500 + k), "Button " + k);
				}
				for (int l = 0; l < NumUnknownAnalogs; l++)
				{
					AddControl((InputControlType)(400 + l), "Analog " + l, 0.2f, 0.9f);
				}
			}
			skipUpdateFrames = 1;
		}

		internal void Initialize(uint deviceHandle, NativeDeviceInfo deviceInfo)
		{
			Initialize(deviceHandle, deviceInfo, profile);
		}

		public override void Update(ulong updateTick, float deltaTime)
		{
			if (skipUpdateFrames > 0)
			{
				skipUpdateFrames--;
				return;
			}
			if (Native.GetDeviceState(Handle, out IntPtr deviceState))
			{
				Marshal.Copy(deviceState, buttons, 0, buttons.Length);
				deviceState = new IntPtr(deviceState.ToInt64() + buttons.Length * 2);
				Marshal.Copy(deviceState, analogs, 0, analogs.Length);
			}
			if (IsKnown)
			{
				int analogCount = profile.AnalogCount;
				for (int i = 0; i < analogCount; i++)
				{
					InputControlMapping inputControlMapping = profile.AnalogMappings[i];
					float value = inputControlMapping.Source.GetValue(this);
					InputControl control = GetControl(inputControlMapping.Target);
					if (!inputControlMapping.IgnoreInitialZeroValue || !control.IsOnZeroTick || !Utility.IsZero(value))
					{
						float value2 = inputControlMapping.MapValue(value);
						control.UpdateWithValue(value2, updateTick, deltaTime);
					}
				}
				int buttonCount = profile.ButtonCount;
				for (int j = 0; j < buttonCount; j++)
				{
					InputControlMapping inputControlMapping2 = profile.ButtonMappings[j];
					bool state = inputControlMapping2.Source.GetState(this);
					UpdateWithState(inputControlMapping2.Target, state, updateTick, deltaTime);
				}
			}
			else
			{
				for (int k = 0; k < NumUnknownButtons; k++)
				{
					UpdateWithState((InputControlType)(500 + k), ReadRawButtonState(k), updateTick, deltaTime);
				}
				for (int l = 0; l < NumUnknownAnalogs; l++)
				{
					UpdateWithValue((InputControlType)(400 + l), ReadRawAnalogValue(l), updateTick, deltaTime);
				}
			}
		}

		internal override bool ReadRawButtonState(int index)
		{
			if (index < buttons.Length)
			{
				return buttons[index] > -32767;
			}
			return false;
		}

		internal override float ReadRawAnalogValue(int index)
		{
			if (index < analogs.Length)
			{
				return (float)analogs[index] / 32767f;
			}
			return 0f;
		}

		private byte FloatToByte(float value)
		{
			return (byte)(Mathf.Clamp01(value) * 255f);
		}

		public override void Vibrate(float leftMotor, float rightMotor)
		{
			Native.SetHapticState(Handle, FloatToByte(leftMotor), FloatToByte(rightMotor));
		}

		public override void SetLightColor(float red, float green, float blue)
		{
			Native.SetLightColor(Handle, FloatToByte(red), FloatToByte(green), FloatToByte(blue));
		}

		public override void SetLightFlash(float flashOnDuration, float flashOffDuration)
		{
			Native.SetLightFlash(Handle, FloatToByte(flashOnDuration), FloatToByte(flashOffDuration));
		}

		public bool HasSameVendorID(NativeDeviceInfo deviceInfo)
		{
			return Info.HasSameVendorID(deviceInfo);
		}

		public bool HasSameProductID(NativeDeviceInfo deviceInfo)
		{
			return Info.HasSameProductID(deviceInfo);
		}

		public bool HasSameVersionNumber(NativeDeviceInfo deviceInfo)
		{
			return Info.HasSameVersionNumber(deviceInfo);
		}

		public bool HasSameLocation(NativeDeviceInfo deviceInfo)
		{
			return Info.HasSameLocation(deviceInfo);
		}

		public bool HasSameSerialNumber(NativeDeviceInfo deviceInfo)
		{
			return Info.HasSameSerialNumber(deviceInfo);
		}
	}
}
