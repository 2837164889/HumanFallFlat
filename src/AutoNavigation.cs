using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoNavigation : MonoBehaviour
{
	private List<NavigationItem> items = new List<NavigationItem>();

	public NavigationDirection direction;

	public Selectable defaultItem;

	public bool selectLastFocused = true;

	public int groupCount = 1;

	public int itemsPerGroup;

	public bool fixedItemsPerGroup;

	public List<Selectable> ignoreObjects;

	private bool invalid = true;

	private Selectable current;

	private AutoNavigation currentChild;

	public Selectable GetSelectableItem(NavigationItemDirection direction)
	{
		if (groupCount > 1 && (current != null || currentChild != null))
		{
			int num = -1;
			for (int i = 0; i < items.Count; i++)
			{
				if ((current != null && items[i].selectable == current) || (currentChild != null && items[i].subNavigation == currentChild))
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				return null;
			}
			NavigationItem navigationItem = ((this.direction != NavigationDirection.Vertical || direction != NavigationItemDirection.Right) && (this.direction != 0 || direction != NavigationItemDirection.Down)) ? (((this.direction != NavigationDirection.Vertical || direction != NavigationItemDirection.Left) && (this.direction != 0 || direction != 0)) ? (((this.direction != NavigationDirection.Vertical || direction != NavigationItemDirection.Down) && (this.direction != 0 || direction != NavigationItemDirection.Right)) ? items[Mathf.Min(items.Count - 1, (num / itemsPerGroup + 1) * itemsPerGroup - 1)] : items[num / itemsPerGroup * itemsPerGroup]) : items[items.Count - 1 - (items.Count - 1 - num) % itemsPerGroup]) : items[num % itemsPerGroup];
			if (navigationItem.selectable != null)
			{
				return navigationItem.selectable;
			}
			return navigationItem.subNavigation.GetSelectableItem(direction);
		}
		if (currentChild != null)
		{
			return currentChild.GetSelectableItem(direction);
		}
		return current ?? defaultItem ?? GetComponentInChildren<Selectable>();
	}

	private void Rebuild()
	{
		if (current != null && !current.isActiveAndEnabled)
		{
			current = null;
		}
		items.Clear();
		CollectChildrenRecursive(base.transform);
		itemsPerGroup = ((!fixedItemsPerGroup) ? ((items.Count + groupCount - 1) / groupCount) : itemsPerGroup);
		if (items.Count != 0)
		{
			for (int i = 0; i < groupCount; i++)
			{
				int num = i * itemsPerGroup;
				int num2 = Mathf.Min((i + 1) * itemsPerGroup, items.Count);
				for (int j = num + 1; j < num2; j++)
				{
					Link(items[j - 1], items[j]);
				}
				if (num2 - 1 > 0 && num2 - 1 < items.Count)
				{
					Link(items[num2 - 1], items[num]);
				}
			}
			for (int k = 1; k < groupCount; k++)
			{
				int num3 = k * itemsPerGroup;
				int num4 = (k + 1) * itemsPerGroup - 1;
				for (int num5 = num4; num5 >= num3; num5--)
				{
					int index = Mathf.Min(num5, items.Count - 1);
					Link(items[num5 - itemsPerGroup], items[index], (direction == NavigationDirection.Horizontal) ? NavigationDirection.Vertical : NavigationDirection.Horizontal);
				}
			}
		}
		AutoNavigation componentInParent = base.transform.parent.GetComponentInParent<AutoNavigation>();
		if (componentInParent != null)
		{
			componentInParent.Rebuild();
		}
	}

	protected void Link(NavigationItem prev, NavigationItem next)
	{
		Link(prev, next, direction);
	}

	protected void Link(NavigationItem prev, NavigationItem next, NavigationDirection direction)
	{
		if (direction == NavigationDirection.Vertical)
		{
			prev.Bind(NavigationItemDirection.Down, next.GetSelectable(NavigationItemDirection.Down));
			next.Bind(NavigationItemDirection.Up, prev.GetSelectable(NavigationItemDirection.Up));
		}
		else
		{
			prev.Bind(NavigationItemDirection.Right, next.GetSelectable(NavigationItemDirection.Right));
			next.Bind(NavigationItemDirection.Left, prev.GetSelectable(NavigationItemDirection.Left));
		}
	}

	private void CollectChildrenRecursive(Transform root)
	{
		for (int i = 0; i < root.childCount; i++)
		{
			Transform child = root.GetChild(i);
			if (!child.gameObject.activeInHierarchy)
			{
				continue;
			}
			Selectable selectable = child.GetComponent<Selectable>();
			if (ignoreObjects != null && ignoreObjects.Contains(selectable))
			{
				selectable = null;
			}
			if (selectable != null)
			{
				items.Add(new NavigationItem
				{
					selectable = selectable
				});
				continue;
			}
			AutoNavigation component = child.GetComponent<AutoNavigation>();
			if (component != null && component.GetComponentInChildren<Selectable>() != null)
			{
				items.Add(new NavigationItem
				{
					subNavigation = component
				});
			}
			else
			{
				CollectChildrenRecursive(child);
			}
		}
	}

	public void OnTransformChildrenChanged()
	{
		invalid = true;
	}

	public void Invalidate()
	{
		invalid = true;
	}

	private void OnEnable()
	{
		invalid = true;
	}

	public void ClearCurrent()
	{
		current = null;
		currentChild = null;
	}

	private void LateUpdate()
	{
		if (selectLastFocused && EventSystem.current.currentSelectedGameObject != null)
		{
			Selectable component = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
			if (component != null && current != component)
			{
				for (int i = 0; i < items.Count; i++)
				{
					if (items[i].selectable == component)
					{
						current = component;
						currentChild = null;
						invalid = true;
						AutoNavigation componentInParent = base.transform.parent.GetComponentInParent<AutoNavigation>();
						if (componentInParent != null)
						{
							componentInParent.ChildSelected(this);
						}
						break;
					}
				}
			}
		}
		if (invalid)
		{
			Rebuild();
			invalid = false;
		}
	}

	private void ChildSelected(AutoNavigation child)
	{
		currentChild = child;
		current = null;
		invalid = true;
	}

	public void Bind(NavigationItemDirection direction, Selectable next)
	{
		if (items.Count == 0)
		{
			return;
		}
		if ((this.direction == NavigationDirection.Vertical && direction == NavigationItemDirection.Left) || (this.direction == NavigationDirection.Horizontal && direction == NavigationItemDirection.Up))
		{
			for (int i = 0; i < itemsPerGroup; i++)
			{
				if (i < items.Count)
				{
					items[i].Bind(direction, next);
				}
			}
		}
		else if ((this.direction == NavigationDirection.Vertical && direction == NavigationItemDirection.Right) || (this.direction == NavigationDirection.Horizontal && direction == NavigationItemDirection.Down))
		{
			for (int j = items.Count - itemsPerGroup; j < items.Count; j++)
			{
				if (j >= 0)
				{
					items[j].Bind(direction, next);
				}
			}
		}
		else if ((this.direction == NavigationDirection.Vertical && direction == NavigationItemDirection.Up) || (this.direction == NavigationDirection.Horizontal && direction == NavigationItemDirection.Left))
		{
			for (int k = 0; k < groupCount; k++)
			{
				if (k * itemsPerGroup < items.Count)
				{
					items[k * itemsPerGroup].Bind(direction, next);
				}
			}
		}
		else
		{
			for (int l = 0; l < groupCount; l++)
			{
				items[Mathf.Min(items.Count - 1, (l + 1) * itemsPerGroup - 1)].Bind(direction, next);
			}
		}
	}
}
