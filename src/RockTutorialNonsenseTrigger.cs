using UnityEngine;

public class RockTutorialNonsenseTrigger : MonoBehaviour
{
	public TutorialBlock mainTutorial;

	public bool playerInside;

	private float leaveTime;

	public void OnTriggerEnter(Collider other)
	{
		if (mainTutorial.isActiveAndEnabled && !(other.tag != "Player"))
		{
			playerInside = true;
			mainTutorial.ReportNonsense();
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (mainTutorial.isActiveAndEnabled && !(other.tag != "Player"))
		{
			playerInside = false;
			mainTutorial.UnreportNonsense();
		}
	}
}
