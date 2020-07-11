using UnityEngine;

namespace HumanAPI.LightLevel
{
	public class ConvexLens : MirrorReflection
	{
		public float focalLength = 10f;

		protected override LightBase CreateLight(Vector3 pos, Vector3 dir)
		{
			LightBeamConvex lightBeamConvex = LightPool.Create<LightBeamConvex>(pos, dir, base.transform);
			lightBeamConvex.focalLength = focalLength;
			return lightBeamConvex;
		}

		protected override Vector3 GetDirection(LightHitInfo info)
		{
			Vector3 vector = base.Direction;
			if (Vector3.Dot(info.source.Direction, vector) < 0f)
			{
				vector = -vector;
			}
			return vector;
		}
	}
}
