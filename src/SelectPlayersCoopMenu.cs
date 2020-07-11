using I2.Loc;
using InControl;
using Multiplayer;
using System.Collections;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectPlayersCoopMenu : MenuTransition
{
	public Button backButton;

	public Button playButton;

	public Button reconfigureButton;

	public GameObject explanation;

	public TextMeshProUGUI leftPrompt;

	public TextMeshProUGUI rightPrompt;

	public static Coroutine coroutine;

	public override void OnGotFocus()
	{
		base.OnGotFocus();
		coroutine = StartCoroutine(ConfigureCoop());
	}

	protected override void Update()
	{
		base.Update();
		if (coroutine == null && MenuSystem.instance.activeMenu == this && NetGame.instance.local.players.Count < 2)
		{
			coroutine = StartCoroutine(ConfigureCoop());
		}
	}

	private string GetDeviceName(InputDevice device)
	{
		if (device == null)
		{
			return ScriptLocalization.Get("MENU/COOP/MouseAndKeys");
		}
		ReadOnlyCollection<InputDevice> devices = InputManager.Devices;
		for (int i = 0; i < devices.Count; i++)
		{
			if (devices[i] == device)
			{
				return string.Format(ScriptLocalization.Get("MENU/COOP/Controller"), i + 1);
			}
		}
		return string.Empty;
	}

	private IEnumerator ConfigureCoop()
	{
		while (NetGame.instance.local.players.Count < 2)
		{
			explanation.SetActive(value: false);
			playButton.gameObject.SetActive(value: false);
			reconfigureButton.gameObject.SetActive(value: false);
			GetComponent<AutoNavigation>().defaultItem = backButton;
			GetComponent<AutoNavigation>().Invalidate();
			backButton.Select();
			leftPrompt.text = ScriptLocalization.Get("MENU/COOP/P1Raise");
			rightPrompt.text = string.Empty;
			while (!PlayerManager.instance.p1Locked)
			{
				yield return null;
			}
			leftPrompt.text = GetDeviceName(PlayerManager.instance.p1Device);
			rightPrompt.text = ((PlayerManager.instance.p1Device != null) ? ScriptLocalization.Get("MENU/COOP/P2Raise") : ScriptLocalization.Get("MENU/COOP/P2RaiseController"));
			while (PlayerManager.instance.p1Locked && !PlayerManager.instance.p2Locked)
			{
				yield return null;
			}
		}
		explanation.SetActive(value: true);
		leftPrompt.text = GetDeviceName(PlayerManager.instance.p1Device);
		rightPrompt.text = GetDeviceName(PlayerManager.instance.p2Device);
		playButton.gameObject.SetActive(value: true);
		reconfigureButton.gameObject.SetActive(value: true);
		GetComponent<AutoNavigation>().defaultItem = playButton;
		GetComponent<AutoNavigation>().Invalidate();
		playButton.Select();
		coroutine = null;
	}

	public void ContinueClick()
	{
		if (MenuSystem.CanInvoke)
		{
			if (coroutine != null)
			{
				StopCoroutine(coroutine);
				coroutine = null;
			}
			TransitionForward<PlayMenu>();
		}
	}

	public void ReconfigureClick()
	{
		PlayerManager.SetSingle();
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			if (coroutine != null)
			{
				StopCoroutine(coroutine);
				coroutine = null;
			}
			PlayerManager.SetSingle();
			TransitionBack<SelectPlayersMenu>();
		}
	}

	public override void ApplyMenuEffects()
	{
		MenuCameraEffects.FadeInCoopMenu();
	}

	public override void OnBack()
	{
		BackClick();
	}
}
