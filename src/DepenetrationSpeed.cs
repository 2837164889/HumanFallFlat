using UnityEngine;

public class DepenetrationSpeed : MonoBehaviour
{
	public float max = 1000f;

	private void OnEnable()
	{
		GetComponent<Rigidbody>().maxDepenetrationVelocity = max;
	}
}
