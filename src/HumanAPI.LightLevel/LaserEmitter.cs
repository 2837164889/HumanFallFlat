using UnityEngine;

namespace HumanAPI.LightLevel
{
	public class LaserEmitter : LightSource
	{
		public NodeOutput hit;

		public NodeInput reset;

		public override void Process()
		{
			base.Process();
			((LaserBeam)emittedLight).Callibrate();
		}

		public void Hit(bool value)
		{
			float num = value ? 1 : 0;
			if (hit.value != num)
			{
				hit.SetValue(num);
			}
		}

		protected override LightBase CreateLight()
		{
			LaserBeam laserBeam = LightPool.Create<LaserBeam>(new Ray(base.transform.position, Direction));
			laserBeam.source = this;
			return laserBeam;
		}
	}
}
