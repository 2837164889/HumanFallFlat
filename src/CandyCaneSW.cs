using Multiplayer;
using System;
using UnityEngine;

public class CandyCaneSW : MonoBehaviour
{
	[Tooltip("The orientation information for a generated configurable joint")]
	public Transform axis;

	[Tooltip("The amount of damping to overcome and pull")]
	public float holdDamper = 5000f;

	[Tooltip("The amount of change in degrees for this to break")]
	public float breakAngleTravel = 90f;

	[Tooltip("Ground plane for this to ignore when poking out of the ground")]
	public Transform groundedIgnoreCollider;

	[Tooltip("Sound to play when this cane is pulled from the ground")]
	public AudioSource audioSource;

	private ConfigurableJoint joint;

	private Vector3 anchor;

	private float totalTravel;

	private NetIdentity identity;

	private uint evtPull;

	[Tooltip("Use this in order to show the prints coming from the script")]
	public bool showDebug;

	private void OnEnable()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Enabled ");
		}
		identity = GetComponentInParent<NetIdentity>();
		evtPull = identity.RegisterEvent(OnPull);
		if (!NetGame.isClient)
		{
			if (joint == null)
			{
				joint = CreateJoint();
			}
			anchor = axis.TransformPoint(Vector3.forward);
			totalTravel = 0f;
			if (groundedIgnoreCollider != null)
			{
				IgnoreCollision.Ignore(base.transform, groundedIgnoreCollider);
			}
		}
	}

	private void OnPull(NetStream stream)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Pulled ");
		}
		if (audioSource != null)
		{
			audioSource.Play();
		}
	}

	private ConfigurableJoint CreateJoint()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Create Joint ");
		}
		ConfigurableJoint configurableJoint = base.gameObject.AddComponent<ConfigurableJoint>();
		configurableJoint.anchor = axis.localPosition;
		configurableJoint.axis = base.transform.InverseTransformDirection(axis.forward);
		configurableJoint.secondaryAxis = base.transform.InverseTransformDirection(axis.right);
		configurableJoint.xMotion = ConfigurableJointMotion.Locked;
		configurableJoint.yMotion = ConfigurableJointMotion.Locked;
		configurableJoint.zMotion = ConfigurableJointMotion.Locked;
		configurableJoint.angularXMotion = ConfigurableJointMotion.Free;
		configurableJoint.angularYMotion = ConfigurableJointMotion.Free;
		configurableJoint.angularZMotion = ConfigurableJointMotion.Free;
		configurableJoint.angularXDrive = new JointDrive
		{
			positionSpring = 0f,
			positionDamper = holdDamper,
			maximumForce = float.PositiveInfinity
		};
		configurableJoint.angularYZDrive = new JointDrive
		{
			positionSpring = 0f,
			positionDamper = holdDamper,
			maximumForce = float.PositiveInfinity
		};
		return configurableJoint;
	}

	public void FixedUpdate()
	{
		if (joint == null || NetGame.isClient || ReplayRecorder.isPlaying)
		{
			return;
		}
		Vector3 vector = axis.TransformPoint(Vector3.forward);
		float num = (vector - anchor).magnitude - 0.1f;
		if (!(num > 0f))
		{
			return;
		}
		totalTravel += num / (breakAngleTravel * ((float)Math.PI / 180f));
		anchor = Vector3.MoveTowards(anchor, vector, num);
		if (totalTravel > 1f)
		{
			NetStream netStream = identity.BeginEvent(evtPull);
			identity.EndEvent();
			OnPull(null);
			UnityEngine.Object.Destroy(joint);
			GetComponent<Rigidbody>().angularDrag = 0.05f;
			GetComponent<Rigidbody>().useGravity = true;
			if (groundedIgnoreCollider != null)
			{
				IgnoreCollision.Unignore(base.transform, groundedIgnoreCollider);
			}
		}
		else
		{
			joint.angularYZDrive = new JointDrive
			{
				positionSpring = 0f,
				positionDamper = holdDamper * (1f - totalTravel),
				maximumForce = float.PositiveInfinity
			};
		}
	}

	private void Awake()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Awake ");
		}
	}

	private void OnBeep(NetStream stream)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Beep ");
		}
		audioSource.Play();
	}
}
