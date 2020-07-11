using HumanAPI;
using UnityEngine;

public class WindEffector : Node
{
	public NodeInput input;

	public Vector3 wind;

	public float maxDist = 5f;

	public float distPower = 0.7f;

	public bool respectArea;

	public float coefDrag = 1f;

	public float cDamp = 0.5f;

	public float humanFlyForce = 100f;

	public float centerBend;

	public bool applyAcceleration;

	private Vector3 worldWind;

	private float D;

	private Vector3 normal;

	private float storedValue;

	public Rigidbody ignoreFan;

	public Transform[] ignoreParents;

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
		worldWind = base.transform.TransformVector(wind) * input.value;
		normal = worldWind.normalized;
		D = Vector3.Dot(base.transform.position, normal);
		storedValue = input.value;
	}

	public void OnTriggerEnter(Collider other)
	{
		OnTriggerStay(other);
	}

	public void OnTriggerStay(Collider other)
	{
		Rigidbody componentInParent = other.GetComponentInParent<Rigidbody>();
		if (componentInParent == null || componentInParent == ignoreFan || componentInParent.isKinematic)
		{
			return;
		}
		worldWind = base.transform.TransformVector(wind) * input.value;
		normal = worldWind.normalized;
		D = Vector3.Dot(base.transform.position, normal);
		if (!(worldWind == Vector3.zero))
		{
			float num = Vector3.Dot(componentInParent.worldCenterOfMass, normal) - D;
			float num2 = num / (maxDist * Mathf.Abs(storedValue));
			num2 = ((!(num2 > 0f)) ? Mathf.Clamp01(Mathf.Pow(Mathf.Clamp01(1f + num2), distPower)) : Mathf.Clamp01(Mathf.Pow(Mathf.Clamp01(1f - num2), distPower)));
			if (centerBend > 0f)
			{
				Vector3 a = componentInParent.worldCenterOfMass - num * normal - base.transform.position;
				normal = (normal - a / centerBend * num2).normalized;
				worldWind = normal * worldWind.magnitude;
			}
			float num3 = Vector3.Dot(componentInParent.velocity, normal);
			float num4 = worldWind.magnitude * num2 - num3;
			float num5 = coefDrag * Mathf.Sign(num4) * num4 * num4;
			float num6 = (0f - num3) * componentInParent.mass * cDamp / Time.fixedDeltaTime;
			Human component = componentInParent.GetComponent<Human>();
			if (applyAcceleration)
			{
				num5 = ((!(component != null)) ? (num5 * componentInParent.mass) : (num5 * (component.mass / (float)component.rigidbodies.Length)));
			}
			if (respectArea)
			{
				componentInParent.SafeAddForce(normal * (num5 * other.bounds.size.sqrMagnitude + num6));
			}
			else
			{
				componentInParent.AddForce(normal * (num5 + num6));
			}
			Debug.DrawRay(componentInParent.worldCenterOfMass, normal * (num5 + num6) / 500f, Color.cyan, 0.3f);
			if (component != null)
			{
				componentInParent.AddForce(component.controls.walkDirection * humanFlyForce);
			}
		}
	}
}
