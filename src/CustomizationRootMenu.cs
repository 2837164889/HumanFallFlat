using Multiplayer;
using System.Collections;
using UnityEngine;

public class CustomizationRootMenu : MenuTransition
{
	private enum StartPhase
	{
		kNone,
		kFadeOutStart,
		kFadeOut,
		kWait,
		kFadeIn,
		kDone
	}

	public ButtonGroup menuButtons;

	private StartPhase mPhase;

	private float mPhaseTimer;

	private const float kFadeInTime = 0.5f;

	private const float kFadeOutTime = 0.25f;

	private WhiteOut mWhiteOut;

	private volatile bool mSceneLoaded;

	private bool goingBack;

	private void NewPhase(StartPhase phase)
	{
		mPhaseTimer = 0f;
		mPhase = phase;
	}

	private void SetupScene()
	{
		if (CustomizationController.instance == null)
		{
			goingBack = true;
			App.instance.EnterCustomization(CustomizationControllerLoaded);
		}
		else
		{
			SetupView();
		}
	}

	public override void ApplyMenuEffects()
	{
		if (CustomizationController.instance != null)
		{
			MenuCameraEffects.FadeInMainMenu();
		}
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (hasFocus)
		{
			OnGotFocus();
		}
	}

	public override void OnGotFocus()
	{
		base.OnGotFocus();
		goingBack = false;
		menuButtons.RebuildLinks();
		SetupScene();
	}

	private void CustomizationControllerLoaded()
	{
		CustomizationController.instance.Initialize();
		SetupView();
		mSceneLoaded = true;
		StartCoroutine(CompleteTransitionIn());
	}

	private IEnumerator CompleteTransitionIn()
	{
		for (int index = 0; index < 3; index++)
		{
			yield return null;
		}
		MenuCameraEffects.FadeFromBlack(0.25f);
		yield return new WaitForSeconds(0.15f);
		Transition(0f, 0.3f);
		goingBack = false;
	}

	private static void SetupView()
	{
		CustomizationController.instance.ShowBoth();
		CustomizationController.instance.cameraController.FocusBoth();
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke && !goingBack)
		{
			goingBack = true;
			StartCoroutine(StallFreeBack());
		}
	}

	private IEnumerator StallFreeBack()
	{
		float dur = 0.1f;
		if (MenuSystem.instance.activeMenu == this)
		{
			MenuSystem.instance.FocusOnMouseOver(enable: false);
			Transition(0.9999f, 0f);
		}
		MenuCameraEffects.FadeToBlack(dur);
		yield return null;
		yield return new WaitForSeconds(dur);
		yield return null;
		App.instance.LeaveCustomization();
		while (!Game.instance.HasSceneLoaded)
		{
			yield return null;
		}
		for (int index = 0; index < 3; index++)
		{
			yield return null;
		}
		goingBack = false;
		MenuSystem.instance.TransitionBack<MainMenu>(this, 0f);
	}

	public override void OnBack()
	{
		BackClick();
	}

	public void EditClick()
	{
		if (MenuSystem.CanInvoke && !goingBack && !(CustomizationController.instance == null) && !(CustomizationController.instance.activeCustomization == null))
		{
			TransitionForward<CustomizationEditMenu>();
			CustomizationController.instance.ShowActive();
		}
	}

	public void TogglePlayerClick()
	{
		if (MenuSystem.CanInvoke && !goingBack && !(CustomizationController.instance == null) && !(CustomizationController.instance.activeCustomization == null))
		{
			CustomizationController.instance.SetActivePlayer((CustomizationController.instance.activePlayer + 1) % 2);
			CustomizationController.instance.cameraController.FocusBoth();
		}
	}

	public void SaveClick()
	{
		if (MenuSystem.CanInvoke && !goingBack && !(CustomizationController.instance == null) && !(CustomizationController.instance.activeCustomization == null))
		{
			CustomizationPresetMenu.mode = CustomizationPresetMenuMode.Save;
			CustomizationController.instance.ShowActive();
			TransitionForward<CustomizationPresetMenu>();
		}
	}

	public void LoadClick()
	{
		if (MenuSystem.CanInvoke && !goingBack && !(CustomizationController.instance == null) && !(CustomizationController.instance.activeCustomization == null))
		{
			CustomizationPresetMenu.mode = CustomizationPresetMenuMode.Load;
			TransitionForward<CustomizationPresetMenu>();
		}
	}
}
