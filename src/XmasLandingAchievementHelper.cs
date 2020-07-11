using UnityEngine;

public class XmasLandingAchievementHelper : MonoBehaviour
{
	public XmasLandAchievement achiScript;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			achiScript.SetJumping(other.transform.parent.gameObject);
		}
		if (other.GetComponentInParent<SnowBoard>() != null)
		{
			achiScript.SetJumpingWithSnowBoard();
		}
	}
}
