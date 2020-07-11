using HumanAPI;
using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizationPaintMenu : MenuTransition
{
	private enum PaintMode
	{
		Blocked,
		Paint,
		Palette,
		Picker
	}

	public Transform maskPanel;

	public Transform channelsContainer;

	public CustomizationColorsMenuChannel channelTemplate;

	public MenuButton maskButton;

	public GameObject maskMarker;

	public MenuButton backfacesButton;

	public GameObject backfacesMarker;

	public float cursorSize = 0.05f;

	public float cursorHardness = 1f;

	public Color color = Color.white;

	public PaintCursor cursor;

	public PaintPicker picker;

	public ColorWheel colorPalette;

	private CustomizationCameraController cameraController;

	public static bool dontReload;

	public bool isActive;

	public PaintTool paint;

	public static bool sJustTransitioned;

	private bool skipPaintingUntilMouseUp;

	private bool maskVisible;

	private bool backfaces;

	private List<CustomizationColorsMenuChannel> channelButtons = new List<CustomizationColorsMenuChannel>();

	public float cursorKernel => Mathf.Lerp(0f, cursorSize, cursorHardness);

	public float cursorFalloff => Mathf.Lerp(cursorSize, 0.001f, cursorHardness);

	public override void OnGotFocus()
	{
		base.OnGotFocus();
		CustomizationController.instance.cameraController.navigationEnabled = false;
		isActive = true;
		if (dontReload)
		{
			dontReload = false;
			return;
		}
		RagdollCustomization activeCustomization = CustomizationController.instance.activeCustomization;
		cameraController = CustomizationController.instance.cameraController;
		paint = GetComponent<PaintTool>();
		paint.Initialize(activeCustomization);
		cursor.Initialize();
		picker.Initialize();
		cursor.color = (paint.color = (colorPalette.color = color));
		cursor.cursorKernel = (paint.cursorKernel = cursorKernel);
		cursor.cursorFalloff = (paint.cursorFalloff = cursorFalloff);
		maskVisible = false;
		maskPanel.gameObject.SetActive(maskVisible);
		ShowMask(show: true);
	}

	private void ShowMask(bool show)
	{
		ShowMask(WorkshopItemType.ModelFull, show);
		ShowMask(WorkshopItemType.ModelHead, show);
		ShowMask(WorkshopItemType.ModelUpperBody, show);
		ShowMask(WorkshopItemType.ModelLowerBody, show);
	}

	private void ShowMask(WorkshopItemType part, bool show)
	{
		RagdollModel model = CustomizationController.instance.activeCustomization.GetModel(part);
		if (!(model == null))
		{
			model.ShowMask(show);
			if (show)
			{
				model.SetMask(paint.GetMask(part));
			}
		}
	}

	public override void OnLostFocus()
	{
		base.OnLostFocus();
		CustomizationController.instance.cameraController.navigationEnabled = true;
	}

	public void Teardown()
	{
		ShowMask(show: false);
		cursor.Process(show: false);
		picker.Process(show: false, pick: false);
		paint.Teardown();
		for (int i = 0; i < channelButtons.Count; i++)
		{
			Object.Destroy(channelButtons[i].gameObject);
		}
		channelButtons.Clear();
	}

	protected override void Update()
	{
		base.Update();
		if (!isActive)
		{
			return;
		}
		if (!cameraController.alt)
		{
			Vector2 mouseScrollDelta = Input.mouseScrollDelta;
			float y = mouseScrollDelta.y;
			if (y != 0f)
			{
				float num = Mathf.Pow(1.1f, 0f - y);
				if (cameraController.ctrl)
				{
					cursorHardness = Mathf.Clamp01(cursorHardness + y * 0.1f);
				}
				else
				{
					cursorSize = Mathf.Clamp(cursorSize * num, 0.01f, 0.2f);
				}
				cursor.cursorKernel = (paint.cursorKernel = cursorKernel);
				cursor.cursorFalloff = (paint.cursorFalloff = cursorFalloff);
			}
		}
		PaintMode paintMode = PaintMode.Paint;
		if (colorPalette.isCursorOver)
		{
			paintMode = PaintMode.Palette;
		}
		else if (cameraController.alt || cameraController.uiLock)
		{
			paintMode = PaintMode.Blocked;
		}
		else if (Input.GetKey(KeyCode.C))
		{
			paintMode = PaintMode.Picker;
		}
		if (!cameraController.lmb)
		{
			skipPaintingUntilMouseUp = false;
		}
		switch (paintMode)
		{
		case PaintMode.Paint:
			if (cameraController.lmb && !skipPaintingUntilMouseUp)
			{
				paint.Paint();
				cursor.Process(show: false);
			}
			else
			{
				paint.EndStroke();
				cursor.Process(show: true);
			}
			picker.Process(show: false, pick: false);
			break;
		case PaintMode.Palette:
			paint.EndStroke();
			cursor.Process(show: false);
			picker.Process(show: true, pick: false);
			break;
		case PaintMode.Picker:
			paint.EndStroke();
			cursor.Process(show: false);
			picker.Process(show: true, pick: true);
			break;
		case PaintMode.Blocked:
			paint.EndStroke();
			cursor.Process(show: false);
			picker.Process(show: false, pick: false);
			break;
		}
		if (paintMode == PaintMode.Paint && !paint.inStroke && Game.GetKey(KeyCode.LeftControl) && Game.GetKeyDown(KeyCode.Z))
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				paint.Redo();
			}
			else
			{
				paint.Undo();
			}
		}
		else if (paintMode == PaintMode.Paint && !paint.inStroke && Game.GetKey(KeyCode.LeftControl) && Game.GetKeyDown(KeyCode.Y))
		{
			paint.Redo();
		}
	}

	public void PreviewColor(Color color)
	{
		if (Game.GetKeyDown(KeyCode.S))
		{
			PickColor(color);
		}
		else
		{
			picker.color = color;
		}
	}

	public void PickColor(Color color)
	{
		picker.color = (cursor.color = (paint.color = (colorPalette.color = color)));
	}

	public override void OnBack()
	{
		BackClick();
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			if (paint.inStroke)
			{
				skipPaintingUntilMouseUp = true;
				paint.CancelStroke();
			}
			else if (paint.hasChanges)
			{
				TransitionForward<CustomizationConfirmDiscardMenu>();
				isActive = false;
			}
			else
			{
				TransitionBack<CustomizationEditMenu>();
				isActive = false;
				CustomizationController.instance.Rollback();
				Teardown();
			}
			SetupMaskButton(visible: false);
		}
	}

	public void ConfirmDiscard()
	{
		CustomizationController.instance.Rollback();
		Teardown();
	}

	public void SaveClick()
	{
		if (MenuSystem.CanInvoke)
		{
			StartCoroutine(Save());
		}
	}

	private IEnumerator Save()
	{
		SubtitleManager.instance.SetProgress(ScriptLocalization.TUTORIAL.SAVING, 1f, 1f);
		yield return null;
		paint.Commit();
		CustomizationController.instance.ClearColors();
		CustomizationController.instance.CommitParts();
	}

	private void SetupMaskButton(bool visible)
	{
		maskButton.isOn = visible;
		maskButton.transform.localScale = ((!visible) ? Vector3.one : (Vector3.one * 1.05f));
		maskMarker.SetActive(visible);
	}

	public void MaskClick()
	{
		if (!MenuSystem.CanInvoke)
		{
			return;
		}
		maskVisible = !maskVisible;
		maskPanel.gameObject.SetActive(maskVisible);
		SetupMaskButton(maskVisible);
		if (maskVisible && channelButtons.Count == 0)
		{
			AddPartButtons(WorkshopItemType.ModelFull, ScriptLocalization.CUSTOMIZATION.PartBody);
			AddPartButtons(WorkshopItemType.ModelHead, ScriptLocalization.CUSTOMIZATION.PartHead);
			AddPartButtons(WorkshopItemType.ModelUpperBody, ScriptLocalization.CUSTOMIZATION.PartUpper);
			AddPartButtons(WorkshopItemType.ModelLowerBody, ScriptLocalization.CUSTOMIZATION.PartLower);
		}
		if (sJustTransitioned)
		{
			for (int i = 0; i < channelButtons.Count; i++)
			{
				channelButtons[i].SetActive(active: true);
			}
			sJustTransitioned = false;
		}
		maskPanel.GetComponent<AutoNavigation>().ClearCurrent();
	}

	public void BackfacesClick()
	{
		if (MenuSystem.CanInvoke)
		{
			backfaces = !backfaces;
			backfacesButton.isOn = backfaces;
			backfacesButton.transform.localScale = ((!backfaces) ? Vector3.one : (Vector3.one * 1.05f));
			backfacesMarker.SetActive(backfaces);
			paint.paintBackface = backfaces;
		}
	}

	public void WebcamClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionForward<CustomizationWebcamMenu>();
			isActive = false;
		}
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
		}
	}

	private void AddChannelButton(WorkshopItemType part, int number, string label)
	{
		CustomizationColorsMenuChannel component = Object.Instantiate(channelTemplate.gameObject, channelsContainer).GetComponent<CustomizationColorsMenuChannel>();
		component.gameObject.SetActive(value: true);
		channelButtons.Add(component);
		component.Bind(part, number, label);
	}

	public void MaskButtonToggle(CustomizationColorsMenuChannel channel)
	{
		channel.SetActive(!channel.GetActive());
		paint.SetMask(channel.part, channel.number, channel.GetActive());
	}
}
