using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI.LightLevel
{
	public class LightHitInfo
	{
		public LightBase source;

		public Vector3 contactPoint;

		public List<LightBase> outputs;

		public float intensity = -1f;

		private LightHitInfo()
		{
		}

		public LightHitInfo(LightBase s)
		{
			source = s;
			outputs = new List<LightBase>();
		}

		public LightHitInfo(LightBase s, float intensity)
		{
			this.intensity = intensity;
			source = s;
			outputs = new List<LightBase>();
		}
	}
}
