using UnityEngine;

public class FishBox : MonoBehaviour
{
	public bool hasFishOnIt;

	private void OnCollisionStay(Collision collision)
	{
		if (collision.collider.name == "FishMesh")
		{
			hasFishOnIt = true;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.name == "FishMesh")
		{
			hasFishOnIt = true;
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (collision.collider.name == "FishMesh")
		{
			hasFishOnIt = false;
		}
	}

	private void Update()
	{
		if (hasFishOnIt)
		{
			Debug.Log("<color=green> hasFishOnIt = </color>" + hasFishOnIt);
		}
	}
}
