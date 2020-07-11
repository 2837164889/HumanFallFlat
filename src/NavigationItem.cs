using UnityEngine.UI;

public class NavigationItem
{
	public Selectable selectable;

	public AutoNavigation subNavigation;

	public Selectable GetSelectable(NavigationItemDirection direction)
	{
		if (selectable != null)
		{
			return selectable;
		}
		return subNavigation.GetSelectableItem(direction);
	}

	public void Bind(NavigationItemDirection direction, Selectable next)
	{
		if (selectable != null)
		{
			Bind(direction, selectable, next);
		}
		else if (subNavigation != null)
		{
			subNavigation.Bind(direction, next);
		}
	}

	private void Bind(NavigationItemDirection direction, Selectable selectable, Selectable next)
	{
		Navigation navigation = selectable.navigation;
		navigation.mode = Navigation.Mode.Explicit;
		switch (direction)
		{
		case NavigationItemDirection.Up:
			navigation.selectOnUp = next;
			break;
		case NavigationItemDirection.Down:
			navigation.selectOnDown = next;
			break;
		case NavigationItemDirection.Left:
			navigation.selectOnLeft = next;
			break;
		case NavigationItemDirection.Right:
			navigation.selectOnRight = next;
			break;
		}
		selectable.navigation = navigation;
	}
}
