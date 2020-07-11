using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InControl
{
	public class InControlManager : SingletonMonoBehavior<InControlManager, MonoBehaviour>
	{
		public bool logDebugInfo;

		public bool invertYAxis;

		public bool useFixedUpdate;

		public bool dontDestroyOnLoad;

		public bool suspendInBackground;

		public bool enableICade;

		public bool enableXInput;

		public bool xInputOverrideUpdateRate;

		public int xInputUpdateRate;

		public bool xInputOverrideBufferSize;

		public int xInputBufferSize;

		public bool enableNativeInput;

		public bool nativeInputEnableXInput = true;

		public bool nativeInputPreventSleep;

		public bool nativeInputOverrideUpdateRate;

		public int nativeInputUpdateRate;

		public List<string> customProfiles = new List<string>();

		private object mouseLock = new object();

		private Vector3 mouseAccumulator = default(Vector3);

		[NonSerialized]
		public Vector3 mouseValue = default(Vector3);

		[CompilerGenerated]
		private static Action<LogMessage> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Action<LogMessage> _003C_003Ef__mg_0024cache1;

		private void OnEnable()
		{
			if (!EnforceSingleton())
			{
				return;
			}
			InputManager.InvertYAxis = invertYAxis;
			InputManager.SuspendInBackground = suspendInBackground;
			InputManager.EnableICade = enableICade;
			InputManager.EnableXInput = enableXInput;
			InputManager.XInputUpdateRate = (uint)Mathf.Max(xInputUpdateRate, 0);
			InputManager.XInputBufferSize = (uint)Mathf.Max(xInputBufferSize, 0);
			InputManager.EnableNativeInput = enableNativeInput;
			InputManager.NativeInputEnableXInput = nativeInputEnableXInput;
			InputManager.NativeInputUpdateRate = (uint)Mathf.Max(nativeInputUpdateRate, 0);
			InputManager.NativeInputPreventSleep = nativeInputPreventSleep;
			if (InputManager.SetupInternal())
			{
				if (logDebugInfo)
				{
					Debug.Log("InControl (version " + InputManager.Version + ")");
					Logger.OnLogMessage -= LogMessage;
					Logger.OnLogMessage += LogMessage;
				}
				foreach (string customProfile in customProfiles)
				{
					Type type = Type.GetType(customProfile);
					if (type == null)
					{
						Debug.LogError("Cannot find class for custom profile: " + customProfile);
					}
					else
					{
						UnityInputDeviceProfileBase unityInputDeviceProfileBase = Activator.CreateInstance(type) as UnityInputDeviceProfileBase;
						if (unityInputDeviceProfileBase != null)
						{
							InputManager.AttachDevice(new UnityInputDevice(unityInputDeviceProfileBase));
						}
					}
				}
			}
			SceneManager.sceneLoaded -= OnSceneWasLoaded;
			SceneManager.sceneLoaded += OnSceneWasLoaded;
			if (dontDestroyOnLoad)
			{
				UnityEngine.Object.DontDestroyOnLoad(this);
			}
		}

		private void OnDisable()
		{
			SceneManager.sceneLoaded -= OnSceneWasLoaded;
			if (SingletonMonoBehavior<InControlManager, MonoBehaviour>.Instance == this)
			{
				InputManager.ResetInternal();
			}
		}

		private void Update()
		{
			lock (mouseLock)
			{
				mouseAccumulator.x += Input.GetAxisRaw("mouse x");
				mouseAccumulator.y += Input.GetAxisRaw("mouse y");
				mouseAccumulator.z += Input.GetAxisRaw("mouse z");
			}
			if (!useFixedUpdate || Utility.IsZero(Time.timeScale))
			{
				InputManager.UpdateInternal();
			}
		}

		private void FixedUpdate()
		{
			lock (mouseLock)
			{
				mouseValue = mouseAccumulator;
				mouseAccumulator = Vector3.zero;
			}
			if (useFixedUpdate)
			{
				InputManager.UpdateInternal();
			}
		}

		private void OnApplicationFocus(bool focusState)
		{
			InputManager.OnApplicationFocus(focusState);
		}

		private void OnApplicationPause(bool pauseState)
		{
			InputManager.OnApplicationPause(pauseState);
		}

		private void OnApplicationQuit()
		{
			InputManager.OnApplicationQuit();
		}

		private void OnSceneWasLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			InputManager.OnLevelWasLoaded();
		}

		private static void LogMessage(LogMessage logMessage)
		{
			switch (logMessage.type)
			{
			case LogMessageType.Info:
				Debug.Log(logMessage.text);
				break;
			case LogMessageType.Warning:
				Debug.LogWarning(logMessage.text);
				break;
			case LogMessageType.Error:
				Debug.LogError(logMessage.text);
				break;
			}
		}
	}
}
