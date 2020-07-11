using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ListView : MonoBehaviour, IScrollHandler, IEventSystemHandler
{
	public delegate void PointerClickAction<T>(T obj, int clickCount, PointerEventData.InputButton button);

	public Button itemTemplate;

	public GameObject itemContainer;

	private List<Button> buttons = new List<Button>();

	private GameObject newSelectedObject;

	private int targetItemIndex;

	private bool forceSnap;

	public bool disableHoverSelect;

	private float fromAnchor;

	private float toAnchor;

	public float transitionDuration = 0.25f;

	private float transitionPhase = 1f;

	private bool ignoreOnSelect;

	public Action<ListViewItem> onSubmit;

	public Action<ListViewItem> onSelect;

	public Action<ListViewItem> onDeSelect;

	public PointerClickAction<ListViewItem> onPointerClick;

	private RectTransform containerRect;

	private RectTransform scrollRect;

	private float itemSize;

	private int maxItemsOnScreen;

	private int orginalItemsInList;

	[NonSerialized]
	public float itemSpacing;

	private bool isHorizontal;

	[NonSerialized]
	public bool isCarousel;

	private float scrollSpeed = 100f;

	private float anchoredPosition
	{
		get
		{
			float result;
			if (isHorizontal)
			{
				Vector2 anchoredPosition = containerRect.anchoredPosition;
				result = anchoredPosition.x;
			}
			else
			{
				Vector2 anchoredPosition2 = containerRect.anchoredPosition;
				result = 0f - anchoredPosition2.y;
			}
			return result;
		}
	}

	private float spaceAfterItem => (!isHorizontal) ? 0f : ((itemSize + itemSpacing) / 2f);

	public int GetNumberItems => orginalItemsInList;

	public void Bind(IList list)
	{
		Clear();
		orginalItemsInList = list.Count;
		if (orginalItemsInList == 0)
		{
			return;
		}
		GameObject gameObject = null;
		int num = 0;
		int num2;
		if (orginalItemsInList > maxItemsOnScreen)
		{
			num2 = maxItemsOnScreen + orginalItemsInList;
			num = (isHorizontal ? Math.Max(orginalItemsInList - maxItemsOnScreen, 0) : 0);
			isCarousel = true;
		}
		else
		{
			num2 = orginalItemsInList;
			isCarousel = false;
		}
		int[] array = new int[num2];
		for (int i = 0; i < num2; i++)
		{
			array[i] = num;
			num++;
			if (num >= list.Count)
			{
				num = 0;
			}
		}
		for (int j = 0; j < num2; j++)
		{
			gameObject = UnityEngine.Object.Instantiate(itemTemplate.gameObject, itemContainer.transform, worldPositionStays: false);
			buttons.Add(gameObject.GetComponent<Button>());
			ListViewItem component = gameObject.GetComponent<ListViewItem>();
			int index = array[j];
			component.Bind(j, list[index]);
			gameObject.SetActive(value: true);
		}
	}

	public GameObject GetButton(int index)
	{
		return buttons[index].gameObject;
	}

	public void FocusItem(int index)
	{
		if (isCarousel)
		{
			index += maxItemsOnScreen;
			if (index > orginalItemsInList)
			{
				index -= orginalItemsInList;
			}
		}
		FocusItem(buttons[index].GetComponent<ListViewItem>());
	}

	public void FocusItem(ListViewItem item)
	{
		EventSystem.current.SetSelectedGameObject(item.gameObject);
		BringItemToStart(item);
	}

	private void JumpToTarget()
	{
		ListViewItem component = buttons[targetItemIndex].GetComponent<ListViewItem>();
		newSelectedObject = component.gameObject;
		forceSnap = true;
		BringItemToStart(component);
		if (onSelect != null)
		{
			onSelect(component);
		}
	}

	public void PageUp()
	{
		if (isCarousel)
		{
			targetItemIndex += maxItemsOnScreen - 2;
			if (targetItemIndex > orginalItemsInList)
			{
				targetItemIndex -= orginalItemsInList;
			}
			JumpToTarget();
		}
	}

	public void PageDown()
	{
		if (isCarousel)
		{
			targetItemIndex -= maxItemsOnScreen - 2;
			if (targetItemIndex <= 0)
			{
				targetItemIndex += orginalItemsInList;
			}
			JumpToTarget();
		}
	}

	private void CenterItem(ListViewItem item)
	{
		ScrollToOffset((float)item.index * (itemSize + itemSpacing) + itemSize / 2f - ((!isHorizontal) ? scrollRect.rect.height : scrollRect.rect.width) / 2f);
	}

	private void BringItemToStart(ListViewItem item)
	{
		ScrollToOffset((float)item.index * (itemSize + itemSpacing) - spaceAfterItem);
	}

	private void BringItemToEnd(ListViewItem item)
	{
		ScrollToOffset((float)item.index * (itemSize + itemSpacing) + itemSize - ((!isHorizontal) ? scrollRect.rect.height : scrollRect.rect.width) + spaceAfterItem);
	}

	private void ScrollToItem(ListViewItem item)
	{
		RectTransform component = item.GetComponent<RectTransform>();
		int num = item.index;
		if (isHorizontal && isCarousel && (item.index >= maxItemsOnScreen + orginalItemsInList - 1 || item.index <= 0))
		{
			int num2 = (item.index > 0) ? 1 : (-1);
			float num3 = (float)orginalItemsInList * (itemSize + itemSpacing);
			num = item.index - orginalItemsInList * num2;
			RectTransform rectTransform = containerRect;
			Vector2 anchoredPosition = containerRect.anchoredPosition;
			float x = anchoredPosition.x + num3 * (float)num2;
			Vector2 anchoredPosition2 = containerRect.anchoredPosition;
			rectTransform.anchoredPosition = new Vector2(x, anchoredPosition2.y);
			newSelectedObject = buttons[num].gameObject;
		}
		targetItemIndex = num;
		float num4 = (float)num * (itemSize + itemSpacing);
		float num5 = num4 + itemSize;
		num4 -= spaceAfterItem;
		num5 += spaceAfterItem;
		float num6 = (!isHorizontal) ? scrollRect.rect.height : scrollRect.rect.width;
		float num7 = 0f - this.anchoredPosition;
		if (num4 < num7)
		{
			ScrollToOffset(num4);
		}
		if (num5 > num7 + num6)
		{
			ScrollToOffset(num5 - num6);
		}
	}

	private void ScrollToOffset(float offset)
	{
		float num = 0f - spaceAfterItem;
		float num2 = (float)(buttons.Count - 1) * (itemSize + itemSpacing) + itemSize - ((!isHorizontal) ? scrollRect.rect.height : scrollRect.rect.width) + spaceAfterItem;
		if (offset > num2)
		{
			offset = num2;
		}
		if (offset < num)
		{
			offset = num;
		}
		fromAnchor = anchoredPosition;
		toAnchor = 0f - offset;
		transitionPhase = 0f;
	}

	private void Update()
	{
		if (newSelectedObject != null)
		{
			ignoreOnSelect = true;
			EventSystem.current.SetSelectedGameObject(newSelectedObject);
			newSelectedObject = null;
		}
		if (transitionPhase < 1f || forceSnap)
		{
			if (forceSnap)
			{
				transitionPhase = 1f;
				forceSnap = false;
			}
			else
			{
				transitionPhase += Time.deltaTime / transitionDuration;
			}
			if (transitionPhase >= 1f)
			{
				transitionPhase = 1f;
			}
			float t = Ease.easeOutQuad(0f, 1f, transitionPhase);
			if (isHorizontal)
			{
				RectTransform rectTransform = containerRect;
				float x = Mathf.Lerp(fromAnchor, toAnchor, t);
				Vector2 anchoredPosition = containerRect.anchoredPosition;
				rectTransform.anchoredPosition = new Vector2(x, anchoredPosition.y);
			}
			else
			{
				RectTransform rectTransform2 = containerRect;
				Vector2 anchoredPosition2 = containerRect.anchoredPosition;
				rectTransform2.anchoredPosition = new Vector2(anchoredPosition2.x, Mathf.Lerp(0f - fromAnchor, 0f - toAnchor, t));
			}
		}
	}

	public void OnPointerClick(ListViewItem item, int clickCount, PointerEventData.InputButton button)
	{
		if (onPointerClick != null)
		{
			onPointerClick(item, clickCount, button);
		}
	}

	public void OnSubmit(ListViewItem item)
	{
		if (onSubmit != null)
		{
			onSubmit(item);
		}
	}

	public void OnSelect(ListViewItem item)
	{
		if (ignoreOnSelect)
		{
			ignoreOnSelect = false;
			return;
		}
		ScrollToItem(item);
		if (!disableHoverSelect)
		{
			if (onSelect != null)
			{
				onSelect(item);
			}
			if (MenuSystem.instance.GetCurrentInputDevice() == MenuSystem.eInputDeviceType.Mouse)
			{
				EventSystem.current.SetSelectedGameObject(item.gameObject);
			}
		}
	}

	public void OnDeSelect(ListViewItem item)
	{
		if (!(this == null) && onSubmit != null && onDeSelect != null && item != null)
		{
			onDeSelect(item);
		}
	}

	private void Awake()
	{
		scrollRect = GetComponent<RectTransform>();
		containerRect = itemContainer.GetComponent<RectTransform>();
		isHorizontal = (itemContainer.GetComponent<HorizontalOrVerticalLayoutGroup>() is HorizontalLayoutGroup);
		itemSpacing = itemContainer.GetComponent<HorizontalOrVerticalLayoutGroup>().spacing;
		itemSize = ((!isHorizontal) ? itemTemplate.GetComponent<LayoutElement>().preferredHeight : itemTemplate.GetComponent<LayoutElement>().preferredWidth);
		itemTemplate.gameObject.SetActive(value: false);
		if (isHorizontal)
		{
			float num = scrollRect.rect.width / (itemSize + itemSpacing);
			maxItemsOnScreen = (int)Mathf.Floor(num + 0.999f);
			maxItemsOnScreen++;
		}
		else
		{
			maxItemsOnScreen = 0;
		}
	}

	public void Clear()
	{
		for (int i = 0; i < buttons.Count; i++)
		{
			UnityEngine.Object.Destroy(buttons[i].gameObject);
		}
		buttons.Clear();
		isCarousel = false;
	}

	public void OnScroll(PointerEventData eventData)
	{
		float num = 0f - toAnchor;
		Vector2 scrollDelta = eventData.scrollDelta;
		ScrollToOffset(num - scrollDelta.y * scrollSpeed);
	}
}
