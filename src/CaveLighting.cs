using Multiplayer;
using System.Collections.Generic;
using UnityEngine;

public class CaveLighting : MonoBehaviour, IReset
{
	public Light mainLight;

	public Color caveFogColor;

	public Color ambientColor;

	public ReflectionProbe caveProbe;

	private CaveAmbientSource[] ambientSources;

	private CaveShadowDisable[] shadows;

	private float defaultShadowNormalBias;

	private Dictionary<NetPlayer, float> phases = new Dictionary<NetPlayer, float>();

	private static List<CaveLighting> all = new List<CaveLighting>();

	private void OnEnable()
	{
		ambientSources = GetComponentsInChildren<CaveAmbientSource>();
		shadows = GetComponentsInChildren<CaveShadowDisable>();
		defaultShadowNormalBias = mainLight.shadowNormalBias;
		all.Add(this);
	}

	private void OnDisable()
	{
		all.Remove(this);
	}

	public void SetPhase(NetPlayer player, float newPhase)
	{
		if (newPhase > 0f)
		{
			phases[player] = newPhase;
		}
		else
		{
			phases.Remove(player);
		}
	}

	public static CaveLighting GetCaveForPlayer(NetPlayer player)
	{
		for (int i = 0; i < all.Count; i++)
		{
			if (all[i].phases.ContainsKey(player))
			{
				return all[i];
			}
		}
		return null;
	}

	public float GetPlaseForPlayer(NetPlayer player)
	{
		if (!phases.ContainsKey(player))
		{
			return 0f;
		}
		return phases[player];
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		phases.Clear();
	}
}
