using HumanAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MagneticBody : Node
{
	public NodeInput input;

	public bool toggleViaSignal;

	[NonSerialized]
	public bool magnetActive = true;

	public bool disableOnContact;

	private List<Rigidbody> contactMagnetBodies = new List<Rigidbody>();

	[Tooltip("After a short time the magnets are fixed together")]
	public bool fixInPlace;

	[Tooltip("Once the Magnets are fixed turn off the Magnet ")]
	public bool disableOnFix;

	[Tooltip("The specific body to fix to this Magnet")]
	public Rigidbody bodyToFix;

	[Tooltip("How long to waut before fixing something")]
	public float timeToFix;

	private Joint fixJoint;

	private float fixTimer;

	private MagneticPoint[] magneticPoints;

	private List<MagneticPoint> nearbyMagnetics = new List<MagneticPoint>();

	[Tooltip("Use this in order to show the prints coming from the script")]
	public bool showDebug;

	public Rigidbody Body
	{
		get;
		private set;
	}

	public IEnumerable<MagneticPoint> NearbyMagnetics
	{
		get;
		private set;
	}

	public bool IsInContact(Rigidbody body)
	{
		return contactMagnetBodies.Contains(body);
	}

	private void Start()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Start ");
		}
		Body = GetComponentInParent<Rigidbody>();
		magneticPoints = GetComponentsInChildren<MagneticPoint>();
		MagneticPoint[] array = magneticPoints;
		foreach (MagneticPoint magneticPoint in array)
		{
			magneticPoint.magneticBody = this;
		}
		NearbyMagnetics = nearbyMagnetics.Distinct();
	}

	private void FixedUpdate()
	{
		if (!fixInPlace || !(fixJoint == null) || !(bodyToFix != null))
		{
			return;
		}
		bool flag = false;
		foreach (Rigidbody contactMagnetBody in contactMagnetBodies)
		{
			if (bodyToFix == contactMagnetBody && !GrabManager.IsGrabbedAny(contactMagnetBody.gameObject))
			{
				flag = true;
			}
		}
		if (flag)
		{
			fixTimer += Time.fixedDeltaTime;
			if (fixTimer >= timeToFix)
			{
				fixJoint = bodyToFix.gameObject.AddComponent<FixedJoint>();
				fixJoint.connectedBody = Body;
				CalculateMagnetActive();
				fixTimer = 0f;
			}
		}
		else
		{
			fixTimer = 0f;
		}
	}

	private void CalculateMagnetActive()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Magnet Active");
		}
		magnetActive = ((!toggleViaSignal || Mathf.Abs(input.value) >= 0.5f) && (!disableOnFix || fixJoint == null));
	}

	public override void Process()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Process ");
		}
		base.Process();
		CalculateMagnetActive();
	}

	private void OnTriggerEnter(Collider other)
	{
		MagneticBody componentInParent = other.gameObject.GetComponentInParent<MagneticBody>();
		if (!(componentInParent != null))
		{
			return;
		}
		MagneticPoint[] array = componentInParent.magneticPoints;
		if (array == null)
		{
			return;
		}
		if (showDebug)
		{
			Debug.Log(base.name + " Other Magnet ");
		}
		MagneticPoint[] array2 = array;
		foreach (MagneticPoint magneticPoint in array2)
		{
			if (magneticPoint.isActiveAndEnabled)
			{
				nearbyMagnetics.Add(magneticPoint);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		MagneticBody componentInParent = other.gameObject.GetComponentInParent<MagneticBody>();
		if (!(componentInParent != null))
		{
			return;
		}
		MagneticPoint[] array = componentInParent.magneticPoints;
		if (array == null)
		{
			return;
		}
		MagneticPoint[] array2 = array;
		foreach (MagneticPoint magneticPoint in array2)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Magnet Gone ");
			}
			if (magneticPoint.isActiveAndEnabled)
			{
				nearbyMagnetics.Remove(magneticPoint);
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.attachedRigidbody == null)
		{
			return;
		}
		MagneticBody componentInChildren = collision.collider.attachedRigidbody.gameObject.GetComponentInChildren<MagneticBody>();
		if (!(componentInChildren == null) && !(componentInChildren == this))
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Adding Magnet ");
			}
			contactMagnetBodies.Add(componentInChildren.Body);
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (collision.collider.attachedRigidbody == null)
		{
			return;
		}
		MagneticBody componentInChildren = collision.collider.attachedRigidbody.gameObject.GetComponentInChildren<MagneticBody>();
		if (!(componentInChildren == null))
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Removing Magnet ");
			}
			contactMagnetBodies.Remove(componentInChildren.Body);
		}
	}
}
