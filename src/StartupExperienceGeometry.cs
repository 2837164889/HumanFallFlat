using HumanAPI;
using UnityEngine;

public class StartupExperienceGeometry : MonoBehaviour
{
	[Tooltip("Var to store the left half of the door")]
	public Rigidbody left;

	[Tooltip("Var to store the right half of thr door")]
	public Rigidbody right;

	[Tooltip("Var to store the sound the door should make when opened")]
	public Sound2 doorRelease;

	[Tooltip("Var to store an Audio source for the door opening")]
	public AudioSource doorReleaseSample;

	[Tooltip("Use this in order to show the prints coming from the script")]
	public bool showDebug;

	public void ReleaseDoor()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Release the doors ");
		}
		Vector3 position = Human.all[0].ragdoll.partWaist.transform.position;
		Vector3 position2 = base.transform.position;
		position.y = position2.y;
		base.transform.position = position;
		left.isKinematic = false;
		right.isKinematic = false;
		doorRelease.Play();
		doorRelease.transform.SetParent(null, worldPositionStays: false);
		Object.DontDestroyOnLoad(doorRelease);
		Object.Destroy(doorRelease, 5f);
	}
}
