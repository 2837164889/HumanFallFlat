using Multiplayer;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class OptimisationVolume : MonoBehaviour
{
	[Tooltip("Array of game objects to deactivate the renderers of when no local player is in this zone")]
	public GameObject[] deactivateRenderers;

	public Light[] deactivateLights;

	public List<GameObject> exemptObjects;

	private List<GameObject> playersinVolume;

	private List<Renderer> exemptRenderers;

	private bool isActive;

	private BoxCollider boxCollider;

	private bool setupDone;

	private void Awake()
	{
		playersinVolume = new List<GameObject>();
		exemptRenderers = new List<Renderer>();
		boxCollider = GetComponent<BoxCollider>();
		if (boxCollider == null)
		{
			Debug.LogError("OptimisationVolume error, no BoxCollider, disabling");
			base.enabled = false;
		}
		foreach (GameObject exemptObject in exemptObjects)
		{
			Renderer[] componentsInChildren = exemptObject.GetComponentsInChildren<Renderer>();
			exemptRenderers.AddRange(componentsInChildren);
		}
		SetExitObjectState();
		setupDone = true;
	}

	private void FixedUpdate()
	{
		if (!setupDone)
		{
			return;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (boxCollider.bounds.Contains(gameObject.transform.position))
			{
				PlayerEnter(gameObject);
			}
			else
			{
				PlayerExit(gameObject);
			}
		}
	}

	private void PlayerEnter(GameObject player)
	{
		if (IsLocalPlayer(player) && !playersinVolume.Contains(player))
		{
			playersinVolume.Add(player);
			if (playersinVolume.Count <= 1)
			{
				SetEnterObjectState();
			}
		}
	}

	private void PlayerExit(GameObject player)
	{
		if (IsLocalPlayer(player) && playersinVolume.Contains(player))
		{
			playersinVolume.Remove(player);
			if (playersinVolume.Count == 0)
			{
				SetExitObjectState();
			}
		}
	}

	private bool IsLocalPlayer(GameObject player)
	{
		return (bool)player.transform.parent.GetComponent<NetPlayer>() && player.transform.parent.GetComponent<NetPlayer>().isLocalPlayer;
	}

	private void SetEnterObjectState()
	{
		isActive = true;
		SetRenderersVisible(visible: true);
	}

	private void SetExitObjectState()
	{
		isActive = false;
		SetRenderersVisible(visible: false);
	}

	private void SetRenderersVisible(bool visible)
	{
		GameObject[] array = deactivateRenderers;
		foreach (GameObject gameObject in array)
		{
			if (!(gameObject != null))
			{
				continue;
			}
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
			Renderer[] array2 = componentsInChildren;
			foreach (Renderer renderer in array2)
			{
				if (renderer.gameObject.name == "Stick")
				{
					bool flag = false;
					flag = !flag;
				}
				if (!exemptRenderers.Contains(renderer))
				{
					renderer.enabled = visible;
				}
			}
		}
		Light[] array3 = deactivateLights;
		foreach (Light light in array3)
		{
			if (light != null)
			{
				light.gameObject.SetActive(visible);
			}
		}
	}
}
