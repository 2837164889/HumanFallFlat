using Multiplayer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammableScriptHeatColourChange1 : MonoBehaviour, INetBehavior
{
	[Tooltip("How Red you would like the coal to get in colour")]
	public Color coldColor = Color.red;

	[Tooltip("How Rellow you would like the coal to get in colour")]
	public Color hotColor = Color.yellow;

	[Tooltip("What Yellow colour you would like to emit")]
	public Color emitColor = Color.yellow;

	[Tooltip("How intense the emittion should be")]
	public float emitIntensity = 2f;

	[Tooltip(" How long it will take this thing to heat up")]
	public float heatUpTime = 1f;

	[Tooltip("How long it will take for this thing to cool down")]
	public float coolDownTime = 3f;

	private Flammable flammbleSettings;

	private float glowFrequency = 6f;

	private float colorFrequency = 6f;

	private float heat = 1E-06f;

	private float targetHeat;

	private Material material;

	[Tooltip("Use this to see the messages coming from the script ")]
	public bool showDebug;

	private static Coroutine update;

	private static List<FlammableScriptHeatColourChange1> all = new List<FlammableScriptHeatColourChange1>();

	private void OnEnable()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " OnEnable ");
		}
		all.Add(this);
		if (update == null)
		{
			update = Coroutines.StartGlobalCoroutine(ProcessUpdates());
		}
		flammbleSettings = GetComponent<Flammable>();
	}

	private void OnDisable()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " onDisable ");
		}
		all.Remove(this);
	}

	private void Awake()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Awake ");
		}
		MeshRenderer component = GetComponent<MeshRenderer>();
		material = component.material;
		component.sharedMaterial = material;
		glowFrequency = UnityEngine.Random.Range(8, 25);
		colorFrequency = UnityEngine.Random.Range(5, 10);
		UpdateInternal();
	}

	public static IEnumerator ProcessUpdates()
	{
		while (all.Count > 0)
		{
			yield return null;
			for (int i = 0; i < all.Count; i++)
			{
				all[i].UpdateInternal();
			}
		}
		Coroutines.StopGlobalCoroutine(update);
		update = null;
	}

	private void UpdateInternal()
	{
		if (heat != targetHeat)
		{
			heat = Mathf.MoveTowards(heat, targetHeat, Time.deltaTime / ((!(targetHeat > heat)) ? coolDownTime : heatUpTime));
			Color color = Color.black;
			float b = 0f;
			if (heat > 0f)
			{
				b = heat * emitIntensity * (1f + 0.1f * Mathf.Sin(Time.time / glowFrequency * (float)Math.PI * 2f));
				color = heat * Color.Lerp(coldColor, hotColor, heat + 0.1f * Mathf.Sin(Time.time / colorFrequency * (float)Math.PI * 2f));
				flammbleSettings.heat = heat;
			}
			material.SetColor("_EmissionColor", emitColor * b);
			material.color = color;
		}
	}

	public void Ignite()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Ignite ");
		}
		targetHeat = 0.9f;
	}

	public void Extinguish()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Extinguish ");
		}
		targetHeat = 0f;
	}

	public void StartNetwork(NetIdentity identity)
	{
	}

	public void CollectState(NetStream stream)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " CollectState ");
		}
		NetBoolEncoder.CollectState(stream, targetHeat > 0f);
	}

	public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " ApplyLerpedState ");
		}
		targetHeat = ((!NetBoolEncoder.ApplyLerpedState(state0, state1, mix)) ? 0f : 0.9f);
	}

	public void ApplyState(NetStream state)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " ApplyState ");
		}
		targetHeat = ((!NetBoolEncoder.ApplyState(state)) ? 0f : 0.9f);
	}

	public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " CalculateDelta ");
		}
		NetBoolEncoder.CalculateDelta(state0, state1, delta);
	}

	public void AddDelta(NetStream state0, NetStream delta, NetStream result)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " AddDelta ");
		}
		NetBoolEncoder.AddDelta(state0, delta, result);
	}

	public int CalculateMaxDeltaSizeInBits()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " CalculateMaxDeltaSizeInBits ");
		}
		return NetBoolEncoder.CalculateMaxDeltaSizeInBits();
	}

	public void SetMaster(bool isMaster)
	{
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
	}
}
