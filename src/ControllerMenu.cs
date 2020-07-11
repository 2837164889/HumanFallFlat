using UnityEngine;

public class ControllerMenu : MenuTransition
{
	public MenuSelector layoutSelector;

	public MenuSelector invertSelector;

	public MenuSelector invertSelectorH;

	public MenuSlider hSensitivity;

	public MenuSlider vSensitivity;

	public MenuSelector lookModeSelector;

	public GameObject[] SALayoutImages;

	public GameObject[] XboxLayoutImages;

	public GameObject[] SwitchLayoutImages;

	public GameObject[] PS4LayoutImages;

	private GameObject[] layoutImages;

	private void Awake()
	{
		layoutImages = XboxLayoutImages;
	}

	public override void ApplyMenuEffects()
	{
		MenuCameraEffects.FadeInPauseMenu();
	}

	public override void OnGotFocus()
	{
		base.OnGotFocus();
		layoutSelector.SelectIndex(Options.controllerLayout);
		invertSelector.SelectIndex(Options.controllerInvert);
		invertSelectorH.SelectIndex(Options.controllerInvertH);
		hSensitivity.value = Options.controllerHSensitivity;
		vSensitivity.value = Options.controllerVSensitivity;
		lookModeSelector.SelectIndex(Options.controllerLookMode);
		RebindControllerImage();
	}

	public override void OnLostFocus()
	{
		TutorialBlock.RefreshTextOnAllBlocks();
		base.OnLostFocus();
	}

	public void LayoutChanged(int value)
	{
		Options.controllerLayout = value;
		RebindControllerImage();
	}

	public void InvertChangedV(int value)
	{
		Options.controllerInvert = value;
	}

	public void InvertChangedH(int value)
	{
		Options.controllerInvertH = value;
	}

	public void HSensitivityChanged(float value)
	{
		Options.controllerHSensitivity = value;
	}

	public void VSensitivityChanged(float value)
	{
		Options.controllerVSensitivity = value;
	}

	public void LookModeChanged(int value)
	{
		Options.controllerLookMode = value;
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionBack<ControlsMenu>();
		}
	}

	public override void OnBack()
	{
		BackClick();
	}

	public void RebindControllerImage()
	{
		for (int i = 0; i < layoutImages.Length; i++)
		{
			layoutImages[i].SetActive(i == layoutSelector.selectedIndex);
		}
	}

	protected override void Update()
	{
		base.Update();
	}
}
