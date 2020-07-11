using UnityEngine;

namespace HumanAPI.LightLevel
{
	public class LaserBeam : LightBeam
	{
		private Collider targetCollider;

		public LaserEmitter source;

		public Color hitColor = Color.green;

		public Color defaultColor = Color.red;

		public bool resetColorOnExit;

		public override Color color
		{
			get
			{
				return mat.color;
			}
			set
			{
				if (value.r + value.g + value.b == 0f)
				{
					DisableLight();
					return;
				}
				if (value.a == 0f)
				{
					value.a = 1f;
				}
				mat.color = value;
			}
		}

		public override float range
		{
			get
			{
				Vector3 localScale = base.transform.localScale;
				return localScale.z;
			}
			protected set
			{
				Vector3 localScale = base.transform.localScale;
				localScale.z = value;
				base.transform.localScale = localScale;
			}
		}

		public void Callibrate()
		{
			targetCollider = hitCollider;
			color = Color.red;
		}

		private void Update()
		{
			if (!(targetCollider == null))
			{
				bool flag = targetCollider != hitCollider;
				if (flag)
				{
					color = hitColor;
				}
				else if (resetColorOnExit)
				{
					color = defaultColor;
				}
				source.Hit(flag);
			}
		}

		public override void EnableLight()
		{
			base.EnableLight();
			Callibrate();
		}
	}
}
