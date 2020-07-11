using HumanAPI;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer
{
	public class MultiplayerLobbyMenu : MenuTransition
	{
		public RawImage levelThumbnail;

		public Text levelName;

		public TextMeshProUGUI titleText;

		public TextMeshProUGUI settingsText;

		public TextMeshProUGUI lobbyText;

		public UnityEngine.UI.Button joinButton;

		public UnityEngine.UI.Button inviteButton;

		public UnityEngine.UI.Button startGameButton;

		public UnityEngine.UI.Button selectLevelButton;

		public UnityEngine.UI.Button settingsButton;

		public UnityEngine.UI.Button leaveButton;

		public UnityEngine.UI.Button selectLobbyButton;

		public LevelInformationBox informationBox;

		private MultiplayerMenuOptions m_MenuOptions;

		private WorkshopLevelMetadata level;

		private float lobbyRefreshTimer;

		public override void OnGotFocus()
		{
			titleText.text = ScriptLocalization.Get((!NetGame.isServer) ? "MULTIPLAYER/LOBBY.TitleClient" : ((!NetGame.friendly) ? "MULTIPLAYER/LOBBY.TitleServerPublic" : "MULTIPLAYER/LOBBY.TitleServer"));
			bool flag = NetGame.instance.transport.CanSendInvite();
			inviteButton.gameObject.SetActive(flag);
			selectLevelButton.gameObject.SetActive(NetGame.isServer);
			startGameButton.gameObject.SetActive(NetGame.isServer);
			settingsButton.gameObject.SetActive(NetGame.isServer);
			WorkshopRepository.instance.LoadBuiltinLevels();
			RebindLevel();
			RebindSettings();
			defaultElement = (NetGame.isServer ? startGameButton.gameObject : ((!flag) ? leaveButton.gameObject : inviteButton.gameObject));
			base.OnGotFocus();
			if (NetGame.isServer)
			{
				lobbyText.text = string.Format("{0}\n<size=75%>{1}</size>", ScriptLocalization.Get("WORKSHOP/SelectLobbyLevel"), WorkshopRepository.GetLobbyTitle(App.instance.GetLobbyTitle()));
			}
			selectLobbyButton.gameObject.SetActive(NetGame.isServer);
			informationBox.gameObject.SetActive(!NetGame.isServer);
			NetGame.instance.transport.RegisterForLobbyData(OnLobbyDataUpdate);
		}

		public void RebindSettings()
		{
			if (NetGame.isServer)
			{
				bool friendly = NetGame.friendly;
				friendly = true;
				string str = ScriptLocalization.Get("MULTIPLAYER/LOBBY.SETTINGS") + "\r\n<size=75%>";
				str += string.Format(ScriptLocalization.Get("MULTIPLAYER/LOBBY.MaxPlayers"), Options.lobbyMaxPlayers);
				if (friendly && Options.lobbyInviteOnly > 0)
				{
					str = str + ", " + ScriptLocalization.Get("MULTIPLAYER/LOBBY.InviteOnly");
				}
				if (Options.lobbyJoinInProgress > 0)
				{
					str = str + ", " + ScriptLocalization.Get("MULTIPLAYER/LOBBY.GameInProgress");
				}
				str += "</size>";
				settingsText.text = str;
			}
		}

		public void RebindLevel()
		{
			if (level != null)
			{
				level.ReleaseThumbnailReference();
			}
			switch (NetGame.instance.currentLevelType)
			{
			case WorkshopItemSource.BuiltIn:
				WorkshopRepository.instance.LoadBuiltinLevels();
				break;
			case WorkshopItemSource.BuiltInLobbies:
				WorkshopRepository.instance.LoadBuiltinLevels(requestLobbies: true);
				break;
			case WorkshopItemSource.EditorPick:
				WorkshopRepository.instance.LoadEditorPickLevels();
				break;
			}
			WorkshopRepository.instance.levelRepo.GetLevel(NetGame.instance.currentLevel, NetGame.instance.currentLevelType, delegate(WorkshopLevelMetadata l)
			{
				level = l;
				bool flag = false;
				if (level != null)
				{
					if (NetGame.isServer && DLC.instance.SupportsDLC() && level.workshopId < 16 && !DLC.instance.LevelIsAvailable((int)level.workshopId))
					{
						flag = true;
					}
					if (!flag)
					{
						BindImage(level.thumbnailTexture);
						levelName.text = level.title;
					}
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					BindImage(null);
					levelName.text = "MISSING";
					NetGame.instance.currentLevel = 0uL;
					if (NetGame.isServer)
					{
						App.instance.ChangeLobbyLevel(0uL, WorkshopItemSource.BuiltIn);
					}
					RebindLevel();
				}
			});
		}

		private void BindImage(Texture2D image)
		{
			if (image != null)
			{
				Rect rect = levelThumbnail.rectTransform.rect;
				float num = 1f;
				float num2 = 1f;
				if ((float)image.width / rect.width > (float)image.height / rect.height)
				{
					num = (float)image.height / rect.height / ((float)image.width / rect.width);
				}
				else
				{
					num2 = (float)image.width / rect.width / ((float)image.height / rect.height);
				}
				levelThumbnail.uvRect = new Rect(0.5f - num / 2f, 0.5f - num2 / 2f, num, num2);
			}
			levelThumbnail.texture = image;
		}

		public override void OnLostFocus()
		{
			base.OnLostFocus();
			NetGame.instance.transport.UnregisterForLobbyData(OnLobbyDataUpdate);
			if (level != null)
			{
				level.ReleaseThumbnailReference();
				level = null;
			}
			m_MenuOptions.SetPlayerActionVisibility(flag: false);
		}

		public void SelectLobbyClick()
		{
			if (MenuSystem.CanInvoke)
			{
				LevelSelectMenu2.instance.SetMultiplayerMode(inMultiplayer: true);
				LevelSelectMenu2.instance.ShowLobbies();
				TransitionForward<LevelSelectMenu2>();
			}
		}

		public void SelectLevelClick()
		{
			if (MenuSystem.CanInvoke)
			{
				LevelSelectMenu2.instance.SetMultiplayerMode(inMultiplayer: true);
				LevelSelectMenu2.instance.ShowPickADream();
				switch (NetGame.instance.currentLevelType)
				{
				case WorkshopItemSource.BuiltIn:
					LevelSelectMenu2.selectedPath = "builtin:" + NetGame.instance.currentLevel;
					break;
				case WorkshopItemSource.EditorPick:
					LevelSelectMenu2.selectedPath = "editorpick:" + NetGame.instance.currentLevel;
					break;
				default:
					LevelSelectMenu2.selectedPath = "ws:" + NetGame.instance.currentLevel;
					break;
				}
				TransitionForward<LevelSelectMenu2>();
			}
		}

		public void StartGameClick()
		{
			if (MenuSystem.CanInvoke)
			{
				Options.multiplayerLobbyLevelStore = Game.multiplayerLobbyLevel;
				App.instance.StartGameServer(NetGame.instance.currentLevel, NetGame.instance.currentLevelType);
				NetGame.instance.transport.SetLobbyStatus(status: true);
			}
		}

		public void SettingsClick()
		{
			if (MenuSystem.CanInvoke)
			{
				TransitionForward<MultiplayerLobbySettingsMenu>();
			}
		}

		public void InviteClick()
		{
			if (MenuSystem.CanInvoke)
			{
				NetGame.instance.transport.SendInvite();
			}
		}

		public void LeaveClick()
		{
			if (MenuSystem.CanInvoke && App.state != AppSate.Menu)
			{
				ConfirmMenu.MultiplayerLeaveLobby();
			}
		}

		public void HideUIClick()
		{
			if (MenuSystem.CanInvoke && MultiplayerLobbyController.instance != null)
			{
				MultiplayerLobbyController.instance.HideUI();
			}
		}

		public override void ApplyMenuEffects()
		{
			MenuCameraEffects.FadeInLobby();
		}

		public override void OnBack()
		{
			HideUIClick();
		}

		private void Awake()
		{
			m_MenuOptions = Object.FindObjectOfType<MultiplayerMenuOptions>();
		}

		protected override void Update()
		{
			base.Update();
			if ((bool)m_MenuOptions)
			{
				m_MenuOptions.OnButtonUpdate(lastFocusedElement);
				m_MenuOptions.OnUpdate();
			}
			if (NetGame.isClient)
			{
				RefreshLobby();
			}
		}

		private void RefreshLobby()
		{
			lobbyRefreshTimer -= Time.deltaTime;
			if (lobbyRefreshTimer < 0f)
			{
				NetGame.instance.transport.RequestLobbyDataRefresh(null, inSession: true);
				lobbyRefreshTimer = NetGame.instance.transport.GetLobbyDataRefreshThrottleTime();
			}
		}

		private void OnLobbyDataUpdate(object lobbyID, NetTransport.LobbyDisplayInfo dispInfo, bool error)
		{
			dispInfo.FeaturesMask &= 3758096383u;
			dispInfo.FeaturesMask &= 4294967263u;
			if (error)
			{
				informationBox.ClearDisplay();
			}
			else
			{
				informationBox.UpdateDisplay(dispInfo);
			}
		}
	}
}
