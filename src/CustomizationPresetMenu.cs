using HumanAPI;
using InControl;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomizationPresetMenu : MenuTransition
{
	public static CustomizationPresetMenuMode mode;

	public static bool deleteSelectedOnLoad;

	public ListView list;

	public UnityEngine.UI.Button loadButton;

	public UnityEngine.UI.Button saveButton;

	public UnityEngine.UI.Button deleteButton;

	public GameObject showMyButton;

	public GameObject showSubscribedButton;

	public GameObject openWorkshopButton;

	public GameObject loadMyTitle;

	public GameObject loadSubscribedTitle;

	public GameObject saveTitle;

	public GameObject noSubscriptionsMessage;

	public GameObject offlineMessage;

	public GameObject noPresetsMessage;

	public static bool dontReload;

	public bool showSubscribed;

	public GameObject PageLeftButton;

	public GameObject PageRightButton;

	private RagdollPresetMetadata selectedItem;

	private List<RagdollPresetMetadata> items;

	public const int kMaxPresetSlots = 128;

	private CustomizationPresetMenuItem selectedMenuItem;

	public static bool InCustomizationPresetMenu
	{
		get;
		private set;
	}

	public override void OnTansitionedIn()
	{
		base.OnTansitionedIn();
		MenuSystem.instance.FocusOnMouseOver(enable: false);
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (hasFocus)
		{
			OnGotFocus();
		}
	}

	private bool SubscribedPresetsExist()
	{
		WorkshopRepository.instance.ReloadSubscriptions();
		if (WorkshopRepository.instance.presetRepo.BySource(WorkshopItemSource.Subscription).Count > 0)
		{
			return true;
		}
		return false;
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
		if (deleteSelectedOnLoad)
		{
			deleteSelectedOnLoad = false;
			CustomizationController.instance.DeletePreset(selectedItem);
			dontReload = true;
			int num = items.IndexOf(selectedItem);
			items.Remove(selectedItem);
			if (num >= items.Count)
			{
				num = items.Count - 1;
			}
			if (items.Count > 0)
			{
				selectedItem = items[num];
			}
			else
			{
				selectedItem = null;
			}
			if (mode == CustomizationPresetMenuMode.Save && items.Count < 128 && (items.Count < 1 || items[items.Count - 1] != null))
			{
				items.Add(null);
			}
		}
		bool flag = true;
		if (!flag && showSubscribed)
		{
			showSubscribed = false;
		}
		showSubscribedButton.SetActive(mode == CustomizationPresetMenuMode.Load && !showSubscribed && flag);
		loadMyTitle.SetActive(mode == CustomizationPresetMenuMode.Load && !showSubscribed);
		bool flag2 = mode == CustomizationPresetMenuMode.Load && showSubscribed;
		showMyButton.SetActive(flag2);
		loadSubscribedTitle.SetActive(flag2);
		openWorkshopButton.SetActive(flag2);
		InCustomizationPresetMenu = flag2;
		saveTitle.SetActive(mode != CustomizationPresetMenuMode.Load);
		noSubscriptionsMessage.SetActive(value: false);
		offlineMessage.SetActive(value: false);
		noPresetsMessage.SetActive(value: false);
		list.onSelect = OnSelect;
		list.onPointerClick = OnPointerClick;
		list.onSubmit = OnSubmit;
		bool flag3 = false;
		flag3 = SteamUser.BLoggedOn();
		if (!dontReload)
		{
			if (mode == CustomizationPresetMenuMode.Load)
			{
				WorkshopRepository.instance.ReloadLocalPresets();
				if (!WorkshopRepository.instance.ReloadSubscriptions())
				{
				}
				items = new List<RagdollPresetMetadata>();
				if (showSubscribed)
				{
					if (flag3)
					{
						items.AddRange(WorkshopRepository.instance.presetRepo.BySource(WorkshopItemSource.Subscription));
					}
				}
				else
				{
					items.AddRange(WorkshopRepository.instance.presetRepo.BySource(WorkshopItemSource.LocalWorkshop));
				}
			}
			else
			{
				WorkshopRepository.instance.ReloadLocalPresets();
				WorkshopRepository.instance.ReloadSubscriptions();
				items = new List<RagdollPresetMetadata>();
				items.AddRange(WorkshopRepository.instance.presetRepo.BySource(WorkshopItemSource.LocalWorkshop));
				if (items.Count < 128)
				{
					items.Add(null);
				}
			}
			selectedItem = null;
			string skinPresetReference = CustomizationController.instance.GetSkinPresetReference();
			for (int i = 0; i < items.Count; i++)
			{
				if (items[i] != null && items[i].folder.Equals(skinPresetReference))
				{
					selectedItem = items[i];
				}
			}
		}
		CustomizationController.instance.cameraController.FocusCharacterModel();
		dontReload = false;
		if (items.Count == 0)
		{
			list.Bind(items);
			if (!flag3)
			{
				offlineMessage.SetActive(value: true);
			}
			else if (showSubscribed)
			{
				noSubscriptionsMessage.SetActive(value: true);
			}
			else
			{
				noPresetsMessage.SetActive(value: true);
			}
			BindButtons();
			EventSystem.current.SetSelectedGameObject(GetComponentInChildren<Selectable>().gameObject);
		}
		else
		{
			list.Bind(items);
			int num2 = items.IndexOf(selectedItem);
			if (num2 < 0)
			{
				num2 = 0;
			}
			list.FocusItem(num2);
		}
		PageLeftButton.SetActive(list.isCarousel);
		PageRightButton.SetActive(list.isCarousel);
	}

	private IEnumerator TryAgainShortly()
	{
		yield return new WaitForSeconds(4f);
		OnGotFocus();
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
		InCustomizationPresetMenu = false;
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
	}

	private void OnSelect(ListViewItem item)
	{
		CustomizationPresetMenuItem customizationPresetMenuItem = item as CustomizationPresetMenuItem;
		if (selectedMenuItem != null)
		{
			selectedMenuItem.SetActive(active: false);
		}
		selectedMenuItem = customizationPresetMenuItem;
		selectedMenuItem.SetActive(active: true);
		selectedItem = (item.data as RagdollPresetMetadata);
		if (mode == CustomizationPresetMenuMode.Load)
		{
			CustomizationController.instance.LoadPreset(selectedItem);
		}
		BindButtons();
	}

	private void BindButtons()
	{
		loadButton.gameObject.SetActive(mode == CustomizationPresetMenuMode.Load && selectedItem != null);
		saveButton.gameObject.SetActive(mode == CustomizationPresetMenuMode.Save);
		deleteButton.gameObject.SetActive(!showSubscribed && selectedItem != null);
	}

	public override void OnBack()
	{
		BackClick();
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionBack<CustomizationRootMenu>();
			if (mode == CustomizationPresetMenuMode.Load)
			{
				CustomizationController.instance.Rollback();
			}
		}
	}

	public void LoadClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionBack<CustomizationRootMenu>();
			CustomizationController.instance.ApplyLoadedPreset();
		}
	}

	public void SaveClick()
	{
		if (MenuSystem.CanInvoke)
		{
			CustomizationPresetEditMenu.targetPreset = selectedItem;
			TransitionForward<CustomizationPresetEditMenu>();
		}
	}

	public void DeleteClick()
	{
		if (MenuSystem.CanInvoke && selectedItem != null)
		{
			TransitionForward<CustomizationConfirmDeleteMenu>();
		}
	}

	public void ShowMyClick()
	{
		showSubscribed = false;
		TransitionForward<CustomizationPresetMenu>();
	}

	public void ShowSubscribedClick()
	{
		showSubscribed = true;
		TransitionForward<CustomizationPresetMenu>();
	}

	public void OpenWorkshop()
	{
		if (MenuSystem.CanInvoke)
		{
			WorkshopUpload.ShowSkinWorkshop();
		}
	}

	protected override void Update()
	{
		base.Update();
		if ((InputManager.ActiveDevice.Action4.IsPressed && InputManager.ActiveDevice.Action4.HasChanged) || Input.GetKeyDown(KeyCode.Y))
		{
			OnGotFocus();
		}
	}
}
