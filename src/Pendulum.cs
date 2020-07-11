using System;
using UnityEngine;

public class Pendulum : MonoBehaviour
{
	public Vector3 axis = new Vector3(0f, 0f, 1f);

	private Quaternion originalRotation;

	public float timeOffset;

	public float period;

	public float amplitude;

	private Rigidbody body;

	private float time;

	private void Awake()
	{
		body = GetComponent<Rigidbody>();
		originalRotation = base.transform.rotation;
	}

	public void FixedUpdate()
	{
		body.MoveRotation(originalRotation * Quaternion.AngleAxis(Mathf.Sin((float)Math.PI * 2f * (timeOffset + time) / period) * amplitude, axis));
		time += Time.fixedDeltaTime;
	}
}
