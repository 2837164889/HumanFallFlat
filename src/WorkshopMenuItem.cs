using HumanAPI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorkshopMenuItem : ListViewItem
{
	public GameObject label;

	public RawImage image;

	public WorkshopLevelMetadata level;

	public int campaignLevelID = -1;

	public Texture2D noThumbnailImage;

	public GameObject activeMarker;

	public GameObject newLevelStar;

	public WorkshopLevelMetadata boundData;

	private bool labelIsUIText;

	private Component labelComponent;

	private void OnEnable()
	{
		if (!(label == null))
		{
			labelComponent = label.GetComponentInChildren<Text>();
			if (labelComponent != null)
			{
				labelIsUIText = true;
			}
			else
			{
				labelComponent = label.GetComponentInChildren<TextMeshProUGUI>();
			}
		}
	}

	public override void Bind(int index, object data)
	{
		base.Bind(index, data);
		level = (WorkshopLevelMetadata)data;
		if (level == null)
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			base.gameObject.SetActive(value: true);
			if (label != null)
			{
				Type type = label.GetType();
				if (labelIsUIText)
				{
					if (labelComponent != null)
					{
						((Text)labelComponent).text = level.title;
					}
				}
				else if (labelComponent != null)
				{
					((TextMeshProUGUI)labelComponent).text = level.title;
				}
				if (image != null && level != null)
				{
					image.texture = level.thumbnailTexture;
				}
			}
			if (newLevelStar != null)
			{
				bool active = false;
				if (!GameSave.HasSeenLatestLevel() && level.levelType == WorkshopItemSource.EditorPick && level.workshopId == 1)
				{
					active = true;
				}
				newLevelStar.SetActive(active);
			}
		}
		if (boundData != null)
		{
			boundData.ReleaseThumbnailReference();
		}
		boundData = level;
	}

	public void OnDestroy()
	{
		if (boundData != null)
		{
			boundData.ReleaseThumbnailReference();
		}
		boundData = null;
	}

	public void SetActive(bool active)
	{
		if (activeMarker != null)
		{
			activeMarker.SetActive(active);
		}
		GetComponent<MenuButton>().isOn = active;
	}

	public override void OnPointerEnter(PointerEventData pointerEventData)
	{
		base.OnPointerEnter(pointerEventData);
	}
}
