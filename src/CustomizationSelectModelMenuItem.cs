using HumanAPI;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationSelectModelMenuItem : ListViewItem
{
	public TextMeshProUGUI label;

	public RawImage image;

	public GameObject activeMarker;

	private WorkshopItemMetadata boundData;

	public override void Bind(int index, object data)
	{
		base.Bind(index, data);
		WorkshopItemMetadata workshopItemMetadata = data as WorkshopItemMetadata;
		if (workshopItemMetadata.isModel)
		{
			string name = (data as RagdollModelMetadata).modelPrefab.name;
			label.text = LocalizationManager.GetTermTranslation("SKINS/" + name);
		}
		else
		{
			label.text = workshopItemMetadata.title;
		}
		image.texture = workshopItemMetadata.thumbnailTexture;
		if (boundData != null)
		{
			boundData.ReleaseThumbnailReference();
		}
		boundData = workshopItemMetadata;
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
		base.transform.localScale = ((!active) ? Vector3.one : (Vector3.one * 1.05f));
		if (activeMarker != null)
		{
			activeMarker.SetActive(active);
		}
		GetComponent<MenuButton>().isOn = active;
	}
}
