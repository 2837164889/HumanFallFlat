using HumanAPI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomizationSelectModelMenu : MenuTransition
{
	public Transform xmasConfig;

	public ListView list;

	public GameObject characterTitle;

	public GameObject headTitle;

	public GameObject upperBodyTitle;

	public GameObject lowerBodyTitle;

	public static WorkshopItemType part;

	public GameObject PageLeftButton;

	public GameObject PageRightButton;

	private RagdollPresetPartMetadata partBackup;

	private RagdollModelMetadata selectedItem;

	private CustomizationSelectModelMenuItem selectedMenuItem;

	private bool MatchesPart(RagdollPresetPartMetadata partRef, WorkshopItemMetadata part)
	{
		if (RagdollPresetPartMetadata.IsEmpty(partRef))
		{
			return RagdollPresetPartMetadata.IsEmpty(part.folder);
		}
		return partRef.modelPath == part.folder;
	}

	private void Start()
	{
		Vector3 localScale = PageLeftButton.transform.localScale;
		localScale.x *= -1f;
		PageLeftButton.transform.localScale = localScale;
	}

	public override void OnGotFocus()
	{
		base.OnGotFocus();
		CustomizationController.instance.cameraController.FocusPart(part);
		characterTitle.SetActive(part == WorkshopItemType.ModelFull);
		headTitle.SetActive(part == WorkshopItemType.ModelHead);
		upperBodyTitle.SetActive(part == WorkshopItemType.ModelUpperBody);
		lowerBodyTitle.SetActive(part == WorkshopItemType.ModelLowerBody);
		list.onSelect = OnSelect;
		list.onPointerClick = OnPointerClick;
		list.onSubmit = OnSubmit;
		selectedItem = null;
		partBackup = CustomizationController.instance.GetPart(part);
		RagdollModelMetadata ragdollModelMetadata = null;
		List<RagdollModelMetadata> partRepository = WorkshopRepository.instance.GetPartRepository(part);
		for (int i = 0; i < partRepository.Count; i++)
		{
			if (MatchesPart(partBackup, partRepository[i]))
			{
				selectedItem = partRepository[i];
			}
			if (RagdollPresetPartMetadata.IsEmpty(partRepository[i].folder))
			{
				ragdollModelMetadata = partRepository[i];
			}
		}
		if (selectedItem == null)
		{
			selectedItem = ragdollModelMetadata;
		}
		int num = partRepository.IndexOf(selectedItem);
		list.Bind(partRepository);
		if (num >= 0)
		{
			list.FocusItem(num);
		}
		PageLeftButton.SetActive(list.isCarousel);
		PageRightButton.SetActive(list.isCarousel);
	}

	public void PageRight()
	{
		list.PageUp();
	}

	public void PageLeft()
	{
		list.PageDown();
	}

	public override void OnLostFocus()
	{
		base.OnLostFocus();
		list.Clear();
	}

	private void OnPointerClick(ListViewItem item, int clickCount, PointerEventData.InputButton button)
	{
		if (button == PointerEventData.InputButton.Left)
		{
			OnSelect(item);
		}
	}

	private void OnSubmit(ListViewItem item)
	{
		OnSelect(item);
		ApplyClick();
	}

	private void OnSelect(ListViewItem item)
	{
		CustomizationSelectModelMenuItem customizationSelectModelMenuItem = item as CustomizationSelectModelMenuItem;
		if (selectedMenuItem != null)
		{
			selectedMenuItem.SetActive(active: false);
		}
		selectedMenuItem = customizationSelectModelMenuItem;
		selectedMenuItem.SetActive(active: true);
		RagdollModelMetadata ragdollModelMetadata = item.data as RagdollModelMetadata;
		if (selectedItem == null || !ragdollModelMetadata.folder.Equals(selectedItem.folder))
		{
			selectedItem = ragdollModelMetadata;
			if (RagdollPresetPartMetadata.IsEmpty(selectedItem.folder))
			{
				CustomizationController.instance.SetPart(part, null);
				return;
			}
			RagdollPresetPartMetadata ragdollPresetPartMetadata = new RagdollPresetPartMetadata();
			ragdollPresetPartMetadata.modelPath = selectedItem.folder;
			RagdollPresetPartMetadata data = ragdollPresetPartMetadata;
			CustomizationController.instance.SetPart(part, data);
		}
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
			CustomizationController.instance.Rollback();
			TransitionBack<CustomizationEditMenu>();
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
}
