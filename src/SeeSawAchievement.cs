using UnityEngine;

public class SeeSawAchievement : MonoBehaviour
{
	public static SeeSawAchievement instance;

	private bool failed;

	private void Start()
	{
		instance = this;
	}

	private void Update()
	{
	}

	public void Fail()
	{
		failed = true;
	}

	public void OnTriggerEnter(Collider other)
	{
		if (!failed)
		{
			foreach (Human item in Human.all)
			{
				if (item.GetComponent<Collider>() == other)
				{
					StatsAndAchievements.UnlockAchievement(Achievement.ACH_ICE_NO_ICE_BABY);
					break;
				}
			}
		}
	}
}
