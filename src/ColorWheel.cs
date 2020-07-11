using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorWheel : MonoBehaviour, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	[Serializable]
	public class ColorEvent : UnityEvent<Color>
	{
	}

	public Color color;

	public Graphic colorBox;

	private Texture2D texture;

	public static bool isInside;

	public static bool isTransparent;

	public ColorEvent onColorPreview;

	public UnityEvent onEndPreview;

	public ColorEvent onColorPick;

	public bool isCursorOver => isInside && !isTransparent;

	private void OnEnable()
	{
		texture = (GetComponent<RawImage>().texture as Texture2D);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isInside = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (isInside)
		{
			isInside = false;
			onEndPreview.Invoke();
		}
	}

	private Color ColorAtScreenPos(Vector2 mousePos)
	{
		Vector2 vector = UICanvas.ScreenPointToLocal(base.transform as RectTransform, mousePos);
		Color pixel = texture.GetPixel(Mathf.Clamp((int)vector.x, 0, 512), Mathf.Clamp((int)vector.y, 0, 512));
		isTransparent = (pixel.a < 0.5f);
		return pixel;
	}

	private void Update()
	{
		if (colorBox != null)
		{
			colorBox.color = color;
		}
		if (!isInside)
		{
			return;
		}
		Color arg = ColorAtScreenPos(Input.mousePosition);
		if (!isTransparent && onColorPick != null)
		{
			if (Input.GetMouseButton(0))
			{
				onColorPick.Invoke(arg);
			}
			onColorPreview.Invoke(arg);
		}
		else
		{
			onEndPreview.Invoke();
		}
	}
}
