using System.Collections;
using UnityEngine;

public class SetRigidRotation : MonoBehaviour
{
	public Vector3 euler;

	private IEnumerator Start()
	{
		yield return null;
		GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(euler));
	}
}
