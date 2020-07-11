using HumanAPI;
using Multiplayer;
using System.Collections.Generic;
using UnityEngine;

public class ColliderLabelTriggerVolume : Node, INetBehavior, IReset
{
	public NodeOutput output;

	[Tooltip("Labels that will trigger this volume")]
	public string[] labelsToCheckFor = new string[1]
	{
		"Default"
	};

	[Tooltip("Value to output to the graph when triggered")]
	public float outputValueInside = 1f;

	[Tooltip("Value to output when not triggered")]
	public float outputValueOutside;

	[Tooltip("Array of gameobject to activate on entry")]
	public GameObject[] activateOnEnter;

	[Tooltip("Array of gameobjects to deactivate on exit")]
	public GameObject[] deactivateOnExit;

	public bool runExitLogicAtStartup;

	[Tooltip("If this is true, colliders are tracked as they enter/leave the volume and volume will deactivate only if no colliders are inside. Otherwise it will deactivate when any collider leaves even if others remain in the volume.")]
	public bool trackColliders = true;

	[Tooltip("Will lot info on collisions")]
	public bool debugOutput;

	private List<Collider> colliders = new List<Collider>();

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
			colliders.Add(other);
			if (colliders.Count > 1)
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
			colliders.Remove(other);
			if (colliders.Count != 0)
			{
				return;
			}
		}
		SetExitObjectState();
	}

	private void Update()
	{
		if (trackColliders)
		{
			foreach (Collider collider in colliders)
			{
				if (collider == null || !collider.gameObject.activeSelf)
				{
					colliders.Remove(collider);
					if (colliders.Count == 0)
					{
						SetExitObjectState();
					}
					break;
				}
			}
		}
	}

	private bool IsCorrectCollider(Collider other)
	{
		if (labelsToCheckFor.Length != 0)
		{
			ColliderLabel component = other.GetComponent<ColliderLabel>();
			if ((bool)component)
			{
				string[] array = labelsToCheckFor;
				foreach (string b in array)
				{
					if (component.Label == b)
					{
						if (debugOutput)
						{
							Debug.Log("Correct collider found: " + other.name);
						}
						return true;
					}
					if (debugOutput)
					{
						Debug.Log("Other collider label does not match: " + component.Label);
					}
				}
			}
			else if (debugOutput)
			{
				Debug.Log("Other collider does not have a label: " + other.name);
			}
		}
		return false;
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
		colliders = new List<Collider>();
		if (output != null)
		{
			output.SetValue(outputValueOutside);
		}
	}
}
