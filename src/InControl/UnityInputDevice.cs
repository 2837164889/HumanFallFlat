using UnityEngine;

namespace InControl
{
	public class UnityInputDevice : InputDevice
	{
		private static string[,] analogQueries;

		private static string[,] buttonQueries;

		public const int MaxDevices = 10;

		public const int MaxButtons = 20;

		public const int MaxAnalogs = 20;

		private UnityInputDeviceProfileBase profile;

		internal int JoystickId
		{
			get;
			private set;
		}

		public override bool IsSupportedOnThisPlatform => profile == null || profile.IsSupportedOnThisPlatform;

		public override bool IsKnown => profile != null;

		internal override int NumUnknownButtons => 20;

		internal override int NumUnknownAnalogs => 20;

		public UnityInputDevice(UnityInputDeviceProfileBase deviceProfile)
			: this(deviceProfile, 0, string.Empty)
		{
		}

		public UnityInputDevice(int joystickId, string joystickName)
			: this(null, joystickId, joystickName)
		{
		}

		public UnityInputDevice(UnityInputDeviceProfileBase deviceProfile, int joystickId, string joystickName)
		{
			profile = deviceProfile;
			JoystickId = joystickId;
			if (joystickId != 0)
			{
				base.SortOrder = 100 + joystickId;
			}
			SetupAnalogQueries();
			SetupButtonQueries();
			base.AnalogSnapshot = null;
			if (IsKnown)
			{
				base.Name = profile.Name;
				base.Meta = profile.Meta;
				base.DeviceClass = profile.DeviceClass;
				base.DeviceStyle = profile.DeviceStyle;
				int analogCount = profile.AnalogCount;
				for (int i = 0; i < analogCount; i++)
				{
					InputControlMapping inputControlMapping = profile.AnalogMappings[i];
					if (Utility.TargetIsAlias(inputControlMapping.Target))
					{
						Debug.LogError("Cannot map control \"" + inputControlMapping.Handle + "\" as InputControlType." + inputControlMapping.Target + " in profile \"" + deviceProfile.Name + "\" because this target is reserved as an alias. The mapping will be ignored.");
						continue;
					}
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
					if (Utility.TargetIsAlias(inputControlMapping2.Target))
					{
						Debug.LogError("Cannot map control \"" + inputControlMapping2.Handle + "\" as InputControlType." + inputControlMapping2.Target + " in profile \"" + deviceProfile.Name + "\" because this target is reserved as an alias. The mapping will be ignored.");
						continue;
					}
					InputControl inputControl2 = AddControl(inputControlMapping2.Target, inputControlMapping2.Handle);
					inputControl2.Passive = inputControlMapping2.Passive;
				}
			}
			else
			{
				base.Name = "Unknown Device";
				base.Meta = "\"" + joystickName + "\"";
				for (int k = 0; k < NumUnknownButtons; k++)
				{
					AddControl((InputControlType)(500 + k), "Button " + k);
				}
				for (int l = 0; l < NumUnknownAnalogs; l++)
				{
					AddControl((InputControlType)(400 + l), "Analog " + l, 0.2f, 0.9f);
				}
			}
		}

		public override void Update(ulong updateTick, float deltaTime)
		{
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

		private static void SetupAnalogQueries()
		{
			if (analogQueries != null)
			{
				return;
			}
			analogQueries = new string[10, 20];
			for (int i = 1; i <= 10; i++)
			{
				for (int j = 0; j < 20; j++)
				{
					analogQueries[i - 1, j] = "joystick " + i + " analog " + j;
				}
			}
		}

		private static void SetupButtonQueries()
		{
			if (buttonQueries != null)
			{
				return;
			}
			buttonQueries = new string[10, 20];
			for (int i = 1; i <= 10; i++)
			{
				for (int j = 0; j < 20; j++)
				{
					buttonQueries[i - 1, j] = "joystick " + i + " button " + j;
				}
			}
		}

		private static string GetAnalogKey(int joystickId, int analogId)
		{
			return analogQueries[joystickId - 1, analogId];
		}

		private static string GetButtonKey(int joystickId, int buttonId)
		{
			return buttonQueries[joystickId - 1, buttonId];
		}

		internal override bool ReadRawButtonState(int index)
		{
			if (index < 20)
			{
				string name = buttonQueries[JoystickId - 1, index];
				return Input.GetKey(name);
			}
			return false;
		}

		internal override float ReadRawAnalogValue(int index)
		{
			if (index < 20)
			{
				string axisName = analogQueries[JoystickId - 1, index];
				return Input.GetAxisRaw(axisName);
			}
			return 0f;
		}
	}
}
