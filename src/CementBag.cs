using System.Collections;
using UnityEngine;

public class CementBag : MonoBehaviour
{
	public float impactTreshold = 100f;

	public GameObject particlesPrefab;

	public float maxImpulse;

	public float explodeDelay = 1f;

	private float lastExplode;

	public bool ReportCollision(float impulse, Vector3 pos)
	{
		maxImpulse = Mathf.Max(maxImpulse, impulse);
		float time = Time.time;
		if (impulse > impactTreshold && time > lastExplode + explodeDelay)
		{
			lastExplode = time;
			StartCoroutine(Explode(pos));
			return true;
		}
		return false;
	}

	private IEnumerator Explode(Vector3 pos)
	{
		GameObject inst = Object.Instantiate(particlesPrefab, pos, Quaternion.identity);
		yield return new WaitForSeconds(1f);
		Object.Destroy(inst);
	}
}
