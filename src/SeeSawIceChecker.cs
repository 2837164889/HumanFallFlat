using HumanAPI.LightLevel;
using UnityEngine;

public class SeeSawIceChecker : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (SeeSawAchievement.instance != null && collision.gameObject.GetComponentInParent<MeltingObject>() != null)
		{
			SeeSawAchievement.instance.Fail();
		}
	}
}
