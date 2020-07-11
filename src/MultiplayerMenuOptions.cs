using I2.Loc;
using InControl;
using Multiplayer;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerMenuOptions : MonoBehaviour
{
	private enum KickState
	{
		Idle,
		KickingSomeone,
		RecentlyKicked
	}

	public RectTransform MutePlayer;

	public RectTransform ViewProfile;

	public RectTransform KickPlayer;

	public TextMeshProUGUI MutePlayerText;

	public TextMeshProUGUI ViewProfileText;

	public TextMeshProUGUI KickPlayerText2;

	public Text KickPlayerText;

	public TextMeshProUGUI MutePlayerGlyph;

	public TextMeshProUGUI ViewProfileGlyph;

	public TextMeshProUGUI KickPlayerGlyph;

	[SerializeField]
	private const float KickedMsgViewTime = 2f;

	private GameObject m_selectedButton;

	private UpdatePlayerStatus m_UpdateButtonComponenet;

	private float m_fKickDelayDuration = 2f;

	private float m_fKickedTimeStamp;

	private float m_fKickingProcessTimer;

	private float m_fKickedTimer;

	private KickState m_eKickState;

	private bool m_bIsPlayerGettingKicked;

	private bool m_bPlayerRecentlyKicked;

	private bool m_bCanKickThisPlayer;

	private KickState eKickState
	{
		get
		{
			return m_eKickState;
		}
		set
		{
			m_eKickState = value;
			UpdateKickPlayerText();
		}
	}

	public bool ShouldShow
	{
		get;
		private set;
	}

	private void Awake()
	{
		SetPlayerActionVisibility(flag: false);
	}

	private void Start()
	{
		ChatManager.Instance.ChatListUpdated += EOnChatListUpdated;
		ChatManager.Instance.ChatUserUpdated += EOnChatUserUpdated;
		KickPlayerText2.enabled = false;
		MutePlayer.gameObject.SetActive(value: false);
		ViewProfile.gameObject.SetActive(value: false);
		KickPlayerGlyph.fontSize = 48f;
		KickPlayerGlyph.rectTransform.anchorMax = new Vector2(1f, 1f);
		KickPlayer.anchorMin = new Vector2(0f, 0f);
		KickPlayer.anchorMax = new Vector2(1f, 1f);
		KickPlayer.pivot = new Vector2(0f, 0.5f);
		KickPlayerText.rectTransform.anchorMin = new Vector2(0.1f, 0.15f);
	}

	private void EOnChatListUpdated(object sender, EventArgs e)
	{
		UpdatePlayerActionText();
	}

	private void EOnChatUserUpdated(object sender, EventArgs e)
	{
		UpdatePlayerActionText();
	}

	private void OnEnable()
	{
		UpdatePlayerActionText();
	}

	private bool KickButtonDown()
	{
		if ((InputManager.ActiveDevice.Action4.IsPressed && InputManager.ActiveDevice.Action4.HasChanged) || Input.GetKeyDown(KeyCode.Y))
		{
			return true;
		}
		return false;
	}

	private bool KickButtonUp()
	{
		if ((!InputManager.ActiveDevice.Action4.IsPressed && InputManager.ActiveDevice.Action4.HasChanged) || Input.GetKeyUp(KeyCode.Y))
		{
			return true;
		}
		return false;
	}

	public void OnUpdate()
	{
		if (!m_UpdateButtonComponenet)
		{
			return;
		}
		if (KickButtonDown() && m_bCanKickThisPlayer)
		{
			eKickState = KickState.KickingSomeone;
			m_fKickedTimer = 0f;
		}
		if (KickButtonUp() && eKickState != KickState.RecentlyKicked)
		{
			eKickState = KickState.Idle;
			m_fKickingProcessTimer = 0f;
		}
		if (eKickState == KickState.KickingSomeone && m_bCanKickThisPlayer)
		{
			m_fKickingProcessTimer += Time.deltaTime;
			if (m_fKickingProcessTimer > m_fKickDelayDuration)
			{
				m_fKickingProcessTimer = 0f;
				m_UpdateButtonComponenet.KickUser();
				eKickState = KickState.RecentlyKicked;
			}
		}
		if (eKickState == KickState.RecentlyKicked)
		{
			ChatManager.Instance.Refresh();
			m_fKickedTimer += Time.deltaTime;
			if (m_fKickedTimer > 2f)
			{
				m_fKickedTimer = 0f;
				eKickState = KickState.Idle;
			}
		}
	}

	public void SetPlayerActionVisibility(bool flag)
	{
		ShouldShow = flag;
	}

	public void OnButtonUpdate(GameObject FocusedButton)
	{
		if (m_selectedButton != FocusedButton)
		{
			if ((bool)m_UpdateButtonComponenet)
			{
				m_UpdateButtonComponenet.SetColor(highlighted: false);
			}
			m_fKickingProcessTimer = 0f;
			m_selectedButton = FocusedButton;
			m_UpdateButtonComponenet = ((!(m_selectedButton != null)) ? null : m_selectedButton.GetComponent<UpdatePlayerStatus>());
			if ((bool)m_UpdateButtonComponenet)
			{
				m_UpdateButtonComponenet.SetColor(highlighted: true);
				m_bCanKickThisPlayer = (App.isServer && m_UpdateButtonComponenet.GetButtonUserIndex != 0);
			}
			SetPlayerActionVisibility(m_UpdateButtonComponenet != null);
			UpdatePlayerActionText();
		}
	}

	public void UpdatePlayerActionText()
	{
		UpdateMuteText();
		UpdateKickPlayerText();
		UpdateViewProfileText();
	}

	private void UpdateMuteText()
	{
		if ((bool)m_UpdateButtonComponenet && ChatManager.Instance.GetChatUserList().Count - 1 >= m_UpdateButtonComponenet.GetButtonUserIndex)
		{
			if (!MutePlayer)
			{
				Debug.LogWarning("MUTE TEXT OBJECT MISSING !!");
				return;
			}
			string text = "\u20fd";
			string text2 = (!ChatManager.Instance.GetChatUserList()[m_UpdateButtonComponenet.GetButtonUserIndex].IsMuted) ? "MULTIPLAYER/VOICECHAT/MUTEPLAYER" : "MULTIPLAYER/VOICECHAT/UNMUTEPLAYER";
		}
	}

	private void UpdateKickPlayerText()
	{
		if (!KickPlayer)
		{
			Debug.LogWarning("KICK TEXT OBJECT MISSING!!");
			return;
		}
		string empty = string.Empty;
		empty = "ℇ/Y";
		string text = string.Empty;
		KickPlayerGlyph.SetText((eKickState != KickState.RecentlyKicked) ? empty : string.Empty);
		switch (eKickState)
		{
		case KickState.Idle:
			text = LocalizationManager.GetTermTranslation("MULTIPLAYER/VOICECHAT/KICKPLAYER");
			break;
		case KickState.RecentlyKicked:
			text = string.Format(LocalizationManager.GetTermTranslation("MULTIPLAYER/VOICECHAT/PLAYERKICKEDMP"), ChatManager.sRecentlyKickedPlayerName);
			break;
		case KickState.KickingSomeone:
			text = LocalizationManager.GetTermTranslation("MULTIPLAYER/VOICECHAT/KICKINGPLAYER");
			break;
		}
		bool active = eKickState == KickState.RecentlyKicked || (m_bCanKickThisPlayer && (bool)m_UpdateButtonComponenet);
		KickPlayer.gameObject.SetActive(active);
		KickPlayerText.text = text;
		KickPlayerText2.text = text;
	}

	private void UpdateViewProfileText()
	{
		if (!ViewProfile)
		{
			Debug.LogWarning("PROFILE TEXT OBJECT MISSING!!");
			return;
		}
		string empty = string.Empty;
		empty = "℈";
	}
}
