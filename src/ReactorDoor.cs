using UnityEngine;

public class ReactorDoor : MonoBehaviour
{
	public bool initiallyClosed = true;

	public Collider closedTrigger;

	private bool isClosed;

	private void Start()
	{
		isClosed = initiallyClosed;
	}

	public bool IsClosed()
	{
		return isClosed;
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other == closedTrigger)
		{
			isClosed = true;
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (other == closedTrigger)
		{
			isClosed = false;
		}
	}
}
