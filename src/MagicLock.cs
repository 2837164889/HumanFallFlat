using UnityEngine;

public class MagicLock : MonoBehaviour
{
	[Tooltip("The Magic key we need to open the lock")]
	public MagicKey magicKey;

	[Tooltip("The game object to diable when the lock is broken")]
	public GameObject objectToDisable;

	[Tooltip("Forced used by the magnet")]
	public float attractionForce;

	private bool magnetActive;

	private Rigidbody keyBody;

	[Tooltip("Use this in order to show the prints coming from the script")]
	public bool showDebug;

	private void Start()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Started ");
		}
		keyBody = magicKey.GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		if (magnetActive && magicKey.isPoweredUp)
		{
			Vector3 force = (base.transform.position - magicKey.transform.position).normalized * attractionForce;
			keyBody.AddForce(force);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == magicKey.gameObject)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Magic Key Entered range ");
			}
			magnetActive = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject == magicKey.gameObject)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Magic Key Lef Range ");
			}
			magnetActive = false;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject == magicKey.gameObject && magicKey.isPoweredUp)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Magic key powered and unlocked me ");
			}
			objectToDisable.SetActive(value: false);
		}
	}
}
