using InControl;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogOverlay : MonoBehaviour
{
	private static DialogOverlay instance;

	public Image background;

	public GameObject progressRing;

	public TextMeshProUGUI titleText;

	public TextMeshProUGUI descriptionText;

	public TextMeshProUGUI backText;

	public GameObject backButton;

	private static Action onBack;

	private int hideDelay;

	public static int backHideDelay;

	public static bool IsShowing
	{
		get;
		private set;
	}

	private void Awake()
	{
		instance = this;
		instance.background.gameObject.SetActive(value: false);
		hideDelay = 0;
	}

	public static bool IsOn()
	{
		return instance != null && instance.background.gameObject.activeSelf && instance.hideDelay == 0;
	}

	public static bool IsOnIncludingDelay()
	{
		return instance != null && instance.background.gameObject.activeSelf;
	}

	public static float GetOpacity()
	{
		float result;
		if (IsOnIncludingDelay())
		{
			Color color = instance.background.color;
			result = color.a;
		}
		else
		{
			result = 0f;
		}
		return result;
	}

	public static string GetCurrentTitle()
	{
		if (!IsOn() || instance == null || instance.titleText == null)
		{
			return null;
		}
		return instance.titleText.text;
	}

	public static void DisableCancel()
	{
		onBack = null;
		instance.backButton.SetActive(value: false);
	}

	public static void Show(float opacity, bool showProgress, string title, string description, string backLabel, Action backAction)
	{
		instance.hideDelay = 0;
		IsShowing = true;
		MenuSystem.keyboardState = KeyboardState.Dialog;
		instance.background.color = new Color(0f, 0f, 0f, opacity);
		instance.background.gameObject.SetActive(value: true);
		instance.transform.SetAsLastSibling();
		instance.progressRing.SetActive(showProgress);
		instance.titleText.text = title;
		if (string.IsNullOrEmpty(description))
		{
			instance.descriptionText.gameObject.SetActive(value: false);
		}
		else
		{
			instance.descriptionText.gameObject.SetActive(value: true);
			instance.descriptionText.text = description;
		}
		if (string.IsNullOrEmpty(backLabel))
		{
			instance.backButton.SetActive(value: false);
			EventSystem.current.SetSelectedGameObject(instance.backButton);
			if ((bool)MenuSystem.instance && (bool)MenuSystem.instance.activeMenu)
			{
				MenuSystem.instance.activeMenu.lastFocusedElement = instance.backButton;
			}
			onBack = backAction;
			backHideDelay = 0;
		}
		else
		{
			instance.backText.text = backLabel;
			onBack = backAction;
			backHideDelay = 0;
			instance.backButton.SetActive(value: true);
			EventSystem.current.SetSelectedGameObject(instance.backButton);
			if ((bool)MenuSystem.instance && (bool)MenuSystem.instance.activeMenu)
			{
				MenuSystem.instance.activeMenu.lastFocusedElement = instance.backButton;
			}
		}
		ButtonLegendBar.RefreshStatus();
	}

	public void BackClick()
	{
		if (hideDelay <= 0 && MenuSystem.CustomCanInvoke(checkMenuState: false, checkNetworkState: false) && onBack != null)
		{
			Action action = onBack;
			Hide(backHideDelay);
			action();
		}
	}

	public static void Hide(int withDelay = 0)
	{
		bool activeSelf = instance.background.gameObject.activeSelf;
		IsShowing = false;
		MenuSystem.keyboardState = KeyboardState.None;
		instance.background.gameObject.SetActive(value: false);
		onBack = null;
		instance.hideDelay = Math.Max(withDelay, instance.hideDelay);
		if (activeSelf && instance.hideDelay > 0)
		{
			instance.background.gameObject.SetActive(value: true);
			instance.background.color = new Color(0f, 0f, 0f, 1f);
			instance.progressRing.SetActive(value: false);
			instance.titleText.text = string.Empty;
			instance.descriptionText.gameObject.SetActive(value: false);
			instance.backButton.SetActive(value: false);
		}
		ButtonLegendBar.RefreshStatus();
	}

	private void Update()
	{
		if (hideDelay > 0)
		{
			if (--hideDelay == 0)
			{
				instance.background.gameObject.SetActive(value: false);
				ButtonLegendBar.RefreshStatus();
			}
		}
		else if (onBack != null)
		{
			InputDevice activeDevice = InputManager.ActiveDevice;
			if (activeDevice.MenuWasPressed || activeDevice.Action2.WasPressed || Input.GetKeyDown(KeyCode.Escape))
			{
				BackClick();
			}
		}
	}
}
