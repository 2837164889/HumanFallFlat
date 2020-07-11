using I2.Loc;
using InControl;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Multiplayer
{
	public class MultiplayerSelectLobbyMenu : MenuTransition
	{
		public enum SelectType
		{
			kNone,
			kSelect,
			kDeselect
		}

		public TextMeshProUGUI titleText;

		public Button hostButton;

		public TextMeshProUGUI hostButtonText;

		public Button hostPrivateButton;

		public TextMeshProUGUI noLobbiesMessage;

		public ListView list;

		public LevelInformationBox levelInformationBox;

		private List<NetTransport.ILobbyEntry> items;

		private LobbySelectMenuOptions lobbySelectMenuOptions;

		private float mRefreshWait;

		private const float kNoRefreshTime = 6.5f;

		private bool showJoinText;

		private bool showFriendText = true;

		private static float sLobbyUpdateTimer;

		private const float kOnlineDelay = 1.5f;

		private MultiplayerSelectLobbyMenuItem selectedMenuItem;

		private ListViewItem previousSelected;

		public static bool InLobbySelectMenu
		{
			get;
			private set;
		}

		private void Awake()
		{
			lobbySelectMenuOptions = Object.FindObjectOfType<LobbySelectMenuOptions>();
		}

		private void Start()
		{
			hostPrivateButton.gameObject.SetActive(value: false);
			hostButtonText.text = ScriptLocalization.Get("MULTIPLAYER/JOIN.HOST");
		}

		private void ToggleFriendlyGame()
		{
			NetGame.friendly = !NetGame.friendly;
			lobbySelectMenuOptions.SetFriendText(NetGame.friendly);
		}

		private void SetJoinFriendButtonState(bool checkFriends = true)
		{
			if (lobbySelectMenuOptions != null && showJoinText != lobbySelectMenuOptions.JoinTextShown())
			{
				lobbySelectMenuOptions.ShowJoinText(showJoinText);
			}
		}

		protected override void Update()
		{
			base.Update();
			if (DialogOverlay.IsOn())
			{
				return;
			}
			if (mRefreshWait <= 0f)
			{
				SetJoinFriendButtonState();
				if ((InputManager.ActiveDevice.Action4.IsPressed && InputManager.ActiveDevice.Action4.HasChanged) || Input.GetKeyDown(KeyCode.Y))
				{
					RefreshClick();
				}
				if ((InputManager.ActiveDevice.RightTrigger.IsPressed && InputManager.ActiveDevice.RightTrigger.HasChanged) || Input.GetKeyDown(KeyCode.F))
				{
					ToggleFriendlyGame();
					RefreshClick();
				}
			}
			else
			{
				SetJoinFriendButtonState(checkFriends: false);
				mRefreshWait -= Time.deltaTime;
				if (mRefreshWait <= 0f && lobbySelectMenuOptions != null)
				{
					lobbySelectMenuOptions.ShowRefreshText(show: true);
					showFriendText = true;
				}
			}
			sLobbyUpdateTimer -= Time.deltaTime;
			if (sLobbyUpdateTimer < 0f)
			{
				if (selectedMenuItem != null && selectedMenuItem.boundData != null)
				{
					NetGame.instance.transport.RequestLobbyDataRefresh(selectedMenuItem.boundData, inSession: false);
				}
				sLobbyUpdateTimer = NetGame.instance.transport.GetLobbyDataRefreshThrottleTime();
			}
		}

		private void SetScreenText()
		{
			titleText.text = ScriptLocalization.Get((!NetGame.friendly) ? "MULTIPLAYER/JOIN.TitlePublic" : "MULTIPLAYER/JOIN.Title");
			noLobbiesMessage.text = ScriptLocalization.Get((!NetGame.friendly) ? "MULTIPLAYER/JOIN.NoServers" : "MULTIPLAYER/JOIN.NoFriends");
		}

		public override void OnGotFocus()
		{
			base.OnGotFocus();
			list.onSelect = OnSelect;
			list.onSubmit = OnSubmit;
			list.onDeSelect = OnDeSelect;
			list.onPointerClick = OnPointerClick;
			NetGame.instance.transport.RegisterForLobbyData(OnLobbyDataUpdate);
			lobbySelectMenuOptions.SetFriendText(NetGame.friendly);
			showFriendText = true;
			lobbySelectMenuOptions.ShowFriendText(value: true);
			showJoinText = false;
			SetScreenText();
			InLobbySelectMenu = true;
			RebindList();
		}

		public override void OnLostFocus()
		{
			base.OnLostFocus();
			InLobbySelectMenu = false;
			NetGame.instance.transport.UnregisterForLobbyData(OnLobbyDataUpdate);
			previousSelected = null;
			list.Clear();
		}

		private IEnumerator DelayedOnlineScreen()
		{
			yield return new WaitForSeconds(1.5f);
			NetGame.instance.transport.ListLobbies(OnLobbiesListed);
		}

		private void RebindList()
		{
			Dialogs.ShowListGamesProgress();
			StartCoroutine(DelayedOnlineScreen());
		}

		private void UpdateLobbies(bool focus = true)
		{
			list.Bind(items);
			if (items.Count == 0)
			{
				selectedMenuItem = null;
				noLobbiesMessage.gameObject.SetActive(value: true);
				if (lobbySelectMenuOptions != null && selectedMenuItem != null)
				{
					showJoinText = selectedMenuItem.IsActive();
				}
				else
				{
					showJoinText = false;
				}
				hostButton.Select();
				LevelInformationBoxEnabled(enabled: false);
			}
			else
			{
				noLobbiesMessage.gameObject.SetActive(value: false);
				if (focus)
				{
					list.FocusItem(0);
					selectedMenuItem = (items[0] as MultiplayerSelectLobbyMenuItem);
					levelInformationBox.UpdateDisplay(selectedMenuItem.boundData);
				}
			}
		}

		private void OnLobbiesListed(List<NetTransport.ILobbyEntry> lobbies)
		{
			Dialogs.HideProgress();
			items = lobbies;
			UpdateLobbies();
		}

		public void RefreshClick()
		{
			if (MenuSystem.CanInvoke)
			{
				SetScreenText();
				RebindList();
			}
		}

		public void HostPublic()
		{
			if (MenuSystem.CanInvoke)
			{
				Game.multiplayerLobbyLevel = Options.multiplayerLobbyLevelStore;
				WorkshopRepository.instance.SetLobbyTitle(Game.multiplayerLobbyLevel);
				NetGame.instance.transport.SetLobbyStatus(status: false);
				RatingMenu.instance.LoadInit();
				Dialogs.HideProgress();
				App.instance.HostGame();
				Physics.autoSimulation = true;
			}
		}

		public void HostPrivate()
		{
		}

		private void OnSubmit(ListViewItem item)
		{
			MultiplayerSelectLobbyMenuItem multiplayerSelectLobbyMenuItem = selectedMenuItem = (item as MultiplayerSelectLobbyMenuItem);
			JoinClick();
		}

		private void OnPointerClick(ListViewItem item, int clickCount, PointerEventData.InputButton button)
		{
			if (clickCount > 1)
			{
				JoinClick();
			}
		}

		private void LevelInformationBoxEnabled(bool enabled)
		{
			levelInformationBox.gameObject.SetActive(enabled);
		}

		private void OnSelect(ListViewItem item)
		{
			MultiplayerSelectLobbyMenuItem multiplayerSelectLobbyMenuItem = selectedMenuItem = (item as MultiplayerSelectLobbyMenuItem);
			selectedMenuItem.SetActive(active: true);
			if (lobbySelectMenuOptions != null && selectedMenuItem != null)
			{
				showJoinText = selectedMenuItem.IsActive();
			}
			else
			{
				showJoinText = false;
			}
			if (!levelInformationBox.gameObject.activeSelf)
			{
				LevelInformationBoxEnabled(enabled: true);
			}
			if (previousSelected != item)
			{
				levelInformationBox.UpdateDisplay(selectedMenuItem.boundData);
				previousSelected = item;
			}
		}

		private void OnDeSelect(ListViewItem item)
		{
			MultiplayerSelectLobbyMenuItem multiplayerSelectLobbyMenuItem = item as MultiplayerSelectLobbyMenuItem;
			if (selectedMenuItem != null)
			{
				selectedMenuItem.SetActive(active: false);
			}
			if (lobbySelectMenuOptions != null && selectedMenuItem != null)
			{
				showJoinText = selectedMenuItem.IsActive();
			}
			else
			{
				showJoinText = false;
			}
		}

		public void JoinClick()
		{
			if (MenuSystem.CanInvoke && selectedMenuItem != null)
			{
				Dialogs.HideProgress();
				if (selectedMenuItem != null && selectedMenuItem.boundData != null)
				{
					App.instance.JoinGame(selectedMenuItem.boundData.lobbyId());
					Physics.autoSimulation = true;
				}
			}
		}

		public void BackClick()
		{
			if (MenuSystem.CanInvoke)
			{
				Dialogs.HideProgress();
				TransitionBack<SelectPlayersMenu>();
			}
		}

		public override void ApplyMenuEffects()
		{
			MenuCameraEffects.FadeInPauseMenu();
		}

		public override void OnBack()
		{
			BackClick();
		}

		private void OnLobbyDataUpdate(object lobbyID, NetTransport.LobbyDisplayInfo dispInfo, bool error)
		{
			if (lobbyID == null)
			{
				return;
			}
			for (int i = 0; i < items.Count; i++)
			{
				if (!items[i].isSameLobbyID(lobbyID))
				{
					continue;
				}
				if (!error)
				{
					items[i].setDisplayInfo(ref dispInfo);
				}
				if (!(selectedMenuItem != null) || !items[i].Equals(selectedMenuItem.boundData))
				{
					continue;
				}
				if (error)
				{
					levelInformationBox.ClearDisplay();
					selectedMenuItem.boundData = null;
					LevelInformationBoxEnabled(enabled: false);
					selectedMenuItem.DataInvalid();
				}
				else if ((dispInfo.FeaturesMask & 0x20) != 0)
				{
					if ((dispInfo.Flags & 0x20) != 0)
					{
						LevelInformationBoxEnabled(enabled: true);
						levelInformationBox.UpdateDisplay(dispInfo);
						selectedMenuItem.DataRefreshed(items[i]);
					}
					else
					{
						levelInformationBox.ClearDisplay();
						LevelInformationBoxEnabled(enabled: false);
						selectedMenuItem.DataInvalid();
					}
				}
				else
				{
					levelInformationBox.UpdateDisplay(dispInfo);
					selectedMenuItem.DataRefreshed(items[i]);
				}
			}
		}
	}
}
