using UnityEngine;

public class ShrinkColliderOnImpact : MonoBehaviour
{
	public new string tag;

	private Vector3 size;

	private BoxCollider collider;

	private float scale = 1f;

	private void OnEnable()
	{
		collider = GetComponent<BoxCollider>();
		size = collider.size;
	}

	public void OnCollisionEnter(Collision collision)
	{
	}

	private void FixedUpdate()
	{
		if (scale != 1f)
		{
			scale = Mathf.MoveTowards(scale, 1f, 0.01f);
			collider.size = size * scale;
		}
	}
}
