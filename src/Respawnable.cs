using Multiplayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawnable : MonoBehaviour
{
	private Vector3 startPos;

	private Quaternion startRot;

	public float despawnHeight = -50f;

	public float respawnHeight;

	private static Coroutine update;

	private static List<Respawnable> all = new List<Respawnable>();

	private void OnDisable()
	{
		all.Remove(this);
	}

	private void OnEnable()
	{
	}

	public static IEnumerator ProcessUpdates()
	{
		int throttle = 0;
		while (all.Count > 0)
		{
			yield return null;
			for (int i = 0; i < all.Count; i++)
			{
				if (throttle++ == 50)
				{
					throttle = 0;
					yield return null;
				}
				if (i < all.Count)
				{
					all[i].UpdateInternal();
				}
			}
		}
		Coroutines.StopGlobalCoroutine(update);
		update = null;
	}

	private void UpdateInternal()
	{
		Vector3 position = base.transform.position;
		if (position.y < despawnHeight)
		{
			Respawn();
		}
	}

	public void Respawn()
	{
		if (ReplayRecorder.isPlaying || NetGame.isClient || GrabManager.IsGrabbedAny(base.gameObject))
		{
			return;
		}
		RestartableRigid component = GetComponent<RestartableRigid>();
		if (component != null)
		{
			component.Reset(Vector3.up * respawnHeight);
			return;
		}
		base.transform.position = startPos + Vector3.up * respawnHeight;
		base.transform.rotation = startRot;
		Rigidbody component2 = GetComponent<Rigidbody>();
		if (component2 != null)
		{
			component2.velocity = Vector3.zero;
			component2.angularVelocity = Vector3.zero;
		}
	}
}
