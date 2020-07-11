using UnityEngine;

public class ForceSleep : MonoBehaviour
{
	public float sleepSpeed = 0.05f;

	public int sleepFrames = 30;

	private Vector3[] positions;

	private Rigidbody[] bodies;

	private int frames;

	private void Awake()
	{
		bodies = GetComponentsInChildren<Rigidbody>();
		positions = new Vector3[bodies.Length];
	}

	private void FixedUpdate()
	{
		bool flag = false;
		float num = sleepSpeed * sleepSpeed * Time.fixedDeltaTime * Time.fixedDeltaTime;
		for (int i = 0; i < bodies.Length; i++)
		{
			Vector3 position = bodies[i].position;
			if (!flag && (position - positions[i]).sqrMagnitude > num)
			{
				flag = true;
			}
			positions[i] = position;
		}
		if (flag)
		{
			frames = 0;
			return;
		}
		frames++;
		if (frames < sleepFrames)
		{
			return;
		}
		for (int j = 0; j < bodies.Length; j++)
		{
			if (!bodies[j].IsSleeping())
			{
				bodies[j].Sleep();
			}
		}
	}
}
