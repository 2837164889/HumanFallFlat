using UnityEngine;

namespace HumanAPI.LightLevel
{
	public class Sun : LightBase
	{
		public static Sun instance;

		public override Color color => Color.white;

		private void Awake()
		{
			instance = this;
		}

		public override Vector3 ClosestPoint(Vector3 point)
		{
			return point;
		}

		protected override void ReturnToPool()
		{
		}
	}
}
