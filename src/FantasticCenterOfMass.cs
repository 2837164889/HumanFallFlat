using UnityEngine;

public class FantasticCenterOfMass : MonoBehaviour
{
	[SerializeField]
	private Transform COMObject;

	private void Start()
	{
		if ((bool)GetComponent<Rigidbody>())
		{
			Rigidbody component = GetComponent<Rigidbody>();
			component.centerOfMass = COMObject.localPosition;
		}
	}
}
