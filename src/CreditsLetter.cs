using UnityEngine;

public class CreditsLetter : MonoBehaviour
{
	public float width = 1f;

	public char character = 'a';

	public void OnCollisionEnter(Collision collision)
	{
		GetComponent<Rigidbody>().isKinematic = false;
	}

	internal void Attach(CreditsBlock creditsBlock, Vector3 offset)
	{
		base.transform.localPosition += offset;
		base.transform.localRotation = Quaternion.Euler(-90f, 180f, 0f);
		GetComponent<Rigidbody>().isKinematic = true;
		base.gameObject.SetActive(value: true);
	}
}
