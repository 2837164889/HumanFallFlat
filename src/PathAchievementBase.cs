using UnityEngine;

public abstract class PathAchievementBase : MonoBehaviour
{
	private Collider trackedPlayer;

	public PathAchievementCollider entryCollider;

	public PathAchievementCollider zoneCollider;

	public PathAchievementCollider passCollider;

	public void PlayerEnter(PathAchievementCollider collider, Collider player)
	{
		if (collider == entryCollider)
		{
			OnEntry(player);
			trackedPlayer = player;
		}
		if (collider == passCollider && trackedPlayer == player)
		{
			UnlockAchievement();
			trackedPlayer = null;
		}
	}

	public void PlayerLeave(PathAchievementCollider collider, Collider player)
	{
		if (collider == zoneCollider && trackedPlayer == player)
		{
			trackedPlayer = null;
		}
	}

	public void PlayerStay(PathAchievementCollider collider, Collider player)
	{
		if (collider == zoneCollider && trackedPlayer == player && !IsValid(trackedPlayer))
		{
			trackedPlayer = null;
		}
	}

	public abstract void UnlockAchievement();

	protected virtual void OnEntry(Collider trackedPlayer)
	{
	}

	protected virtual bool IsValid(Collider trackedPlayer)
	{
		return true;
	}
}
