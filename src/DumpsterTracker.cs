using UnityEngine;

public class DumpsterTracker : MonoBehaviour, IReset
{
	private Vector3 startPos;

	private Collider trackedHuman;

	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			trackedHuman = other;
			startPos = base.transform.position;
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (other == trackedHuman)
		{
			trackedHuman = null;
			float magnitude = (startPos - base.transform.position).To2D().magnitude;
			StatsAndAchievements.AddDumpster(magnitude);
		}
	}

	public void Update()
	{
		if (trackedHuman != null)
		{
			float magnitude = (startPos - base.transform.position).To2D().magnitude;
			if (magnitude > 2f)
			{
				StatsAndAchievements.AddDumpster(magnitude);
				startPos = base.transform.position;
			}
		}
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		trackedHuman = null;
	}
}
