using HumanAPI;
using System.Collections;
using UnityEngine;

public class SharkBehaviour : Node
{
	public NodeInput enraged;

	public Rigidbody[] m_Tail;

	public Rigidbody[] front;

	public float flopStrength = 1000f;

	public float flopCooldownMin = 1f;

	public float flopCooldownMax = 4f;

	private SharkState m_State = SharkState.Ground;

	private IEnumerator currentState;

	private Rigidbody body;

	public WaterSensor sensor;

	private float swimSpeed = 10f;

	public float multiplier = 10f;

	public float maxTailAngle = 30f;

	private FloatingMesh1[] parts;

	private float flopCooldown;

	private Vector3 startingDir;

	private void Start()
	{
		body = GetComponent<Rigidbody>();
		ChangeSharkState(m_State);
		if (sensor == null)
		{
			sensor = GetComponentInChildren<WaterSensor>();
		}
		parts = GetComponentsInChildren<FloatingMesh1>();
		startingDir = m_Tail[m_Tail.Length - 1].transform.forward;
	}

	private void FixedUpdate()
	{
		SharkState sharkState = (!(sensor.waterBody != null)) ? SharkState.Ground : SharkState.Water;
		if (m_State != sharkState)
		{
			ChangeSharkState(sharkState);
		}
		if (currentState != null)
		{
			currentState.MoveNext();
		}
		ApplyForces();
	}

	private void ApplyForces()
	{
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		float num = Vector3.Angle(m_Tail[m_Tail.Length - 1].transform.forward, startingDir);
		switch (m_State)
		{
		case SharkState.Ground:
			if (!(flopCooldown > Time.time))
			{
				flopCooldown = Time.time + Random.Range(flopCooldownMin, flopCooldownMax);
			}
			break;
		case SharkState.Water:
			zero += sensor.transform.forward * swimSpeed;
			break;
		}
	}

	private void ChangeSharkState(SharkState state)
	{
		switch (state)
		{
		case SharkState.Water:
			currentState = SwimState();
			break;
		case SharkState.Ground:
			currentState = FlopState();
			break;
		}
		m_State = state;
	}

	private IEnumerator SwimState()
	{
		int dir = 1;
		int tailFrames = 0;
		while (true)
		{
			float strength = (float)dir * (enraged.value * 3f + 0.3f) * flopStrength;
			if (tailFrames < 100)
			{
				tailFrames++;
			}
			else
			{
				dir = -dir;
				tailFrames = 0;
			}
			Vector3 d = strength * base.transform.up;
			Rigidbody[] tail = m_Tail;
			foreach (Rigidbody rigidbody in tail)
			{
				rigidbody.AddTorque(d, ForceMode.Force);
			}
			if (enraged.value > 0f)
			{
				base.transform.InverseTransformVector(front[0].velocity);
				front[0].AddForce(-front[0].transform.up * enraged.value * multiplier, ForceMode.Impulse);
			}
			yield return new WaitForFixedUpdate();
		}
	}

	private IEnumerator FlopState()
	{
		flopCooldown = Time.time + flopCooldownMax;
		while (Time.time < flopCooldown)
		{
			yield return new WaitForEndOfFrame();
		}
		while (true)
		{
			float strength = flopStrength * (float)(Random.Range(0, 2) * 2 - 1);
			Vector3 dir = strength * base.transform.up;
			for (int j = 0; j < 20; j++)
			{
				Rigidbody[] array = front;
				foreach (Rigidbody rigidbody in array)
				{
					rigidbody.AddTorque(-dir, ForceMode.Impulse);
				}
				Rigidbody[] tail = m_Tail;
				foreach (Rigidbody rigidbody2 in tail)
				{
					rigidbody2.AddTorque(dir, ForceMode.Impulse);
				}
				yield return new WaitForFixedUpdate();
			}
			dir *= -0.5f;
			for (int i = 0; i < 20; i++)
			{
				Rigidbody[] array2 = front;
				foreach (Rigidbody rigidbody3 in array2)
				{
					rigidbody3.AddTorque(-dir, ForceMode.Impulse);
				}
				Rigidbody[] tail2 = m_Tail;
				foreach (Rigidbody rigidbody4 in tail2)
				{
					rigidbody4.AddTorque(dir, ForceMode.Impulse);
				}
				yield return new WaitForFixedUpdate();
			}
			flopCooldown = Time.time + Random.Range(flopCooldownMin, flopCooldownMax);
			while (Time.time < flopCooldown)
			{
				yield return new WaitForEndOfFrame();
			}
		}
	}
}
