using UnityEngine;

public class SlideBehaviour : MonoBehaviour
{
	public float SideDrag = 0.5f;

	public float groundDistance = 0.6f;

	private Rigidbody rb;

	private bool grounded = true;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		grounded = Physics.Raycast(new Ray(base.transform.position, -base.transform.up), groundDistance);
		Color color = (!grounded) ? Color.red : Color.green;
		Debug.DrawRay(base.transform.position, -base.transform.up * groundDistance, color);
	}

	private void FixedUpdate()
	{
		if (grounded)
		{
			Vector3 vector = base.transform.InverseTransformVector(rb.velocity);
			rb.AddForce(-base.transform.right * SideDrag * vector.x + base.transform.forward * SideDrag * Mathf.Abs(vector.x) * Mathf.Sign(vector.z) * 0.5f);
			Debug.DrawRay(base.transform.position, -base.transform.right * SideDrag * vector.x / 10f);
		}
	}
}
