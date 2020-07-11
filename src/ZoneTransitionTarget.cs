using UnityEngine;

public class ZoneTransitionTarget : MonoBehaviour
{
	public ZoneTransitionTutorial tutorial;

	public void OnTriggerEnter(Collider other)
	{
		if (!(other.tag != "Player"))
		{
			tutorial.OnTargetReached();
		}
	}
}
