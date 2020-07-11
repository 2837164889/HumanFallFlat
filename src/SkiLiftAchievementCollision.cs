using UnityEngine;

public class SkiLiftAchievementCollision : MonoBehaviour
{
	public SkiLiftAchievement achievementTracker;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnCollisionEnter(Collision collision)
	{
		Human componentInParent = collision.rigidbody.GetComponentInParent<Human>();
		if (componentInParent != null)
		{
			achievementTracker.AddHuman(componentInParent);
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		Human componentInParent = collision.rigidbody.GetComponentInParent<Human>();
		if (componentInParent != null)
		{
			achievementTracker.RemoveHuman(componentInParent);
		}
	}
}
