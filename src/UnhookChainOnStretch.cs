using UnityEngine;

public class UnhookChainOnStretch : MonoBehaviour
{
	public Transform chainRoot;

	public float maxLength;

	private void Update()
	{
		Vector3 vector = chainRoot.position - base.transform.position;
		if (vector.sqrMagnitude > maxLength * maxLength)
		{
			base.transform.position += vector.normalized * 0.2f;
		}
	}
}
