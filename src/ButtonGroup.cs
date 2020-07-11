using UnityEngine;
using UnityEngine.UI;

public class ButtonGroup : MonoBehaviour
{
	private void OnEnable()
	{
		RebuildLinks();
	}

	public void RebuildLinks(bool makeExplicit = false)
	{
		Selectable[] componentsInChildren = GetComponentsInChildren<Selectable>();
		if (componentsInChildren.Length > 1)
		{
			for (int i = 1; i < componentsInChildren.Length; i++)
			{
				Link(componentsInChildren[i - 1], componentsInChildren[i]);
			}
			Link(componentsInChildren[componentsInChildren.Length - 1], componentsInChildren[0]);
		}
		if (makeExplicit)
		{
			Selectable[] array = componentsInChildren;
			foreach (Selectable selectable in array)
			{
				Navigation navigation = selectable.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				selectable.navigation = navigation;
			}
		}
	}

	private void Link(Selectable above, Selectable below)
	{
		Navigation navigation = above.navigation;
		navigation.selectOnDown = below;
		above.navigation = navigation;
		navigation = below.navigation;
		navigation.selectOnUp = above;
		below.navigation = navigation;
	}
}
