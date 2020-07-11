using UnityEngine;

public class CGShift : MonoBehaviour, IReset
{
	[Tooltip("Offset in local space")]
	public Vector3 localOffset;

	[Tooltip("Local offset for the effect")]
	public Vector3 offset;

	[Tooltip("Multiplier for the centre of gravity")]
	public Vector3 inertiaMultiplier = Vector3.one;

	private Vector3 originalCG;

	private Vector3 originalInertia;

	public bool showdebug;

	private bool SeenString1;

	private bool SeenString2;

	private bool SeenString3;

	private Rigidbody body;

	private bool applied;

	private void Awake()
	{
		body = GetComponent<Rigidbody>();
		originalCG = body.centerOfMass;
		originalInertia = body.inertiaTensor;
	}

	private void OnEnable()
	{
		if (localOffset == Vector3.zero && offset != Vector3.zero)
		{
			if (showdebug)
			{
				Debug.Log(base.name + " Local offset is zero and offset is not zero ");
			}
			localOffset = body.centerOfMass + base.transform.InverseTransformVector(offset);
		}
		body.centerOfMass = localOffset;
		body.inertiaTensor = Vector3.Scale(originalInertia, inertiaMultiplier);
	}

	public void OnDrawGizmosSelected()
	{
		if (Application.isPlaying)
		{
			Gizmos.DrawSphere(GetComponent<Rigidbody>().worldCenterOfMass, 0.05f);
			if (!SeenString1)
			{
				if (showdebug)
				{
					Debug.Log(base.name + " Application is playing , draw the gizmo ");
				}
				SeenString1 = true;
			}
			return;
		}
		if (localOffset == Vector3.zero && offset != Vector3.zero)
		{
			if (!SeenString3)
			{
				if (showdebug)
				{
					Debug.Log(base.name + " Local offset is zero and offset is not zero ");
				}
				SeenString3 = true;
			}
			Gizmos.DrawSphere(GetComponent<Rigidbody>().worldCenterOfMass + offset, 0.05f);
			return;
		}
		if (!SeenString2)
		{
			if (showdebug)
			{
				Debug.Log(base.name + " Local offset is not zero and offset is zero ");
			}
			SeenString2 = true;
		}
		Gizmos.DrawSphere(base.transform.TransformPoint(localOffset), 0.05f);
	}

	public void ResetCG()
	{
		if (showdebug)
		{
			Debug.Log(base.name + " Trying to reset the CG effect ");
		}
		if (!body)
		{
			Debug.Log(base.name + " Body is not set ");
		}
		if (!(body == null))
		{
			body.centerOfMass = originalCG;
			body.inertiaTensor = originalInertia;
		}
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		if (showdebug)
		{
			Debug.Log(base.name + " Trying to reset the state of the effect ");
		}
		if (!(body == null))
		{
			body.centerOfMass = localOffset;
			body.inertiaTensor = Vector3.Scale(originalInertia, inertiaMultiplier);
		}
	}
}
