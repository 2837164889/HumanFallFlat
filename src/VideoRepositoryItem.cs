using Multiplayer;
using UnityEngine;

public class VideoRepositoryItem : MonoBehaviour
{
	public int level;

	public int checkpoint;

	public string title;

	public bool showMouse;

	public bool showController;

	public bool IsVisible()
	{
		bool flag = false;
		for (int i = 0; i < NetGame.instance.players.Count; i++)
		{
			flag |= !NetGame.instance.players[i].controls.mouseControl;
		}
		if (!flag && showMouse)
		{
			return true;
		}
		if (flag && showController)
		{
			return true;
		}
		return false;
	}

	public void LocalizeTitle(string value)
	{
		title = value;
	}
}
