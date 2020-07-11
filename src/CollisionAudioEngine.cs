using HumanAPI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CollisionAudioEngine : MonoBehaviour
{
	public float minVelocity = 0.5f;

	public float minImpulse = 5f;

	public float hitDelay = 0.1f;

	public float unitImpulse = 100f;

	public float unitVelocity = 5f;

	public GameObject runtimeConfigs;

	public static CollisionAudioEngine instance;

	public Dictionary<SurfaceType, Dictionary<SurfaceType, CollisionAudioSurfSurfConfig>> map = new Dictionary<SurfaceType, Dictionary<SurfaceType, CollisionAudioSurfSurfConfig>>();

	public SurfaceType soloSurface;

	private Dictionary<ushort, CollisionAudioHitConfig> configIdMap = new Dictionary<ushort, CollisionAudioHitConfig>();

	private void OnEnable()
	{
		instance = this;
		RebuildMap();
	}

	public void RebuildMap()
	{
		instance = this;
		map.Clear();
		CollisionAudioSurfSurfConfig[] componentsInChildren = runtimeConfigs.GetComponentsInChildren<CollisionAudioSurfSurfConfig>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
		}
		Array values = Enum.GetValues(typeof(SurfaceType));
		for (int j = 0; j < values.Length; j++)
		{
			SurfaceType surfaceType = (SurfaceType)values.GetValue(j);
			if (surfaceType == SurfaceType.Unknown)
			{
				continue;
			}
			Dictionary<SurfaceType, CollisionAudioSurfSurfConfig> dictionary = new Dictionary<SurfaceType, CollisionAudioSurfSurfConfig>();
			map[surfaceType] = dictionary;
			for (int k = 0; k < values.Length; k++)
			{
				SurfaceType surfaceType2 = (SurfaceType)values.GetValue(k);
				if (surfaceType2 != 0)
				{
					dictionary[surfaceType2] = null;
				}
			}
		}
		ushort num = 0;
		configIdMap.Clear();
		CollisionAudioSurfSurfConfig[] componentsInChildren2 = GetComponentsInChildren<CollisionAudioSurfSurfConfig>();
		foreach (CollisionAudioSurfSurfConfig cfg in componentsInChildren2)
		{
			AddCofigToMap(cfg);
		}
		foreach (CollisionAudioSurfSurfConfig collisionAudioSurfSurfConfig in componentsInChildren2)
		{
			if (collisionAudioSurfSurfConfig.isDefault)
			{
				Dictionary<SurfaceType, CollisionAudioSurfSurfConfig> dictionary2 = map[collisionAudioSurfSurfConfig.surf1];
				foreach (SurfaceType item in new List<SurfaceType>(dictionary2.Keys))
				{
					if (dictionary2[item] != null)
					{
						CollisionAudioSurfSurfConfig collisionAudioSurfSurfConfig2 = dictionary2[item];
						if (collisionAudioSurfSurfConfig2.hit != null)
						{
							collisionAudioSurfSurfConfig2.hit.netId = num++;
							configIdMap[collisionAudioSurfSurfConfig2.hit.netId] = collisionAudioSurfSurfConfig2.hit;
						}
						if (collisionAudioSurfSurfConfig2.slide != null)
						{
							collisionAudioSurfSurfConfig2.slide.netId = num++;
							configIdMap[collisionAudioSurfSurfConfig2.slide.netId] = collisionAudioSurfSurfConfig2.slide;
						}
					}
					if (dictionary2[item] == null && item != SurfaceType.RagdollBall)
					{
						CollisionAudioSurfSurfConfig collisionAudioSurfSurfConfig3 = new GameObject().AddComponent<CollisionAudioSurfSurfConfig>();
						collisionAudioSurfSurfConfig3.surf1 = collisionAudioSurfSurfConfig.surf1;
						collisionAudioSurfSurfConfig3.surf2 = item;
						collisionAudioSurfSurfConfig3.link = collisionAudioSurfSurfConfig.controlConfig;
						collisionAudioSurfSurfConfig3.name = $"{collisionAudioSurfSurfConfig3.surf1}{collisionAudioSurfSurfConfig3.surf2}Cloned{collisionAudioSurfSurfConfig.name}";
						collisionAudioSurfSurfConfig3.transform.SetParent(runtimeConfigs.transform, worldPositionStays: false);
						AddCofigToMap(collisionAudioSurfSurfConfig3);
					}
				}
			}
		}
	}

	private void AddCofigToMap(CollisionAudioSurfSurfConfig cfg)
	{
		if (map[cfg.surf1][cfg.surf2] != null)
		{
			Debug.LogError("CollisionAudioSurfSurfConfig already defined for " + cfg.surf1 + " " + cfg.surf2, cfg);
		}
		map[cfg.surf1][cfg.surf2] = cfg;
		if (cfg.surf1 != cfg.surf2)
		{
			CollisionAudioSurfSurfConfig collisionAudioSurfSurfConfig = new GameObject().AddComponent<CollisionAudioSurfSurfConfig>();
			collisionAudioSurfSurfConfig.surf1 = cfg.surf2;
			collisionAudioSurfSurfConfig.surf2 = cfg.surf1;
			collisionAudioSurfSurfConfig.mirror = cfg;
			collisionAudioSurfSurfConfig.name = $"{collisionAudioSurfSurfConfig.surf1}{collisionAudioSurfSurfConfig.surf2}Mirror";
			collisionAudioSurfSurfConfig.transform.SetParent(runtimeConfigs.transform, worldPositionStays: false);
			map[cfg.surf2][cfg.surf1] = collisionAudioSurfSurfConfig;
		}
	}

	public CollisionAudioSurfSurfConfig Resolve(SurfaceType surf1, SurfaceType surf2)
	{
		if (map.TryGetValue(surf1, out Dictionary<SurfaceType, CollisionAudioSurfSurfConfig> value) && value.TryGetValue(surf2, out CollisionAudioSurfSurfConfig value2))
		{
			return value2;
		}
		return null;
	}

	public CollisionAudioHitConfig GetConfig(ushort libId)
	{
		configIdMap.TryGetValue(libId, out CollisionAudioHitConfig value);
		return value;
	}

	public CollisionAudioSurfSurfConfig ResolveFinal(SurfaceType surf1, SurfaceType surf2)
	{
		CollisionAudioSurfSurfConfig collisionAudioSurfSurfConfig = Resolve(surf1, surf2);
		if (collisionAudioSurfSurfConfig == null)
		{
			return null;
		}
		return collisionAudioSurfSurfConfig.controlConfig;
	}

	public bool ReportCollision(CollisionAudioSensor sensor, SurfaceType surf1, SurfaceType surf2, Vector3 pos, float impulse, float normalVelocity, float tangentVelocity, float volume, float pitch)
	{
		if (surf1 == SurfaceType.RagdollBall || surf2 == SurfaceType.RagdollBall)
		{
			return false;
		}
		if (soloSurface != 0 && surf1 != soloSurface && surf2 != soloSurface)
		{
			return false;
		}
		CollisionAudioSurfSurfConfig collisionAudioSurfSurfConfig = Resolve(surf1, surf2);
		if (collisionAudioSurfSurfConfig != null)
		{
			AudioChannel channel = AudioChannel.Physics;
			if (surf1 == SurfaceType.RagdollBody || surf2 == SurfaceType.RagdollBody)
			{
				channel = AudioChannel.Body;
			}
			else if (surf1 == SurfaceType.RagdollFeet || surf2 == SurfaceType.RagdollFeet)
			{
				channel = AudioChannel.Footsteps;
			}
			return collisionAudioSurfSurfConfig.PlayImpact(sensor, channel, pos, impulse, normalVelocity, tangentVelocity, volume, pitch);
		}
		return false;
	}
}
