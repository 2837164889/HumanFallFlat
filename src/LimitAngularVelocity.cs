using UnityEngine;

public class LimitAngularVelocity : MonoBehaviour
{
	public float maxAngular;

	private void Start()
	{
		GetComponent<Rigidbody>().maxAngularVelocity = maxAngular;
	}
}
