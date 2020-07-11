using UnityEngine;

public class FantasticWind : MonoBehaviour
{
	[SerializeField]
	private Rigidbody[] blownObjects;

	[SerializeField]
	private int percentChance;

	[SerializeField]
	private Vector3 direction;

	[SerializeField]
	private int frameSkip = 10;

	[SerializeField]
	private float minForce;

	[SerializeField]
	private float maxForce;

	private int frame;

	private void Start()
	{
	}

	private void FixedUpdate()
	{
		if (frame == frameSkip)
		{
			frame = 0;
			Rigidbody[] array = blownObjects;
			foreach (Rigidbody rigidbody in array)
			{
				if (Random.Range(0, 100) < percentChance)
				{
					float d = Random.Range(minForce, maxForce);
					rigidbody.AddForce(Vector3.Normalize(direction) * d);
				}
			}
		}
		else
		{
			frame++;
		}
	}
}
