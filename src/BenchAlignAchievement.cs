using UnityEngine;

public class BenchAlignAchievement : MonoBehaviour
{
	private bool awarded;

	private void Update()
	{
		if (awarded)
		{
			return;
		}
		Vector3 forward = base.transform.forward;
		if (forward.y > 0.97f)
		{
			Vector3 up = base.transform.up;
			if (up.x < -0.97f)
			{
				StatsAndAchievements.UnlockAchievement(Achievement.ACH_PUSH_BENCH_ALIGN);
				awarded = true;
			}
		}
	}
}
