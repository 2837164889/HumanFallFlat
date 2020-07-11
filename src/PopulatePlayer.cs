using I2.Loc;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopulatePlayer : MonoBehaviour
{
	[SerializeField]
	private int MaxPlayerCount = 8;

	private UpdatePlayerStatus[] m_arButtonList;

	private AutoNavigation m_NavigationObject;

	private bool m_bIsGridList;

	public GameObject m_PlayerSlotButton;

	public bool m_DarkBackground;

	private void Awake()
	{
		m_arButtonList = new UpdatePlayerStatus[MaxPlayerCount];
		for (int i = 0; i < MaxPlayerCount; i++)
		{
			AddButton();
		}
		m_arButtonList = base.gameObject.transform.GetComponentsInChildren<UpdatePlayerStatus>(includeInactive: true);
		UpdatePlayerStatus[] arButtonList = m_arButtonList;
		foreach (UpdatePlayerStatus updatePlayerStatus in arButtonList)
		{
			updatePlayerStatus.m_DarkBackground = m_DarkBackground;
		}
		m_NavigationObject = GetComponent<AutoNavigation>();
		m_bIsGridList = GetComponent<GridCellScaler>();
		m_NavigationObject.fixedItemsPerGroup = m_bIsGridList;
		m_NavigationObject.itemsPerGroup = (m_bIsGridList ? 4 : 0);
	}

	private void Start()
	{
		if (ChatManager.Instance == null)
		{
			Debug.LogWarning("Chat Manager Null");
			return;
		}
		ChatManager.Instance.ChatListUpdated += EOnChatListUpdated;
		ChatManager.Instance.ChatUserUpdated += EOnChatUserUpdated;
		UpdateAllPlayerSlots();
	}

	private void UpdateNavigation()
	{
		if (m_bIsGridList && (bool)m_NavigationObject)
		{
			m_NavigationObject.groupCount = ((ChatManager.Instance.GetChatUserCount() <= 4) ? 1 : 2);
		}
		m_NavigationObject.Invalidate();
		if (EventSystem.current.currentSelectedGameObject != null)
		{
			UpdatePlayerStatus component = EventSystem.current.currentSelectedGameObject.GetComponent<UpdatePlayerStatus>();
			Selectable component2 = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
			if ((bool)component && (bool)component2 && !component2.isActiveAndEnabled && component.GetButtonUserIndex >= 1 && component.GetButtonUserIndex <= m_arButtonList.Length)
			{
				EventSystem.current.SetSelectedGameObject(m_arButtonList[component.GetButtonUserIndex - 1].gameObject);
			}
		}
	}

	private void UpdateAllPlayerSlots()
	{
		bool flag = false;
		for (int i = 0; i < MaxPlayerCount; i++)
		{
			if (i < ChatManager.Instance.GetChatUserCount())
			{
				m_arButtonList[i].gameObject.SetActive(value: true);
				UpdatePlayer(i);
				flag = true;
			}
			else
			{
				m_arButtonList[i].gameObject.SetActive(value: false);
			}
		}
		UpdateNavigation();
	}

	private void UpdatePlayer(int PlayerIndex)
	{
		if (PlayerIndex < m_arButtonList.Length && m_arButtonList[PlayerIndex] != null)
		{
			string gamerTag = ChatManager.Instance.GetChatUserList()[PlayerIndex].GamerTag;
			bool isTalking = ChatManager.Instance.GetChatUserList()[PlayerIndex].IsTalking;
			bool isMuted = ChatManager.Instance.GetChatUserList()[PlayerIndex].IsMuted;
			m_arButtonList[PlayerIndex].UpdateStatus(PlayerIndex, (!string.IsNullOrEmpty(gamerTag)) ? gamerTag : LocalizationManager.GetTermTranslation("MULTIPLAYER/VOICECHAT/JOINING"), isTalking, isMuted);
		}
		else
		{
			Debug.LogWarning("PlayerIndex not pointing to a player.");
		}
	}

	private void EOnChatListUpdated(object sender, EventArgs e)
	{
		UpdateAllPlayerSlots();
	}

	private void EOnChatUserUpdated(object sender, EventArgs e)
	{
		ChatUserEventArgs chatUserEventArgs = (ChatUserEventArgs)e;
		if (chatUserEventArgs != null)
		{
			UpdatePlayer(chatUserEventArgs.Index);
		}
		UpdateAllPlayerSlots();
	}

	private void AddButton()
	{
		if (!m_PlayerSlotButton)
		{
			Debug.LogWarning("No Button prefab specified, Dang it !!");
		}
		else
		{
			UnityEngine.Object.Instantiate(m_PlayerSlotButton, base.transform, worldPositionStays: false).SetActive(value: true);
		}
	}
}
