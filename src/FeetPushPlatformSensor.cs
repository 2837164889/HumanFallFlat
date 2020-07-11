using UnityEngine;

public class FeetPushPlatformSensor : MonoBehaviour
{
	public FeetPushTutorial mainTutorial;

	public GameObject platform;

	private void Update()
	{
		if (!mainTutorial.isActiveAndEnabled)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < Human.all.Count; i++)
		{
			Human human = Human.all[i];
			if (human.grabManager.IsGrabbed(platform) && human.onGround && !human.groundManager.IsStanding(platform.gameObject))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			mainTutorial.ReportNonsense();
		}
		else
		{
			mainTutorial.UnreportNonsense();
		}
	}
}
