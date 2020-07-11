using HumanAPI;
using UnityEngine;

public class CollisionAudioSurfSurfConfig : MonoBehaviour
{
	public SurfaceType surf1;

	public SurfaceType surf2;

	public bool isDefault;

	public CollisionAudioSurfSurfConfig link;

	public CollisionAudioSurfSurfConfig mirror;

	public CollisionAudioHitConfig hit = new CollisionAudioHitConfig();

	public CollisionAudioHitConfig slide = new CollisionAudioHitConfig();

	public CollisionAudioHitMonitor hitMonitor = new CollisionAudioHitMonitor();

	public CollisionAudioHitMonitor slideMonitor = new CollisionAudioHitMonitor();

	public float levelDB;

	public float slideTreshold;

	public float lastSlideAmount;

	public CollisionAudioSurfSurfConfig controlConfig
	{
		get
		{
			if (mirror != null)
			{
				return mirror.controlConfig;
			}
			if (link != null)
			{
				return link.controlConfig;
			}
			return this;
		}
	}

	public CollisionAudioSurfSurfConfig monitorConfig
	{
		get
		{
			if (mirror != null)
			{
				return mirror.monitorConfig;
			}
			return this;
		}
	}

	public bool PlayImpact(CollisionAudioSensor sensor, AudioChannel channel, Vector3 pos, float impulse, float normalVelocity, float tangentVelocity, float volume, float pitch)
	{
		float num = 0f;
		if (controlConfig.slide != null && controlConfig.slide.sampleLib != null && tangentVelocity != 0f)
		{
			Vector2 normalized = new Vector2(normalVelocity, tangentVelocity).normalized;
			num = normalized.y;
		}
		monitorConfig.lastSlideAmount = num;
		if (num > controlConfig.slideTreshold)
		{
			return controlConfig.slide.Play(sensor, channel, monitorConfig.slideMonitor, pos, impulse, tangentVelocity, volume, pitch);
		}
		return controlConfig.hit.Play(sensor, channel, monitorConfig.hitMonitor, pos, impulse, normalVelocity, volume, pitch);
	}
}
