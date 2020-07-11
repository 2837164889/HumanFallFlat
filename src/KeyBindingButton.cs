using InControl;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyBindingButton : MonoBehaviour
{
	public TextMeshProUGUI keysText;

	public TextMeshProUGUI labelText;

	[NonSerialized]
	public PlayerAction action;

	public ConfigureKeysMenu parent;

	[NonSerialized]
	private InControlInputModule inputModule;

	public Image listenRect;

	public Image coverRect;

	private void OnEnable()
	{
		listenRect.GetComponent<CanvasRenderer>().SetColor(new Color(1f, 1f, 1f, 0f));
		coverRect.GetComponent<CanvasRenderer>().SetColor(new Color(1f, 1f, 1f, 0f));
	}

	private void Start()
	{
		BindData();
	}

	public void BindData()
	{
		BindKeysTextToAction();
	}

	public void OnClick()
	{
		MenuSystem.keyboardState = KeyboardState.BindKeyboardKey;
		inputModule = UnityEngine.Object.FindObjectOfType<InControlInputModule>();
		inputModule.gameObject.SetActive(value: false);
		listenRect.CrossFadeAlpha(1f, 0.2f, ignoreTimeScale: true);
		coverRect.GetComponent<CanvasRenderer>().SetColor(Color.white);
		GetComponent<Image>().CrossFadeColor(new Color(1f, 0.8f, 0f, 1f), 0f, ignoreTimeScale: true, useAlpha: true);
		keysText.text = "????";
		action.Owner.ListenOptions.OnBindingFound = OnBindingFound;
		action.Owner.ListenOptions.OnBindingAdded = OnBindingAdded;
		action.Owner.ListenOptions.OnBindingRejected = OnBindingRejected;
		action.ListenForBinding();
	}

	private bool OnBindingFound(PlayerAction action, BindingSource binding)
	{
		bool result;
		if (binding == new KeyBindingSource(Key.Escape))
		{
			action.StopListeningForBinding();
			result = false;
		}
		else
		{
			keysText.text = binding.Name;
			StartCoroutine(SaveBindingsNextFrame());
			result = true;
		}
		action.Owner.ListenOptions.OnBindingFound = null;
		action.Owner.ListenOptions.OnBindingAdded = null;
		action.Owner.ListenOptions.OnBindingRejected = null;
		listenRect.CrossFadeAlpha(0f, 0.2f, ignoreTimeScale: true);
		coverRect.GetComponent<CanvasRenderer>().SetColor(new Color(1f, 1f, 1f, 0f));
		inputModule.gameObject.SetActive(value: true);
		MenuSystem.keyboardState = KeyboardState.None;
		return result;
	}

	private IEnumerator SaveBindingsNextFrame()
	{
		yield return null;
		Options.SaveKeyboardBindings();
		parent.RebindKeys();
	}

	private void OnBindingAdded(PlayerAction action, BindingSource binding)
	{
		Debug.Log("Binding added... " + binding.DeviceName + ": " + binding.Name);
	}

	private void OnBindingRejected(PlayerAction action, BindingSource binding, BindingSourceRejectionType reason)
	{
		Debug.Log("Binding rejected... " + binding.DeviceName + ": " + binding.Name + " " + reason);
	}

	private void Update()
	{
	}

	private void BindKeysTextToAction()
	{
		if (action.Bindings.Count > 0)
		{
			keysText.text = action.Bindings[0].Name;
		}
		else
		{
			keysText.text = "---";
		}
	}
}
