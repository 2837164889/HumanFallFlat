using System.Collections.Generic;
using UnityEngine;

public class SkiLiftAchievement : MonoBehaviour
{
	private class HumanData
	{
		public Human human;

		public int contactCount;

		public int waypointCount;

		public bool givenAchievement;
	}

	private FollowWaypoints waypoints;

	private int curPoint;

	private List<HumanData> humanData = new List<HumanData>();

	private void Start()
	{
		waypoints = GetComponent<FollowWaypoints>();
		curPoint = waypoints.curPoint;
	}

	private void Update()
	{
		if (waypoints.curPoint == curPoint)
		{
			return;
		}
		curPoint = waypoints.curPoint;
		for (int num = humanData.Count - 1; num >= 0; num--)
		{
			if (!humanData[num].givenAchievement)
			{
				humanData[num].waypointCount++;
				if (humanData[num].waypointCount > waypoints.waypoints.Length)
				{
					humanData[num].givenAchievement = true;
					StatsAndAchievements.UnlockAchievement(Achievement.ACH_ICE_TAKING_THE_PISTE);
				}
			}
		}
	}

	public void AddHuman(Human h)
	{
		int num = this.humanData.FindIndex((HumanData hd) => hd.human == h);
		if (num < 0)
		{
			HumanData humanData = new HumanData();
			humanData.human = h;
			humanData.waypointCount = 0;
			humanData.contactCount = 1;
			this.humanData.Add(humanData);
		}
		else
		{
			this.humanData[num].contactCount++;
		}
	}

	public void RemoveHuman(Human h)
	{
		int num = humanData.FindIndex((HumanData hd) => hd.human == h);
		if (num >= 0)
		{
			humanData[num].contactCount--;
			if (humanData[num].contactCount <= 0)
			{
				humanData.RemoveAt(num);
			}
		}
	}
}
