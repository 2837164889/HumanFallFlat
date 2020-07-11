using UnityEngine;

public class HintTriggerOff : MonoBehaviour
{
	public Hint hint;

	public Collider acceptedCollider;

	public bool requireEnter;

	public void OnTriggerEnter(Collider other)
	{
		if ((!requireEnter || hint.wasActivated) && (acceptedCollider == null || other == acceptedCollider))
		{
			hint.StopHint();
		}
	}
}
