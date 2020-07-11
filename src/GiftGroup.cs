using System.Collections.Generic;
using UnityEngine;

public class GiftGroup : MonoBehaviour
{
	private GiftSpawn[] spawns;

	private List<float> spawnProbabilityMap = new List<float>();

	private float spawnProbabilitySum;

	private void Awake()
	{
		spawns = GetComponentsInChildren<GiftSpawn>();
		spawnProbabilityMap.Clear();
		spawnProbabilitySum = 0f;
		for (int i = 0; i < spawns.Length; i++)
		{
			spawnProbabilitySum += spawns[i].probability;
			spawnProbabilityMap.Add(spawnProbabilitySum);
		}
	}

	public GiftSpawn GetRandomSpawn()
	{
		float num = Random.Range(0f, spawnProbabilitySum);
		for (int i = 0; i < spawns.Length; i++)
		{
			if (num < spawnProbabilityMap[i])
			{
				return spawns[i];
			}
		}
		return spawns[0];
	}
}
