using System.Collections.Generic;
using UnityEngine;

public class Current : MonoBehaviour
{
	public float radius = 10f;

	public float weight = 1f;

	public float power = 1f;

	public Vector3 flow;

	public Vector3 globalFlow;

	private static List<Current> all = new List<Current>();

	private static Current[] allArray;

	private void OnEnable()
	{
		all.Add(this);
		globalFlow = base.transform.TransformVector(flow);
		allArray = null;
	}

	private void OnDisable()
	{
		all.Remove(this);
		allArray = null;
	}

	public static Vector3 Sample(Vector3 pos, out float weight)
	{
		if (allArray == null)
		{
			allArray = all.ToArray();
		}
		weight = 0f;
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < allArray.Length; i++)
		{
			Current current = allArray[i];
			float num = 1f - (pos - current.transform.position).magnitude / current.radius;
			if (!(num < 0f))
			{
				num = Mathf.Pow(num, current.power);
				weight += num;
				zero += num * current.globalFlow;
			}
		}
		if (weight > 1f)
		{
			zero /= weight;
			weight = Mathf.Clamp01(weight);
		}
		return zero;
	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(base.transform.position, radius);
		Gizmos.DrawRay(base.transform.position, base.transform.TransformVector(flow));
	}
}
