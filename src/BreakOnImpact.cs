using System.Collections.Generic;
using UnityEngine;

public class BreakOnImpact : MonoBehaviour
{
	public Joint jointToBreak;

	public float treshold = 1500f;

	public float clampImpact = 130f;

	public int windowFrames = 20;

	public float maxTotal;

	public float maxImpactThisFrame;

	public float lastImpact;

	public float total;

	private List<float> impacts = new List<float>();

	private bool broken;

	private void OnEnable()
	{
		if (jointToBreak == null)
		{
			jointToBreak = GetComponent<Joint>();
		}
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (!broken)
		{
			HandleCollision(collision);
		}
	}

	public void OnCollisionStay(Collision collision)
	{
		if (!broken)
		{
			HandleCollision(collision);
		}
	}

	private void FixedUpdate()
	{
		if (broken)
		{
			return;
		}
		float num = Mathf.Clamp(Mathf.Min(lastImpact, maxImpactThisFrame), 0f, clampImpact);
		lastImpact = maxImpactThisFrame;
		impacts.Add(num);
		total += num;
		if (total > treshold)
		{
			broken = true;
			if (jointToBreak != null)
			{
				Object.Destroy(jointToBreak);
			}
		}
		if (total > maxTotal)
		{
			maxTotal = total;
		}
		if (impacts.Count > 20)
		{
			total -= impacts[0];
			impacts.RemoveAt(0);
		}
		maxImpactThisFrame = 0f;
	}

	private void HandleCollision(Collision collision)
	{
		if (collision.impulse.magnitude > maxImpactThisFrame)
		{
			maxImpactThisFrame = collision.impulse.magnitude;
		}
	}
}
