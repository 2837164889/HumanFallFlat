using HumanAPI;
using I2.Loc;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomizationColorsMenu : MenuTransition
{
	public Transform channelsContainer;

	public Transform colorsContainer;

	public Transform colorsPanel;

	public CustomizationColorsMenuChannel channelTemplate;

	public CustomizationColorsMenuColor colorTemplate;

	public Color[] colors;

	public Color[] baseColors;

	public CustomizationColorsMenuColor transparent;

	public GameObject title;

	public RectTransform buttons;

	private List<CustomizationColorsMenuChannel> channelButtons = new List<CustomizationColorsMenuChannel>();

	private List<CustomizationColorsMenuColor> colorButtons = new List<CustomizationColorsMenuColor>();

	private CustomizationColorsMenuChannel selectedChannel;

	private Color colorBackup;

	private bool showPalette;

	private bool colorApplied;

	public override void OnTansitionedIn()
	{
		base.OnTansitionedIn();
		MenuSystem.instance.FocusOnMouseOver(enable: false);
	}

	public override void OnGotFocus()
	{
		base.OnGotFocus();
		CustomizationController.instance.cameraController.FocusCharacterModel();
		AddPartButtons(WorkshopItemType.ModelFull, ScriptLocalization.CUSTOMIZATION.PartBody);
		AddPartButtons(WorkshopItemType.ModelHead, ScriptLocalization.CUSTOMIZATION.PartHead);
		AddPartButtons(WorkshopItemType.ModelUpperBody, ScriptLocalization.CUSTOMIZATION.PartUpper);
		AddPartButtons(WorkshopItemType.ModelLowerBody, ScriptLocalization.CUSTOMIZATION.PartLower);
		selectedChannel = channelButtons[0];
		EventSystem.current.SetSelectedGameObject(selectedChannel.gameObject);
		AddColorButtons();
	}

	public override void OnLostFocus()
	{
		base.OnLostFocus();
		for (int i = 0; i < channelButtons.Count; i++)
		{
			Object.Destroy(channelButtons[i].gameObject);
		}
		channelButtons.Clear();
		if (colorsContainer != null)
		{
			AutoNavigation component = colorsContainer.GetComponent<AutoNavigation>();
			if (component != null)
			{
				component.ClearCurrent();
			}
		}
		for (int j = 0; j < colorButtons.Count; j++)
		{
			Object.Destroy(colorButtons[j].gameObject);
		}
		colorButtons.Clear();
	}

	public override void OnBack()
	{
		BackClick();
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			if (showPalette)
			{
				ShowColorPalette(show: false);
				CustomizationController.instance.SetColor(selectedChannel.part, selectedChannel.number, colorBackup);
				EventSystem.current.SetSelectedGameObject(selectedChannel.gameObject);
				colorApplied = false;
			}
			else
			{
				TransitionBack<CustomizationEditMenu>();
				CustomizationController.instance.Rollback();
			}
		}
	}

	public void ApplyClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionBack<CustomizationEditMenu>();
			CustomizationController.instance.CommitParts();
		}
	}

	private void AddColorButtons()
	{
		colors = new Color[baseColors.Length * 5];
		for (int i = 0; i < baseColors.Length; i++)
		{
			colors[i + 0 * baseColors.Length] = baseColors[i].Desaturate(0.2f).Desaturate(0.4f).Brighten((i != 0) ? 0.4f : 1f);
			colors[i + baseColors.Length] = baseColors[i].Desaturate(0.2f).Desaturate(0.2f).Brighten(0.2f);
			colors[i + 2 * baseColors.Length] = baseColors[i].Desaturate(0.2f);
			colors[i + 3 * baseColors.Length] = baseColors[i].Desaturate(0.2f).Desaturate(0.2f).Darken(0.2f);
			colors[i + 4 * baseColors.Length] = baseColors[i].Desaturate(0.2f).Desaturate(0.4f).Darken((i != 0) ? 0.4f : 1f);
		}
		for (int j = 0; j < colors.Length; j++)
		{
			AddColorButton(colors[j]);
		}
	}

	private void AddColorButton(Color color)
	{
		CustomizationColorsMenuColor component = Object.Instantiate(colorTemplate.gameObject, colorsContainer).GetComponent<CustomizationColorsMenuColor>();
		component.gameObject.SetActive(value: true);
		colorButtons.Add(component);
		component.Bind(color);
	}

	private void AddPartButtons(WorkshopItemType part, string prefix)
	{
		if (RagdollPresetPartMetadata.IsEmpty(CustomizationController.instance.activeCustomization.preset.GetPart(part)))
		{
			return;
		}
		RagdollModel model = CustomizationController.instance.activeCustomization.GetModel(part);
		if (!(model == null))
		{
			if (model.mask1)
			{
				AddChannelButton(part, 1, string.Format(prefix, 1));
			}
			if (model.mask2)
			{
				AddChannelButton(part, 2, string.Format(prefix, 2));
			}
			if (model.mask3)
			{
				AddChannelButton(part, 3, string.Format(prefix, 3));
			}
			if (channelButtons.Count > 9)
			{
				title.gameObject.SetActive(value: false);
				buttons.pivot = new Vector2(0f, 0.3f);
			}
			else
			{
				title.gameObject.SetActive(value: true);
				buttons.pivot = new Vector2(0f, 0.4f);
			}
		}
	}

	private void AddChannelButton(WorkshopItemType part, int number, string label)
	{
		CustomizationColorsMenuChannel component = Object.Instantiate(channelTemplate.gameObject, channelsContainer).GetComponent<CustomizationColorsMenuChannel>();
		component.gameObject.SetActive(value: true);
		channelButtons.Add(component);
		component.Bind(part, number, label);
	}

	public void HighlightChannel(CustomizationColorsMenuChannel channel)
	{
	}

	public void SelectChannel(CustomizationColorsMenuChannel channel)
	{
		if (selectedChannel != null)
		{
			if (colorApplied)
			{
				colorApplied = false;
				CustomizationController.instance.SetColor(selectedChannel.part, selectedChannel.number, colorBackup);
			}
			selectedChannel.SetActive(active: false);
		}
		selectedChannel = channel;
		selectedChannel.SetActive(active: true);
		CustomizationController.instance.cameraController.FocusPart(channel.part);
		ShowColorPalette(show: true);
		colorBackup = CustomizationController.instance.GetColor(selectedChannel.part, selectedChannel.number);
		int num = -1;
		float num2 = (transparent.color - colorBackup).sqrMagnitude();
		for (int i = 0; i < colorButtons.Count; i++)
		{
			float num3 = (colorButtons[i].color - colorBackup).sqrMagnitude();
			if (num3 < num2)
			{
				num2 = num3;
				num = i;
			}
		}
		transparent.SetActive(-1 == num);
		for (int j = 0; j < colorButtons.Count; j++)
		{
			colorButtons[j].SetActive(j == num);
		}
		if (num >= 0)
		{
			EventSystem.current.SetSelectedGameObject(colorButtons[num].gameObject);
		}
		else
		{
			EventSystem.current.SetSelectedGameObject(transparent.gameObject);
		}
	}

	public void SelectColor(CustomizationColorsMenuColor color)
	{
		CustomizationController.instance.SetColor(selectedChannel.part, selectedChannel.number, color.color);
		colorApplied = true;
	}

	public void ApplyColor(CustomizationColorsMenuColor color)
	{
		CustomizationController.instance.SetColor(selectedChannel.part, selectedChannel.number, color.color);
		ShowColorPalette(show: false);
		EventSystem.current.SetSelectedGameObject(selectedChannel.gameObject);
		colorApplied = false;
	}

	private void ShowColorPalette(bool show)
	{
		if (showPalette != show)
		{
			showPalette = show;
			colorsPanel.gameObject.SetActive(show);
			selectedChannel.SetActive(show);
		}
	}

	public void LeaveColorPicker()
	{
		if (showPalette && colorApplied)
		{
			CustomizationController.instance.SetColor(selectedChannel.part, selectedChannel.number, colorBackup);
			colorApplied = false;
		}
	}
}
