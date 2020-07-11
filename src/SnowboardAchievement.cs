using System;
using UnityEngine;

public class SnowboardAchievement : MonoBehaviour
{
	private class SnowboardData
	{
		public Human h0;

		public Human h1;

		public bool passedFirstCheckpoint;
	}

	public static SnowboardAchievement instance;

	public SnowBoard[] snowboards;

	private SnowboardData[] snowboardData;

	private void Start()
	{
		instance = this;
		snowboardData = new SnowboardData[snowboards.Length];
		for (int i = 0; i < snowboardData.Length; i++)
		{
			snowboardData[i] = new SnowboardData();
		}
	}

	public void RegisterAttach(SnowBoard snowboard, Human human)
	{
		int num = Array.IndexOf(snowboards, snowboard);
		if (num >= 0)
		{
			SnowboardData snowboardData = this.snowboardData[num];
			if (snowboardData.h0 == null)
			{
				snowboardData.h0 = human;
				snowboardData.passedFirstCheckpoint = false;
			}
			else if (snowboardData.h1 == null)
			{
				snowboardData.h1 = human;
				snowboardData.passedFirstCheckpoint = false;
			}
			else
			{
				Debug.LogWarning("Tried to attach a third human to a snowboard");
			}
		}
		else
		{
			Debug.LogWarning("Snowboard hasn't been registered in SnowboardAchievement object");
		}
	}

	public void RegisterDetach(SnowBoard snowboard, Human human)
	{
		int num = Array.IndexOf(snowboards, snowboard);
		if (num >= 0)
		{
			SnowboardData snowboardData = this.snowboardData[num];
			if (snowboardData.h0 == human)
			{
				snowboardData.h0 = null;
				snowboardData.passedFirstCheckpoint = false;
			}
			else if (snowboardData.h1 == human)
			{
				snowboardData.h1 = null;
				snowboardData.passedFirstCheckpoint = false;
			}
			else
			{
				Debug.LogWarning("Tried to detach a human that hadn't been attached to a snowboard");
			}
		}
		else
		{
			Debug.LogWarning("Snowboard hasn't been registered in SnowboardAchievement object");
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		SnowBoard componentInParent = other.GetComponentInParent<SnowBoard>();
		if (!(componentInParent != null))
		{
			return;
		}
		int num = Array.IndexOf(snowboards, componentInParent);
		if (num >= 0)
		{
			SnowboardData data = snowboardData[num];
			Human human;
			if (data.h0 != null && data.h1 == null)
			{
				human = data.h0;
			}
			else
			{
				if (!(data.h1 != null) || !(data.h0 == null))
				{
					return;
				}
				human = data.h1;
			}
			int num2 = Array.FindIndex(snowboardData, (SnowboardData d) => d != data && (d.h0 == human || d.h1 == human));
			if (num2 >= 0)
			{
				StatsAndAchievements.UnlockAchievement(Achievement.ACH_ICE_TRICKY);
			}
		}
		else
		{
			Debug.LogWarning("Snowboard hasn't been registered in SnowboardAchievement object");
		}
	}

	private void Update()
	{
	}
}
