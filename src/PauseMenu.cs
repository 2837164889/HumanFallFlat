using Multiplayer;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MenuTransition
{
	public TextMeshProUGUI textArea;

	public GameObject workshopArea;

	public GameObject workshopLevelButton;

	public Button reloadLevelButton;

	public RawImage workshopThumbnail;

	public Text workshopTitle;

	public Text workshopDescription;

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
		textArea.gameObject.SetActive(Game.instance.workshopLevel == null);
		workshopArea.SetActive(Game.instance.workshopLevel != null);
		reloadLevelButton.gameObject.SetActive(Game.instance.workshopLevel != null && Game.instance.workshopLevelIsCustom);
		workshopLevelButton.SetActive(value: false);
		if (Game.instance.workshopLevel == null)
		{
			List<string> list = TutorialRepository.instance.ListShownItems(Game.instance.currentLevelNumber, Game.instance.currentCheckpointNumber + 1);
			while (list.Count > 5)
			{
				list.RemoveAt(0);
			}
			textArea.text = string.Join("\r\n-----\r\n", list.ToArray());
		}
		else
		{
			Texture2D thumbnailTexture = Game.instance.workshopLevel.thumbnailTexture;
			workshopThumbnail.texture = thumbnailTexture;
			workshopTitle.text = Game.instance.workshopLevel.title;
			workshopDescription.text = LevelSelectMenu2.RichTextProcess(Game.instance.workshopLevel.description);
			workshopLevelButton.SetActive(Game.instance.IsWorkshopLevel());
			NetGame.instance.transport.RegisterForGameOverlayActivation(OnGameOverlayActivation);
		}
		base.OnGotFocus();
	}

	public override void OnLostFocus()
	{
		base.OnLostFocus();
		if (Game.instance.workshopLevel != null)
		{
			Game.instance.workshopLevel.ReleaseThumbnailReference();
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

	public void LoadClick()
	{
		if (MenuSystem.CanInvoke)
		{
			Game.instance.RestartCheckpoint();
			Resume();
		}
	}

	private void ResetLevelProgress()
	{
		if (Game.instance.workshopLevel != null)
		{
			Game.instance.gameProgress.CustomLevelProgress(Game.instance.workshopLevel.hash, -1);
		}
	}

	public void RestartClick()
	{
		if (MenuSystem.CanInvoke)
		{
			ResetLevelProgress();
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
			TransitionForward<ConfirmExitMenu>();
		}
	}

	public void WorkshopClick()
	{
		if (Game.instance.IsWorkshopLevel())
		{
			WorkshopUpload.ShowWorkshopItem(Game.instance.workshopLevel.workshopId);
		}
	}

	public override void OnBack()
	{
		ResumeClick();
	}

	public void ReloadLevelClick()
	{
		if (MenuSystem.CanInvoke)
		{
			Game.instance.ReloadBundle();
			Resume();
		}
	}
}
