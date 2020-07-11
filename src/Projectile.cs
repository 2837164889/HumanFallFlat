using UnityEngine;

public class Projectile : MonoBehaviour, IReset
{
	public float explodeForce = 1000f;

	private bool explode;

	private bool exploded;

	public void OnCollisionEnter(Collision collision)
	{
		if (!exploded && string.Equals(collision.gameObject.tag, "Explodable"))
		{
			explode = true;
		}
	}

	private void FixedUpdate()
	{
		if (!explode)
		{
			return;
		}
		exploded = true;
		explode = false;
		Collider[] array = Physics.OverlapSphere(base.transform.position, 3f);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (string.Equals(collider.gameObject.tag, "Explodable"))
			{
				Rigidbody component = collider.GetComponent<Rigidbody>();
				if (!(component == null))
				{
					component.AddExplosionForce(explodeForce, base.transform.position, 3f, 0.5f, ForceMode.Impulse);
				}
			}
		}
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		explode = (exploded = false);
	}
}
