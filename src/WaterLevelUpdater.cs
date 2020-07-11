using UnityEngine;

public class WaterLevelUpdater : MonoBehaviour
{
	[SerializeField]
	private float liftSpeed = 10f;

	[SerializeField]
	private bool debug;

	private Vector3 targetPosition;

	public float maxHeight = 10f;

	private float startingHeight;

	private void Awake()
	{
		targetPosition = base.transform.position;
		Vector3 position = base.transform.position;
		startingHeight = position.y;
	}

	private void OnTriggerEnter(Collider other)
	{
		WaterItemVolume component = other.GetComponent<WaterItemVolume>();
		if (component != null && !component.hasBeenDrown)
		{
			component.hasBeenDrown = true;
			targetPosition += Vector3.up * component.Volume;
			if (targetPosition.y > startingHeight + maxHeight)
			{
				targetPosition.y = startingHeight + maxHeight;
			}
		}
	}

	private void Update()
	{
		base.transform.position = Vector3.Lerp(base.transform.position, targetPosition, Time.deltaTime * liftSpeed);
	}
}
