using UnityEngine;

public class PreserveRoll : MonoBehaviour
{
	private Rigidbody body;

	private void OnEnable()
	{
		body = GetComponent<Rigidbody>();
		body.maxAngularVelocity = 20f;
	}
}
