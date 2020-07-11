using System;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSensor : MonoBehaviour
{
	private const float JOINT_BREAK_FORCE = 20000f;

	public CollisionSensor otherSide;

	public bool forwardCollisionAudio = true;

	private GrabManager grabManager;

	private GroundManager groundManager;

	private float handToHandClimb = 0.2f;

	public float knockdown;

	public bool groundCheck;

	public bool grab;

	public Vector3 grabPosition;

	public Vector3 targetPosition;

	private float grabPrecision = 0.1f;

	public Collider grabFilter;

	public bool onGround;

	public Transform groundObject;

	public ConfigurableJoint grabJoint;

	public Rigidbody grabBody;

	public GameObject grabObject;

	public Action<GameObject, Vector3, PhysicMaterial, Vector3> onCollideTap;

	public Action<GameObject, Vector3, PhysicMaterial, Vector3> onGrabTap;

	public Action offGrabTap;

	public Action<CollisionSensor, Collision> onGroundTap2;

	public Action<CollisionSensor> offGroundTap2;

	public Action<CollisionSensor, Collision> onStayTap2;

	private List<Collider> activeCollider = new List<Collider>();

	private List<Collider> ativeGrounded = new List<Collider>();

	private float blockGrab;

	private Transform thisTransform;

	private Rigidbody thisBody;

	[NonSerialized]
	public Human human;

	private Vector3 entryTangentVelocityImpulse;

	private Vector3 normalTangentVelocityImpulse;

	public float groundAngle;

	private ReleaseGrabTrigger blockGrabTrigger;

	public bool IsGrabbed(GameObject go)
	{
		if (grabObject == null)
		{
			return false;
		}
		Transform transform = grabObject.transform;
		while (transform != null)
		{
			if (transform.gameObject == go)
			{
				return true;
			}
			transform = transform.parent;
		}
		return false;
	}

	private void OnEnable()
	{
		thisTransform = base.transform;
		thisBody = GetComponent<Rigidbody>();
		grabManager = GetComponentInParent<GrabManager>();
		groundManager = GetComponentInParent<GroundManager>();
		human = GetComponentInParent<Human>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.contacts.Length != 0)
		{
			entryTangentVelocityImpulse = (normalTangentVelocityImpulse = collision.GetNormalTangentVelocitiesAndImpulse(thisBody));
			HandleCollision(collision, enter: true);
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (collision.contacts.Length != 0)
		{
			normalTangentVelocityImpulse = collision.GetNormalTangentVelocitiesAndImpulse(thisBody);
			HandleCollision(collision, enter: false);
			if (onStayTap2 != null)
			{
				onStayTap2(this, collision);
			}
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (!groundCheck)
		{
			return;
		}
		Transform transform = collision.transform;
		if (transform.root != thisTransform.root)
		{
			if (onGround && offGroundTap2 != null)
			{
				offGroundTap2(this);
			}
			onGround = false;
			groundObject = null;
		}
	}

	private void HandleCollision(Collision collision, bool enter)
	{
		if (collision.contacts.Length == 0 || !(thisTransform != null))
		{
			return;
		}
		Transform transform = collision.transform;
		if (!(transform.root != thisTransform.root))
		{
			return;
		}
		Rigidbody rigidbody = collision.rigidbody;
		Collider collider = collision.collider;
		ContactPoint[] contacts = collision.contacts;
		if (contacts.Length != 0)
		{
			if (grab && (grabFilter == null || grabFilter == collider))
			{
				GrabCheck(transform, rigidbody, collider, contacts);
			}
			if (groundCheck)
			{
				GroundCheck(collision);
			}
			if (enter && onCollideTap != null)
			{
				onCollideTap(base.gameObject, collision.contacts[0].point, collision.collider.sharedMaterial, normalTangentVelocityImpulse);
			}
		}
	}

	private void GroundCheck(Collision collision)
	{
		float num = Vector3.Angle(collision.GetImpulse().normalized, Vector3.up);
		if (num < groundAngle)
		{
			groundAngle = num;
		}
		ContactPoint[] contacts = collision.contacts;
		float num2 = 90f;
		foreach (ContactPoint contact in contacts)
		{
			float surfaceAngle = GetSurfaceAngle(contact, Vector3.up);
			if (surfaceAngle < 60f)
			{
				groundManager.ReportSurfaceAngle(surfaceAngle);
				if (surfaceAngle < num2)
				{
					num2 = surfaceAngle;
				}
			}
		}
		if (num2 < 60f)
		{
			if (num2 < groundAngle)
			{
				groundAngle = num2;
			}
			if (!onGround && onGroundTap2 != null)
			{
				onGroundTap2(this, collision);
			}
			onGround = true;
			groundObject = collision.transform;
			groundManager.ObjectEnter(groundObject.gameObject);
		}
	}

	private void GrabCheck(Transform collisionTransform, Rigidbody collisionRigidbody, Collider collider, ContactPoint[] contacts)
	{
		if (grabFilter != null || blockGrab > 0f || !(grabJoint == null) || !(blockGrabTrigger == null) || !(collisionTransform.tag != "NoGrab") || (targetPosition - contacts[0].point).magnitude > 0.5f)
		{
			return;
		}
		bool flag = collider.GetComponentInParent<Human>() != null;
		if (human.onGround && collider.tag != "Target" && !flag)
		{
			bool flag2 = collider.GetComponentInParent<Rigidbody>();
			Vector3 rhs = base.transform.position - targetPosition;
			Vector3 normal = contacts[0].normal;
			float num = Vector3.Dot(normal, rhs.normalized);
			if (num < ((!flag2) ? 0.7f : 0.4f))
			{
				return;
			}
			float num2 = Vector3.Dot(normal, rhs);
			if (num2 < ((!flag2) ? 0.2f : 0.05f))
			{
				return;
			}
		}
		IGrabbable componentInParent = collisionTransform.GetComponentInParent<IGrabbable>();
		if (componentInParent != null)
		{
			grabObject = (componentInParent as MonoBehaviour).gameObject;
		}
		else if (grabBody != null)
		{
			grabObject = grabBody.gameObject;
		}
		else
		{
			grabObject = collider.gameObject;
		}
		if (!CheatCodes.climbCheat && human.state == HumanState.Climb && otherSide.grabObject == grabObject)
		{
			Vector3 position = base.transform.position;
			float y = position.y;
			Vector3 position2 = otherSide.transform.position;
			if (y > position2.y + handToHandClimb)
			{
				return;
			}
		}
		grabJoint = base.gameObject.AddComponent<ConfigurableJoint>();
		if ((bool)collisionRigidbody)
		{
			grabJoint.connectedBody = collisionRigidbody;
		}
		grabJoint.xMotion = ConfigurableJointMotion.Locked;
		grabJoint.yMotion = ConfigurableJointMotion.Locked;
		grabJoint.zMotion = ConfigurableJointMotion.Locked;
		grabJoint.angularXMotion = ConfigurableJointMotion.Locked;
		grabJoint.angularYMotion = ConfigurableJointMotion.Locked;
		grabJoint.angularZMotion = ConfigurableJointMotion.Locked;
		grabJoint.breakForce = 20000f;
		grabJoint.breakTorque = 20000f;
		grabJoint.enablePreprocessing = false;
		grabBody = collisionRigidbody;
		grabManager.ObjectGrabbed(grabObject);
		if (onGrabTap != null)
		{
			onGrabTap(base.gameObject, contacts[0].point, collider.sharedMaterial, normalTangentVelocityImpulse);
		}
	}

	private float GetSurfaceAngle(ContactPoint contact, Vector3 direction)
	{
		return Vector3.Angle(contact.normal, direction);
	}

	private bool SurfaceWithinAngle(ContactPoint contact, Vector3 direction, float angle)
	{
		bool result = false;
		float num = Vector3.Angle(contact.normal, direction);
		if (num <= angle)
		{
			result = true;
		}
		return result;
	}

	public void ReleaseGrab(float blockTime = 0f)
	{
		if (grabJoint != null)
		{
			if (grabObject != null)
			{
				grabManager.ObjectReleased(grabObject);
			}
			UnityEngine.Object.Destroy(grabJoint);
			grabJoint = null;
			grabBody = null;
			grabObject = null;
			if (offGrabTap != null)
			{
				offGrabTap();
			}
		}
		blockGrab = Mathf.Max(blockGrab, blockTime);
	}

	public void BlockGrab(ReleaseGrabTrigger releaseGrabTrigger)
	{
		blockGrabTrigger = releaseGrabTrigger;
		ReleaseGrab();
	}

	public void UnblockBlockGrab()
	{
		blockGrabTrigger = null;
	}

	private void FixedUpdate()
	{
		if (!grab && grabJoint != null)
		{
			ReleaseGrab();
		}
		blockGrab -= Time.fixedDeltaTime;
		if (grabFilter == null || !grab || !(grabJoint == null) || !(blockGrab <= 0f))
		{
			return;
		}
		Vector3 position = base.transform.position;
		Vector3 vector = grabPosition - position;
		float radius = GetComponent<SphereCollider>().radius;
		Collider collider = grabFilter;
		Transform component = collider.GetComponent<Transform>();
		Rigidbody componentInParent = collider.GetComponentInParent<Rigidbody>();
		IGrabbable componentInParent2 = component.GetComponentInParent<IGrabbable>();
		if (componentInParent2 != null)
		{
			grabObject = (componentInParent2 as MonoBehaviour).gameObject;
		}
		else if (grabBody != null)
		{
			grabObject = grabBody.gameObject;
		}
		else
		{
			grabObject = collider.gameObject;
		}
		if (!CheatCodes.climbCheat && human.state == HumanState.Climb && otherSide.grabObject == grabObject)
		{
			float y = grabPosition.y;
			Vector3 position2 = otherSide.transform.position;
			if (y > position2.y + handToHandClimb)
			{
				return;
			}
		}
		grabJoint = base.gameObject.AddComponent<ConfigurableJoint>();
		grabJoint.autoConfigureConnectedAnchor = false;
		grabJoint.anchor = base.transform.InverseTransformPoint(position + vector.normalized * radius);
		if ((bool)componentInParent)
		{
			grabJoint.connectedBody = componentInParent;
			grabJoint.connectedAnchor = componentInParent.transform.InverseTransformPoint(grabPosition);
		}
		else
		{
			grabJoint.connectedAnchor = grabPosition;
		}
		grabJoint.xMotion = ConfigurableJointMotion.Locked;
		grabJoint.yMotion = ConfigurableJointMotion.Locked;
		grabJoint.zMotion = ConfigurableJointMotion.Locked;
		grabJoint.angularXMotion = ConfigurableJointMotion.Locked;
		grabJoint.angularYMotion = ConfigurableJointMotion.Locked;
		grabJoint.angularZMotion = ConfigurableJointMotion.Locked;
		grabJoint.breakForce = 20000f;
		grabJoint.breakTorque = 20000f;
		grabJoint.enablePreprocessing = false;
		grabBody = componentInParent;
		grabManager.ObjectGrabbed(grabObject);
		if (onGrabTap != null)
		{
			onGrabTap(base.gameObject, grabPosition, collider.sharedMaterial, normalTangentVelocityImpulse);
		}
	}

	private void OnJointBreak(float breakForce)
	{
		if (breakForce >= 20000f)
		{
			Debug.LogError("Joint break force: " + breakForce.ToString());
			ReleaseGrab();
		}
	}
}
