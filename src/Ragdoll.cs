using HumanAPI;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
	public HumanSegment partHead;

	public HumanSegment partChest;

	public HumanSegment partWaist;

	public HumanSegment partHips;

	public HumanSegment partLeftArm;

	public HumanSegment partLeftForearm;

	public HumanSegment partLeftHand;

	public HumanSegment partLeftThigh;

	public HumanSegment partLeftLeg;

	public HumanSegment partLeftFoot;

	public HumanSegment partRightArm;

	public HumanSegment partRightForearm;

	public HumanSegment partRightHand;

	public HumanSegment partRightThigh;

	public HumanSegment partRightLeg;

	public HumanSegment partRightFoot;

	public HumanSegment partBall;

	public Transform[] bones;

	public float handLength;

	private bool initialized;

	private void Awake()
	{
		if (!initialized)
		{
			initialized = true;
			CollectSegments();
			SetupColliders();
			handLength = (partLeftArm.transform.position - partLeftForearm.transform.position).magnitude + (partLeftForearm.transform.position - partLeftHand.transform.position).magnitude;
		}
	}

	private void CollectSegments()
	{
		Dictionary<string, Transform> dictionary = new Dictionary<string, Transform>();
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			dictionary[componentsInChildren[i].name.ToLower()] = componentsInChildren[i];
		}
		partHead = FindSegment(dictionary, "head");
		partChest = FindSegment(dictionary, "chest");
		partWaist = FindSegment(dictionary, "waist");
		partHips = FindSegment(dictionary, "hips");
		partLeftArm = FindSegment(dictionary, "leftArm");
		partLeftForearm = FindSegment(dictionary, "leftForearm");
		partLeftHand = FindSegment(dictionary, "leftHand");
		partLeftThigh = FindSegment(dictionary, "leftThigh");
		partLeftLeg = FindSegment(dictionary, "leftLeg");
		partLeftFoot = FindSegment(dictionary, "leftFoot");
		partRightArm = FindSegment(dictionary, "rightArm");
		partRightForearm = FindSegment(dictionary, "rightForearm");
		partRightHand = FindSegment(dictionary, "rightHand");
		partRightThigh = FindSegment(dictionary, "rightThigh");
		partRightLeg = FindSegment(dictionary, "rightLeg");
		partRightFoot = FindSegment(dictionary, "rightFoot");
		SetupHeadComponents(partHead);
		SetupBodyComponents(partChest);
		SetupBodyComponents(partWaist);
		SetupBodyComponents(partHips);
		SetupLimbComponents(partLeftArm);
		SetupLimbComponents(partLeftForearm);
		SetupLimbComponents(partLeftThigh);
		SetupLimbComponents(partLeftLeg);
		SetupLimbComponents(partRightArm);
		SetupLimbComponents(partRightForearm);
		SetupLimbComponents(partRightThigh);
		SetupLimbComponents(partRightLeg);
		SetupFootComponents(partLeftFoot);
		SetupFootComponents(partRightFoot);
		SetupHandComponents(partLeftHand);
		SetupHandComponents(partRightHand);
		partLeftHand.sensor.otherSide = partRightHand.sensor;
		partRightHand.sensor.otherSide = partLeftHand.sensor;
		AddAntistretch(partLeftHand, partChest);
		AddAntistretch(partRightHand, partChest);
		AddAntistretch(partLeftFoot, partHips);
		AddAntistretch(partRightFoot, partHips);
	}

	private void AddAntistretch(HumanSegment seg1, HumanSegment seg2)
	{
		ConfigurableJoint configurableJoint = seg1.rigidbody.gameObject.AddComponent<ConfigurableJoint>();
		ConfigurableJointMotion configurableJointMotion2 = configurableJoint.zMotion = ConfigurableJointMotion.Limited;
		configurableJointMotion2 = (configurableJoint.xMotion = (configurableJoint.yMotion = configurableJointMotion2));
		configurableJoint.linearLimit = new SoftJointLimit
		{
			limit = (seg1.transform.position - seg2.transform.position).magnitude
		};
		configurableJoint.autoConfigureConnectedAnchor = false;
		configurableJoint.anchor = Vector3.zero;
		configurableJoint.connectedBody = seg2.rigidbody;
		configurableJoint.connectedAnchor = Vector3.zero;
	}

	public void BindBall(Transform ballTransform)
	{
		partBall = BindSegmanet(ballTransform);
		SpringJoint component = partBall.rigidbody.GetComponent<SpringJoint>();
		component.autoConfigureConnectedAnchor = false;
		component.connectedAnchor = partHips.transform.InverseTransformPoint(base.transform.position + Vector3.up * ((partBall.collider as SphereCollider).radius + component.maxDistance));
		component.connectedBody = partHips.rigidbody;
	}

	private void SetupHeadComponents(HumanSegment part)
	{
		part.sensor = part.transform.gameObject.AddComponent<CollisionSensor>();
		part.transform.gameObject.AddComponent<CollisionAudioSensor>();
		part.sensor.knockdown = 2f;
	}

	private void SetupBodyComponents(HumanSegment part)
	{
		part.sensor = part.transform.gameObject.AddComponent<CollisionSensor>();
		part.transform.gameObject.AddComponent<CollisionAudioSensor>();
		part.sensor.knockdown = 1f;
	}

	private void SetupLimbComponents(HumanSegment part)
	{
		part.sensor = part.transform.gameObject.AddComponent<CollisionSensor>();
	}

	private void SetupHandComponents(HumanSegment part)
	{
		part.sensor = part.transform.gameObject.AddComponent<CollisionSensor>();
		part.transform.gameObject.AddComponent<CollisionAudioSensor>();
	}

	private void SetupFootComponents(HumanSegment part)
	{
		part.sensor = part.transform.gameObject.AddComponent<CollisionSensor>();
		part.transform.gameObject.AddComponent<FootCollisionAudioSensor>();
		part.sensor.groundCheck = true;
	}

	private void SetupColliders()
	{
		Physics.IgnoreCollision(partChest.collider, partHead.collider);
		Physics.IgnoreCollision(partChest.collider, partLeftArm.collider);
		Physics.IgnoreCollision(partChest.collider, partLeftForearm.collider);
		Physics.IgnoreCollision(partChest.collider, partRightArm.collider);
		Physics.IgnoreCollision(partChest.collider, partRightForearm.collider);
		Physics.IgnoreCollision(partChest.collider, partWaist.collider);
		Physics.IgnoreCollision(partHips.collider, partChest.collider);
		Physics.IgnoreCollision(partHips.collider, partWaist.collider);
		Physics.IgnoreCollision(partHips.collider, partLeftThigh.collider);
		Physics.IgnoreCollision(partHips.collider, partLeftLeg.collider);
		Physics.IgnoreCollision(partHips.collider, partLeftFoot.collider);
		Physics.IgnoreCollision(partHips.collider, partRightThigh.collider);
		Physics.IgnoreCollision(partHips.collider, partRightLeg.collider);
		Physics.IgnoreCollision(partHips.collider, partRightFoot.collider);
		Physics.IgnoreCollision(partLeftForearm.collider, partLeftArm.collider);
		Physics.IgnoreCollision(partLeftForearm.collider, partLeftHand.collider);
		Physics.IgnoreCollision(partLeftArm.collider, partLeftHand.collider);
		Physics.IgnoreCollision(partRightForearm.collider, partRightArm.collider);
		Physics.IgnoreCollision(partRightForearm.collider, partRightHand.collider);
		Physics.IgnoreCollision(partRightArm.collider, partRightHand.collider);
		Physics.IgnoreCollision(partLeftThigh.collider, partLeftLeg.collider);
		Physics.IgnoreCollision(partRightThigh.collider, partRightLeg.collider);
	}

	private HumanSegment FindSegment(Dictionary<string, Transform> children, string name)
	{
		return BindSegmanet(children[name.ToLower()]);
	}

	private HumanSegment BindSegmanet(Transform transform)
	{
		HumanSegment humanSegment = new HumanSegment();
		humanSegment.transform = transform;
		humanSegment.collider = humanSegment.transform.GetComponent<Collider>();
		humanSegment.rigidbody = humanSegment.transform.GetComponent<Rigidbody>();
		humanSegment.startupRotation = humanSegment.transform.localRotation;
		humanSegment.sensor = humanSegment.transform.GetComponent<CollisionSensor>();
		humanSegment.bindPose = humanSegment.transform.worldToLocalMatrix * base.transform.localToWorldMatrix;
		return humanSegment;
	}

	internal void StretchHandsLegs(Vector3 direction, Vector3 right, int force)
	{
		partLeftHand.rigidbody.SafeAddForce((direction - right) * force / 2f);
		partRightHand.rigidbody.SafeAddForce((direction + right) * force / 2f);
		partLeftFoot.rigidbody.SafeAddForce((-direction - right) * force / 2f);
		partRightFoot.rigidbody.SafeAddForce((-direction + right) * force / 2f);
		partLeftForearm.rigidbody.SafeAddForce((direction - right) * force / 2f);
		partRightForearm.rigidbody.SafeAddForce((direction + right) * force / 2f);
		partLeftLeg.rigidbody.SafeAddForce((-direction - right) * force / 2f);
		partRightLeg.rigidbody.SafeAddForce((-direction + right) * force / 2f);
	}

	public void AllowHandBallRotation(bool allow)
	{
		ConfigurableJointMotion configurableJointMotion = allow ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked;
		ConfigurableJoint component = partLeftHand.rigidbody.GetComponent<ConfigurableJoint>();
		component.angularXMotion = configurableJointMotion;
		component.angularYMotion = configurableJointMotion;
		component.angularZMotion = configurableJointMotion;
		component = partRightHand.rigidbody.GetComponent<ConfigurableJoint>();
		component.angularXMotion = configurableJointMotion;
		component.angularYMotion = configurableJointMotion;
		component.angularZMotion = configurableJointMotion;
	}

	public void Lock()
	{
		Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].isKinematic = true;
		}
	}

	public void ToggleHeavyArms(bool left, bool right)
	{
		if (left)
		{
			Rigidbody rigidbody = partLeftHand.rigidbody;
			float num = 20f;
			partLeftArm.rigidbody.mass = num;
			num = num;
			partLeftForearm.rigidbody.mass = num;
			rigidbody.mass = num;
		}
		if (right)
		{
			Rigidbody rigidbody2 = partRightHand.rigidbody;
			float num = 20f;
			partRightArm.rigidbody.mass = num;
			num = num;
			partRightForearm.rigidbody.mass = num;
			rigidbody2.mass = num;
		}
	}

	public void ReleaseHeavyArms()
	{
		Rigidbody rigidbody = partLeftHand.rigidbody;
		float num = 5f;
		partLeftArm.rigidbody.mass = num;
		num = num;
		partLeftForearm.rigidbody.mass = num;
		rigidbody.mass = num;
		Rigidbody rigidbody2 = partRightHand.rigidbody;
		num = 5f;
		partRightArm.rigidbody.mass = num;
		num = num;
		partRightForearm.rigidbody.mass = num;
		rigidbody2.mass = num;
	}
}
