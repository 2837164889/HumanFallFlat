using HumanAPI;
using Multiplayer;
using UnityEngine;

public class TriggerVolume : Node, INetBehavior, IReset
{
	public NodeOutput output;

	[Tooltip("Collider that will trigger this volume. If blank, any player will trigger.")]
	public Collider colliderToCheckFor;

	[Tooltip("Any gameobject that's a child of an entry in this list will also trigger output.")]
	public GameObject[] additionalColliders;

	[Tooltip("Value to output to the graph when players are inside the volume")]
	public float outputValueInside = 1f;

	[Tooltip("Value to output when the player is outside the volume")]
	public float outputValueOutside;

	[Tooltip("Array of gameobject to activate on entry")]
	public GameObject[] activateOnEnter;

	[Tooltip("Array of gameobjects to deactivate on exit")]
	public GameObject[] deactivateOnExit;

	public bool runExitLogicAtStartup;

	[Tooltip("If this is true, colliders are tracked as they enter/leave the volume and volume will deactivate only if no colliders are inside. Otherwise it will deactivate when any collider leaves even if others remain in the volume.")]
	public bool trackColliders = true;

	private int colliderCount;

	private bool isActive;

	private void Start()
	{
		if (output != null)
		{
			output.SetValue(outputValueOutside);
		}
		if (runExitLogicAtStartup)
		{
			SetExitObjectState();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!IsCorrectCollider(other))
		{
			return;
		}
		if (trackColliders)
		{
			colliderCount++;
			if (colliderCount > 1)
			{
				return;
			}
		}
		SetEnterObjectState();
	}

	private void OnTriggerExit(Collider other)
	{
		if (!IsCorrectCollider(other))
		{
			return;
		}
		if (trackColliders)
		{
			colliderCount--;
			if (colliderCount != 0)
			{
				return;
			}
		}
		SetExitObjectState();
	}

	private bool IsCorrectCollider(Collider other)
	{
		bool result = false;
		if (colliderToCheckFor == null && additionalColliders.Length == 0)
		{
			foreach (Human item in Human.all)
			{
				if (item.GetComponent<Collider>() == other)
				{
					return true;
				}
			}
			return result;
		}
		result = (other == colliderToCheckFor && colliderToCheckFor != null);
		if (!result)
		{
			Transform transform = other.gameObject.transform;
			if (additionalColliders.Length != 0)
			{
				GameObject[] array = additionalColliders;
				foreach (GameObject gameObject in array)
				{
					if (transform.IsChildOf(gameObject.transform))
					{
						result = true;
						break;
					}
				}
			}
		}
		return result;
	}

	private void SetEnterObjectState()
	{
		isActive = true;
		if (output != null)
		{
			output.SetValue(outputValueInside);
		}
		GameObject[] array = activateOnEnter;
		foreach (GameObject gameObject in array)
		{
			if (gameObject == null)
			{
				Debug.LogError("Trigger Volume error @ Object " + base.gameObject.name);
			}
			else
			{
				gameObject.SetActive(value: true);
			}
		}
	}

	private void SetExitObjectState()
	{
		isActive = false;
		if (output != null)
		{
			output.SetValue(outputValueOutside);
		}
		GameObject[] array = deactivateOnExit;
		foreach (GameObject gameObject in array)
		{
			if (gameObject == null)
			{
				Debug.LogError("Trigger Volume error @ Object " + base.gameObject.name);
			}
			else
			{
				gameObject.SetActive(value: false);
			}
		}
	}

	public void CollectState(NetStream stream)
	{
		NetBoolEncoder.CollectState(stream, isActive);
	}

	private void ApplyState(bool state)
	{
		if (state && !isActive)
		{
			SetEnterObjectState();
		}
		else if (!state && isActive)
		{
			SetEnterObjectState();
		}
	}

	public void ApplyState(NetStream state)
	{
		ApplyState(NetBoolEncoder.ApplyState(state));
	}

	public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
	{
		ApplyState(NetBoolEncoder.ApplyLerpedState(state0, state1, mix));
	}

	public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
	{
		NetBoolEncoder.CalculateDelta(state0, state1, delta);
	}

	public void AddDelta(NetStream state0, NetStream delta, NetStream result)
	{
		NetBoolEncoder.AddDelta(state0, delta, result);
	}

	public int CalculateMaxDeltaSizeInBits()
	{
		return NetBoolEncoder.CalculateMaxDeltaSizeInBits();
	}

	public void StartNetwork(NetIdentity identity)
	{
	}

	public void SetMaster(bool isMaster)
	{
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		if (output != null)
		{
			output.SetValue(outputValueOutside);
		}
		colliderCount = 0;
	}
}
