using UnityEngine;

public class JointEnableFix : MonoBehaviour
{
	private Quaternion initialLocalRotation;

	private Vector3 initialLocalPosition;

	private Quaternion localRotationOnDisable;

	private Vector3 localPositionOnDisable;

	private bool hasDisabled;

	private void Awake()
	{
		initialLocalRotation = base.transform.localRotation;
		initialLocalPosition = base.transform.localPosition;
	}

	private void OnDisable()
	{
		localRotationOnDisable = base.transform.localRotation;
		base.transform.localRotation = initialLocalRotation;
		localPositionOnDisable = base.transform.localPosition;
		base.transform.localPosition = initialLocalPosition;
		hasDisabled = true;
	}

	private void OnEnable()
	{
		if (hasDisabled)
		{
			hasDisabled = false;
			base.transform.localRotation = localRotationOnDisable;
			base.transform.localPosition = localPositionOnDisable;
		}
	}
}
