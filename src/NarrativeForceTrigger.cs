using UnityEngine;

public class NarrativeForceTrigger : MonoBehaviour
{
	public NarrativeBlock narrative;

	public void OnTriggerEnter(Collider other)
	{
		if (!(other.tag != "Player"))
		{
			narrative.Play();
		}
	}
}
