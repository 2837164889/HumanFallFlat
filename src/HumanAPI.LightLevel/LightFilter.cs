using System;
using UnityEngine;

namespace HumanAPI.LightLevel
{
	public abstract class LightFilter : MonoBehaviour
	{
		protected LightConsume consume;

		public abstract int priority
		{
			get;
		}

		public virtual void Init(LightConsume c)
		{
			consume = c;
			LightConsume lightConsume = consume;
			lightConsume.lightRemoved = (Action<LightBase>)Delegate.Combine(lightConsume.lightRemoved, new Action<LightBase>(OnLightExit));
		}

		public abstract void ApplyFilter(LightHitInfo info);

		protected virtual void OnLightExit(LightBase source)
		{
		}
	}
}
