using UnityEngine;

public class CounterweightStretch : MonoBehaviour
{
	public Transform target;

	private void Start()
	{
	}

	private void Update()
	{
		Vector3 position = target.transform.position;
		float y = position.y;
		Vector3 position2 = base.transform.position;
		float y2 = (y - position2.y) / 2f;
		base.transform.localScale = new Vector3(1f, y2, 1f);
	}
}
