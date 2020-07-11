using System;
using System.Collections.Generic;
using UnityEngine;

namespace InControl
{
	public abstract class InputDeviceProfile
	{
		private static HashSet<Type> hideList = new HashSet<Type>();

		private float sensitivity = 1f;

		private float lowerDeadZone;

		private float upperDeadZone = 1f;

		[SerializeField]
		public string Name
		{
			get;
			protected set;
		}

		[SerializeField]
		public string Meta
		{
			get;
			protected set;
		}

		[SerializeField]
		public InputControlMapping[] AnalogMappings
		{
			get;
			protected set;
		}

		[SerializeField]
		public InputControlMapping[] ButtonMappings
		{
			get;
			protected set;
		}

		[SerializeField]
		public string[] IncludePlatforms
		{
			get;
			protected set;
		}

		[SerializeField]
		public string[] ExcludePlatforms
		{
			get;
			protected set;
		}

		[SerializeField]
		public int MaxSystemBuildNumber
		{
			get;
			protected set;
		}

		[SerializeField]
		public int MinSystemBuildNumber
		{
			get;
			protected set;
		}

		[SerializeField]
		public InputDeviceClass DeviceClass
		{
			get;
			protected set;
		}

		[SerializeField]
		public InputDeviceStyle DeviceStyle
		{
			get;
			protected set;
		}

		[SerializeField]
		public float Sensitivity
		{
			get
			{
				return sensitivity;
			}
			protected set
			{
				sensitivity = Mathf.Clamp01(value);
			}
		}

		[SerializeField]
		public float LowerDeadZone
		{
			get
			{
				return lowerDeadZone;
			}
			protected set
			{
				lowerDeadZone = Mathf.Clamp01(value);
			}
		}

		[SerializeField]
		public float UpperDeadZone
		{
			get
			{
				return upperDeadZone;
			}
			protected set
			{
				upperDeadZone = Mathf.Clamp01(value);
			}
		}

		[Obsolete("This property has been renamed to IncludePlatforms.", false)]
		public string[] SupportedPlatforms
		{
			get
			{
				return IncludePlatforms;
			}
			protected set
			{
				IncludePlatforms = value;
			}
		}

		public virtual bool IsSupportedOnThisPlatform
		{
			get
			{
				int systemBuildNumber = Utility.GetSystemBuildNumber();
				if (MaxSystemBuildNumber > 0 && systemBuildNumber > MaxSystemBuildNumber)
				{
					return false;
				}
				if (MinSystemBuildNumber > 0 && systemBuildNumber < MinSystemBuildNumber)
				{
					return false;
				}
				if (ExcludePlatforms != null)
				{
					int num = ExcludePlatforms.Length;
					for (int i = 0; i < num; i++)
					{
						if (InputManager.Platform.Contains(ExcludePlatforms[i].ToUpper()))
						{
							return false;
						}
					}
				}
				if (IncludePlatforms == null || IncludePlatforms.Length == 0)
				{
					return true;
				}
				if (IncludePlatforms != null)
				{
					int num2 = IncludePlatforms.Length;
					for (int j = 0; j < num2; j++)
					{
						if (InputManager.Platform.Contains(IncludePlatforms[j].ToUpper()))
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		internal bool IsHidden => hideList.Contains(GetType());

		public int AnalogCount => AnalogMappings.Length;

		public int ButtonCount => ButtonMappings.Length;

		public InputDeviceProfile()
		{
			Name = string.Empty;
			Meta = string.Empty;
			AnalogMappings = new InputControlMapping[0];
			ButtonMappings = new InputControlMapping[0];
			IncludePlatforms = new string[0];
			ExcludePlatforms = new string[0];
			MinSystemBuildNumber = 0;
			MaxSystemBuildNumber = 0;
			DeviceClass = InputDeviceClass.Unknown;
			DeviceStyle = InputDeviceStyle.Unknown;
		}

		internal static void Hide(Type type)
		{
			hideList.Add(type);
		}
	}
}
