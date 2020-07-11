using HumanAPI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationColorsMenuChannel : MonoBehaviour
{
	public TextMeshProUGUI text;

	public GameObject activeMarker;

	public WorkshopItemType part;

	public int number;

	public string label;

	private ColorBlock defaultColors;

	private void Awake()
	{
		defaultColors = GetComponent<UnityEngine.UI.Button>().colors;
	}

	internal void Bind(WorkshopItemType part, int number, string label)
	{
		this.part = part;
		this.number = number;
		this.label = label;
		text.text = label;
	}

	public bool GetActive()
	{
		return activeMarker.activeSelf;
	}

	public void SetActive(bool active)
	{
		base.transform.localScale = ((!active) ? Vector3.one : (Vector3.one * 1.05f));
		activeMarker.SetActive(active);
		GetComponent<MenuButton>().isOn = active;
	}
}
