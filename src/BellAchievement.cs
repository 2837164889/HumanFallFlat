using UnityEngine;

public class BellAchievement : MonoBehaviour
{
	public float impulseThreshold = 100f;

	public float velocityThreshold = 5f;

	public void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.name.Contains("Rock") && (collision.impulse.magnitude > impulseThreshold || collision.rigidbody.velocity.magnitude > velocityThreshold))
		{
			StatsAndAchievements.UnlockAchievement(Achievement.ACH_SIEGE_BELL);
		}
	}
}
