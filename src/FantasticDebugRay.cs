using UnityEngine;

[ExecuteInEditMode]
public class FantasticDebugRay : MonoBehaviour
{
	[SerializeField]
	private bool local = true;

	[SerializeField]
	private Color color = Color.white;

	[SerializeField]
	private float length = 1f;

	private void Start()
	{
	}

	private void Update()
	{
		Vector3 forward = Vector3.forward;
		if (local)
		{
			forward = base.transform.forward;
		}
		Debug.DrawRay(base.transform.position, forward.normalized * length, color);
	}
}
