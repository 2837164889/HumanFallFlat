using I2.Loc;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Multiplayer
{
	public class MultiplayerPauseMenu : MenuTransition
	{
		public GameObject checkpointButton;

		public GameObject restartButton;

		public GameObject inviteButton;

		public TextMeshProUGUI ExitText;

		public LevelInformationBox informationBox;

		public GameObject workshopLevelButton;

		private MultiplayerMenuOptions m_MenuOptions;

		private float lobbyRefreshTimer;

		private const float kLobbyRefreshTime = 10f;

		public override void ApplyMenuEffects()
		{
			MenuCameraEffects.FadeInPauseMenu();
		}

		private void OnGameOverlayActivation(byte active)
		{
			if (active == 0 && !RatingMenu.instance.WorkshopLevelRated())
			{
				RatingMenu.instance.QueryRatingStatus(Game.instance.workshopLevel.workshopId, reset: false);
			}
		}

		public override void OnGotFocus()
		{
			base.OnGotFocus();
			restartButton.SetActive(NetGame.isServer);
			inviteButton.SetActive(NetGame.isServer && NetGame.instance.transport.CanSendInvite());
			if (!ExitText)
			{
				Debug.LogWarning("EXIT BUTTON reference not set in editor");
				return;
			}
			if (NetGame.isServer)
			{
				ExitText.SetText(ScriptLocalization.Get("MENU/PAUSE/EXIT TO LOBBY"));
			}
			else
			{
				ExitText.SetText(ScriptLocalization.Get("MENU/PAUSE/EXIT TO MAIN MENU"));
			}
			informationBox.gameObject.SetActive(value: true);
			NetGame.instance.transport.RegisterForLobbyData(OnLobbyDataUpdate);
			lobbyRefreshTimer = 0f;
			WorkshopItemSource currentLevelType = Game.instance.currentLevelType;
			if (currentLevelType == WorkshopItemSource.BuiltIn || currentLevelType == WorkshopItemSource.EditorPick)
			{
				workshopLevelButton.gameObject.SetActive(value: false);
				return;
			}
			workshopLevelButton.gameObject.SetActive(value: true);
			NetGame.instance.transport.RegisterForGameOverlayActivation(OnGameOverlayActivation);
		}

		public override void OnLostFocus()
		{
			base.OnLostFocus();
			m_MenuOptions.SetPlayerActionVisibility(flag: false);
		}

		public void WorkshopClick()
		{
			if (Game.instance.currentLevelType == WorkshopItemSource.Subscription)
			{
				WorkshopUpload.ShowWorkshopItem(Game.instance.workshopLevel.workshopId);
			}
		}

		private IEnumerator WaitForJoystickClear()
		{
			List<NetPlayer> players = NetGame.instance.local.players;
			while (true)
			{
				bool canResume = true;
				foreach (NetPlayer item in players)
				{
					if (item.controls != null && item.controls.ControllerJumpPressed())
					{
						canResume = false;
						break;
					}
				}
				if (canResume)
				{
					break;
				}
				yield return null;
			}
			Resume();
		}

		public void ResumeClick()
		{
			if (MenuSystem.CanInvoke)
			{
				StartCoroutine(WaitForJoystickClear());
			}
		}

		private void Resume()
		{
			Game.instance.Resume();
			MenuSystem.instance.HideMenus();
		}

		public void InviteClick()
		{
			if (MenuSystem.CanInvoke)
			{
				NetGame.instance.transport.SendInvite();
			}
		}

		public void LoadClick()
		{
			if (MenuSystem.CanInvoke)
			{
				if (NetGame.isClient)
				{
					NetGame.instance.SendRequestRespawn();
				}
				else
				{
					Game.instance.RestartCheckpoint();
				}
				Resume();
			}
		}

		public void RestartClick()
		{
			if (MenuSystem.CanInvoke)
			{
				Game.instance.RestartLevel();
				Resume();
			}
		}

		public void OptionsClick()
		{
			if (MenuSystem.CanInvoke)
			{
				OptionsMenu.returnToPause = true;
				TransitionForward<OptionsMenu>();
			}
		}

		public void ExitClick()
		{
			if (MenuSystem.CanInvoke)
			{
				ConfirmMenu.MultiplayerExitGame();
				NetGame.instance.transport.SetLobbyStatus(status: false);
			}
		}

		public override void OnBack()
		{
			ResumeClick();
		}

		private void Awake()
		{
			m_MenuOptions = UnityEngine.Object.FindObjectOfType<MultiplayerMenuOptions>();
		}

		protected override void Update()
		{
			base.Update();
			if ((bool)m_MenuOptions)
			{
				m_MenuOptions.OnButtonUpdate(lastFocusedElement);
				m_MenuOptions.OnUpdate();
			}
			RefreshLobby();
		}

		private void RefreshLobby()
		{
			lobbyRefreshTimer -= Time.deltaTime;
			if (lobbyRefreshTimer < 0f)
			{
				NetGame.instance.transport.RequestLobbyDataRefresh(null, inSession: true);
				lobbyRefreshTimer = Math.Max(10f, NetGame.instance.transport.GetLobbyDataRefreshThrottleTime());
			}
		}

		private void OnLobbyDataUpdate(object lobbyID, NetTransport.LobbyDisplayInfo dispInfo, bool error)
		{
			if (error)
			{
				dispInfo.InitBlank();
			}
			else
			{
				dispInfo.LevelID = NetGame.instance.currentLevel;
				dispInfo.LevelType = NetGame.instance.currentLevelType;
			}
			dispInfo.FeaturesMask &= 4294967263u;
			if (dispInfo.FeaturesMask == 0)
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
