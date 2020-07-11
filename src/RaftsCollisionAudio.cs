using System.Collections.Generic;
using UnityEngine;

public class RaftsCollisionAudio : MonoBehaviour
{
	[SerializeField]
	private GameObject soundMovingPrefab;

	private List<GameObject> rafters;

	private void Awake()
	{
		rafters = new List<GameObject>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!rafters.Contains(collision.collider.gameObject))
		{
			rafters.Add(collision.collider.gameObject);
			GameObject gameObject = Object.Instantiate(soundMovingPrefab);
			gameObject.transform.SetParent(collision.collider.gameObject.transform, worldPositionStays: false);
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		Transform transform = collision.collider.gameObject.transform.Find(soundMovingPrefab.name + "(Clone)");
		if (transform != null)
		{
			Object.Destroy(transform.gameObject);
			if (rafters.Contains(collision.collider.gameObject))
			{
				rafters.Remove(collision.collider.gameObject);
			}
		}
	}
}
