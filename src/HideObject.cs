using UnityEngine;

public class HideObject : MonoBehaviour
{
	public Vector3 Translation;

	public void Hide()
	{
		Rigidbody component = GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = true;
		}
		base.gameObject.SetActive(value: false);
		base.transform.localPosition += Translation;
	}

	public void Show()
	{
		base.gameObject.SetActive(value: true);
	}
}
