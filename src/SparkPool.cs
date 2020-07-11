using System.Collections.Generic;
using UnityEngine;

public class SparkPool : MonoBehaviour
{
	public static SparkPool instance;

	public GameObject template;

	public int minItems;

	public int maxItems = 50;

	private Queue<GameObject> available = new Queue<GameObject>();

	private void OnEnable()
	{
		if (instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		instance = this;
		for (int i = 1; i < minItems; i++)
		{
			GameObject gameObject = Object.Instantiate(template);
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			available.Enqueue(gameObject);
		}
	}

	public void Emit(Vector3 pos, float speedMult = 3f)
	{
		Spark component;
		if (available.Count > 0)
		{
			GameObject gameObject = available.Dequeue();
			component = gameObject.GetComponent<Spark>();
		}
		else
		{
			GameObject gameObject = Object.Instantiate(template);
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			component = gameObject.GetComponent<Spark>();
		}
		Vector3 speed = Random.insideUnitSphere * speedMult;
		component.Ignite(pos + Random.insideUnitSphere * 0.1f, speed);
	}

	internal void Return(Spark spark)
	{
		available.Enqueue(spark.gameObject);
	}

	public void Emit(int count, Vector3 position, float speedMult = 3f)
	{
		for (int i = 0; i < count; i++)
		{
			Emit(position, speedMult);
		}
	}
}
