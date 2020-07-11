using HumanAPI;
using Multiplayer;
using System.Collections.Generic;
using UnityEngine;

public class SnowBallGrowth : Node, IPostEndReset, IPreReset
{
	public float growthMultiplier = 1f;

	public NodeInput melt;

	public float meltSpeed = 0.1f;

	private Rigidbody rb;

	private Quaternion lastRot;

	[SerializeField]
	private float minimumScale = 0.25f;

	[SerializeField]
	private float radius = 0.45f;

	[SerializeField]
	private float density = 35f;

	[SerializeField]
	private Transform ignoreHat;

	private const string kSnowMaterialIndicator = "Snow";

	private Dictionary<Material, bool> snowyMaterials = new Dictionary<Material, bool>();

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		if (NetGame.isClient)
		{
			return;
		}
		Quaternion rotation = base.transform.rotation;
		float f = Quaternion.Angle(lastRot, rotation);
		lastRot = rotation;
		float num = 0f;
		if (InsideIceCaves.instance == null || !InsideIceCaves.instance.SnowballInsideCave(this))
		{
			Vector3 position = base.transform.position;
			Vector3 down = Vector3.down;
			Vector3 lossyScale = base.transform.lossyScale;
			if (Physics.Raycast(position, down, out RaycastHit hitInfo, lossyScale.x / 2f + 0.2f, 1))
			{
				MeshRenderer component = hitInfo.collider.gameObject.GetComponent<MeshRenderer>();
				bool value = false;
				if (component != null)
				{
					Material[] sharedMaterials = component.sharedMaterials;
					Material[] array = sharedMaterials;
					foreach (Material material in array)
					{
						if (snowyMaterials.TryGetValue(material, out value))
						{
							break;
						}
						value = material.name.Contains("Snow");
						snowyMaterials.Add(material, value);
					}
				}
				if (value)
				{
					num = Mathf.Abs(f) * growthMultiplier / 360f;
				}
			}
		}
		num -= melt.value * meltSpeed * Time.fixedDeltaTime;
		if (num != 0f)
		{
			UpdateSize(num);
		}
	}

	private void UpdateSize(float growth)
	{
		Vector3 localScale = base.transform.localScale + new Vector3(growth, growth, growth);
		if (localScale.x > minimumScale && localScale.y > minimumScale && localScale.z > minimumScale)
		{
			base.transform.localScale = localScale;
			rb.SetDensity(density);
			rb.mass = rb.mass;
			AdjustHandAnchors(growth);
		}
	}

	public void AdjustHandAnchors(float growth)
	{
		for (int i = 0; i < Human.all.Count; i++)
		{
			Human human = Human.all[i];
			AdjustHandAnchors(human.ragdoll.partLeftHand.sensor.grabJoint, growth);
			AdjustHandAnchors(human.ragdoll.partRightHand.sensor.grabJoint, growth);
		}
	}

	public void AdjustHandAnchors(ConfigurableJoint joint, float growth)
	{
		if (!(joint == null) && !(joint.connectedBody != rb))
		{
			Vector3 vector = joint.connectedBody.transform.TransformVector(joint.connectedAnchor);
			joint.autoConfigureConnectedAnchor = false;
			Vector3 a = vector;
			Vector3 localScale = joint.connectedBody.transform.localScale;
			vector = a * (localScale.x * radius / vector.magnitude);
			joint.connectedAnchor = joint.connectedBody.transform.InverseTransformVector(vector);
		}
	}

	void IPostEndReset.PostEndResetState(int checkpoint)
	{
		lastRot = base.transform.rotation;
	}

	private void OnJointBreak(float breakForce)
	{
		IgnoreCollision.Unignore(base.transform, ignoreHat);
	}

	void IPreReset.PreResetState(int checkpoint)
	{
		IgnoreCollision.Ignore(base.transform, ignoreHat);
	}
}
