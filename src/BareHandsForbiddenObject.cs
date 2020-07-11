using UnityEngine;

public class BareHandsForbiddenObject : MonoBehaviour
{
	public void OnCollisionEnter(Collision collision)
	{
		VoronoiShatter component = collision.gameObject.GetComponent<VoronoiShatter>();
		if (!(component != null))
		{
			return;
		}
		for (int i = 0; i < BareHandsAchievement.instance.walls.Length; i++)
		{
			if (BareHandsAchievement.instance.walls[i] == component)
			{
				BareHandsAchievement.instance.CancelAchievement();
			}
		}
	}
}
