using UnityEngine;

public abstract class WaterBody : WaterBodyCollider
{
	public bool canDrown;

	public abstract float SampleDepth(Vector3 pos, out Vector3 velocity);
}
