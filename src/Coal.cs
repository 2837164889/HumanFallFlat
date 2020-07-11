using Multiplayer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coal : MonoBehaviour, INetBehavior
{
	public Color coldColor = Color.red;

	public Color hotColor = Color.yellow;

	public Color emitColor = Color.yellow;

	public float emitIntensity = 2f;

	public float heatUpTime = 1f;

	public float coolDownTime = 3f;

	private float glowFrequency = 6f;

	private float colorFrequency = 6f;

	private float heat = 1E-06f;

	private float targetHeat;

	private Material material;

	private static Coroutine update;

	private static List<Coal> all = new List<Coal>();

	private void OnEnable()
	{
		all.Add(this);
		if (update == null)
		{
			update = Coroutines.StartGlobalCoroutine(ProcessUpdates());
		}
	}

	private void OnDisable()
	{
		all.Remove(this);
	}

	private void Awake()
	{
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
			}
			material.SetColor("_EmissionColor", emitColor * b);
			material.color = color;
		}
	}

	internal void Ignite()
	{
		targetHeat = 0.9f;
	}

	internal void Extinguish(bool instant = false)
	{
		targetHeat = 0f;
		if (instant)
		{
			heat = 0.01f;
		}
	}

	public void StartNetwork(NetIdentity identity)
	{
	}

	public void CollectState(NetStream stream)
	{
		NetBoolEncoder.CollectState(stream, targetHeat > 0f);
	}

	public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
	{
		targetHeat = ((!NetBoolEncoder.ApplyLerpedState(state0, state1, mix)) ? 0f : 0.9f);
	}

	public void ApplyState(NetStream state)
	{
		targetHeat = ((!NetBoolEncoder.ApplyState(state)) ? 0f : 0.9f);
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

	public void SetMaster(bool isMaster)
	{
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
	}
}
