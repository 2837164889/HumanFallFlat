using Multiplayer;
using UnityEngine;

public class BareHandsAchievement : MonoBehaviour, IReset
{
	public static BareHandsAchievement instance;

	public VoronoiShatter[] walls;

	public bool isCancelled;

	private void Awake()
	{
		instance = this;
	}

	public void OnDestroy()
	{
		instance = null;
	}

	internal void CancelAchievement()
	{
		isCancelled = true;
	}

	public void Update()
	{
		if (NetGame.isClient || isCancelled)
		{
			return;
		}
		for (int i = 0; i < walls.Length; i++)
		{
			if (!walls[i].shattered)
			{
				return;
			}
		}
		StatsAndAchievements.UnlockAchievement(Achievement.ACH_BREAK_BARE_HANDS);
		isCancelled = true;
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		isCancelled = false;
	}
}
