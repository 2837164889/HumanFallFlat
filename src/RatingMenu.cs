using Multiplayer;
using Steamworks;
using UnityEngine;

public sealed class RatingMenu : MenuTransition
{
	public enum RatingDestination
	{
		kNone,
		kMainMenu,
		kLevelSelectMenu,
		kMultiplayerLobby,
		kLostConnection
	}

	public const float kSecondsOfPlayToTriggerRating = 180f;

	public const string kLegendBarName = "ButtonLegendBar";

	public static RatingMenu instance;

	private bool mCurrentWorkshopLevelRated = true;

	private ulong mCurrentWorkshopLevel;

	private RatingDestination mDestination = RatingDestination.kMainMenu;

	private bool mNeedToShowRatingMenu;

	private float mLevelPlayedTime;

	private GameObject LegendBar;

	private CallResult<GetUserItemVoteResult_t> m_GetUserItemVoteResult;

	private CallResult<SetUserItemVoteResult_t> m_SetUserItemVoteResult;

	private void Awake()
	{
		instance = this;
		LegendBar = GameObject.Find("ButtonLegendBar");
	}

	public bool WorkshopLevelRated()
	{
		return mCurrentWorkshopLevelRated;
	}

	public override void OnGotFocus()
	{
		base.OnGotFocus();
		LegendBar.SetActive(value: false);
	}

	public override void OnLostFocus()
	{
		base.OnLostFocus();
		LegendBar.SetActive(value: true);
	}

	public void LoadInit()
	{
		mCurrentWorkshopLevel = 0uL;
		mNeedToShowRatingMenu = false;
	}

	protected override void Update()
	{
		base.Update();
		if (mCurrentWorkshopLevelRated)
		{
			NextMenu();
		}
	}

	public void LevelOver()
	{
		mNeedToShowRatingMenu = false;
		float num = Time.unscaledTime - mLevelPlayedTime;
		if (num > 180f && Game.instance.workshopLevel != null && !Game.instance.workshopLevel.folder.StartsWith("lvl:") && Game.instance.workshopLevel.workshopId == mCurrentWorkshopLevel && !mCurrentWorkshopLevelRated)
		{
			mNeedToShowRatingMenu = true;
		}
	}

	public void SetDestination(RatingDestination destination)
	{
		mDestination = destination;
	}

	public bool ShowRatingMenu()
	{
		return mNeedToShowRatingMenu;
	}

	private void NextMenu()
	{
		mNeedToShowRatingMenu = false;
		switch (mDestination)
		{
		case RatingDestination.kNone:
		case RatingDestination.kMainMenu:
			TransitionBack<MainMenu>();
			break;
		case RatingDestination.kLevelSelectMenu:
			MenuSystem.instance.ShowMainMenu<LevelSelectMenu2>();
			break;
		case RatingDestination.kMultiplayerLobby:
			MenuSystem.instance.ShowMainMenu<MultiplayerLobbyMenu>();
			break;
		case RatingDestination.kLostConnection:
			Dialogs.ConnectionLost(delegate
			{
				MenuSystem.instance.ShowMainMenu();
			});
			break;
		}
	}

	public void DownrateButton()
	{
		Vote(uprated: false, downrated: true, skip: false);
		NextMenu();
	}

	public void UprateButton()
	{
		Vote(uprated: true, downrated: false, skip: false);
		NextMenu();
	}

	public void SkipButton()
	{
		Vote(uprated: false, downrated: false, skip: true);
		NextMenu();
	}

	public void Vote(bool uprated, bool downrated, bool skip)
	{
		if (!skip)
		{
			if (m_SetUserItemVoteResult != null)
			{
				m_SetUserItemVoteResult.Cancel();
			}
			m_SetUserItemVoteResult = CallResult<SetUserItemVoteResult_t>.Create(OnSetUserItemVoteResult);
			SteamAPICall_t hAPICall = SteamUGC.SetUserItemVote(new PublishedFileId_t(mCurrentWorkshopLevel), uprated);
			m_SetUserItemVoteResult.Set(hAPICall);
		}
	}

	private void OnSetUserItemVoteResult(SetUserItemVoteResult_t result, bool bIOFailure)
	{
		Debug.Log("OnSetUserItemVoteResult: " + result.m_eResult);
	}

	private void OnGetUserItemVoteResult(GetUserItemVoteResult_t result, bool bIOFailure)
	{
		if (result.m_eResult == EResult.k_EResultOK)
		{
			if (result.m_bVotedUp || result.m_bVotedDown || result.m_bVoteSkipped)
			{
				mCurrentWorkshopLevelRated = true;
			}
			else
			{
				mCurrentWorkshopLevelRated = false;
			}
		}
	}

	public void QueryRatingStatus(ulong item, bool reset = true)
	{
		if (reset)
		{
			mCurrentWorkshopLevelRated = true;
			mCurrentWorkshopLevel = item;
			mLevelPlayedTime = Time.unscaledTime;
		}
		if (m_GetUserItemVoteResult != null)
		{
			m_GetUserItemVoteResult.Cancel();
		}
		m_GetUserItemVoteResult = CallResult<GetUserItemVoteResult_t>.Create(OnGetUserItemVoteResult);
		SteamAPICall_t userItemVote = SteamUGC.GetUserItemVote(new PublishedFileId_t(item));
		m_GetUserItemVoteResult.Set(userItemVote);
	}
}
