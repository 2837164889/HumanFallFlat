using System;
using UnityEngine;
using UnityEngine.UI;

public class UpdatePlayerStatus : MonoBehaviour
{
	public Sprite IdleIcon;

	public Sprite TalkingIcon;

	public Sprite MutedIcon;

	private int m_iUserIndex = -1;

	public Text PlayerSlotText;

	public Image ChatStatusImage;

	public string test = "DUMMY TEXT";

	[NonSerialized]
	public bool m_DarkBackground;

	public int GetButtonUserIndex => m_iUserIndex;

	public void SetColor(bool highlighted)
	{
		Color color = highlighted ? Color.white : ((!m_DarkBackground) ? Color.black : Color.grey);
		PlayerSlotText.color = color;
		ChatStatusImage.color = color;
	}

	private void Start()
	{
		if (ChatStatusImage != null)
		{
			ChatStatusImage.enabled = false;
		}
		SetColor(highlighted: false);
	}

	private void UpdateButtonText(string NewText)
	{
		if ((bool)PlayerSlotText)
		{
			PlayerSlotText.text = NewText;
		}
	}

	private void UpdateIcon(bool IsTalking, bool IsMuted)
	{
		if ((bool)ChatStatusImage)
		{
			if (IsMuted)
			{
				ChatStatusImage.sprite = MutedIcon;
			}
			else if (IsTalking)
			{
				ChatStatusImage.sprite = TalkingIcon;
			}
			else
			{
				ChatStatusImage.sprite = IdleIcon;
			}
		}
		else
		{
			Debug.LogWarning("TextMesh Not found !!");
		}
	}

	public void UpdateStatus(int UserIndex, string GamerTag, bool IsTalking, bool IsMuted)
	{
		m_iUserIndex = UserIndex;
		UpdateButtonText(GamerTag);
		UpdateIcon(IsTalking, IsMuted);
	}

	public void KickUser()
	{
		if ((m_iUserIndex >= 0) & (m_iUserIndex < ChatManager.Instance.GetChatUserCount()))
		{
			ChatManager.Instance.KickPlayer(m_iUserIndex);
		}
	}

	public void ViewPlayerProfile()
	{
		Debug.Log("View profile : " + m_iUserIndex);
		if ((m_iUserIndex >= 0) & (m_iUserIndex < ChatManager.Instance.GetChatUserCount()))
		{
			ChatManager.Instance.ViewPlayerProfile(m_iUserIndex);
		}
	}

	public void TogglePlayerMute()
	{
		if ((m_iUserIndex >= 0) & (m_iUserIndex < ChatManager.Instance.GetChatUserCount()))
		{
			ChatManager.Instance.ToggleChatUserMuteState(m_iUserIndex, !ChatManager.Instance.GetChatUserList()[m_iUserIndex].IsMuted);
		}
	}
}
