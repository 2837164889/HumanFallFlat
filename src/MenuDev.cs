using Multiplayer;
using UnityEngine;

public class MenuDev : MonoBehaviour
{
	private MenuTransition statupMenu;

	private void Awake()
	{
		MenuTransition[] array = Object.FindObjectsOfType<MenuTransition>();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].gameObject.activeSelf)
			{
				statupMenu = array[i];
			}
		}
	}

	private void Start()
	{
		Dependencies.Initialize<App>();
		GetComponentInChildren<Camera>().gameObject.SetActive(value: false);
		App.instance.BeginMenuDev();
		if (statupMenu == null)
		{
			statupMenu = MenuSystem.instance.GetMenu<MainMenu>();
		}
		MenuSystem.instance.FadeInForward(statupMenu);
		MenuSystem.instance.EnterMenuInputMode();
		SubtitleManager.instance.Hide();
		MenuSystem.instance.OnShowingMainMenu();
	}
}
