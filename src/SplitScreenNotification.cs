using I2.Loc;
using TMPro;
using UnityEngine;

public class SplitScreenNotification : MonoBehaviour
{
	public TextMeshProUGUI ButtonGlyphText;

	public TextMeshProUGUI SecondPlayerStatusText;

	public TextMeshProUGUI SplitRefreshGlyph;

	public TextMeshProUGUI SplitRefreshText;

	public TextMeshProUGUI DLCAvailableGlyph;

	public TextMeshProUGUI DLCAvailableText;

	private PlayerManager.PlayerStatus currentSecondPlayerStatus;

	private string joinButtonGlyph = string.Empty;

	private string leaveButtonGlyph = string.Empty;

	private const string kYButton = "Y/<size=48><voffset=-.08em>ℇ";

	private void Awake()
	{
		LocalizationManager.OnLocalisation += OnLocalisation;
	}

	private void OnLocalisation()
	{
		UpdateSecondPlayerStatus();
	}

	public void SplitRefreshEnable(bool enable)
	{
	}

	public void DLCAvailableEnable(bool enable)
	{
		DLCAvailableGlyph.transform.parent.transform.gameObject.SetActive(value: false);
	}

	private void SetupSplitRefresh()
	{
		SplitRefreshGlyph.text = "Y/<size=48><voffset=-.08em>ℇ";
		SplitRefreshGlyph.fontSize = 40f;
		SplitRefreshText.rectTransform.anchorMin = new Vector2(0.35f, 0.1f);
		SplitRefreshGlyph.rectTransform.anchorMax = new Vector2(0.4f, 1f);
	}

	private void SetupDLCAvailable()
	{
		DLCAvailableGlyph.text = "Y/<size=48><voffset=-.08em>ℇ";
	}

	private void Start()
	{
		joinButtonGlyph = "℆";
		leaveButtonGlyph = "℆";
		ButtonGlyphText.fontSize = 48f;
		ButtonGlyphText.rectTransform.anchorMax = new Vector2(0.15f, 0.75f);
		SecondPlayerStatusText.rectTransform.anchorMin = new Vector2(0.15f, 0f);
		SecondPlayerStatusText.rectTransform.anchorMax = new Vector2(1f, 1f);
		SetupSplitRefresh();
		SetupDLCAvailable();
		UpdateSecondPlayerStatus();
	}

	private void Update()
	{
		if (currentSecondPlayerStatus != PlayerManager.instance.SecondPlayerStatus)
		{
			currentSecondPlayerStatus = PlayerManager.instance.SecondPlayerStatus;
			UpdateSecondPlayerStatus();
		}
	}

	public void UpdateSecondPlayerStatus()
	{
		ButtonGlyphText.text = GetGlyph();
		switch (currentSecondPlayerStatus)
		{
		case PlayerManager.PlayerStatus.CanJoin:
			SecondPlayerStatusText.text = " " + LocalizationManager.GetTermTranslation("MENU.MAIN/PLAYER2JOIN");
			break;
		case PlayerManager.PlayerStatus.Joining:
			SecondPlayerStatusText.text = " " + LocalizationManager.GetTermTranslation("MENU.MAIN/PLAYER2JOINING");
			break;
		case PlayerManager.PlayerStatus.Joined:
			SecondPlayerStatusText.text = " " + LocalizationManager.GetTermTranslation("MENU.MAIN/PLAYER2JOINED");
			break;
		case PlayerManager.PlayerStatus.CanLeave:
			SecondPlayerStatusText.text = " " + LocalizationManager.GetTermTranslation("MENU.MAIN/PLAYER2LEAVE");
			break;
		case PlayerManager.PlayerStatus.Leaving:
			SecondPlayerStatusText.text = " " + LocalizationManager.GetTermTranslation("MENU.MAIN/PLAYER2LEAVING");
			break;
		case PlayerManager.PlayerStatus.Left:
		case PlayerManager.PlayerStatus.PostLeft:
			SecondPlayerStatusText.text = " " + LocalizationManager.GetTermTranslation("MENU.MAIN/PLAYER2LEFT");
			break;
		case PlayerManager.PlayerStatus.Online:
			SecondPlayerStatusText.text = string.Empty;
			break;
		}
	}

	private string GetGlyph()
	{
		string text = string.Empty;
		switch (currentSecondPlayerStatus)
		{
		case PlayerManager.PlayerStatus.CanJoin:
		case PlayerManager.PlayerStatus.Joining:
			text += joinButtonGlyph;
			break;
		case PlayerManager.PlayerStatus.CanLeave:
		case PlayerManager.PlayerStatus.Leaving:
			text += leaveButtonGlyph;
			break;
		}
		return text;
	}
}
