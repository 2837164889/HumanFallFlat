using HumanAPI;
using UnityEngine;

public class FootCollisionAudioSensor : CollisionAudioSensor
{
	private Human human;

	protected override void OnEnable()
	{
		base.OnEnable();
		human = GetComponentInParent<Human>();
	}

	protected override bool ReportCollision(SurfaceType surf1, SurfaceType surf2, Vector3 pos, float impulse, float normalVelocity, float tangentVelocity, float volume, float pitch)
	{
		if (human == null)
		{
			return false;
		}
		float value = Mathf.Min(human.controls.walkSpeed * 2f + 0.5f, human.velocity.ZeroY().magnitude);
		pitch = ((surf2 != SurfaceType.Gravel) ? (pitch * (Map(value, 0f, 2f, 0.9f, 1f) * Random.Range(0.95f, 1.1f))) : (pitch * (Map(value, 0f, 2f, 0.5f, 1f) * Random.Range(0.95f, 1.1f))));
		volume *= Random.Range(0.9f, 1.1f);
		return base.ReportCollision(surf1, surf2, pos, impulse, normalVelocity, tangentVelocity, volume, pitch);
	}

	public static float Map(float value, float sourceFrom, float sourceTo, float targetFrom, float targetTo)
	{
		if (value < sourceFrom)
		{
			value = 0f;
		}
		return Mathf.Lerp(targetFrom, targetTo, Mathf.InverseLerp(sourceFrom, sourceTo, value));
	}
}
