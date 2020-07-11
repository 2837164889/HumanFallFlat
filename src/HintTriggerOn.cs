using UnityEngine;

public class HintTriggerOn : MonoBehaviour
{
	public Hint hint;

	public Collider acceptedCollider;

	public void OnTriggerEnter(Collider other)
	{
		if (acceptedCollider == null || other == acceptedCollider)
		{
			hint.TriggerHint();
		}
	}
}
