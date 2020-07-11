using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomizationColorsMenuColor : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler
{
	public Graphic swatch;

	public GameObject activeMarker;

	public Color color => swatch.color;

	public void SetActive(bool active)
	{
		activeMarker.SetActive(active);
		if (active && color.sqrMagnitude() > 2.5f)
		{
			activeMarker.GetComponent<Graphic>().color = Color.black;
		}
	}

	internal void Bind(Color color)
	{
		swatch.color = color;
		SetActive(active: false);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		EventSystem.current.SetSelectedGameObject(base.gameObject);
	}
}
