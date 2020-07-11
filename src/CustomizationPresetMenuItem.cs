using HumanAPI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationPresetMenuItem : ListViewItem
{
	public GameObject normalTemplate;

	public GameObject newSlotTemplate;

	public GameObject label;

	public TextMeshProUGUI newSlotLabel;

	public RawImage image;

	public GameObject activeMarker;

	private RagdollPresetMetadata boundData;

	private bool labelIsUIText;

	private Component labelComponent;

	private void OnEnable()
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

	public override void Bind(int index, object data)
	{
		base.Bind(index, data);
		RagdollPresetMetadata ragdollPresetMetadata = data as RagdollPresetMetadata;
		if (ragdollPresetMetadata != null)
		{
			if (labelComponent == null)
			{
				OnEnable();
			}
			if (labelIsUIText)
			{
				((Text)labelComponent).text = ragdollPresetMetadata.title;
			}
			else
			{
				((TextMeshProUGUI)labelComponent).text = ragdollPresetMetadata.title;
			}
			image.texture = ragdollPresetMetadata.thumbnailTexture;
		}
		normalTemplate.SetActive(ragdollPresetMetadata != null);
		newSlotTemplate.SetActive(ragdollPresetMetadata == null);
		MenuButton component = GetComponent<MenuButton>();
		if (ragdollPresetMetadata == null)
		{
			component.SetLabel(newSlotLabel);
		}
		else if (labelIsUIText)
		{
			component.SetLabel((Text)labelComponent);
		}
		else
		{
			component.SetLabel((TextMeshProUGUI)labelComponent);
		}
		if (boundData != null)
		{
			boundData.ReleaseThumbnailReference();
		}
		boundData = ragdollPresetMetadata;
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
