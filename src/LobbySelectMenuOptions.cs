using I2.Loc;
using TMPro;
using UnityEngine;

public class LobbySelectMenuOptions : MonoBehaviour
{
	public GameObject JoinGame;

	public TextMeshProUGUI RefreshGlyph;

	public TextMeshProUGUI JoinGameGlyph;

	public TextMeshProUGUI FriendGlyph;

	public TextMeshProUGUI RefreshText;

	public TextMeshProUGUI JoinGameText;

	public TextMeshProUGUI FriendText;

	private GameObject FriendGame;

	private void Start()
	{
		RefreshGlyph.text = "Y/<size=48><voffset=-0.1em>ℇ";
		RefreshGlyph.fontSize = 40f;
		RefreshText.rectTransform.anchorMin = new Vector2(0.35f, 0.1f);
		RefreshGlyph.rectTransform.anchorMax = new Vector2(0.4f, 1f);
		LocalizationManager.OnLocalisation += Localise;
		Localise();
		JoinGameGlyph.fontSize = 48f;
		JoinGameText.rectTransform.anchorMin = new Vector2(0.5f, 0.1f);
		JoinGameGlyph.rectTransform.anchorMax = new Vector2(0.5f, 0.7f);
		FriendGlyph.text = "F/<size=48><voffset=-0.1em>ℚ";
		FriendGlyph.fontSize = 40f;
		FriendText.rectTransform.anchorMin = new Vector2(0.35f, 0.1f);
		FriendGlyph.rectTransform.anchorMax = new Vector2(0.4f, 0.9f);
		GetFriendGame();
	}

	private void Localise()
	{
		JoinGameGlyph.text = "<size=34>" + ScriptLocalization.Get("MULTIPLAYER/ReturnKey") + "<size=40>/<size=48><voffset=-0.15em>\u20fd</voffset>";
	}

	private void GetFriendGame()
	{
		FriendGame = FriendText.transform.parent.gameObject;
	}

	public void ShowRefreshText(bool show)
	{
		if ((bool)RefreshGlyph)
		{
			RefreshGlyph.enabled = show;
		}
		if ((bool)RefreshText)
		{
			RefreshText.enabled = show;
		}
	}

	public bool JoinTextShown()
	{
		return JoinGame.activeInHierarchy;
	}

	public void ShowJoinText(bool value)
	{
		JoinGame.SetActive(value);
	}

	public bool FriendTextShown()
	{
		return FriendGame.activeInHierarchy;
	}

	public void ShowFriendText(bool value)
	{
		FriendGame.SetActive(value);
	}

	public void SetFriendText(bool mode)
	{
		string term = (!mode) ? "MENU/PLAYERS/FRIENDS" : "MULTIPLAYER/LOBBY.ALL";
		FriendText.text = ScriptLocalization.Get(term);
	}
}
