using UnityEngine;

public class PathAchievementCollider : MonoBehaviour
{
	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			GetComponentInParent<PathAchievementBase>().PlayerEnter(this, other);
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			GetComponentInParent<PathAchievementBase>().PlayerLeave(this, other);
		}
	}

	public void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player")
		{
			GetComponentInParent<PathAchievementBase>().PlayerStay(this, other);
		}
	}
}
