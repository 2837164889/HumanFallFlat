using InControl;
using Multiplayer;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuSystem : MonoBehaviour
{
	public enum eInputDeviceType
	{
		Mouse,
		Keyboard,
		Controller
	}

	public GameObject[] pagePrefabs;

	public static MenuSystem instance;

	private InControlInputModule inputModule;

	private EventSystem eventSystem;

	[NonSerialized]
	public MenuTransition activeMenu;

	public MenuSystemState state;

	public GameObject mouseBlocker;

	private bool coopShown;

	public const float defaultFadeOutTime = 0.3f;

	public const float defaultFadeInTime = 0.3f;

	private bool useMenuInput;

	private bool focusOnMouseOver = true;

	private bool mouseMode;

	private bool controllerLastInput;

	public static KeyboardState keyboardState;

	private static float timeSinceLast;

	private eInputDeviceType mDevice = eInputDeviceType.Controller;

	public Action<eInputDeviceType> InputDeviceChange;

	public static bool CanInvoke => CustomCanInvoke();

	public static bool CanInvokeFromGame => CustomCanInvoke(checkMenuState: false);

	private void Awake()
	{
		for (int i = 0; i < pagePrefabs.Length; i++)
		{
			if (pagePrefabs[i] != null)
			{
				UnityEngine.Object.Instantiate(pagePrefabs[i], base.transform, worldPositionStays: false);
			}
		}
	}

	private void OnEnable()
	{
		state = MenuSystemState.Inactive;
		instance = this;
		inputModule = UnityEngine.Object.FindObjectOfType<InControlInputModule>();
		eventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();
		MenuTransition[] componentsInChildren = GetComponentsInChildren<MenuTransition>();
		MenuTransition[] array = componentsInChildren;
		foreach (MenuTransition menuTransition in array)
		{
			menuTransition.gameObject.SetActive(value: false);
		}
	}

	public void TransitionForward<T>(MenuTransition from, float fadeOutTime = 0.3f, float fadeInTime = 0.3f) where T : MenuTransition
	{
		T menu = GetMenu<T>();
		if (!((UnityEngine.Object)menu == (UnityEngine.Object)null))
		{
			FadeOutForward(from, fadeOutTime);
			FadeInForward(menu, fadeInTime);
		}
	}

	public void TransitionBack<T>(MenuTransition from, float fadeOutTime = 0.3f, float fadeInTime = 0.3f) where T : MenuTransition
	{
		T menu = GetMenu<T>();
		if (!((UnityEngine.Object)menu == (UnityEngine.Object)null))
		{
			FadeOutBack(from, fadeOutTime);
			FadeInBack(menu, fadeInTime);
		}
	}

	public T GetMenu<T>() where T : MenuTransition
	{
		T[] componentsInChildren = GetComponentsInChildren<T>(includeInactive: true);
		if (componentsInChildren.Length == 0)
		{
			throw new InvalidOperationException("Did not find menu" + typeof(T));
		}
		return componentsInChildren[0];
	}

	public void FadeInBack(MenuTransition to, float fadeInTime = 0.3f)
	{
		to.gameObject.SetActive(value: true);
		to.Transition(-1f, 0f);
		to.Transition(0f, fadeInTime);
		to.ApplyMenuEffects();
		activeMenu = to;
		to.OnGotFocus();
	}

	public void FadeInForward(MenuTransition to, float fadeInTime = 0.3f)
	{
		to.gameObject.SetActive(value: true);
		to.Transition(1f, 0f);
		to.Transition(0f, fadeInTime);
		to.ApplyMenuEffects();
		activeMenu = to;
		to.OnGotFocus();
	}

	public void MenuTransitionedIn(MenuTransition menu)
	{
		FocusOnMouseOver(enable: true);
		menu.OnTansitionedIn();
	}

	public void FadeOutForward(MenuTransition from, float fadeOutTime = 0.3f)
	{
		FocusOnMouseOver(enable: false);
		from.Transition(-1f, fadeOutTime);
		from.OnLostFocus();
	}

	public void FadeOutActive()
	{
		state = MenuSystemState.Inactive;
		if (activeMenu != null)
		{
			activeMenu.FadeOutForward();
		}
	}

	public void FadeOutBack(MenuTransition from, float fadeOutTime = 0.3f)
	{
		FocusOnMouseOver(enable: false);
		from.Transition(1f, fadeOutTime);
		from.OnLostFocus();
	}

	public void EnterMenuInputMode()
	{
		useMenuInput = true;
		SingletonMonoBehavior<InControlManager, MonoBehaviour>.Instance.useFixedUpdate = false;
		InputManager.ClearInputState();
		InputManager.UpdateInternal();
		InputManager.OnLevelWasLoaded();
	}

	public void ExitMenuInputMode()
	{
		useMenuInput = false;
		SingletonMonoBehavior<InControlManager, MonoBehaviour>.Instance.useFixedUpdate = true;
		InputManager.ClearInputState();
		InputManager.UpdateInternal();
		InputManager.OnLevelWasLoaded();
	}

	public void FocusOnMouseOver(bool enable)
	{
		focusOnMouseOver = enable;
		inputModule.focusOnMouseHover = enable;
	}

	public void ShowPauseMenu()
	{
		state = MenuSystemState.PauseMenu;
		MenuTransition to = (Game.instance.currentLevelNumber != Game.instance.levelCount) ? ((MenuTransition)GetMenu<PauseMenu>()) : ((MenuTransition)GetMenu<CreditsMenu>());
		if (!NetGame.isLocal)
		{
			to = GetMenu<MultiplayerPauseMenu>();
		}
		FadeInForward(to);
		EnterMenuInputMode();
		SubtitleManager.instance.Hide();
	}

	public void OnShowingMainMenu()
	{
		state = MenuSystemState.MainMenu;
		for (int i = 0; i < Human.all.Count; i++)
		{
			Human.all[i].Reset();
		}
	}

	internal void ShowMainMenu(bool hideLogo = false, bool hideOldMenu = false)
	{
		App.state = AppSate.Menu;
		if (activeMenu == null)
		{
			MainMenu menu = GetMenu<MainMenu>();
			FadeInForward(menu);
		}
		else if (hideOldMenu)
		{
			TransitionForward<MainMenu>(activeMenu, 0f);
		}
		else
		{
			TransitionForward<MainMenu>(activeMenu);
		}
		EnterMenuInputMode();
		SubtitleManager.instance.Hide();
		OnShowingMainMenu();
	}

	public void ShowMainMenu<T>(bool hideOldMenu = false) where T : MenuTransition
	{
		if (activeMenu == null)
		{
			T menu = GetMenu<T>();
			FadeInForward(menu);
		}
		else if (hideOldMenu)
		{
			activeMenu.TransitionForward<T>(0f);
		}
		else
		{
			activeMenu.TransitionForward<T>();
		}
		EnterMenuInputMode();
		SubtitleManager.instance.Hide();
		OnShowingMainMenu();
	}

	public void ShowMenu<T>(bool hideOldMenu = false) where T : MenuTransition
	{
		state = MenuSystemState.MainMenu;
		if (activeMenu == null)
		{
			T menu = GetMenu<T>();
			FadeInForward(menu);
		}
		else if (hideOldMenu)
		{
			activeMenu.TransitionForward<T>(0f);
		}
		else
		{
			activeMenu.TransitionForward<T>();
		}
		EnterMenuInputMode();
		SubtitleManager.instance.Hide();
	}

	public void HideMenus()
	{
		MenuCameraEffects.FadeOut();
		MenuTransition menuTransition = activeMenu;
		instance.ExitMenus();
		if (menuTransition != null)
		{
			menuTransition.FadeOutBack();
		}
	}

	public void ExitMenus()
	{
		if (activeMenu != null)
		{
			activeMenu.Transition(1f, 1f);
			activeMenu.Transition(1f, 0f);
		}
		state = MenuSystemState.Inactive;
		activeMenu = null;
		ExitMenuInputMode();
		SubtitleManager.instance.Show();
	}

	private void OnGUI()
	{
		UpdateInputDeviceType();
		BindCursor();
	}

	private void BindCursor(bool force = true)
	{
		bool flag = Options.controllerBindings.HasInput();
		if (flag)
		{
			mouseMode = false;
		}
		if (InputModuleActionAdapter.actions != null && InputModuleActionAdapter.actions.Move.Value != Vector2.zero)
		{
			mouseMode = false;
		}
		if (Options.keyboardBindings != null)
		{
			if (Options.keyboardBindings.Move.Value != Vector2.zero)
			{
				mouseMode = false;
			}
			if (Options.keyboardBindings.Look.Value != Vector2.zero)
			{
				mouseMode = true;
			}
		}
		if (Input.GetAxisRaw("mouse x") != 0f || Input.GetAxisRaw("mouse y") != 0f)
		{
			controllerLastInput = false;
		}
		if (flag)
		{
			controllerLastInput = true;
		}
		mouseMode = (useMenuInput && !controllerLastInput);
		if (useMenuInput && mouseMode)
		{
			if (force || mouseBlocker.activeSelf)
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
				Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
				mouseBlocker.SetActive(value: false);
			}
			if (focusOnMouseOver != inputModule.focusOnMouseHover)
			{
				inputModule.focusOnMouseHover = focusOnMouseOver;
			}
		}
		else
		{
			if (force || !mouseBlocker.activeSelf)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				Cursor.SetCursor(Texture2D.blackTexture, Vector2.zero, CursorMode.ForceSoftware);
				mouseBlocker.SetActive(value: true);
			}
			inputModule.focusOnMouseHover = false;
		}
	}

	public void OnApplicationFocus(bool focus)
	{
		if (focus)
		{
			BindCursor();
			return;
		}
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		mouseBlocker.SetActive(value: false);
	}

	private void Update()
	{
		if (state == MenuSystemState.MainMenu && NetGame.instance.players.Count == 1 && !NetGame.isNetStarted)
		{
			Human human = Human.all[0];
			Vector3 position = human.transform.position;
			if (position.y < 50f)
			{
				human.SetPosition(new Vector3(0f, 100f, 0f));
			}
		}
		timeSinceLast += Time.unscaledDeltaTime;
		if (!(activeMenu == null) && keyboardState == KeyboardState.None)
		{
			if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.activeInHierarchy && EventSystem.current.currentSelectedGameObject.GetComponentInParent<MenuTransition>() == activeMenu)
			{
				activeMenu.lastFocusedElement = EventSystem.current.currentSelectedGameObject;
			}
			else if (activeMenu.lastFocusedElement != null && activeMenu.lastFocusedElement.gameObject.activeInHierarchy)
			{
				EventSystem.current.SetSelectedGameObject(activeMenu.lastFocusedElement);
			}
			InputDevice activeDevice = InputManager.ActiveDevice;
			if (activeDevice.Action2.WasPressed || Input.GetKeyDown(KeyCode.Escape))
			{
				activeMenu.OnBack();
			}
		}
	}

	public static bool CustomCanInvoke(bool checkMenuState = true, bool checkNetworkState = true, bool checkTimeSinceLast = true)
	{
		if (checkMenuState && instance.state == MenuSystemState.Inactive)
		{
			return false;
		}
		if (checkNetworkState && NetGame.instance.transport.ShouldInhibitUIExceptCancel())
		{
			return false;
		}
		if (checkTimeSinceLast && timeSinceLast <= 0.1f)
		{
			return false;
		}
		timeSinceLast = 0f;
		return true;
	}

	public eInputDeviceType GetCurrentInputDevice()
	{
		return mDevice;
	}

	private void UpdateInputDeviceType()
	{
		if (mDevice != eInputDeviceType.Controller && ControlerInput())
		{
			SetInputDevice(eInputDeviceType.Controller);
		}
		else if (mDevice != 0 && MouseInput())
		{
			SetInputDevice(eInputDeviceType.Mouse);
		}
		else if (mDevice != eInputDeviceType.Keyboard && KeyboardInput())
		{
			SetInputDevice(eInputDeviceType.Keyboard);
		}
	}

	private void SetInputDevice(eInputDeviceType device)
	{
		mDevice = device;
		InputDeviceChange(mDevice);
	}

	private bool MouseInput()
	{
		if (Event.current.isMouse)
		{
			return true;
		}
		if (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f)
		{
			return true;
		}
		return false;
	}

	private bool KeyboardInput()
	{
		return Event.current.isKey;
	}

	private bool ControlerInput()
	{
		if (Input.GetKey(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.JoystickButton2) || Input.GetKey(KeyCode.JoystickButton3) || Input.GetKey(KeyCode.JoystickButton4) || Input.GetKey(KeyCode.JoystickButton5) || Input.GetKey(KeyCode.JoystickButton6) || Input.GetKey(KeyCode.JoystickButton7) || Input.GetKey(KeyCode.JoystickButton8) || Input.GetKey(KeyCode.JoystickButton9) || Input.GetKey(KeyCode.JoystickButton10) || Input.GetKey(KeyCode.JoystickButton11) || Input.GetKey(KeyCode.JoystickButton12) || Input.GetKey(KeyCode.JoystickButton13) || Input.GetKey(KeyCode.JoystickButton14) || Input.GetKey(KeyCode.JoystickButton15) || Input.GetKey(KeyCode.JoystickButton16) || Input.GetKey(KeyCode.JoystickButton17) || Input.GetKey(KeyCode.JoystickButton18) || Input.GetKey(KeyCode.JoystickButton19))
		{
			return true;
		}
		string[] joystickNames = Input.GetJoystickNames();
		if (Math.Abs(Input.GetAxis("joystick 1 analog 0")) > 0.5f || Math.Abs(Input.GetAxis("joystick 1 analog 1")) > 0.5f || Math.Abs(Input.GetAxis("joystick 1 analog 2")) > 0.5f || Math.Abs(Input.GetAxis("joystick 1 analog 5")) > 0.5f || Math.Abs(Input.GetAxis("joystick 1 analog 6")) > 0.5f || Math.Abs(Input.GetAxis("joystick 1 analog 7")) > 0.5f || Math.Abs(Input.GetAxis("joystick 1 analog 8")) > 0.5f || Math.Abs(Input.GetAxis("joystick 1 analog 9")) > 0.5f || Math.Abs(Input.GetAxis("joystick 1 analog 10")) > 0.5f || Math.Abs(Input.GetAxis("joystick 1 analog 11")) > 0.5f || Math.Abs(Input.GetAxis("joystick 1 analog 12")) > 0.5f || Math.Abs(Input.GetAxis("joystick 1 analog 13")) > 0.5f || Math.Abs(Input.GetAxis("joystick 1 analog 14")) > 0.5f || Math.Abs(Input.GetAxis("joystick 1 analog 15")) > 0.5f || Math.Abs(Input.GetAxis("joystick 1 analog 16")) > 0.5f || Math.Abs(Input.GetAxis("joystick 1 analog 17")) > 0.5f || Math.Abs(Input.GetAxis("joystick 1 analog 18")) > 0.5f || Math.Abs(Input.GetAxis("joystick 1 analog 19")) > 0.5f)
		{
			return true;
		}
		return false;
	}
}
