using I2.Loc;
using Multiplayer;
using UnityEngine;

public class TutorialRepositoryItem : MonoBehaviour
{
	[Multiline]
	public string tutorialText;

	[Multiline]
	public string controllerText;

	public string term;

	public string keyboardTerm;

	public int level;

	public int checkpoint;

	public bool showInPause = true;

	public string GetText()
	{
		bool flag = false;
		for (int i = 0; i < NetGame.instance.players.Count; i++)
		{
			flag |= !NetGame.instance.players[i].controls.mouseControl;
		}
		if (flag && !string.IsNullOrEmpty(term))
		{
			return ScriptLocalization.Get(term);
		}
		if (!flag && !string.IsNullOrEmpty(keyboardTerm))
		{
			return ScriptLocalization.Get(keyboardTerm);
		}
		return null;
	}
}
