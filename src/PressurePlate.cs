using UnityEngine;

public class PressurePlate : MonoBehaviour
{
	public Transform sensor;

	public float pressTreshold = 0.06f;

	public float releaseTreshold = 0.12f;

	public bool isPressed;

	public float holdState = 1f;

	private float timer;

	private void Update()
	{
		timer -= Time.deltaTime;
		if (isPressed)
		{
			Vector3 localPosition = sensor.localPosition;
			if (localPosition.y > releaseTreshold && timer <= 0f)
			{
				isPressed = false;
				timer = holdState;
			}
		}
		if (!isPressed)
		{
			Vector3 localPosition2 = sensor.localPosition;
			if (localPosition2.y < pressTreshold && timer <= 0f)
			{
				isPressed = true;
				timer = holdState;
			}
		}
	}
}
