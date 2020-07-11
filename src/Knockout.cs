using UnityEngine;

public class Knockout : MonoBehaviour
{
	[Tooltip("Use this in order to show the prints coming from the script")]
	public bool showDebug;

	public void OnCollisionEnter(Collision collision)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Entered Collision ");
		}
		HandleCollision(collision);
	}

	public void OnCollisionStay(Collision collision)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Stayed inside collision ");
		}
		HandleCollision(collision);
	}

	private void HandleCollision(Collision collision)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Handling Collision ");
		}
		if (collision.contacts.Length != 0)
		{
			CollisionSensor component = collision.transform.gameObject.GetComponent<CollisionSensor>();
			if (!(component == null) && component.knockdown != 0f && !component.human.grabManager.IsGrabbed(base.gameObject))
			{
				component.human.ReceiveHit(-collision.GetImpulse() / collision.rigidbody.mass * component.knockdown);
			}
		}
	}
}
