using System.Collections.Generic;
using UnityEngine;

public class InsideIceCaves : MonoBehaviour
{
	private const int kMaxSnowballs = 4;

	public static InsideIceCaves instance;

	private static List<SnowBallGrowth> snowballsInside = new List<SnowBallGrowth>(4);

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	public bool SnowballInsideCave(SnowBallGrowth snowball)
	{
		return snowballsInside.Contains(snowball);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.isTrigger)
		{
			SnowBallGrowth component = other.gameObject.GetComponent<SnowBallGrowth>();
			if (component != null)
			{
				snowballsInside.Add(component);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!other.isTrigger)
		{
			SnowBallGrowth component = other.gameObject.GetComponent<SnowBallGrowth>();
			if (component != null)
			{
				snowballsInside.Remove(component);
			}
		}
	}
}
