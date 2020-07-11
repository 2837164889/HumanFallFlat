using UnityEngine;
using UnityEngine.EventSystems;

public class ListViewItem : MonoBehaviour, IScrollHandler, ISubmitHandler, ISelectHandler, IDeselectHandler, IPointerClickHandler, IPointerEnterHandler, IEventSystemHandler
{
	public int index;

	public object data;

	public WorkshopItemSource type;

	public virtual void Bind(int index, object data)
	{
		this.index = index;
		this.data = data;
	}

	public void OnPointerClick(PointerEventData pointerEventData)
	{
		GetComponentInParent<ListView>().OnPointerClick(this, pointerEventData.clickCount, pointerEventData.button);
	}

	public virtual void OnPointerEnter(PointerEventData pointerEventData)
	{
		if (MenuSystem.instance.GetCurrentInputDevice() == MenuSystem.eInputDeviceType.Mouse)
		{
			GetComponentInParent<ListView>().OnSelect(this);
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		if (MenuSystem.instance.GetCurrentInputDevice() != 0)
		{
			GetComponentInParent<ListView>().OnSelect(this);
		}
	}

	public void OnSubmit(BaseEventData eventData)
	{
		GetComponentInParent<ListView>().OnSubmit(this);
	}

	public void OnScroll(PointerEventData eventData)
	{
		GetComponentInParent<ListView>().OnScroll(eventData);
	}

	public void OnDeselect(BaseEventData eventData)
	{
		if (!(this == null))
		{
			ListView componentInParent = GetComponentInParent<ListView>();
			if ((bool)componentInParent)
			{
				componentInParent.OnDeSelect(this);
			}
		}
	}
}
