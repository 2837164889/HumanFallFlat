using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI
{
	public class BendableSegment : MonoBehaviour
	{
		public Bendable bandable;

		public int index;

		public float treshold = 100f;

		public float clampImpact = 130f;

		public int windowFrames = 20;

		public float maxTotal;

		public float maxImpactThisFrame;

		public Vector3 maxImpactVector;

		public float lastImpact;

		public float total;

		private List<float> impacts = new List<float>();

		private Vector3 point;

		public void OnCollisionEnter(Collision collision)
		{
			HandleCollision(collision);
		}

		public void OnCollisionStay(Collision collision)
		{
			HandleCollision(collision);
		}

		private void FixedUpdate()
		{
			float num = Mathf.Clamp(Mathf.Min(lastImpact, maxImpactThisFrame), 0f, clampImpact);
			lastImpact = maxImpactThisFrame;
			impacts.Add(num);
			total += num;
			if (total > treshold)
			{
				bandable.ReportBend(index, maxImpactVector);
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
			maxImpactVector = Vector3.zero;
		}

		private void HandleCollision(Collision collision)
		{
			if (collision.contacts.Length != 0)
			{
				point = collision.GetPoint();
				Vector3 impulse = collision.GetImpulse();
				BendableTool component = collision.rigidbody.GetComponent<BendableTool>();
				if (component != null)
				{
					impulse *= component.forceMultiplier;
				}
				float magnitude = impulse.magnitude;
				if (magnitude > maxImpactThisFrame)
				{
					maxImpactThisFrame = magnitude;
					maxImpactVector = impulse;
				}
			}
		}

		public void OnDrawGizmosSelected()
		{
			Gizmos.DrawRay(point, maxImpactVector * 100f);
		}
	}
}
