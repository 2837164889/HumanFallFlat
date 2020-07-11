using System;
using UnityEngine;

public class RagdollBoneEnvelope : MonoBehaviour
{
	public float innerThickness;

	public float outerThickness;

	public float power;

	public float innerWeight;

	public float outerWeight;

	public Vector3 centerOffset;

	public float lengthMultiplier;

	[NonSerialized]
	public Vector3 start;

	[NonSerialized]
	public Vector3 direction;

	[NonSerialized]
	public float height;

	[NonSerialized]
	public float radius;

	public float innerRadius
	{
		get
		{
			float num = radius - ((innerThickness == 0f) ? 0.125f : innerThickness);
			return (!(num > 0f)) ? 0f : num;
		}
	}

	public float outerRadius => radius + ((outerThickness == 0f) ? 0.125f : outerThickness);

	public void ReadCollider()
	{
		Collider component = GetComponent<Collider>();
		if (component is SphereCollider)
		{
			SphereCollider sphereCollider = component as SphereCollider;
			start = sphereCollider.center + centerOffset;
			direction = Vector3.zero;
			height = 0f;
			radius = sphereCollider.radius;
		}
		else if (component is CapsuleCollider)
		{
			CapsuleCollider capsuleCollider = component as CapsuleCollider;
			direction = new Vector3(1f, 0f, 0f);
			if (capsuleCollider.direction == 1)
			{
				direction = new Vector3(0f, 1f, 0f);
			}
			else if (capsuleCollider.direction == 2)
			{
				direction = new Vector3(0f, 0f, 1f);
			}
			height = Math.Max(0f, capsuleCollider.height - 2f * capsuleCollider.radius);
			if (lengthMultiplier != 0f)
			{
				height *= lengthMultiplier;
			}
			start = capsuleCollider.center - direction * height / 2f + centerOffset;
			radius = capsuleCollider.radius;
		}
	}
}
