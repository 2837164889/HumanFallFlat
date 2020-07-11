using HumanAPI;
using System.Collections.Generic;
using UnityEngine;

public class Wind2 : Node
{
	public NodeInput input;

	public Vector3 wind;

	public float cDrag = 1f;

	public float humanFlyForce = 100f;

	private Vector3 worldWind;

	private float D;

	private Vector3 normal;

	private float storedValue;

	public Transform[] ignoreParents;

	private List<Rigidbody> bodiesAffected = new List<Rigidbody>();

	public float maxAcceleration = 20f;

	public float centeringSpring = 50f;

	public float centeringDamper = 10f;

	public float coreRadius = 1f;

	public float radialFalloff = 1f;

	public float dist = 5f;

	public float distFalloff = 5f;

	protected override void OnEnable()
	{
		base.OnEnable();
		Collider component = GetComponent<Collider>();
		for (int i = 0; i < ignoreParents.Length; i++)
		{
			Collider[] componentsInChildren = ignoreParents[i].GetComponentsInChildren<Collider>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				Physics.IgnoreCollision(component, componentsInChildren[j]);
			}
		}
	}

	public override void Process()
	{
		base.Process();
		storedValue = input.value;
	}

	public void OnTriggerEnter(Collider other)
	{
		OnTriggerStay(other);
	}

	public void OnTriggerStay(Collider other)
	{
		Rigidbody componentInParent = other.GetComponentInParent<Rigidbody>();
		if (componentInParent == null || componentInParent.isKinematic || bodiesAffected.Contains(componentInParent))
		{
			return;
		}
		bodiesAffected.Add(componentInParent);
		worldWind = base.transform.TransformVector(wind) * input.value;
		normal = worldWind.normalized;
		D = Vector3.Dot(base.transform.position, normal);
		if (!(worldWind == Vector3.zero))
		{
			float num = Vector3.Dot(componentInParent.worldCenterOfMass, normal) - D;
			Vector3 vector = componentInParent.worldCenterOfMass - num * normal - base.transform.position;
			float num2 = Mathf.InverseLerp(coreRadius + radialFalloff, coreRadius, vector.magnitude);
			float num3 = Mathf.InverseLerp(dist + distFalloff, dist, num);
			Human component = componentInParent.GetComponent<Human>();
			float num4 = componentInParent.mass;
			if (component != null)
			{
				num4 = component.mass / (float)component.rigidbodies.Length;
			}
			float d = num4;
			Vector3 vector2 = (worldWind - componentInParent.velocity).magnitude * (worldWind - componentInParent.velocity) * cDrag * d;
			if (vector.magnitude > 0.1f)
			{
				vector2 += (0f - componentInParent.mass) * vector * centeringSpring;
				vector2 += componentInParent.mass * Vector3.Project(-componentInParent.velocity, vector) * centeringDamper;
			}
			vector2 *= num2 * num3;
			vector2 = Vector3.ClampMagnitude(vector2, num4 * maxAcceleration);
			if (vector2.magnitude / num4 > 10f && GrabManager.IsGrabbedAny(componentInParent.gameObject))
			{
				GrabManager.Release(componentInParent.gameObject, 0.2f);
			}
			componentInParent.AddForce(vector2);
			Debug.DrawRay(componentInParent.worldCenterOfMass, vector2 / 10f / componentInParent.mass, Color.cyan, 0.1f);
			if (component != null)
			{
				componentInParent.AddForce(component.controls.walkDirection * humanFlyForce);
			}
		}
	}

	private void FixedUpdate()
	{
		bodiesAffected.Clear();
	}
}
