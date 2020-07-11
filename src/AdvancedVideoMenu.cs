using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AdvancedVideoMenu : MenuTransition
{
	public MenuSelector presetSelector;

	public MenuSlider cloudSlider;

	public MenuSlider shadowSlider;

	public MenuSlider textureSlider;

	public MenuSelector ambientSelector;

	public MenuSelector antialiasingSelector;

	public MenuSelector vsyncSelector;

	public MenuSelector hdrSelector;

	public MenuSelector bloomSelector;

	public MenuSelector depthSelector;

	public MenuSelector chromaSelector;

	public MenuSelector exposureSelector;

	public Button applyButton;

	private List<LayoutElement> allButtons;

	private List<float> allButtonHeights;

	public static bool hasUnappliedChanges;

	public override void ApplyMenuEffects()
	{
		MenuCameraEffects.FadeInPauseMenu();
	}

	public override void OnGotFocus()
	{
		if (allButtons == null)
		{
			allButtons = new List<LayoutElement>();
			allButtonHeights = new List<float>();
			for (int i = 2; i < applyButton.transform.parent.childCount - 4; i++)
			{
				LayoutElement component = applyButton.transform.parent.GetChild(i).GetComponent<LayoutElement>();
				if (component != null)
				{
					allButtons.Add(component);
					allButtonHeights.Add(component.preferredHeight);
				}
			}
		}
		base.OnGotFocus();
		if (!hasUnappliedChanges)
		{
			HideApply();
			presetSelector.SelectIndex(Options.advancedVideoPreset);
			cloudSlider.value = Options.advancedVideoClouds;
			shadowSlider.value = Options.advancedVideoShadows;
			textureSlider.value = Options.advancedVideoTexture;
			ambientSelector.SelectIndex(Options.advancedVideoAO);
			antialiasingSelector.SelectIndex(Options.advancedVideoAA);
			vsyncSelector.SelectIndex(Options.advancedVideoVsync);
			hdrSelector.SelectIndex(Options.advancedVideoHDR);
			bloomSelector.SelectIndex(Options.advancedVideoBloom);
			depthSelector.SelectIndex(Options.advancedVideoDOF);
			chromaSelector.SelectIndex(Options.advancedVideoChromatic);
			exposureSelector.SelectIndex(Options.advancedVideoExposure);
		}
	}

	public void PresetChanged(int value)
	{
		if (MenuSystem.CanInvoke)
		{
			ShowApply();
			if (value != 0)
			{
				OptionsVideoPreset optionsVideoPreset = OptionsVideoPreset.Create(value);
				cloudSlider.value = optionsVideoPreset.clouds;
				shadowSlider.value = optionsVideoPreset.shadows;
				textureSlider.value = optionsVideoPreset.texture;
				ambientSelector.SelectIndex(optionsVideoPreset.ao);
				antialiasingSelector.SelectIndex(optionsVideoPreset.aa);
				vsyncSelector.SelectIndex(optionsVideoPreset.vsync);
				hdrSelector.SelectIndex(optionsVideoPreset.hdr);
				bloomSelector.SelectIndex(optionsVideoPreset.bloom);
				depthSelector.SelectIndex(optionsVideoPreset.dof);
				chromaSelector.SelectIndex(optionsVideoPreset.chroma);
				exposureSelector.SelectIndex(optionsVideoPreset.exposure);
			}
		}
	}

	public void CloudChanged(float value)
	{
		if (MenuSystem.CanInvoke)
		{
			presetSelector.SelectIndex(0);
			ShowApply();
		}
	}

	public void ShadowChanged(float value)
	{
		if (MenuSystem.CanInvoke)
		{
			presetSelector.SelectIndex(0);
			ShowApply();
		}
	}

	public void TextureChanged(float value)
	{
		if (MenuSystem.CanInvoke)
		{
			presetSelector.SelectIndex(0);
			ShowApply();
		}
	}

	public void AmbientChanged(int value)
	{
		if (MenuSystem.CanInvoke)
		{
			presetSelector.SelectIndex(0);
			ShowApply();
		}
	}

	public void AntialiasingChanged(int value)
	{
		if (MenuSystem.CanInvoke)
		{
			presetSelector.SelectIndex(0);
			ShowApply();
		}
	}

	public void VsyncChanged(int value)
	{
		if (MenuSystem.CanInvoke)
		{
			presetSelector.SelectIndex(0);
			ShowApply();
		}
	}

	public void OptionChanged(int value)
	{
		if (MenuSystem.CanInvoke)
		{
			presetSelector.SelectIndex(0);
			ShowApply();
		}
	}

	public void ApplyClick()
	{
		if (MenuSystem.CanInvoke)
		{
			Options.advancedVideoPreset = presetSelector.selectedIndex;
			Options.advancedVideoClouds = (int)cloudSlider.value;
			Options.advancedVideoShadows = (int)shadowSlider.value;
			Options.advancedVideoTexture = (int)textureSlider.value;
			Options.advancedVideoAO = ambientSelector.selectedIndex;
			Options.advancedVideoAA = antialiasingSelector.selectedIndex;
			Options.advancedVideoVsync = vsyncSelector.selectedIndex;
			Options.advancedVideoHDR = hdrSelector.selectedIndex;
			Options.advancedVideoBloom = bloomSelector.selectedIndex;
			Options.advancedVideoDOF = depthSelector.selectedIndex;
			Options.advancedVideoChromatic = chromaSelector.selectedIndex;
			Options.advancedVideoExposure = exposureSelector.selectedIndex;
			EventSystem.current.SetSelectedGameObject(applyButton.navigation.selectOnUp.gameObject);
			HideApply();
			Options.ApplyAdvancedVideo();
		}
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			if (hasUnappliedChanges)
			{
				ConfirmDiscardVideoMenu.confirmTarget = ConfirmDiscardNavigationTarget.Video;
				ConfirmDiscardVideoMenu.backTarget = ConfirmDiscardNavigationTarget.AdvancedVideo;
				TransitionForward<ConfirmDiscardVideoMenu>();
			}
			else
			{
				TransitionBack<VideoMenu>();
			}
		}
	}

	public override void OnBack()
	{
		BackClick();
	}

	private void ShowApply()
	{
		hasUnappliedChanges = true;
		applyButton.gameObject.SetActive(value: true);
		applyButton.GetComponentInParent<AutoNavigation>().Invalidate();
	}

	private void HideApply()
	{
		hasUnappliedChanges = false;
		applyButton.gameObject.SetActive(value: false);
		if (EventSystem.current.currentSelectedGameObject == applyButton.gameObject)
		{
			EventSystem.current.SetSelectedGameObject(defaultElement);
		}
	}
}
