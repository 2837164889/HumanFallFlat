using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuSlider : Slider
{
	private RectTransform hitRect;

	protected override void OnEnable()
	{
		base.OnEnable();
		hitRect = base.fillRect.parent.GetComponent<RectTransform>();
	}

	private bool IsRightOfSlider(Vector2 screenPos, Camera camera)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(hitRect, screenPos, camera, out Vector2 localPoint);
		localPoint -= hitRect.rect.position;
		return localPoint.x < 0f;
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		if (IsRightOfSlider(eventData.position, eventData.pressEventCamera))
		{
			IntPtr functionPointer = typeof(Selectable).GetMethod("OnPointerDown").MethodHandle.GetFunctionPointer();
			Action<PointerEventData> action = (Action<PointerEventData>)Activator.CreateInstance(typeof(Action<PointerEventData>), this, functionPointer);
			action(eventData);
		}
		else
		{
			base.OnPointerDown(eventData);
		}
	}

	public override void OnDrag(PointerEventData eventData)
	{
		if (!IsRightOfSlider(eventData.pressPosition, eventData.pressEventCamera))
		{
			base.OnDrag(eventData);
		}
	}
}
