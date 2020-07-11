using HumanAPI;
using UnityEngine;

public class ScrewHingePlatform : Node
{
	[SerializeField]
	private Rigidbody rotableRigidbody;

	[SerializeField]
	private float localMaxY;

	[SerializeField]
	private float localMinY;

	[SerializeField]
	private float speed = 5f;

	[SerializeField]
	private float maxAngularVelocityY = 1f;

	public NodeInput grabState;

	public NodeInput isCollidingBottom;

	private Vector3 topLocalPosition;

	private Vector3 bottomLocalPosition;

	private Vector3 maxAngularVelocityVector;

	private Vector3 targetPosition;

	[Tooltip("Use this in order to show the prints coming from the script")]
	public bool showDebug;

	private void Start()
	{
		topLocalPosition = new Vector3(0f, localMaxY, 0f);
		bottomLocalPosition = new Vector3(0f, localMinY, 0f);
		maxAngularVelocityVector = new Vector3(0f, maxAngularVelocityY, 0f);
	}

	private void Update()
	{
		Vector3 localPosition = base.transform.localPosition;
		if (localPosition.y >= bottomLocalPosition.y + 0.1f && grabState.value < 1f && isCollidingBottom.value != 1f)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Falling , player has let go ");
			}
			rotableRigidbody.AddRelativeTorque(Vector3.up * 8000f, ForceMode.Force);
		}
		Vector3 angularVelocity = rotableRigidbody.angularVelocity;
		if (angularVelocity.y > maxAngularVelocityY)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Soinning too fast + ");
			}
			rotableRigidbody.angularVelocity = maxAngularVelocityVector;
		}
		else
		{
			Vector3 angularVelocity2 = rotableRigidbody.angularVelocity;
			if (angularVelocity2.y < 0f - maxAngularVelocityY)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Soinning too fast - ");
				}
				rotableRigidbody.angularVelocity = -maxAngularVelocityVector;
			}
		}
		Vector3 localPosition2 = base.transform.localPosition;
		if (localPosition2.y >= topLocalPosition.y - 0.05f)
		{
			Vector3 angularVelocity3 = rotableRigidbody.angularVelocity;
			if (angularVelocity3.y < 0f)
			{
				goto IL_01f9;
			}
		}
		Vector3 localPosition3 = base.transform.localPosition;
		if (localPosition3.y <= bottomLocalPosition.y + 0.05f)
		{
			Vector3 angularVelocity4 = rotableRigidbody.angularVelocity;
			if (angularVelocity4.y > 0f)
			{
				goto IL_01f9;
			}
		}
		Vector3 angularVelocity5 = rotableRigidbody.angularVelocity;
		if (!(angularVelocity5.y > 0f) || isCollidingBottom.value != 1f)
		{
			Vector3 angularVelocity6 = rotableRigidbody.angularVelocity;
			if (angularVelocity6.y > 0f)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Going Down");
				}
				Transform transform = base.transform;
				Vector3 localPosition4 = base.transform.localPosition;
				Vector3 target = bottomLocalPosition;
				float num = Time.fixedDeltaTime * speed;
				Vector3 angularVelocity7 = rotableRigidbody.angularVelocity;
				transform.localPosition = Vector3.MoveTowards(localPosition4, target, num * Mathf.Abs(angularVelocity7.y));
				return;
			}
			Vector3 angularVelocity8 = rotableRigidbody.angularVelocity;
			if (angularVelocity8.y < 0f)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Going Up ");
				}
				Transform transform2 = base.transform;
				Vector3 localPosition5 = base.transform.localPosition;
				Vector3 target2 = topLocalPosition;
				float num2 = Time.fixedDeltaTime * speed;
				Vector3 angularVelocity9 = rotableRigidbody.angularVelocity;
				transform2.localPosition = Vector3.MoveTowards(localPosition5, target2, num2 * Mathf.Abs(angularVelocity9.y));
			}
			return;
		}
		goto IL_01f9;
		IL_01f9:
		if (showDebug)
		{
			Debug.Log(base.name + " Sector D ");
		}
		rotableRigidbody.angularVelocity = Vector3.zero;
	}
}
