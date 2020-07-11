using UnityEngine;

namespace HumanAPI.LightLevel
{
	public class LightSource : Node
	{
		public bool enableOnStart = true;

		[SerializeField]
		protected Color lightColor = Color.white;

		[SerializeField]
		private Vector3 direction = Vector3.forward;

		public LightBase emittedLight;

		public bool isOn
		{
			get;
			protected set;
		}

		public virtual Vector3 Direction
		{
			get
			{
				return base.transform.TransformDirection(direction);
			}
			set
			{
				base.transform.forward = value;
			}
		}

		public virtual Color color
		{
			get
			{
				return lightColor;
			}
			set
			{
				lightColor = color;
			}
		}

		protected virtual void Start()
		{
			if (enableOnStart)
			{
				EnableLight();
			}
		}

		private void Update()
		{
			if (!(emittedLight == null) && (emittedLight.transform.position != base.transform.position || emittedLight.Direction == Direction))
			{
				emittedLight.transform.position = base.transform.position;
				emittedLight.Direction = Direction;
				emittedLight.UpdateLight();
			}
		}

		protected virtual LightBase CreateLight()
		{
			return LightPool.Create<LightBeam>(new Ray(base.transform.position, Direction));
		}

		public virtual void EnableLight()
		{
			isOn = true;
			emittedLight = CreateLight();
		}

		public virtual void DisableLight()
		{
			if (isOn)
			{
				isOn = false;
				emittedLight.DisableLight();
			}
		}

		private void OnDrawGizmos()
		{
			Debug.DrawRay(base.transform.position, Direction);
			Gizmos.DrawIcon(base.transform.position, "LightBulb.png", allowScaling: false);
		}
	}
}
