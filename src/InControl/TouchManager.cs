using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace InControl
{
	[ExecuteInEditMode]
	public class TouchManager : SingletonMonoBehavior<TouchManager, InControlManager>
	{
		public enum GizmoShowOption
		{
			Never,
			WhenSelected,
			UnlessPlaying,
			Always
		}

		[Space(10f)]
		public Camera touchCamera;

		public GizmoShowOption controlsShowGizmos = GizmoShowOption.Always;

		[HideInInspector]
		public bool enableControlsOnTouch;

		[SerializeField]
		[HideInInspector]
		private bool _controlsEnabled = true;

		[HideInInspector]
		public int controlsLayer = 5;

		private InputDevice device;

		private Vector3 viewSize;

		private Vector2 screenSize;

		private Vector2 halfScreenSize;

		private float percentToWorld;

		private float halfPercentToWorld;

		private float pixelToWorld;

		private float halfPixelToWorld;

		private TouchControl[] touchControls;

		private TouchPool cachedTouches;

		private List<Touch> activeTouches;

		private ReadOnlyCollection<Touch> readOnlyActiveTouches;

		private Vector2 lastMousePosition;

		private bool isReady;

		private Touch mouseTouch;

		public bool controlsEnabled
		{
			get
			{
				return _controlsEnabled;
			}
			set
			{
				if (_controlsEnabled != value)
				{
					int num = touchControls.Length;
					for (int i = 0; i < num; i++)
					{
						touchControls[i].enabled = value;
					}
					_controlsEnabled = value;
				}
			}
		}

		public static ReadOnlyCollection<Touch> Touches => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.readOnlyActiveTouches;

		public static int TouchCount => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.activeTouches.Count;

		public static Camera Camera => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.touchCamera;

		public static InputDevice Device => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.device;

		public static Vector3 ViewSize => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.viewSize;

		public static float PercentToWorld => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.percentToWorld;

		public static float HalfPercentToWorld => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.halfPercentToWorld;

		public static float PixelToWorld => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.pixelToWorld;

		public static float HalfPixelToWorld => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.halfPixelToWorld;

		public static Vector2 ScreenSize => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.screenSize;

		public static Vector2 HalfScreenSize => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.halfScreenSize;

		public static GizmoShowOption ControlsShowGizmos => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.controlsShowGizmos;

		public static bool ControlsEnabled
		{
			get
			{
				return SingletonMonoBehavior<TouchManager, InControlManager>.Instance.controlsEnabled;
			}
			set
			{
				SingletonMonoBehavior<TouchManager, InControlManager>.Instance.controlsEnabled = value;
			}
		}

		public static event Action OnSetup;

		protected TouchManager()
		{
		}

		private void OnEnable()
		{
			InControlManager component = GetComponent<InControlManager>();
			if (component == null)
			{
				Debug.LogError("Touch Manager component can only be added to the InControl Manager object.");
				UnityEngine.Object.DestroyImmediate(this);
				return;
			}
			if (!EnforceSingletonComponent())
			{
				Debug.LogWarning("There is already a Touch Manager component on this game object.");
				return;
			}
			touchControls = GetComponentsInChildren<TouchControl>(includeInactive: true);
			if (Application.isPlaying)
			{
				InputManager.OnSetup += Setup;
				InputManager.OnUpdateDevices += UpdateDevice;
				InputManager.OnCommitDevices += CommitDevice;
			}
		}

		private void OnDisable()
		{
			if (Application.isPlaying)
			{
				InputManager.OnSetup -= Setup;
				InputManager.OnUpdateDevices -= UpdateDevice;
				InputManager.OnCommitDevices -= CommitDevice;
			}
			Reset();
		}

		private void Setup()
		{
			UpdateScreenSize(GetCurrentScreenSize());
			CreateDevice();
			CreateTouches();
			if (TouchManager.OnSetup != null)
			{
				TouchManager.OnSetup();
				TouchManager.OnSetup = null;
			}
		}

		private void Reset()
		{
			device = null;
			mouseTouch = null;
			cachedTouches = null;
			activeTouches = null;
			readOnlyActiveTouches = null;
			touchControls = null;
			TouchManager.OnSetup = null;
		}

		private IEnumerator UpdateScreenSizeAtEndOfFrame()
		{
			yield return new WaitForEndOfFrame();
			UpdateScreenSize(GetCurrentScreenSize());
			yield return null;
		}

		private void Update()
		{
			Vector2 currentScreenSize = GetCurrentScreenSize();
			if (!isReady)
			{
				StartCoroutine(UpdateScreenSizeAtEndOfFrame());
				UpdateScreenSize(currentScreenSize);
				isReady = true;
				return;
			}
			if (screenSize != currentScreenSize)
			{
				UpdateScreenSize(currentScreenSize);
			}
			if (TouchManager.OnSetup != null)
			{
				TouchManager.OnSetup();
				TouchManager.OnSetup = null;
			}
		}

		private void CreateDevice()
		{
			device = new TouchInputDevice();
			device.AddControl(InputControlType.LeftStickLeft, "LeftStickLeft");
			device.AddControl(InputControlType.LeftStickRight, "LeftStickRight");
			device.AddControl(InputControlType.LeftStickUp, "LeftStickUp");
			device.AddControl(InputControlType.LeftStickDown, "LeftStickDown");
			device.AddControl(InputControlType.RightStickLeft, "RightStickLeft");
			device.AddControl(InputControlType.RightStickRight, "RightStickRight");
			device.AddControl(InputControlType.RightStickUp, "RightStickUp");
			device.AddControl(InputControlType.RightStickDown, "RightStickDown");
			device.AddControl(InputControlType.DPadUp, "DPadUp");
			device.AddControl(InputControlType.DPadDown, "DPadDown");
			device.AddControl(InputControlType.DPadLeft, "DPadLeft");
			device.AddControl(InputControlType.DPadRight, "DPadRight");
			device.AddControl(InputControlType.LeftTrigger, "LeftTrigger");
			device.AddControl(InputControlType.RightTrigger, "RightTrigger");
			device.AddControl(InputControlType.LeftBumper, "LeftBumper");
			device.AddControl(InputControlType.RightBumper, "RightBumper");
			for (InputControlType inputControlType = InputControlType.Action1; inputControlType <= InputControlType.Action4; inputControlType++)
			{
				device.AddControl(inputControlType, inputControlType.ToString());
			}
			device.AddControl(InputControlType.Menu, "Menu");
			for (InputControlType inputControlType2 = InputControlType.Button0; inputControlType2 <= InputControlType.Button19; inputControlType2++)
			{
				device.AddControl(inputControlType2, inputControlType2.ToString());
			}
			InputManager.AttachDevice(device);
		}

		private void UpdateDevice(ulong updateTick, float deltaTime)
		{
			UpdateTouches(updateTick, deltaTime);
			SubmitControlStates(updateTick, deltaTime);
		}

		private void CommitDevice(ulong updateTick, float deltaTime)
		{
			CommitControlStates(updateTick, deltaTime);
		}

		private void SubmitControlStates(ulong updateTick, float deltaTime)
		{
			int num = touchControls.Length;
			for (int i = 0; i < num; i++)
			{
				TouchControl touchControl = touchControls[i];
				if (touchControl.enabled && touchControl.gameObject.activeInHierarchy)
				{
					touchControl.SubmitControlState(updateTick, deltaTime);
				}
			}
		}

		private void CommitControlStates(ulong updateTick, float deltaTime)
		{
			int num = touchControls.Length;
			for (int i = 0; i < num; i++)
			{
				TouchControl touchControl = touchControls[i];
				if (touchControl.enabled && touchControl.gameObject.activeInHierarchy)
				{
					touchControl.CommitControlState(updateTick, deltaTime);
				}
			}
		}

		private void UpdateScreenSize(Vector2 currentScreenSize)
		{
			touchCamera.rect = new Rect(0f, 0f, 0.99f, 1f);
			touchCamera.rect = new Rect(0f, 0f, 1f, 1f);
			screenSize = currentScreenSize;
			halfScreenSize = screenSize / 2f;
			viewSize = ConvertViewToWorldPoint(Vector2.one) * 0.02f;
			percentToWorld = Mathf.Min(viewSize.x, viewSize.y);
			halfPercentToWorld = percentToWorld / 2f;
			if (touchCamera != null)
			{
				halfPixelToWorld = touchCamera.orthographicSize / screenSize.y;
				pixelToWorld = halfPixelToWorld * 2f;
			}
			if (touchControls != null)
			{
				int num = touchControls.Length;
				for (int i = 0; i < num; i++)
				{
					touchControls[i].ConfigureControl();
				}
			}
		}

		private void CreateTouches()
		{
			cachedTouches = new TouchPool();
			mouseTouch = new Touch();
			mouseTouch.fingerId = Touch.FingerID_Mouse;
			activeTouches = new List<Touch>(32);
			readOnlyActiveTouches = new ReadOnlyCollection<Touch>(activeTouches);
		}

		private void UpdateTouches(ulong updateTick, float deltaTime)
		{
			activeTouches.Clear();
			cachedTouches.FreeEndedTouches();
			if (mouseTouch.SetWithMouseData(updateTick, deltaTime))
			{
				activeTouches.Add(mouseTouch);
			}
			for (int i = 0; i < Input.touchCount; i++)
			{
				UnityEngine.Touch touch = Input.GetTouch(i);
				Touch touch2 = cachedTouches.FindOrCreateTouch(touch.fingerId);
				touch2.SetWithTouchData(touch, updateTick, deltaTime);
				activeTouches.Add(touch2);
			}
			int count = cachedTouches.Touches.Count;
			for (int j = 0; j < count; j++)
			{
				Touch touch3 = cachedTouches.Touches[j];
				if (touch3.phase != TouchPhase.Ended && touch3.updateTick != updateTick)
				{
					touch3.phase = TouchPhase.Ended;
					activeTouches.Add(touch3);
				}
			}
			InvokeTouchEvents();
		}

		private void SendTouchBegan(Touch touch)
		{
			int num = touchControls.Length;
			for (int i = 0; i < num; i++)
			{
				TouchControl touchControl = touchControls[i];
				if (touchControl.enabled && touchControl.gameObject.activeInHierarchy)
				{
					touchControl.TouchBegan(touch);
				}
			}
		}

		private void SendTouchMoved(Touch touch)
		{
			int num = touchControls.Length;
			for (int i = 0; i < num; i++)
			{
				TouchControl touchControl = touchControls[i];
				if (touchControl.enabled && touchControl.gameObject.activeInHierarchy)
				{
					touchControl.TouchMoved(touch);
				}
			}
		}

		private void SendTouchEnded(Touch touch)
		{
			int num = touchControls.Length;
			for (int i = 0; i < num; i++)
			{
				TouchControl touchControl = touchControls[i];
				if (touchControl.enabled && touchControl.gameObject.activeInHierarchy)
				{
					touchControl.TouchEnded(touch);
				}
			}
		}

		private void InvokeTouchEvents()
		{
			int count = activeTouches.Count;
			if (enableControlsOnTouch && count > 0 && !controlsEnabled)
			{
				Device.RequestActivation();
				controlsEnabled = true;
			}
			for (int i = 0; i < count; i++)
			{
				Touch touch = activeTouches[i];
				switch (touch.phase)
				{
				case TouchPhase.Began:
					SendTouchBegan(touch);
					break;
				case TouchPhase.Moved:
					SendTouchMoved(touch);
					break;
				case TouchPhase.Ended:
					SendTouchEnded(touch);
					break;
				case TouchPhase.Canceled:
					SendTouchEnded(touch);
					break;
				}
			}
		}

		private bool TouchCameraIsValid()
		{
			if (touchCamera == null)
			{
				return false;
			}
			if (Utility.IsZero(touchCamera.orthographicSize))
			{
				return false;
			}
			if (Utility.IsZero(touchCamera.rect.width) && Utility.IsZero(touchCamera.rect.height))
			{
				return false;
			}
			if (Utility.IsZero(touchCamera.pixelRect.width) && Utility.IsZero(touchCamera.pixelRect.height))
			{
				return false;
			}
			return true;
		}

		private Vector3 ConvertScreenToWorldPoint(Vector2 point)
		{
			if (TouchCameraIsValid())
			{
				Camera camera = touchCamera;
				float x = point.x;
				float y = point.y;
				Vector3 position = touchCamera.transform.position;
				return camera.ScreenToWorldPoint(new Vector3(x, y, 0f - position.z));
			}
			return Vector3.zero;
		}

		private Vector3 ConvertViewToWorldPoint(Vector2 point)
		{
			if (TouchCameraIsValid())
			{
				Camera camera = touchCamera;
				float x = point.x;
				float y = point.y;
				Vector3 position = touchCamera.transform.position;
				return camera.ViewportToWorldPoint(new Vector3(x, y, 0f - position.z));
			}
			return Vector3.zero;
		}

		private Vector3 ConvertScreenToViewPoint(Vector2 point)
		{
			if (TouchCameraIsValid())
			{
				Camera camera = touchCamera;
				float x = point.x;
				float y = point.y;
				Vector3 position = touchCamera.transform.position;
				return camera.ScreenToViewportPoint(new Vector3(x, y, 0f - position.z));
			}
			return Vector3.zero;
		}

		private Vector2 GetCurrentScreenSize()
		{
			if (TouchCameraIsValid())
			{
				return new Vector2(touchCamera.pixelWidth, touchCamera.pixelHeight);
			}
			return new Vector2(Screen.width, Screen.height);
		}

		public static Touch GetTouch(int touchIndex)
		{
			return SingletonMonoBehavior<TouchManager, InControlManager>.Instance.activeTouches[touchIndex];
		}

		public static Touch GetTouchByFingerId(int fingerId)
		{
			return SingletonMonoBehavior<TouchManager, InControlManager>.Instance.cachedTouches.FindTouch(fingerId);
		}

		public static Vector3 ScreenToWorldPoint(Vector2 point)
		{
			return SingletonMonoBehavior<TouchManager, InControlManager>.Instance.ConvertScreenToWorldPoint(point);
		}

		public static Vector3 ViewToWorldPoint(Vector2 point)
		{
			return SingletonMonoBehavior<TouchManager, InControlManager>.Instance.ConvertViewToWorldPoint(point);
		}

		public static Vector3 ScreenToViewPoint(Vector2 point)
		{
			return SingletonMonoBehavior<TouchManager, InControlManager>.Instance.ConvertScreenToViewPoint(point);
		}

		public static float ConvertToWorld(float value, TouchUnitType unitType)
		{
			return value * ((unitType != TouchUnitType.Pixels) ? PercentToWorld : PixelToWorld);
		}

		public static Rect PercentToWorldRect(Rect rect)
		{
			float num = rect.xMin - 50f;
			Vector3 vector = ViewSize;
			float x = num * vector.x;
			float num2 = rect.yMin - 50f;
			Vector3 vector2 = ViewSize;
			float y = num2 * vector2.y;
			float width = rect.width;
			Vector3 vector3 = ViewSize;
			float width2 = width * vector3.x;
			float height = rect.height;
			Vector3 vector4 = ViewSize;
			return new Rect(x, y, width2, height * vector4.y);
		}

		public static Rect PixelToWorldRect(Rect rect)
		{
			float xMin = rect.xMin;
			Vector2 vector = HalfScreenSize;
			float x = Mathf.Round(xMin - vector.x) * PixelToWorld;
			float yMin = rect.yMin;
			Vector2 vector2 = HalfScreenSize;
			return new Rect(x, Mathf.Round(yMin - vector2.y) * PixelToWorld, Mathf.Round(rect.width) * PixelToWorld, Mathf.Round(rect.height) * PixelToWorld);
		}

		public static Rect ConvertToWorld(Rect rect, TouchUnitType unitType)
		{
			return (unitType != TouchUnitType.Pixels) ? PercentToWorldRect(rect) : PixelToWorldRect(rect);
		}

		public static implicit operator bool(TouchManager instance)
		{
			return instance != null;
		}
	}
}
