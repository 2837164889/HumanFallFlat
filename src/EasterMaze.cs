using UnityEngine;

public class EasterMaze : PathAchievementBase
{
	public Transform lantern;

	private float timeToCool;

	protected override bool IsValid(Collider trackedPlayer)
	{
		if (timeToCool > 0f || zoneCollider.GetComponent<BoxCollider>().bounds.Contains(lantern.position))
		{
			return false;
		}
		return true;
	}

	public override void UnlockAchievement()
	{
		if (!(timeToCool > 0f))
		{
			GetComponent<AudioSource>().Play();
			timeToCool = 60f;
		}
	}

	private void Update()
	{
		if (timeToCool > 0f)
		{
			timeToCool -= Time.deltaTime;
		}
	}
}
