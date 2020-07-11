using HumanAPI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DLCMenu : MenuTransition
{
	public AutoNavigation topPanel;

	public RawImage levelThumbnail;

	public TextMeshProUGUI levelName;

	public GameObject levelInfoPanel;

	public GameObject levelImage;

	public GameObject levelDescription;

	public ListView list;

	public GameObject BackButton;

	public GameObject PageLeftButton;

	public GameObject PageRightButton;

	public Image itemListImage;

	public Color itemListNormal = new Color(0f, 0f, 0f, 49f / 255f);

	public Color itemListError = new Color(1f, 1f, 1f, 64f / 255f);

	private WorkshopMenuItem selectedMenuItem;

	private DLC.DLCBundles levelBundleID;

	private GameObject previousSelectedItem;

	private string selectedPath;

	private WorkshopLevelMetadata boundLevel;

	public static bool InDLCMenu
	{
		get;
		private set;
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (hasFocus)
		{
			boundLevel = null;
			Rebind();
		}
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
		list.onSelect = OnSelect;
		list.onSubmit = OnSubmit;
		list.onPointerClick = OnPointerClick;
		levelInfoPanel.SetActive(value: true);
		levelImage.SetActive(value: true);
		Rebind();
		InDLCMenu = true;
	}

	public override void OnLostFocus()
	{
		base.OnLostFocus();
		list.Clear();
		if (boundLevel != null)
		{
			boundLevel.ReleaseThumbnailReference();
			boundLevel = null;
		}
		InDLCMenu = false;
	}

	private void SetupAcquiredDots(bool[] acquired, int count)
	{
		for (int i = 0; i < count; i++)
		{
			GameObject button = list.GetButton(i);
			button.GetComponentInChildren<FindDot>().dot.enabled = acquired[i];
		}
	}

	public void Rebind()
	{
		List<WorkshopLevelMetadata> list = null;
		List<WorkshopLevelMetadata> list2 = new List<WorkshopLevelMetadata>();
		WorkshopRepository.instance.LoadBuiltinLevels();
		list = WorkshopRepository.instance.levelRepo.BySource(WorkshopItemSource.BuiltIn);
		int count = list.Count;
		int num = -1;
		int num2 = 0;
		bool[] array = new bool[list.Count];
		for (int i = 0; i < count; i++)
		{
			DLC.DLCBundles dLCBundles = DLC.instance.LevelIsDLC(list[i].workshopId);
			if (dLCBundles != 0)
			{
				list2.Add(list[i]);
				array[num2] = DLC.instance.BundleActive(dLCBundles);
				if (num == -1 && !array[num2])
				{
					num = num2;
				}
				num2++;
			}
		}
		itemListImage.color = itemListNormal;
		this.list.Bind(list2);
		int index = (num != -1) ? num : 0;
		this.list.FocusItem(index);
		SetupAcquiredDots(array, num2);
		PageLeftButton.SetActive(this.list.isCarousel);
		PageRightButton.SetActive(this.list.isCarousel);
	}

	public void PageRight()
	{
		list.PageUp();
	}

	public void PageLeft()
	{
		list.PageDown();
	}

	private void ClearSelectedItem()
	{
		boundLevel = null;
		selectedMenuItem = null;
	}

	public override void OnTansitionedIn()
	{
		base.OnTansitionedIn();
		MenuSystem.instance.FocusOnMouseOver(enable: false);
	}

	public override void ApplyMenuEffects()
	{
		MenuCameraEffects.FadeInPauseMenu();
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionBack<MainMenu>();
			selectedMenuItem = null;
		}
	}

	public override void OnBack()
	{
		BackClick();
	}

	private void OnPointerClick(ListViewItem item, int clickCount, PointerEventData.InputButton button)
	{
		if (clickCount > 1)
		{
			FindMore();
		}
	}

	private void OnSubmit(ListViewItem item)
	{
		if (MenuSystem.CanInvoke)
		{
			FindMore();
		}
	}

	private void BindLevelIfNeeded(WorkshopMenuItem item)
	{
		if (!(item == null))
		{
			WorkshopLevelMetadata level = item.level;
			if (boundLevel == null || level.folder != boundLevel.folder)
			{
				BindLevel(level);
			}
		}
	}

	private void OnSelect(ListViewItem item)
	{
		WorkshopMenuItem workshopMenuItem = item as WorkshopMenuItem;
		if (selectedMenuItem != null)
		{
			selectedMenuItem.SetActive(active: false);
		}
		selectedMenuItem = workshopMenuItem;
		selectedMenuItem.SetActive(active: true);
		BindLevelIfNeeded(selectedMenuItem);
		bool flag = true;
		if (DLC.instance.SupportsDLC())
		{
			levelBundleID = DLC.instance.LevelIsDLC(workshopMenuItem.level.title);
			if (levelBundleID != 0)
			{
				flag = DLC.instance.BundleActive(levelBundleID);
			}
		}
	}

	public void FindMore()
	{
		DLC.instance.OpenStorePage(levelBundleID);
		Debug.Log("Show store page: " + levelBundleID);
	}

	public void FindMoreClick()
	{
		if (MenuSystem.CanInvoke)
		{
			FindMore();
		}
	}

	public void BindLevel(WorkshopLevelMetadata level)
	{
		if (boundLevel != null)
		{
			boundLevel.ReleaseThumbnailReference();
		}
		boundLevel = level;
		if (boundLevel != null)
		{
			selectedPath = boundLevel.folder;
			BindImage(boundLevel.thumbnailTexture);
			levelName.text = boundLevel.title;
		}
		else
		{
			selectedPath = null;
			BindImage(null);
			levelName.text = "MISSING";
		}
	}

	private void BindImage(Texture2D image)
	{
		if (image != null)
		{
			Rect rect = levelThumbnail.rectTransform.rect;
			float num = 1f;
			float num2 = 1f;
			if ((float)image.width / rect.width > (float)image.height / rect.height)
			{
				num = (float)image.height / rect.height / ((float)image.width / rect.width);
			}
			else
			{
				num2 = (float)image.width / rect.width / ((float)image.height / rect.height);
			}
			levelThumbnail.uvRect = new Rect(0.5f - num / 2f, 0.5f - num2 / 2f, num, num2);
		}
		levelThumbnail.texture = image;
	}

	protected override void Update()
	{
		base.Update();
		if (DLC.instance.SupportsDLC() && DLC.instance.Poll())
		{
			Rebind();
		}
	}
}
