using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VideoMenu : MenuTransition
{
	public MenuSlider brightnessSlider;

	public MenuSelector resolutionSelector;

	public MenuSelector fullscreenSelector;

	public Button applyButton;

	private List<Vector2> resolutions;

	private int currentResolutionIndex = -1;

	public static bool hasUnappliedChanges;

	public override void ApplyMenuEffects()
	{
		MenuCameraEffects.FadeInPauseMenu();
	}

	public override void OnGotFocus()
	{
		base.OnGotFocus();
		if (hasUnappliedChanges)
		{
			return;
		}
		HideApply();
		brightnessSlider.value = Options.videoBrightness;
		Vector2 a = new Vector2(Screen.width, Screen.height);
		bool fullScreen = Screen.fullScreen;
		Resolution[] array = Screen.resolutions;
		resolutions = new List<Vector2>();
		List<string> list = new List<string>();
		float num = float.MaxValue;
		for (int i = 0; i < array.Length; i++)
		{
			Resolution resolution = array[i];
			Vector2 b = new Vector2(resolution.width, resolution.height);
			string item = $"{resolution.width} x {resolution.height}";
			if (!list.Contains(item))
			{
				float sqrMagnitude = (a - b).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					currentResolutionIndex = list.Count;
				}
				resolutions.Add(new Vector2(resolution.width, resolution.height));
				list.Add(item);
			}
		}
		resolutionSelector.options = list.ToArray();
		resolutionSelector.SelectIndex(currentResolutionIndex);
		fullscreenSelector.SelectIndex(fullScreen ? 1 : 0);
	}

	public void BrightnessChanged(float value)
	{
		Options.videoBrightness = (int)value;
	}

	public void ResolutionChanged(int value)
	{
		ShowApply();
	}

	public void FullscreenChanged(int value)
	{
		ShowApply();
	}

	public void AdvancedClick()
	{
		if (MenuSystem.CanInvoke)
		{
			if (hasUnappliedChanges)
			{
				ConfirmDiscardVideoMenu.confirmTarget = ConfirmDiscardNavigationTarget.AdvancedVideo;
				ConfirmDiscardVideoMenu.backTarget = ConfirmDiscardNavigationTarget.Video;
				TransitionForward<ConfirmDiscardVideoMenu>();
			}
			else
			{
				TransitionForward<AdvancedVideoMenu>();
			}
		}
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			if (hasUnappliedChanges)
			{
				ConfirmDiscardVideoMenu.confirmTarget = ConfirmDiscardNavigationTarget.Options;
				ConfirmDiscardVideoMenu.backTarget = ConfirmDiscardNavigationTarget.Video;
				TransitionForward<ConfirmDiscardVideoMenu>();
			}
			else
			{
				TransitionBack<OptionsMenu>();
			}
		}
	}

	public override void OnBack()
	{
		BackClick();
	}

	public void ApplyClick()
	{
		if (MenuSystem.CanInvoke)
		{
			Vector2 vector = resolutions[resolutionSelector.selectedIndex];
			int width = (int)vector.x;
			Vector2 vector2 = resolutions[resolutionSelector.selectedIndex];
			StartCoroutine(ForceResolution(width, (int)vector2.y, fullscreenSelector.selectedIndex > 0));
			EventSystem.current.SetSelectedGameObject(applyButton.navigation.selectOnUp.gameObject);
			HideApply();
		}
	}

	private IEnumerator ForceResolution(int width, int height, bool fullScreen)
	{
		if (Screen.fullScreen == fullScreen)
		{
			Screen.SetResolution(width, height, fullScreen);
		}
		else if (fullScreen)
		{
			Screen.SetResolution(width, height, fullScreen);
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			Screen.fullScreen = fullScreen;
		}
		else
		{
			Screen.fullScreen = fullScreen;
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			Screen.SetResolution(width, height, fullScreen);
		}
		Canvas.ForceUpdateCanvases();
	}

	private void ShowApply()
	{
		hasUnappliedChanges = true;
		applyButton.gameObject.SetActive(value: true);
		Link(applyButton, applyButton.navigation.selectOnDown);
		Link(applyButton.navigation.selectOnUp, applyButton);
	}

	private void HideApply()
	{
		hasUnappliedChanges = false;
		applyButton.gameObject.SetActive(value: false);
		Link(applyButton.navigation.selectOnUp, applyButton.navigation.selectOnDown);
		if (EventSystem.current.currentSelectedGameObject == applyButton.gameObject)
		{
			EventSystem.current.SetSelectedGameObject(defaultElement);
		}
	}
}
