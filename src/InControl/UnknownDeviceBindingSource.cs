using System.IO;
using UnityEngine;

namespace InControl
{
	public class UnknownDeviceBindingSource : BindingSource
	{
		public UnknownDeviceControl Control
		{
			get;
			protected set;
		}

		public override string Name
		{
			get
			{
				if (base.BoundTo == null)
				{
					return string.Empty;
				}
				string str = string.Empty;
				UnknownDeviceControl control = Control;
				if (control.SourceRange == InputRangeType.ZeroToMinusOne)
				{
					str = "Negative ";
				}
				else
				{
					UnknownDeviceControl control2 = Control;
					if (control2.SourceRange == InputRangeType.ZeroToOne)
					{
						str = "Positive ";
					}
				}
				InputDevice device = base.BoundTo.Device;
				if (device == InputDevice.Null)
				{
					return str + Control.Control.ToString();
				}
				UnknownDeviceControl control3 = Control;
				InputControl control4 = device.GetControl(control3.Control);
				if (control4 == InputControl.Null)
				{
					return str + Control.Control.ToString();
				}
				return str + control4.Handle;
			}
		}

		public override string DeviceName
		{
			get
			{
				if (base.BoundTo == null)
				{
					return string.Empty;
				}
				InputDevice device = base.BoundTo.Device;
				if (device == InputDevice.Null)
				{
					return "Unknown Controller";
				}
				return device.Name;
			}
		}

		public override InputDeviceClass DeviceClass => InputDeviceClass.Controller;

		public override InputDeviceStyle DeviceStyle => InputDeviceStyle.Unknown;

		public override BindingSourceType BindingSourceType => BindingSourceType.UnknownDeviceBindingSource;

		internal override bool IsValid
		{
			get
			{
				if (base.BoundTo == null)
				{
					Debug.LogError("Cannot query property 'IsValid' for unbound BindingSource.");
					return false;
				}
				InputDevice device = base.BoundTo.Device;
				int result;
				if (device != InputDevice.Null)
				{
					UnknownDeviceControl control = Control;
					result = (device.HasControl(control.Control) ? 1 : 0);
				}
				else
				{
					result = 1;
				}
				return (byte)result != 0;
			}
		}

		internal UnknownDeviceBindingSource()
		{
			Control = UnknownDeviceControl.None;
		}

		public UnknownDeviceBindingSource(UnknownDeviceControl control)
		{
			Control = control;
		}

		public override float GetValue(InputDevice device)
		{
			return Control.GetValue(device);
		}

		public override bool GetState(InputDevice device)
		{
			if (device == null)
			{
				return false;
			}
			return Utility.IsNotZero(GetValue(device));
		}

		public override bool Equals(BindingSource other)
		{
			if (other == null)
			{
				return false;
			}
			UnknownDeviceBindingSource unknownDeviceBindingSource = other as UnknownDeviceBindingSource;
			if (unknownDeviceBindingSource != null)
			{
				return Control == unknownDeviceBindingSource.Control;
			}
			return false;
		}

		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}
			UnknownDeviceBindingSource unknownDeviceBindingSource = other as UnknownDeviceBindingSource;
			if (unknownDeviceBindingSource != null)
			{
				return Control == unknownDeviceBindingSource.Control;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Control.GetHashCode();
		}

		internal override void Load(BinaryReader reader, ushort dataFormatVersion)
		{
			UnknownDeviceControl control = default(UnknownDeviceControl);
			control.Load(reader);
			Control = control;
		}

		internal override void Save(BinaryWriter writer)
		{
			Control.Save(writer);
		}
	}
}
