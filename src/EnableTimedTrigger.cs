using UnityEngine;

public class EnableTimedTrigger : MonoBehaviour, IReset
{
	public GameObject triggerToControl;

	public float enableTime;

	private float enableCountdown;

	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			triggerToControl.SetActive(value: true);
			enableCountdown = enableTime;
		}
	}

	private void Update()
	{
		if (enableCountdown > 0f)
		{
			enableCountdown -= Time.deltaTime;
			if (enableCountdown <= 0f)
			{
				triggerToControl.SetActive(value: false);
			}
		}
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		triggerToControl.SetActive(value: false);
	}
}
