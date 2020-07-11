using HumanAPI;
using InControl;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomizationPresetEditMenu : MenuTransition
{
	public TMP_InputField title;

	public TMP_InputField description;

	public GameObject titleArea;

	public GameObject descriptionArea;

	private RagdollCustomization ragdollCustomization;

	public static RagdollPresetMetadata targetPreset;

	public GameObject titleText;

	public GameObject descriptionText;

	public GameObject buttonSave;

	public GameObject buttonBack;

	public override void OnLostFocus()
	{
		base.OnLostFocus();
		EventSystem.current.sendNavigationEvents = true;
	}

	public override void OnGotFocus()
	{
		base.OnGotFocus();
		EventSystem.current.sendNavigationEvents = false;
		RagdollPresetMetadata skin = CustomizationController.instance.GetSkin();
		title.text = ((targetPreset == null) ? skin.title : targetPreset.title);
		description.text = ((targetPreset == null) ? skin.description : targetPreset.description);
	}

	public override void OnTansitionedIn()
	{
		base.OnTansitionedIn();
		MenuSystem.instance.FocusOnMouseOver(enable: false);
	}

	public override void OnBack()
	{
		BackClick();
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			CustomizationPresetMenu.dontReload = true;
			TransitionBack<CustomizationPresetMenu>();
		}
	}

	private bool Up()
	{
		InputDevice activeDevice = InputManager.ActiveDevice;
		if (((bool)activeDevice.DPadUp && activeDevice.DPadUp.HasChanged) || (activeDevice.LeftStickUp.State && !activeDevice.LeftStickUp.LastState) || Input.GetKeyDown(KeyCode.UpArrow))
		{
			return true;
		}
		return false;
	}

	private bool Down()
	{
		InputDevice activeDevice = InputManager.ActiveDevice;
		if (((bool)InputManager.ActiveDevice.DPadDown && InputManager.ActiveDevice.DPadDown.HasChanged) || (InputManager.ActiveDevice.LeftStickDown.State && !InputManager.ActiveDevice.LeftStickDown.LastState) || Input.GetKeyDown(KeyCode.DownArrow))
		{
			return true;
		}
		return false;
	}

	private bool Tab()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			return true;
		}
		return false;
	}

	private bool Select()
	{
		InputDevice activeDevice = InputManager.ActiveDevice;
		if ((activeDevice.Action1.IsPressed && activeDevice.Action1.HasChanged) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
		{
			return true;
		}
		return false;
	}

	protected override void Update()
	{
		base.Update();
		GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
		Selectable selectable = null;
		if (Up())
		{
			selectable = currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
		}
		if (Down())
		{
			selectable = currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
		}
		if (selectable != null)
		{
			EventSystem.current.SetSelectedGameObject(selectable.gameObject);
		}
		if (Tab())
		{
		}
		selectable = null;
		if (Select())
		{
			UnityEngine.UI.Button component = currentSelectedGameObject.GetComponent<UnityEngine.UI.Button>();
			if (component != null)
			{
				BaseEventData eventData = new BaseEventData(EventSystem.current);
				component.OnSubmit(eventData);
			}
		}
	}

	public void SaveClick()
	{
		if (MenuSystem.CanInvoke)
		{
			int length = Math.Min(128, title.text.Length);
			string text = title.text.Substring(0, length);
			int length2 = Math.Min(7999, description.text.Length);
			string text2 = description.text.Substring(0, length2);
			CustomizationSavedMenu.preset = CustomizationController.instance.SavePreset(targetPreset, text, text2);
			TransitionForward<CustomizationSavedMenu>();
		}
	}
}
